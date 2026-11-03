"""Static posed bodies baked from the Mudo build: corpses, the intro decoy, Lukas turned into a Mudo.
The same sculpt as the Mudo at a lower budget, painted per variant; the pose is frozen into the mesh, so no rig or Animator is needed. Call make()."""
import importlib
import os
import sys

import bpy
import numpy as np

ROOT = r"C:\Users\Chris\Heartbeat"
sys.path.insert(0, ROOT + r"\Blender3DAssets\Scripts")
import build_enemy as be
import enemy_paint as ep
import enemy_sculpt as es
import hb_anim
import hb_monster as hm
import hb_paint as hp

importlib.reload(hb_anim)
X = hp.hex_col
from hb_anim import Author

OUT = ROOT + r"\Assets\_Project\Models\Statics"

FALLEN = {"Hips": (-84, 0, 10), "Chest": (6, 0, 0), "Head": (4, 0, -24), "LeftUpperArm": (-10, -56, 0), "RightUpperArm": (-8, 34, 0), "LeftLowerArm": (-24, 0, 0), "RightLowerArm": (-10, 0, 0), "LeftUpperLeg": (-20, -8, 0), "RightUpperLeg": (-2, 8, 0), "LeftLowerLeg": (32, 0, 0), "RightLowerLeg": (4, 0, 0), "LeftFoot": (28, 0, 0), "RightFoot": (24, 0, 0), "loc": (0, 0.3, -.84)}
STILL = {"Neck": (10, 0, 2), "Head": (20, 6, 0), "LeftShoulder": (4, 0, 0), "RightShoulder": (6, 0, 0)}

def _ramp(k, tint=(1, 1, 1)):
    return [c * k * np.array(tint, dtype=np.float32) for c in ep.MUDO_SKIN]


# name: (pose, shirt, trousers, skin ramp, back stencil, chest tag)
VARIANTS = {
    "CH_Corpse_Staff": (FALLEN, ep.STAFF, ep.STAFF_DK, _ramp(.8), "VITALIS", "VK-0233"),
    "CH_Corpse_Guard": (FALLEN, ep.GUARD, ep.GUARD_DK, _ramp(.78), "WERKSCHUTZ", "KS-118"),
    "CH_Corpse_Subject": (FALLEN, ep.GOWN, ep.GOWN * .7, _ramp(.82), "SUBJEKT", "SUBJ 12"),
    "CH_Mudo_Still": (STILL, ep.STAFF, ep.STAFF_DK, _ramp(.95), "VITALIS", "VK-0417"),
    "CH_Lukas_Mudo": (STILL, X("#355654"), X("#1a2527"), _ramp(.72, (.9, .86, 1)), "R-H-07", "R-H-07"),
}


def make():
    os.makedirs(OUT + r"\Textures", exist_ok=True)
    report = {}
    for name, (pose, shirt, trousers, ramp, label, tag) in VARIANTS.items():
        bpy.ops.wm.read_homefile(use_empty=True)
        col = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(col)
        spec = es.mudo(target=3500, claws=name == "CH_Mudo_Still")
        t = es.bones("Mudo")
        hz = t["Head"][0][2]
        rig = hm.make_rig("Rig", t, col)
        report[name] = be.bind_and_paint(name, spec, rig, col, OUT + r"\Textures", lambda B: ep.mudo(B, hz, shirt, trousers, ramp, label, tag))
        ob = bpy.data.objects[name]
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
    bpy.ops.wm.read_homefile(use_empty=True)
    return report
