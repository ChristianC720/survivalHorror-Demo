"""Renders review sheets into Blender3DAssets/Review (not versioned)."""
import os

import bpy
import numpy as np
from mathutils import Vector


class Stage:
    def __init__(self, scale=2.4, center_z=1.0, size=(300, 400)):
        sc = bpy.context.scene
        self.sc = sc
        self.size = size
        self.center_z = center_z
        cd = bpy.data.cameras.new("c")
        cd.type = "ORTHO"
        cd.ortho_scale = scale
        self.cam = bpy.data.objects.new("c", cd)
        sc.collection.objects.link(self.cam)
        sc.camera = self.cam
        sd = bpy.data.lights.new("s", "SUN")
        sd.energy = 3
        sun = bpy.data.objects.new("s", sd)
        sc.collection.objects.link(sun)
        sun.rotation_euler = (1.0, 0.3, 0.4)
        w = bpy.data.worlds.new("w")
        w.use_nodes = True
        w.node_tree.nodes["Background"].inputs[0].default_value = (.5, .5, .5, 1)
        w.node_tree.nodes["Background"].inputs[1].default_value = .6
        sc.world = w
        sc.render.engine = "BLENDER_EEVEE"
        sc.render.image_settings.file_format = "PNG"
        sc.render.resolution_percentage = 100
        sc.render.resolution_x, sc.render.resolution_y = size

    def tile(self, view, tmp):
        p = Vector((view[0] * 4, view[1] * 4, self.center_z + view[2] * 4))
        d = Vector((0, 0, self.center_z)) - p
        self.cam.location = p
        self.cam.rotation_euler = d.to_track_quat("-Z", "Y").to_euler()
        self.sc.render.filepath = tmp
        bpy.ops.render.render(write_still=True)
        im = bpy.data.images.load(tmp)
        w, h = self.size
        a = np.array(im.pixels[:], dtype=np.float32).reshape(h, w, 4)
        bpy.data.images.remove(im)
        return a


def sheet(tiles, cols, path, size):
    w, h = size
    rows = (len(tiles) + cols - 1) // cols
    M = np.ones((rows * h, cols * w, 4), dtype=np.float32)
    for i, a in enumerate(tiles):
        r = rows - 1 - i // cols
        c = i % cols
        M[r * h:(r + 1) * h, c * w:(c + 1) * w] = a
    os.makedirs(os.path.dirname(path), exist_ok=True)
    s = bpy.data.images.new("sheet", cols * w, rows * h)
    s.pixels.foreach_set(M.ravel())
    s.filepath_raw = path
    s.file_format = "PNG"
    s.save()


FRONT, BACK, RIGHT, Q34 = (0, -1, 0), (0, 1, 0), (-1, 0, 0), (-0.6, -0.8, 0.1)
