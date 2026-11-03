"""Clips of the 4 normal enemies (GDD Personajes y Animacion). Call make(name) with Monsters/<Name>/<Name>.blend open.

Enemies stay in an upright rest pose so the shared base cycles apply; every clip carries the character's posture.
The base cycles come from Lukas's src_* actions in Player.blend. Unity adds the broken-rhythm noise on top (EnemyAnimator).
"""
import importlib
import math
import sys

import bpy

ROOT = r"C:\Users\Chris\Heartbeat\Blender3DAssets"
sys.path.insert(0, ROOT + r"\Scripts")
import hb_anim

importlib.reload(hb_anim)
from hb_anim import Author, grip


def m(*ds):
    out = {}
    for d in ds:
        out.update(d)
    return out


def load_sources():
    with bpy.data.libraries.load(ROOT + r"\Player\Player.blend", link=False) as (src, dst):
        dst.actions = [n for n in src.actions if n.startswith("src_") and n not in bpy.data.actions]
        dst.armatures = ["Rig_Humanoid"]
    arm = dst.armatures[0]
    rest = {b.name: b.matrix_local.to_quaternion() for b in arm.bones}
    bpy.data.armatures.remove(arm)
    return rest


def alto(A):
    # Hunched, head pushed forward; the long arms hang almost to the floor.
    P = {"Spine": (24, 0, 0), "Chest": (22, 0, 0), "Neck": (-12, 0, 0), "Head": (-22, 0, 0), "LeftShoulder": (10, 0, 0), "RightShoulder": (10, 0, 0), "LeftUpperArm": (-24, 0, 0), "RightUpperArm": (-24, 0, 0), "LeftLowerArm": (-10, 0, 0), "RightLowerArm": (-10, 0, 0), "LeftUpperLeg": (-8, 0, 0), "RightUpperLeg": (-8, 0, 0), "LeftLowerLeg": (12, 0, 0), "RightLowerLeg": (12, 0, 0), "LeftFoot": (-4, 0, 0), "RightFoot": (-4, 0, 0), "loc": (0, 0, -.03)}
    arms = {"LeftUpperArm": .35, "RightUpperArm": .35, "LeftLowerArm": .3, "RightLowerArm": .3}
    A.clip("AN_Alto_Idle", 91, {1: P}, base="src_Idle", loop=True)
    A.clip("AN_Alto_Patrol", 46, {1: P}, base="src_Walk", swing=arms, loop=True)
    A.clip("AN_Alto_Hunt", 31, {1: m(P, {"Spine": (32, 0, 0), "LeftUpperArm": (-34, 4, 0), "RightUpperArm": (-34, -4, 0)})}, base="src_Run", still=("LeftUpperArm", "RightUpperArm", "LeftLowerArm", "RightLowerArm", "LeftHand", "RightHand", "LeftShoulder", "RightShoulder"), loop=True)
    # Long strike: the right arm swings from behind the shoulder down in front of Lukas.
    A.clip("AN_Alto_Attack", 34, {1: P, 12: m(P, {"Spine": (12, 0, 14), "Chest": (10, 0, 14), "RightUpperArm": (-150, 0, 0), "RightLowerArm": (-30, 0, 0)}), 18: m(P, {"Spine": (36, 0, -18), "Chest": (28, 0, -14), "RightUpperArm": (-58, 0, 0), "RightLowerArm": (-4, 0, 0)}), 34: P})
    REACH = m(P, grip("R", .6), grip("L", .6), {"Spine": (30, 0, 0), "RightUpperArm": (-75, -10, 0), "LeftUpperArm": (-75, 10, 0), "RightLowerArm": (-16, 0, 0), "LeftLowerArm": (-16, 0, 0)})
    A.clip("AN_Alto_Grab", 30, {1: P, 12: REACH, 22: m(REACH, {"Spine": (18, 0, 0), "RightUpperArm": (-88, -10, 0), "LeftUpperArm": (-88, 10, 0)}), 30: REACH})
    STUN = m(P, {"Spine": (8, 4, 0), "Chest": (6, 4, 0), "Head": (-34, 8, 0), "LeftUpperArm": (-8, -10, 0), "RightUpperArm": (-10, 10, 0)})
    A.clip("AN_Alto_Stunned", 31, {1: STUN, 15: m(STUN, {"Head": (-30, -6, 0)}), 31: STUN}, loop=True)
    return P


def fallen():
    return {"Hips": (-84, 0, 10), "Chest": (6, 0, 0), "Head": (4, 0, -24), "LeftUpperArm": (-10, -56, 0), "RightUpperArm": (-8, 34, 0), "LeftLowerArm": (-24, 0, 0), "RightLowerArm": (-10, 0, 0), "LeftUpperLeg": (-20, -8, 0), "RightUpperLeg": (-2, 8, 0), "LeftLowerLeg": (32, 0, 0), "RightLowerLeg": (4, 0, 0), "LeftFoot": (28, 0, 0), "RightFoot": (24, 0, 0)}


def death(A, n, start, hip_drop):
    F = m(fallen(), {"loc": (0, .3, -hip_drop)})
    A.clip(n, 45, {1: start, 8: m(start, {"LeftLowerLeg": (34, 0, 0), "RightLowerLeg": (34, 0, 0), "LeftUpperLeg": (-24, 0, 0), "RightUpperLeg": (-24, 0, 0), "loc": (0, .06, -hip_drop * .2)}), 26: F, 45: F})


def mudo(A):
    # Reads as a still person or a corpse: no motion, head dropped, arms dead. Only the burst breaks it.
    P = {"Neck": (10, 0, 2), "Head": (20, 6, 0), "LeftShoulder": (4, 0, 0), "RightShoulder": (6, 0, 0), "LeftUpperArm": (0, 2, 0), "RightUpperArm": (0, -2, 0)}
    A.clip("AN_Mudo_Still", 2, {1: P}, loop=True)
    RUN = {"Spine": (18, 0, 0), "Chest": (10, 0, 0), "Neck": (-10, 0, 0), "Head": (-6, 4, 0), "LeftUpperArm": (-30, 8, 0), "RightUpperArm": (-40, -8, 0)}
    A.clip("AN_Mudo_Burst", 21, {1: RUN}, base="src_Run", still=("LeftUpperArm", "RightUpperArm", "LeftLowerArm", "RightLowerArm", "LeftHand", "RightHand", "LeftShoulder", "RightShoulder"), loop=True)
    # A short activation: the head snaps up before the burst.
    A.clip("AN_Mudo_Activate", 12, {1: P, 4: m(P, {"Head": (-24, -8, 0), "Neck": (-8, 0, 0)}), 8: m(P, {"Head": (6, 10, 0), "Chest": (6, 4, 0)}), 12: RUN})
    REACH = m(RUN, grip("R", .6), grip("L", .6), {"RightUpperArm": (-84, -8, 0), "LeftUpperArm": (-84, 8, 0), "RightLowerArm": (-12, 0, 0), "LeftLowerArm": (-12, 0, 0)})
    A.clip("AN_Mudo_Grab", 24, {1: RUN, 8: REACH, 18: m(REACH, {"Spine": (8, 0, 0)}), 24: REACH})
    A.clip("AN_Mudo_Attack", 24, {1: RUN, 7: m(RUN, {"RightUpperArm": (-130, -6, 0), "RightLowerArm": (-30, 0, 0)}), 12: m(RUN, {"RightUpperArm": (-50, -6, 0), "Spine": (28, 0, -12)}), 24: RUN})
    STUN = m(P, {"Spine": (6, 3, 0), "Chest": (4, 3, 0), "Head": (26, -8, 0)})
    A.clip("AN_Mudo_Stunned", 31, {1: STUN, 15: m(STUN, {"Head": (22, 6, 0)}), 31: STUN}, loop=True)
    return P


def boca(A):
    # Blind: the head tilts listening. Stooped, slow, lurching.
    P = {"Spine": (14, 0, 0), "Chest": (12, 2, 0), "Neck": (-4, 0, 0), "Head": (-8, 0, 8), "LeftUpperArm": (-10, -4, 0), "RightUpperArm": (-10, 4, 0), "LeftLowerArm": (-14, 0, 0), "RightLowerArm": (-14, 0, 0), "LeftUpperLeg": (-34, 0, 0), "RightUpperLeg": (-34, 0, 0), "LeftLowerLeg": (58, 0, 0), "RightLowerLeg": (58, 0, 0), "LeftFoot": (-24, 0, 0), "RightFoot": (-24, 0, 0), "loc": (0, 0, -.11)}
    A.clip("AN_Boca_Idle", 91, {1: P, 45: m(P, {"Head": (-8, 4, -10)}), 91: P}, base="src_Idle", loop=True)

    def lurch(t):
        return t * .78 if t < .64 else .5 + (t - .64) * (.5 / .36)

    A.clip("AN_Boca_Wander", 46, {1: P}, base="src_Walk", warp=lurch, swing={"LeftUpperLeg": .8, "RightUpperLeg": .6, "LeftUpperArm": .3, "RightUpperArm": .3}, loop=True)
    A.clip("AN_Boca_Hunt", 31, {1: m(P, {"Spine": (22, 0, 0), "Head": (-14, 0, 0)})}, base="src_Walk", swing={"LeftUpperLeg": 1.1, "RightUpperLeg": 1.0, "LeftUpperArm": .4, "RightUpperArm": .4}, loop=True)
    OPEN = {"JawL": (0, 30, 0), "JawR": (0, -30, 0)}
    SCREAM = m(P, OPEN, {"Spine": (-4, 0, 0), "Chest": (-10, 0, 0), "Neck": (-12, 0, 0), "Head": (-20, 0, 0), "LeftUpperArm": (-6, -26, 0), "RightUpperArm": (-6, 26, 0), "LeftLowerArm": (-20, 0, 0), "RightLowerArm": (-20, 0, 0)})
    A.clip("AN_Boca_Scream", 60, {1: P, 12: SCREAM, 30: m(SCREAM, {"Head": (-16, 2, 0)}), 48: SCREAM, 60: P})
    BITE = m(P, {"Spine": (34, 0, 0), "Chest": (20, 0, 0), "Head": (-24, 0, 0)})
    A.clip("AN_Boca_Attack", 26, {1: P, 8: m(P, OPEN, {"Spine": (2, 0, 0), "Chest": (0, 0, 0), "Head": (-30, 0, 0)}), 13: m(BITE, {"JawL": (0, 0, 0), "JawR": (0, 0, 0)}), 26: P})
    STUN = m(P, {"Spine": (6, 4, 0), "Head": (10, 10, 0), "LeftUpperArm": (-4, -8, 0)})
    A.clip("AN_Boca_Stunned", 31, {1: STUN, 15: m(STUN, {"Head": (6, -6, 0)}), 31: STUN}, loop=True)
    return P


def rastrero(A):
    # Always on four legs (D-152): torso horizontal, the arms are the front legs, head turned up to look forward.
    B = {"Hips": (72, 0, 0), "Spine": (6, 0, 0), "Chest": (6, 0, 0), "Neck": (-30, 0, 0), "Head": (-34, 0, 0), "LeftUpperArm": (-41, -6, 0), "RightUpperArm": (-41, 6, 0), "LeftLowerArm": (-85, 0, 0), "RightLowerArm": (-85, 0, 0), "LeftHand": (-48, 0, 0), "RightHand": (-48, 0, 0), "LeftUpperLeg": (-112, 0, 0), "RightUpperLeg": (-112, 0, 0), "LeftLowerLeg": (118, 0, 0), "RightLowerLeg": (118, 0, 0), "LeftFoot": (-53, 0, 0), "RightFoot": (-53, 0, 0), "loc": (0, .3, -.4)}

    def add(d, **kw):
        o = dict(d)
        for bone, e in kw.items():
            a = o.get(bone, (0, 0, 0))
            o[bone] = (a[0] + e[0], a[1] + e[1], a[2] + e[2])
        return o

    def crawl(n, frames, amp, loop=True, extra=None):
        keys = {}
        steps = 8
        for i in range(steps + 1):
            ph = 2 * math.pi * i / steps
            s, c = math.sin(ph), math.cos(ph)
            k = add(B, LeftUpperArm=(amp * s, 0, 0), RightUpperArm=(-amp * s, 0, 0),
                    LeftLowerArm=(-max(0, c) * amp * .5, 0, 0), RightLowerArm=(-max(0, -c) * amp * .5, 0, 0),
                    LeftUpperLeg=(-amp * s, 0, 0), RightUpperLeg=(amp * s, 0, 0),
                    LeftLowerLeg=(max(0, c) * amp * .6, 0, 0), RightLowerLeg=(max(0, -c) * amp * .6, 0, 0),
                    Chest=(0, 0, amp * .18 * s), Spine=(0, amp * .1 * c, 0))
            if extra:
                k = add(k, **extra)
            keys[1 + round((frames - 1) * i / steps)] = k
        A.clip(n, frames, keys, loop=loop)

    A.clip("AN_Rastrero_Idle", 61, {1: B, 30: add(B, Chest=(3, 0, 0), Head=(0, 0, 8)), 61: B}, loop=True)
    crawl("AN_Rastrero_Crawl", 25, 26)
    crawl("AN_Rastrero_Hunt", 15, 38)
    A.clip("AN_Rastrero_Attack", 24, {1: B, 8: add(B, Hips=(-14, 0, 0), RightUpperArm=(-50, 0, 0), RightLowerArm=(-30, 0, 0)), 13: add(B, Hips=(6, 0, 0), RightUpperArm=(34, 0, 0), Head=(-14, 0, 0)), 24: B})
    A.clip("AN_Rastrero_Stunned", 31, {1: add(B, Head=(20, 12, 0)), 15: add(B, Head=(14, -10, 0)), 31: add(B, Head=(20, 12, 0))}, loop=True)
    return B


FN = {"Alto": alto, "Mudo": mudo, "Boca": boca, "Rastrero": rastrero}
HIP_DROP = {"Alto": 1.08, "Mudo": .84, "Boca": .74, "Rastrero": .76}


def make(name):
    rest = load_sources()
    rig = bpy.data.objects["Rig_" + name]
    A = Author(rig, rest)
    P = FN[name](A)
    death(A, f"AN_{name}_Death", P, HIP_DROP[name])
    rig.animation_data.action = None
    bpy.ops.wm.save_mainfile()
    return sorted(a.name for a in bpy.data.actions if a.name.startswith("AN_"))
