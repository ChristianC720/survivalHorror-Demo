"""The four assimilated and their recycled forms as sculpts, following Blender3DAssets/Image_References (D-154).

Each function returns a Spec: the volume, the hand-built hard parts (teeth, claws, spines, cables, metal) and the weights rule.
Clothing follows the GDD where the sheets show bare bodies. Facing -Y, +X is the character's left.
"""
from types import SimpleNamespace

import numpy as np

import hb_anatomy as ha
import hb_monster as hm
import hb_paint as hp
from hb_sculpt import BANDAGE, MOUTH, PANTS, SHIRT, VEST


BONES = {
    "Alto": dict(hip=1.24, spine=0.78, neck=0.13, head=0.30, shoulder=0.25, upper_arm=0.66, lower_arm=0.66, hand=0.18, finger=0.22, hip_w=0.12, upper_leg=0.60, lower_leg=0.60, foot=0.26, arm_out=0.16),
    "Mudo": dict(hip=.96, spine=.5, neck=.11, head=.24, shoulder=.19, upper_arm=.3, lower_arm=.27, hand=.1, finger=.09, hip_w=.10, upper_leg=.46, lower_leg=.44, foot=.2),
    "Boca": dict(hip=.86, spine=.56, neck=.08, head=.3, shoulder=.2, upper_arm=.3, lower_arm=.32, hand=.12, finger=.13, hip_w=.11, upper_leg=.4, lower_leg=.42, foot=.22),
    "Rastrero": dict(hip=.88, spine=.58, neck=.1, head=.24, shoulder=.2, upper_arm=.36, lower_arm=.36, hand=.12, finger=.14, hip_w=.12, upper_leg=.4, lower_leg=.4, foot=.2, arm_out=.2),
}


def bones(name):
    return hm.humanoid_bones(**BONES[name])


def _keep(hz):
    """The decimator spends its triangles on the face, then on hands and feet."""
    def fn(v):
        w = np.full(len(v), .25, dtype=np.float32)
        w[(v[:, 2] < .14)] = .5
        w[np.abs(v[:, 0]) > .22] = np.maximum(w[np.abs(v[:, 0]) > .22], .5)
        w[v[:, 2] > hz - .05] = 1.0
        return w
    return fn


def _torn(P, seed, cover, f=7.0):
    """1 over the cloth, 0 in a hole; the edge is ragged at two scales."""
    n = hp.fbm(P, f, 3, seed)
    return np.clip((cover - n) * 40, 0, 1)


def _folds(seed, amp=.005, f=(11, 11, 2.2)):
    return lambda P: amp + amp * 1.6 * (hp.noise(P, f, seed) - .5)


def _hem(P, z, seed, rag=.03, f=26):
    """Coverage above height z, with a torn edge."""
    return np.clip((P[:, 2] - z - (hp.noise(P, (f, f, 3), seed) - .5) * rag) / .006, 0, 1)


def _claws(bm, tips, faces, length, r, part):
    for b in tips:
        hm.cone(bm, b, (b[0], b[1] - length * .28, b[2] - length), r, part, sides=5, faces=faces)


def _toe_claws(bm, tips, faces, length, r):
    for b in tips:
        hm.cone(bm, (b[0], b[1] + .006, b[2] + .002), (b[0], b[1] - length, .002), r, "toeclaw", sides=4, faces=faces)


def _spines(bm, faces, rng, zs, y, length, r, x=0.0, part="bone"):
    for z in zs:
        hm.cone(bm, (x, y - .012, z), (x, y + length * rng.uniform(.8, 1.15), z + length * .32), r, part, sides=5, faces=faces)


def _plate(bm, faces, c, w, h, part="metal"):
    """Riveted back plate. Its rivets are tiny boxes."""
    hm.bmesh_box(bm, c, (w, .02, h), part, faces)
    for sx in (-1, 1):
        for sz in (-1, 1):
            hm.bmesh_box(bm, (c[0] + sx * w * .41, c[1] + .013, c[2] + sz * h * .41), (.013, .009, .013), "rivet", faces)


def _spec(V, hard, hz, target=10500, over=None):
    return SimpleNamespace(V=V, hard=hard, keep=_keep(hz), target=target, over=over or (lambda co, parts: {}))


# ------------------------------------------------------------------ Mudo

def _mudo_head(V, hz):
    """Smooth, faceless, long. The face has planes and hollows, and nothing else."""
    V.ball((0, .012, hz + .15), (.072, .09, .09), k=.02)                                    # cranium
    V.limb((0, -.022, hz + .135), (0, -.04, hz + .012), .058, .021, k=.035)                # face narrows to the chin
    V.ball((0, -.02, hz + .05), (.04, .04, .036), k=.035)                                   # jaw
    V.ball((0, -.072, hz + .142), (.036, .016, .013), k=.03)                                # brow, inside the skull's outline
    V.ball((0, -.084, hz + .105), (.011, .012, .03), k=.03)                                 # where a nose would be
    for s in (-1, 1):
        V.cut_ball((s * .031, -.098, hz + .112), (.024, .022, .018), k=.02)                 # sockets, skin over them
        V.cut_ball((s * .06, -.06, hz + .06), (.015, .018, .03), k=.03)                     # hollow cheek
        V.ball((s * .07, .012, hz + .1), (.006, .014, .024), k=.01)                         # stub of an ear


def mudo(recycled=False, target=10500, claws=True):
    t = bones("Mudo")
    hz = t["Head"][0][2]
    V, sock = ha.body(t)
    _mudo_head(V, hz)
    if recycled:
        # Clinical uniform with a skirt, bandaged head (D-130).
        # The skirt thickens downward over the legs until they merge: a straight clinical hem, not a balloon.
        V.limb((0, 0, .9), (0, 0, .53), .055, .05, k=.03)
        V.cloth(lambda P: np.clip((P[:, 2] - .47) * 90, 0, 1) * np.clip((1.0 - P[:, 2]) / .5 + .12, 0, 1) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)), .075, SHIRT)
        V.cloth(lambda P: np.clip((P[:, 2] - .68) * 80, 0, 1) * np.clip((hz - .02 - P[:, 2]) * 80, 0, 1) * (np.abs(P[:, 0]) < .34) * _torn(P, 7, .74), .008, SHIRT, _folds(4))
        V.cloth(lambda P: (P[:, 2] > hz - .02) * (hp.noise(P, 26, 4) > .12), .009, BANDAGE, lambda P: .003 * np.sin(P[:, 2] * 150 + P[:, 0] * 50))
    else:
        V.cloth(lambda P: np.where(V.sample(V.armd, P) < V.sample(V.bodyd, P), 1.0, _hem(P, .97, 3, .01)) * np.clip((hz - .03 - P[:, 2]) * 80, 0, 1) * _sleeve(P, t, .72) * _torn(P, 7, .74), .009, SHIRT, _folds(4))
        V.cloth(lambda P: _hem(P, .095, 5, .05) * np.clip((1.0 - P[:, 2]) * 80, 0, 1) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)) * _torn(P, 8, .76), .008, PANTS, _folds(5))

    def hard(bm, faces, rng):
        if claws:
            _claws(bm, sock["hand"], faces, .03, .0055, "claw")
            _toe_claws(bm, sock["foot"], faces, .014, .0055)
        if recycled:
            hm.bmesh_box(bm, (0, -.09, hz + .1), (.13, .025, .17), "metal", faces)
            for x in (-.05, .05):
                for z in (.035, .165):
                    hm.bmesh_box(bm, (x, -.106, hz + z), (.013, .01, .013), "rivet", faces)

    return _spec(V, hard, hz, target)


def _sleeve(P, t, k):
    """Sleeves stop part way down the forearm: rolled up, the pale tendons show."""
    el = t["LeftUpperArm"][1][2]
    wr = t["LeftLowerArm"][1][2]
    limit = el + (wr - el) * k
    arm_side = np.abs(P[:, 0]) > t["LeftUpperArm"][0][0] - .04
    return np.where(arm_side, np.clip((P[:, 2] - limit) * 150, 0, 1), 1.0)


# ------------------------------------------------------------------ Alto

def alto(recycled=False):
    t = bones("Alto")
    hz = t["Head"][0][2]
    V, sock = ha.body(t, m=1.14, finger_r=.012, hollow=1.4, arm_m=1.22, leg_m=1.2)
    # Tall skull, split brow, gaping mouth.
    V.ball((0, .02, hz + .19), (.092, .112, .11), k=.02)
    V.limb((0, -.03, hz + .17), (0, -.05, hz + .03), .074, .036, k=.03)
    V.limb((-.056, -.096, hz + .17), (.056, -.096, hz + .17), .014, k=.02)
    V.limb((0, -.1, hz + .17), (0, -.116, hz + .115), .012, .011, k=.016)
    for s in (-1, 1):
        V.cut_ball((s * .038, -.112, hz + .142), (.022, .02, .018), k=.014)
        V.cut_ball((s * .078, -.055, hz + .07), (.018, .03, .042), k=.024)
        V.ball((s * .06, -.074, hz + .112), (.014, .008, .01), k=.04)                        # cheekbone, under the skin
        V.ball((s * .046, -.06, hz + .024), (.014, .01, .012), k=.04)                        # corner of the jaw
        V.ball((s * .09, .012, hz + .125), (.007, .017, .03), k=.012)                        # ear
    V.cut_ball((0, -.106, hz + .055), (.042, .046, .058), k=.008, mat=MOUTH)
    V.cut_limb((0, -.095, hz + .275), (0, -.106, hz + .15), .006, .0035, k=.006, mat=MOUTH)   # split forehead
    # Remains of the Werkschutz: ripped vest, torn trousers cut high on the shin (D-152).
    V.cloth(lambda P: _hem(P, 1.46, 3, .1, 18) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)) * np.clip((1.97 - np.abs(P[:, 0]) * .5 - P[:, 2]) * 80, 0, 1) * _torn(P, 12, .68, 9), .017, VEST, _folds(3, .006))
    V.cloth(lambda P: _hem(P, .46, 6, .12, 16) * np.clip((1.28 - P[:, 2]) * 80, 0, 1) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)) * _torn(P, 9, .63, 8), .01, PANTS, _folds(6, .006))

    def hard(bm, faces, rng):
        _claws(bm, sock["hand"], faces, .085, .0085, "claw")
        _toe_claws(bm, sock["foot"], faces, .02, .007)
        # Spines fan out from the upper back, the sheet's strongest profile mark.
        zs = np.linspace(1.5, 2.02, 8)
        for s in (-1, 0, 1):
            _spines(bm, faces, rng, zs[1:] if s == 0 else zs[3:], .115 - abs(s) * .03, .17 - abs(s) * .05, .021, x=s * .075)
        for s in (-1, 1):
            hm.cone(bm, (s * .165, .05, 1.97), (s * .25, .17, 2.1), .021, "bone", faces=faces, sides=5)
            hm.tube(bm, [(s * .03, .1, hz + .2), (s * .055, .135, hz + .12), (s * .06, .11, hz + .02), (s * .075, .09, hz - .1), (s * .13, .1, hz - .25)], .0065, "cable", faces)
            hm.tube(bm, [(s * .05, .1, hz - .22), (s * .17, .13, hz - .42), (s * .1, .1, hz - .6)], .0055, "cable", faces)
        for i in range(7):
            x = -.03 + i * .01
            y = V.front_y(x, hz + .108)
            hm.cone(bm, (x, y + .006, hz + .106), (x, y + .004, hz + .07 - .01 * (i % 2)), .0055, "tooth", sides=4, faces=faces)
            y = V.front_y(x, hz - .004)
            hm.cone(bm, (x, y + .006, hz - .002), (x, y + .004, hz + .034 + .01 * (i % 2)), .0055, "tooth", sides=4, faces=faces)
        for s in (-1, 1):                                                 # strap and pouches of the old gear
            hm.bmesh_box(bm, (s * .09, -.138, 1.58), (.075, .034, .1), "pouch", faces)
            hm.bmesh_box(bm, (s * .165, -.015, 1.1), (.05, .09, .12), "pouch", faces)
        if recycled:
            for z in np.linspace(1.3, 2.32, 11):                          # metal vertebra lengthens the spine (D-130)
                hm.bmesh_box(bm, (0, .13, z), (.085, .05, .075), "metal", faces)
            for side, s in (("Left", 1), ("Right", -1)):
                el = t[side + "UpperArm"][1]
                hm.cone(bm, el, (el[0] + s * .05, el[1] + .1, el[2] - .5), .032, "armmetal", sides=5, faces=faces)

    def over(co, parts):
        return {"Head": hm.verts_of_parts(*parts, ("tooth",))}

    return _spec(V, hard, hz, over=over)


# ------------------------------------------------------------------ Boca

def boca(recycled=False):
    t = bones("Boca")
    hz = t["Head"][0][2]
    V, sock = ha.body(t, m=.88 if recycled else 1.18, finger_r=.0105, hollow=1.8 if recycled else 1.15, arm_m=1.08, leg_m=1.1)
    # Two great lobes on a thick neck base, the head split to the sternum.
    V.limb((0, 0, hz - .1), (0, -.01, hz + .08), .075, .09, k=.03)
    for s in (-1, 1):
        V.limb((s * .03, -.01, hz + .05), (s * .1, 0, hz + .22), .075, .09, k=.03)
        V.ball((s * .112, .012, hz + .25), (.098, .098, .094), k=.03)
        V.limb((s * .07, .05, hz + .04), (s * .13, .065, hz + .2), .022, .03, k=.03)         # skull seam at the back
    # The cut: wide V up the front, narrow slit down the throat and chest. Depth comes from the carve itself.
    for s in (-1, 1):
        V.cut_limb((s * .006, -.09, hz + .02), (s * .118, -.085, hz + .3), .03, .078, k=.008, sy=.95, mat=MOUTH)
        V.cut_limb((s * .004, -.05, hz + .0), (s * .092, -.05, hz + .23), .02, .05, k=.008, mat=MOUTH)
        for i in range(8):
            k = i / 7
            V.cut_limb((s * (.03 + .098 * k), -.045, hz + .09 + .19 * k), (s * (.006 + .05 * k), -.065, hz + .05 + .1 * k), .0034, k=.006, mat=MOUTH)   # ribbed palate
    V.cut_limb((0, -.075, hz + .04), (0, -.135, hz - .34), .03, .005, k=.008, sy=1.1, mat=MOUTH)
    if recycled:
        V.cloth(lambda P: _hem(P, .68, 9, .09, 16) * np.clip((1.02 - P[:, 2]) * 80, 0, 1) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)) * _torn(P, 4, .6), .007, PANTS, _folds(6))
    else:
        V.cloth(lambda P: _hem(P, .7, 9, .08, 16) * np.clip((1.0 - P[:, 2]) * 80, 0, 1) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)) * _torn(P, 4, .58), .008, PANTS, _folds(6))
        # Bandage on the upper arm, from the sheet.
        el = t["RightUpperArm"]
        V.cloth(lambda P: (P[:, 0] < -.2) * (np.abs(P[:, 2] - (el[0][2] * .6 + el[1][2] * .4)) < .04), .006, BANDAGE, lambda P: .002 * np.sin(P[:, 2] * 220))

    def hard(bm, faces, rng):
        _claws(bm, sock["hand"], faces, .06, .008, "claw")
        _toe_claws(bm, sock["foot"], faces, .05, .0085)
        # Teeth rooted on both lips of the cut, pointing across it toward the other lip.
        for s, tag in ((1, "toothL"), (-1, "toothR")):
            for i in range(15):
                k = i / 14
                ax, az = s * (.006 + .112 * k), hz + .02 + .28 * k
                r = .03 + .048 * k
                for side, ln in ((1, .036 + .018 * np.sin(k * 3.1)), (-1, .024)):
                    x, z = ax + s * side * .93 * r, az - side * .37 * r
                    y = V.front_y(x, z)
                    if y > .05:
                        continue
                    hm.cone(bm, (x, y + .005, z), (x - s * side * .93 * ln, y + .012, z + side * .37 * ln), .0072 if side > 0 else .0056, tag, sides=4, faces=faces)
        if not recycled:
            _spines(bm, faces, rng, np.linspace(1.04, 1.34, 5), .095, .1, .017, x=.05)
            _spines(bm, faces, rng, np.linspace(1.04, 1.34, 5), .095, .1, .017, x=-.05)
            _plate(bm, faces, (0, .112, 1.2), .18, .16)
            for s in (-1, 1):
                hm.tube(bm, [(s * .04, .09, hz + .16), (s * .07, .14, hz + .02), (s * .04, .13, hz - .13), (s * .03, .11, 1.29)], .0062, "cable", faces)
            hm.tube(bm, [(.05, .11, 1.12), (.03, .13, 1.0), (.05, .1, .92)], .0055, "cable", faces)

    def over(co, parts):
        high = co[:, 2] > hz + .07
        return {"JawL": np.nonzero(high & (co[:, 0] > 0))[0].tolist() + hm.verts_of_parts(*parts, ("toothL",)),
                "JawR": np.nonzero(high & (co[:, 0] <= 0))[0].tolist() + hm.verts_of_parts(*parts, ("toothR",))}

    return _spec(V, hard, hz, over=over)


# ------------------------------------------------------------------ Rastrero

def rastrero(recycled=False):
    t = bones("Rastrero")
    hz = t["Head"][0][2]
    V, sock = ha.body(t, m=1.0, finger_r=.0105, hollow=1.3, arm_m=1.22, leg_m=1.18)
    # Bald dome, and the long sagging jaw of the sheet: it hangs from the head in front of the chest. In the crawling
    # clips the head rises and the jaw hangs free (Head bone only, see `over`).
    fy = -.205
    V.ball((0, -.03, hz + .135), (.09, .105, .095), k=.02)
    V.limb((0, -.05, hz + .12), (0, fy + .03, hz + .05), .078, .074, k=.05, sy=.85)
    V.limb((0, fy + .03, hz + .05), (0, fy, hz - .2), .08, .112, k=.05, sy=.62)            # widens toward the end of the jaw
    V.ball((0, fy, hz - .29), (.112, .072, .08), k=.03)
    for s in (-1, 1):
        V.cut_ball((s * .045, -.135, hz + .16), (.022, .012, .012), k=.03)                   # sockets long healed over
        V.limb((s * .092, -.01, hz + .13), (s * .092, -.006, hz + .09), .012, .007, k=.008)   # ear
    # The maw: real depth, ridged inside.
    V.cut_ball((0, fy - .082, hz - .12), (.082, .075, .205), k=.012, mat=MOUTH)
    V.cut_ball((0, fy - .01, hz - .16), (.03, .034, .05), k=.012, mat=MOUTH)
    for i in range(11):
        zz = hz + .07 - i * .037
        V.cut_limb((-.056, fy - .012, zz), (.056, fy - .012, zz), .0036, k=.008, mat=MOUTH)
    # Striped shirt in rags, holes on the back; dark overalls cut at the shin.
    V.cloth(lambda P: _hem(P, .9, 2, .1, 18) * np.clip((hz - .015 - P[:, 2]) * 80, 0, 1) * (P[:, 1] > -.135) * _sleeve(P, t, .0) * _torn(P, 14, .56, 8), .007, SHIRT, _folds(2))
    V.cloth(lambda P: _hem(P, .34, 3, .12, 16) * np.clip((1.02 - P[:, 2]) * 80, 0, 1) * (V.sample(V.armd, P) > V.sample(V.bodyd, P)) * _torn(P, 6, .68, 8), .011, PANTS, _folds(3, .006))

    def hard(bm, faces, rng):
        _claws(bm, sock["hand"], faces, .055, .0078, "claw")
        _toe_claws(bm, sock["foot"], faces, .04, .0075)
        _spines(bm, faces, rng, np.linspace(.98, 1.44, 10), .082, .075, .017)
        # Teeth rooted all around the lips of the maw, pointing in; a shorter row behind them.
        n = 26
        for i in range(n):
            a = 2 * np.pi * i / n
            ca, sa = np.cos(a), np.sin(a)
            x, z = .088 * ca, hz - .12 + .212 * sa
            y = V.front_y(x, z)
            ln = .045 + .025 * rng.random()
            hm.cone(bm, (x, y + .006, z), (x - ca * ln, y - .008, z - sa * ln), .0115, "tooth", sides=4, faces=faces)
        for i in range(n // 2):
            a = 2 * np.pi * (i + .5) / (n // 2)
            ca, sa = np.cos(a), np.sin(a)
            x, z = .068 * ca, hz - .12 + .17 * sa
            y = V.front_y(x, z)
            hm.cone(bm, (x, y + .004, z), (x - ca * .024, y - .006, z - sa * .024), .0058, "tooth", sides=4, faces=faces)
        _plate(bm, faces, (0, .112, 1.2), .17, .16)
        for s in (-1, 1):
            hm.tube(bm, [(s * .035, .05, hz + .12), (s * .07, .11, hz + .0), (s * .045, .11, hz - .14), (s * .03, .11, 1.3)], .0058, "cable", faces)
            hm.tube(bm, [(s * .08, .1, 1.17), (s * .1, .11, 1.06), (s * .05, .09, .99)], .005, "cable", faces)
        if recycled:
            for z in np.linspace(.98, 1.5, 6):                            # the metal vertebra that stops it standing (D-130)
                hm.bmesh_box(bm, (0, .12, z), (.08, .05, .07), "metal", faces)
            for s in (-1, 1):                                             # pincers fused to the face, like chelicerae
                hm.cone(bm, (s * .07, -.14, hz + .09), (s * .02, -.34, hz + .0), .024, "metal", sides=5, faces=faces)

    def over(co, parts):
        jaw = ((co[:, 1] < -.112) & (co[:, 2] > hz - .42)) | ((co[:, 1] < -.06) & (co[:, 2] > hz - .02))
        return {"Head": np.nonzero(jaw)[0].tolist() + hm.verts_of_parts(*parts, ("tooth",))}

    return _spec(V, hard, hz, over=over)


SCULPT = {"Mudo": mudo, "Alto": alto, "Boca": boca, "Rastrero": rastrero}
