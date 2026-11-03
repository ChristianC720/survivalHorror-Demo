"""Anatomy shared by the assimilated: lean, elongated bodies after Image_References. All positions come from the bone table,
so the sculpt always sits on the unchanged skeleton (clips and Unity avatars stay valid)."""
import numpy as np

from hb_sculpt import Volume


def lerp(a, b, k):
    return tuple(x + (y - x) * k for x, y in zip(a, b))


def off(p, y):
    return (p[0], p[1] + y, p[2])


def bounds(t, pad=.2):
    pts = np.array([p for b in t.values() for p in b[:2]])
    lo, hi = pts.min(axis=0) - pad, pts.max(axis=0) + pad
    lo[2] = -.02
    return tuple(lo.tolist()), tuple(hi.tolist())


def torso(V, t, m=1.0, hollow=1.0):
    """m scales muscle mass, hollow how far the ribs and belly sink."""
    hip, top, neck = t["Hips"][0], t["Chest"][1], t["Neck"][1]
    sh_x = t["LeftUpperArm"][0][0]
    H = top[2] - hip[2]
    w = (sh_x - .045) * m
    z = lambda k: hip[2] + H * k
    # Pelvis, waist, ribcage, shoulder girdle.
    V.ball((0, .006, hip[2] + .005), (.118 * m, .086 * m, .1), k=.03)
    V.limb((0, 0, z(.14)), (0, 0, z(.42)), .1 * m, .1 * m, k=.05, sy=.7)
    V.ball((0, 0, z(.68)), (w, .092 * m, H * .36), k=.05)
    V.limb((-sh_x + .03, .006, z(.9)), (sh_x - .03, .006, z(.9)), .048 * m, k=.04, sy=.9)
    # Neck with its cords.
    V.limb(top, neck, .048 * m, .036 * m, k=.025)
    V.limb((0, .012, z(.93)), (0, .006, neck[2] - (neck[2] - top[2]) * .45), .1 * m, .04 * m, k=.05, sy=.8)   # slope of the neck into the shoulders
    for s in (-1, 1):
        V.limb((s * .05, -.012, z(.95)), (s * .02, -.02, neck[2] - .004), .0115, .009, k=.012)   # sternocleidomastoid
        V.limb((s * (sh_x - .03), .028, z(.95)), (s * .03, .022, top[2] + (neck[2] - top[2]) * .5), .034 * m, .022, k=.05)   # trapezius, no gap under it
        V.limb((s * .018, -.056 * m, z(.93)), (s * (sh_x - .015), -.018 * m, z(.96)), .0085, k=.02)   # clavicle
        # Flat, wide pectoral with its lower edge carved: a muscle sheet, not a mound.
        V.ball((s * .068 * m, -.062 * m, z(.8)), (.08 * m, .024 * m, .056), k=.04, tilt=-.1)
        V.cut_limb((s * .012, -.104 * m, z(.68)), (s * .118 * m, -.075 * m, z(.72)), .0045, k=.018)
        # Shoulder blade and lats.
        V.ball((s * .078 * m, .07 * m, z(.77)), (.058 * m, .024 * m, .085), k=.025)
        V.limb((s * (w - .012), .03 * m, z(.7)), (s * .09 * m, .016, z(.26)), .028 * m, .012, k=.035)
        # Glute, hip crease.
        V.ball((s * .062 * m, .06, hip[2] - .022), (.072 * m, .056 * m, .08), k=.035)
        V.cut_limb((s * .105 * m, -.066, z(.1)), (s * .012, -.09, hip[2] - .1), .003, k=.016)
        # Ribs: soft grooves following the cage down the flank.
        for i in range(6):
            zz = z(.62 - i * .06)
            V.cut_limb((s * .05, -.092 * m, zz + .012), (s * w * .92, -.03, zz - .03), .0024 * hollow, k=.012)
            V.cut_limb((s * w * .92, .03, zz - .03), (s * .07, .088 * m, zz + .006), .002 * hollow, k=.012)
    # Sunken belly, sternum line, rectus, navel.
    V.cut_ball((0, -.1 * m - .016, z(.42)), (.085 * m, .024 * hollow, .05), k=.035)
    V.cut_limb((0, -.1 * m, z(.86)), (0, -.08, z(.14)), .0035, k=.012)
    for i in range(3):
        V.cut_limb((-.036, -.082, z(.34 - i * .082)), (.036, -.082, z(.34 - i * .082)), .0022, k=.01)
    V.cut_ball((0, -.082, z(.075)), (.007, .008, .009), k=.005)
    # Spine: a groove with knobs in it.
    V.cut_limb((0, .094 * m, z(.9)), (0, .07, z(.04)), .005, k=.014)
    for i in range(12):
        V.ball((0, .068 + .018 * np.sin(i / 11 * 3.1) * m, z(.94 - i * .08)), (.0115, .012, .0125), k=.008)


def arm(V, t, side, s, m=1.0):
    sh, el, wr = t[side + "UpperArm"][0], t[side + "UpperArm"][1], t[side + "LowerArm"][1]
    ua = float(np.linalg.norm(np.subtract(el, sh)))
    la = float(np.linalg.norm(np.subtract(wr, el)))
    # Deltoid, biceps, triceps, elbow knob, forearm belly, tendons.
    V.ball(lerp(sh, el, .16), (.05 * m, .054 * m, min(.085, ua * .3)), k=.035)
    V.limb(sh, el, .042 * m, .03, k=.02)
    V.ball(off(lerp(sh, el, .46), -.018), (.03 * m, .028 * m, ua * .3), k=.018)
    V.ball(off(lerp(sh, el, .52), .02), (.03 * m, .028 * m, ua * .32), k=.018)
    V.ball(off(el, .014), (.02, .02, .023), k=.012)
    V.limb(el, wr, .03 * m, .023 * m, k=.015)
    V.ball(lerp(el, wr, .25), (.036 * m, .035 * m, la * .26), k=.018)
    for dy in (-.012, .012):
        V.limb(off(lerp(el, wr, .55), dy), off(wr, dy), .0075, .0065, k=.007)
    V.ball(wr, (.024 * m, .023 * m, .016), k=.008)


def hand(V, t, side, s, finger_r=.0095):
    """Palm and 5 real fingers, spread as in the sheets. The skeleton's single Fingers bone drives the 3 outer ones."""
    wr, tip = t[side + "Hand"][0], t[side + "Hand"][1]
    hl = float(np.linalg.norm(np.subtract(tip, wr)))
    V.ball(lerp(wr, tip, .55), (.016, .046, max(hl * .62, .05)), k=.01)
    length = abs(t[side + "Fingers"][0][2] - t[side + "Fingers"][1][2])
    knuck = lerp(wr, tip, .98)
    tips = []
    for y, kl in ((-.034, .92), (-.0115, 1.0), (.0115, .95), (.034, .78)):
        a = off(knuck, y)
        mid = (a[0] + s * .012, a[1] + y * .16, a[2] - length * kl * .52)
        b = (a[0] + s * .012, a[1] + y * .44, a[2] - length * kl)
        V.limb(a, mid, finger_r, finger_r * .86, k=.003)
        V.limb(mid, b, finger_r * .86, finger_r * .6, k=.002)
        tips.append(b)
    th0, th1 = t[side + "ThumbProximal"][0], t[side + "ThumbDistal"][1]
    th1 = (th1[0] - s * .006, th1[1] - .012, th1[2] - length * .08)
    V.limb(th0, lerp(th0, th1, .55), finger_r * 1.2, finger_r, k=.006)
    V.limb(lerp(th0, th1, .55), th1, finger_r, finger_r * .66, k=.002)
    tips.append(th1)
    return tips


def leg(V, t, side, s, m=1.0):
    hp, kn, an = t[side + "UpperLeg"][0], t[side + "UpperLeg"][1], t[side + "LowerLeg"][1]
    ul = float(np.linalg.norm(np.subtract(kn, hp)))
    ll = float(np.linalg.norm(np.subtract(an, kn)))
    V.limb(hp, kn, .066 * m, .042 * m, k=.03)
    # Quadriceps, hamstring, inner thigh; kneecap; calf, shin, Achilles; ankle bones.
    V.ball(off(lerp(hp, kn, .42), -.03), (.05 * m, .036 * m, ul * .34), k=.022)
    V.ball((hp[0] + s * .012, .03, lerp(hp, kn, .55)[2]), (.04 * m, .034 * m, ul * .34), k=.022)
    V.ball((hp[0] - s * .026, 0, lerp(hp, kn, .3)[2]), (.032 * m, .04 * m, ul * .26), k=.022)
    V.ball(off(kn, -.036 * m), (.03 * m, .02, .032), k=.012)
    V.limb(kn, an, .04 * m, .026 * m, k=.018)
    V.ball(off(lerp(kn, an, .28), .026), (.05 * m, .042 * m, ll * .26), k=.022)
    V.limb((kn[0], kn[1] - .03, kn[2] - .05), (an[0], an[1] - .017, an[2] + .03), .009, k=.012)
    V.limb(lerp(kn, an, .62), off(an, .034), .013, .01, k=.01)
    for dx in (-1, 1):
        V.ball((an[0] + dx * .022, an[1], an[2]), (.012, .015, .013), k=.006)
    # Foot: heel, arch, ball, 5 toes.
    ft = t[side + "Foot"][1]
    V.ball((an[0], an[1] + .036, .03), (.032, .042, .03), k=.012)
    V.limb((an[0], an[1], an[2] - .012), (ft[0], ft[1] + .012, .03), .032, .026, k=.014)
    V.ball((ft[0], ft[1], .024), (.052, .03, .024), k=.01)
    tips = []
    for i, dx in enumerate((-.036, -.018, 0, .017, .034)):
        x = ft[0] + s * dx
        L = (.056, .052, .046, .04, .034)[i]
        r = (.013, .0095, .009, .0085, .0078)[i]
        a = (x, ft[1] - .016, .019)
        b = (x + s * dx * .25, ft[1] - .016 - L, .013)
        V.limb(a, b, r, r * .84, k=.003)
        tips.append(b)
    return tips


def body(t, m=1.0, h=.005, finger_r=.0095, hollow=1.0, arm_m=1.0, leg_m=1.0):
    """The whole bare body except the head, plus claw sockets. Arms are sculpted apart: V.armd tells them from the torso
    after the merge, so no sleeve or arm vertex can be bound to the ribs beside it."""
    lo, hi = bounds(t)
    V = Volume(lo, hi, h)
    A = Volume(lo, hi, h)
    torso(V, t, m, hollow)
    sockets = {"hand": [], "foot": []}
    for side, s in (("Left", 1), ("Right", -1)):
        arm(A, t, side, s, m * arm_m)
        sockets["hand"] += hand(A, t, side, s, finger_r)
        sockets["foot"] += leg(V, t, side, s, m * leg_m)
    V.bodyd = V.d.copy()
    V.armd = A.d
    q = np.clip(.5 + .5 * (V.d - A.d) / .018, 0, 1)
    V.d = V.d * (1 - q) + A.d * q - .018 * q * (1 - q)
    return V, sockets
