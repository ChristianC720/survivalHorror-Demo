"""Environment models by floor: modular kit, doors, lamps and every prop (OBJECT_CATALOG.md), built in Blender (D-156).

Each floor has its own language so a screenshot tells the floor, and its own atlas and material so a room stays one draw call:
  F1 Maintenance: concrete below, riveted corrugated steel above, conduit, amber bands, rust runs.
  F2 Illegal clinic: tile below, stained plaster above, green rubber rail, improvised plastic and tape, cold tubes.
  F3 Core: dark machine panels, cable trunks, crimson light slits, grating.
Blender axes: front faces -Y (+Z in Unity), Blender X is Unity -X, metres. Kit pivots follow KIT_SPEC.md.

No light leaks by construction: every kit piece has a closed core ("hidden") reaching past its own edges into the floor, ceiling
and neighbouring pieces, so a crack between two pieces shows solid dark, never the room behind. Doors carry a frame that closes
the gap around the leaf. Faces nobody can see use "hidden", which spends no texture space.
"""
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
import hb_props as hbp

MODELS = ROOT + r"\Assets\_Project\Models"
ENV = MODELS + r"\Env"
X = hp.hex_col
F, UP = hbp.F, hbp.UP
BACK = (0, 1, 0)
INK = hbp.INK
PAPER, AMBER, RED, GREEN = hbp.PAPER, hbp.AMBER, hbp.RED, hbp.GREEN
ATLAS = 2048
CYAN = np.array((.05, .52, .62), dtype=np.float32)   # Vitalis Konzern (D-157); Lukas's teal is a different, greyer colour

hbp.COLORS.update({
    "hidden": (.085, .08, .075), "wet": (.27, .28, .27), "cyan": (.05, .52, .62), "glow_cyan": (.25, .92, 1.0), "concrete": (.34, .32, .29), "plate": (.27, .25, .23), "tile": (.66, .68, .64), "plaster": (.55, .56, .52),
    "vinyl": (.35, .4, .38), "rubber": (.1, .2, .17), "chrome": (.62, .64, .66), "plastic": (.74, .76, .72), "machine": (.13, .11, .12),
    "machine2": (.2, .17, .18), "grate": (.12, .11, .11), "cable": (.05, .045, .05), "tape": (.7, .6, .28), "mattress": (.4, .42, .4),
    "coat": (.72, .73, .68), "blue": (.2, .32, .4), "glow_white": (.86, .95, 1.0), "glow_teal": (.3, .95, .85), "bulb": (1.0, .68, .3),
})
hbp.GLOW.update({"glow_white", "glow_teal", "bulb", "glow_cyan"})
hbp.SEAMED.update({"plate", "machine", "machine2", "concrete"})
hbp.NAMES[:] = list(hbp.COLORS) + ["glass"]


def vitalis(M, center, normal, w):
    """The corporate mark: a cyan label with the name."""
    M.plate(center, normal, (w, w * .28), CYAN)
    return M.text(tuple(a + b * .0005 for a, b in zip(center, normal)), normal, w * .12, "VITALIS", PAPER)


def env_material(floor):
    name = "M_Env_" + floor
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    return m


class Env(hbp.Model):
    """hb_props.Model plus shapes for kit pieces: min/max boxes, painted-on-one-side slabs, tubes, extruded profiles."""
    SKIP = ("glass", "hidden")

    def __init__(self, name, floor, post=None):
        super().__init__(name)
        self.floor = floor
        self.post = post

    def bx(self, lo, hi, col):
        """Box from its corners, handy when pieces must meet exactly."""
        c = tuple((a + b) / 2 for a, b in zip(lo, hi))
        s = tuple(b - a for a, b in zip(lo, hi))
        return self.box(c, s, col)

    def slab(self, lo, hi, col, face="front"):
        """Box painted on one face only; the other five stay dark and take no texture."""
        x0, y0, z0 = lo
        x1, y1, z1 = hi
        v = [self.bm.verts.new((x, y, z)) for z in (z0, z1) for y in (y0, y1) for x in (x0, x1)]
        quads = {"bottom": (0, 2, 3, 1), "top": (4, 5, 7, 6), "front": (0, 1, 5, 4), "back": (2, 6, 7, 3), "left": (0, 4, 6, 2), "right": (1, 3, 7, 5)}
        for k, q in quads.items():
            self._face([v[i] for i in q], col if k == face else "hidden")
        return self

    def core(self, lo, hi):
        """The closed block that stops light at every seam."""
        return self.slab(lo, hi, "hidden", face="none")

    def tube(self, pts, r, col, sides=6):
        """Capped tube through points; each ring faces along the local direction."""
        pts = [Vector(p) for p in pts]
        rings = []
        for i, p in enumerate(pts):
            d = (pts[min(i + 1, len(pts) - 1)] - pts[max(i - 1, 0)]).normalized()
            u = d.orthogonal().normalized()
            v = d.cross(u)
            rings.append([self.bm.verts.new(p + (u * math.cos(2 * math.pi * j / sides) + v * math.sin(2 * math.pi * j / sides)) * r) for j in range(sides)])
        for a, b in zip(rings, rings[1:]):
            for j in range(sides):
                k = (j + 1) % sides
                self._face((a[j], a[k], b[k], b[j]), col)
        self._face(list(reversed(rings[0])), col)
        self._face(rings[-1], col)
        return self

    def profile(self, pts, y0, y1, col):
        """Extrudes an outline given as (x, z) along Y."""
        a = [self.bm.verts.new((x, y0, z)) for x, z in pts]
        b = [self.bm.verts.new((x, y1, z)) for x, z in pts]
        n = len(pts)
        for i in range(n):
            j = (i + 1) % n
            self._face((a[i], a[j], b[j], b[i]), col)
        self._face(list(reversed(a)), col)
        self._face(b, col)
        return self

    def finish(self, collection):
        bmesh.ops.recalc_face_normals(self.bm, faces=list(self.bm.faces))
        me = bpy.data.meshes.new(self.name)
        self.bm.to_mesh(me)
        self.bm.free()
        ob = bpy.data.objects.new(self.name, me)
        collection.objects.link(ob)
        me.materials.append(env_material(self.floor))
        glass = bpy.data.materials.get("MAT_Glass") or bpy.data.materials.new("MAT_Glass")
        me.materials.append(glass)
        me.calc_loop_triangles()
        layer = me.attributes["col"].data
        self.parts = [hbp.NAMES[layer[i].value] for i in range(len(me.polygons))]
        return ob, len(me.loop_triangles)


# ------------------------------------------------------------ surface paint per floor (model space, so it crosses polygons)

def _rust_runs(B, c, mask, seed=3, k=.62):
    P = B.P
    r = (hp.noise(P, (30, 30, 1.2), seed) > .78) & mask
    c[r] = c[r] * k + np.array(hbp.COLORS["rust"], dtype=np.float32) * (1 - k)


def _stains(B, c, mask, seed, col, cov=.62, a=.5):
    n = hp.fbm(B.P, 2.6, 3, seed)
    m = (n > cov) & mask
    c[m] = c[m] * (1 - a) + np.asarray(col, dtype=np.float32) * a


def _grid(P, a, b, pa, pb, w):
    """Mask of grid lines over two coordinate arrays."""
    ga = np.abs(((a + pa / 2) % pa) - pa / 2)
    gb = np.abs(((b + pb / 2) % pb) - pb / 2)
    return (ga < w) | (gb < w)


def post_f1_wall(B, c, e):
    P = B.P
    con, pl, yel = B.is_("concrete"), B.is_("plate"), B.is_("yellow")
    # Cast block courses, offset every other row.
    row = np.floor(P[:, 2] / .25)
    off = (P[:, 0] + (row % 2) * .25) % .5
    c[con & ((np.abs(off) < .007) | (((P[:, 2] + .004) % .25) < .009))] *= .62
    _stains(B, c, con, 3, X("#1c1712"), .58, .45)
    # Oil-black tide line along the floor.
    low = con & (P[:, 2] < .22 + (hp.noise(P, 6, 2) - .5) * .16)
    c[low] *= .55
    _rust_runs(B, c, pl, 4)
    _stains(B, c, pl, 6, X("#3a1f12"), .6, .5)
    # Worn amber safety band.
    c[yel & (hp.noise(P, 26, 7) > .68)] *= .45


def post_f1_floor(B, c, e):
    P = B.P
    pl = B.is_("plate")
    # Tread plate: staggered diagonal lozenges, the classic checker of engine rooms.
    d = np.floor(P[:, 0] / .06) + np.floor(P[:, 1] / .06)
    a = ((P[:, 0] + P[:, 1] * (1 - 2 * (d % 2))) * 42) % 1
    c[pl & (a < .3)] *= 1.32
    # Plate seams with bolts.
    c[pl & _grid(P, P[:, 0], P[:, 1], 1.0, 2.0, .012)] *= .38
    # Oil only in a few pools; large blotches read as camouflage at this resolution.
    _stains(B, c, pl, 9, X("#100c0a"), .68, .4)


def post_f1_ceiling(B, c, e):
    _stains(B, c, B.is_("concrete"), 11, X("#0e0c0a"), .5, .6)
    _rust_runs(B, c, B.is_("plate"), 12)


def post_f2_wall(B, c, e):
    P = B.P
    t, p = B.is_("tile"), B.is_("plaster")
    # Square glazed tile with dirty grout.
    grout = t & _grid(P, P[:, 0], P[:, 2], .2, .2, .006)
    c[t] *= (.92 + .16 * hp.noise(np.floor(P * np.array((5, 5, 5), dtype=np.float32)), 1.7, 4))[t, None]
    c[grout] = X("#2c332f")
    # A few tiles gone: raw mortar behind.
    broken = t & (hp.noise(np.floor(P * 5), 2.3, 6) > .86)
    c[broken] = X("#5a5a52") * (.8 + .4 * hp.noise(P, 40, 2))[broken, None]
    _stains(B, c, t, 8, X("#39402f"), .62, .35)
    # Damp plaster: stain tongues running down, hairline cracks.
    _stains(B, c, p, 13, X("#6c6650"), .55, .4)
    drip = p & (hp.noise(P, (22, 22, 1.1), 3) > .72)
    c[drip] *= .78
    crack = p & (np.abs(hp.fbm(P, 5, 3, 21) - .5) < .0055)
    c[crack] = X("#262a26")
    c[p & (P[:, 2] > 2.75)] *= .78


def post_f2_floor(B, c, e):
    P = B.P
    v = B.is_("vinyl")
    # Vinyl squares, two tones, scuffed down the middle where trolleys pass.
    alt = (np.floor(P[:, 0] / .5) + np.floor(P[:, 1] / .5)) % 2
    c[v & (alt == 0)] *= .78
    c[v & _grid(P, P[:, 0], P[:, 1], .5, .5, .006)] = X("#141a18")
    _stains(B, c, v, 15, X("#1e2520"), .69, .35)
    c[v & (hp.noise(P, (60, 8, 1), 5) > .75)] *= .84


def post_f2_ceiling(B, c, e):
    P = B.P
    t = B.is_("plaster")
    c[t & _grid(P, P[:, 0], P[:, 1], .5, .5, .007)] = X("#20241f")
    _stains(B, c, t, 17, X("#5e5a40"), .66, .4)


def post_f3_wall(B, c, e):
    P = B.P
    m = B.is_("machine", "machine2")
    # Panel breaks and stencilled plate numbers, oil-dark sheen near the floor.
    c[m & (((P[:, 2] + .01) % .75) < .012)] *= .4
    _stains(B, c, m, 19, X("#080607"), .55, .6)
    _rust_runs(B, c, m, 20, .8)


def post_f3_floor(B, c, e):
    P = B.P
    g = B.is_("grate")
    # Open grating: dark bars over a faint crimson glow from below.
    bars = _grid(P, P[:, 0], P[:, 1], .08, .08, .011)
    deep = g & ~bars
    glow = np.clip(hp.fbm(P, 1.6, 2, 8) - .42, 0, 1)[:, None] * np.array(hbp.COLORS["glow_crimson"], dtype=np.float32)
    c[deep] = (INK * 1.2 + glow * .55)[deep]
    e[deep] = (glow * .55)[deep]
    c[g & _grid(P, P[:, 0], P[:, 1], 1.0, 1.0, .02)] = X("#2c2526")


def post_f3_ceiling(B, c, e):
    _stains(B, c, B.is_("machine"), 23, X("#040304"), .5, .7)


# ------------------------------------------------------------ kit: walls

HEIGHT = 3.0
FRONT = -.3


def _core(M, top, x=1.03):
    M.core((-x, -.26, -.08), (x, -.03, top + .08))


def wall_f1(M, h=HEIGHT):
    M.pitch = 99
    M.slab((-1, FRONT, 0), (1, 0, 1.1), "concrete")
    M.slab((-1, FRONT, 1.1), (1, 0, h), "plate")
    _core(M, h)
    for i in range(8):
        x = -.875 + i * .25
        M.bx((x - .055, FRONT - .022, 1.2), (x + .055, FRONT, h - .08), "plate")
    M.bx((-1, FRONT - .03, 1.04), (1, FRONT, 1.16), "yellow")
    M.bx((-1, FRONT - .03, h - .08), (1, FRONT, h), "dark")
    for z in (2.55,) if h <= 3 else (2.55, 5.4):
        M.tube([(-1, FRONT - .07, z), (1, FRONT - .07, z)], .036, "rust", sides=6)
        M.tube([(-1, FRONT - .15, z - .09), (1, FRONT - .15, z - .09)], .018, "copper", sides=5)
        for x in (-.62, .62):
            M.bx((x - .03, FRONT - .12, z - .15), (x + .03, FRONT, z + .05), "dark")
    M.text((.62, FRONT - .03, .72), F, .09, "M-" + ("14" if h <= 3 else "22"), PAPER * .5, wear=.5)
    if h > 3:
        M.bx((-1, FRONT - .03, 3.0), (1, FRONT, 3.1), "yellow")


def wall_f2(M, h=HEIGHT):
    M.pitch = 99
    M.slab((-1, FRONT, 0), (1, 0, 1.2), "tile")
    M.slab((-1, FRONT, 1.2), (1, 0, h), "plaster")
    _core(M, h)
    M.bx((-1, FRONT - .02, 0), (1, FRONT, .1), "rubber")
    M.bx((-1, FRONT - .045, 1.16), (1, FRONT, 1.26), "rubber")
    # Improvised services: surface conduit taped along the wall, clipped every metre.
    M.bx((-1, FRONT - .03, 2.5), (1, FRONT, 2.56), "plastic")
    for x in (-.5, .5):
        M.bx((x - .02, FRONT - .04, 2.48), (x + .02, FRONT, 2.58), "dark")
    M.tube([(-.3, FRONT - .02, 2.5), (-.3, FRONT - .02, 1.3)], .012, "cable", sides=4)
    M.text((.5, FRONT - .05, 1.21), F, .05, "B-7", PAPER, wear=.2)
    if h > 3:
        M.bx((-1, FRONT - .03, 3.0), (1, FRONT, 3.14), "rubber")
        M.bx((-1, FRONT - .03, 5.4), (1, FRONT, 5.46), "plastic")


def wall_f3(M, h=HEIGHT):
    M.pitch = 99
    M.slab((-1, FRONT, 0), (1, 0, h), "machine")
    _core(M, h)
    # Plinth and head beam stand proud; between them recessed fins and a crimson slit.
    M.bx((-1, FRONT - .07, 0), (1, FRONT, .26), "machine2")
    M.bx((-1, FRONT - .07, h - .24), (1, FRONT, h), "machine2")
    for x in (-.9, -.56, .56, .9):
        M.bx((x - .06, FRONT - .05, .26), (x + .06, FRONT, h - .24), "machine2")
    M.bx((-.045, FRONT - .012, .34), (.045, FRONT, h - .32), "glow_crimson")
    M.bx((-.11, FRONT - .03, .3), (-.05, FRONT, h - .28), "dark")
    M.bx((.05, FRONT - .03, .3), (.11, FRONT, h - .28), "dark")
    for x in (-.74, .74):
        M.tube([(x, FRONT - .05, .26), (x, FRONT - .05, h - .24)], .042, "cable", sides=6)
        M.tube([(x + .08, FRONT - .03, .26), (x + .08, FRONT - .03, h - .24)], .02, "cable", sides=5)
    M.text((-.3, FRONT - .001, 2.0), F, .07, "K-3", PAPER * .45, wear=.4)
    M.code((.33, FRONT - .001, 1.0), F, .1, 5, PAPER * .5)
    if h > 3:
        M.bx((-1, FRONT - .07, 2.95), (1, FRONT, 3.2), "machine2")
        M.bx((-.045, FRONT - .012, 3.3), (.045, FRONT, 5.6), "glow_crimson")


def wall_high(fn):
    return lambda M: fn(M, 6.0)


def _window_shell(M, col_low, col_high, split):
    """Opening per KIT_SPEC: x +-.6, z 1.2 to 2.0, closed reveal on all sides."""
    M.pitch = 99
    for a, b, col in ((0, split, col_low), (split, 1.2, col_high)):
        if b > a:
            M.slab((-1, FRONT, a), (1, 0, b), col)
    M.slab((-1, FRONT, 2.0), (1, 0, HEIGHT), col_high)
    M.slab((-1, FRONT, 1.2), (-.6, 0, 2.0), col_high)
    M.slab((.6, FRONT, 1.2), (1, 0, 2.0), col_high)
    M.core((-1.03, -.26, -.08), (1.03, -.03, 1.16))
    M.core((-1.03, -.26, 2.04), (1.03, -.03, HEIGHT + .08))
    M.core((-1.03, -.26, 1.1), (-.64, -.03, 2.1))
    M.core((.64, -.03 - .23, 1.1), (1.03, -.03, 2.1))


def _window_frame(M, col, w=.05, proud=.03):
    M.bx((-.6 - w, FRONT - proud, 1.2 - w), (.6 + w, 0, 1.2), col)
    M.bx((-.6 - w, FRONT - proud, 2.0), (.6 + w, 0, 2.0 + w), col)
    M.bx((-.6 - w, FRONT - proud, 1.2), (-.6, 0, 2.0), col)
    M.bx((.6, FRONT - proud, 1.2), (.6 + w, 0, 2.0), col)


def window_f1(M):
    _window_shell(M, "concrete", "plate", 1.1)
    _window_frame(M, "dark")
    M.bx((-1, FRONT - .03, 1.04), (-.65, FRONT, 1.16), "yellow").bx((.65, FRONT - .03, 1.04), (1, FRONT, 1.16), "yellow")
    M.bx((-.65, FRONT - .03, 1.04), (.65, FRONT, 1.15), "yellow")
    M.bx((-1, FRONT - .03, HEIGHT - .08), (1, FRONT, HEIGHT), "dark")
    # Guard bars: a maintenance booth, not a lab.
    for x in (-.4, -.2, 0, .2, .4):
        M.tube([(x, -.15, 1.2), (x, -.15, 2.0)], .012, "steel", sides=5)


def window_f2(M):
    _window_shell(M, "tile", "plaster", 1.2)
    _window_frame(M, "plastic", .04)
    M.bx((-1, FRONT - .02, 0), (1, FRONT, .1), "rubber")
    M.bx((-1, FRONT - .045, 1.1), (1, FRONT, 1.2), "rubber")
    M.bx((-.6, -.16, 1.2), (.6, -.14, 2.0), "glass")
    # Observation blind, half lowered, one slat hanging.
    for i in range(8):
        z = 1.99 - i * .035
        M.bx((-.58, -.2, z - .01), (.58 - (.3 if i == 7 else 0), -.17, z), "plastic")


def window_f3(M):
    _window_shell(M, "machine", "machine", 0)
    M.bx((-1, FRONT - .07, 0), (1, FRONT, .26), "machine2")
    M.bx((-1, FRONT - .07, HEIGHT - .24), (1, FRONT, HEIGHT), "machine2")
    _window_frame(M, "machine2", .09, .06)
    # Armoured slit: the upper half is a steel shutter that never fully opens.
    M.bx((-.6, FRONT - .02, 1.62), (.6, -.1, 2.0), "steel")
    M.bx((-.6, -.16, 1.2), (.6, -.14, 1.62), "glass")
    M.hazard((0, FRONT - .021, 1.82), F, (1.2, .3), pitch=.2)
    M.bx((-.95, FRONT - .05, 1.3), (-.7, FRONT, 1.34), "glow_crimson").bx((.7, FRONT - .05, 1.3), (.95, FRONT, 1.34), "glow_crimson")


def doorway_f1(M):
    M.pitch = 99
    M.slab((-1, FRONT, 2.2), (1, 0, HEIGHT), "plate")
    M.core((-1.03, -.26, 2.24), (1.03, -.03, HEIGHT + .08))
    M.bx((-1, FRONT - .05, 2.2), (1, FRONT, 2.42), "yellow")
    M.hazard((0, FRONT - .051, 2.31), F, (2.0, .22), pitch=.28)
    M.bx((-1, FRONT - .03, HEIGHT - .08), (1, FRONT, HEIGHT), "dark")
    M.tube([(-1, FRONT - .07, 2.55), (1, FRONT - .07, 2.55)], .036, "rust", sides=6)
    M.text((0, FRONT - .002, 2.76), F, .1, "SCHOTT", PAPER * .6, wear=.4)


def doorway_f2(M):
    M.pitch = 99
    M.slab((-1, FRONT, 2.2), (1, 0, HEIGHT), "plaster")
    M.core((-1.03, -.26, 2.24), (1.03, -.03, HEIGHT + .08))
    M.bx((-1, FRONT - .03, 2.2), (1, FRONT, 2.3), "rubber")
    M.bx((-1, FRONT - .03, 2.5), (1, FRONT, 2.56), "plastic")
    # Backlit exit-style sign, the one green thing on the floor (GDD: green is the facility's information).
    M.bx((-.24, FRONT - .05, 2.62), (.24, FRONT, 2.82), "dark")
    M.bx((-.2, FRONT - .052, 2.65), (.2, FRONT - .05, 2.79), "glow_green")
    M.text((0, FRONT - .0525, 2.72), F, .08, "TUR", INK)


def doorway_f3(M):
    M.pitch = 99
    M.slab((-1, FRONT, 2.2), (1, 0, HEIGHT), "machine")
    M.core((-1.03, -.26, 2.24), (1.03, -.03, HEIGHT + .08))
    M.bx((-1, FRONT - .09, 2.2), (1, FRONT, 2.5), "machine2")
    M.bx((-1, FRONT - .07, HEIGHT - .24), (1, FRONT, HEIGHT), "machine2")
    M.bx((-.5, FRONT - .092, 2.34), (.5, FRONT - .09, 2.38), "glow_crimson")
    for x in (-.74, .74):
        M.tube([(x, FRONT - .05, 2.5), (x, FRONT - .05, HEIGHT - .24)], .042, "cable", sides=6)


# ------------------------------------------------------------ kit: floors, ceilings, columns (pivot at the min corner in Unity)

def _floor_shell(M, col):
    M.pitch = 99
    M.slab((-2, -2, -.2), (0, 0, 0), col, face="top")
    M.core((-2.03, -2.03, -.18), (.03, .03, -.02))


def floor_f1(M):
    _floor_shell(M, "plate")


def floor_f2(M):
    _floor_shell(M, "vinyl")


def floor_f3(M):
    _floor_shell(M, "grate")


def _ceil_shell(M, col):
    M.pitch = 99
    M.slab((-2, -2, 3), (0, 0, 3.2), col, face="bottom")
    M.core((-2.03, -2.03, 3.02), (.03, .03, 3.18))


def ceiling_f1(M):
    _ceil_shell(M, "concrete")
    # Steel joists and a pipe run that lines up tile to tile.
    for x in (-1.5, -.5):
        M.bx((x - .05, -2, 2.86), (x + .05, 0, 3), "plate")
        M.bx((x - .08, -2, 2.84), (x + .08, 0, 2.87), "dark")
    M.tube([(-2, -1, 2.78), (0, -1, 2.78)], .05, "rust", sides=6)


def ceiling_f2(M):
    _ceil_shell(M, "plaster")
    # Drop-ceiling tube, dim on purpose: the room's real light is the emergency lamp.
    M.bx((-1.35, -1.08, 2.94), (-.65, -.92, 3), "plastic")
    M.bx((-1.3, -1.05, 2.935), (-.7, -.95, 2.94), "glow_white")
    M.bx((-2, -.03, 2.96), (0, .0, 3), "plastic").bx((-.03, -2, 2.96), (0, 0, 3), "plastic")


def ceiling_f3(M):
    _ceil_shell(M, "machine")
    # Heavy cable tray.
    M.bx((-2, -1.25, 2.86), (0, -.75, 2.9), "machine2")
    for x in (-1.6, -.4):
        M.bx((x - .03, -1.3, 2.86), (x + .03, -1.26, 3), "dark").bx((x - .03, -.74, 2.86), (x + .03, -.7, 3), "dark")
    for y in (-1.14, -1.02, -.9):
        M.tube([(-2, y, 2.84), (0, y, 2.84)], .035, "cable", sides=5)


def column_f1(M):
    M.pitch = 99
    # Riveted I-beam, hazard wrapped at bumper height.
    M.bx((-.2, -.2, 0), (.2, -.14, 3), "plate").bx((-.2, .14, 0), (.2, .2, 3), "plate").bx((-.04, -.14, 0), (.04, .14, 3), "plate")
    M.bx((-.22, -.22, 0), (.22, .22, .12), "dark")
    M.hazard((0, -.201, .55), F, (.4, .7), pitch=.14).hazard((0, .201, .55), BACK, (.4, .7), pitch=.14)


def column_f2(M):
    M.pitch = 99
    M.bx((-.2, -.2, 0), (.2, .2, 1.2), "tile").bx((-.2, -.2, 1.2), (.2, .2, 3), "plaster")
    M.bx((-.22, -.22, 0), (.22, .22, .1), "rubber").bx((-.23, -.23, 1.16), (.23, .23, 1.26), "rubber")


def column_f3(M):
    M.pitch = 99
    M.bx((-.2, -.2, 0), (.2, .2, 3), "machine")
    for z in (.1, 2.75):
        M.bx((-.22, -.22, z), (.22, .22, z + .15), "machine2")
    # Wound coil bands: full rings around the shaft, so nothing pokes through its corners as squares.
    for z in np.linspace(.4, 2.5, 8):
        M.bx((-.215, -.215, float(z)), (.215, .215, float(z) + .1), "copper")
    M.bx((-.216, -.216, 1.46), (.216, .216, 1.5), "glow_crimson")


# ------------------------------------------------------------ doors: frame stays, leaf slides up (DoorLeaf)

def _frame(M, col, trim):
    """Closes the 5 cm gap around the leaf and covers the wall ends of both rooms. Centered on the shared socket."""
    M.pitch = 99
    for s in (-1, 1):
        a, b = sorted((s * .95, s * 1.0))
        M.bx((a, -.3, 0), (b, .3, 2.2), col)
        a2, b2 = sorted((s * .93, s * 1.0))
        M.bx((a2, -.32, 0), (b2, -.3, 2.2), trim).bx((a2, .3, 0), (b2, .32, 2.2), trim)
    M.bx((-1, -.3, 2.15), (1, .3, 2.2), col)
    M.bx((-1, -.32, 2.12), (1, -.3, 2.2), trim).bx((-1, .3, 2.12), (1, .32, 2.2), trim)
    M.bx((-1, -.3, -.02), (1, .3, .006), "dark")


def _leaf_body(M, col):
    M.pitch = 99
    M.bx((-.95, -.04, -1.075), (.95, .04, 1.075), col)


def frame_f1(M):
    _frame(M, "plate", "yellow")
    M.hazard((0, -.321, 2.16), F, (2.0, .08), pitch=.2).hazard((0, .321, 2.16), BACK, (2.0, .08), pitch=.2)


def frame_f2(M):
    _frame(M, "plastic", "rubber")


def frame_f3(M):
    _frame(M, "machine2", "machine")
    for y in (-.321, .321):
        M.bx((-.4, y - .001, 2.14), (.4, y + .001, 2.17), "glow_crimson")


def leaf_f1(M):
    """Watertight bulkhead: frame ribs, a wheel on each face, chevrons at the foot."""
    _leaf_body(M, "plate")
    for y, n in ((-.04, -1), (.04, 1)):
        d = .02 * n
        for z in (-.75, 0, .75):
            a, b = sorted((y, y + d))
            M.bx((-.85, a, z - .04), (.85, b, z + .04), "dark")
        for x in (-.85, .85):
            a, b = sorted((y, y + d))
            M.bx((x - .04, a, -1.0), (x + .04, b, 1.0), "dark")
        M.cyl((0, y, .08), .07 * n, .24, "rust", sides=10, axis="y").cyl((0, y + .07 * n, .08), .02 * n, .06, "steel", sides=8, axis="y")
        for a_ in (0, 1.05, 2.09):
            a0, b0 = sorted((y + .05 * n, y + .08 * n))
            M.box((0, (a0 + b0) / 2, .08), (.46, .03, .03), "steel", rot=0).box((0, (a0 + b0) / 2, .08), (.03, .03, .46), "steel")
        v = F if n < 0 else BACK
        M.hazard((0, y + d * 1.05, -.9), v, (1.7, .3), pitch=.24)
        M.text((0, y + d * .1, .7), v, .08, "SCHOTT 4", PAPER * .6, wear=.4)
        vitalis(M, (0, y + d * .1, .4), v, .36)


def leaf_f2(M):
    """One half of the hospital sliding door; the pivot is its own centre and the meeting edge carries a rubber strip."""
    M.pitch = 99
    M.bx((-.465, -.04, -1.075), (.465, .04, 1.075), "plaster")
    for y, n in ((-.04, -1), (.04, 1)):
        a, b = sorted((y, y + .004 * n))
        M.bx((-.455, a, -1.07), (.455, b, -.72), "steel")
        M.cyl((0, y, .52), .012 * n, .2, "dark", sides=12, axis="y")
        M.cyl((0, y + .012 * n, .52), .001 * n, .16, "glass", sides=12, axis="y")
        v = F if n < 0 else BACK
        M.bx((-.34, a, .1), (.34, b, .14), "cyan")
        vitalis(M, (0, y + .001 * n, -.1), v, .3)
        M.text((0, y + .001 * n, -.4), v, .06, "OP 2", INK, wear=.25)
    for x in (-.47, .47):
        M.bx((x - .005, -.045, -1.075), (x + .005, .045, 1.075), "rubber")


def leaf_f3(M):
    """Blast door: split along a diagonal, heavy ribs, crimson status light."""
    _leaf_body(M, "machine")
    for y, n in ((-.04, -1), (.04, 1)):
        d = .025 * n
        a, b = sorted((y, y + d))
        for z in (-.8, -.4, .4, .8):
            M.bx((-.9, a, z - .05), (.9, b, z + .05), "machine2")
        M.box((0, (a + b) / 2, 0), (.05, abs(d) * 1.2, 2.5), "dark", rot=0)
        a2, b2 = sorted((y, y + .004 * n))
        M.bx((-.5, a2, -.02), (.5, b2, .03), "glow_crimson")
        v = F if n < 0 else BACK
        M.hazard((0, y + d * 1.05, -.98), v, (1.7, .14), pitch=.2)
        M.text((-.5, y + d * .1, .62), v, .06, "KERN", PAPER * .5, wear=.3)


# ------------------------------------------------------------ lamps: no shadows, so the fixture never blocks its own light

def lamp_f1(M):
    M.bx((-.14, -.14, .12), (.14, .14, .2), "dark")
    M.cyl((0, 0, -.02), .14, .09, "bulb", sides=8)
    for a in range(4):
        c, s = math.cos(a * 1.57), math.sin(a * 1.57)
        M.tube([(.11 * c, .11 * s, .12), (.11 * c, .11 * s, -.05)], .008, "dark", sides=4)
    M.cyl((0, 0, -.06), .02, .115, "dark", sides=8)


def lamp_f2(M):
    M.bx((-.6, -.08, .12), (.6, .08, .2), "plastic")
    M.bx((-.55, -.05, .1), (.55, .05, .12), "glow_white")


def lamp_f3(M):
    M.cyl((0, 0, .12), .08, .13, "machine2", sides=8)
    M.cyl((0, 0, .0), .12, .09, "glow_crimson", sides=8)
    M.bx((-.13, -.012, -.02), (.13, .012, .13), "dark").bx((-.012, -.13, -.02), (.012, .13, .13), "dark")


# ------------------------------------------------------------ props: shared

def speaker(M):
    M.bx((-.04, .08, .1), (.04, .15, .4), "dark")
    M.cyl((0, .1, .25), .05, .05, "metal", axis="y", sides=8)
    M.cyl((0, -.15, .25), .25, .19, "metal", axis="y", sides=10, r2=.06)
    M.cyl((0, -.155, .25), .006, .17, "black", axis="y", sides=10)
    M.text((0, .06, .05), BACK, .03, "PA 2", PAPER * .6)


def cable_bundle(M):
    for y, z, r, col in ((0, .08, .03, "cable"), (.05, .05, .02, "cable"), (-.05, .05, .018, "crimson"), (.0, .02, .016, "yellow")):
        M.tube([(-1, y, z), (-.3, y + .01, z + .01), (.3, y - .01, z), (1, y, z)], r, col, sides=5)
    for x in (-.7, 0, .7):
        M.bx((x - .02, -.08, 0), (x + .02, .08, .12), "dark")


def pipe2m(M):
    M.tube([(-1, 0, .15), (1, 0, .15)], .13, "metal", sides=10)
    for x in (-.96, .96):
        M.cyl((x - .03, 0, .15), .06, .15, "dark", axis="x", sides=10)
    M.text((0, -.13, .17), F, .05, "H2O", PAPER * .6, wear=.4)
    M.bx((-.2, -.131, .08), (.2, -.12, .1), "yellow")


def pipe_elbow(M):
    pts = [(-1, 0, .15), (-.35, 0, .15), (-.15, 0, .2), (0, 0, .35), (.05, 0, .55), (.05, 0, 1.0)]
    M.tube(pts, .13, "metal", sides=10)
    M.cyl((-.97, 0, .15), .06, .15, "dark", axis="x", sides=10).cyl((.05, 0, .96), .06, .15, "dark", sides=10)


def trash(M):
    rng = np.random.default_rng(3)
    for i in range(16):
        s = rng.uniform(.06, .22, 3)
        M.box((rng.uniform(-.38, .38), rng.uniform(-.38, .38), s[2] / 2 + rng.uniform(0, .05)), tuple(s), ("dark", "rust", "paper", "plastic", "plate")[i % 5], rot=float(rng.uniform(0, 3)))
    M.tube([(-.3, -.4, .03), (0, -.1, .05), (.35, .2, .04)], .012, "cable", sides=4)


def crate(M, s=1.0):
    M.pitch = 99
    h = s
    M.bx((-s / 2, -s / 2, 0), (s / 2, s / 2, h), "plate")
    b = .05 * s
    for z in (0, h - b):
        M.bx((-s / 2 - .005, -s / 2 - .005, z), (s / 2 + .005, s / 2 + .005, z + b), "dark")
    for x in (-s / 2, s / 2 - b):
        for y in (-s / 2, s / 2 - b):
            M.bx((x - .005, y - .005, 0), (x + b + .005, y + b + .005, h), "dark")
    M.bx((-s * .4, -s / 2 - .01, h * .1), (s * .4, -s / 2, h * .9), "plate")
    vitalis(M, (0, -s / 2 - .012, h * .62), F, .8 * s)
    M.code((-.22 * s, -s / 2 - .012, h * .28), F, .1 * s, 3, PAPER * .7)
    M.hazard((.2 * s, -s / 2 - .012, h * .28), F, (.3 * s, .12 * s), pitch=.08)


def poster(M):
    M.bx((-.4, -.012, 0), (.4, 0, 1.1), "paper")
    M.bx((-.34, -.013, .86), (.34, -.012, 1.02), "cyan")
    M.text((0, -.0135, .94), F, .1, "VITALIS", PAPER)
    M.text((0, -.0135, .72), F, .04, "IHRE GESUNDHEIT", INK, wear=.1)
    M.text((0, -.0135, .62), F, .04, "IST UNSER AUFTRAG", INK, wear=.1)
    M.code((-.24, -.0135, .2), F, .14, 2)
    M.bx((-.04, -.014, 1.06), (.04, -.012, 1.1), "tape").bx((-.4, -.014, 0), (-.3, -.012, .04), "tape")


def grate(M):
    M.bx((-.5, -.5, 0), (.5, .5, .015), "dark")
    for i in range(9):
        x = -.4 + i * .1
        M.bx((x - .02, -.46, .012), (x + .02, .46, .02), "steel")
    for x, y in ((-.46, -.46), (.46, -.46), (-.46, .46), (.46, .46)):
        M.cyl((x, y, .012), .012, .014, "steel", sides=6)


def puddle(M):
    # Flat irregular slab just above the floor: no z-fight, and no thickness to light from the side.
    pts = [(.75 * math.cos(a) * (1 + .22 * math.sin(a * 3)), .5 * math.sin(a) * (1 + .18 * math.cos(a * 2))) for a in np.linspace(0, 6.283, 14)[:-1]]
    v0 = [M.bm.verts.new((x, y, .002)) for x, y in pts]
    v1 = [M.bm.verts.new((x * 1.02, y * 1.02, .008)) for x, y in pts]
    for i in range(len(pts)):
        j = (i + 1) % len(pts)
        M._face((v0[i], v0[j], v1[j], v1[i]), "wet")
    # Dark water, not black: a black shape on the floor reads as a broken shadow.
    M._face(v1, "wet")
    M._face(list(reversed(v0)), "hidden")


def hack_panel(M):
    M.bx((-.14, -.05, .9), (.14, 0, 1.4), "dark")
    M.bx((-.11, -.052, 1.14), (.11, -.05, 1.34), "black")
    M.screen((0, -.053, 1.24), F, (.22, .2), 4)
    M.lamps((0, -.053, 1.03), F, 3, 3, .05, 2)
    M.tube([(.1, -.02, .9), (.1, -.02, .3), (.1, 0, .1)], .012, "cable", sides=4)
    M.text((0, -.052, .93), F, .02, "SCHLEUSE", PAPER * .8)


def power_switch(M):
    M.bx((-.3, -.2, 0), (.3, .2, 1.6), "yellow")
    M.bx((-.31, -.21, 0), (.31, .21, .1), "dark").bx((-.31, -.21, 1.52), (.31, .21, 1.6), "dark")
    M.bx((-.24, -.205, .2), (.24, -.2, 1.45), "plate")
    M.bx((-.08, -.21, .96), (.08, -.205, 1.34), "dark")
    M.plate((0, -.206, .62), F, (.34, .2), AMBER).text((0, -.207, .66), F, .05, "STROM", INK).text((0, -.207, .57), F, .05, "HAUPT", INK)
    vitalis(M, (0, -.207, 1.42), F, .26)
    M.lamps((0, -.206, .32), F, 4, 2, .07, 6)
    M.hazard((0, -.211, .05), F, (.6, .09), pitch=.1)
    M.tube([(.2, .1, 1.6), (.2, .1, 2.2)], .025, "cable", sides=5)


def switch_lever(M):
    # Own transform, so the switch can move it; origin at the pivot.
    M.bx((-.04, -.05, -.17), (.04, .05, .18), "crimson")
    M.bx((-.06, -.08, .14), (.06, .03, .2), "dark")


def tray(M):
    M.bx((-.35, -.25, .86), (.35, .25, .9), "steel")
    M.bx((-.36, -.26, .9), (.36, -.24, .94), "steel").bx((-.36, .24, .9), (.36, .26, .94), "steel")
    M.bx((-.36, -.25, .9), (-.34, .25, .94), "steel").bx((.34, -.25, .9), (.36, .25, .94), "steel")
    M.tube([(0, 0, 0), (0, 0, .86)], .03, "metal", sides=8)
    M.cyl((0, 0, 0), .03, .2, "dark", sides=10)
    M.plate((0, -.261, .92), F, (.3, .03), PAPER).text((0, -.262, .92), F, .018, "R-H EFFEKTEN", INK)


def platform(M):
    M.pitch = 99
    M.slab((-2, -2, 1.3), (0, 0, 1.5), "grate", face="top")
    M.core((-2.02, -2.02, 1.33), (.02, .02, 1.48))
    M.bx((-2, -2, 1.3), (0, -1.95, 1.5), "yellow").bx((-2, -.05, 1.3), (0, 0, 1.5), "yellow")
    # Rails 0.6 m above the deck, posts in the corners and mid-span; the colliders stay as they were.
    for a, b in (((-2, -1.97), (0, -1.97)), ((-2, -.03), (0, -.03))):
        M.tube([(a[0], a[1], 2.48), (b[0], b[1], 2.48)], .018, "steel", sides=5)
    for x in (-1.97, -.03):
        M.tube([(x, -1.97, 2.48), (x, -.03, 2.48)], .018, "steel", sides=5)
        for y in (-1.97, -1, -.03):
            M.tube([(x, y, 1.5), (x, y, 2.5)], .018, "steel", sides=5)
    M.tube([(-1, -1.97, 1.5), (-1, -1.97, 2.5)], .018, "steel", sides=5).tube([(-1, -.03, 1.5), (-1, -.03, 2.5)], .018, "steel", sides=5)


def ramp(M):
    M.pitch = 99
    # Wedge in Unity: x 0..2, z 0..4, rising 0 to 1.5 along z.
    v = [M.bm.verts.new(p) for p in ((-2, 0, 0), (0, 0, 0), (-2, -4, 1.5), (0, -4, 1.5), (-2, -4, 0), (0, -4, 0))]
    M._face((v[0], v[1], v[3], v[2]), "plate")
    M._face((v[4], v[5], v[1], v[0]), "hidden")
    M._face((v[2], v[3], v[5], v[4]), "dark")
    M._face((v[0], v[2], v[4]), "dark")
    M._face((v[1], v[5], v[3]), "dark")
    for k in range(9):
        y = -.2 - k * .45
        z = -y * 1.5 / 4
        M.bx((-1.95, y - .03, z), (-.05, y + .03, z + .025), "yellow")


def hostile_cam_env(M):
    M.bx((-.15, -.2, -.12), (.15, .3, .12), "machine")
    M.cyl((0, -.28, 0), .1, .1, "machine2", axis="y", sides=10).cyl((0, -.3, 0), .025, .075, "glow_crimson", axis="y", sides=10)
    M.bx((-.04, .05, .12), (.04, .15, .32), "dark")
    M.tube([(.1, .3, 0), (.2, .4, .1), (.2, .4, .4)], .012, "cable", sides=4)
    M.text((0, -.205, .07), F, .03, "K-07", PAPER * .7)


def exit_door(M):
    # The Mother's room exit: a real door among machines, on the same size as the old cube.
    M.bx((-1, -.05, -1.1), (1, .05, 1.1), "machine")
    for z in (-.7, 0, .7):
        M.bx((-.92, -.075, z - .05), (.92, -.05, z + .05), "machine2")
    M.bx((-.02, -.076, -1.1), (.02, -.05, 1.1), "dark")
    M.bx((-.6, -.077, .92), (.6, -.075, 1.02), "glow_green")
    M.text((0, -.078, .97), F, .07, "AUSGANG", INK)
    M.hazard((0, -.076, -.98), F, (1.84, .2), pitch=.2)


# ------------------------------------------------------------ props: F1

def generator(M):
    M.pitch = .5
    M.bx((-1, -.75, 0), (1, .75, .24), "dark")
    M.bx((-.95, -.65, .24), (.95, .65, 1.86), "metal")
    # Engine block: finned cylinders on the top, exhaust, fuel line.
    for i in range(4):
        x = -.6 + i * .4
        M.cyl((x, .1, 1.86), .22, .14, "steel", sides=10)
        for k in range(3):
            M.cyl((x, .1, 1.9 + k * .06), .02, .17, "dark", sides=10)
    M.tube([(-.7, .3, 1.86), (-.7, .3, 2.4), (-.4, .3, 2.5)], .06, "rust", sides=8)
    M.tube([(.95, -.3, .5), (1.0, -.3, 1.6), (.6, -.2, 1.9)], .025, "copper", sides=5)
    M.bx((-.45, -.66, .95), (.45, -.65, 1.65), "dark")
    M.screen((-.2, -.661, 1.4), F, (.35, .3), 3).lamps((.22, -.661, 1.4), F, 4, 3, .07, 3)
    for x in (.55, .3):
        M.cyl((x, -.68, 1.1), .03, .1, "steel", axis="y", sides=10)
    M.bx((-.9, -.655, .4), (-.2, -.65, .85), "yellow")
    M.plate((-.55, -.656, .63), F, (.66, .38), AMBER).text((-.55, -.657, .7), F, .08, "GEFAHR").text((-.55, -.657, .55), F, .08, "VORSICHT")
    for i in range(7):
        M.bx((-.98, -.5 + i * .16, .4), (-.95, -.42 + i * .16, 1.7), "dark").bx((.95, -.5 + i * .16, .4), (.98, -.42 + i * .16, 1.7), "dark")
    M.text((.25, -.652, .6), F, .1, "AGG 3", PAPER * .55, wear=.5).hazard((0, -.751, .12), F, (2.0, .2))
    M.code((.75, -.652, .55), F, .14, 4, PAPER)
    M.bx((.1, -.68, 1.72), (.3, -.65, 1.78), "glow_amber")


def boiler(M):
    M.pitch = 99
    M.bx((-.75, -.75, 0), (.75, .75, .1), "dark")
    M.cyl((0, 0, .1), 2.5, .72, "rust", sides=18)
    M.cyl((0, 0, 2.6), .22, .72, "metal", sides=18, r2=.34).cyl((0, 0, 2.82), .18, .16, "metal", sides=10)
    for z in (.4, 1.05, 1.7, 2.35):
        M.cyl((0, 0, z), .05, .74, "dark", sides=18)
    # Firebox door, gauge, valves and the riser that leaves through the ceiling line.
    M.bx((-.3, -.74, .55), (.3, -.62, 1.15), "dark").cyl((0, -.76, .85), .03, .14, "steel", axis="y", sides=10)
    M.bx((-.24, -.75, .6), (.24, -.74, .72), "glow_amber")
    M.cyl((.5, -.57, 1.9), .03, .1, "paper", axis="y", sides=12)
    M.tube([(.7, 0, 2.2), (1.15, 0, 2.2), (1.15, 0, 0)], .06, "metal", sides=8)
    M.cyl((1.15, 0, 1.1), .1, .1, "crimson", sides=10)
    M.tube([(-.7, .2, .5), (-1.0, .2, .5), (-1.0, .2, 2.95)], .04, "copper", sides=6)
    M.text((0, -.73, 1.5), F, .12, "KESSEL 2", PAPER * .75, wear=.45).text((0, -.73, 1.28), F, .09, "HEISS", AMBER, wear=.3)
    M.hazard((0, -.751, .05), F, (1.5, .09))


def water_tank(M):
    M.pitch = 99
    for a in (.78, 2.36, 3.93, 5.5):
        M.bx((.8 * math.cos(a) - .05, .8 * math.sin(a) - .05, 0), (.8 * math.cos(a) + .05, .8 * math.sin(a) + .05, .4), "dark")
    M.cyl((0, 0, .35), .18, .95, "metal", sides=18, r2=1.0).cyl((0, 0, .53), 1.7, 1.0, "metal", sides=18).cyl((0, 0, 2.23), .27, 1.0, "metal", sides=18, r2=.3)
    for z in (.95, 1.5, 2.05):
        M.cyl((0, 0, z), .05, 1.02, "dark", sides=18)
    M.tube([(-.3, -.95, .7), (-.3, -1.0, .1)], .05, "copper", sides=6)
    M.tube([(.5, -.86, .75), (.5, -.96, 2.1)], .018, "glass", sides=5)
    M.text((0, -1.015, 1.75), F, .22, "H2O", PAPER * .7, wear=.5).text((0, -1.015, 1.22), F, .08, "NICHT TRINKBAR", AMBER, wear=.4)


def forklift(M):
    M.pitch = .6
    M.bx((-.55, -.45, .25), (.55, 1.05, .85), "yellow")
    M.bx((-.45, .3, .85), (.45, .95, 1.25), "yellow")
    # Cab cage, seat, steering, mast and forks.
    for x in (-.5, .5):
        M.bx((x - .03, -.2, .85), (x + .03, -.14, 2.2), "dark").bx((x - .03, .92, .85), (x + .03, .98, 2.2), "dark")
        M.cyl((x * 1.12, -.2, .28), .2, .28, "black", axis="x", sides=12).cyl((x * 1.12, .85, .24), .2, .24, "black", axis="x", sides=12)
        M.cyl((x * 1.14, -.2, .28), .06, .12, "steel", axis="x", sides=8)
        M.bx((x * .4 - .04, -.76, .05), (x * .4 + .04, -.68, 2.3), "metal")
    M.bx((-.55, -.2, 2.17), (.55, .98, 2.23), "dark")
    M.bx((-.28, .22, .85), (.28, .62, .98), "black").bx((-.28, .55, .98), (.28, .65, 1.45), "black")
    M.tube([(0, .02, .85), (0, -.05, 1.25)], .018, "dark", sides=5).cyl((0, -.09, 1.25), .03, .14, "black", sides=10)
    for x in (-.3, .3):
        M.bx((x - .06, -1.2, .05), (x + .06, -.72, .1), "steel").bx((x - .06, -.78, .05), (x + .06, -.72, .95), "steel")
    M.bx((-.42, -.8, .85), (.42, -.74, 1.02), "dark").bx((-.42, -.8, .32), (.42, -.74, .46), "dark")
    M.cyl((.3, .95, 1.1), .3, .1, "yellow", axis="y", sides=8)
    M.text((0, -.46, .55), F, .08, "VK-14", INK, wear=.4).hazard((0, 1.051, .55), BACK, (1.1, .6), pitch=.2)
    M.bx((.36, 1.05, 1.0), (.48, 1.06, 1.08), "glow_amber")


def gantry(M):
    M.pitch = .75
    M.bx((-3, -.175, .125), (3, .175, .475), "yellow")
    M.bx((-3, -.25, .5), (3, .25, .58), "dark").bx((-3, -.25, .02), (3, .25, .1), "dark")
    for i in range(12):
        x = -2.75 + i * .5
        M.tube([(x, -.18, .1), (x + .25, -.18, .5), (x + .5, -.18, .1)], .022, "dark", sides=4)
    M.bx((.25, -.27, -.25), (.95, .27, .05), "metal").cyl((.6, -.3, -.1), .6, .14, "dark", axis="y", sides=12)
    M.tube([(.6, 0, -.25), (.6, 0, -1.2)], .018, "black", sides=4)
    M.bx((.53, -.06, -1.28), (.67, .06, -1.18), "rust")
    M.tube([(.6, 0, -1.28), (.6, -.03, -1.36), (.6, .05, -1.42)], .025, "rust", sides=5)
    for x in (-2.9, 2.9):
        M.bx((x - .1, -.35, -.05), (x + .1, .35, .65), "metal")
        M.cyl((x, -.36, .1), .72, .08, "dark", axis="y", sides=10)
    M.hazard((0, -.176, .3), F, (6, .35), pitch=.28).text((-1.6, -.177, .3), F, .16, "5 T", INK)


def pump(M):
    M.pitch = 99
    M.bx((-.5, -.75, 0), (.5, .75, .2), "dark")
    M.cyl((0, -.5, .55), .75, .24, "rust", axis="y", sides=12)
    M.cyl((0, -.55, .55), .08, .3, "dark", axis="y", sides=12).cyl((0, .25, .55), .08, .3, "dark", axis="y", sides=12)
    for a in np.linspace(0, 6.28, 7)[:-1]:
        M.cyl((.3 * math.cos(a), -.56, .55 + .3 * math.sin(a)), .02, .02, "steel", axis="y", sides=6)
    M.bx((-.08, -.1, .78), (.08, .1, .92), "metal").cyl((0, 0, .92), .3, .1, "metal", sides=10).cyl((0, 0, 1.22), .05, .16, "steel", sides=10)
    M.tube([(.24, .15, .55), (.5, .15, .55), (.5, .15, 1.0)], .07, "copper", sides=8)
    M.tube([(-.24, .15, .55), (-.5, .15, .55), (-.5, .15, .2)], .07, "copper", sides=8)
    M.bx((-.04, .33, .2), (.04, .75, .4), "dark").bx((-.2, .45, .2), (.2, .7, .5), "metal")
    M.text((0, -.1, .86), F, .045, "PUMPE B", PAPER * .9, wear=.3).hazard((0, -.751, .1), F, (1.0, .2))


def valve(M):
    M.bx((-.2, .0, .0), (.2, .1, .4), "dark")
    M.cyl((0, -.1, .2), .1, .05, "steel", axis="y", sides=8)
    M.cyl((0, -.13, .2), .03, .16, "crimson", axis="y", sides=12)
    M.bx((-.14, -.14, .185), (.14, -.12, .215), "crimson").bx((-.015, -.14, .06), (.015, -.12, .34), "crimson")
    M.text((0, .0, .05), F, .03, "ZU", PAPER)


def thick_pipe(M):
    M.tube([(-1, 0, .4), (1, 0, .4)], .38, "rust", sides=14)
    for x in (-.97, 0, .97):
        M.cyl((x - .04, 0, .4), .08, .4, "dark", axis="x", sides=14)
    M.bx((-.7, -.4, 0), (-.5, .4, .12), "dark").bx((.5, -.4, 0), (.7, .4, .12), "dark")
    M.text((-.45, -.38, .48), F, .1, "ROHR 6", PAPER * .55, wear=.45).hazard((.55, -.38, .4), F, (.4, .12), pitch=.08)


def workbench(M):
    M.pitch = 99
    M.bx((-1, -.4, .82), (1, .4, .9), "wood")
    M.bx((-.95, -.35, .3), (-.4, .35, .82), "metal")
    for z in (.36, .52, .68):
        M.bx((-.9, -.36, z), (-.45, -.35, z + .12), "plate").bx((-.72, -.365, z + .05), (-.62, -.36, z + .07), "steel")
    for x, y in ((.92, -.33), (.92, .33), (-.92, -.33), (-.92, .33)):
        M.bx((x - .04, y - .04, 0), (x + .04, y + .04, .82), "dark")
    M.bx((-.4, -.3, .22), (.92, .3, .26), "plate")
    M.bx((.3, -.4, .9), (.42, -.2, .98), "dark").cyl((.36, -.32, .98), .03, .05, "steel", sides=8).bx((.28, -.38, 1.0), (.44, -.26, 1.04), "dark")
    M.cyl((-.2, .15, .9), .2, .08, "yellow", sides=10).bx((-.25, .1, .9), (-.15, .2, .95), "dark")
    M.bx((.55, .1, .9), (.85, .3, .96), "rust")
    M.text((-.7, -.37, .78), F, .03, "WERKBANK 2", PAPER * .6)


def tools(M):
    M.bx((-.3, -.15, 0), (.3, .15, .02), "dark")
    M.bx((-.24, -.03, .02), (.0, -.01, .035), "steel").cyl((-.25, -.02, .025), .015, .025, "steel", sides=8, axis="z")
    M.bx((.05, -.1, .02), (.27, -.08, .04), "yellow").bx((.2, -.11, .02), (.28, -.07, .05), "steel")
    M.tube([(-.2, .08, .03), (.05, .08, .03)], .012, "steel", sides=5).bx((.05, .06, .02), (.09, .1, .05), "crimson")
    M.cyl((.2, .07, .02), .06, .03, "copper", sides=8)


def gauge(M):
    M.bx((-.15, .02, 0), (.15, .1, .3), "dark")
    M.cyl((0, -.04, .15), .06, .13, "metal", axis="y", sides=12)
    M.cyl((0, -.041, .15), .002, .11, "paper", axis="y", sides=12)
    M.bx((-.004, -.043, .15), (.004, -.042, .24), "crimson")
    M.text((0, -.043, .08), F, .018, "BAR", INK)


def container(M):
    M.pitch = 99
    M.bx((-1, -3, .08), (1, 3, 2.42), "rust")
    for i in range(12):
        y = -2.75 + i * .5
        M.bx((-1.02, y - .1, .2), (-1, y + .1, 2.3), "rust").bx((1, y - .1, .2), (1.02, y + .1, 2.3), "rust")
    for x in (-.95, .95):
        for y in (-2.95, 2.95):
            M.bx((x - .06, y - .06, 0), (x + .06, y + .06, 2.5), "dark")
    M.bx((-1, -3, 0), (1, 3, .1), "dark").bx((-1, -3, 2.4), (1, 3, 2.5), "dark")
    M.bx((-.02, -3.01, .15), (.02, -3, 2.35), "dark")
    for x in (-.45, .45):
        M.tube([(x, -3.03, .2), (x, -3.03, 2.3)], .018, "steel", sides=5)
        M.bx((x - .06, -3.04, 1.1), (x + .06, -3.01, 1.16), "yellow")
    M.text((-1.021, 0, 1.6), (-1, 0, 0), .32, "VK LOG", PAPER * .55, wear=.55).text((1.021, 0, 1.6), (1, 0, 0), .32, "VK LOG", PAPER * .55, wear=.55)
    M.code((-.55, -3.003, 1.7), F, .22, 7, PAPER)


def tool_shelf(M):
    M.pitch = 99
    for x in (-.28, .28):
        for y in (-.97, 0, .97):
            M.bx((x - .02, y - .02, 0), (x + .02, y + .02, 2.2), "dark")
    for z in (.12, .72, 1.32, 1.92):
        M.bx((-.3, -1, z), (.3, 1, z + .04), "plate")
    rng = np.random.default_rng(5)
    for z in (.16, .76, 1.36, 1.96):
        y = -.9
        while y < .75:
            w = float(rng.uniform(.14, .34))
            h = float(rng.uniform(.1, .34))
            col = ("yellow", "rust", "dark", "metal", "crimson")[int(rng.integers(5))]
            if rng.random() > .25:
                M.bx((-.2, y, z), (.2, y + w, z + h), col)
            y += w + .05
    M.text((-.301, 0, 2.1), (-1, 0, 0), .05, "WERKZEUG", PAPER * .6)


def recon_capsule(M):
    M.pitch = 99
    M.cyl((0, 0, 0), .16, .5, "plastic", sides=14)
    M.cyl((0, 0, 2.05), .15, .5, "plastic", sides=14)
    M.cyl((0, 0, .16), 1.89, .47, "glass", sides=14)
    for a in (.52, 2.62, 4.71):
        M.bx((.48 * math.cos(a) - .03, .48 * math.sin(a) - .03, .1), (.48 * math.cos(a) + .03, .48 * math.sin(a) + .03, 2.1), "steel")
    M.bx((-.25, -.51, .03), (.25, -.45, .14), "cyan")
    M.text((0, -.512, .085), F, .05, "R-H", PAPER, wear=.1)
    M.bx((.2, -.49, .95), (.4, -.44, 1.25), "dark").screen((.3, -.491, 1.1), F, (.18, .26), 5)
    M.tube([(.42, .25, 2.2), (.42, .25, 2.5)], .03, "cable", sides=5).tube([(-.42, .25, 2.2), (-.42, .25, 2.5)], .03, "cable", sides=5)
    M.tube([(0, .4, .14), (0, .55, .06), (0, .7, .02)], .04, "cable", sides=6)
    M.hazard((0, -.501, .015), F, (1.0, .03), pitch=.08)
    M.bx((-.1, -.49, 2.12), (.1, -.48, 2.16), "glow_teal")


# ------------------------------------------------------------ props: F2

def lab_chair(M):
    M.bx((-.25, -.25, .42), (.25, .25, .5), "mattress").bx((-.25, .2, .5), (.25, .27, 1.0), "mattress")
    M.tube([(0, 0, .08), (0, 0, .42)], .03, "chrome", sides=8)
    for a in np.linspace(0, 6.28, 6)[:-1]:
        M.tube([(0, 0, .08), (.28 * math.cos(a), .28 * math.sin(a), .03)], .015, "chrome", sides=5)
        M.cyl((.28 * math.cos(a), .28 * math.sin(a), 0), .03, .02, "black", sides=6)
    for x in (-.28, .28):
        M.tube([(x, .1, .5), (x, .1, .7), (x, -.15, .7)], .015, "chrome", sides=5)
        M.bx((x - .03, -.2, .69), (x + .03, -.02, .72), "black")
        M.bx((x - .04, -.08, .72), (x + .04, -.04, .725), "tape")
    M.bx((-.26, -.01, .6), (.26, .01, .63), "dark")


def capsule_shell(M, r, h, lid, base="plastic"):
    M.pitch = 99
    M.cyl((0, 0, 0), .18, r, base, sides=16).cyl((0, 0, h - .14), .14, r, lid, sides=16)
    M.cyl((0, 0, .18), h - .32, r * .95, "glass", sides=16)
    for a in (.4, 2.5, 4.6):
        M.bx((r * .97 * math.cos(a) - .03, r * .97 * math.sin(a) - .03, .1), (r * .97 * math.cos(a) + .03, r * .97 * math.sin(a) + .03, h - .1), "steel")
    for z in (.18, h - .18):
        M.cyl((0, 0, z - .015), .03, r * .98, "dark", sides=16)


def human_capsule(M):
    capsule_shell(M, .5, 2.2, "plastic")
    # Occupant: a blurred shape behind the fogged glass, never a face.
    M.tube([(0, .05, .35), (0, .05, 1.35)], .16, "skin", sides=8).cyl((0, .03, 1.4), .24, .1, "skin", sides=8)
    M.tube([(.2, .05, 1.3), (.25, .05, .7)], .045, "skin", sides=6).tube([(-.2, .05, 1.3), (-.25, .05, .7)], .045, "skin", sides=6)
    M.bx((.18, -.52, .95), (.42, -.45, 1.25), "dark").screen((.3, -.521, 1.1), F, (.22, .26), 2)
    M.tube([(0, .48, 1.0), (0, .6, .1)], .03, "cable", sides=5)
    vitalis(M, (0, -.502, .3), F, .3)
    M.text((0, -.502, .09), F, .07, "P-07", INK, wear=.2).bx((-.2, -.502, .14), (.2, -.5, .16), "tape")


def monster_capsule(M):
    capsule_shell(M, .7, 2.6, "metal", "metal")
    # Twisted shape with more limbs than it should have, chained at the wrists.
    M.tube([(0, 0, .3), (.08, 0, 1.1), (-.05, .05, 1.75)], .24, "skin", sides=8).cyl((-.05, .05, 1.75), .3, .15, "skin", sides=8)
    for a in (.6, 2.5, 3.8, 5.4):
        M.tube([(0, 0, 1.4), (.35 * math.cos(a), .35 * math.sin(a), 1.2), (.45 * math.cos(a), .45 * math.sin(a), .7)], .05, "skin", sides=5)
    for a in (1.0, 4.2):
        M.tube([(.66 * math.cos(a), .66 * math.sin(a), 2.3), (.45 * math.cos(a), .45 * math.sin(a), 1.5)], .012, "black", sides=4)
    M.text((0, -.702, .09), F, .08, "S-12", AMBER, wear=.3).hazard((0, -.703, .02), F, (1.4, .03), pitch=.1)
    M.bx((.3, -.72, 1.0), (.55, -.64, 1.3), "dark").screen((.42, -.721, 1.15), F, (.22, .26), 7)


def resonance(M):
    M.pitch = 99
    M.bx((-.6, -1.3, 0), (.6, 1.3, .2), "plastic")
    M.cyl((0, -1.2, .75), 2.4, .55, "glass", axis="y", sides=14)
    M.cyl((0, -1.28, .75), .14, .6, "plastic", axis="y", sides=14).cyl((0, 1.14, .75), .14, .6, "plastic", axis="y", sides=14)
    M.bx((-.2, -1, 1.28), (.2, 1, 1.4), "dark")
    for y in (-.8, -.25, .3, .85):
        M.cyl((0, y, .75), .08, .58, "copper", axis="y", sides=14)
    M.bx((-.3, -.5, .2), (.3, .9, .3), "mattress")
    M.bx((-.2, -1.31, .9), (.2, -1.28, 1.2), "dark").screen((0, -1.311, 1.05), F, (.34, .26), 9)
    M.text((0, -.61, .12), F, .055, "RESONANZ", INK, wear=.2).hazard((0, -1.301, .1), F, (1.2, .18))
    M.text((0, 0, 1.401), UP, .06, "R-07", PAPER)


def desk(M):
    M.pitch = 99
    M.bx((-.8, -.4, .73), (.8, .4, .79), "plastic")
    M.bx((.22, -.37, 0), (.78, .37, .73), "metal")
    for z in (.05, .28, .51):
        M.bx((.25, -.38, z), (.75, -.37, z + .2), "plate").bx((.42, -.39, z + .08), (.58, -.38, z + .11), "chrome")
    M.bx((-.78, -.35, 0), (-.72, .35, .73), "metal").bx((-.72, .3, .25), (.22, .34, .73), "metal")
    # Paper left behind, a stained mug, an old terminal.
    M.bx((-.5, -.2, .79), (-.2, .1, .8), "paper").bx((-.46, -.25, .8), (-.18, .03, .805), "paper")
    M.cyl((-.05, -.18, .79), .09, .04, "plastic", sides=8)
    M.bx((.2, .05, .79), (.62, .3, .83), "dark").bx((.25, .18, .83), (.57, .22, 1.12), "dark").screen((.41, .179, .98), F, (.3, .25), 4)


def console(M):
    M.pitch = 99
    M.profile([(-.5, 0), (.5, 0), (.5, 1.2), (-.5, 1.2)], -.15, .3, "plastic")
    M.profile([(-.5, .75), (.5, .75), (.5, .98), (-.5, .98)], -.3, -.15, "plastic")
    M.bx((-.42, -.31, .76), (.42, -.3, .96), "dark").lamps((0, -.311, .86), F, 10, 3, .07, 11)
    M.bx((-.42, -.151, .99), (.42, -.15, 1.17), "dark").screen((-.18, -.152, 1.08), F, (.4, .16), 12).screen((.24, -.152, 1.08), F, (.3, .16), 13)
    M.bx((-.42, -.31, 0), (.42, -.15, .06), "rubber")
    vitalis(M, (0, -.152, .5), F, .34)
    M.text((0, -.152, .36), F, .06, "STATION 3", INK, wear=.25).code((.3, -.152, .2), F, .12, 2)
    M.tube([(.45, .3, .3), (.5, .4, .1), (.55, .5, .02)], .02, "cable", sides=4)


def research_bed(M):
    M.pitch = 99
    M.bx((-.48, -1, .46), (.48, 1, .54), "steel")
    M.bx((-.45, -.97, .54), (.45, .97, .62), "mattress")
    M.bx((-.42, .55, .62), (.42, .94, .68), "plastic")
    for x in (-.42, .42):
        for y in (-.9, .9):
            M.tube([(x, y, .46), (x, y, .1)], .025, "chrome", sides=6)
            M.cyl((x - .03, y, .05), .06, .05, "black", axis="x", sides=8)
    # Restraint straps across chest and shins.
    for y in (-.45, .2):
        M.bx((-.47, y - .05, .535), (.47, y + .05, .63), "dark")
        M.bx((-.04, y - .03, .63), (.04, y + .03, .64), "steel")
    M.bx((-.42, -1.02, .44), (.42, -1, .6), "plastic")
    M.text((0, -1.021, .52), F, .04, "BETT 12", INK, wear=.2)


def coat_rack(M):
    M.cyl((0, 0, 0), .03, .24, "chrome", sides=10)
    M.tube([(0, 0, .03), (0, 0, 1.8)], .018, "chrome", sides=6)
    for a in (0, 1.57, 3.14, 4.71):
        M.tube([(0, 0, 1.68), (.13 * math.cos(a), .13 * math.sin(a), 1.76)], .01, "chrome", sides=4)
    # A lab coat that has been hanging a long time, one stain, one tag.
    M.profile([(-.22, .72), (.22, .72), (.2, 1.52), (.07, 1.66), (-.07, 1.66), (-.2, 1.52)], -.13, -.02, "coat")
    M.bx((-.02, -.135, .75), (.02, -.13, 1.6), "dark")
    M.bx((.07, -.135, 1.32), (.17, -.13, 1.38), "paper")
    M.bx((-.2, -.135, .95), (-.08, -.13, 1.04), "crimson")


def surgery_table(M):
    M.pitch = 99
    M.bx((-.4, -1, .82), (.4, 1, .88), "steel")
    M.bx((-.4, -1, .8), (.4, -.96, .9), "dark").bx((-.4, .96, .8), (.4, 1, .9), "dark")
    M.cyl((0, 0, 0), .05, .36, "dark", sides=12).tube([(0, 0, .05), (0, 0, .82)], .09, "chrome", sides=10)
    M.bx((-.25, -.6, .78), (.25, .6, .82), "dark")
    # Drain channel; a hose runs down to a bucket under the head end.
    M.bx((-.03, -.95, .88), (.03, .95, .883), "dark")
    M.tube([(0, .95, .85), (.05, 1.0, .5), (.1, .85, .1)], .012, "cable", sides=4)
    M.cyl((.1, .85, 0), .18, .1, "plastic", sides=10, r2=.12)
    for y in (-.35, .4):
        for x in (-.41, .41):
            M.bx((x - .015, y - .06, .74), (x + .015, y + .06, .9), "dark")


def surgical_tools(M):
    M.bx((-.3, -.2, .0), (.3, .2, .012), "chrome")
    M.bx((-.31, -.21, .0), (.31, -.2, .03), "chrome").bx((-.31, .2, 0), (.31, .21, .03), "chrome")
    M.bx((-.31, -.2, 0), (-.3, .2, .03), "chrome").bx((.3, -.2, 0), (.31, .2, .03), "chrome")
    M.tube([(-.22, -.1, .018), (-.02, -.1, .018)], .005, "chrome", sides=4).bx((-.26, -.112, .012), (-.22, -.088, .018), "chrome")
    M.tube([(-.22, .02, .018), (-.02, .05, .018)], .005, "chrome", sides=4).tube([(-.22, .05, .018), (-.02, .02, .018)], .005, "chrome", sides=4)
    M.tube([(.05, -.12, .018), (.22, -.12, .018)], .006, "chrome", sides=4).bx((.05, -.13, .012), (.08, -.11, .024), "dark")
    M.bx((.04, .02, .012), (.26, .12, .016), "crimson")
    M.bx((-.2, .1, .012), (-.05, .15, .018), "plastic")


def surgical_lamp(M):
    # Hangs from the top: the prefab sits it at height.
    M.cyl((0, 0, .0), .1, .4, "chrome", sides=16, r2=.34).cyl((0, 0, .1), .08, .34, "plastic", sides=16, r2=.18)
    for a in np.linspace(0, 6.28, 7)[:-1]:
        M.cyl((.24 * math.cos(a), .24 * math.sin(a), -.005), .01, .07, "glow_white", sides=8)
    M.cyl((0, 0, -.005), .01, .08, "glow_white", sides=8)
    M.tube([(0, 0, .18), (0, 0, .7)], .025, "chrome", sides=8)
    M.bx((-.06, -.06, .66), (.06, .06, .72), "dark")
    M.tube([(0, 0, .3), (.2, 0, .4), (.2, 0, .7)], .008, "cable", sides=4)


def screen_prop(M):
    M.pitch = 99
    for x in (-.88, 0, .88):
        M.tube([(x, 0, .08), (x, 0, 1.8)], .018, "chrome", sides=6)
        M.tube([(x, -.22, .03), (x, .22, .03)], .012, "chrome", sides=5)
        M.cyl((x, -.22, 0), .04, .025, "black", sides=6).cyl((x, .22, 0), .04, .025, "black", sides=6)
    # Translucent curtain panels, stained, held by tape where the rail gave up.
    for a, b in ((-.86, -.02), (.02, .86)):
        M.bx((a, -.006, .3), (b, .006, 1.78), "plastic")
    M.tube([(-.88, 0, 1.8), (.88, 0, 1.8)], .014, "chrome", sides=5)
    M.bx((-.5, -.009, 1.1), (-.3, -.006, 1.13), "tape")


def iv_cart(M):
    M.cyl((0, 0, .06), .02, .25, "chrome", sides=10)
    for a in np.linspace(0, 6.28, 6)[:-1]:
        M.cyl((.24 * math.cos(a), .24 * math.sin(a), 0), .06, .025, "black", sides=6)
    M.tube([(0, 0, .08), (0, 0, 1.75)], .015, "chrome", sides=6)
    M.tube([(-.14, 0, 1.72), (.14, 0, 1.72)], .008, "chrome", sides=4)
    M.bx((.07, -.04, 1.42), (.2, .04, 1.68), "plastic").bx((.09, -.041, 1.46), (.18, -.04, 1.6), "crimson")
    M.tube([(.135, 0, 1.42), (.15, -.04, 1.0), (.05, -.08, .6)], .004, "plastic", sides=3)
    M.bx((-.12, -.09, .95), (.09, .09, 1.11), "dark").screen((-.015, -.091, 1.03), F, (.18, .13), 6)


def filing_cabinet(M):
    M.pitch = 99
    M.bx((-.25, -.3, 0), (.25, .3, 1.4), "metal")
    for i in range(4):
        z = .06 + i * .335
        M.bx((-.23, -.31, z), (.23, -.3, z + .3), "plate").bx((-.08, -.32, z + .15), (.08, -.31, z + .18), "chrome")
        M.plate((0, -.312, z + .23), F, (.16, .05), PAPER).text((0, -.313, z + .23), F, .025, ("A-F", "G-L", "M-R", "S-Z")[i], INK)
    M.bx((-.23, -.46, .4), (.23, -.31, .68), "plate")
    M.bx((-.2, -.44, .68), (.2, -.32, .7), "paper")


def sample_fridge(M):
    M.pitch = 99
    M.bx((-.4, -.35, .08), (.4, .35, 2), "plastic")
    M.bx((-.42, -.37, 0), (.42, .37, .08), "dark")
    M.bx((-.36, -.36, .15), (.36, -.35, 1.4), "dark").bx((-.32, -.361, .2), (.32, -.36, 1.36), "glass")
    for z in (.4, .7, 1.0, 1.3):
        M.bx((-.3, -.3, z), (.3, .2, z + .02), "chrome")
        for k in range(5):
            M.cyl((-.22 + k * .11, -.2, z + .02), .11, .018, ("crimson", "paper", "teal", "crimson", "paper")[k], sides=6)
    M.bx((-.36, -.36, 1.45), (.36, -.35, 1.95), "plate").bx((.28, -.38, .6), (.31, -.36, 1.1), "chrome")
    vitalis(M, (.14, -.361, 1.8), F, .24)
    M.screen((-.12, -.361, 1.75), F, (.3, .14), 8).text((.12, -.361, 1.55), F, .04, "-20 C", INK)
    M.bx((-.36, -.362, .78), (-.1, -.36, .84), "tape")


def photo_table(M):
    # The final room's table and the impossible photograph (D-56). Three separate objects keep their own transforms.
    M.pitch = 99
    M.bx((-.25, -.25, .64), (.25, .25, .7), "wood")
    for x, y in ((-.21, -.21), (.21, -.21), (-.21, .21), (.21, .21)):
        M.bx((x - .025, y - .025, 0), (x + .025, y + .025, .64), "wood")
    M.bx((-.2, -.2, .3), (.2, .2, .32), "wood")
    M.bx((-.015, -.12, .71), (.015, .12, .72), "dark")


# ------------------------------------------------------------ props: F3

def cryo_tank(M):
    M.pitch = 99
    M.cyl((0, 0, 0), .2, .85, "machine", sides=18).cyl((0, 0, .2), 2.5, .8, "steel", sides=18).cyl((0, 0, 2.7), .3, .8, "machine2", sides=18, r2=.3)
    for z in (.5, 1.05, 2.15):
        M.cyl((0, 0, z), .1, .82, "machine", sides=18)
    # Inspection window with frost, status light, feed lines.
    M.bx((-.3, -.83, 1.2), (.3, -.74, 1.95), "machine2")
    M.bx((-.24, -.835, 1.26), (.24, -.83, 1.89), "frost")
    M.bx((.45, -.72, 1.0), (.6, -.68, 1.08), "glow_crimson")
    M.tube([(.8, 0, 2.4), (1.0, 0, 2.4), (1.0, 0, 0)], .05, "cable", sides=6).tube([(-.8, .1, 2.2), (-1.0, .1, 2.2), (-1.0, .1, 0)], .05, "cable", sides=6)
    M.text((0, -.83, 2.21), F, .09, "KRYO 12", PAPER, wear=.15).text((0, -.83, .3), F, .09, "KALT", AMBER, wear=.2)
    vitalis(M, (0, -.83, 2.48), F, .46)


def transformer(M):
    M.pitch = 99
    M.bx((-.75, -.75, 0), (.75, .75, .2), "machine2")
    M.bx((-.65, -.65, .2), (.65, .65, 2.0), "machine")
    for i in range(8):
        y = -.56 + i * .16
        M.bx((-.74, y - .03, .3), (-.65, y + .03, 1.9), "machine2").bx((.65, y - .03, .3), (.74, y + .03, 1.9), "machine2")
    for x in (-.35, .35):
        M.cyl((x, 0, 2.0), .05, .13, "dark", sides=10)
        for k in range(3):
            M.cyl((x, 0, 2.05 + k * .03), .02, .1 - k * .015, "copper", sides=10)
        M.cyl((x, 0, 2.14), .06, .05, "glow_crimson", sides=8)
    M.plate((0, -.651, 1.5), F, (.62, .34), AMBER).text((0, -.652, 1.57), F, .09, "GEFAHR").text((0, -.652, 1.43), F, .09, "TRAFO 1")
    M.hazard((0, -.751, .1), F, (1.5, .2)).code((0, -.651, .9), F, .16, 9, PAPER)
    M.tube([(0, .65, 1.2), (0, .85, 1.0), (0, .85, 0)], .05, "cable", sides=6)


def cooling_column(M):
    M.pitch = 99
    M.cyl((0, 0, 0), .3, .55, "machine2", sides=16).cyl((0, 0, .3), 5.4, .45, "steel", sides=16).cyl((0, 0, 5.7), .3, .55, "machine2", sides=16)
    for z in np.linspace(.6, 5.3, 6):
        M.cyl((0, 0, float(z)), .08, .48, "machine", sides=16)
    for a in (0, 2.09, 4.19):
        M.tube([(.47 * math.cos(a), .47 * math.sin(a), .3), (.47 * math.cos(a), .47 * math.sin(a), 5.7)], .035, "cable", sides=5)
    M.cyl((0, 0, 2.7), .1, .465, "glow_crimson", sides=16)
    M.text((0, -.46, 1.4), F, .1, "KUHLUNG", PAPER * .7, wear=.3)


def containment_console(M):
    M.pitch = 99
    M.bx((-.6, -.2, 0), (.6, .4, 1.2), "machine")
    M.profile([(-.6, .7), (.6, .7), (.6, .95), (-.6, .95)], -.4, -.2, "machine2")
    M.bx((-.5, -.41, .72), (.5, -.4, .93), "dark").lamps((0, -.411, .82), F, 12, 3, .07, 15)
    M.bx((-.5, -.201, .97), (.5, -.2, 1.17), "dark")
    M.screen((-.22, -.202, 1.07), F, (.46, .18), 14, np.array(hbp.COLORS["glow_crimson"], dtype=np.float32)).screen((.26, -.202, 1.07), F, (.4, .18), 16)
    M.bx((-.45, -.21, .1), (.45, -.2, .6), "machine2")
    vitalis(M, (0, -.211, .52), F, .32)
    M.text((0, -.211, .34), F, .06, "EINSCHLUSS", PAPER * .6, wear=.3).hazard((0, -.211, .17), F, (.9, .08), pitch=.1)
    M.bx((-.58, -.42, .96), (-.5, -.2, 1.0), "dark")
    M.tube([(.55, .4, .5), (.65, .55, .3), (.7, .7, .02)], .03, "cable", sides=5)


def broken_capsule(M):
    M.pitch = 99
    M.cyl((0, 0, 0), .16, .6, "machine", sides=14)
    M.cyl((0, 0, 2.05), .15, .6, "machine", sides=14)
    for i, a in enumerate((.3, 1.3, 2.3, 3.6, 4.5, 5.6)):
        h = .3 + (i * 37 % 9) * .13
        M.box((.56 * math.cos(a), .56 * math.sin(a), .16 + h / 2), (.3, .03, h), "glass", rot=a + 1.57)
    for a in (0, 3.14):
        M.bx((.58 * math.cos(a) - .03, .58 * math.sin(a) - .03, .1), (.58 * math.cos(a) + .03, .58 * math.sin(a) + .03, 2.1), "machine2")
    for x, y, r in ((.45, -.75, .4), (-.3, -.8, 1.4), (.1, -.65, 2.4)):
        M.box((x, y, .012), (.26, .2, .024), "glass", rot=r)
    M.tube([(0, 0, 2.05), (.2, -.1, 1.55), (.1, -.2, 1.2)], .03, "cable", sides=5)
    M.text((0, -.601, .08), F, .07, "B-3", AMBER, wear=.4)
    M.bx((.1, -.585, .06), (.5, -.57, .1), "crimson")


def pa_horn(M):
    M.bx((-.08, .1, .15), (.08, .35, .45), "machine2")
    M.cyl((0, .1, .3), .1, .12, "machine", axis="y", sides=10)
    M.cyl((0, -.35, .3), .45, .3, "machine", axis="y", sides=12, r2=.09)
    M.cyl((0, -.355, .3), .006, .27, "black", axis="y", sides=12)
    M.bx((-.3, .32, .05), (.3, .35, .55), "dark")


def heavy_cabling(M):
    for y, z, r in ((-.2, .32, .07), (0, .34, .08), (.2, .31, .065), (-.1, .2, .05), (.12, .2, .05)):
        M.tube([(-1, y, z), (-.5, y + .02, z - .05), (0, y - .02, z - .08), (.5, y + .01, z - .04), (1, y, z)], r, "cable", sides=6)
    for x in (-.7, .7):
        M.bx((x - .04, -.3, .36), (x + .04, .3, .4), "machine2")
    M.tube([(-1, .28, .36), (1, .28, .36)], .02, "crimson", sides=5)


def mixing_table(M):
    M.pitch = 99
    M.bx((-.8, -.4, 0), (.8, .4, .9), "machine")
    M.profile([(-.85, .88), (.85, .88), (.85, .96), (-.85, .96)], -.5, .4, "machine2")
    for i in range(10):
        x = -.65 + i * .145
        M.bx((x - .03, -.35, .96), (x + .03, .05, .985), "dark")
        y = -.3 + (i * 7 % 5) * .07
        M.bx((x - .022, y - .02, .985), (x + .022, y + .02, 1.03), "steel")
    M.bx((-.75, .15, .96), (.75, .3, 1.32), "machine2")
    M.bx((-.5, .148, 1.08), (-.1, .15, 1.28), "dark").screen((-.3, .147, 1.18), F, (.38, .18), 3)
    M.bx((.15, .148, 1.08), (.55, .15, 1.28), "dark").screen((.35, .147, 1.18), F, (.38, .18), 6, np.array(hbp.COLORS["glow_crimson"], dtype=np.float32))
    for x in (-.6, .68):
        M.cyl((x, .148, 1.2), .03, .05, "steel", axis="y", sides=10)
    M.text((0, -.401, .75), F, .05, "MISCHPULT 1", PAPER * .9, wear=.25).code((.55, -.401, .4), F, .14, 2, PAPER)
    M.lamps((-.45, -.44, .961), UP, 6, 2, .04, 8)
    M.tube([(-.6, .4, .3), (-.7, .5, .1), (-.75, .7, .02)], .02, "cable", sides=5).tube([(.6, .4, .3), (.7, .5, .1), (.75, .7, .02)], .02, "cable", sides=5)


def catwalk(M):
    # Room deck over the core: grating plus the frame it hangs from.
    M.pitch = 99
    M.slab((-3, -1, -.1), (3, 1, .1), "grate", face="top")
    M.core((-3.01, -1.01, -.08), (3.01, 1.01, .08))
    M.bx((-3, -1, -.1), (3, -.94, .1), "machine2").bx((-3, .94, -.1), (3, 1, .1), "machine2")
    for x in (-2.4, -.8, .8, 2.4):
        M.bx((x - .04, -1, -.2), (x + .04, 1, -.1), "machine2")
    for y in (-.96, .96):
        M.tube([(-3, y, 1.0), (3, y, 1.0)], .02, "steel", sides=5).tube([(-3, y, .55), (3, y, .55)], .012, "steel", sides=4)
        for x in (-3, -1.5, 0, 1.5, 3):
            M.tube([(x, y, .1), (x, y, 1.02)], .02, "steel", sides=5)


def mezzanine(M):
    M.pitch = 99
    M.slab((-1, -1.5, -.1), (1, 1.5, .1), "plate", face="top")
    M.core((-1.01, -1.51, -.08), (1.01, 1.51, .08))
    M.bx((-1, -1.5, -.1), (1, -1.44, .1), "yellow").bx((-1, 1.44, -.1), (1, 1.5, .1), "yellow")
    for y in (-1, 0, 1):
        M.bx((-1, y - .04, -.22), (1, y + .04, -.1), "dark")


# ------------------------------------------------------------ stairs and floor exits, per floor

def _steps(M, tread, riser):
    """Same footprint as the ramp it dresses: Unity x 0..2, z 0..4, up to 1.5 m. The ramp keeps the collider."""
    n = 10
    for k in range(n):
        y0, y1 = -k * .4, -(k + 1) * .4
        z = (k + 1) * .15
        M.bx((-2, y1, 0), (0, y0, z), riser)
        M.bx((-2, y1, z - .02), (0, y0 + .02, z), tread)


def _rails(M, col):
    for x in (-1.97, -.03):
        M.tube([(x, -.05, 1.0), (x, -4, 2.5)], .02, col, sides=5)
        for k in range(5):
            y = -.2 - k * .9
            base = -y * .375
            M.tube([(x, y, base), (x, y, base + .98)], .016, col, sides=4)


def stairs_f1(M):
    M.pitch = 99
    _steps(M, "plate", "dark")
    _rails(M, "yellow")


def stairs_f2(M):
    M.pitch = 99
    _steps(M, "vinyl", "tile")
    _rails(M, "chrome")


def stairs_f3(M):
    M.pitch = 99
    _steps(M, "grate", "machine")
    _rails(M, "machine2")
    for k in range(10):
        M.bx((-1.98, -(k + 1) * .4 + .01, (k + 1) * .15 - .03), (-.02, -(k + 1) * .4 + .02, (k + 1) * .15 - .02), "glow_crimson")


def _exit_shell(M, col, trim):
    M.bx((-1.1, -.15, 0), (-.95, .15, 2.4), col).bx((.95, -.15, 0), (1.1, .15, 2.4), col).bx((-1.1, -.15, 2.2), (1.1, .15, 2.6), col)
    M.bx((-.95, .1, 0), (.95, .15, 2.2), "black")
    M.bx((-1.12, -.17, 2.2), (1.12, -.15, 2.3), trim)


def exit_f1(M):
    M.pitch = 99
    _exit_shell(M, "plate", "yellow")
    M.hazard((0, -.171, 2.25), F, (2.2, .09), pitch=.2)
    M.text((0, -.151, 2.44), F, .12, "ABWARTS", PAPER * .7, wear=.3)
    M.bx((-1.05, -.2, 1.0), (-.98, -.15, 1.32), "dark").screen((-1.015, -.201, 1.16), F, (.06, .28), 2)


def exit_f2(M):
    M.pitch = 99
    _exit_shell(M, "plastic", "rubber")
    M.bx((-.4, -.17, 2.34), (.4, -.15, 2.54), "glow_green")
    M.text((0, -.171, 2.44), F, .1, "LIFT", INK)
    M.bx((-.95, -.05, 0), (-.9, .05, 2.2), "chrome").bx((.9, -.05, 0), (.95, .05, 2.2), "chrome")


def exit_f3(M):
    M.pitch = 99
    _exit_shell(M, "machine", "machine2")
    M.bx((-.5, -.171, 2.42), (.5, -.15, 2.46), "glow_crimson")
    for x in (-1.03, 1.03):
        M.tube([(x, -.2, 0), (x, -.2, 2.2)], .04, "cable", sides=6)
    M.text((0, -.171, 2.26), F, .08, "KERN", PAPER * .6, wear=.2)


# ------------------------------------------------------------ one-offs: the final room's photograph and the intro's sets

def photo_frame(M):
    M.pitch = 99
    M.bx((-.12, -.015, -.15), (.12, .015, .15), "dark")
    M.bx((-.095, -.017, -.125), (.095, -.015, .125), "paper")
    # Family of three; every face is the same blur, which is the wrong part of the photograph (D-56).
    for x, h, r in ((-.05, .05, .02), (.0, .02, .016), (.05, .05, .02)):
        M.bx((x - r, -.018, -.12), (x + r, -.017, h - .03), "cloth")
        M.cyl((x, -.018, h), .001, r * .9, "skin", axis="y", sides=8)
        M.cyl((x, -.0192, h), .0008, r * .55, "pink", axis="y", sides=6)
    M.tube([(0, .015, .05), (0, .08, -.15)], .006, "dark", sides=4)


def intro_table(M):
    M.bx((-.7, -.4, .69), (.7, .4, .75), "wood")
    for x, y in ((-.64, -.34), (.64, -.34), (-.64, .34), (.64, .34)):
        M.bx((x - .03, y - .03, 0), (x + .03, y + .03, .69), "brown")
    M.bx((-.62, -.02, .55), (.62, .02, .61), "brown")
    M.cyl((.42, .15, .75), .08, .045, "white", sides=8)
    M.bx((-.3, -.2, .75), (-.02, .05, .76), "paper")


def intro_chair(M):
    M.bx((-.22, -.22, .42), (.22, .22, .48), "wood")
    M.bx((-.22, .18, .48), (.22, .22, .9), "wood")
    M.bx((-.2, .2, .6), (.2, .23, .78), "cloth")
    for x, y in ((-.19, -.19), (.19, -.19), (-.19, .19), (.19, .19)):
        M.bx((x - .02, y - .02, 0), (x + .02, y + .02, .42), "brown")


def intro_photo(M):
    M.bx((-.09, -.01, -.12), (.09, .01, .12), "brown")
    M.bx((-.075, -.012, -.1), (.075, -.01, .1), "paper")
    M.bx((-.05, -.013, -.08), (.05, -.012, .02), "cloth")
    M.cyl((0, -.013, .05), .001, .025, "skin", axis="y", sides=8)


def intro_hatch(M):
    """Vitalis service hatch: sealed, corporate, its logo in crimson."""
    M.pitch = 99
    M.bx((-1, -.1, -1.1), (1, .1, 1.1), "metal")
    for y, n in ((-.1, -1), (.1, 1)):
        a, b = sorted((y, y + .02 * n))
        M.bx((-.015, a, -1.1), (.015, b, 1.1), "dark")
        for z in (-.8, .8):
            M.bx((-.92, a, z - .05), (.92, b, z + .05), "plate")
        v = F if n < 0 else BACK
        M.hazard((0, y + .021 * n, -1.0), v, (1.8, .16), pitch=.2)
        M.text((0, y + .021 * n, .35), v, .16, "VITALIS", CYAN, wear=.2)
        M.text((0, y + .021 * n, .1), v, .07, "SEKTOR V-7", PAPER * .7, wear=.3)


def intro_lift_door(M):
    M.pitch = 99
    M.bx((-1, -.1, -1.1), (1, .1, 1.1), "steel")
    for y, n in ((-.1, -1), (.1, 1)):
        a, b = sorted((y, y + .012 * n))
        M.bx((-.01, a, -1.1), (.01, b, 1.1), "black")
        for x in (-.7, .7):
            M.bx((x - .18, a, -.95), (x + .18, b, .95), "metal")
        v = F if n < 0 else BACK
        M.text((.7, y + .013 * n, .55), v, .07, "LIFT 3", PAPER * .7, wear=.4)


def wall_panel(M):
    """Facility keypad: dark box, green screen. Faces +Z in Unity; the box sizes match the panels it replaces."""
    M.bx((-.15, -.04, -.2), (.15, .04, .2), "dark")
    M.bx((-.12, -.042, .02), (.12, -.04, .17), "black").screen((0, -.043, .095), F, (.22, .13), 7)
    M.lamps((0, -.043, -.09), F, 3, 3, .055, 4)


# ------------------------------------------------------------ CRT monitors: green phosphor is the facility's information

def _tube_back(M, y0, y1, x, hw0, hh0, hw1, hh1, zc, col):
    """The tapering back of a tube monitor, closed at both ends."""
    a = [M.bm.verts.new((x + u, y0, zc + v)) for u, v in ((-hw0, -hh0), (hw0, -hh0), (hw0, hh0), (-hw0, hh0))]
    b = [M.bm.verts.new((x + u, y1, zc + v)) for u, v in ((-hw1, -hh1), (hw1, -hh1), (hw1, hh1), (-hw1, hh1))]
    for i in range(4):
        j = (i + 1) % 4
        M._face((a[i], a[j], b[j], b[i]), col)
    M._face(list(reversed(a)), col)
    M._face(b, col)


def crt_unit(M, x, z, w, h, col, seed, screen=None):
    """One monitor. x: centre, z: its bottom, w/h: front size. The screen faces -Y."""
    zc = z + h / 2
    M.bx((x - w / 2, -.2, z), (x + w / 2, -.1, z + h), col)
    _tube_back(M, -.1, .24, x, w * .46, h * .46, w * .28, h * .28, zc, col)
    M.bx((x - w * .4, -.206, zc - h * .34), (x + w * .4, -.2, zc + h * .38), "black")
    M.screen((x, -.207, zc + h * .02), F, (w * .74, h * .66), seed, screen if screen is not None else GREEN)
    M.bx((x - w * .4, -.204, z + h * .03), (x + w * .4, -.2, z + h * .13), "dark")
    for k in range(3):
        M.cyl((x + w * (.1 + k * .11), -.215, z + h * .08), .012, .012, "steel", sides=6, axis="y")
    M.bx((x - w * .05, -.21, z + h * .05), (x + w * .02, -.204, z + h * .11), "glow_green")


def crt_f1(M):
    """Maintenance terminal: dented steel, a service sticker."""
    M.pitch = 99
    M.bx((-.14, -.16, 0), (.14, .14, .03), "dark").bx((-.05, -.06, .03), (.05, .06, .07), "dark")
    crt_unit(M, 0, .07, .42, .36, "metal", 21)
    M.plate((-.1, -.2005, .405), F, (.12, .018), AMBER)
    M.tube([(.05, .24, .2), (.09, .32, .06), (.12, .34, .01)], .012, "cable", sides=4)


def crt_f2(M):
    """Clinic monitor: yellowed plastic, a strip of tape, the Vitalis label."""
    M.pitch = 99
    M.bx((-.16, -.17, 0), (.16, .15, .025), "plastic").cyl((0, 0, .025), .05, .09, "plastic", sides=8)
    crt_unit(M, 0, .075, .46, .38, "plastic", 22)
    vitalis(M, (-.13, -.2005, .1), F, .13)
    M.bx((-.2, -.203, .428), (-.06, -.2, .445), "tape")
    M.tube([(-.05, .24, .2), (-.09, .32, .06), (-.12, .34, .01)], .012, "cable", sides=4)


def crt_bank(M):
    """Core monitor bank: a rack of black tubes, one showing a red alarm. Stands on the floor against a wall."""
    M.pitch = 99
    M.bx((-.78, -.18, 0), (-.74, .28, 1.74), "machine2").bx((.74, -.18, 0), (.78, .28, 1.74), "machine2")
    M.bx((-.74, .24, 0), (.74, .28, 1.74), "machine")
    for z in (0, .56, 1.14, 1.7):
        M.bx((-.74, -.18, z), (.74, .24, z + .04), "machine2")
    red = np.array(hbp.COLORS["glow_crimson"], dtype=np.float32)
    for row, z in enumerate((.04, .6, 1.18)):
        for col, x in enumerate((-.37, .37)):
            crt_unit(M, x, z, .68, .5, "machine", 30 + row * 2 + col, red if (row, col) == (1, 1) else None)
    M.tube([(.6, .28, 1.7), (.66, .36, .9), (.7, .38, .02)], .03, "cable", sides=5)


# ------------------------------------------------------------ the elevator Lukas arrived in: dead behind him on floors 2 and 3 (D-158)

def _elevator(M, frame, door, gap):
    """Mounted on a wall face: back at y=0, front toward -Y, pivot on the floor at its centre. gap: half opening between the jammed doors."""
    M.pitch = 99
    M.core((-1.3, -.1, 0), (1.3, .03, 2.9))
    # The shaft behind the doors stays black: nothing is coming.
    M.bx((-1.15, -.16, .02), (1.15, -.1, 2.45), "black")
    for s_ in (-1, 1):
        a, b = sorted((s_ * 1.15, s_ * 1.4))
        M.bx((a, -.3, 0), (b, 0, 2.95), frame)
        a, b = sorted((s_ * gap, s_ * 1.15))
        M.bx((a, -.22, .02), (b, -.16, 2.45), door)
        for z in (.5, 1.25, 2.0):
            M.bx((a + .05, -.225, z - .02), (b - .05, -.22, z + .02), "dark")
    M.bx((-1.15, -.3, 2.45), (1.15, 0, 2.95), frame)
    M.bx((-1.15, -.3, -.01), (1.15, 0, .02), "steel")
    # Dead floor indicator: every lamp off.
    M.bx((-.5, -.32, 2.58), (.5, -.3, 2.84), "black")
    for i in range(4):
        M.cyl((-.36 + i * .24, -.33, 2.71), .012, .05, "dark", sides=8, axis="y")


def _call_panel(M, x, seed):
    """Call panel with its cover hanging open and the wiring pulled out."""
    M.bx((x - .13, -.32, 1.0), (x + .13, -.3, 1.5), "dark")
    M.bx((x - .1, -.325, 1.28), (x + .1, -.32, 1.44), "black")
    M.tube([(x - .05, -.33, 1.05), (x - .08, -.38, .85), (x - .03, -.36, .62)], .009, "crimson", sides=4)
    M.tube([(x + .04, -.33, 1.05), (x + .07, -.37, .9), (x + .1, -.35, .7)], .009, "yellow", sides=4)
    M.code((x, -.326, 1.14), F, .1, seed, PAPER * .5)


def elevator_f2(M):
    """Clinic lift: corporate, sealed from the far side, a taped notice and nobody coming to read it."""
    _elevator(M, "plastic", "steel", .004)
    M.bx((-.004, -.225, .02), (.004, -.16, 2.45), "dark")
    M.bx((-1.15, -.227, 1.5), (1.15, -.222, 1.56), "cyan")
    vitalis(M, (0, -.301, 2.52), F, .6)
    _call_panel(M, 1.27, 12)
    # Crossed tape and the notice.
    M.tube([(-1.05, -.235, .55), (1.05, -.235, 2.2)], .012, "tape", sides=4)
    M.tube([(-1.05, -.24, 2.2), (1.05, -.24, .55)], .012, "tape", sides=4)
    M.bx((-.34, -.25, 1.05), (.34, -.246, 1.5), "paper")
    M.text((0, -.2505, 1.36), F, .05, "AUSSER", INK)
    M.text((0, -.2505, 1.2), F, .05, "BETRIEB", INK)
    M.bx((-.12, -.252, 1.48), (.12, -.25, 1.53), "tape")


def elevator_f3(M):
    """Core freight lift: blast doors wrenched a hand apart and jammed, a dead red alarm, cables torn out of the header."""
    _elevator(M, "machine2", "machine", .1)
    M.hazard((0, -.301, 2.55), F, (2.3, .12), pitch=.2)
    M.hazard((0, -.223, .25), F, (2.3, .3), pitch=.26)
    _call_panel(M, 1.27, 13)
    # A steel bar jammed across the gap: it will not open further, or close.
    M.tube([(-.45, -.28, .95), (.45, -.29, 1.2)], .03, "rust", sides=6)
    M.text((-.62, -.223, 1.75), F, .05, "GESPERRT", PAPER * .6, wear=.3)
    M.bx((.5, -.32, 2.62), (.72, -.3, 2.78), "glow_crimson")
    for x in (-.7, -.2, .4):
        M.tube([(x, -.15, 2.46), (x + .05, -.2, 2.2), (x - .03, -.18, 1.95)], .02, "cable", sides=5)


def elevator_f1(M):
    """Maintenance lift of the floor 1 final room (C-21, C-37): the one that still works, so its floor lamp is lit amber."""
    _elevator(M, "plate", "steel", .004)
    M.bx((-.004, -.225, .02), (.004, -.16, 2.45), "dark")
    M.hazard((0, -.301, 2.55), F, (2.3, .12), pitch=.2)
    M.hazard((0, -.301, .05), F, (2.3, .1), pitch=.2)
    M.bx((-1.15, -.227, 1.2), (1.15, -.222, 1.26), "yellow")
    M.tube([(-1.15, -.33, 2.9), (1.15, -.33, 2.9)], .036, "rust", sides=6)
    M.text((-.62, -.223, 1.75), F, .05, "ABTEILUNG B", PAPER * .6, wear=.4)
    M.bx((-.14, -.335, 2.63), (.14, -.33, 2.79), "bulb")   # this floor's lamp: the lift is here
    _call_panel(M, 1.27, 11)


# ------------------------------------------------------------ pickups per GDD Objetos y Coleccionables

def item_heal(M):
    """Dirty white rigid case with a handle, cyan trim and the Vitalis mark; the handle is the silhouette."""
    M.pitch = 99
    M.bx((-.15, -.045, 0), (.15, .045, .15), "plastic")
    M.bx((-.155, -.05, .06), (.155, .05, .09), "cyan")
    for x in (-.055, .055):
        M.bx((x - .012, -.012, .15), (x + .012, .012, .185), "dark")
    M.bx((-.07, -.014, .178), (.07, .014, .2), "dark")
    for x in (-.11, .11):
        M.bx((x - .015, -.052, .05), (x + .015, -.05, .1), "steel")
    vitalis(M, (-.06, -.0455, .12), F, .12)
    M.text((0, -.0455, .03), F, .008, "REGENERATIONSMITTEL", INK)
    M.code((.0, .0455, .12), BACK, .04, 3)


def item_ammo(M):
    """Clear blister of 4 copper caseless rounds on an aluminium lid with the Vitalis stock code."""
    M.bx((-.065, -.045, 0), (.065, .045, .006), "chrome")
    for x in (-.045, -.015, .015, .045):
        M.cyl((x, -.03, .015), .06, .009, "copper", sides=6, axis="y")
    # The clear blister is only a rim: an opaque-looking pane would hide the rounds the GDD wants seen.
    for x0, x1 in ((-.065, -.061), (.061, .065)):
        M.bx((x0, -.045, .006), (x1, .045, .012), "steel")
    M.text((0, -.0455, .003), F, .004, "VK-MUN-4", INK)


def item_spike(M):
    """Matte black cylinder as long as a forearm: rubber grip, metal tip, amber band, green charge LED."""
    M.cyl((0, -.2, .03), .3, .02, "black", axis="y", sides=8).cyl((0, .1, .03), .1, .024, "rubber", axis="y", sides=8)
    M.cyl((0, -.25, .03), .05, .004, "steel", axis="y", sides=6, r2=.02).cyl((0, .0, .03), .03, .021, "yellow", axis="y", sides=8)
    M.bx((.019, .05, .024), (.023, .062, .036), "glow_green")
    M.text((0, -.1, .05), UP, .009, "KESSLER WERKSCHUTZ", PAPER * .7)


def item_key_mandatory(M):
    """Large plate: cyan band and a stair icon. Shape and icon tell it apart, not only the colour."""
    M.bx((-.08, -.05, 0), (.08, .05, .012), "plastic")
    M.bx((-.08, .015, .012), (.08, .045, .014), "cyan")
    for i in range(3):
        M.bx((-.05 + i * .018, -.035, .012), (-.034 + i * .018, -.017 + i * .012, .014), "dark")
    M.text((.03, .03, .0145), UP, .012, "R-H", PAPER).code((.05, -.02, .0125), UP, .028, 7)


def item_key_optional(M):
    """Standard magnetic card with a grey band."""
    M.bx((-.045, -.0275, 0), (.045, .0275, .004), "plastic")
    M.bx((-.045, .009, .004), (.045, .023, .005), "metal")
    vitalis(M, (-.018, -.012, .0042), UP, .036)
    M.code((.03, -.012, .0042), UP, .018, 5)


# ------------------------------------------------------------ catalogue: name -> (floor, tile px, builder, output folder)

def post_for(floor, kind):
    return {("F1", "wall"): post_f1_wall, ("F1", "floor"): post_f1_floor, ("F1", "ceiling"): post_f1_ceiling, ("F2", "wall"): post_f2_wall, ("F2", "floor"): post_f2_floor, ("F2", "ceiling"): post_f2_ceiling, ("F3", "wall"): post_f3_wall, ("F3", "floor"): post_f3_floor, ("F3", "ceiling"): post_f3_ceiling}.get((floor, kind))


CATALOG = {}


def _add(floor, name, tile, fn, post=None, folder=None):
    CATALOG[name] = dict(floor=floor, tile=tile, fn=fn, post=post, folder=folder)


for f, wall, window, doorway, floor_, ceil_, col, frame, leaf, lamp in (
    ("F1", wall_f1, window_f1, doorway_f1, floor_f1, ceiling_f1, column_f1, frame_f1, leaf_f1, lamp_f1),
    ("F2", wall_f2, window_f2, doorway_f2, floor_f2, ceiling_f2, column_f2, frame_f2, leaf_f2, lamp_f2),
    ("F3", wall_f3, window_f3, doorway_f3, floor_f3, ceiling_f3, column_f3, frame_f3, leaf_f3, lamp_f3),
):
    _add(f, f"KIT_{f}_Wall_2m", 512, wall, post_for(f, "wall"))
    _add(f, f"KIT_{f}_Wall_2m_High", 640, wall_high(wall), post_for(f, "wall"))
    _add(f, f"KIT_{f}_WallWindow_2m", 512, window, post_for(f, "wall"))
    _add(f, f"KIT_{f}_Doorway_2m", 256, doorway, post_for(f, "wall"))
    _add(f, f"KIT_{f}_Floor_2x2", 384, floor_, post_for(f, "floor"))
    _add(f, f"KIT_{f}_Ceiling_2x2", 256, ceil_, post_for(f, "ceiling"))
    _add(f, f"KIT_{f}_Column", 128, col, post_for(f, "wall"))
    _add(f, f"Door_{f}_Frame", 192, frame)
    _add(f, f"Door_{f}_Leaf", 192 if f == "F2" else 320, leaf, post_for(f, "wall"))
    _add(f, f"CH_Lamp_{f}", 64, lamp)

for name, tile, fn in (
    ("PROP_Shared_Speaker", 96, speaker), ("PROP_Shared_CableBundle", 64, cable_bundle), ("PROP_Shared_Pipe_2m", 128, pipe2m), ("PROP_Shared_Pipe_Elbow", 128, pipe_elbow),
    ("PROP_Shared_TrashPile", 128, trash), ("PROP_Shared_MetalCrate", 256, crate), ("PROP_Shared_MetalCrate_Half", 128, lambda M: crate(M, .5)), ("PROP_Shared_VitalisPoster", 192, poster),
    ("PROP_Shared_Grate", 96, grate), ("PROP_Shared_Puddle", 64, puddle), ("PROP_Shared_HackPanel", 192, hack_panel), ("CH_INT_PowerSwitch", 192, power_switch), ("CH_INT_Lever", 48, switch_lever),
    ("KIT_Shared_Platform_2x2", 256, platform), ("KIT_Shared_Ramp_2x4", 256, ramp), ("CH_Env_HostileCamera", 96, hostile_cam_env), ("CH_Env_MotherExit", 192, exit_door), ("CH_Env_Mezzanine", 256, mezzanine), ("CH_Env_Catwalk", 320, catwalk),
):
    _add("Shared", name, tile, fn)

for name, tile, fn, folder in (
    ("CH_F1_Generator", 448, generator, "Props"), ("CH_F1_Boiler", 448, boiler, "Props"), ("CH_F1_Forklift", 448, forklift, "Props"), ("CH_F1_GantryCrane", 320, gantry, "Props"), ("CH_F1_WaterPump", 256, pump, "Props"), ("CH_Capsule_Reconstruction", 320, recon_capsule, "Props"),
    ("PROP_F1_WaterTank", 320, water_tank, None), ("PROP_F1_PressureValve", 64, valve, None), ("PROP_F1_ThickPipe", 192, thick_pipe, None), ("PROP_F1_Workbench", 256, workbench, None), ("PROP_F1_Tools", 64, tools, None),
    ("PROP_F1_Gauge", 64, gauge, None), ("PROP_F1_BigContainer", 448, container, None), ("PROP_F1_ToolShelf", 256, tool_shelf, None), ("PROP_F1_PersonalTray", 96, tray, None),
):
    _add("F1", name, tile, fn, folder=folder)

for name, tile, fn, folder in (
    ("CH_F2_HumanCapsule", 256, human_capsule, "Props"), ("CH_F2_MonsterCapsule", 320, monster_capsule, "Props"), ("CH_F2_ResonanceCapsule", 256, resonance, "Props"),
    ("PROP_F2_LabChair", 128, lab_chair, None), ("PROP_F2_Desk", 192, desk, None), ("PROP_F2_Console", 192, console, None), ("PROP_F2_ResearchBed", 192, research_bed, None), ("PROP_F2_LabCoatRack", 128, coat_rack, None),
    ("PROP_F2_SurgeryTable", 192, surgery_table, None), ("PROP_F2_SurgicalTools", 64, surgical_tools, None), ("PROP_F2_SurgicalLamp", 128, surgical_lamp, None), ("PROP_F2_Screen", 192, screen_prop, None),
    ("PROP_F2_IVCart", 128, iv_cart, None), ("PROP_F2_FilingCabinet", 192, filing_cabinet, None), ("PROP_F2_SampleFridge", 256, sample_fridge, None), ("CH_Env_PhotoTable", 96, photo_table, None),
):
    _add("F2", name, tile, fn, folder=folder)

for name, tile, fn, folder in (
    ("CH_F3_CryoTank", 384, cryo_tank, "Props"), ("CH_F3_Transformer", 384, transformer, "Props"), ("CH_F3_BrokenCapsule", 256, broken_capsule, "Props"), ("CH_F3_MixingTable", 320, mixing_table, "Props"),
    ("PROP_F3_CoolingColumn", 320, cooling_column, None), ("PROP_F3_ContainmentConsole", 192, containment_console, None), ("PROP_F3_PAHorn", 96, pa_horn, None), ("PROP_F3_HeavyCabling", 96, heavy_cabling, None),
):
    _add("F3", name, tile, fn, folder=folder)

_add("F1", "PROP_F1_CRT", 128, crt_f1)
_add("F2", "PROP_F2_CRT", 192, crt_f2)
_add("F3", "PROP_F3_CRTBank", 320, crt_bank)
_add("F1", "CH_Elevator_F1", 320, elevator_f1)
_add("F2", "CH_Elevator_F2", 320, elevator_f2)
_add("F3", "CH_Elevator_F3", 320, elevator_f3)
for name, tile, fn in (("CH_Item_Heal", 256, item_heal), ("CH_Item_Ammo", 96, item_ammo), ("CH_Item_Spike", 128, item_spike), ("CH_Item_KeyMandatory", 128, item_key_mandatory), ("CH_Item_KeyOptional", 128, item_key_optional)):
    _add("Shared", name, tile, fn, folder="Props")

for f, st, ex in (("F1", stairs_f1, exit_f1), ("F2", stairs_f2, exit_f2), ("F3", stairs_f3, exit_f3)):
    _add(f, f"CH_Stairs_{f}", 256, st)
    _add(f, f"CH_Exit_{f}", 192, ex)
_add("F2", "CH_Env_PhotoFrame", 64, photo_frame)
for name, tile, fn in (("CH_Intro_Table", 128, intro_table), ("CH_Intro_Chair", 96, intro_chair), ("CH_Intro_Photo", 48, intro_photo), ("CH_Intro_Hatch", 256, intro_hatch), ("CH_Intro_LiftDoor", 256, intro_lift_door), ("CH_Intro_Panel", 64, wall_panel)):
    _add("Shared", name, tile, fn)

# Budgets from the owner (2026-11-04): small 3k, medium 5k, large 10k triangles. Kit pieces repeat by the dozen, so far lower.
KIT_LIMIT, PROP_LIMIT = 400, 5000


def audit(me):
    """What makes light leak or surfaces flicker. Closed shells share every edge between exactly two faces, wound opposite."""
    bm = bmesh.new()
    bm.from_mesh(me)
    opened = sum(1 for e in bm.edges if len(e.link_faces) == 1)
    degenerate = sum(1 for f in bm.faces if f.calc_area() < 1e-9)
    nonmanifold = sum(1 for e in bm.edges if len(e.link_faces) > 2)
    bm.free()
    return opened, degenerate, nonmanifold


def _export(ob, folder, name):
    os.makedirs(folder, exist_ok=True)
    vl = bpy.context.view_layer
    vl.update()
    for o in bpy.context.scene.objects:
        o.select_set(False)
    ob.select_set(True)
    vl.objects.active = ob
    with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
        bpy.ops.export_scene.fbx(filepath=folder + rf"\{name}.fbx", use_selection=True, object_types={"MESH"}, apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", bake_space_transform=True, bake_anim=False, mesh_smooth_type="FACE")


def add_to_atlas(name):
    """Builds one new model into the free space after its floor's existing atlas, leaving every other model and texel as it
    is (C-37). A later full build() packs it with the rest. Reads and rewrites T_Env_<floor>.png and its emission map."""
    d = CATALOG[name]
    floor, tile = d["floor"], d["tile"]
    x, y, row = 16, 0, 0
    for n, j in sorted(((n, j) for n, j in CATALOG.items() if j["floor"] == floor and n != name), key=lambda j: -j[1]["tile"]):
        if x + j["tile"] > ATLAS:
            x, y, row = 0, y + row, 0
        x += j["tile"]
        row = max(row, j["tile"])
    if x + tile > ATLAS:
        x, y = 0, y + row
    assert y + tile <= ATLAS, f"{floor} atlas full at {name}"
    bpy.ops.wm.read_homefile(use_empty=True)
    col = bpy.data.collections.new(floor)
    bpy.context.scene.collection.children.link(col)
    tex_dir = ENV + r"\Textures"
    maps = {}
    for key, file in (("albedo", f"T_Env_{floor}.png"), ("emission", f"T_Env_{floor}_Emission.png")):
        im = bpy.data.images.load(tex_dir + "\\" + file)
        a = np.empty(im.size[0] * im.size[1] * 4, dtype=np.float32)
        im.pixels.foreach_get(a)
        maps[key] = a.reshape(im.size[1], im.size[0], 4)
        bpy.data.images.remove(im)
    M = Env(name, floor, d["post"])
    d["fn"](M)
    ob, tris = M.finish(col)
    B, c, e = M.paint(ob.data, tile, (x, y))
    maps["albedo"][B.py, B.px, :3] = c
    maps["emission"][B.py, B.px, :3] = e
    uv = ob.data.uv_layers.active.data
    for i, p in enumerate(ob.data.polygons):
        if M.parts[i] == "hidden":
            for li in p.loop_indices:
                uv[li].uv = (.001, .001)
    _export(ob, MODELS + "\\" + d["folder"] if d["folder"] else ENV + "\\" + floor, name)
    hp.save(tex_dir + rf"\T_Env_{floor}.png", "T_Env_" + floor, maps["albedo"])
    hp.save(tex_dir + rf"\T_Env_{floor}_Emission.png", "T_Env_" + floor + "_Emission", maps["emission"])
    opened, degenerate, nonman = audit(ob.data)
    return dict(name=name, tile=(x, y, tile), tris=tris, open=opened, degenerate=degenerate)


def build(floors=("F1", "F2", "F3", "Shared")):
    report = {}
    for floor in floors:
        bpy.ops.wm.read_homefile(use_empty=True)
        col = bpy.data.collections.new(floor)
        bpy.context.scene.collection.children.link(col)
        tex_dir = ENV + r"\Textures"
        os.makedirs(tex_dir, exist_ok=True)
        albedo = np.zeros((ATLAS, ATLAS, 4), dtype=np.float32)
        emission = np.zeros((ATLAS, ATLAS, 4), dtype=np.float32)
        albedo[:, :, 3] = emission[:, :, 3] = 1
        albedo[:, :, :3] = np.array(hbp.COLORS["hidden"], dtype=np.float32)   # UV (0,0): what "hidden" faces sample
        jobs = sorted(((n, d) for n, d in CATALOG.items() if d["floor"] == floor), key=lambda j: -j[1]["tile"])
        x, y, row = 16, 0, 0   # first texels stay dark for hidden faces
        for name, d in jobs:
            tile = d["tile"]
            if x + tile > ATLAS:
                x, y, row = 0, y + row, 0
            assert y + tile <= ATLAS, f"{floor} atlas full at {name}"
            M = Env(name, floor, d["post"])
            d["fn"](M)
            ob, tris = M.finish(col)
            B, c, e = M.paint(ob.data, tile, (x, y))
            albedo[B.py, B.px, :3] = c
            emission[B.py, B.px, :3] = e
            x += tile
            row = max(row, tile)
            uv = ob.data.uv_layers.active.data
            for i, p in enumerate(ob.data.polygons):
                if M.parts[i] == "hidden":
                    for li in p.loop_indices:
                        uv[li].uv = (.001, .001)
            folder = MODELS + "\\" + d["folder"] if d["folder"] else ENV + "\\" + floor
            os.makedirs(folder, exist_ok=True)
            vl = bpy.context.view_layer
            vl.update()
            for o in bpy.context.scene.objects:
                o.select_set(False)
            ob.select_set(True)
            vl.objects.active = ob
            with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
                bpy.ops.export_scene.fbx(filepath=folder + rf"\{name}.fbx", use_selection=True, object_types={"MESH"}, apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", bake_space_transform=True, bake_anim=False, mesh_smooth_type="FACE")
            opened, degenerate, nonman = audit(ob.data)
            report[name] = dict(tris=tris, open=opened, degenerate=degenerate)
        hp.save(tex_dir + rf"\T_Env_{floor}.png", "T_Env_" + floor, albedo)
        hp.save(tex_dir + rf"\T_Env_{floor}_Emission.png", "T_Env_" + floor + "_Emission", emission)
        report["_atlas_" + floor] = f"used to row y={y + row} of {ATLAS}"
    return report
