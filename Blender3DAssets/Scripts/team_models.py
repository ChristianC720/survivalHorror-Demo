"""Turns the team's delivery (Assets/_Project/Models/ModelsNew) into the FBX files the game uses. The delivered .blend files are
opened and never saved; their designs stay as delivered. This adds only what each one lacks to run in the game:
Mudo a rig and clips; Rastrero one mesh, UVs, paint, rig and clips; every enemy an Idle and a recycled form (D-130)."""
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
from hb_sculpt import MOUTH  # noqa: F401  (painters share the id)

X = hp.hex_col
PARTS_TEX = MODELS + r"\Enemies\Textures\T_RecycledParts.png"
SWATCH = {"metal": (0, X("#6d6a68")), "rust": (1, X("#5c2f1c")), "bandage": (2, X("#b6ae96")), "cloth": (3, X("#8d8a82")), "rivet": (4, X("#9a9894")), "dark": (5, X("#2a2c2e"))}


def open_new(rel):
    bpy.ops.wm.open_mainfile(filepath=NEW + "\\" + rel)
    bpy.context.scene.render.fps = 30
    bpy.context.scene.frame_set(1)


def parts_texture():
    """One small swatch per colour: recycled pieces need flat colour only, on a second material slot."""
    img = np.ones((8, 48, 4), dtype=np.float32)
    for i, c in SWATCH.values():
        img[:, i * 8:(i + 1) * 8, :3] = c
    hp.save(PARTS_TEX, "T_RecycledParts", img)


class Pieces:
    """Hand-built recycled pieces: each is rigid on one bone and winds outward. Joined as a second material slot."""

    def __init__(self):
        self.bm = bmesh.new()
        self.uv = self.bm.loops.layers.uv.new("UVMap")
        self.bones = []  # one per vertex

    def _face(self, verts, sw):
        f = self.bm.faces.new(verts)
        i = SWATCH[sw][0]
        for l in f.loops:
            l[self.uv].uv = ((i + .5) / 6, .5)
        return f

    def _v(self, p, bone):
        self.bones.append(bone)
        return self.bm.verts.new(p)

    def box(self, c, size, sw, bone):
        cx, cy, cz = c
        w, d, h = size[0] / 2, size[1] / 2, size[2] / 2
        vs = [self._v((cx + sx * w, cy + sy * d, cz + sz * h), bone) for sz in (-1, 1) for sy in (-1, 1) for sx in (-1, 1)]
        for q in ((0, 2, 3, 1), (4, 5, 7, 6), (0, 1, 5, 4), (2, 6, 7, 3), (0, 4, 6, 2), (1, 3, 7, 5)):
            self._face([vs[i] for i in q], sw)

    def spike(self, a, b, r, sw, bone, sides=5):
        a, b = Vector(a), Vector(b)
        d = (b - a).normalized()
        u = d.orthogonal().normalized()
        v = d.cross(u)
        ring = [self._v(a + (u * math.cos(2 * math.pi * i / sides) + v * math.sin(2 * math.pi * i / sides)) * r, bone) for i in range(sides)]
        tip = self._v(b, bone)
        for i in range(sides):
            self._face((ring[i], ring[(i + 1) % sides], tip), sw)

    def ring(self, c, rx, ry, h, sw, bone, tilt=0.0, seg=12):
        """Band around a body part. Two-sided, because the inside can show past a loose edge."""
        cx, cy, cz = c
        lo, hi = [], []
        for i in range(seg):
            a = 2 * math.pi * i / seg
            x, y = cx + rx * math.cos(a), cy + ry * math.sin(a)
            lo.append(self._v((x, y, cz - h / 2 + tilt * x), bone))
            hi.append(self._v((x, y, cz + h / 2 + tilt * x), bone))
        for i in range(seg):
            j = (i + 1) % seg
            self._face((lo[i], lo[j], hi[j], hi[i]), sw)

    def cone_band(self, z0, r0, z1, r1, c, sw, bone, seg=14, depth=.8):
        """Skirt: open and two-sided, so it never vanishes when seen from below."""
        for flip in (False, True):
            a_, b_ = [], []
            for i in range(seg):
                a = 2 * math.pi * i / seg
                ca, sa = math.cos(a), math.sin(a) * depth
                a_.append(self._v((c[0] + ca * r0, c[1] + sa * r0, z0), bone))
                b_.append(self._v((c[0] + ca * r1, c[1] + sa * r1, z1), bone))
            for i in range(seg):
                j = (i + 1) % seg
                q = (a_[i], a_[j], b_[j], b_[i])
                self._face(q[::-1] if flip else q, sw)

    def attach(self, body, keep_winding=True):
        """Joins into `body` (a skinned object) as slot 2 with rigid weights."""
        for f in self.bm.faces:
            f.material_index = 1
        if not keep_winding:
            bmesh.ops.recalc_face_normals(self.bm, faces=self.bm.faces)
        me = bpy.data.meshes.new("pieces")
        self.bm.to_mesh(me)
        self.bm.free()
        # Vertex indices keep creation order, so bones[i] still names vertex i.
        base = len(body.data.vertices)
        tmp = bpy.data.objects.new("pieces", me)
        body.users_collection[0].objects.link(tmp)
        for name in set(self.bones):
            g = tmp.vertex_groups.new(name=name)
            g.add([i for i, b in enumerate(self.bones) if b == name], 1.0, "REPLACE")
        mat = bpy.data.materials.get("M_RecycledParts") or bpy.data.materials.new("M_RecycledParts")
        while len(body.data.materials) < 1:
            body.data.materials.append(mat)
        body.data.materials.append(mat)
        tmp.data.materials.append(body.data.materials[0])
        tmp.data.materials.append(mat)
        vl = bpy.context.view_layer
        for o in vl.objects:
            o.select_set(False)
        body.select_set(True)
        tmp.select_set(True)
        vl.objects.active = body
        tmp.matrix_world = body.matrix_world
        with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
            bpy.ops.object.join()
        return base


def recycled_copy(body, name):
    ob = body.copy()
    ob.data = body.data.copy()
    ob.name = ob.data.name = name
    body.users_collection[0].objects.link(ob)
    return ob


def thin(ob, amount):
    """Recycled Boca: 'bone-thin' (D-130). Vertices sink along their normals; the outline stays hers."""
    me = ob.data
    me.update()
    for v in me.vertices:
        v.co -= v.normal * amount


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
    rec = recycled_copy(body, "CH_Alto_Recycled")
    # Metal vertebra along the spine, metal extensions at the elbows (D-130). Measured on the delivered rest pose.
    P = Pieces()
    B = lambda n: rig.data.bones[n]
    a, b = B("spine").head_local, B("neck").tail_local
    for i in range(10):
        k = i / 9
        c = a.lerp(b, k)
        P.box((0, c.y + .1, c.z), (.08, .05, .06), "rust" if i % 3 == 2 else "metal", "spine" if k < .3 else "chest" if k < .8 else "neck")
    for s, side in ((1, "L"), (-1, "R")):
        el, wr = B("forearm_" + side).head_local, B("forearm_" + side).tail_local
        P.spike(el, el + (wr - el).normalized() * .05 + Vector((s * .03, .11, -.42)), .03, "metal", "forearm_" + side)
    P.attach(rec)
    return finish("Alto", rig, [body, rec])


def boca():
    open_new(r"Monsters\Monster2\Monster2.blend")
    rig = bpy.data.objects["Monster2_Rig"]
    rig.name = "Rig_Boca"
    body = bpy.data.objects["Monster2_Body"]
    body.name = body.data.name = "CH_Boca"
    idle_from(rig, "Deambular", "AN_Boca_Idle")
    rename_actions({"Deambular": "AN_Boca_Wander", "Caza": "AN_Boca_Hunt", "Mordida": "AN_Boca_Attack", "Atacar": "AN_Boca_Attack_Claw", "Grito": "AN_Boca_Scream", "Aturdido": "AN_Boca_Stunned", "Muerte": "AN_Boca_Death"})
    rec = recycled_copy(body, "CH_Boca_Recycled")
    thin(rec, .014)
    return finish("Boca", rig, [body, rec])


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


def mudo():
    open_new(r"Monsters\Monster1\Monster1.blend")
    body = bpy.data.objects["Monster1_Body"]
    body.name = body.data.name = "CH_Mudo"
    me = body.data
    co = np.array([v.co[:] for v in me.vertices], dtype=np.float32)
    j = {k: _snap(co, v) if k not in ("hips", "spine", "chest", "neck", "head", "crown") else v for k, v in MUDO_J.items()}
    j["ball"] = (.31, float(co[(np.abs(co[:, 0] - .31) < .05) & (co[:, 2] < .05), 1].mean()), .03)
    j["toe"] = (.325, float(co[(np.abs(co[:, 0] - .32) < .08) & (co[:, 2] < .04), 1].min()) + .01, .015)
    t = _table(co, j)
    col = body.users_collection[0]
    rig = hm.make_rig("Rig_Mudo", t, col)
    # Arm or torso: nearer the arm's bones, measured against each part's own thickness.
    arms = [(np.array(t[s + b][0]), np.array(t[s + b][1])) for s in ("Left", "Right") for b in ("UpperArm", "LowerArm", "Hand", "Fingers")]
    da = np.min([hm.segment_distance(co.astype(np.float64), a, b) for a, b in arms], axis=0)
    limb = np.where((da < .085) & (np.abs(co[:, 0]) > .17), np.where(co[:, 0] > 0, 1, 2), 0)
    _limb_attr(me, limb)
    body.parent = rig
    mod = body.modifiers.new("Armature", "ARMATURE")
    mod.object = rig
    hm.auto_weights(body, rig)
    rec = recycled_copy(body, "CH_Mudo_Recycled")
    hz = 1.8
    P = Pieces()
    # Silent-Hill-nurse form of D-130: riveted plate over the face, bandaged head, clinical skirt.
    fy = float(co[(np.abs(co[:, 0]) < .03) & (np.abs(co[:, 2] - 1.96) < .06), 1].min())
    P.box((0, fy - .004, 1.95), (.15, .02, .2), "metal", "Head")
    for x in (-.058, .058):
        for z in (1.87, 2.03):
            P.box((x, fy - .016, z), (.014, .01, .014), "rivet", "Head")
    for i in range(7):
        z = 1.84 + i * .042
        r = .118 - abs(i - 2.6) * .009
        P.ring((0, -.03, z), r, r * 1.04, .036, "bandage", "Head", tilt=.14 if i % 2 else -.12)
    P.cone_band(1.04, .19, .56, .3, (0, -.005), "cloth", "Hips")
    P.attach(rec)
    A = _author(rig)
    enemy_mudo(A)
    rig.animation_data.action = None
    return finish("Mudo", rig, [body, rec])


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
    holes = hp.fbm(P, 8, 3, 14) > .58
    sh = ((Tr & (P[:, 2] > 1.02)) | (Ar & (P[:, 2] > 1.2)) | (Tt & (P[:, 2] > .98))) & ~holes
    tr = ((Lg & (P[:, 2] > .3)) | (Tr & (P[:, 2] <= 1.02)) | (Tt & (P[:, 2] <= .98))) & ~(hp.fbm(P, 7, 3, 6) > .66)
    t = (ep._value(B) * 1.2 + .12)[:, None]
    stripe = ((P[:, 0] * 26) % 1) < .4
    c[sh] = (np.where(stripe[:, None], X("#5d7382"), X("#b5b6ae")) * t)[sh]
    hp.grime(B, c, sh, 2, .55)
    c[tr] = (X("#232c31") * t)[tr]
    hp.grime(B, c, tr, 3, .4)
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
    hp.blood(B, c, sh | tr, 17, coverage=.8)
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
    rec = recycled_copy(body, "CH_Rastrero_Recycled")
    P = Pieces()
    for i in range(7):
        k = i / 6
        P.box((0, .135 - k * .3, 1.12 + k * .62), (.08, .05, .07), "rust" if i % 3 == 2 else "metal", "Spine" if k < .3 else "Chest")
    for s in (-1, 1):
        P.spike((s * .1, -.68, 1.36), (s * .03, -.92, 1.2), .026, "metal", "Head")
    P.attach(rec)
    A = _author(rig)
    enemy_rastrero(A)
    rig.animation_data.action = None
    return finish("Rastrero", rig, [body, rec])


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

PROPS = {"Municion": "CH_Item_Ammo", "Autoinyector": "CH_Item_Heal", "Stunject": "CH_Item_Spike", "Llave_Maestra": "CH_Item_KeyMandatory", "Tarjeta_Acceso": "CH_Item_KeyOptional", "Aturdidor": "CH_Item_Aturdidor", "Terminal": "CH_Item_Terminal"}


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
COUNTER_SCREEN = (.0147, -.123, .032)


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
    # Long barrel under a square shroud with ventilation cuts, and a muzzle crown ahead of it.
    length = .155
    y0, y1 = -.104, -.104 - length
    G.box((-W / 2 + .001, y1, bz - .016), (W / 2 - .001, y0, bz + .018), "ceramic")
    G.prism(y1 + .002, y1 - .012, bz, .0125, .0115, "metal", sides=10)
    G.prism(y1 - .012, y1 - .0125, bz, .0065, .0065, "bore", sides=8)
    # Rib and sights.
    G.box((-.005, y1 + .002, bz + .018), (.005, y0 + .012, bz + .023), "polymer")
    G.wedge([(y1 + .003, bz + .022), (y1 + .003, bz + .037), (y1 + .018, bz + .022)], .005, "sight")
    G.box((-.009, -.03, .094), (-.004, -.016, .102), "polymer")
    G.box((.004, -.03, .094), (.009, -.016, .102), "polymer")
    # Ejector rod under the barrel, shrouded at the front.
    G.prism(-.104, y1 + .05, bz - .023, .0052, .0052, "metal", sides=8)
    G.box((-.009, y1 + .05, bz - .03), (.009, y1 + .08, bz - .016), "ceramic")
    # Shrouded hammer spur.
    G.wedge([(.03, .056), (.042, .07), (.04, .08), (.03, .074)], .009, "polymer")
    # Counter screen and biometric plate stand proud of the frame on both sides.
    for s in (-1, 1):
        x0, x1 = sorted((s * W / 2, s * (W / 2 + .0012)))
        G.box((x0, -.104 - .036, bz - .01), (x1, -.104 - .006, bz + .012), "counter")
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
    vent = cer & (P[:, 1] < y0 - .045) & (P[:, 1] > y1 + .006) & (np.abs(B.N[:, 0]) > .7) & (np.abs(P[:, 2] - (bz + .004)) < .0052) & (((P[:, 1] * 62) % 1) < .55)
    c[vent] = X("#08080a")
    # Stencils: maker on the shroud, service number on the frame.
    for s in (-1, 1):
        hp.stamp(B, c, (s * .0135, y1 + .05, bz - .009), (s, 0, 0), .0052, X("#c4bfae"), hp.text_bitmap("KESSLER"), wear=.1, seed=3)
        hp.stamp(B, c, (s * .0135, -.012, .012), (s, 0, 0), .0042, X("#c4bfae"), hp.text_bitmap("KS-M5"), wear=.1, seed=4)
    # Counter screen stays dark: RevolverCounter draws the live round count over it.
    c[B.is_("counter")] = X("#08241E")
    # Biometric plate: dark contact, one thin amber line and a lit dot: it reads Lukas's arm terminal.
    bio = B.is_("bio")
    c[bio] = X("#121314")
    c[bio & (np.abs(P[:, 2] - .04) < .0012)] = AMBER * 1.25
    c[bio & (((P[:, 1] - .012) ** 2 + (P[:, 2] - .049) ** 2) < .0022 ** 2)] = PHOSPHOR
    return hp.posterize(B, c, levels=20)


def mudo_still():
    """The intro's decoy: the delivered Mudo, frozen in its Still pose."""
    mudo()
    open_new(r"Monsters\Monster1\Monster1.blend")
    body = bpy.data.objects["Monster1_Body"]
    body.name = body.data.name = "CH_Mudo_Still"
    co = np.array([v.co[:] for v in body.data.vertices], dtype=np.float32)
    j = {k: _snap(co, v) if k not in ("hips", "spine", "chest", "neck", "head", "crown") else v for k, v in MUDO_J.items()}
    j["ball"] = (.31, float(co[(np.abs(co[:, 0] - .31) < .05) & (co[:, 2] < .05), 1].mean()), .03)
    j["toe"] = (.325, float(co[(np.abs(co[:, 0] - .32) < .08) & (co[:, 2] < .04), 1].min()) + .01, .015)
    t = _table(co, j)
    rig = hm.make_rig("Rig", t, body.users_collection[0])
    arms = [(np.array(t[s + b][0]), np.array(t[s + b][1])) for s in ("Left", "Right") for b in ("UpperArm", "LowerArm", "Hand", "Fingers")]
    da = np.min([hm.segment_distance(co.astype(np.float64), a, b) for a, b in arms], axis=0)
    _limb_attr(body.data, np.where((da < .085) & (np.abs(co[:, 0]) > .17), np.where(co[:, 0] > 0, 1, 2), 0))
    body.parent = rig
    body.modifiers.new("Armature", "ARMATURE").object = rig
    hm.auto_weights(body, rig)
    A = hb_anim.Author(rig)
    A.clip("pose", 2, {1: {"Neck": (10, 0, 2), "Head": (20, 6, 0), "LeftShoulder": (4, 0, 0), "RightShoulder": (6, 0, 0)}})
    bpy.context.scene.frame_set(1)
    bpy.context.view_layer.update()
    baked = bpy.data.meshes.new_from_object(body.evaluated_get(bpy.context.evaluated_depsgraph_get()))
    st = bpy.data.objects.new("CH_Mudo_Still", baked)
    body.name = "src"
    st.name = "CH_Mudo_Still"
    body.users_collection[0].objects.link(st)
    _export_static(st, MODELS + r"\Statics\CH_Mudo_Still.fbx")
    return len(baked.polygons)


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


FN = {"Alto": alto, "Boca": boca, "Mudo": mudo, "Rastrero": rastrero, "Props": props, "Revolver": revolver, "MudoStill": mudo_still, "MetalSmooth": metal_smooth}
