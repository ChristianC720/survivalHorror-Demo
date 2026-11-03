"""All Lukas R-H-07 clips (GDD Personajes y Animacion). Run inside Blender with Player.blend open."""
import importlib
import sys

import bpy

sys.path.insert(0, r"C:\Users\Chris\Heartbeat\Blender3DAssets\Scripts")
import hb_anim

importlib.reload(hb_anim)
from hb_anim import Author, grip

rig = bpy.data.objects["Rig_Lukas"]
A = Author(rig)
for old, new in (("AN_Lukas_Walk_Fine", "src_Walk"), ("AN_Lukas_Run", "src_Run"), ("AN_Lukas_Idle_Fine", "src_Idle"), ("AN_Lukas_Stairs", "src_Stairs")):
    if old in bpy.data.actions and new not in bpy.data.actions:
        bpy.data.actions[old].name = new


def merged(*ds):
    out = {}
    for d in ds:
        out.update(d)
    return out


# Posture by health (M-05): upright, hunched and limping, dragging the step.
INJURED = {"Spine": (8, 0, 0), "Chest": (6, 2, 0), "Neck": (-4, 0, 0), "Head": (-3, 0, 0), "LeftUpperArm": (-8, 12, 0), "LeftLowerArm": (-40, 8, 0), "RightShoulder": (3, 0, 0)}
CRITICAL = {"Spine": (16, 3, 0), "Chest": (12, 4, 0), "Neck": (-4, 0, 0), "Head": (-1, 2, 0), "LeftUpperArm": (-18, 16, 0), "LeftLowerArm": (-65, 10, 0), "RightUpperArm": (6, 0, 0), "RightShoulder": (6, 0, 0), "LeftUpperLeg": (-10, 0, 0), "RightUpperLeg": (-10, 0, 0), "LeftLowerLeg": (18, 0, 0), "RightLowerLeg": (18, 0, 0), "LeftFoot": (-8, 0, 0), "RightFoot": (-8, 0, 0), "loc": (0, 0, -0.035)}
CROUCH = {"Spine": (18, 0, 0), "Chest": (14, 0, 0), "Neck": (-14, 0, 0), "Head": (-14, 0, 0), "LeftUpperLeg": (-62, -6, 0), "RightUpperLeg": (-62, 6, 0), "LeftLowerLeg": (92, 0, 0), "RightLowerLeg": (92, 0, 0), "LeftFoot": (-30, 0, 0), "RightFoot": (-30, 0, 0), "LeftUpperArm": (-18, 0, 0), "RightUpperArm": (-18, 0, 0), "LeftLowerArm": (-22, 0, 0), "RightLowerArm": (-22, 0, 0), "loc": (0, 0.02, -0.30)}

A.clip("AN_Lukas_Idle_Fine", 91, base="src_Idle", loop=True)
A.clip("AN_Lukas_Idle_Injured", 91, {1: INJURED}, base="src_Idle", loop=True)
A.clip("AN_Lukas_Idle_Critical", 91, {1: CRITICAL}, base="src_Idle", loop=True)
stride = {"LeftUpperLeg": 1.2, "RightUpperLeg": 1.2, "LeftLowerLeg": 1.15, "RightLowerLeg": 1.15}
A.clip("AN_Lukas_Walk_Fine", 31, base="src_Walk", swing=stride, loop=True)
A.clip("AN_Lukas_Run", 21, base="src_Run", swing=stride, loop=True)


# Limp: the good leg hurries so the hurt one bears weight for less time; the arm keeps pressing on the ribs.
def limp(t):
    return t * 0.8 if t < 0.625 else 0.5 + (t - 0.625) * (0.5 / 0.375)


def drag(t):
    return t * 0.7 if t < 0.72 else 0.5 + (t - 0.72) * (0.5 / 0.28)


A.clip("AN_Lukas_Walk_Injured", 37, {1: INJURED}, base="src_Walk", warp=limp, swing={"LeftUpperLeg": .95, "RightUpperLeg": .8, "RightLowerLeg": .7}, loop=True)
A.clip("AN_Lukas_Walk_Critical", 46, {1: CRITICAL}, base="src_Walk", warp=drag, swing={"LeftUpperLeg": .75, "RightUpperLeg": .55, "RightLowerLeg": .35, "RightFoot": .3, "LeftUpperArm": .3, "RightUpperArm": .5}, loop=True)
A.clip("AN_Lukas_Crouch_Idle", 61, {1: CROUCH, 31: merged(CROUCH, {"Chest": (15, 0, 0), "loc": (0, 0.02, -0.305)}), 61: CROUCH}, loop=True)
A.clip("AN_Lukas_Crouch_Walk", 37, {1: CROUCH}, base="src_Walk", swing={"LeftUpperLeg": .6, "RightUpperLeg": .6, "LeftLowerLeg": .6, "RightLowerLeg": .6, "LeftUpperArm": .3, "RightUpperArm": .3, "LeftLowerArm": .3, "RightLowerArm": .3}, loop=True)

AIM = merged(grip("R"), {"Spine": (4, 0, -6), "Chest": (2, 0, -8), "Neck": (0, 0, 8), "Head": (-2, 0, 6), "RightShoulder": (-4, 0, 0), "RightUpperArm": (-82, -12, 0), "RightLowerArm": (-6, 0, 0), "RightHand": (6, 0, 0), "LeftUpperArm": (-58, 16, 0), "LeftLowerArm": (-30, 0, -32)}, grip("L", .5))
A.clip("AN_Lukas_Aim", 31, {1: AIM, 16: merged(AIM, {"Chest": (3, 0, -8)}), 31: AIM}, loop=True)
RECOIL = merged(AIM, {"RightUpperArm": (-92, -12, 0), "RightLowerArm": (-16, 0, 0), "RightHand": (-10, 0, 0), "Chest": (-2, 0, -8)})
A.clip("AN_Lukas_Fire", 12, {1: AIM, 3: RECOIL, 12: AIM})
# One round per ~0.7 s (M-04). The left hand fetches a round from the belt pouch and pushes it into the drum.
RH_OPEN = merged(grip("R"), {"Spine": (8, 0, 0), "Head": (14, 0, 0), "RightUpperArm": (-35, -8, 0), "RightLowerArm": (-65, 0, 20), "RightHand": (10, 0, 0)})
RELOAD_A = merged(RH_OPEN, {"LeftUpperArm": (-40, 12, 0), "LeftLowerArm": (-70, 0, -25)})
A.clip("AN_Lukas_Reload_Round", 21, {1: RELOAD_A, 8: merged(RH_OPEN, {"LeftUpperArm": (10, -10, 0), "LeftLowerArm": (-20, 0, 0), "LeftHand": (20, 0, 0)}), 14: merged(RH_OPEN, {"LeftUpperArm": (-30, 12, 0), "LeftLowerArm": (-85, 0, -25)}), 21: RELOAD_A})
PUSH = {"Spine": (10, 0, 0), "Chest": (6, 0, 0), "RightUpperArm": (-70, -8, 0), "LeftUpperArm": (-70, 8, 0), "RightLowerArm": (-6, 0, 0), "LeftLowerArm": (-6, 0, 0)}
A.clip("AN_Lukas_Shove", 18, {1: {}, 5: {"Spine": (-6, 0, 0), "RightUpperArm": (-40, -6, 0), "LeftUpperArm": (-40, 6, 0), "RightLowerArm": (-80, 0, 0), "LeftLowerArm": (-80, 0, 0)}, 9: PUSH, 18: {}})
STRIKE = merged(CROUCH, grip("R"), {"Spine": (34, 0, 0), "RightUpperArm": (-70, -10, 0), "RightLowerArm": (-10, 0, 0), "loc": (0, -0.05, -0.22)})
A.clip("AN_Lukas_GroundStrike", 26, {1: {}, 8: merged(CROUCH, grip("R"), {"RightUpperArm": (-130, -10, 0), "RightLowerArm": (-60, 0, 0)}), 13: STRIKE, 26: {}})
FIN = merged(CROUCH, grip("R"), {"Spine": (36, 0, 0), "Chest": (18, 0, 0), "RightUpperArm": (-60, -4, 0), "RightLowerArm": (-30, 0, 0), "LeftUpperArm": (-60, 8, 0), "LeftLowerArm": (-10, 0, 0), "loc": (0, -0.04, -0.34)})
A.clip("AN_Lukas_Finisher", 45, {1: {}, 12: FIN, 22: merged(FIN, {"RightUpperArm": (-100, -4, 0), "RightLowerArm": (-50, 0, 0)}), 28: merged(FIN, {"RightUpperArm": (-48, -4, 0), "RightLowerArm": (-8, 0, 0), "Chest": (24, 0, 0)}), 36: FIN, 45: {}})

REACH = {"Spine": (6, 0, 0), "RightUpperArm": (-68, -6, 0), "RightLowerArm": (-14, 0, 0)}
A.clip("AN_Lukas_Use_Door", 20, {1: {}, 8: REACH, 12: merged(REACH, {"Spine": (9, 0, 0), "RightUpperArm": (-58, -6, 0)}), 20: {}})
A.clip("AN_Lukas_Use_Panel", 24, {1: {}, 8: merged(REACH, {"RightUpperArm": (-84, -4, 0)}), 16: merged(REACH, {"RightUpperArm": (-80, -4, 0), "RightLowerArm": (-22, 0, 0)}), 24: {}})
A.clip("AN_Lukas_PowerSwitch", 30, {1: {}, 9: merged(grip("R"), REACH, {"RightUpperArm": (-110, -6, 0)}), 18: merged(grip("R"), REACH, {"RightUpperArm": (-52, -6, 0), "Spine": (14, 0, 0)}), 30: {}})
LOW = merged(CROUCH, grip("R", .4), {"Spine": (34, 0, 0), "Chest": (18, 0, 0), "RightUpperArm": (-40, -6, 0), "RightLowerArm": (-14, 0, 0), "loc": (0, -0.02, -0.34)})
A.clip("AN_Lukas_Pick_Floor", 26, {1: {}, 11: LOW, 15: LOW, 26: {}})
A.clip("AN_Lukas_Pick_Shelf", 20, {1: {}, 9: merged(grip("R", .4), {"Spine": (4, 0, 0), "RightUpperArm": (-92, -6, 0), "RightLowerArm": (-10, 0, 0)}), 20: {}})
# Terminal (Tab, hacking): the left forearm comes up across the body with the outer face towards the eyes.
TERM = {"Spine": (4, 0, 0), "Neck": (6, 0, 0), "Head": (14, 0, -10), "LeftUpperArm": (-30, 10, 0), "LeftLowerArm": (-98, 0, -42)}
A.clip("AN_Lukas_Terminal_Open", 12, {1: {}, 12: TERM})
A.clip("AN_Lukas_Terminal_Hold", 61, {1: TERM, 31: merged(TERM, {"Head": (16, 0, -10)}), 61: TERM}, loop=True)
HACK = merged(TERM, {"RightUpperArm": (-28, -10, 0), "RightLowerArm": (-84, 0, 30), "RightIndexProximal": (0, -10, 0)})
A.clip("AN_Lukas_Hack_Loop", 31, {1: HACK, 8: merged(HACK, {"RightLowerArm": (-90, 0, 32)}), 16: HACK, 23: merged(HACK, {"RightLowerArm": (-78, 0, 28)}), 31: HACK}, loop=True)
# Heal ~1.5 s (M-07): autoinjector to the left forearm, cancelled by a hit via the Animator.
HEAL = merged(grip("R"), {"Spine": (8, 0, 0), "Head": (12, 0, 0), "LeftUpperArm": (-30, 10, 0), "LeftLowerArm": (-85, 0, -30), "RightUpperArm": (-32, -10, 0), "RightLowerArm": (-80, 0, 26)})
A.clip("AN_Lukas_Heal", 45, {1: {}, 14: HEAL, 24: merged(HEAL, {"RightLowerArm": (-74, 0, 26)}), 34: HEAL, 45: {}})
EXA = merged({"Spine": (2, 0, 0), "Head": (16, 0, 0), "RightUpperArm": (-32, -8, 0), "RightLowerArm": (-78, 0, 18), "LeftUpperArm": (-32, 8, 0), "LeftLowerArm": (-78, 0, -18)}, grip("R", .4), grip("L", .4))
A.clip("AN_Lukas_Examine", 40, {1: {}, 12: EXA, 28: merged(EXA, {"Head": (18, 0, 3)}), 40: {}})

A.clip("AN_Lukas_Hit", 14, {1: {}, 4: {"Spine": (-8, 3, 0), "Chest": (-8, 3, 0), "Head": (-12, 0, 0), "LeftUpperArm": (-16, -6, 0), "RightUpperArm": (-16, 6, 0)}, 14: {}})
GRAB = merged(grip("R", .7), grip("L", .7), {"Spine": (-10, 0, 0), "Chest": (-8, 0, 0), "Neck": (-6, 0, 0), "Head": (-6, 0, 0), "RightUpperArm": (-50, -12, 0), "RightLowerArm": (-72, 0, 20), "LeftUpperArm": (-50, 12, 0), "LeftLowerArm": (-72, 0, -20), "LeftUpperLeg": (-10, 0, 0), "RightUpperLeg": (-4, 0, 0), "LeftLowerLeg": (14, 0, 0), "loc": (0, 0.05, 0)})
A.clip("AN_Lukas_Grab_Struggle", 24, {1: GRAB, 7: merged(GRAB, {"Chest": (-14, 5, 0), "Head": (-14, 6, 0)}), 13: merged(GRAB, {"Chest": (-4, -5, 0), "Head": (-8, -6, 0)}), 19: merged(GRAB, {"Chest": (-12, 3, 0)}), 24: GRAB}, loop=True)
LIFTED = merged(GRAB, {"LeftUpperLeg": (-16, 0, 0), "RightUpperLeg": (6, 0, 0), "LeftLowerLeg": (30, 0, 0), "RightLowerLeg": (34, 0, 0), "LeftFoot": (28, 0, 0), "RightFoot": (28, 0, 0)})
A.clip("AN_Lukas_Grab_Lifted", 24, {1: LIFTED, 12: merged(LIFTED, {"LeftUpperLeg": (4, 0, 0), "RightUpperLeg": (-16, 0, 0)}), 24: LIFTED}, loop=True)
RELEASE = merged(grip("R"), {"Spine": (12, 0, 0), "Chest": (6, 0, -10), "RightUpperArm": (-96, -10, 0), "RightLowerArm": (-30, 0, 0), "LeftUpperArm": (-50, 10, 0), "LeftLowerArm": (-40, 0, 0)})
A.clip("AN_Lukas_Grab_Release", 24, {1: GRAB, 7: RELEASE, 12: merged(RELEASE, {"RightUpperArm": (-72, -10, 0)}), 24: {}})
FALLEN = {"Hips": (-82, 0, 8), "Spine": (6, 0, 0), "Chest": (8, 0, 0), "Neck": (4, 0, -12), "Head": (6, 0, -18), "LeftUpperArm": (-12, -48, 0), "RightUpperArm": (-8, 40, 0), "LeftLowerArm": (-30, 0, 0), "RightLowerArm": (-12, 0, 0), "LeftUpperLeg": (-22, -6, 0), "RightUpperLeg": (-4, 10, 0), "LeftLowerLeg": (34, 0, 0), "RightLowerLeg": (6, 0, 0), "LeftFoot": (28, 0, 0), "RightFoot": (24, 0, 0), "loc": (0, 0.30, -0.80)}
A.clip("AN_Lukas_Death_Hit", 45, {1: {}, 8: {"Spine": (-14, 0, 0), "Chest": (-10, 0, 0), "loc": (0, 0.05, -0.10), "LeftLowerLeg": (30, 0, 0), "RightLowerLeg": (30, 0, 0), "LeftUpperLeg": (-20, 0, 0), "RightUpperLeg": (-20, 0, 0)}, 24: FALLEN, 45: FALLEN})
A.clip("AN_Lukas_Death_Grab", 50, {1: LIFTED, 20: merged(LIFTED, {"Spine": (-16, 0, 0), "Head": (-22, 0, 0)}), 32: merged(LIFTED, {"Head": (24, 6, 0), "Chest": (8, 0, 0), "LeftUpperArm": (6, -14, 0), "RightUpperArm": (6, 14, 0), "LeftLowerArm": (-6, 0, 0), "RightLowerArm": (-6, 0, 0), "LeftFingers": (0, 0, 0), "RightFingers": (0, 0, 0)}), 50: FALLEN})

rig.animation_data.action = bpy.data.actions["AN_Lukas_Idle_Fine"]
result_clips = sorted(a.name for a in bpy.data.actions if a.name.startswith("AN_"))
