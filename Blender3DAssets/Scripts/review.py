"""Review sheets for the Signalis pass: neutral light plus a dark flashlight view close to the game's emergency rooms."""
import os

import bpy
import numpy as np
from mathutils import Vector

import preview

OUT = r"C:\Users\Chris\Heartbeat\Blender3DAssets\Review"


def dark_tile(stage, view, tmp, center):
    """Hard warm spot from the camera side, near-black world, low internal resolution like the game's ~540p."""
    sc = bpy.context.scene
    bg = sc.world.node_tree.nodes["Background"]
    keep = bg.inputs[1].default_value
    sun = bpy.data.objects.get("s")
    sun.hide_render = True
    bg.inputs[1].default_value = .012
    d = bpy.data.lights.new("spot", "SPOT")
    d.energy = 2600
    d.spot_size = .62
    d.spot_blend = .12
    d.color = (1, .93, .78)
    d.use_shadow = True
    ob = bpy.data.objects.new("spot", d)
    sc.collection.objects.link(ob)
    p = Vector((view[0] * 4 + .9, view[1] * 4, center + 1.3))
    ob.location = p
    ob.rotation_euler = (Vector((0, 0, center)) - p).to_track_quat("-Z", "Y").to_euler()
    a = stage.tile(view, tmp)
    bpy.data.objects.remove(ob)
    bpy.data.lights.remove(d)
    bg.inputs[1].default_value = keep
    sun.hide_render = False
    return a


def sheet(name, objects, scale, center, frame=None):
    """Shows only `objects` (plus their rigs): front, 3/4, back, then the dark view."""
    sc = bpy.context.scene
    for o in bpy.data.objects:
        if o.type == "MESH":
            o.hide_render = o not in objects
    st = preview.Stage(scale=scale, center_z=center, size=(420, 560))
    if frame is not None:
        sc.frame_set(frame)
    tmp = os.path.join(OUT, "tmp.png")
    tiles = [st.tile(v, tmp) for v in (preview.FRONT, preview.Q34, preview.BACK)]
    tiles.append(dark_tile(st, preview.Q34, tmp, center))
    path = os.path.join(OUT, name + ".png")
    preview.sheet(tiles, 4, path, (420, 560))
    for extra in ("c", "s"):
        o = bpy.data.objects.get(extra)
        if o:
            bpy.data.objects.remove(o)
    return path


def props_sheet(name, names, cols=6, size=(280, 280)):
    """One orthographic 3/4 tile per model, framed on its bounds."""
    sc = bpy.context.scene
    st = preview.Stage(scale=1, center_z=0, size=size)
    tmp = os.path.join(OUT, "tmp.png")
    tiles = []
    for n in names:
        ob = bpy.data.objects[n]
        for o in bpy.data.objects:
            if o.type == "MESH":
                o.hide_render = o is not ob
        pts = [Vector(v) for v in ob.bound_box]
        lo, hi = Vector([min(p[i] for p in pts) for i in range(3)]), Vector([max(p[i] for p in pts) for i in range(3)])
        st.cam.data.ortho_scale = max((hi - lo).length * 1.08, .05)
        mid = (lo + hi) / 2
        view = Vector((-.6, -.8, .45)).normalized()
        st.cam.location = mid + view * 12
        st.cam.rotation_euler = (-view).to_track_quat("-Z", "Y").to_euler()
        st.cam.data.clip_end = 40
        sc.render.filepath = tmp
        bpy.ops.render.render(write_still=True)
        im = bpy.data.images.load(tmp)
        tiles.append(np.array(im.pixels[:], dtype=np.float32).reshape(size[1], size[0], 4))
        bpy.data.images.remove(im)
    path = os.path.join(OUT, name + ".png")
    preview.sheet(tiles, cols, path, size)
    return path
