"""Builds one enemy: run with ENEMY set to Alto, Mudo, Boca or Rastrero. Writes Monsters/<Name>/<Name>.blend."""
import importlib
import math
import os
import sys

import bmesh
import bpy
from mathutils import Vector

ROOT = r"C:\Users\Chris\Heartbeat\Blender3DAssets"
sys.path.insert(0, ROOT + r"\Scripts")
import enemy_paint as ep
import hb_monster as hm
import hb_paint as hp

importlib.reload(hm)
importlib.reload(hp)
importlib.reload(ep)


def build_alto(col):
    t = hm.humanoid_bones(hip=1.24, spine=0.78, neck=0.13, head=0.30, shoulder=0.25, upper_arm=0.66, lower_arm=0.66, hand=0.18, finger=0.22, hip_w=0.12, upper_leg=0.60, lower_leg=0.60, foot=0.26, arm_out=0.16)
    r = dict(hips=(.13, .10), waist=(.10, .08), ribs=(.15, .10), chest=(.18, .11), neck=(.05, .05), shoulder=(.065, .065), elbow=(.042, .042), wrist=(.032, .032), palm=(.05, .022), thigh=(.08, .08), knee=(.05, .055), ankle=(.038, .042), foot=(.05, .06), toe=(.045, .03), finger=(.016, .016))
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    skin = hm.skin_parts("CH_Alto", chains, col, 5200)

    def extras(bm, faces, rng):
        hm.blob(bm, (0, -.03, hd.z + .15), .095, .115, .16, "skin", faces)
        hm.blob(bm, (0, -.09, hd.z + .03), .062, .058, .06, "skin", faces)   # jaw
        for i in range(7):
            z = 1.42 + i * .1
            hm.cone(bm, (0, .08, z), (0, .19 - .004 * abs(i - 3), z + .05), .022, "bone", faces=faces)
        for s in (-1, 1):
            hm.cone(bm, (s * .13, .07, 1.92), (s * .2, .17, 2.0), .02, "bone", faces=faces)
        vest = lambda c: 1.46 < c.z < 2.0 and abs(c.x) < .26
        pants = lambda c: .6 < c.z < 1.28 and abs(c.x) < .28
        hm.shell(bm, vest, .012, "vest", .03, rng, faces)
        bm.normal_update()
        hm.shell(bm, pants, .008, "pants", .03, rng, faces)
        hm.claws(bm, t, faces, .24, .014)
        for s in (-1, 1):
            hm.cone(bm, (s * .13, -.15, 1.98), (s * .02, -.16, 1.48), .014, "strap", sides=3, faces=faces)
            hm.bmesh_box(bm, (s * .085, -.165, 1.66), (.08, .035, .1), "pouch", faces)
        for i in range(9):
            a = i / 8 * 6.28
            x, y = .17 * math.cos(a), .12 * math.sin(a)
            hm.cone(bm, (x, y, 1.5), (x * 1.08, y * 1.1, 1.5 - rng.uniform(.14, .34)), .022, "vest", sides=3, faces=faces)
        for i in range(5):
            a = -.05 + i * .025
            hm.cone(bm, (a, -.15, hd.z + .045), (a, -.155, hd.z + .08), .007, "tooth", sides=3, faces=faces)

    return t, skin, extras, {}, ("bone", "tooth")


def build_mudo(col):
    t = hm.humanoid_bones(hip=.96, spine=.5, neck=.11, head=.24, shoulder=.19, upper_arm=.3, lower_arm=.27, hand=.1, finger=.09, hip_w=.10, upper_leg=.46, lower_leg=.44, foot=.2)
    r = dict(hips=(.125, .09), waist=(.095, .07), ribs=(.125, .085), chest=(.145, .09), neck=(.04, .04), shoulder=(.042, .042), elbow=(.032, .032), wrist=(.026, .025), palm=(.04, .018), thigh=(.068, .068), knee=(.044, .048), ankle=(.033, .036), foot=(.042, .056), toe=(.038, .026), finger=(.013, .013))
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    skin = hm.skin_parts("CH_Mudo", chains, col, 3200)

    def extras(bm, faces, rng):
        hm.blob(bm, (0, -.005, hd.z + .14), .08, .095, .12, "skin", faces)
        hm.blob(bm, (0, -.04, hd.z + .05), .056, .06, .06, "skin", faces)      # jaw: a head, not an egg
        shirt = lambda c: 1.0 < c.z < 1.5 or (.72 < c.z < 1.5 and abs(c.x) > .2)
        trousers = lambda c: .1 < c.z < 1.02 and abs(c.x) < .2
        hm.shell(bm, shirt, .011, "shirt", 0, rng, faces)
        bm.normal_update()
        hm.shell(bm, trousers, .009, "trousers", 0, rng, faces)
        hm.claws(bm, t, faces, .08, .011, part="finger")

    return t, skin, extras, {}, ()


def build_boca(col, thin=1.0):
    t = hm.humanoid_bones(hip=.86, spine=.56, neck=.08, head=.3, shoulder=.2, upper_arm=.3, lower_arm=.32, hand=.12, finger=.13, hip_w=.11, upper_leg=.4, lower_leg=.42, foot=.22)
    r = dict(hips=(.125, .095), waist=(.105, .08), ribs=(.14, .1), chest=(.16, .11), neck=(.085, .08), shoulder=(.052, .052), elbow=(.032, .034), wrist=(.028, .026), palm=(.04, .02), thigh=(.062, .066), knee=(.044, .048), ankle=(.033, .036), foot=(.04, .052), toe=(.036, .026), finger=(.014, .014))
    r = {k: (v[0] * thin, v[1] * thin) for k, v in r.items()}
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    # Two lobes split from the neck: the head opens along the middle (D-152).
    skin = hm.skin_parts("CH_Boca", chains, col, 5000)
    jaw = {"JawL": ((.03, -.02, hd.z), (.12, -.01, hd.z + .28), "Head"), "JawR": ((-.03, -.02, hd.z), (-.12, -.01, hd.z + .28), "Head")}

    def extras(bm, faces, rng):
        for s in (-1, 1):
            # Thick halves of one head, barely parted: taller and narrower reads as ears.
            hm.blob(bm, (s * .074, -.01, hd.z + .11), .082, .115, .165, "skin", faces, rings=7, tilt=s * .13)
        rags = lambda c: .6 < c.z < 1.06 and abs(c.x) < .2
        hm.shell(bm, rags, .009, "rags", .02, rng, faces)
        hm.claws(bm, t, faces, .16, .012)
        for s in (-1, 1):
            for i in range(7):
                z = hd.z + .03 + i * .034
                x = s * (.018 + i * .009)
                hm.cone(bm, (x, -.09, z), (x - s * .03, -.11, z - .012), .009, "tooth" + ("L" if s > 0 else "R"), sides=3, faces=faces)

    overrides = {"JawL": ("toothL",), "JawR": ("toothR",)}
    return t, skin, extras, jaw, overrides


def build_rastrero(col):
    t = hm.humanoid_bones(hip=.88, spine=.58, neck=.1, head=.24, shoulder=.2, upper_arm=.36, lower_arm=.36, hand=.12, finger=.14, hip_w=.12, upper_leg=.4, lower_leg=.4, foot=.2, arm_out=.2)
    r = dict(hips=(.115, .085), waist=(.09, .07), ribs=(.13, .09), chest=(.15, .095), neck=(.042, .042), shoulder=(.044, .044), elbow=(.03, .032), wrist=(.025, .025), palm=(.042, .018), thigh=(.06, .062), knee=(.04, .044), ankle=(.028, .032), foot=(.036, .048), toe=(.034, .024), finger=(.013, .013))
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    skin = hm.skin_parts("CH_Rastrero", chains, col, 4400)

    def extras(bm, faces, rng):
        hm.blob(bm, (0, -.01, hd.z + .12), .09, .105, .125, "skin", faces)
        hm.blob(bm, (0, -.08, hd.z + .2), .062, .042, .05, "skin", faces)   # chin on top: inverted face
        shirt = lambda c: 1.0 < c.z < 1.46 and abs(c.x) < .34
        pants = lambda c: .35 < c.z < 1.02 and abs(c.x) < .24
        hm.shell(bm, shirt, .01, "shirt", .03, rng, faces)
        bm.normal_update()
        hm.shell(bm, pants, .008, "pants", .03, rng, faces)
        hm.claws(bm, t, faces, .18, .013)
        for i in range(10):
            z = .98 + i * .052
            hm.cone(bm, (0, .09, z), (0, .16, z + .04), .017, "bone", faces=faces)
        for i in range(6):
            a = -.05 + i * .02
            hm.cone(bm, (a, -.085, hd.z + .165), (a, -.1, hd.z + .13), .007, "tooth", sides=3, faces=faces)
            hm.cone(bm, (a, -.085, hd.z + .1), (a, -.1, hd.z + .135), .007, "tooth", sides=3, faces=faces)

    return t, skin, extras, {}, ("bone", "tooth")


def recycled(name, base, t):
    """Each recycled type changes its silhouette (GDD Personajes, D-130); the metal is the laboratory's (D-152)."""
    extras0 = base
    hd = Vector(t["Head"][0])

    def spine_metal(bm, faces, z0, z1, n):
        for i in range(n):
            z = z0 + (z1 - z0) * i / max(n - 1, 1)
            hm.bmesh_box(bm, (0, .1, z), (.09, .05, (z1 - z0) / n * .8), "metal", faces)

    if name == "Alto":
        def extras(bm, faces, rng):
            extras0(bm, faces, rng)
            spine_metal(bm, faces, 1.3, 2.32, 11)   # vertebra lengthens the spine
            for side, s_ in (("Left", 1), ("Right", -1)):
                el = Vector(t[side + "UpperArm"][1])
                hm.cone(bm, el, el + Vector((s_ * .05, .1, -.5)), .03, "armmetal", sides=4, faces=faces)
    elif name == "Rastrero":
        def extras(bm, faces, rng):
            extras0(bm, faces, rng)
            spine_metal(bm, faces, .98, 1.5, 6)
            for s_ in (-1, 1):                       # pincers fused to the inverted face, like chelicerae
                hm.cone(bm, (s_ * .05, -.1, hd.z + .12), (s_ * .012, -.3, hd.z + .03), .022, "metal", sides=4, faces=faces)
    elif name == "Mudo":
        def extras(bm, faces, rng):
            extras0(bm, faces, rng)
            hm.skirt(bm, .98, .5, .15, .2, "skirt", faces, rng=rng)
            hm.bmesh_box(bm, (0, -.1, hd.z + .12), (.15, .03, .2), "metal", faces)
            for x in (-.06, .06):
                for z in (.05, .19):
                    hm.bmesh_box(bm, (x, -.118, hd.z + z), (.014, .01, .014), "rust", faces)
    else:
        extras = extras0

    return extras


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
    fn = {"Alto": build_alto, "Mudo": build_mudo, "Boca": build_boca, "Rastrero": build_rastrero}[name]
    t, skin, extras, extra_bones, over = fn(col)
    hz = t["Head"][0][2]
    tex = ROOT + rf"\Monsters\{name}\Texture"
    rig = hm.make_rig("Rig_" + name, t, col, extra_bones)
    os.makedirs(ROOT + rf"\Monsters\{name}\Texture", exist_ok=True)
    tris = {}

    def finish(mesh_name, skin_data, ex, painter):
        ob, parts, n = hm.assemble(mesh_name, rig, skin_data, ex, col)
        overrides = {}
        if isinstance(over, dict):
            for bone, ps in over.items():
                overrides[bone] = hm.verts_of_parts(ob, parts, ps)
        hm.auto_weights(ob, rig, overrides)
        B = hp.bake(ob.data, parts, hm.TEX, align_longest=True)
        im = hp.save(tex + rf"\{mesh_name}_Albedo.png", mesh_name + "_Albedo", hp.write(tex + rf"\{mesh_name}_Albedo.png", mesh_name + "_Albedo", hm.TEX, [(B, painter(B))]))
        hm.finish_material(ob, im)
        tris[mesh_name] = n
        return parts

    finish("CH_" + name, skin, extras, lambda B: getattr(ep, name.lower())(B, hz))
    # Recycled Boca is bone-thin: same chains with slimmer radii. The others keep the body and gain metal.
    _, skin2, ex2, _, _ = fn(col) if name != "Boca" else build_boca(col, thin=.72)
    finish(f"CH_{name}_Recycled", skin2, recycled(name, ex2, t), lambda B: ep.recycled(name, B, hz))
    bpy.context.scene.render.fps = 30
    bpy.ops.wm.save_mainfile()
    return tris, []
