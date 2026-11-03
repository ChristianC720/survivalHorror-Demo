"""Builds organic low-poly creatures on the shared humanoid skeleton (fallback of BLENDER_PLAN.md section 2).

hb_sculpt.py makes the closed organic body; this module has the skeleton, the hard parts, the weights and the assembly.
hb_paint.py then paints it texel by texel. Blender axes: facing -Y, +X is the character's left.
"""
import math
import random

import bmesh
import bpy
import numpy as np
from mathutils import Vector

TEX = 1024


def mirror(p):
    return (-p[0], p[1], p[2])


def humanoid_bones(hip=0.92, spine=0.5, neck=0.1, head=0.24, shoulder=0.2, upper_arm=0.3, lower_arm=0.26, hand=0.1, finger=0.09, hip_w=0.11, upper_leg=0.42, lower_leg=0.42, foot=0.2, arm_out=0.13):
    """Bone table {name: (head, tail, parent)} with the names of Lukas's Rig_Humanoid, in an arms-down rest pose."""
    z = hip
    b = {}
    b["Hips"] = ((0, 0, z), (0, 0, z + spine * 0.25), None)
    b["Spine"] = ((0, 0, z + spine * 0.25), (0, 0, z + spine * 0.6), "Hips")
    b["Chest"] = ((0, 0, z + spine * 0.6), (0, 0, z + spine), "Spine")
    b["Neck"] = ((0, 0, z + spine), (0, 0, z + spine + neck), "Chest")
    b["Head"] = ((0, 0, z + spine + neck), (0, 0, z + spine + neck + head), "Neck")
    sz = z + spine * 0.96
    for side, s in (("Left", 1), ("Right", -1)):
        def P(x, y, zz):
            return (s * x, y, zz)
        el_x = shoulder + upper_arm * arm_out
        wr_x = el_x + lower_arm * arm_out
        hd_x = wr_x + hand * arm_out * 0.5
        el_z = sz - upper_arm
        wr_z = el_z - lower_arm
        hd_z = wr_z - hand
        b[side + "Shoulder"] = (P(0.03, 0, sz), P(shoulder, 0, sz), "Chest")
        b[side + "UpperArm"] = (P(shoulder, 0, sz), P(el_x, 0, el_z), side + "Shoulder")
        b[side + "LowerArm"] = (P(el_x, 0, el_z), P(wr_x, 0, wr_z), side + "UpperArm")
        b[side + "Hand"] = (P(wr_x, 0, wr_z), P(hd_x, 0, hd_z), side + "LowerArm")
        b[side + "ThumbProximal"] = (P(wr_x - 0.005, -0.03, wr_z - hand * 0.4), P(wr_x - 0.01, -0.05, wr_z - hand * 0.4 - finger * 0.4), side + "Hand")
        b[side + "ThumbDistal"] = (b[side + "ThumbProximal"][1], P(wr_x - 0.015, -0.06, wr_z - hand * 0.4 - finger * 0.8), side + "ThumbProximal")
        b[side + "IndexProximal"] = (P(hd_x, -0.022, hd_z), P(hd_x, -0.022, hd_z - finger * 0.5), side + "Hand")
        b[side + "IndexDistal"] = (b[side + "IndexProximal"][1], P(hd_x, -0.022, hd_z - finger), side + "IndexProximal")
        b[side + "Fingers"] = (P(hd_x, 0.012, hd_z), P(hd_x, 0.012, hd_z - finger), side + "Hand")
        kz = hip - upper_leg
        az = kz - lower_leg
        b[side + "UpperLeg"] = (P(hip_w, 0, hip - 0.04), P(hip_w + 0.01, -0.005, kz), "Hips")
        b[side + "LowerLeg"] = (P(hip_w + 0.01, -0.005, kz), P(hip_w + 0.01, 0.02, az), side + "UpperLeg")
        b[side + "Foot"] = (P(hip_w + 0.01, 0.02, az), P(hip_w + 0.02, -foot * 0.6, 0.03), side + "LowerLeg")
        b[side + "Toes"] = (P(hip_w + 0.02, -foot * 0.6, 0.03), P(hip_w + 0.03, -foot, 0.02), side + "Foot")
    return b


def make_rig(name, table, collection, extra=None):
    arm = bpy.data.armatures.new(name + "_data")
    rig = bpy.data.objects.new(name, arm)
    collection.objects.link(rig)
    vl = bpy.context.view_layer
    vl.objects.active = rig
    win = bpy.context.window_manager.windows[0]
    bones = dict(table)
    bones.update(extra or {})
    with bpy.context.temp_override(window=win):
        bpy.ops.object.mode_set(mode="EDIT")
        eb = arm.edit_bones
        for n, (h, t, _) in bones.items():
            e = eb.new(n)
            e.head = h
            e.tail = t
            e.use_deform = True
        for n, (_, _, parent) in bones.items():
            if parent:
                eb[n].parent = eb[parent]
        for n in eb:
            if any(k in n.name for k in ("Thumb", "Index", "Fingers")):
                n.align_roll(Vector((1, 0, 0)))
        bpy.ops.object.mode_set(mode="OBJECT")
    for b in rig.pose.bones:
        b.rotation_mode = "QUATERNION"
    return rig


def bmesh_box(bm, c, size, part, faces):
    """Axis-aligned box tagged as `part`; size is width, depth, height."""
    cx, cy, cz = c
    w, d, h = size[0] / 2, size[1] / 2, size[2] / 2
    vs = [bm.verts.new((cx + sx * w, cy + sy * d, cz + sz * h)) for sz in (-1, 1) for sy in (-1, 1) for sx in (-1, 1)]
    for q in ((0, 2, 3, 1), (4, 5, 7, 6), (0, 1, 5, 4), (2, 6, 7, 3), (0, 4, 6, 2), (1, 3, 7, 5)):
        faces.append((bm.faces.new([vs[i] for i in q]), part))


def cone(bm, base, tip, r, part, sides=4, faces=None):
    base = Vector(base)
    tip = Vector(tip)
    d = (tip - base).normalized()
    u = d.orthogonal().normalized()
    v = d.cross(u)
    ring = [bm.verts.new(base + (u * math.cos(2 * math.pi * i / sides) + v * math.sin(2 * math.pi * i / sides)) * r) for i in range(sides)]
    t = bm.verts.new(tip)
    for i in range(sides):
        f = bm.faces.new((ring[i], ring[(i + 1) % sides], t))
        faces.append((f, part))


def tube(bm, pts, r, part, faces, sides=5):
    """Cable through pts; both ends sink into the body, so it needs no caps."""
    pts = [Vector(p) for p in pts]
    rings = []
    for i, p in enumerate(pts):
        d = (pts[min(i + 1, len(pts) - 1)] - pts[max(i - 1, 0)]).normalized()
        u = d.orthogonal().normalized()
        v = d.cross(u)
        rings.append([bm.verts.new(p + (u * math.cos(2 * math.pi * j / sides) + v * math.sin(2 * math.pi * j / sides)) * r) for j in range(sides)])
    for a, b in zip(rings, rings[1:]):
        for j in range(sides):
            k = (j + 1) % sides
            faces.append((bm.faces.new((a[j], a[k], b[k], b[j])), part))


def segment_distance(p, a, b):
    ab = b - a
    t = np.clip(((p - a) @ ab) / max(ab @ ab, 1e-8), 0, 1)
    return np.linalg.norm(p - (a + np.outer(t, ab)), axis=1)


def auto_weights(ob, rig, overrides=None):
    """Distance weights, blocked across the body's mid-line so a hand can never pull a thigh on the other side.
    `overrides` maps a face-part vertex list to a single bone (teeth, spines, jaw lobes)."""
    m = ob.data
    co = np.array([v.co[:] for v in m.vertices])
    names = [b.name for b in rig.data.bones]
    a = np.array([rig.data.bones[n].head_local[:] for n in names])
    b = np.array([rig.data.bones[n].tail_local[:] for n in names])
    d = np.stack([segment_distance(co, a[i], b[i]) for i in range(len(names))], axis=1)
    for i, n in enumerate(names):
        if n.startswith("Left"):
            d[co[:, 0] < -0.015, i] = 1e3
        elif n.startswith("Right"):
            d[co[:, 0] > 0.015, i] = 1e3
        if n == "JawL":
            d[co[:, 0] < 0, i] = 1e3
        elif n == "JawR":
            d[co[:, 0] > 0, i] = 1e3
    limb = np.array([x.value for x in m.attributes["limb"].data])
    for i, n in enumerate(names):
        arm = "Arm" in n or "Hand" in n or "Thumb" in n or "Index" in n or "Fingers" in n
        if arm:
            d[limb == 0, i] = 1e3
        elif "Shoulder" not in n and n != "Chest":
            d[limb != 0, i] = 1e3
    groups = {n: ob.vertex_groups.new(name=n) for n in names}
    for vi in range(len(co)):
        idx = np.argsort(d[vi])[:3]
        w = 1.0 / np.maximum(d[vi][idx], 0.005) ** 4
        w = w / w.sum()
        for i, wt in zip(idx, w):
            if wt > 0.02:
                groups[names[i]].add([vi], float(wt), "REPLACE")
    if overrides:
        for bone, verts in overrides.items():
            for vi in verts:
                for g in list(m.vertices[vi].groups):
                    ob.vertex_groups[g.group].remove([vi])
                groups[bone].add([vi], 1.0, "REPLACE")


def finish_material(ob, im):
    mat = bpy.data.materials.get("M_" + ob.name) or bpy.data.materials.new("M_" + ob.name)
    mat.use_nodes = True
    tree = mat.node_tree
    bsdf = next(n for n in tree.nodes if n.type == "BSDF_PRINCIPLED")
    tx = tree.nodes.new("ShaderNodeTexImage")
    tx.image = im
    tx.interpolation = "Closest"
    tree.links.new(tx.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.9
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def assemble(name, rig, skin_mesh_data, extras_fn, collection):
    """Turns the skin mesh into the final object. extras_fn(bm, faces, rng) adds clothes, teeth and spines.
    Returns (object, parts, overrides-by-vertex-position helper)."""
    bm = bmesh.new()
    limb = bm.verts.layers.int.new("limb")
    for data, code in skin_mesh_data:
        before = set(bm.verts)
        bm.from_mesh(data)
        limb = bm.verts.layers.int.get("limb") or bm.verts.layers.int.new("limb")
        for v in bm.verts:
            if v not in before:
                v[limb] = code
    bm.normal_update()
    bm.verts.ensure_lookup_table()
    bm.verts.index_update()
    rng = random.Random(11)
    tag = bm.faces.layers.int.new("part")
    names = ["skin"]

    class Tagger:
        """Tags each new face as it is made; face references do not survive later bmesh operations."""

        def append(self, item):
            f, p = item
            if p not in names:
                names.append(p)
            f[tag] = names.index(p)

    extras_fn(bm, Tagger(), rng)
    # URP culls back faces: hand-built parts must wind outward whatever order they were made in.
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    for f in bm.faces:
        if names[f[tag]] in ("claw", "finger", "armmetal"):
            for v in f.verts:
                v[limb] = 1 if v.co.x > 0 else 2
    bmesh.ops.triangulate(bm, faces=[f for f in bm.faces if len(f.verts) > 4])
    for f in bm.faces:
        f.smooth = names[f[tag]] == "skin"   # sculpted surface reads smooth; teeth, claws and metal keep hard facets
    me = bpy.data.meshes.new(name)
    bm.to_mesh(me)
    bm.free()
    for data, _ in skin_mesh_data:
        bpy.data.meshes.remove(data)
    ob = bpy.data.objects.new(name, me)
    collection.objects.link(ob)
    layer = me.attributes["part"].data
    parts = [names[layer[i].value] for i in range(len(me.polygons))]
    ob.parent = rig
    mod = ob.modifiers.new("Armature", "ARMATURE")
    mod.object = rig
    me.calc_loop_triangles()
    return ob, parts, len(me.loop_triangles)


def verts_of_parts(ob, parts, wanted):
    out = set()
    for i, p in enumerate(ob.data.polygons):
        if parts[i] in wanted:
            out.update(p.vertices)
    return list(out)
