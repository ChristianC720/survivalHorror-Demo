"""Static posed bodies baked from the Mudo build: corpses, the intro decoy, Lukas turned into a Mudo.
Colour rules per variant; the pose is frozen into the mesh, so no rig or Animator is needed. Call make()."""
import importlib
import os
import sys

import bpy

ROOT = r"C:\Users\Chris\Heartbeat"
sys.path.insert(0, ROOT + r"\Blender3DAssets\Scripts")
import hb_anim
import hb_monster as hm

importlib.reload(hm)
importlib.reload(hb_anim)
from hb_anim import Author

OUT = ROOT + r"\Assets\_Project\Models\Statics"

FALLEN = {"Hips": (-84, 0, 10), "Chest": (6, 0, 0), "Head": (4, 0, -24), "LeftUpperArm": (-10, -56, 0), "RightUpperArm": (-8, 34, 0), "LeftLowerArm": (-24, 0, 0), "RightLowerArm": (-10, 0, 0), "LeftUpperLeg": (-20, -8, 0), "RightUpperLeg": (-2, 8, 0), "LeftLowerLeg": (32, 0, 0), "RightLowerLeg": (4, 0, 0), "LeftFoot": (28, 0, 0), "RightFoot": (24, 0, 0), "loc": (0, 0.3, -.84)}
STILL = {"Neck": (10, 0, 2), "Head": (20, 6, 0), "LeftShoulder": (4, 0, 0), "RightShoulder": (6, 0, 0)}

# name: (pose, shirt, trousers, skin)
VARIANTS = {
    "CH_Corpse_Staff": (FALLEN, (.30, .30, .30), (.20, .20, .21), (.44, .40, .38)),
    "CH_Corpse_Guard": (FALLEN, (.10, .12, .14), (.09, .10, .12), (.42, .37, .40)),
    "CH_Corpse_Subject": (FALLEN, (.42, .44, .46), (.30, .31, .33), (.46, .41, .38)),
    "CH_Mudo_Still": (STILL, (.30, .30, .30), (.20, .20, .21), (.50, .46, .43)),
    "CH_Lukas_Mudo": (STILL, (.21, .34, .33), (.21, .34, .33), (.40, .34, .40)),
}


def make():
    os.makedirs(OUT + r"\Textures", exist_ok=True)
    ns = {}
    report = {}
    for name, (pose, shirt, trousers, skin) in VARIANTS.items():
        bpy.ops.wm.read_homefile(use_empty=True)
        col = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(col)
        t = hm.humanoid_bones(hip=.96, spine=.5, neck=.11, head=.24, shoulder=.19, upper_arm=.3, lower_arm=.27, hand=.1, finger=.09, hip_w=.10, upper_leg=.46, lower_leg=.44, foot=.2)
        r = dict(hips=(.14, .10), waist=(.115, .085), ribs=(.15, .105), chest=(.165, .11), neck=(.05, .05), shoulder=(.055, .055), elbow=(.04, .04), wrist=(.032, .03), palm=(.042, .02), thigh=(.08, .08), knee=(.052, .055), ankle=(.04, .042), foot=(.048, .06), toe=(.042, .028), finger=(.014, .014))
        skin_data = hm.skin_parts(name, hm.body_chains(t, r), col, 2200)
        hz = t["Head"][0][2]

        def extras(bm, faces, rng):
            hm.blob(bm, (0, 0, hz + .12), .088, .105, .135, "skin", faces)
            hm.shell(bm, lambda c: 1.0 < c.z < 1.5 or (.72 < c.z < 1.5 and abs(c.x) > .2), .011, "shirt", .06, rng, faces)
            bm.normal_update()
            hm.shell(bm, lambda c: .1 < c.z < 1.02 and abs(c.x) < .2, .009, "trousers", .06, rng, faces)

        def color(c, n, part):
            k = .8 + .2 * max(0, n.z)
            base = {"shirt": shirt, "trousers": trousers}.get(part, skin)
            return tuple(x * k for x in base)

        rig = hm.make_rig("Rig", t, col)
        ob, parts, tris = hm.assemble(name, rig, skin_data, extras, col)
        hm.auto_weights(ob, rig)
        im = hm.paint(ob, parts, color, OUT + rf"\Textures\{name}_Albedo.png")
        hm.finish_material(ob, im)
        A = Author(rig)
        A.clip("pose", 2, {1: pose})
        bpy.context.scene.frame_set(1)
        bpy.context.view_layer.update()
        dg = bpy.context.evaluated_depsgraph_get()
        baked = bpy.data.meshes.new_from_object(ob.evaluated_get(dg))
        st = bpy.data.objects.new(name + "_baked", baked)
        col.objects.link(st)
        vl = bpy.context.view_layer
        for o in vl.objects:
            o.select_set(False)
        st.select_set(True)
        vl.objects.active = st
        with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
            bpy.ops.export_scene.fbx(filepath=OUT + rf"\{name}.fbx", use_selection=True, object_types={"MESH"}, apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", bake_space_transform=True, bake_anim=False, mesh_smooth_type="FACE")
        report[name] = tris
    bpy.ops.wm.read_homefile(use_empty=True)
    return report
