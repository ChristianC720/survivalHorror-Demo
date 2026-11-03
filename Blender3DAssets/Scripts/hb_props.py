"""Hard-surface models by code (BLENDER_PLAN.md sections 2 and 7): The Mother, machines, the revolver, items, confiscated objects.

Every model shares one 2048 px atlas and one material, so all of it batches. Each model paints its own tile from model space:
ink edges, panel seams, wear, plus its own stencils, hazard bands and lamps (the Signalis lesson: low polygons carry graphic
detail, the same way a real machine carries signage). Run build_all(). Blender axes: the front of a model faces -Y (+Z in
Unity), origin at the floor center, metres.
"""
import importlib
import math
import os
import sys

import bmesh
import bpy
import numpy as np
from mathutils import Vector

ROOT = r"C:\Users\Chris\Heartbeat"
sys.path.insert(0, ROOT + r"\Blender3DAssets\Scripts")
import hb_paint as hp

importlib.reload(hp)

OUT = ROOT + r"\Assets\_Project\Models\Props"
SOURCE = ROOT + r"\Blender3DAssets\Props\Props.blend"

# Palette from GDD Direccion de Arte: warm greys, rust, crimson, plus the few item colours the GDD names.
COLORS = {
    "metal": (.29, .25, .24), "steel": (.46, .44, .42), "dark": (.11, .08, .08), "black": (.04, .03, .03),
    "rust": (.30, .17, .11), "crimson": (.48, .11, .17), "white": (.70, .67, .60), "skin": (.55, .45, .38),
    "copper": (.62, .36, .20), "amber": (.72, .47, .10), "paper": (.76, .73, .64), "ceramic": (.20, .20, .22),
    "polymer": (.09, .09, .10), "plush": (.60, .53, .48), "wood": (.32, .21, .14), "cloth": (.26, .27, .29),
    "yellow": (.66, .52, .10), "teal": (.21, .34, .33), "pink": (.62, .36, .38), "brown": (.24, .17, .12),
    "glow_crimson": (.86, .18, .25), "glow_green": (.01, .87, .55), "glow_red": (.95, .10, .08), "glow_amber": (.95, .62, .15),
    "frost": (.62, .68, .72),
}
GLOW = {"glow_crimson", "glow_green", "glow_red", "glow_amber"}
NAMES = list(COLORS) + ["glass"]
ATLAS = 2048
SEAMED = {"metal", "steel", "rust", "yellow", "white", "ceramic", "crimson", "dark"}
INK = np.array((.045, .025, .03), dtype=np.float32)
BARE = np.array((.59, .57, .55), dtype=np.float32)
RED = np.array(COLORS["crimson"], dtype=np.float32)
GREEN = np.array(COLORS["glow_green"], dtype=np.float32)
PAPER = np.array(COLORS["paper"], dtype=np.float32)
AMBER = np.array(COLORS["amber"], dtype=np.float32)
F = (0, -1, 0)
UP = (0, 0, 1)


class Model:
    def __init__(self, name):
        self.name = name
        self.bm = bmesh.new()
        self.tag = self.bm.faces.layers.int.new("col")
        self.marks = []
        self.pitch = .5                # panel seam spacing, metres

    def _face(self, verts, col):
        f = self.bm.faces.new(verts)
        f.smooth = False
        f.material_index = 1 if col == "glass" else 0
        f[self.tag] = NAMES.index(col)
        return f

    # Marks are applied in model space, so they cross faces the way paint does.
    def text(self, center, normal, height, string, col=INK, wear=.0, glow=False):
        self.marks.append(("bitmap", center, normal, height, hp.text_bitmap(string), col, wear, glow))
        return self

    def plate(self, center, normal, size, col=PAPER):
        self.marks.append(("bitmap", center, normal, tuple(size), np.ones((4, 4), dtype=bool), col, 0, False))
        return self

    def code(self, center, normal, height, seed=1, col=INK):
        """Data-matrix label: reads as machine-printed at any distance."""
        bits = np.random.default_rng(seed).random((8, 8)) > .5
        bits[:, 0] = bits[7, :] = True
        self.marks.append(("bitmap", center, normal, (height, height), bits, col, 0, False))
        return self

    def hazard(self, center, normal, size, pitch=.14):
        self.marks.append(("hazard", center, normal, tuple(size), pitch))
        return self

    def lamps(self, center, normal, cols, rows, pitch, seed=1):
        """Indicator field: mostly dark tiles, a few lit in the room's accent."""
        self.marks.append(("lamps", center, normal, cols, rows, pitch, seed))
        return self

    def screen(self, center, normal, size, seed=1, col=GREEN):
        """CRT: bezel, phosphor ground, a few lines of lit data."""
        self.marks.append(("screen", center, normal, tuple(size), seed, col))
        return self

    def box(self, c, size, col, taper=1.0, rot=0.0):
        """c: center of the box; size: width (x), depth (y), height (z). taper scales the top."""
        cx, cy, cz = c
        w, d, h = size[0] / 2, size[1] / 2, size[2] / 2
        cs, sn = math.cos(rot), math.sin(rot)

        def P(x, y, z):
            return Vector((cx + x * cs - y * sn, cy + x * sn + y * cs, cz + z))

        vs = [self.bm.verts.new(P(sx * w * (taper if sz > 0 else 1), sy * d * (taper if sz > 0 else 1), sz * h)) for sz in (-1, 1) for sy in (-1, 1) for sx in (-1, 1)]
        for q in ((0, 2, 3, 1), (4, 5, 7, 6), (0, 1, 5, 4), (2, 6, 7, 3), (0, 4, 6, 2), (1, 3, 7, 5)):
            self._face([vs[i] for i in q], col)
        return self

    def cyl(self, base, h, r, col, sides=10, axis="z", r2=None):
        """Capped cylinder from `base` along +axis; r2 tapers the far end."""
        r2 = r if r2 is None else r2
        b = Vector(base)
        ax = Vector({"x": (1, 0, 0), "y": (0, 1, 0), "z": (0, 0, 1)}[axis])
        u = Vector((0, 0, 1)) if axis != "z" else Vector((1, 0, 0))
        v = ax.cross(u)
        lo, hi = [], []
        for i in range(sides):
            a = 2 * math.pi * i / sides
            d = u * math.cos(a) + v * math.sin(a)
            lo.append(self.bm.verts.new(b + d * r))
            hi.append(self.bm.verts.new(b + ax * h + d * r2))
        for i in range(sides):
            j = (i + 1) % sides
            self._face((lo[i], lo[j], hi[j], hi[i]), col)
        self._face(list(reversed(lo)), col)
        self._face(hi, col)
        return self

    def finish(self, collection):
        bmesh.ops.recalc_face_normals(self.bm, faces=list(self.bm.faces))
        me = bpy.data.meshes.new(self.name)
        self.bm.to_mesh(me)
        self.bm.free()
        ob = bpy.data.objects.new(self.name, me)
        collection.objects.link(ob)
        me.materials.append(mat("M_Palette"))
        me.materials.append(mat("MAT_Glass"))
        me.calc_loop_triangles()
        layer = me.attributes["col"].data
        self.parts = [NAMES[layer[i].value] for i in range(len(me.polygons))]
        return ob, len(me.loop_triangles)

    def paint(self, me, tile, offset):
        B = hp.bake(me, self.parts, tile, offset, ATLAS, margin=1, edges=True, skip=("glass",))
        base = np.array([COLORS.get(n, (0, 0, 0)) for n in B.names], dtype=np.float32)   # glass has its own material
        c = base[B.part].copy()
        e = np.zeros_like(c)
        seam = np.isin(B.part, [i for i, n in enumerate(B.names) if n in SEAMED])
        P, d = B.P, B.density
        extent = float(np.ptp(P[:, 2])) if B.n else 1.0
        E = B.E * d
        if extent > .5:                # a machine, not something that fits in a pocket
            n = hp.fbm(P, 2.4, 2, 5)
            k = 1 - np.clip((n - .45) * 2, 0, 1) * .34
            c[seam] *= k[seam, None]
            # Soil settles low on the machine; rust runs down from what is above.
            low = np.clip((P[:, 2] - P[:, 2].min()) / max(extent * .3, 1e-3), .62, 1)
            c *= low[:, None]
            runs = (hp.noise(P, (34, 34, 1.5), 8) > .84) & seam
            c[runs] *= .78
            # Panel seams and rivets on large faces.
            flat = seam & (B.size > (self.pitch * .6) ** 2)
            p = self.pitch
            gu, gv = np.abs(((B.U + p / 2) % p) - p / 2), np.abs(((B.V + p / 2) % p) - p / 2)
            px = 1 / d
            c[flat & ((gu < px * .75) | (gv < px * .75))] *= .5
            rv = flat & (gu > px * 2) & (gu < px * 3.6) & (gv > px * 2) & (gv < px * 3.6)
            c[rv] = c[rv] * 1.5
        # Chipped paint at the edges, then the ink line every model shares.
        solid = B.size * d * d > 36
        chip = solid & seam & (E < 2.5) & (hp.noise(P, 30, 11) > .78)
        c[chip] = c[chip] * .45 + BARE * .55
        c[solid & (E >= 1) & (E < 2)] *= 1.14
        line = solid & (E < 1)
        c[line] = c[line] * .35 + INK * .65
        for m in self.marks:
            kind = m[0]
            if kind == "bitmap":
                _, ct, n, size, bits, col, wear, glow = m
                hp.stamp(B, c, ct, n, size, col, bits, wear=wear, seed=3)
                if glow:
                    hp.stamp(B, e, ct, n, size, col, bits)
            elif kind == "hazard":
                _, ct, n, size, pitch = m
                hp.stripes(B, c, ct, n, size, np.array(COLORS["amber"]) * .85, INK, pitch=pitch, wear=.6, seed=7)
            elif kind == "lamps":
                self._lamps(B, c, e, *m[1:])
            elif kind == "screen":
                self._screen(B, c, e, *m[1:])
        c = hp.posterize(B, c, levels=12)
        for i, n in enumerate(B.names):
            if n in GLOW:
                c[B.part == i] = base[i]
                e[B.part == i] = base[i]
        return B, np.clip(c, 0, 1), e

    @staticmethod
    def _lamps(B, c, e, center, normal, cols, rows, pitch, seed):
        lit = np.random.default_rng(seed).random((rows, cols))
        hit, s, t, _ = hp.decal(B, center, normal, (cols * pitch, rows * pitch), None)
        if not hit.any():
            return
        fx, fy = s[hit] * cols, t[hit] * rows
        ix, iy = fx.astype(int), fy.astype(int)
        tile = ((fx % 1) > .14) & ((fx % 1) < .86) & ((fy % 1) > .22) & ((fy % 1) < .78)
        idx = np.nonzero(hit)[0]
        c[idx] = INK
        v = lit[np.clip(iy, 0, rows - 1), np.clip(ix, 0, cols - 1)]
        on, warm = tile & (v > .72), tile & (v > .9)
        amber = np.array(COLORS["glow_amber"], dtype=np.float32)
        c[idx[tile]] = np.array((.16, .15, .15), dtype=np.float32)
        c[idx[on]] = GREEN
        e[idx[on]] = GREEN
        c[idx[warm]] = amber
        e[idx[warm]] = amber

    @staticmethod
    def _screen(B, c, e, center, normal, size, seed, col):
        hit, s, t, _ = hp.decal(B, center, normal, size, None)
        if not hit.any():
            return
        idx = np.nonzero(hit)[0]
        ss, tt = s[hit], t[hit]
        inner = (ss > .08) & (ss < .92) & (tt > .1) & (tt < .9)
        c[idx] = INK
        c[idx[inner]] = col * .12
        e[idx[inner]] = col * .05
        rows = 9
        ln = np.random.default_rng(seed).random(rows) * .7 + .1
        ry = (tt - .1) / .8 * rows
        on = inner & ((ry % 1) < .45) & (ss < .1 + ln[np.clip(ry.astype(int), 0, rows - 1)]) & (((ss * 46).astype(int) % 5) != 4)
        c[idx[on]] = col
        e[idx[on]] = col


def mat(name):
    m = bpy.data.materials.get(name)
    if m:
        return m
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    tree = m.node_tree
    b = next(n for n in tree.nodes if n.type == "BSDF_PRINCIPLED")
    if name == "M_Palette":
        n = tree.nodes.new("ShaderNodeTexImage")
        n.image = bpy.data.images["T_Palette"]
        n.interpolation = "Closest"
        tree.links.new(n.outputs["Color"], b.inputs["Base Color"])
    else:
        b.inputs["Base Color"].default_value = (.5, .6, .65, 1)
        b.inputs["Alpha"].default_value = .25
    return m


# ---------- machines ----------

def generator(M):
    M.box((0, 0, .12), (2.0, 1.5, .24), "dark").box((0, 0, 1.05), (1.9, 1.3, 1.62), "metal").box((0, -.66, 1.3), (.9, .04, .7), "dark")
    M.cyl((.55, -.67, 1.25), .03, .1, "steel", axis="y").cyl((.3, -.67, 1.25), .03, .1, "steel", axis="y")
    M.box((-.55, -.67, .9), (.7, .02, .34), "yellow").cyl((0, .0, 1.86), .14, .25, "rust")
    for i in range(6):
        M.box((-.96, -.45 + i * .18, 1.0), (.04, .1, 1.1), "dark")
    M.box((0, -.68, 1.62), (.12, .02, .06), "glow_amber")
    M.cyl((-.7, .3, 1.86), .5, .06, "steel").cyl((-.4, .3, 1.86), .35, .06, "steel").box((-.55, .3, 2.34), (.5, .1, .1), "dark")
    M.cyl((.95, -.3, .5), 1.1, .03, "black", sides=6)
    M.plate((-.55, -.69, .9), F, (.66, .32), AMBER).text((-.55, -.7, .97), F, .09, "GEFAHR").text((-.55, -.7, .83), F, .09, "VORSICHT")
    M.text((0, -.66, 1.72), F, .12, "AGGREGAT 3", PAPER * .55, wear=.5).hazard((0, -.76, .13), F, (2.0, .2))
    M.lamps((0, -.685, 1.3), F, 8, 5, .09, 3).code((.75, -.66, .55), F, .14, 4, PAPER)


def boiler(M):
    M.cyl((0, 0, .1), 2.5, .72, "rust", sides=14).cyl((0, 0, 2.6), .25, .72, "metal", sides=14, r2=.3).cyl((0, 0, 2.85), .15, .18, "metal", sides=8)
    M.box((0, 0, .05), (1.5, 1.5, .1), "dark")
    for z in (.6, 1.3, 2.0):
        M.cyl((0, 0, z), .06, .74, "dark", sides=14)
    M.cyl((0, -.7, 1.0), .05, .3, "steel", axis="y", sides=10).cyl((.5, -.55, 1.8), .04, .1, "steel", axis="y", sides=8)
    M.box((.5, -.6, 1.8), (.06, .01, .06), "glow_amber")
    M.cyl((.72, 0, 2.0), .9, .06, "steel", axis="x", sides=6).cyl((1.55, 0, 0), 2.0, .07, "metal", sides=6)
    M.text((0, -.72, 1.55), F, .1, "KESSEL 2", PAPER * .8, wear=.45).text((0, -.72, .9), F, .09, "HEISS", AMBER, wear=.3)
    M.hazard((0, -.76, .05), F, (1.5, .09))


def forklift(M):
    M.box((0, .3, .5), (1.1, 1.5, .6), "yellow").box((0, .55, 1.0), (.9, .6, .4), "yellow")
    for x in (-.5, .5):
        M.box((x, -.15, 1.55), (.06, .06, 1.3), "dark").box((x, .95, 1.55), (.06, .06, 1.3), "dark")
        M.cyl((x * 1.1, -.2, .28), .2, .28, "black", axis="x", sides=10).cyl((x * 1.1, .85, .24), .2, .24, "black", axis="x", sides=10)
    M.box((0, .4, 2.2), (1.1, 1.3, .06), "dark").box((0, -.72, 1.2), (.12, .1, 2.2), "metal")
    for x in (-.3, .3):
        M.box((x, -1.1, .1), (.12, .8, .05), "steel")
        M.box((x, -.75, 1.1), (.12, .06, 2.0), "metal")
    M.box((0, .25, .85), (.5, .5, .1), "black")
    M.text((.0, -.46, .55), F, .07, "VK-14", INK, wear=.4)
    M.hazard((0, .4, 2.23), UP, (1.1, 1.3), pitch=.2).hazard((0, -.78, .3), F, (.12, .6), pitch=.1)


def gantry(M):
    M.box((0, 0, .3), (6, .35, .35), "yellow").box((0, 0, .54), (6, .5, .08), "dark").box((0, 0, .06), (6, .5, .08), "dark")
    M.box((.6, 0, -.1), (.6, .5, .3), "metal").cyl((.6, 0, -1.2), .95, .02, "black", sides=4).box((.6, 0, -1.25), (.1, .1, .12), "rust")
    for x in (-2.9, 2.9):
        M.box((x, 0, .3), (.2, .7, .7), "metal")
    M.hazard((0, -.18, .3), F, (6, .35), pitch=.28).text((-1.6, -.18, .3), F, .16, "5 T", INK)


def pump(M):
    M.box((0, 0, .1), (1.0, 1.5, .2), "dark").cyl((0, -.65, .55), 1.0, .24, "rust", axis="y", sides=10)
    M.cyl((0, -.05, .9), .3, .1, "metal").cyl((0, -.05, 1.2), .05, .16, "steel")
    M.cyl((-.5, .15, .55), .4, .07, "copper", axis="x", sides=8).cyl((.1, .15, .55), .4, .07, "copper", axis="x", sides=8)
    M.cyl((.5, .15, .55), .35, .07, "copper", axis="z", sides=8).cyl((.0, -.72, .55), .1, .3, "dark", axis="y", sides=10)
    M.text((0, -.25, .78), F, .05, "PUMPE B", PAPER * .9, wear=.3).hazard((0, -.76, .1), F, (1.0, .2))


def capsule_base(M, r, h, lid="metal", body=None):
    M.cyl((0, 0, 0), .16, r, "dark", sides=12).cyl((0, 0, h - .1), .1, r, lid, sides=12)
    M.cyl((0, 0, .16), h - .26, r * .95, "glass", sides=12)
    for a in (0, 2.09, 4.19):
        M.box((r * .97 * math.cos(a), r * .97 * math.sin(a), h / 2), (.06, .06, h - .2), "metal")
    if body:
        M.box((0, 0, .95), (body * .7, body * .4, 1.3), "skin", taper=.75).cyl((0, 0, 1.6), .24, .1, "skin", sides=8)


def human_capsule(M):
    capsule_base(M, .5, 2.2, body=.5)
    M.box((.3, -.45, .95), (.14, .02, .08), "glow_green")
    M.text((0, -.5, .08), F, .09, "P-07", PAPER * .9, wear=.2).cyl((.0, .5, 1.1), 1.0, .03, "black", sides=6)
    M.screen((.3, -.46, 1.1), F, (.2, .14), 2)


def reconstruction(M):
    capsule_base(M, .5, 2.2)
    M.box((0, -.46, .08), (.5, .1, .14), "crimson")
    M.box((.3, -.45, 1.0), (.14, .02, .08), "glow_green")
    M.cyl((0, 0, 2.1), .1, .22, "steel", sides=8)
    M.text((0, -.52, .08), F, .09, "R-H", PAPER, wear=.1).cyl((.48, .3, 1.5), .7, .03, "black", sides=6).cyl((-.48, .3, 1.5), .7, .03, "black", sides=6)
    M.screen((.3, -.46, 1.14), F, (.2, .14), 5).hazard((0, -.51, .015), F, (1.0, .03), pitch=.08)


def monster_capsule(M):
    capsule_base(M, .7, 2.6, lid="rust", body=.8)
    M.cyl((.45, 0, 1.0), .7, .05, "dark", sides=6)
    M.text((0, -.7, .08), F, .09, "S-12", AMBER, wear=.3).hazard((0, -.71, .015), F, (1.4, .03), pitch=.1)


def resonance(M):
    M.box((0, 0, .1), (1.2, 2.6, .2), "dark")
    M.cyl((0, -1.2, .75), 2.4, .55, "glass", axis="y", sides=12)
    M.cyl((0, -1.25, .75), .1, .6, "metal", axis="y", sides=12).cyl((0, 1.15, .75), .1, .6, "metal", axis="y", sides=12)
    M.box((0, 0, 1.36), (.4, 2.2, .06), "crimson")
    M.text((0, -.62, .12), F, .055, "RESONANZ", PAPER, wear=.2).hazard((0, -1.31, .1), F, (1.2, .2)).text((0, 0, 1.4), UP, .05, "R-07", PAPER)


def cryo_tank(M):
    M.cyl((0, 0, 0), .2, .85, "dark", sides=14).cyl((0, 0, .2), 2.5, .8, "steel", sides=14).cyl((0, 0, 2.7), .3, .8, "metal", sides=14, r2=.3)
    M.cyl((0, 0, 2.15), .12, .84, "crimson", sides=14).cyl((0, 0, 1.2), .6, .82, "glass", sides=14)
    M.cyl((0, 0, .9), .3, .84, "frost", sides=14)
    M.box((.55, -.55, 1.0), (.14, .02, .08), "glow_crimson", rot=.78)
    M.text((0, -.84, 2.21), F, .06, "KRYO 12", PAPER).text((0, -.84, .34), F, .09, "KALT", AMBER, wear=.2)
    M.cyl((.8, .0, 2.4), .3, .05, "dark", axis="x", sides=6).cyl((1.1, .0, 0), 2.4, .05, "dark", sides=6)


def transformer(M):
    M.box((0, 0, .1), (1.5, 1.5, .2), "dark").box((0, 0, 1.1), (1.3, 1.3, 1.8), "metal")
    for i in range(7):
        y = -.54 + i * .18
        M.box((-.7, y, 1.1), (.12, .05, 1.6), "dark").box((.7, y, 1.1), (.12, .05, 1.6), "dark")
    for x in (-.35, .35):
        M.cyl((x, 0, 2.0), .1, .1, "steel", sides=8).cyl((x, 0, 2.1), .1, .07, "glow_crimson", sides=8)
    M.box((0, -.66, 1.5), (.66, .02, .38), "yellow")
    M.plate((0, -.66, 1.5), F, (.62, .34), AMBER).text((0, -.67, 1.57), F, .09, "GEFAHR").text((0, -.67, 1.43), F, .09, "TRAFO 1")
    M.hazard((0, -.76, .1), F, (1.5, .2)).code((0, -.655, .9), F, .16, 9, PAPER)


def broken_capsule(M):
    M.cyl((0, 0, 0), .16, .6, "dark", sides=12)
    for i, a in enumerate((.3, 1.3, 2.3, 3.6, 4.5, 5.6)):
        h = .4 + (i * 37 % 9) * .13
        M.box((.56 * math.cos(a), .56 * math.sin(a), .16 + h / 2), (.3, .05, h), "glass", rot=a + 1.57)
    M.cyl((0, 0, 2.1), .1, .6, "metal", sides=12)
    for a in (0, 3.14):
        M.box((.6 * math.cos(a), .6 * math.sin(a), 1.1), (.06, .06, 2.0), "metal")
    M.box((.4, -.75, .03), (.3, .25, .03), "glass", rot=.4)


# ---------- The Mother (D-129: a cathedral shelf of frosted vials with crimson light, machine only) ----------

def mother(M):
    w, h, d = 8.0, 5.2, 2.0
    M.box((0, 0, h - .1), (w + 1, d + .3, .2), "dark")
    for x in (-4, -2.67, -1.33, 0, 1.33, 2.67, 4):
        M.box((x, 0, h / 2), (.14, d, h), "metal")
    for z in (.1, 1.1, 2.1, 3.1, 4.1):
        M.box((0, 0, z), (w, d, .12), "steel")
    M.box((0, .9, h / 2), (w, .1, h), "dark")
    # Vials: rows of frosted cylinders. Crimson light sits at the back of each bay.
    for row in range(4):
        z = .16 + row
        for bay in range(6):
            cx = -3.33 + bay * 1.33
            M.box((cx, .8, z + .4), (1.0, .05, .6), "glow_crimson")
            for k in range(3):
                x = cx - .36 + k * .36
                M.cyl((x, -.2, z), .72, .14, "frost", sides=6).cyl((x, -.2, z + .72), .08, .08, "steel", sides=6)
    for x in (-3, 3):
        M.box((x, 0, h + .5), (.3, .3, 1.0), "yellow")
    M.box((0, 0, h + 1.0), (w + 2, .5, .4), "yellow")
    M.pitch = 1.33
    for row in range(4):
        for bay in range(6):
            M.text((-3.33 + bay * 1.33, -1.0, .1 + row), F, .07, f"{'ABCDEF'[bay]}{row + 1}", PAPER, wear=.2)
    M.text((0, -1.0, 4.1), F, .09, "VITALIS KONZERN", RED, wear=.1)
    M.hazard((0, -.25, 6.2), F, (10, .4), pitch=.3).hazard((-3, -.15, 5.7), F, (.3, 1.0), pitch=.2).hazard((3, -.15, 5.7), F, (.3, 1.0), pitch=.2)


def collector_arm(M):
    # Hangs from the origin, downward; the claw is a separate child model that opens.
    M.box((0, 0, 3.0), (.8, .8, .4), "yellow").cyl((0, 0, .75), 2.1, .16, "steel", sides=8).cyl((0, 0, 0), .85, .22, "metal", sides=8)
    M.box((0, 0, .1), (.5, .5, .24), "dark").cyl((.22, 0, .9), 1.2, .05, "black", sides=6)
    M.hazard((0, -.4, 3.0), F, (.8, .4), pitch=.14).text((0, -.22, .45), F, .06, "M-01", PAPER * .9, wear=.3)


def claw(M):
    M.box((0, 0, -.1), (.5, .5, .2), "metal").cyl((0, 0, -.28), .1, .14, "dark", sides=8)
    for a in (0, 2.09, 4.19):
        c, s = math.cos(a), math.sin(a)
        M.box((.26 * c, .26 * s, -.4), (.12, .12, .55), "steel", rot=a)
        M.box((.3 * c, .3 * s, -.72), (.1, .1, .3), "rust", taper=.3, rot=a)


def hostile_cam(M):
    M.box((0, .05, 0), (.34, .5, .3), "dark").cyl((0, -.2, 0), .22, .11, "metal", axis="y", sides=10)
    M.cyl((0, -.27, 0), .08, .09, "glow_red", axis="y", sides=8).box((0, .15, .26), (.08, .08, .3), "metal")
    M.text((0, -.2, .12), F, .03, "K-07", PAPER)


def mixing_table(M):
    M.box((0, 0, .45), (1.6, .8, .9), "metal").box((0, -.05, 1.0), (1.7, .9, .08), "dark", taper=.95)
    for i in range(10):
        x = -.65 + i * .145
        M.box((x, -.15, 1.06), (.06, .4, .03), "steel").box((x, -.1 - (i * 7 % 5) * .05, 1.09), (.04, .05, .03), "black")
    M.box((0, .25, 1.2), (1.5, .08, .36), "black").box((-.3, .2, 1.22), (.4, .02, .2), "glow_green").box((.35, .2, 1.22), (.3, .02, .2), "glow_crimson")
    M.text((0, -.4, .75), F, .05, "MISCHPULT 1", PAPER * .9, wear=.25).code((.55, -.4, .4), F, .14, 2, PAPER)
    M.lamps((-.45, -.36, 1.04), UP, 6, 2, .04, 8)
    M.screen((-.3, .19, 1.22), F, (.4, .2), 3).screen((.35, .19, 1.22), F, (.3, .2), 6, np.array(COLORS["glow_crimson"], dtype=np.float32))


# ---------- revolver and items ----------

def revolver(M):
    # Origin at the grip's top; barrel along -Y (+Z in Unity), matching WeaponSocket.
    M.box((0, -.04, .0), (.032, .13, .05), "ceramic").cyl((0, -.03, -.005), .05, .028, "polymer", axis="y", sides=8)
    M.box((0, -.18, .012), (.024, .17, .03), "ceramic").cyl((0, -.265, .0), .03, .011, "black", axis="y", sides=6)
    M.box((0, -.135, -.018), (.022, .1, .012), "polymer").box((0, .02, -.055), (.03, .045, .1), "polymer", rot=0)
    M.box((0, .012, -.11), (.028, .05, .02), "ceramic").box((0, -.03, .033), (.012, .03, .008), "black")
    M.box((.017, -.05, .018), (.001, .03, .014), "glow_green")
    for sx in (-1, 1):
        M.text((sx * .012, -.18, .012), (sx, 0, 0), .005, "KS-M5", PAPER * .9)
    M.box((-.017, -.05, .018), (.001, .03, .014), "glow_green")


def heal(M):
    M.box((0, 0, .075), (.3, .09, .15), "white").box((0, -.046, .075), (.31, .004, .02), "crimson")
    M.box((0, 0, .165), (.13, .02, .03), "dark").box((-.055, 0, .155), (.02, .02, .02), "dark").box((.055, 0, .155), (.02, .02, .02), "dark")
    M.box((.1, -.046, .1), (.05, .004, .05), "crimson")
    M.marks.append(("bitmap", (-.08, -.046, .09), F, (.06, .06), np.array([[0, 1, 0], [1, 1, 1], [0, 1, 0]], dtype=bool), RED, 0, False))
    M.text((.0, -.046, .135), F, .012, "VK-MED", INK).code((.0, .046, .08), (0, 1, 0), .05, 3)


def ammo(M):
    M.box((0, 0, .004), (.13, .09, .006), "steel")
    for x in (-.045, -.015, .015, .045):
        M.cyl((x, 0, .007), .018, .012, "copper", sides=6)
    M.box((0, 0, .018), (.125, .05, .028), "glass")


def spike(M):
    M.cyl((0, -.2, .03), .3, .02, "black", axis="y", sides=8).cyl((0, .1, .03), .1, .024, "dark", axis="y", sides=8)
    M.cyl((0, -.25, .03), .05, .004, "steel", axis="y", sides=6, r2=.02).cyl((0, .0, .03), .03, .021, "amber", axis="y", sides=8)
    M.box((.02, .05, .03), (.003, .012, .012), "glow_green")
    M.hazard((0, .1, .054), UP, (.05, .1), pitch=.018).text((0, -.05, .05), UP, .009, "800V", AMBER)


def key_mandatory(M):
    M.box((0, 0, .01), (.16, .1, .012), "white").box((0, .03, .017), (.16, .03, .002), "crimson")
    for i in range(3):
        M.box((-.03 + i * .018, -.02 + i * .0, .017), (.016, .018 + i * .012, .002), "dark")
    M.text((.03, .03, .018), UP, .012, "R-H", PAPER).code((.055, -.02, .016), UP, .028, 7)


def key_optional(M):
    M.box((0, 0, .008), (.09, .055, .004), "white").box((0, .016, .011), (.09, .014, .002), "metal")
    M.text((0, -.01, .0105), UP, .007, "K-07", INK).code((.03, -.012, .0105), UP, .018, 5)


def ring(M):
    n = 12
    for i in range(n):
        a = 2 * math.pi * i / n
        M.box((.011 * math.cos(a), 0, .012 + .011 * math.sin(a)), (.006, .004, .006), "steel", rot=0)


def document(M):
    M.box((0, 0, .004), (.21, .29, .006), "paper").box((0, .1, .008), (.14, .012, .001), "dark")
    for i in range(5):
        M.box((-.01, .06 - i * .03, .008), (.16 - i * .012, .008, .001), "brown")
    M.text((0, .1, .009), UP, .009, "VITALIS", PAPER)


# ---------- confiscated objects (GDD Objetos y Coleccionables): everyday things with an inventory tag ----------

def tag(M, x=0.0, y=.0, z=.02):
    """Inventory tag: the object is a file now (GDD Objetos y Coleccionables)."""
    M.box((x, y, z), (.05, .03, .002), "paper")
    M.text((x, y + .004, z + .0012), UP, .0045, "KONF", INK).text((x, y - .006, z + .0012), UP, .0035, "0417", RED)


def watch(M):
    M.cyl((0, 0, .004), .01, .022, "steel", sides=8).box((0, .045, .004), (.018, .05, .004), "brown").box((0, -.045, .004), (.018, .05, .004), "brown")
    tag(M, .05, .0, .006)


def thermos(M):
    M.cyl((0, 0, 0), .2, .04, "steel", sides=8).cyl((0, 0, .2), .05, .03, "dark", sides=8).box((0, -.04, .1), (.04, .002, .03), "black")
    tag(M, .06, 0, .006)


def glasses(M):
    M.cyl((-.032, 0, .02), .004, .024, "black", axis="y", sides=8).cyl((.032, 0, .02), .004, .024, "glass", axis="y", sides=8)
    M.box((0, 0, .026), (.016, .004, .004), "black").box((-.06, .05, .024), (.004, .1, .004), "black").box((.06, .05, .024), (.004, .1, .004), "black")
    tag(M, 0, -.05, .004)


def lunchbox(M):
    M.box((0, 0, .05), (.2, .12, .1), "teal").box((0, 0, .105), (.205, .125, .012), "metal").box((0, 0, .12), (.07, .012, .02), "dark")
    M.box((.04, -.062, .05), (.07, .002, .05), "paper")
    tag(M, -.1, -.06, .01)


def rabbit(M):
    M.box((0, 0, .06), (.09, .07, .12), "plush", taper=.8).box((0, 0, .15), (.08, .07, .07), "plush")
    M.box((-.022, .0, .22), (.022, .018, .1), "plush", taper=.6).box((.022, 0, .22), (.022, .018, .1), "plush", taper=.6)
    M.box((-.055, -.02, .05), (.03, .03, .05), "plush").box((.055, -.02, .05), (.03, .03, .05), "plush")
    M.box((-.02, -.06, .02), (.03, .05, .03), "plush").box((.02, -.06, .02), (.03, .05, .03), "plush")
    M.box((.055, -.02, .08), (.036, .036, .008), "white").box((-.016, -.036, .16), (.008, .002, .008), "black").box((.016, -.036, .16), (.008, .002, .008), "black")


def wallet(M):
    M.box((0, 0, .008), (.11, .085, .014), "brown").box((.01, 0, .016), (.07, .05, .002), "white")
    tag(M, .07, 0, .004)


def rosary(M):
    for i in range(14):
        a = 2 * math.pi * i / 14
        M.box((.05 * math.cos(a), .04 * math.sin(a), .004), (.008, .008, .008), "white")
    M.box((0, -.07, .004), (.006, .03, .004), "white").box((0, -.075, .004), (.02, .006, .004), "white")
    tag(M, .08, 0, .004)


def drawing(M):
    M.box((0, 0, .003), (.21, .29, .003), "paper")
    M.box((-.02, -.04, .0055), (.1, .08, .001), "brown").box((-.02, .02, .0055), (.12, .05, .001), "crimson", taper=.2)
    M.cyl((.07, .1, .005), .001, .025, "yellow", sides=8)


def earplugs(M):
    M.cyl((-.015, 0, .006), .02, .006, "pink", axis="y", sides=6).cyl((.015, .005, .006), .02, .006, "crimson", axis="y", sides=6)
    tag(M, 0, -.03, .004)


def badge(M):
    M.box((0, 0, .004), (.06, .07, .005), "ceramic", taper=.9).box((0, 0, .008), (.04, .04, .002), "steel")
    tag(M, .05, 0, .004)


def cassette(M):
    M.box((0, 0, .007), (.1, .065, .012), "black").box((0, .006, .014), (.08, .035, .001), "paper")
    for x in (-.022, .022):
        M.cyl((x, -.01, .014), .001, .009, "white", sides=6)
    tag(M, .065, 0, .004)


def headphones(M):
    M.cyl((-.08, 0, .03), .03, .045, "black", axis="x", sides=8).cyl((.05, 0, .03), .03, .045, "black", axis="x", sides=8, r2=.02)
    for i in range(7):
        a = math.pi * i / 6
        M.box((-.08 * math.cos(a), 0, .07 + .07 * math.sin(a)), (.04, .02, .012), "dark" if i % 3 else "rust")


SETS = {
    "Machines": {"CH_F1_Generator": generator, "CH_F1_Boiler": boiler, "CH_F1_Forklift": forklift, "CH_F1_GantryCrane": gantry, "CH_F1_WaterPump": pump, "CH_Capsule_Reconstruction": reconstruction, "CH_F2_HumanCapsule": human_capsule, "CH_F2_MonsterCapsule": monster_capsule, "CH_F2_ResonanceCapsule": resonance, "CH_F3_CryoTank": cryo_tank, "CH_F3_Transformer": transformer, "CH_F3_BrokenCapsule": broken_capsule},
    "Boss": {"CH_TheMother": mother, "CH_TheMother_Arm": collector_arm, "CH_TheMother_Claw": claw, "CH_TheMother_Camera": hostile_cam, "CH_F3_MixingTable": mixing_table},
    "Items": {"CH_Revolver": revolver, "CH_Item_Heal": heal, "CH_Item_Ammo": ammo, "CH_Item_Spike": spike, "CH_Item_KeyMandatory": key_mandatory, "CH_Item_KeyOptional": key_optional, "CH_Item_Ring": ring, "CH_Item_Document": document},
    "Confiscated": {"CH_Conf_Watch": watch, "CH_Conf_Thermos": thermos, "CH_Conf_Glasses": glasses, "CH_Conf_Lunchbox": lunchbox, "CH_Conf_PlushRabbit": rabbit, "CH_Conf_Wallet": wallet, "CH_Conf_Rosary": rosary, "CH_Conf_ChildDrawing": drawing, "CH_Conf_Earplugs": earplugs, "CH_Conf_BadgeKS": badge, "CH_Conf_Cassette": cassette, "CH_Conf_Headphones": headphones},
}


TILE = {"Machines": 384, "Boss": 192, "Items": 192, "Confiscated": 128}
TILE_FOR = {"CH_TheMother": 768, "CH_F3_MixingTable": 384, "CH_Revolver": 256}


def build_all():
    bpy.ops.wm.read_homefile(use_empty=True)
    os.makedirs(OUT + r"\Textures", exist_ok=True)
    albedo = np.zeros((ATLAS, ATLAS, 4), dtype=np.float32)
    emission = np.zeros((ATLAS, ATLAS, 4), dtype=np.float32)
    albedo[:, :, 3] = emission[:, :, 3] = 1
    hp.save(OUT + r"\Textures\T_Palette.png", "T_Palette", albedo)
    report = {}
    win = bpy.context.window_manager.windows[0]
    x = y = row = 0
    jobs = [(g, n, f, TILE_FOR.get(n, TILE[g])) for g, ms in SETS.items() for n, f in ms.items()]
    jobs.sort(key=lambda j: -j[3])
    cols = {}
    for group, name, fn, tile in jobs:
        col = cols.get(group)
        if col is None:
            col = cols[group] = bpy.data.collections.new(group)
            bpy.context.scene.collection.children.link(col)
        if x + tile > ATLAS:
            x, y, row = 0, y + row, 0
        assert y + tile <= ATLAS, name
        M = Model(name)
        fn(M)
        ob, tris = M.finish(col)
        B, c, e = M.paint(ob.data, tile, (x, y))
        albedo[B.py, B.px, :3] = c
        emission[B.py, B.px, :3] = e
        report[name] = tris
        x += tile
        row = max(row, tile)
        vl = bpy.context.view_layer
        for o in vl.objects:
            o.select_set(False)
        ob.select_set(True)
        vl.objects.active = ob
        with bpy.context.temp_override(window=win):
            bpy.ops.export_scene.fbx(filepath=OUT + rf"\{name}.fbx", use_selection=True, object_types={"MESH"}, apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", bake_space_transform=True, bake_anim=False, mesh_smooth_type="FACE")
    hp.save(OUT + r"\Textures\T_Palette.png", "T_Palette", albedo)
    hp.save(OUT + r"\Textures\T_Palette_Emission.png", "T_Palette_Emission", emission)
    os.makedirs(os.path.dirname(SOURCE), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=SOURCE)
    return report
