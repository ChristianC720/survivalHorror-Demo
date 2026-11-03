"""Builds one enemy: run with ENEMY set to Alto, Mudo, Boca or Rastrero. Writes Monsters/<Name>/<Name>.blend."""
import importlib
import os
import sys

import bpy
import numpy as np

ROOT = r"C:\Users\Chris\Heartbeat\Blender3DAssets"
sys.path.insert(0, ROOT + r"\Scripts")
import enemy_paint as ep
import enemy_sculpt as es
import hb_anatomy as ha
import hb_monster as hm
import hb_paint as hp
import hb_sculpt as hs

for _m in (hm, hp, hs, ha, ep, es):
    importlib.reload(_m)


def bind_and_paint(mesh_name, spec, rig, col, tex, painter):
    """Sculpt to finished skinned mesh: closed surface, hard parts, weights, 1024 px bake. Returns the triangle count."""
    V = spec.V
    V.freeze()
    verts, quads = hs.surface_nets(V)
    data = hs.to_mesh(mesh_name, verts, quads, spec.target, spec.keep, col)
    ob, parts, n = hm.assemble(mesh_name, rig, [(data, 0)], spec.hard, col)
    me = ob.data
    co = np.array([v.co[:] for v in me.vertices], dtype=np.float32)
    # Arm or torso comes from which sculpt the vertex is closer to, not from its distance to a bone.
    limb = np.array([x.value for x in me.attributes["limb"].data])
    organic = np.zeros(len(co), dtype=bool)
    organic[hm.verts_of_parts(ob, parts, ("skin",))] = True
    arm = V.sample(V.armd, co) < V.sample(V.bodyd, co)
    limb[organic] = np.where(arm, np.where(co[:, 0] > 0, 1, 2), 0)[organic]
    me.attributes["limb"].data.foreach_set("value", limb)
    hm.auto_weights(ob, rig, spec.over(co, (ob, parts)))
    B = hp.bake(me, parts, hm.TEX, align_longest=True)
    hs.bake_fields(B, V)
    path = tex + rf"\{mesh_name}_Albedo.png"
    im = hp.save(path, mesh_name + "_Albedo", hp.write(path, mesh_name + "_Albedo", hm.TEX, [(B, painter(B))]))
    hm.finish_material(ob, im)
    return n


def build(name):
    path = ROOT + rf"\Monsters\{name}\{name}.blend"
    bpy.ops.wm.open_mainfile(filepath=path)
    for ob in [o for o in bpy.data.objects if o.type in ("MESH", "ARMATURE")]:
        bpy.data.objects.remove(ob)
    for a in list(bpy.data.actions):
        bpy.data.actions.remove(a)
    col = bpy.data.collections.get("CH_" + name) or bpy.data.collections.new("CH_" + name)
    if col.name not in bpy.context.scene.collection.children:
        bpy.context.scene.collection.children.link(col)
    t = es.bones(name)
    hz = t["Head"][0][2]
    tex = ROOT + rf"\Monsters\{name}\Texture"
    extra_bones = {}
    if name == "Boca":
        extra_bones = {"JawL": ((.03, -.02, hz), (.12, -.01, hz + .28), "Head"), "JawR": ((-.03, -.02, hz), (-.12, -.01, hz + .28), "Head")}
    rig = hm.make_rig("Rig_" + name, t, col, extra_bones)
    os.makedirs(tex, exist_ok=True)
    tris = {}

    def finish(mesh_name, spec, painter):
        tris[mesh_name] = bind_and_paint(mesh_name, spec, rig, col, tex, painter)

    finish("CH_" + name, es.SCULPT[name](), lambda B: getattr(ep, name.lower())(B, hz))
    finish(f"CH_{name}_Recycled", es.SCULPT[name](recycled=True), lambda B: ep.recycled(name, B, hz))
    bpy.context.scene.render.fps = 30
    bpy.ops.wm.save_mainfile()
    return tris, []
