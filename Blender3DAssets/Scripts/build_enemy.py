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
import hb_monster as hm

importlib.reload(hm)

SKIN = (0.46, 0.42, 0.40)
DARK = (0.10, 0.04, 0.06)      # palette #270a16 family
BLOOD = (0.2, 0.05, 0.08)      # palette #51111f family
MOUTH = (0.24, 0.05, 0.08)
BONE = (0.56, 0.52, 0.44)
TOOTH = (0.72, 0.68, 0.56)


def stain(c, seed, chance=0.1):
    v = abs(math.sin(c.x * 37.1 + c.y * 91.7 + c.z * 53.3 + seed))
    return v < chance


def skin_tone(c, n, base, seed=0):
    # Light from above keeps volumes readable in the dark rooms; darker lower down and in the folds.
    shade = 0.82 + 0.18 * max(0, n.z)
    col = tuple(x * shade for x in base)
    if stain(c, seed, 0.06):
        # Mixed, not replaced: whole dark faces read as squares at this polygon size.
        tint = BLOOD if stain(c, seed + 3, 0.5) else DARK
        col = tuple(a * 0.55 + b * 0.45 for a, b in zip(col, tint))
    return col


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
        hm.shell(bm, vest, .012, "vest", .1, rng, faces)
        bm.normal_update()
        hm.shell(bm, pants, .008, "pants", .12, rng, faces)
        hm.claws(bm, t, faces, .24, .014)
        for s in (-1, 1):
            hm.cone(bm, (s * .13, -.15, 1.98), (s * .02, -.16, 1.48), .014, "strap", sides=3, faces=faces)
        for i in range(5):
            a = -.05 + i * .025
            hm.cone(bm, (a, -.15, hd.z + .045), (a, -.155, hd.z + .08), .007, "tooth", sides=3, faces=faces)

    def color(c, n, part):
        if part == "vest":
            return (.10, .12, .14) if n.z < .75 else (.14, .16, .18)
        if part == "pants":
            return (.09, .10, .12)
        if part == "strap":
            return (.05, .05, .06)
        if part == "bone":
            return BONE
        if part == "claw":
            return (.16, .12, .10)
        if part == "tooth":
            return TOOTH
        if c.z > hd.z + .02 and c.y < -.09 and abs(c.x) < .05 and c.z < hd.z + .1:
            return MOUTH
        if abs(c.x) < .013 and c.y < -.05 and c.z > hd.z + .11:
            return BLOOD                                     # split forehead
        if .03 < abs(c.x) < .07 and c.y < -.08 and hd.z + .12 < c.z < hd.z + .17:
            return DARK                                      # sockets
        # GDD: iridescent purple on bare skin.
        p = (.40 + .05 * math.sin(c.z * 9), .36, .44 + .06 * math.sin(c.z * 7 + c.x * 9))
        return skin_tone(c, n, p, 1)

    return t, skin, extras, color, {}, ("bone", "tooth")


def build_mudo(col):
    t = hm.humanoid_bones(hip=.96, spine=.5, neck=.11, head=.24, shoulder=.19, upper_arm=.3, lower_arm=.27, hand=.1, finger=.09, hip_w=.10, upper_leg=.46, lower_leg=.44, foot=.2)
    r = dict(hips=(.14, .10), waist=(.115, .085), ribs=(.15, .105), chest=(.165, .11), neck=(.05, .05), shoulder=(.055, .055), elbow=(.04, .04), wrist=(.032, .03), palm=(.042, .02), thigh=(.08, .08), knee=(.052, .055), ankle=(.04, .042), foot=(.048, .06), toe=(.042, .028), finger=(.014, .014))
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    skin = hm.skin_parts("CH_Mudo", chains, col, 3200)

    def extras(bm, faces, rng):
        hm.blob(bm, (0, 0, hd.z + .12), .088, .105, .135, "skin", faces)
        shirt = lambda c: 1.0 < c.z < 1.5 or (.72 < c.z < 1.5 and abs(c.x) > .2)
        trousers = lambda c: .1 < c.z < 1.02 and abs(c.x) < .2
        hm.shell(bm, shirt, .011, "shirt", .02, rng, faces)
        bm.normal_update()
        hm.shell(bm, trousers, .009, "trousers", .03, rng, faces)
        hm.claws(bm, t, faces, .08, .011, part="finger")

    def color(c, n, part):
        shade = .8 + .2 * max(0, n.z)
        if part == "shirt":
            return tuple(x * shade for x in ((.30, .30, .30) if not hm_stain(c) else DARK))
        if part == "trousers":
            return tuple(x * shade for x in (.20, .20, .21))
        if part == "finger":
            return tuple(x * shade for x in (.5, .46, .43))
        # Faceless: a smooth pale head, with a slight hollow where the eyes should be.
        if c.z > hd.z and c.y < -.05 and .02 < abs(c.x) < .06 and hd.z + .07 < c.z < hd.z + .13:
            return tuple(x * .78 for x in SKIN)
        return tuple(x * shade for x in (.5, .46, .43))

    def hm_stain(c):
        return stain(c, 4, 0.05)

    return t, skin, extras, color, {}, ()


def build_boca(col):
    t = hm.humanoid_bones(hip=.86, spine=.56, neck=.08, head=.3, shoulder=.2, upper_arm=.3, lower_arm=.32, hand=.12, finger=.13, hip_w=.11, upper_leg=.4, lower_leg=.42, foot=.22)
    r = dict(hips=(.115, .09), waist=(.09, .07), ribs=(.125, .09), chest=(.145, .1), neck=(.075, .075), shoulder=(.05, .05), elbow=(.032, .034), wrist=(.028, .026), palm=(.04, .02), thigh=(.066, .07), knee=(.055, .058), ankle=(.04, .044), foot=(.05, .06), toe=(.046, .03), finger=(.014, .014))
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    # Two lobes split from the neck: the head opens along the middle (D-152).
    skin = hm.skin_parts("CH_Boca", chains, col, 5000)
    jaw = {"JawL": ((.03, -.02, hd.z), (.12, -.01, hd.z + .28), "Head"), "JawR": ((-.03, -.02, hd.z), (-.12, -.01, hd.z + .28), "Head")}

    def extras(bm, faces, rng):
        for s in (-1, 1):
            hm.blob(bm, (s * .068, -.02, hd.z + .13), .075, .105, .2, "skin", faces)
        rags = lambda c: .6 < c.z < 1.06 and abs(c.x) < .2
        hm.shell(bm, rags, .009, "rags", .16, rng, faces)
        hm.claws(bm, t, faces, .16, .012)
        for s in (-1, 1):
            for i in range(7):
                z = hd.z + .05 + i * .042
                x = s * (.03 + i * .012)
                hm.cone(bm, (x, -.08, z), (x - s * .035, -.09, z - .012), .009, "tooth" + ("L" if s > 0 else "R"), sides=3, faces=faces)

    def color(c, n, part):
        if part == "rags":
            return (.17, .16, .15)
        if part.startswith("tooth"):
            return TOOTH
        if part == "claw":
            return (.16, .12, .10)
        inner = c.z > hd.z and c.y < -.015 and n.x * c.x < 0
        if inner or (c.z > hd.z and abs(c.x) < .045 and c.y < 0):
            return MOUTH if not stain(c, 2, .3) else DARK
        if c.z > hd.z + .02 and abs(c.x) < .05:
            return DARK
        return skin_tone(c, n, SKIN, 2)

    overrides = {"JawL": ("toothL",), "JawR": ("toothR",)}
    return t, skin, extras, color, jaw, overrides


def build_rastrero(col):
    t = hm.humanoid_bones(hip=.88, spine=.58, neck=.1, head=.24, shoulder=.2, upper_arm=.36, lower_arm=.36, hand=.12, finger=.14, hip_w=.12, upper_leg=.4, lower_leg=.4, foot=.2, arm_out=.2)
    r = dict(hips=(.14, .1), waist=(.115, .085), ribs=(.16, .11), chest=(.185, .12), neck=(.055, .055), shoulder=(.065, .065), elbow=(.042, .044), wrist=(.033, .032), palm=(.05, .022), thigh=(.085, .088), knee=(.055, .06), ankle=(.04, .046), foot=(.052, .06), toe=(.048, .03), finger=(.015, .015))
    chains = hm.body_chains(t, r)
    hd = Vector(t["Head"][0])
    skin = hm.skin_parts("CH_Rastrero", chains, col, 4400)

    def extras(bm, faces, rng):
        hm.blob(bm, (0, -.01, hd.z + .12), .09, .105, .125, "skin", faces)
        hm.blob(bm, (0, -.08, hd.z + .2), .062, .042, .05, "skin", faces)   # chin on top: inverted face
        shirt = lambda c: 1.0 < c.z < 1.46 and abs(c.x) < .34
        pants = lambda c: .35 < c.z < 1.02 and abs(c.x) < .24
        hm.shell(bm, shirt, .01, "shirt", .14, rng, faces)
        bm.normal_update()
        hm.shell(bm, pants, .008, "pants", .12, rng, faces)
        hm.claws(bm, t, faces, .18, .013)
        for i in range(10):
            z = .98 + i * .052
            hm.cone(bm, (0, .09, z), (0, .16, z + .04), .017, "bone", faces=faces)
        for i in range(6):
            a = -.05 + i * .02
            hm.cone(bm, (a, -.085, hd.z + .165), (a, -.1, hd.z + .13), .007, "tooth", sides=3, faces=faces)
            hm.cone(bm, (a, -.085, hd.z + .1), (a, -.1, hd.z + .135), .007, "tooth", sides=3, faces=faces)

    def color(c, n, part):
        if part == "shirt":
            return (.36, .38, .4) if int(c.x * 16 + 100) % 2 == 0 else (.2, .24, .28)
        if part == "pants":
            return (.08, .09, .11)
        if part == "bone":
            return BONE
        if part == "claw":
            return (.16, .12, .10)
        if part == "tooth":
            return TOOTH
        if c.z > hd.z + .08 and c.z < hd.z + .17 and c.y < -.05 and abs(c.x) < .065:
            return MOUTH                                     # horizontal mouth above the eyes
        if c.z > hd.z + .01 and c.z < hd.z + .07 and c.y < -.06 and .025 < abs(c.x) < .065:
            return DARK                                      # eyes under the mouth
        return skin_tone(c, n, (.44, .38, .34), 3)

    return t, skin, extras, color, {}, ("bone", "tooth")


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
    t, skin, extras, color, extra_bones, over = fn(col)
    rig = hm.make_rig("Rig_" + name, t, col, extra_bones)
    ob, parts, tris = hm.assemble("CH_" + name, rig, skin, extras, col)
    overrides = {}
    if isinstance(over, dict):
        for bone, ps in over.items():
            overrides[bone] = hm.verts_of_parts(ob, parts, ps)
    hm.auto_weights(ob, rig, overrides)
    os.makedirs(ROOT + rf"\Monsters\{name}\Texture", exist_ok=True)
    tex = ROOT + rf"\Monsters\{name}\Texture\CH_{name}_Albedo.png"
    im = hm.paint(ob, parts, color, tex)
    hm.finish_material(ob, im)
    bpy.context.scene.render.fps = 30
    bpy.ops.wm.save_mainfile()
    return tris, sorted(set(parts))
