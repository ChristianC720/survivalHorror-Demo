"""Hard-surface models by code (BLENDER_PLAN.md section 2): The Mother, machines, the revolver, items, confiscated objects.

Every model shares one 256 px palette and one material, so all of it batches. Run build_all(). Blender axes: the front of a
model faces -Y (+Z in Unity), origin at the floor center, metres.
"""
import math
import os
import random

import bmesh
import bpy
import numpy as np
from mathutils import Vector

ROOT = r"C:\Users\Chris\Heartbeat"
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
NAMES = list(COLORS)
CELL = 8       # each colour takes one row of 3 shades; 256 / 8 leaves room for 32 colours
TEX = 256
assert len(NAMES) <= TEX // CELL


def palette_uv(name, shade):
    row = NAMES.index(name)
    return ((shade + .5) * CELL / TEX, (row + .5) * CELL / TEX)


def write_palette():
    os.makedirs(OUT + r"\Textures", exist_ok=True)
    a = np.zeros((TEX, TEX, 4), dtype=np.float32)
    e = np.zeros((TEX, TEX, 4), dtype=np.float32)
    a[:, :, 3] = 1
    e[:, :, 3] = 1
    for r, n in enumerate(NAMES):
        for s, k in enumerate((.82, 1.0, 1.14)):
            a[r * CELL:(r + 1) * CELL, s * CELL:(s + 1) * CELL, :3] = [min(1, c * k) for c in COLORS[n]]
            if n in GLOW:
                e[r * CELL:(r + 1) * CELL, s * CELL:(s + 1) * CELL, :3] = COLORS[n]
    for arr, nm in ((a, "T_Palette"), (e, "T_Palette_Emission")):
        im = bpy.data.images.get(nm) or bpy.data.images.new(nm, TEX, TEX)
        im.pixels.foreach_set(arr.ravel())
        im.filepath_raw = OUT + rf"\Textures\{nm}.png"
        im.file_format = "PNG"
        im.save()


class Model:
    def __init__(self, name):
        self.name = name
        self.bm = bmesh.new()
        self.uv = self.bm.loops.layers.uv.new("UVMap")
        self.glass = []
        self.rng = random.Random(hash(name) & 0xffff)

    def _face(self, verts, col):
        f = self.bm.faces.new(verts)
        f.smooth = False
        if col == "glass":
            f.material_index = 1
            uv = palette_uv("frost", 1)
            for l in f.loops:
                l[self.uv].uv = uv
            return f
        f.material_index = 0
        uv = palette_uv(col, self.rng.randint(0, 2))
        for l in f.loops:
            l[self.uv].uv = uv
        return f

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
        return ob, len(me.loop_triangles)


def mat(name):
    m = bpy.data.materials.get(name)
    if m:
        return m
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    tree = m.node_tree
    b = next(n for n in tree.nodes if n.type == "BSDF_PRINCIPLED")
    if name == "M_Palette":
        t = bpy.data.images["T_Palette"]
        n = tree.nodes.new("ShaderNodeTexImage")
        n.image = t
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
    M.box((-.55, -.67, .9), (.5, .02, .3), "yellow").cyl((0, .0, 1.86), .14, .25, "rust")
    for i in range(6):
        M.box((-.96, -.45 + i * .18, 1.0), (.04, .1, 1.1), "dark")
    M.box((0, -.68, 1.62), (.12, .02, .06), "glow_amber")


def boiler(M):
    M.cyl((0, 0, .1), 2.5, .72, "rust", sides=14).cyl((0, 0, 2.6), .25, .72, "metal", sides=14, r2=.3).cyl((0, 0, 2.85), .15, .18, "metal", sides=8)
    M.box((0, 0, .05), (1.5, 1.5, .1), "dark")
    for z in (.6, 1.3, 2.0):
        M.cyl((0, 0, z), .06, .74, "dark", sides=14)
    M.cyl((0, -.7, 1.0), .05, .3, "steel", axis="y", sides=10).cyl((.5, -.55, 1.8), .04, .1, "steel", axis="y", sides=8)
    M.box((.5, -.6, 1.8), (.06, .01, .06), "glow_amber")


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


def gantry(M):
    M.box((0, 0, .3), (6, .35, .35), "yellow").box((0, 0, .54), (6, .5, .08), "dark").box((0, 0, .06), (6, .5, .08), "dark")
    M.box((.6, 0, -.1), (.6, .5, .3), "metal").cyl((.6, 0, -1.2), .95, .02, "black", sides=4).box((.6, 0, -1.25), (.1, .1, .12), "rust")
    for x in (-2.9, 2.9):
        M.box((x, 0, .3), (.2, .7, .7), "metal")


def pump(M):
    M.box((0, 0, .1), (1.0, 1.5, .2), "dark").cyl((0, -.65, .55), 1.0, .24, "rust", axis="y", sides=10)
    M.cyl((0, -.05, .9), .3, .1, "metal").cyl((0, -.05, 1.2), .05, .16, "steel")
    M.cyl((-.5, .15, .55), .4, .07, "copper", axis="x", sides=8).cyl((.1, .15, .55), .4, .07, "copper", axis="x", sides=8)


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


def reconstruction(M):
    capsule_base(M, .5, 2.2)
    M.box((0, -.46, .08), (.5, .1, .14), "crimson")
    M.box((.3, -.45, 1.0), (.14, .02, .08), "glow_green")
    M.cyl((0, 0, 2.1), .1, .22, "steel", sides=8)


def monster_capsule(M):
    capsule_base(M, .7, 2.6, lid="rust", body=.8)
    M.cyl((.45, 0, 1.0), .7, .05, "dark", sides=6)


def resonance(M):
    M.box((0, 0, .1), (1.2, 2.6, .2), "dark")
    M.cyl((0, -1.2, .75), 2.4, .55, "glass", axis="y", sides=12)
    M.cyl((0, -1.25, .75), .1, .6, "metal", axis="y", sides=12).cyl((0, 1.15, .75), .1, .6, "metal", axis="y", sides=12)
    M.box((0, 0, 1.36), (.4, 2.2, .06), "crimson")


def cryo_tank(M):
    M.cyl((0, 0, 0), .2, .85, "dark", sides=14).cyl((0, 0, .2), 2.5, .8, "steel", sides=14).cyl((0, 0, 2.7), .3, .8, "metal", sides=14, r2=.3)
    M.cyl((0, 0, 2.15), .12, .84, "crimson", sides=14).cyl((0, 0, 1.2), .6, .82, "glass", sides=14)
    M.cyl((0, 0, .9), .3, .84, "frost", sides=14)
    M.box((.55, -.55, 1.0), (.14, .02, .08), "glow_crimson", rot=.78)


def transformer(M):
    M.box((0, 0, .1), (1.5, 1.5, .2), "dark").box((0, 0, 1.1), (1.3, 1.3, 1.8), "metal")
    for i in range(7):
        y = -.54 + i * .18
        M.box((-.7, y, 1.1), (.12, .05, 1.6), "dark").box((.7, y, 1.1), (.12, .05, 1.6), "dark")
    for x in (-.35, .35):
        M.cyl((x, 0, 2.0), .1, .1, "steel", sides=8).cyl((x, 0, 2.1), .1, .07, "glow_crimson", sides=8)
    M.box((0, -.66, 1.5), (.5, .02, .36), "yellow")


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


def collector_arm(M):
    # Hangs from the origin, downward; the claw is a separate child model that opens.
    M.box((0, 0, 3.0), (.8, .8, .4), "yellow").cyl((0, 0, .75), 2.1, .16, "steel", sides=8).cyl((0, 0, 0), .85, .22, "metal", sides=8)
    M.box((0, 0, .1), (.5, .5, .24), "dark").cyl((.22, 0, .9), 1.2, .05, "black", sides=6)


def claw(M):
    M.box((0, 0, -.1), (.5, .5, .2), "metal").cyl((0, 0, -.28), .1, .14, "dark", sides=8)
    for a in (0, 2.09, 4.19):
        c, s = math.cos(a), math.sin(a)
        M.box((.26 * c, .26 * s, -.4), (.12, .12, .55), "steel", rot=a)
        M.box((.3 * c, .3 * s, -.72), (.1, .1, .3), "rust", taper=.3, rot=a)


def hostile_cam(M):
    M.box((0, .05, 0), (.34, .5, .3), "dark").cyl((0, -.2, 0), .22, .11, "metal", axis="y", sides=10)
    M.cyl((0, -.27, 0), .08, .09, "glow_red", axis="y", sides=8).box((0, .15, .26), (.08, .08, .3), "metal")


def mixing_table(M):
    M.box((0, 0, .45), (1.6, .8, .9), "metal").box((0, -.05, 1.0), (1.7, .9, .08), "dark", taper=.95)
    for i in range(10):
        x = -.65 + i * .145
        M.box((x, -.15, 1.06), (.06, .4, .03), "steel").box((x, -.1 - (i * 7 % 5) * .05, 1.09), (.04, .05, .03), "black")
    M.box((0, .25, 1.2), (1.5, .08, .36), "black").box((-.3, .2, 1.22), (.4, .02, .2), "glow_green").box((.35, .2, 1.22), (.3, .02, .2), "glow_crimson")


# ---------- revolver and items ----------

def revolver(M):
    # Origin at the grip's top; barrel along -Y (+Z in Unity), matching WeaponSocket.
    M.box((0, -.04, .0), (.032, .13, .05), "ceramic").cyl((0, -.03, -.005), .05, .028, "polymer", axis="y", sides=8)
    M.box((0, -.18, .012), (.024, .17, .03), "ceramic").cyl((0, -.265, .0), .03, .011, "black", axis="y", sides=6)
    M.box((0, -.135, -.018), (.022, .1, .012), "polymer").box((0, .02, -.055), (.03, .045, .1), "polymer", rot=0)
    M.box((0, .012, -.11), (.028, .05, .02), "ceramic").box((0, -.03, .033), (.012, .03, .008), "black")
    M.box((.017, -.05, .018), (.001, .03, .014), "glow_green")


def heal(M):
    M.box((0, 0, .075), (.3, .09, .15), "white").box((0, -.046, .075), (.31, .004, .02), "crimson")
    M.box((0, 0, .165), (.13, .02, .03), "dark").box((-.055, 0, .155), (.02, .02, .02), "dark").box((.055, 0, .155), (.02, .02, .02), "dark")
    M.box((.1, -.046, .1), (.05, .004, .05), "crimson")


def ammo(M):
    M.box((0, 0, .004), (.13, .09, .006), "steel")
    for x in (-.045, -.015, .015, .045):
        M.cyl((x, 0, .007), .018, .012, "copper", sides=6)
    M.box((0, 0, .018), (.125, .05, .028), "glass")


def spike(M):
    M.cyl((0, -.2, .03), .3, .02, "black", axis="y", sides=8).cyl((0, .1, .03), .1, .024, "dark", axis="y", sides=8)
    M.cyl((0, -.25, .03), .05, .004, "steel", axis="y", sides=6, r2=.02).cyl((0, .0, .03), .03, .021, "amber", axis="y", sides=8)
    M.box((.02, .05, .03), (.003, .012, .012), "glow_green")


def key_mandatory(M):
    M.box((0, 0, .01), (.16, .1, .012), "white").box((0, .03, .017), (.16, .03, .002), "crimson")
    for i in range(3):
        M.box((-.03 + i * .018, -.02 + i * .0, .017), (.016, .018 + i * .012, .002), "dark")


def key_optional(M):
    M.box((0, 0, .008), (.09, .055, .004), "white").box((0, .016, .011), (.09, .014, .002), "metal")


def ring(M):
    n = 12
    for i in range(n):
        a = 2 * math.pi * i / n
        M.box((.011 * math.cos(a), 0, .012 + .011 * math.sin(a)), (.006, .004, .006), "steel", rot=0)


def document(M):
    M.box((0, 0, .004), (.21, .29, .006), "paper").box((0, .1, .008), (.14, .012, .001), "dark")
    for i in range(5):
        M.box((-.01, .06 - i * .03, .008), (.16 - i * .012, .008, .001), "brown")


# ---------- confiscated objects (GDD Objetos y Coleccionables): everyday things with an inventory tag ----------

def tag(M, x=0.0, y=.0, z=.02):
    M.box((x, y, z), (.05, .03, .002), "paper")


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


def build_all():
    bpy.ops.wm.read_homefile(use_empty=True)
    write_palette()
    report = {}
    win = bpy.context.window_manager.windows[0]
    for group, models in SETS.items():
        col = bpy.data.collections.new(group)
        bpy.context.scene.collection.children.link(col)
        os.makedirs(OUT, exist_ok=True)
        for name, fn in models.items():
            M = Model(name)
            fn(M)
            ob, tris = M.finish(col)
            report[name] = tris
            vl = bpy.context.view_layer
            for o in vl.objects:
                o.select_set(False)
            ob.select_set(True)
            vl.objects.active = ob
            with bpy.context.temp_override(window=win):
                bpy.ops.export_scene.fbx(filepath=OUT + rf"\{name}.fbx", use_selection=True, object_types={"MESH"}, apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", bake_space_transform=True, bake_anim=False, mesh_smooth_type="FACE")
    os.makedirs(os.path.dirname(SOURCE), exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=SOURCE)
    return report
