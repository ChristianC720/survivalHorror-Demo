"""Turns the team's delivery (Assets/_Project/Models/ModelsNew) into the FBX files the game uses. The delivered .blend files are
opened and never saved; their designs stay as delivered. This adds only what each one lacks to run in the game:
Mudo a rig and clips; Rastrero one mesh, UVs, paint, rig and clips; every enemy an Idle and the clothing layer the GDD asks for."""
import math
import os
import sys

import bmesh
import bpy
import numpy as np
from mathutils import Matrix, Vector
from mathutils.bvhtree import BVHTree

ROOT = r"C:\Users\Chris\Heartbeat"
NEW = ROOT + r"\Assets\_Project\Models\ModelsNew"
MODELS = ROOT + r"\Assets\_Project\Models"
sys.path.insert(0, ROOT + r"\Blender3DAssets\Scripts")
import enemy_clips
import enemy_paint as ep
import export_fbx
import hb_anim
import hb_monster as hm
import hb_paint as hp

X = hp.hex_col
def open_new(rel):
    bpy.ops.wm.open_mainfile(filepath=NEW + "\\" + rel)
    bpy.context.scene.render.fps = 30
    bpy.context.scene.frame_set(1)


def idle_from(rig, src, name, frames=91, breathe_bone="chest", deg=1.5):
    """Holds the first pose of a delivered clip with slow breathing: the team's rigs have no idle."""
    rig.animation_data_create()
    a = bpy.data.actions[src]
    rig.animation_data.action = a
    if a.slots:
        rig.animation_data.action_slot = a.slots[0]
    bpy.context.scene.frame_set(int(a.frame_range[0]))
    bpy.context.view_layer.update()
    pose = {b.name: (b.rotation_mode, b.location.copy(), b.rotation_quaternion.copy(), b.rotation_euler.copy()) for b in rig.pose.bones}
    new = bpy.data.actions.new(name)
    rig.animation_data.action = new
    for f in (1, (frames + 1) // 2, frames):
        k = math.sin(math.pi * (f - 1) / (frames - 1))
        for b in rig.pose.bones:
            mode, loc, q, e = pose[b.name]
            b.location = loc
            b.keyframe_insert("location", frame=f, group=b.name)
            b.rotation_mode = "QUATERNION"
            base = q if mode == "QUATERNION" else e.to_quaternion()
            if b.name == breathe_bone:
                base = base @ Matrix.Rotation(math.radians(deg) * k, 4, "X").to_quaternion()
            b.rotation_quaternion = base
            b.keyframe_insert("rotation_quaternion", frame=f, group=b.name)
    rig.animation_data.action = None


# ------------------------------------------------------------ clothing layer

def _category(bone):
    """Team and shared rigs name bones differently; clothing only needs the body region."""
    n = bone.lower()
    for key, cat in (("clavicle", "chest"), ("shoulder", "chest"), ("upperarm", "uarm"), ("forearm", "farm"), ("lowerarm", "farm"), ("thigh", "thigh"), ("upperleg", "thigh"), ("shin", "shin"), ("lowerleg", "shin"), ("ankle", "foot"), ("foot", "foot"), ("toe", "foot"), ("hand", "hand"), ("thumb", "hand"), ("finger", "hand"), ("index", "hand"), ("jaw", "head"), ("lobe", "head"), ("head", "head"), ("neck", "neck"), ("chest", "chest"), ("spine", "spine"), ("hips", "hips")):
        if key in n:
            return cat
    return "other"


def add_clothing(body, garments, thick=.012, torn=.0, seed=4, body_filter=None, limits=None):
    """Shell over the body's own faces with the same bone weights, so cloth can never part from the skin it covers.
    garments: {part: set of regions}. thick: metres, or {part: metres}. torn: share of faces ripped out. limits: {part: fn(face center) -> bool} for a hem at a height.
    A strip back to the body closes every hem and tear.
    The result goes on material slot 2; returns the per-polygon part names."""
    me = body.data
    me.update()
    names = {g.index: g.name for g in body.vertex_groups}
    region = {}
    for v in me.vertices:
        best = max(v.groups, key=lambda g: g.weight, default=None)
        region[v.index] = _category(names[best.group]) if best is not None else "other"
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.verts.ensure_lookup_table()
    bm.normal_update()
    dl = bm.verts.layers.deform.verify()
    tag = bm.faces.layers.int.new("cloth_part")
    keys = list(garments)
    for f in bm.faces:
        f[tag] = 0
    co = np.array([v.co[:] for v in bm.verts], dtype=np.float32)
    fold = hp.noise(co, (9, 9, 3), seed) - .5
    made = {}

    def lifted(v, part):
        if v.index not in made:
            t = thick[part] if isinstance(thick, dict) else thick
            nv = bm.verts.new(v.co + v.normal * (t + float(fold[v.index]) * t * 1.1))
            for k, w in v[dl].items():
                nv[dl][k] = w
            made[v.index] = nv
        return made[v.index]

    src = list(bm.faces)
    rips = hp.fbm(np.array([f.calc_center_median()[:] for f in src], dtype=np.float32), 9, 2, seed + 1)
    chosen = {}
    for f, r in zip(src, rips):
        if body_filter is not None and not body_filter(f):
            continue
        regs = [region[v.index] for v in f.verts]
        for i, part in enumerate(keys):
            ok = limits is None or part not in limits or limits[part](f.calc_center_median())
            if ok and all(x in garments[part] for x in regs) and r < 1 - torn:
                chosen[f] = i
                break
    cloth = []
    for f, i in chosen.items():
        try:
            nf = bm.faces.new([lifted(v, keys[i]) for v in f.verts])
        except ValueError:
            continue
        nf[tag] = i + 1
        nf.material_index = 1
        nf.smooth = True
        cloth.append(nf)
    # Hems and tears: an edge with one chosen face gets a strip down to the skin.
    for f, i in chosen.items():
        for e in f.edges:
            if sum(1 for lf in e.link_faces if lf in chosen) != 1:
                continue
            a, b = e.verts
            if a.index in made and b.index in made:
                try:
                    nf = bm.faces.new((a, b, made[b.index], made[a.index]))
                except ValueError:
                    continue
                nf[tag] = i + 1
                nf.material_index = 1
                nf.smooth = False
                cloth.append(nf)
    # Cloth winds with its source face; strips are the only ones that may not.
    bmesh.ops.recalc_face_normals(bm, faces=cloth)
    bm.to_mesh(me)
    bm.free()
    labels = ["skin"] + keys
    layer = me.attributes["cloth_part"].data
    return [labels[layer[i].value] for i in range(len(me.polygons))]


def paint_cloth(body, parts, name, painter, folder=r"\Enemies\Textures", size=1024):
    """Bakes only the clothing polygons: the delivered body keeps its UVs and its maps."""
    B = hp.bake(body.data, parts, size, align_longest=True, skip=("skin",))
    B.Ns, B.ao, B.crease, B.mat = B.N, np.full(B.n, .9, dtype=np.float32), np.zeros(B.n, dtype=np.float32), np.zeros(B.n, dtype=np.uint8)
    tex = MODELS + folder + rf"\{name}.png"
    hp.write(tex, name, size, [(B, painter(B))])
    # Slot 2 must exist or the FBX merges cloth into the body's submesh.
    mat = bpy.data.materials.new("M_" + name)
    mat.use_nodes = True
    node = mat.node_tree.nodes.new("ShaderNodeTexImage")
    node.image = bpy.data.images[name]
    node.interpolation = "Closest"
    mat.node_tree.links.new(node.outputs["Color"], next(n for n in mat.node_tree.nodes if n.type == "BSDF_PRINCIPLED").inputs["Base Color"])
    body.data.materials.append(mat)
    return B


def _worn(B, c, region, seed):
    """Frayed dark edge at rips: hard and thin, the Signalis way."""
    hp.grime(B, c, region, seed, .45)
    hp.folds(B, c, region, seed + 1, strength=.34)


def cloth_alto(B):
    """Remains of the Werkschutz uniform: ripped quilted vest, torn trousers with a service stripe."""
    P, c = B.P, B.fill(ep.GUARD)
    V, T = B.is_("vest"), B.is_("pants")
    k = (ep._value(B) * 1.15 + .15)[:, None]
    c[V] = (ep.GUARD * 1.3 * k)[V]
    c[T] = (ep.GUARD_DK * 1.5 * k)[T]
    _worn(B, c, V | T, 7)
    m = V & (((P[:, 2] * 11) % 1) < .07)
    c[m] = c[m] * .55
    m = T & (np.abs(np.abs(P[:, 0]) - .2) < .012)
    c[m] = ep.CRIMSON * .8
    hp.stamp(B, c, (0, .2, 1.85), (0, 1, 0), .06, X("#9d9a90"), hp.text_bitmap("WERKSCHUTZ"), wear=.2, seed=4)
    hp.stamp(B, c, (.09, -.25, 1.85), (0, -1, 0), .035, X("#9d9a90"), hp.text_bitmap("KS"), wear=.25, seed=5)
    hp.blood(B, c, V | T, 9, coverage=.7)
    return hp.posterize(B, c, levels=20)


def cloth_boca(B):
    """Rags of the experiment gown (GDD: restos de ropa de experimentacion)."""
    P, c = B.P, B.fill(ep.GOWN)
    R = B.is_("rags")
    k = (ep._value(B) * 1.15 + .15)[:, None]
    c[R] = (ep.GOWN * .82 * k)[R]
    _worn(B, c, R, 8)
    hp.stamp(B, c, (0, -.2, 1.0), (0, -1, 0), .04, ep.INK * 2, hp.text_bitmap("SUBJ 31"), wear=.15, seed=6)
    hp.blood(B, c, R, 11, coverage=.64)
    return hp.posterize(B, c, levels=20)


def cloth_rastrero(B):
    """Striped subject shirt and dark torn trousers, the colours of its sheet."""
    P, c = B.P, B.fill(ep.GOWN)
    S, T = B.is_("shirt"), B.is_("pants")
    k = (ep._value(B) * 1.2 + .12)[:, None]
    stripe = ((P[:, 0] * 26) % 1) < .4
    c[S] = (np.where(stripe[:, None], X("#5d7382"), X("#b5b6ae")) * k)[S]
    c[T] = (X("#232c31") * 1.3 * k)[T]
    _worn(B, c, S | T, 3)
    hp.blood(B, c, S | T, 17, coverage=.74)
    return hp.posterize(B, c, levels=20)


def rename_actions(table):
    for src, dst in table.items():
        bpy.data.actions[src].name = dst


def finish(name, rig, meshes):
    export_fbx.export(MODELS + rf"\Enemies\CH_{name}.fbx", rig, meshes)
    return {m.name: sum(len(p.vertices) - 2 for p in m.data.polygons) for m in meshes}


# ------------------------------------------------------------ the two delivered complete enemies

def alto():
    open_new(r"Monsters\Monster3\Monster3.blend")
    rig = bpy.data.objects["Monster3_Rig"]
    rig.name = "Rig_Alto"
    body = bpy.data.objects["Monster3_Body"]
    body.name = body.data.name = "CH_Alto"
    idle_from(rig, "Patrulla", "AN_Alto_Idle")
    rename_actions({"Patrulla": "AN_Alto_Patrol", "Caza": "AN_Alto_Hunt", "Golpe largo derecho": "AN_Alto_Attack", "Golpe largo izquierdo": "AN_Alto_Attack_Left", "Agarre": "AN_Alto_Grab", "Aturdido": "AN_Alto_Stunned", "Grito": "AN_Alto_Scream", "Muerte": "AN_Alto_Death"})
    # Full padded vest down to the waist and loose trousers. The earlier version stays as CH_Alto_V1.fbx for the owner to compare.
    waist = max(b.head_local.z for b in rig.data.bones if _category(b.name) == "hips") + .08
    parts = add_clothing(body, {"vest": {"chest", "spine", "hips", "uarm"}, "pants": {"hips", "thigh", "shin"}}, thick={"vest": .03, "pants": .032}, torn=.09, seed=12,
                         limits={"vest": lambda c: c.z > waist, "pants": lambda c: c.z <= waist})
    paint_cloth(body, parts, "CH_Alto_Cloth", cloth_alto)
    return finish("Alto", rig, [body])


def boca():
    open_new(r"Monsters\Monster2\Monster2.blend")
    rig = bpy.data.objects["Monster2_Rig"]
    rig.name = "Rig_Boca"
    body = bpy.data.objects["Monster2_Body"]
    body.name = body.data.name = "CH_Boca"
    idle_from(rig, "Deambular", "AN_Boca_Idle")
    rename_actions({"Deambular": "AN_Boca_Wander", "Caza": "AN_Boca_Hunt", "Mordida": "AN_Boca_Attack", "Atacar": "AN_Boca_Attack_Claw", "Grito": "AN_Boca_Scream", "Aturdido": "AN_Boca_Stunned", "Muerte": "AN_Boca_Death"})
    parts = add_clothing(body, {"rags": {"hips", "thigh"}}, thick=.014, torn=.26, seed=4)
    paint_cloth(body, parts, "CH_Boca_Cloth", cloth_boca)
    return finish("Boca", rig, [body])


# ------------------------------------------------------------ Mudo: delivered as a bare mesh

def _snap(co, p, r=.07):
    """Takes a joint's depth from the mesh around it, so bones sit inside the limb."""
    m = np.abs(co[:, 0] - p[0]) < r
    m &= np.abs(co[:, 2] - p[2]) < r
    return (p[0], float(co[m, 1].mean()) if m.any() else p[1], p[2])


def _table(co, j):
    """Bone table with the names of the shared humanoid, from joints {name: (x, z)} on the left side."""
    t = hm.humanoid_bones()
    def S(k, s=1):
        x, y, z = j[k]
        return (x * s, y, z)
    t["Hips"] = (j["hips"], j["spine"], None)
    t["Spine"] = (j["spine"], j["chest"], "Hips")
    t["Chest"] = (j["chest"], j["neck"], "Spine")
    t["Neck"] = (j["neck"], j["head"], "Chest")
    t["Head"] = (j["head"], j["crown"], "Neck")
    for side, s in (("Left", 1), ("Right", -1)):
        sh, el, wr, tip = S("shoulder", s), S("elbow", s), S("wrist", s), S("tip", s)
        hand_end = tuple(a + (b - a) * .55 for a, b in zip(wr, tip))
        t[side + "Shoulder"] = ((s * .03, sh[1], sh[2]), sh, "Chest")
        t[side + "UpperArm"] = (sh, el, side + "Shoulder")
        t[side + "LowerArm"] = (el, wr, side + "UpperArm")
        t[side + "Hand"] = (wr, hand_end, side + "LowerArm")
        off = lambda p, dy, dz=0: (p[0], p[1] + dy, p[2] + dz)
        t[side + "ThumbProximal"] = (off(hand_end, -.03, .03), off(hand_end, -.05, -.01), side + "Hand")
        t[side + "ThumbDistal"] = (off(hand_end, -.05, -.01), off(hand_end, -.06, -.04), side + "ThumbProximal")
        mid = tuple((a + b) / 2 for a, b in zip(hand_end, tip))
        t[side + "IndexProximal"] = (off(hand_end, -.022), off(mid, -.022), side + "Hand")
        t[side + "IndexDistal"] = (off(mid, -.022), off(tip, -.022), side + "IndexProximal")
        t[side + "Fingers"] = (off(hand_end, .012), off(tip, .012), side + "Hand")
        t[side + "UpperLeg"] = (S("hip", s), S("knee", s), "Hips")
        t[side + "LowerLeg"] = (S("knee", s), S("ankle", s), side + "UpperLeg")
        t[side + "Foot"] = (S("ankle", s), S("ball", s), side + "LowerLeg")
        t[side + "Toes"] = (S("ball", s), S("toe", s), side + "Foot")
    return t


def _limb_attr(me, values):
    if "limb" not in me.attributes:
        me.attributes.new("limb", "INT", "POINT")
    me.attributes["limb"].data.foreach_set("value", values)


def _source_clips():
    """Base cycles for the shared humanoid come from the team's Lukas, under the names enemy_clips asks for."""
    with bpy.data.libraries.load(NEW + r"\Lukas\Player.blend", link=False) as (src, dst):
        dst.actions = ["Lukas_Idle_Bien", "Lukas_Caminar_Bien", "Lukas_Correr_Bien"]
        dst.armatures = ["PJ_Rig"] if "PJ_Rig" in src.armatures else [n for n in src.armatures][:1]
    arm = dst.armatures[0]
    rest = {b.name: b.matrix_local.to_quaternion() for b in arm.bones}
    bpy.data.armatures.remove(arm)
    for a, n in zip(dst.actions, ("src_Idle", "src_Walk", "src_Run")):
        a.name = n
    return rest


MUDO_J = {"hips": (0, 0, 1.02), "spine": (0, 0, 1.14), "chest": (0, 0, 1.36), "neck": (0, 0, 1.68), "head": (0, -.015, 1.8), "crown": (0, -.03, 2.13),
          "shoulder": (.24, 0, 1.66), "elbow": (.38, 0, 1.25), "wrist": (.47, 0, .95), "tip": (.46, 0, .74),
          "hip": (.1, 0, 1.0), "knee": (.195, 0, .63), "ankle": (.285, 0, .1), "ball": (.31, 0, .03), "toe": (.325, 0, .015)}


def _mudo_body(garments=None, torn=.03, thick=.012):
    """The delivered Mudo, rigged; dressed only if garments are given. Returns (body, rig, cloth parts)."""
    open_new(r"Monsters\Monster1\Monster1.blend")
    body = bpy.data.objects["Monster1_Body"]
    body.name = body.data.name = "CH_Mudo"
    me = body.data
    co = np.array([v.co[:] for v in me.vertices], dtype=np.float32)
    j = {k: _snap(co, v) if k not in ("hips", "spine", "chest", "neck", "head", "crown") else v for k, v in MUDO_J.items()}
    j["ball"] = (.31, float(co[(np.abs(co[:, 0] - .31) < .05) & (co[:, 2] < .05), 1].mean()), .03)
    j["toe"] = (.325, float(co[(np.abs(co[:, 0] - .32) < .08) & (co[:, 2] < .04), 1].min()) + .01, .015)
    t = _table(co, j)
    rig = hm.make_rig("Rig_Mudo", t, body.users_collection[0])
    # Arm or torso: nearer the arm's bones, measured against each part's own thickness.
    arms = [(np.array(t[s + b][0]), np.array(t[s + b][1])) for s in ("Left", "Right") for b in ("UpperArm", "LowerArm", "Hand", "Fingers")]
    da = np.min([hm.segment_distance(co.astype(np.float64), a, b) for a, b in arms], axis=0)
    _limb_attr(me, np.where((da < .085) & (np.abs(co[:, 0]) > .17), np.where(co[:, 0] > 0, 1, 2), 0))
    body.parent = rig
    body.modifiers.new("Armature", "ARMATURE").object = rig
    hm.auto_weights(body, rig)
    parts = add_clothing(body, garments, thick=thick, torn=torn, seed=7) if garments else ["skin"] * len(me.polygons)
    return body, rig, parts


def mudo():
    body, rig, parts = _mudo_body()
    A = _author(rig)
    enemy_mudo(A)
    rig.animation_data.action = None
    return finish("Mudo", rig, [body])


def _author(rig):
    rest = _source_clips()
    return hb_anim.Author(rig, rest)


def enemy_mudo(A):
    P = enemy_clips.mudo(A)
    enemy_clips.death(A, "AN_Mudo_Death", P, .84)


# ------------------------------------------------------------ Rastrero: delivered as loose parts

RAS_J = {"hips": (0, .1, 1.0), "spine": (0, .07, 1.14), "chest": (0, -.08, 1.3), "neck": (0, -.24, 1.62), "head": (0, -.44, 1.56), "crown": (0, -.62, 1.46),
         "shoulder": (.31, -.22, 1.62), "elbow": (.42, -.2, 1.1), "wrist": (.51, -.27, .76), "tip": (.5, -.31, .44),
         "hip": (.15, .08, .98), "knee": (.28, -.09, .56), "ankle": (.26, .09, .2), "ball": (.33, -.07, .05), "toe": (.35, -.24, .02)}
RAS_PARTS = {"Head": "head", "Torso": "torso", "Arm_L": "arm", "Arm_R": "arm", "Leg_L": "leg", "Leg_R": "leg", "Cloth_Tatters": "tatter", "Teeth": "tooth", "Spine_Spikes": "bone", "Cables": "cable", "Metal_Plates": "metal"}


def _join_parts():
    """One mesh from the delivered parts, positions untouched. Returns (mesh, per-face part name)."""
    bm = bmesh.new()
    tag = bm.faces.layers.int.new("part")
    names = []
    for src, part in RAS_PARTS.items():
        ob = bpy.data.objects[src]
        if part not in names:
            names.append(part)
        # The delivery runs 11.5k triangles; the enemy limit is 8k. Only the big organic parts give up polygons.
        if part in ("head", "torso", "arm", "leg"):
            dec = ob.modifiers.new("dec", "DECIMATE")
            dec.ratio = .42
            bpy.context.view_layer.update()
            tmp = bpy.data.meshes.new_from_object(ob.evaluated_get(bpy.context.evaluated_depsgraph_get()))
        else:
            tmp = ob.data.copy()
        tmp.transform(ob.matrix_world)
        before = set(bm.faces)
        bm.from_mesh(tmp)
        tag = bm.faces.layers.int.get("part")
        for f in bm.faces:
            if f not in before:
                f[tag] = names.index(part)
        bpy.data.meshes.remove(tmp)
        bpy.data.objects.remove(ob)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    for f in bm.faces:
        f.smooth = False
    me = bpy.data.meshes.new("CH_Rastrero")
    bm.to_mesh(me)
    bm.free()
    layer = me.attributes["part"].data
    return me, [names[layer[i].value] for i in range(len(me.polygons))]


def _face_ao(me):
    """Occlusion per polygon from short rays: the delivered parts carry no maps to shade from."""
    me.calc_loop_triangles()
    bvh = BVHTree.FromPolygons([v.co[:] for v in me.vertices], [p.vertices[:] for p in me.polygons], all_triangles=False)
    dirs = [Vector((x, y, z)).normalized() for x, y, z in ((1, 0, 1), (-1, 0, 1), (0, 1, 1), (0, -1, 1), (0, 0, 1), (1, 1, 0), (-1, 1, 0), (1, -1, 0), (-1, -1, 0))]
    out = np.ones(len(me.polygons), dtype=np.float32)
    for p in me.polygons:
        hit = n = 0
        o = p.center + p.normal * .004
        for d in dirs:
            if d.dot(p.normal) < .1:
                continue
            n += 1
            if bvh.ray_cast(o, d, .22)[0] is not None:
                hit += 1
        out[p.index] = 1 - hit / max(n, 1)
    return out


def paint_rastrero(B, part_ao):
    """Colours of the team's sheet Monster4.jpeg, on the painter of the earlier enemies."""
    P = B.P
    B.Ns, B.crease, B.mat = B.N, np.zeros(B.n, dtype=np.float32), np.zeros(B.n, dtype=np.uint8)
    B.ao = part_ao
    c = B.fill(ep.FLESH[2])
    H, Tr, Ar, Lg, Tt = B.is_("head"), B.is_("torso"), B.is_("arm"), B.is_("leg"), B.is_("tatter")
    skin = H | Tr | Ar | Lg
    ep._skin(B, c, skin, ep.FLESH, 7)
    t = (ep._value(B) * 1.2 + .12)[:, None]
    # The hanging tatters of the delivery stay cloth; the rest of the clothing is the shell on top (cloth_rastrero).
    up = Tt & (P[:, 2] > .98)
    stripe = ((P[:, 0] * 26) % 1) < .4
    c[up] = (np.where(stripe[:, None], X("#5d7382"), X("#b5b6ae")) * t)[up]
    low = Tt & ~up
    c[low] = (X("#232c31") * t)[low]
    # Bandage on the forearm, from the sheet.
    m = Ar & (np.abs(P[:, 2] - .9) < .055)
    c[m] = (X("#b6ae96") * t)[m]
    c[m & (((P[:, 2] * 90) % 1) < .22)] *= .72
    # The maw: inside the ring of teeth on the front of the jaw.
    maw = H & (P[:, 1] < -.5) & (((P[:, 0] / .17) ** 2 + ((P[:, 2] - 1.08) / .34) ** 2) < 1)
    B.ao = np.where(maw, part_ao * .7, part_ao)
    ep._mouth(B, c, maw)
    lip = H & (P[:, 1] < -.5) & ~maw & (((P[:, 0] / .205) ** 2 + ((P[:, 2] - 1.08) / .38) ** 2) < 1)
    c[lip] = c[lip] * .4 + ep.BLOOD * .6
    ep._gashes(B, c, skin & ~maw, 12, 16)
    ep._dirty_hands(B, c, Ar | Lg, 9, .78)
    ep._hard(B, c)
    return hp.posterize(B, c, levels=24)


def rastrero():
    open_new(r"Monsters\Monster4\Monster4.blend")
    me, parts = _join_parts()
    col = bpy.context.scene.collection
    body = bpy.data.objects.new("CH_Rastrero", me)
    col.objects.link(body)
    co = np.array([v.co[:] for v in me.vertices], dtype=np.float32)
    t = _table(co, RAS_J)
    rig = hm.make_rig("Rig_Rastrero", t, col)
    # Parts decide what binds to what: the delivered mesh keeps arms, legs and jaw as separate pieces.
    limb = np.zeros(len(co), dtype=np.int32)
    head_v = set()
    for p in me.polygons:
        if parts[p.index] == "arm":
            for v in p.vertices:
                limb[v] = 1 if co[v, 0] > 0 else 2
        elif parts[p.index] in ("head", "tooth"):
            head_v.update(p.vertices)
    _limb_attr(me, limb)
    body.parent = rig
    mod = body.modifiers.new("Armature", "ARMATURE")
    mod.object = rig
    # Only the root of the jaw blends into the neck.
    over = {"Head": [v for v in head_v if not (co[v, 1] > -.42 and co[v, 2] > 1.42)]}
    hm.auto_weights(body, rig, over)
    B = hp.bake(me, parts, 2048, align_longest=True)
    ao = _face_ao(me)
    # Per-texel lookup of the polygon's occlusion: bake() writes polygons in order, so counts rebuild the mapping.
    B.ao = _texel_ao(me, parts, B, ao)
    tex = MODELS + r"\Enemies\Textures\CH_Rastrero_Albedo.png"
    im = hp.save(tex, "CH_Rastrero_Albedo", hp.write(tex, "CH_Rastrero_Albedo", 2048, [(B, paint_rastrero(B, B.ao))]))
    hm.finish_material(body, im)
    # Clothing covers torso, arms and legs only: the jaw and head never get a sleeve.
    cloth = add_clothing(body, {"shirt": {"chest", "spine", "uarm"}, "pants": {"hips", "thigh", "shin"}}, thick=.012, torn=.24, seed=14, body_filter=lambda f: parts[f.index] in ("torso", "arm", "leg"))
    paint_cloth(body, cloth, "CH_Rastrero_Cloth", cloth_rastrero)
    A = _author(rig)
    enemy_rastrero(A)
    rig.animation_data.action = None
    return finish("Rastrero", rig, [body])


def _texel_ao(me, parts, B, ao):
    """hp.bake fills each polygon's island in polygon order; recompute island sizes from the bake's own arrays."""
    # Nearest polygon by position is exact enough here: islands are flat and each texel lies on its polygon's plane.
    from mathutils.kdtree import KDTree
    kd = KDTree(len(me.polygons))
    for p in me.polygons:
        kd.insert(p.center, p.index)
    kd.balance()
    out = np.empty(B.n, dtype=np.float32)
    # Sparse sampling then fill: a KD query per texel of a 2048 px map would take minutes.
    step = 7
    idx = np.arange(0, B.n, step)
    vals = np.array([ao[kd.find(B.P[i])[1]] for i in idx], dtype=np.float32)
    out[:] = np.repeat(vals, step)[:B.n]
    return out


def enemy_rastrero(A):
    # Rest is the sheet's standing pose: the clip lowers it to the sheet's crawl (D-152: always on four legs).
    def add(d, **kw):
        o = dict(d)
        for bone, e in kw.items():
            a = o.get(bone, (0, 0, 0))
            o[bone] = (a[0] + e[0], a[1] + e[1], a[2] + e[2])
        return o

    B = {"Hips": (34, 0, 0), "Spine": (6, 0, 0), "Chest": (8, 0, 0), "Neck": (-26, 0, 0), "Head": (-22, 0, 0),
         "LeftUpperArm": (-44, -4, 0), "RightUpperArm": (-44, 4, 0), "LeftLowerArm": (-18, 0, 0), "RightLowerArm": (-18, 0, 0), "LeftHand": (-26, 0, 0), "RightHand": (-26, 0, 0),
         "LeftUpperLeg": (-64, 0, 0), "RightUpperLeg": (-64, 0, 0), "LeftLowerLeg": (54, 0, 0), "RightLowerLeg": (54, 0, 0), "LeftFoot": (-24, 0, 0), "RightFoot": (-24, 0, 0), "loc": (0, .08, -.26)}

    def crawl(n, frames, amp):
        keys = {}
        for i in range(9):
            ph = 2 * math.pi * i / 8
            s, c = math.sin(ph), math.cos(ph)
            keys[1 + round((frames - 1) * i / 8)] = add(B, LeftUpperArm=(amp * s, 0, 0), RightUpperArm=(-amp * s, 0, 0),
                                                       LeftLowerArm=(-max(0, c) * amp * .5, 0, 0), RightLowerArm=(-max(0, -c) * amp * .5, 0, 0),
                                                       LeftUpperLeg=(-amp * s, 0, 0), RightUpperLeg=(amp * s, 0, 0),
                                                       LeftLowerLeg=(max(0, c) * amp * .6, 0, 0), RightLowerLeg=(max(0, -c) * amp * .6, 0, 0),
                                                       Chest=(0, 0, amp * .18 * s), Spine=(0, amp * .1 * c, 0))
        A.clip(n, frames, keys, loop=True)

    A.clip("AN_Rastrero_Idle", 61, {1: B, 30: add(B, Chest=(3, 0, 0), Head=(0, 0, 8)), 61: B}, loop=True)
    crawl("AN_Rastrero_Crawl", 25, 24)
    crawl("AN_Rastrero_Hunt", 15, 34)
    A.clip("AN_Rastrero_Attack", 24, {1: B, 8: add(B, Hips=(-14, 0, 0), RightUpperArm=(-46, 0, 0), RightLowerArm=(-26, 0, 0)), 13: add(B, Hips=(6, 0, 0), RightUpperArm=(30, 0, 0), Head=(-12, 0, 0)), 24: B})
    A.clip("AN_Rastrero_Stunned", 31, {1: add(B, Head=(18, 12, 0)), 15: add(B, Head=(12, -10, 0)), 31: add(B, Head=(18, 12, 0))}, loop=True)
    F = {"Hips": (-40, 0, 10), "Chest": (6, 0, 0), "Head": (10, 0, -24), "LeftUpperArm": (-30, -56, 0), "RightUpperArm": (-20, 34, 0), "LeftLowerLeg": (32, 0, 0), "RightLowerLeg": (4, 0, 0), "loc": (0, .3, -.72)}
    A.clip("AN_Rastrero_Death", 45, {1: B, 26: F, 45: F})


# ------------------------------------------------------------ Lukas

def lukas(old_blend):
    """The team's Lukas with its clips; the action clips it lacks come from the previous rig, whose bones share names."""
    open_new(r"Lukas\Player.blend")
    rig = bpy.data.objects["PJ_Rig"]
    body = bpy.data.objects["PJ_Cuerpo"]
    gun = bpy.data.objects["Pistola_Preview"]
    body.name = body.data.name = "CH_Lukas_RH07"
    # Team's grip, as markers on the hand: origin, barrel direction and top of the gun (read in LukasAttachments).
    markers = []
    for nm, offset in (("WeaponGrip", (-.055, 0, .055)), ("WeaponGrip_Barrel", (.045, 0, .055)), ("WeaponGrip_Up", (-.055, 0, .155))):
        e = bpy.data.objects.new(nm, None)
        markers.append(e)
        body.users_collection[0].objects.link(e)
        e.parent, e.parent_type, e.parent_bone = rig, "BONE", "RightHand"
        e.matrix_parent_inverse = Matrix.Identity(4)
        e.matrix_local = gun.matrix_local @ Matrix.Translation(offset)
    bpy.context.view_layer.update()
    for nm, keep in (("Cadera", 0), ("Cadera_respaldo", 0), ("Head", 0), ("Torso", 0), ("Pistola_Preview", 0)):
        bpy.data.objects.remove(bpy.data.objects[nm])
    # Team clips are 24 fps: stretch to the game's 30 so they keep their speed.
    for a in list(bpy.data.actions):
        if a.name.startswith("Lukas_"):
            for fc in hb_anim.fcurves_of(a):
                for k in fc.keyframe_points:
                    k.co.x *= 1.25
                    k.handle_left.x *= 1.25
                    k.handle_right.x *= 1.25
                fc.update()
    table = {"Idle_Bien": "Idle_Fine", "Idle_Herido": "Idle_Injured", "Idle_Critico": "Idle_Critical", "Caminar_Bien": "Walk_Fine", "Caminar_Herido": "Walk_Injured", "Caminar_Critico": "Walk_Critical", "Correr_Bien": "Run", "Agachado_Bien": "Crouch_Idle", "CaminarAgachado_Bien": "Crouch_Walk", "Apuntar_Bien": "Aim"}
    for a in list(bpy.data.actions):
        if a.name.startswith("Lukas_"):
            short = a.name[len("Lukas_"):]
            a.name = "AN_Lukas_" + table.get(short, "Team_" + short)
    have = {a.name for a in bpy.data.actions}
    with bpy.data.libraries.load(old_blend, link=False) as (src, dst):
        dst.actions = [n for n in src.actions if n.startswith("AN_Lukas_") and n not in have]
        dst.armatures = [n for n in src.armatures][:1]
    arm = dst.armatures[0]
    new_rest = {b.name: b.matrix_local.to_quaternion() for b in rig.data.bones}
    delta = max((abs(new_rest[b.name].rotation_difference(b.matrix_local.to_quaternion()).angle) for b in arm.bones if b.name in new_rest), default=0)
    bpy.data.armatures.remove(arm)
    extra = [a.name for a in dst.actions]
    # The FBX exporter drops a whole action if one curve names a missing bone, and this rig has no fingers.
    for a in dst.actions:
        for layer in a.layers:
            for strip in layer.strips:
                for bag in strip.channelbags:
                    for fc in [f for f in bag.fcurves if '"' in f.data_path and f.data_path.split('"')[1] not in rig.data.bones]:
                        bag.fcurves.remove(fc)
    A, w, h = _pixels("PJ_Personaje_Albedo")
    import lukas_variants as lv
    lv.TEXDIR = MODELS + r"\Characters\Textures"
    lv.make_variants(body, A, w, h)
    civil, dropped = lv.make_civil(body, A, w, h)
    meshes = [body, civil]
    export_fbx.export(MODELS + r"\Characters\CH_Lukas_RH07.fbx", rig, meshes, markers=markers)
    return dict(borrowed=extra, rest_delta_deg=math.degrees(delta), tris=sum(len(p.vertices) - 2 for p in body.data.polygons), civil_dropped=dropped)


def _pixels(name):
    im = bpy.data.images[name]
    w, h = im.size
    return np.array(im.pixels[:], dtype=np.float32).reshape(h, w, 4), w, h


# ------------------------------------------------------------ props

PROPS = {"Municion": "CH_Item_Ammo", "Autoinyector": "CH_Item_Heal", "Stunject": "CH_Item_Spike", "Llave_Maestra": "CH_Item_KeyMandatory", "Tarjeta_Acceso": "CH_Item_KeyOptional", "Terminal": "CH_Item_Terminal"}


def _export_static(ob, path):
    vl = bpy.context.view_layer
    vl.update()
    for o in bpy.context.scene.objects:
        o.select_set(False)
    ob.select_set(True)
    vl.objects.active = ob
    with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
        bpy.ops.export_scene.fbx(filepath=path, use_selection=True, object_types={"MESH"}, apply_unit_scale=True, apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", bake_space_transform=True, bake_anim=False, mesh_smooth_type="FACE")


def props():
    open_new(r"Props\Props.blend")
    out = {}
    for src, dst in PROPS.items():
        ob = bpy.data.objects[src]
        ob.location = (0, 0, 0)
        ob.name = ob.data.name = dst
        _export_static(ob, MODELS + rf"\Props\{dst}.fbx")
        out[dst] = len(ob.data.polygons)
    return out


# ------------------------------------------------------------ the Kessler revolver

GRIP = np.array((-.055, 0, .055))   # top of the delivered grip, in Pistola's space


class Gun:
    """Parts in the old CH_Revolver convention: origin on top of the grip, barrel toward -Y, up +Z."""

    def __init__(self, bm, tag, names):
        self.bm, self.tag, self.names = bm, tag, names

    def _f(self, vs, part):
        if part not in self.names:
            self.names.append(part)
        f = self.bm.faces.new(vs)
        f[self.tag] = self.names.index(part)

    def box(self, lo, hi, part):
        x0, y0, z0 = lo
        x1, y1, z1 = hi
        v = [self.bm.verts.new((x, y, z)) for z in (z0, z1) for y in (y0, y1) for x in (x0, x1)]
        for q in ((0, 2, 3, 1), (4, 5, 7, 6), (0, 1, 5, 4), (2, 6, 7, 3), (0, 4, 6, 2), (1, 3, 7, 5)):
            self._f([v[i] for i in q], part)

    def prism(self, y0, y1, z, r0, r1, part, sides=10, x=0.0, wave=None):
        """Along Y, capped. wave(i) scales the radius of side i: flutes on the cylinder."""
        def ring(y, r):
            return [self.bm.verts.new((x + r * (wave(i) if wave else 1) * math.cos(2 * math.pi * i / sides), y, z + r * (wave(i) if wave else 1) * math.sin(2 * math.pi * i / sides))) for i in range(sides)]
        a, b = ring(y0, r0), ring(y1, r1)
        for i in range(sides):
            j = (i + 1) % sides
            self._f((a[i], a[j], b[j], b[i]), part)
        self._f(a[::-1], part)
        self._f(b, part)

    def wedge(self, pts, width, part):
        """Extruded side profile, pts as (y, z), centred on X."""
        w = width / 2
        l = [self.bm.verts.new((-w, y, z)) for y, z in pts]
        r = [self.bm.verts.new((w, y, z)) for y, z in pts]
        n = len(pts)
        for i in range(n):
            j = (i + 1) % n
            self._f((l[i], l[j], r[j], r[i]), part)
        self._f(l, part)
        self._f(r[::-1], part)


# Where RevolverCounter's number sits: the centre of the screen on the frame, read by the Unity wiring. Blender axes.
COUNTER_SCREEN = (.0135, -.128, .049)


def revolver():
    """The delivered Pistola's grip, trigger and guard under a new long-barrel revolver body. The barrel sits level with the
    lower chamber; ceramic frame, polymer grip, counter screen and biometric plate per GDD (D-74, D-75)."""
    open_new(r"Props\Props.blend")
    src = bpy.data.objects["Pistola"]
    bm = bmesh.new()
    bm.from_mesh(src.data)
    tag = bm.faces.layers.int.new("gun_part")   # the delivered mesh may already carry an attribute named "part"
    drop = [f for f in bm.faces if f.calc_center_median().z > .066 or (f.calc_center_median().x > .04 and f.calc_center_median().z > .05)]
    bmesh.ops.delete(bm, geom=drop, context="FACES")
    for f in bm.faces:
        f[tag] = 0
    # Quarter turn about Z keeps handedness and winding.
    for v in bm.verts:
        x, y, z = v.co
        v.co = (y, -(x - GRIP[0]), z - GRIP[2])
    names = ["kept"]
    G = Gun(bm, tag, names)
    axis_z = .05                # cylinder
    chamber = .019              # the lower chamber fires: barrel axis this far under the cylinder's
    bz = axis_z - chamber
    W = .027
    # Frame: a closed window around the cylinder, cut from flat ceramic planes.
    G.wedge([(.03, .0), (.03, .05), (.012, .088), (-.03, .094), (-.03, .004), (.0, -.004)], W, "ceramic")
    G.wedge([(-.03, .094), (-.092, .092), (-.092, .08), (-.03, .083)], W * .8, "ceramic")     # top strap over the cylinder
    G.wedge([(-.03, .013), (-.03, -.004), (-.092, -.004), (-.092, .013)], W, "ceramic")       # bottom strap
    G.wedge([(-.092, .092), (-.104, .08), (-.104, -.006), (-.092, -.006)], W, "ceramic")     # front post
    # Fluted six-chamber cylinder.
    G.prism(-.031, -.089, axis_z, .0305, .0305, "metal", sides=18, wave=lambda i: .87 if i % 3 == 1 else 1.0)
    for i in range(6):
        a = 2 * math.pi * (i + .5) / 6
        G.prism(-.0888, -.0896, axis_z + .019 * math.sin(a), .0068, .0068, "bore", sides=6, x=.019 * math.cos(a))
    G.prism(-.089, -.094, axis_z, .007, .007, "metal", sides=6)                               # crane pin
    # Long barrel: one slab with the frame, as tall as the front post where they meet and tapering to the muzzle, so it reads
    # as the gun's own body. The bore stays low, level with the lower chamber (D-74).
    length = .17
    y0, y1 = -.104, -.104 - length
    G.wedge([(y0 + .002, .08), (y0 + .002, -.006), (y1, bz - .022), (y1, bz + .03), (y1 + .05, .066), (y0 - .03, .08)], W * .92, "ceramic")
    G.prism(y1 + .004, y1 - .012, bz, .0185, .0175, "metal", sides=12)
    G.prism(y1 - .012, y1 - .0125, bz, .0105, .0105, "bore", sides=10)
    # Rib along the top, with the sight at its end, level with the top strap.
    # It follows the barrel's top line exactly, sunk into it, so no gap can open above the slab.
    G.wedge([(y0 + .002, .092), (y0 + .002, .074), (y0 - .03, .074), (y1 + .05, .06), (y1 + .004, .055), (y1 + .004, .066), (y1 + .05, .071), (y0 - .03, .085)], .009, "polymer")
    G.wedge([(y1 + .004, .062), (y1 + .004, .08), (y1 + .022, .068)], .006, "sight")
    G.box((-.009, -.03, .094), (-.004, -.016, .102), "polymer")
    G.box((.004, -.03, .094), (.009, -.016, .102), "polymer")
    # Ejector rod in a channel under the barrel.
    G.prism(y0, y1 + .06, -.006, .0058, .0058, "metal", sides=8)
    G.box((-.0095, y1 + .06, -.016), (.0095, y1 + .085, .004), "ceramic")
    # Shrouded hammer spur.
    G.wedge([(.03, .056), (.042, .07), (.04, .08), (.03, .074)], .009, "polymer")
    # Counter screen and biometric plate stand proud of the frame on both sides.
    for s in (-1, 1):
        x0, x1 = sorted((s * W / 2, s * (W / 2 + .0012)))
        G.box((x0 * .92 if s > 0 else x0, -.104 - .04, .036), (x1 if s > 0 else x1 * .92, -.104 - .008, .062), "counter")
        G.box((x0, -.01, .034), (x1, .018, .056), "bio")
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    for f in bm.faces:
        f.smooth = False
    me = bpy.data.meshes.new("CH_Revolver")
    bm.to_mesh(me)
    bm.free()
    for uv in list(me.uv_layers):
        me.uv_layers.remove(uv)
    for o in list(bpy.data.objects):
        bpy.data.objects.remove(o)
    ob = bpy.data.objects.new("CH_Revolver", me)
    bpy.context.scene.collection.objects.link(ob)
    layer = me.attributes["gun_part"].data
    parts = [names[layer[i].value] for i in range(len(me.polygons))]
    tris = sum(len(p.vertices) - 2 for p in me.polygons)
    B = hp.bake(me, parts, 1024, align_longest=True, edges=True)
    c = paint_revolver(B, bz, y0, y1)
    tex = MODELS + r"\Props\Textures\T_Revolver.png"
    hp.write(tex, "T_Revolver", 1024, [(B, c)])
    emit = np.zeros((B.n, 3), dtype=np.float32)
    lit = B.is_("bio") & (c.max(axis=1) > .4)
    emit[lit] = c[lit]
    hp.write(tex.replace(".png", "_Emission.png"), "T_Revolver_Emission", 1024, [(B, emit)])
    mat = bpy.data.materials.new("M_Revolver")
    mat.use_nodes = True
    node = mat.node_tree.nodes.new("ShaderNodeTexImage")
    node.image = bpy.data.images["T_Revolver"] if "T_Revolver" in bpy.data.images else None
    node.interpolation = "Closest"
    mat.node_tree.links.new(node.outputs["Color"], next(n for n in mat.node_tree.nodes if n.type == "BSDF_PRINCIPLED").inputs["Base Color"])
    me.materials.append(mat)
    _export_static(ob, MODELS + r"\Props\CH_Revolver.fbx")
    span = np.array([v.co[:] for v in me.vertices])
    return dict(tris=tris, length_m=round(float(np.ptp(span[:, 1])), 3), height_m=round(float(np.ptp(span[:, 2])), 3))


def paint_revolver(B, bz, y0, y1):
    """Matte ceramic and polymer, flat colour with hard edges: Signalis's method (D-153), with the game's own marks."""
    P = B.P
    CERAMIC, POLY, STEEL = X("#6f6c66"), X("#1b1c1e"), X("#3a3d40")
    PHOSPHOR, AMBER = X("#02DF8B"), X("#c98a2b")
    k = (np.clip(B.N @ np.array((.3, -.3, .9), dtype=np.float32) * .5 + .5, 0, 1) * .45 + .62)[:, None]
    c = B.fill(POLY)
    kept, cer, met = B.is_("kept"), B.is_("ceramic"), B.is_("metal")
    c[cer] = (CERAMIC * k)[cer]
    c[kept] = (POLY * k)[kept]
    c[met] = (STEEL * k)[met]
    c[B.is_("bore")] = X("#050506")
    c[B.is_("sight")] = AMBER
    # Hard dark lines on the polygon edges of ceramic and steel: the drawn look at low resolution.
    e = (cer | met) & (B.E < .0009)
    c[e] *= .55
    # Stippled polymer on the grip, ventilation cuts along the shroud.
    st = kept & (P[:, 2] < -.008) & ((((P[:, 1] * 260).astype(int) + (P[:, 2] * 260).astype(int)) % 2) == 0)
    c[st] *= .68
    vent = cer & (P[:, 1] < y0 - .05) & (P[:, 1] > y1 + .012) & (np.abs(B.N[:, 0]) > .7) & (np.abs(P[:, 2] - (bz + .008)) < .006) & (((P[:, 1] * 62) % 1) < .55)
    c[vent] = X("#08080a")
    # Stencils: maker on the shroud, service number on the frame.
    for s in (-1, 1):
        hp.stamp(B, c, (s * .0125, y1 + .05, bz - .012), (s, 0, 0), .0058, X("#c4bfae"), hp.text_bitmap("KESSLER"), wear=.1, seed=3)
        hp.stamp(B, c, (s * .0135, -.012, .012), (s, 0, 0), .0042, X("#c4bfae"), hp.text_bitmap("KS-M5"), wear=.1, seed=4)
    # Counter screen stays dark: RevolverCounter draws the live round count over it.
    c[B.is_("counter")] = X("#08241E")
    # Biometric plate: dark contact, one thin amber line and a lit dot: it reads Lukas's arm terminal.
    bio = B.is_("bio")
    c[bio] = X("#121314")
    c[bio & (np.abs(P[:, 2] - .04) < .0012)] = AMBER * 1.25
    c[bio & (((P[:, 1] - .012) ** 2 + (P[:, 2] - .049) ** 2) < .0022 ** 2)] = PHOSPHOR
    return hp.posterize(B, c, levels=20)


FALLEN = {"Hips": (-84, 0, 10), "Chest": (6, 0, 0), "Head": (4, 0, -24), "LeftUpperArm": (-10, -56, 0), "RightUpperArm": (-8, 34, 0), "LeftLowerArm": (-24, 0, 0), "RightLowerArm": (-10, 0, 0), "LeftUpperLeg": (-20, 8, 0), "RightUpperLeg": (-2, -8, 0), "LeftLowerLeg": (32, 0, 0), "RightLowerLeg": (4, 0, 0), "LeftFoot": (28, 0, 0), "RightFoot": (24, 0, 0), "loc": (0, 0.3, -.92)}
STILL = {"Neck": (10, 0, 2), "Head": (20, 6, 0), "LeftShoulder": (4, 0, 0), "RightShoulder": (6, 0, 0)}
STATICS = {
    "CH_Corpse_Staff": (FALLEN, "#6d6866", "#332b2c", "VITALIS", "VK-0233", .78),
    "CH_Corpse_Guard": (FALLEN, "#2c343a", "#191d21", "WERKSCHUTZ", "KS-118", .76),
    "CH_Corpse_Subject": (FALLEN, "#8d8a82", "#6a6862", "SUBJEKT", "SUBJ 12", .8),
    "CH_Mudo_Still": (STILL, None, None, None, None, 1.0),   # bare, like the enemy
    "CH_Lukas_Mudo": (STILL, "#355654", "#1a2527", "R-H-07", "R-H-07", .8),
}
DRESSED = {"shirt": {"chest", "spine", "uarm"}, "pants": {"hips", "thigh", "shin"}}
# Lukas become a Mudo keeps what he wore: the whole R-H-07 overall, torn open.
OVERALL = {"shirt": {"chest", "spine", "uarm", "farm"}, "pants": {"hips", "thigh", "shin"}}
STATIC_TRIS = 3500


def statics():
    """Corpses and decoys: the delivered Mudo in clothes of each group, posed and frozen. One texture: delivered albedo in the
    left half, clothing in the right, so scenes keep their single material."""
    out = {}
    for name, (pose, top, bottom, label, tag_text, tone) in STATICS.items():
        lukas_body = name == "CH_Lukas_Mudo"
        body, rig, parts = _mudo_body(OVERALL if lukas_body else DRESSED, torn=.3, thick=.016) if lukas_body else _mudo_body(DRESSED if top else None)
        me = body.data
        uv = me.uv_layers.active.data
        for p in me.polygons:
            if parts[p.index] == "skin":
                for li in p.loop_indices:
                    uv[li].uv = (uv[li].uv[0] * .5, uv[li].uv[1])
        layers = []
        if top:
            B = hp.bake(me, parts, 1024, offset=(1024, 0), atlas=2048, align_longest=True, skip=("skin",))
            B.Ns, B.ao, B.crease, B.mat = B.N, np.full(B.n, .9, dtype=np.float32), np.zeros(B.n, dtype=np.float32), np.zeros(B.n, dtype=np.uint8)
            S, T = B.is_("shirt"), B.is_("pants")
            c = B.fill(X(top))
            k = (ep._value(B) * 1.15 + .15)[:, None]
            c[S] = (X(top) * k)[S]
            c[T] = (X(bottom) * 1.25 * k)[T]
            _worn(B, c, S | T, 6)
            if lukas_body:
                # The overall's red trim at the belt and its seams, gone dark; soot and old blood where it tore.
                P = B.P
                c[(S | T) & (np.abs(P[:, 2] - 1.02) < .03)] = ep.CRIMSON * .8
                c[S & (np.abs(np.abs(P[:, 0]) - .1) < .006)] = X("#6e9fa0") * .5
                hp.grime(B, c, S | T, 14, .7)
            hp.stamp(B, c, (0, .13, 1.44), (0, 1, 0), .05, X("#9d9a90"), hp.text_bitmap(label), wear=.15, seed=2)
            hp.stamp(B, c, (.1, -.14, 1.46), (0, -1, .1), (.08, .018), X("#c4bfae"), hp.text_bitmap(tag_text))
            hp.blood(B, c, S | T, 8, coverage=.58 if lukas_body else .66)
            layers = [(B, hp.posterize(B, c, levels=20))]
        img = np.zeros((2048, 2048, 4), dtype=np.float32)
        img[:, :, 3] = 1
        src = bpy.data.images["Monster1_Albedo"].copy()
        src.scale(1024, 2048)
        half = np.array(src.pixels[:], dtype=np.float32).reshape(2048, 1024, 4)
        bpy.data.images.remove(src)
        half[:, :, :3] *= tone
        img[:, :1024] = half
        path = MODELS + rf"\Statics\Textures\{name}_Albedo.png"
        hp.write(path, name + "_Albedo", 2048, layers, existing=img)
        A = hb_anim.Author(rig)
        A.clip("pose", 2, {1: pose})
        bpy.context.scene.frame_set(1)
        bpy.context.view_layer.update()
        baked = bpy.data.meshes.new_from_object(body.evaluated_get(bpy.context.evaluated_depsgraph_get()))
        st = bpy.data.objects.new("static", baked)
        body.users_collection[0].objects.link(st)
        n = sum(len(p.vertices) - 2 for p in baked.polygons)
        if n > STATIC_TRIS:
            st.modifiers.new("dec", "DECIMATE").ratio = STATIC_TRIS / n
            bpy.context.view_layer.update()
            lean = bpy.data.meshes.new_from_object(st.evaluated_get(bpy.context.evaluated_depsgraph_get()))
            st.modifiers.clear()
            st.data = lean
        st.data.materials.clear()
        body.name = "src"
        st.name = st.data.name = name
        _export_static(st, MODELS + rf"\Statics\{name}.fbx")
        out[name] = sum(len(p.vertices) - 2 for p in st.data.polygons)
    return out


def metal_smooth():
    """URP reads metal from R and smoothness from A of one map; the team delivers metallic and roughness apart."""
    os.makedirs(MODELS + r"\Enemies\Textures", exist_ok=True)
    out = []
    for name, folder in (("Mudo", "Monster1"), ("Boca", "Monster2"), ("Alto", "Monster3")):
        base = NEW + rf"\Monsters\{folder}\Texture\{folder}_"
        rough = bpy.data.images.load(base + "Roughness.png")
        rough.colorspace_settings.name = "Non-Color"
        rough.scale(1024, 1024)
        r = np.array(rough.pixels[:], dtype=np.float32).reshape(1024, 1024, 4)[:, :, 0]
        bpy.data.images.remove(rough)
        m = np.zeros_like(r)
        if os.path.exists(base + "Metallic.png"):
            met = bpy.data.images.load(base + "Metallic.png")
            met.colorspace_settings.name = "Non-Color"
            met.scale(1024, 1024)
            m = np.array(met.pixels[:], dtype=np.float32).reshape(1024, 1024, 4)[:, :, 0]
            bpy.data.images.remove(met)
        img = np.zeros((1024, 1024, 4), dtype=np.float32)
        img[:, :, 0] = img[:, :, 1] = img[:, :, 2] = m
        img[:, :, 3] = 1 - r
        path = MODELS + rf"\Enemies\Textures\T_{name}_MetalSmooth.png"
        im = bpy.data.images.new(f"T_{name}_MetalSmooth", 1024, 1024, alpha=True)
        im.colorspace_settings.name = "Non-Color"
        im.alpha_mode = "STRAIGHT"
        im.pixels.foreach_set(img.ravel())
        im.filepath_raw = path
        im.file_format = "PNG"
        im.save()
        out.append(os.path.basename(path))
    return out


FN = {"Alto": alto, "Boca": boca, "Mudo": mudo, "Rastrero": rastrero, "Props": props, "Revolver": revolver, "Statics": statics, "MetalSmooth": metal_smooth}
