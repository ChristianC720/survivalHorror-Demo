"""Builds organic low-poly creatures on the shared humanoid skeleton (fallback of BLENDER_PLAN.md section 2).

The body is a Skin-modifier graph along the bones, softened and decimated to budget, which gives the flat faceted look of
Lukas. Each triangle gets its own small tile in a 512 px texture, painted by a per-character colour rule, so stains,
mouths and clothing are per-face paint with no UV seams to hide. Blender axes: facing -Y, +X is the character's left.
"""
import math
import random

import bmesh
import bpy
import numpy as np
from mathutils import Vector

TEX = 512
RADIUS_GAIN = 1.5


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


def skin_mesh(name, chains, collection, subdiv=2, target_tris=4000):
    """chains: list of [(point, rx, ry), ...]; points shared by value are welded, which builds the branches."""
    me = bpy.data.meshes.new(name + "_skin")
    verts, edges, radii, index = [], [], [], {}
    for chain in chains:
        prev = None
        for (p, rx, ry) in chain:
            key = tuple(round(c, 4) for c in p)
            if key not in index:
                index[key] = len(verts)
                verts.append(p)
                radii.append((rx, ry))
            cur = index[key]
            if prev is not None and prev != cur:
                edges.append((prev, cur))
            prev = cur
    me.from_pydata(verts, list(set(edges)), [])
    ob = bpy.data.objects.new(name + "_skin", me)
    collection.objects.link(ob)
    sk = ob.modifiers.new("skin", "SKIN")
    sk.branch_smoothing = 0.3
    sk.use_x_symmetry = False
    for i, d in enumerate(me.skin_vertices[0].data):
        d.radius = (radii[i][0] * RADIUS_GAIN, radii[i][1] * RADIUS_GAIN)
        d.use_root = i == 0
    ob.modifiers.new("sub", "SUBSURF").levels = subdiv
    dec = ob.modifiers.new("dec", "DECIMATE")
    dec.ratio = 1.0
    dg = bpy.context.evaluated_depsgraph_get()
    probe = bpy.data.meshes.new_from_object(ob.evaluated_get(dg))
    probe.calc_loop_triangles()
    n = len(probe.loop_triangles)
    bpy.data.meshes.remove(probe)
    dec.ratio = min(1.0, target_tris / max(n, 1))
    bpy.context.view_layer.update()
    dg = bpy.context.evaluated_depsgraph_get()
    out = bpy.data.meshes.new_from_object(ob.evaluated_get(dg))
    bpy.data.objects.remove(ob)
    bpy.data.meshes.remove(me)
    return out


def skin_parts(name, chains, collection, target_tris):
    """Arms are skinned apart from the body, so a sleeve can never be bound to the ribs beside it (or the reverse)."""
    width = max(abs(p[0][0]) for c in chains for p in c if p[0][2] > 0) * 0.45
    arms = {1: [], 2: []}
    body = []
    for c in chains:
        first = c[0][0]
        if abs(first[0]) > width and first[2] > 0.6:
            arms[1 if first[0] > 0 else 2].append(c)
        else:
            body.append(c)
    return [(skin_mesh(name + "_b", body, collection, target_tris=int(target_tris * .64)), 0),
            (skin_mesh(name + "_l", arms[1], collection, target_tris=int(target_tris * .18)), 1),
            (skin_mesh(name + "_r", arms[2], collection, target_tris=int(target_tris * .18)), 2)]


def body_chains(t, r):
    """Standard limbs from the bone table. r holds radii by region: (rx, ry) pairs."""
    H = lambda n: t[n][0]
    T = lambda n: t[n][1]
    chains = [[(H("Hips"), *r["hips"]), (H("Spine"), *r["waist"]), (H("Chest"), *r["ribs"]), (T("Chest"), *r["chest"]), (T("Neck"), *r["neck"])]]
    for side in ("Left", "Right"):
        def L(a, b, k):
            return tuple(x + (y - x) * k for x, y in zip(a, b))

        def S(pair, k):
            return (pair[0] * k, pair[1] * k)

        sh, el, wr = T(side + "Shoulder"), T(side + "UpperArm"), T(side + "LowerArm")
        chains.append([(sh, *r["shoulder"]), (L(sh, el, .35), *S(r["shoulder"], .85)), (el, *r["elbow"]), (L(el, wr, .3), *S(r["elbow"], 1.2)), (wr, *r["wrist"]), (T(side + "Hand"), *r["palm"])])
        th, kn, an = H(side + "UpperLeg"), T(side + "UpperLeg"), T(side + "LowerLeg")
        chains.append([(H("Hips"), *r["hips"]), (th, *r["thigh"]), (L(th, kn, .5), *S(r["thigh"], .85)), (kn, *r["knee"]), (L(kn, an, .3), *S(r["knee"], 1.2)), (an, *r["ankle"]), (T(side + "Foot"), *r["foot"]), (T(side + "Toes"), *r["toe"])])
        f = r["finger"]
        chains.append([(H(side + "Hand"), *r["wrist"]), (H(side + "ThumbProximal"), *f), (T(side + "ThumbDistal"), f[0] * .55, f[1] * .55)])
        chains.append([(T(side + "Hand"), *r["palm"]), (T(side + "IndexDistal"), f[0] * .5, f[1] * .5)])
        chains.append([(T(side + "Hand"), *r["palm"]), (T(side + "Fingers"), f[0] * 1.3, f[1] * .6)])
    return chains


def blob(bm, center, rx, ry, rz, part, faces, rings=6, seg=10):
    """Low-poly ellipsoid: heads need controlled volume, which the skin modifier cannot give."""
    c = Vector(center)
    top = bm.verts.new(c + Vector((0, 0, rz)))
    bot = bm.verts.new(c - Vector((0, 0, rz)))
    grid = []
    for i in range(1, rings):
        a = math.pi * i / rings
        grid.append([bm.verts.new(c + Vector((rx * math.sin(a) * math.sin(2 * math.pi * j / seg), -ry * math.sin(a) * math.cos(2 * math.pi * j / seg), rz * math.cos(a)))) for j in range(seg)])
    for j in range(seg):
        k = (j + 1) % seg
        faces.append((bm.faces.new((top, grid[0][k], grid[0][j])), part))
        faces.append((bm.faces.new((bot, grid[-1][j], grid[-1][k])), part))
        for i in range(rings - 2):
            faces.append((bm.faces.new((grid[i][j], grid[i][k], grid[i + 1][k], grid[i + 1][j])), part))


def bmesh_box(bm, c, size, part, faces):
    """Axis-aligned box tagged as `part`; size is width, depth, height."""
    cx, cy, cz = c
    w, d, h = size[0] / 2, size[1] / 2, size[2] / 2
    vs = [bm.verts.new((cx + sx * w, cy + sy * d, cz + sz * h)) for sz in (-1, 1) for sy in (-1, 1) for sx in (-1, 1)]
    for q in ((0, 2, 3, 1), (4, 5, 7, 6), (0, 1, 5, 4), (2, 6, 7, 3), (0, 4, 6, 2), (1, 3, 7, 5)):
        faces.append((bm.faces.new([vs[i] for i in q]), part))


def claws(bm, t, faces, length, r, part="claw"):
    """Four fingers and a thumb on each hand: the palm alone reads as a mitten."""
    for side, s in (("Left", 1), ("Right", -1)):
        tip = Vector(t[side + "Hand"][1])
        for y, k in ((-.027, .95), (-.009, 1.0), (.009, .95), (.027, .8)):
            base = tip + Vector((0, y, .015))
            cone(bm, base, base + Vector((s * .012, y * .25, -length * k)), r, part, sides=4, faces=faces)
        hd = Vector(t[side + "ThumbProximal"][0])
        cone(bm, hd, Vector(t[side + "ThumbDistal"][1]) + Vector((0, -.012, -.01)), r * 1.1, part, sides=4, faces=faces)


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


def shell(bm, select, offset, part, tear, rng, faces):
    """Clothing: a duplicate of the body faces chosen by select(center), pushed out, with random tears."""
    src = [f for f in bm.faces if select(f.calc_center_median()) and f.normal.length > 0]
    made = {}
    limb = bm.verts.layers.int.get("limb")
    for f in src:
        if rng.random() < tear:
            continue
        vs = []
        for v in f.verts:
            if v.index not in made or made[v.index] is None:
                made[v.index] = bm.verts.new(v.co + v.normal * offset)
                made[v.index][limb] = v[limb]
            vs.append(made[v.index])
        try:
            nf = bm.faces.new(vs)
            faces.append((nf, part))
        except ValueError:
            pass


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


def paint(ob, parts, color_fn, path):
    """One tile per triangle; returns nothing but writes `path` (albedo). parts[i] tags polygon i."""
    m = ob.data
    if not m.uv_layers:
        m.uv_layers.new(name="UVMap")
    uv = m.uv_layers.active.data
    n = len(m.polygons)
    cols = int(math.ceil(math.sqrt(n)))
    tile = TEX // cols
    img = np.zeros((TEX, TEX, 4), dtype=np.float32)
    img[:, :, 3] = 1
    rng = np.random.default_rng(7)
    for i, p in enumerate(m.polygons):
        cx, cy = (i % cols) * tile, (i // cols) * tile
        c = np.array(color_fn(Vector(p.center), Vector(p.normal), parts[i]), dtype=np.float32)
        noise = rng.normal(0, 0.012, (tile, tile, 1)).astype(np.float32)
        img[cy:cy + tile, cx:cx + tile, :3] = np.clip(c + noise, 0, 1)
        ls = list(p.loop_indices)
        inset = 1.0 / TEX
        corners = [(0.1, 0.1), (0.9, 0.1), (0.9, 0.9), (0.1, 0.9)]
        for k, li in enumerate(ls[:4]):
            a, b = corners[k] if len(ls) == 4 else [(0.1, 0.1), (0.9, 0.1), (0.5, 0.9)][k]
            uv[li].uv = ((cx + a * tile) / TEX + inset * 0, (cy + b * tile) / TEX)
    im = bpy.data.images.get(ob.name + "_Albedo") or bpy.data.images.new(ob.name + "_Albedo", TEX, TEX)
    im.scale(TEX, TEX)
    im.pixels.foreach_set(img.ravel())
    im.filepath_raw = path
    im.file_format = "PNG"
    im.save()
    return im


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
    for f in bm.faces:
        if names[f[tag]] in ("claw", "finger", "armmetal"):
            for v in f.verts:
                v[limb] = 1 if v.co.x > 0 else 2
    bmesh.ops.triangulate(bm, faces=[f for f in bm.faces if len(f.verts) > 4])
    for f in bm.faces:
        f.smooth = False
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


def hsv_noise(c, k, amount=0.03):
    return tuple(max(0, min(1, x + math.sin(k * 12.9898 + i * 4.1) * amount)) for i, x in enumerate(c))
