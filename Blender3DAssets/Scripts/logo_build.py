"""Game logos, built only from the game's own material: VT323, Vitalis cyan and the monitor's heartbeat, in the font's pixel grid.
The name comes apart into pixels at its end (dissolution). Run in the background on an empty file:
    blender -b --factory-startup -P logo_build.py
Writes Logo.blend and two transparent PNGs (horizontal, square) to Blender3DAssets/Logo. Same seed, same logo."""
import os
import random

import bmesh
import bpy
from mathutils import Vector
from mathutils.bvhtree import BVHTree

ROOT = r"C:\Users\Chris\Heartbeat"
OUT = ROOT + r"\Blender3DAssets\Logo"
FONT = ROOT + r"\Assets\_Project\UI\Fonts\VT323-Regular.ttf"
CYAN = (.098, .769, .863)        # sRGB, Vitalis cyan
PALE = (.91, .97, .98)           # the monitor's trace
DEEP = (.043, .165, .188)        # monitor panel
SHADOW = (.016, .05, .06)        # keeps the letters readable on any background
FADE = (.06, .44, .50)           # cyan going out
ECG = [(0, 7), (3, 7), (4, 8), (5, 8), (6, 7), (8, 7), (9, 12), (10, 2), (11, 7), (13, 7), (14, 9), (15, 9), (16, 7), (20, 7)]
PANEL = (23, 16)                 # monitor size, in cells
FONT_PIXEL = 4                   # sub-cells per pixel of VT323, as sampled
DISSOLVE_FROM = .84              # share of the word that stays whole


def linear(c):
    return tuple(v / 12.92 if v <= .04045 else ((v + .055) / 1.055) ** 2.4 for v in c)


def material(name, colour):
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    m.use_nodes = True
    nodes = m.node_tree.nodes
    nodes.clear()
    e, o = nodes.new("ShaderNodeEmission"), nodes.new("ShaderNodeOutputMaterial")
    e.inputs["Color"].default_value = (*linear(colour), 1)
    m.node_tree.links.new(e.outputs[0], o.inputs[0])
    return m


def glyph_cells(line):
    """(col, row) of each font pixel of a line of text, bottom-left at 0,0."""
    curve = bpy.data.curves.new("t", "FONT")
    curve.body = line
    curve.font = bpy.data.fonts.load(FONT, check_existing=True)
    ob = bpy.data.objects.new("t", curve)
    bpy.context.scene.collection.objects.link(ob)
    bpy.context.view_layer.update()
    dg = bpy.context.evaluated_depsgraph_get()
    ev = ob.evaluated_get(dg)
    me = ev.to_mesh()
    vs = [v.co.copy() for v in me.vertices]
    bvh = BVHTree.FromPolygons(vs, [list(p.vertices) for p in me.polygons])
    xs = sorted({round(v.x, 4) for v in vs})
    gaps = [b - a for a, b in zip(xs, xs[1:]) if b - a > 1e-3]
    u = min(gaps)
    x0, y0, x1, y1 = min(v.x for v in vs), min(v.y for v in vs), max(v.x for v in vs), max(v.y for v in vs)
    cols, rows = round((x1 - x0) / u), round((y1 - y0) / u)
    cells = []
    for c in range(cols):
        for r in range(rows):
            p = Vector((x0 + (c + .5) * u, y0 + (r + .5) * u, 1))
            if bvh.ray_cast(p, Vector((0, 0, -1)))[0]:
                cells.append((c, r))
    ev.to_mesh_clear()
    bpy.data.objects.remove(ob)
    bpy.data.curves.remove(curve)
    return cells, cols, rows


def ecg_cells():
    out = set()
    for (x0, y0), (x1, y1) in zip(ECG, ECG[1:]):
        steps = max(abs(x1 - x0), abs(y1 - y0))
        for i in range(steps + 1):
            out.add((round(x0 + (x1 - x0) * i / steps), round(y0 + (y1 - y0) * i / steps)))
    return out


class Sheet:
    """Square tiles of one logo, in one mesh with one material per colour."""

    def __init__(self, name, collection):
        self.bm, self.name, self.collection = bmesh.new(), name, collection
        self.colours = []

    def tile(self, x, y, size, colour, z=0.0):
        if colour not in self.colours:
            self.colours.append(colour)
        s = size * 1.01       # a hair of overlap: no seams between neighbours
        vs = [self.bm.verts.new((x + a * s, y + b * s, z)) for a, b in ((0, 0), (1, 0), (1, 1), (0, 1))]
        self.bm.faces.new(vs).material_index = self.colours.index(colour)

    def monitor(self, x, y, cell):
        w, h = PANEL
        self.tile_rect(x, y, w * cell, h * cell, DEEP, -.02)
        for c in range(w):
            for r in (0, h - 1):
                self.tile(x + c * cell, y + r * cell, cell, CYAN)
        for r in range(h):
            for c in (0, w - 1):
                self.tile(x + c * cell, y + r * cell, cell, CYAN)
        for c, r in ecg_cells():
            self.tile(x + (c + 1) * cell, y + r * cell, cell, PALE)

    def tile_rect(self, x, y, w, h, colour, z):
        if colour not in self.colours:
            self.colours.append(colour)
        vs = [self.bm.verts.new(p) for p in ((x, y, z), (x + w, y, z), (x + w, y + h, z), (x, y + h, z))]
        self.bm.faces.new(vs).material_index = self.colours.index(colour)

    def word(self, cells, x, y, cell, cols, rows, dissolve=False):
        """Whole font pixels come off, not specks: only the last letters, and the word stays readable."""
        rng = random.Random(7)
        fate = {}
        for c, r in sorted(cells):
            px, py, size, colour = x + c * cell, y + r * cell, cell, CYAN
            t = (c / cols - DISSOLVE_FROM) / (1 - DISSOLVE_FROM)
            if dissolve and t > 0:
                key = (c // FONT_PIXEL, r // FONT_PIXEL)
                if key not in fate:
                    roll = rng.random()
                    fate[key] = None if roll < t * .10 else (
                        (rng.randint(1, 2 + int(t * 5)) * FONT_PIXEL, rng.randint(-1, 2 + int(t * 2)) * FONT_PIXEL, FADE if rng.random() < t else CYAN)
                        if roll < t * .42 else (0, 0, CYAN))
                if fate[key] is None:
                    continue
                dx, dy, colour = fate[key]
                px += dx * cell
                py += dy * cell
            self.tile(px + cell * .45, py - cell * .45, size, SHADOW, -.01)
            self.tile(px, py, size, colour)

    def finish(self):
        me = bpy.data.meshes.new(self.name)
        self.bm.to_mesh(me)
        self.bm.free()
        for c in self.colours:
            me.materials.append(material("M_Logo_%02x%02x%02x" % tuple(int(v * 255) for v in c), c))
        ob = bpy.data.objects.new(self.name, me)
        self.collection.objects.link(ob)
        return ob


def lines():
    """IDENTITY is set larger so both words have the same width, as one block."""
    a, ac, ar = glyph_cells("IDENTITY")
    b, bc, br = glyph_cells("DISSOLUTION")
    return (a, ac, ar), (b, bc, br)


def word_block(sheet, x, y, width, top, bottom):
    (a, ac, ar), (b, bc, br) = top, bottom
    cb, ca = width / bc, width / ac
    sheet.word(b, x, y, cb, bc, br, dissolve=True)
    gap = cb * 3
    sheet.word(a, x, y + br * cb + gap, ca, ac, ar)
    return br * cb + gap + ar * ca


def camera(name, ob, collection, px_long, square):
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    lo, hi = Vector(min(c[i] for c in corners) for i in range(3)), Vector(max(c[i] for c in corners) for i in range(3))
    margin = .03 * max(hi.x - lo.x, hi.y - lo.y)
    w, h = hi.x - lo.x + margin * 2, hi.y - lo.y + margin * 2
    if square:
        w = h = max(w, h)
    cam = bpy.data.objects.new(name, bpy.data.cameras.new(name))
    collection.objects.link(cam)
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = max(w, h)
    cam.location = ((lo.x + hi.x) / 2, (lo.y + hi.y) / 2, 5)
    return cam, (px_long, round(px_long * h / w))


def render(cam, size, collection, others, path):
    scene = bpy.context.scene
    scene.camera = cam
    scene.render.resolution_x, scene.render.resolution_y = size
    collection.hide_render = False
    for o in others:
        o.hide_render = True
    scene.render.filepath = path
    bpy.ops.render.render(write_still=True)


def main():
    bpy.ops.wm.read_factory_settings(use_empty=True)
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.film_transparent = True
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = "PNG"
    scene.render.image_settings.color_mode = "RGBA"
    scene.display_settings.display_device = "sRGB"
    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    top, bottom = lines()

    horizontal = bpy.data.collections.new("logo_horizontal")
    scene.collection.children.link(horizontal)
    sheet = Sheet("LOGO_Horizontal", horizontal)
    cell = 1.0
    # Text first, to learn its height: the monitor matches it and sits to its left.
    probe = Sheet("probe", horizontal)
    height = word_block(probe, 0, 0, 90 * cell, top, bottom)
    probe.bm.free()
    mc = height / PANEL[1]
    word_block(sheet, PANEL[0] * mc + 3 * cell, 0, 90 * cell, top, bottom)
    sheet.monitor(0, 0, mc)
    ob_h = sheet.finish()

    square = bpy.data.collections.new("logo_cuadrado")
    scene.collection.children.link(square)
    sheet = Sheet("LOGO_Cuadrado", square)
    height = word_block(sheet, 0, 0, 90 * cell, top, bottom)
    mc = 2.4 * cell
    sheet.monitor((90 * cell - PANEL[0] * mc) / 2, height + 5 * cell, mc)
    ob_s = sheet.finish()
    bpy.context.view_layer.update()

    cam_h, size_h = camera("Cam_Horizontal", ob_h, horizontal, 2000, False)
    cam_s, size_s = camera("Cam_Cuadrado", ob_s, square, 2048, True)
    os.makedirs(OUT, exist_ok=True)
    render(cam_h, size_h, horizontal, [square], os.path.join(OUT, "identity-dissolution-logo-horizontal.png"))
    render(cam_s, size_s, square, [horizontal], os.path.join(OUT, "identity-dissolution-logo-cuadrado.png"))
    square.hide_render = False
    bpy.ops.wm.save_as_mainfile(filepath=os.path.join(OUT, "Logo.blend"))
    print("LOGOS", size_h, size_s, len(top[0]), len(bottom[0]), top[1:], bottom[1:])


main()
