"""Paint rules for the assimilated (GDD Personajes, Direccion de Arte palette). Facing -Y, +X is the character's left.

Method taken from Signalis: strong value contrast, a dark body with one accent, graphic marks (stencils, bands, seams) as the
detail, dark dried blood in runs. The palette and every design here are this game's own.
"""
import numpy as np

import hb_paint as hp

X = hp.hex_col
PALE = X("#96928c")
FLESH = X("#746a68")
BRUISE = X("#452635")
INK = X("#0f0607")
CRIMSON = X("#922235")
BLOOD = X("#51111f")
OLD = X("#270a16")
TOOTH = X("#c9c2a8")
BONE = X("#a39d8a")
STAFF = X("#6d6866")
STAFF_DK = X("#332b2c")
GUARD = X("#20262a")
GUARD_DK = X("#121518")
GOWN = X("#8d8a82")
METAL = X("#5a5150")
RUST = X("#5c2f1c")
PURPLE = [X("#080409"), X("#241029"), X("#492153"), X("#6a4478"), X("#8f7f99")]


def _dead_extremities(B, c, mask, wrist_z):
    """Hands and feet go dark first: the eye reads the pale forearm, then the hand as a blot."""
    k = np.clip((wrist_z - B.P[:, 2]) / .14, 0, 1)
    m = mask & (np.abs(B.P[:, 0]) > .2) & (k > 0)
    c[m] = c[m] * (1 - k[m, None] * .7) + BRUISE * k[m, None] * .7


def _skin(B, c, mask, base, seed):
    n = hp.noise(B.P, 5, seed)
    c[mask] = base * (.86 + .28 * n[mask, None])
    veins = (hp.noise(B.P, (26, 26, 8), seed + 2) > .86) & (n < .42) & mask
    c[veins] = c[veins] * .6 + BRUISE * .4


def _ellipse(P, x, z, rx, rz):
    return ((np.abs(P[:, 0]) - x) / rx) ** 2 + ((P[:, 2] - z) / rz) ** 2 < 1


def _shoes(B, c, mask, z=.13):
    m = mask & (B.P[:, 2] < z)
    c[m] = INK * 1.6
    c[m & (B.P[:, 2] < .03)] = INK


def _finish(B, c, levels=8):
    hp.top_light(B, c)
    return hp.posterize(B, c, levels=levels)


def mudo(B, hz, shirt=STAFF, trousers=STAFF_DK, skin=None, label="VITALIS", tag="VK-0417", blood_cov=.69):
    P = B.P
    skin = PALE * .95 if skin is None else skin
    c = B.fill(skin)
    S, T, F = B.is_("shirt"), B.is_("trousers"), B.is_("skin", "finger")
    _skin(B, c, F, skin, 3)
    # Skin under the clothes never shows pale through a gap.
    c[F & (P[:, 2] > .72) & (P[:, 2] < hz - .06)] = INK
    c[F & (P[:, 2] < .72) & (np.abs(P[:, 0]) < .2)] = INK
    c[S] = shirt
    c[T] = trousers
    hp.folds(B, c, S, 4)
    hp.folds(B, c, T, 5)
    hp.grime(B, c, S | T, 6, .4)
    # Placket, buttons, collar, belt, trouser seam: the uniform reads from a few hard lines.
    front = P[:, 1] < -.02
    m = S & front & (np.abs(P[:, 0]) < .009)
    c[m] = c[m] * .45
    m = S & front & (np.abs(P[:, 0]) < .03) & (np.abs(((P[:, 2] + .02) % .085) - .04) < .007) & (np.abs(P[:, 0]) < .009)
    c[m] = PALE
    cz = hz - .03
    c[S & (P[:, 2] > cz)] = shirt * .55
    m = S & (P[:, 2] > cz - .03) & (P[:, 2] < cz) & (np.abs(P[:, 0]) < .1)
    c[m] = shirt * 1.25
    c[S & (np.abs(P[:, 2] - 1.0) < .018)] = INK * 1.6
    c[T & (np.abs(P[:, 2] - 1.0) < .018)] = INK * 1.6
    m = T & (np.abs(np.abs(P[:, 0]) - .165) < .006)
    c[m] = c[m] * .6
    # Sleeve band and chest tag: the only colour on the body.
    m = S & (np.abs(P[:, 0]) > .2) & (np.abs(P[:, 2] - (hz - .28)) < .03)
    c[m] = CRIMSON
    tagc = (.09, -.13, hz - .26)
    hp.stamp(B, c, tagc, (0, -1, .1), (.08, .05), PALE * 1.2, np.ones((5, 8), dtype=bool))
    hp.stamp(B, c, tagc, (0, -1, .1), (.07, .018), INK, hp.text_bitmap(tag))
    hp.stamp(B, c, (0, .13, hz - .22), (0, 1, 0), .055, shirt * .42, hp.text_bitmap(label), wear=.15, seed=2)
    hp.blood(B, c, S | T, 8, coverage=blood_cov)
    # Face: nothing. A shadow where the sockets should be, and a mouth that healed shut.
    H = F & (P[:, 2] > hz)
    m = H & (P[:, 1] < -.05) & _ellipse(P, .04, hz + .135, .03, .02)
    c[m] = c[m] * .74
    m = H & (P[:, 1] < -.05) & (np.abs(P[:, 2] - (hz + .06)) < .004) & (np.abs(P[:, 0]) < .03)
    c[m] = c[m] * .6
    _dead_extremities(B, c, F, hz - .56)
    _shoes(B, c, F | T, .12)
    return _finish(B, c)


def alto(B, hz):
    P = B.P
    c = B.fill(PURPLE[2])
    F = B.is_("skin")
    # Iridescent skin (D-152): hue bands that slide over the volume, never a flat purple.
    # Follows the facets, not the surface: a flat-shaded body turning under light shifts hue the way oil does.
    t = .36 + .3 * (B.N @ np.array((.55, -.45, .7), dtype=np.float32)) + (hp.noise(P, 3, 3) - .5) * .18
    c[F] = hp.ramp(np.clip(t, 0, .999), PURPLE)[F]
    c[F & (np.abs(P[:, 0]) < .3) & (P[:, 2] > .6) & (P[:, 2] < 1.28)] = INK   # never through a tear in the trousers
    V, Pn = B.is_("vest"), B.is_("pants")
    c[V] = GUARD
    c[Pn] = GUARD_DK
    hp.folds(B, c, V | Pn, 3)
    hp.grime(B, c, V | Pn, 7, .35)
    m = V & (((P[:, 2] * 10) % 1) < .07)                      # quilted vest rows
    c[m] = c[m] * .6
    m = Pn & (np.abs(np.abs(P[:, 0]) - .17) < .012)            # service stripe
    c[m] = CRIMSON * .8
    hp.stamp(B, c, (0, .16, 1.78), (0, 1, 0), .075, PALE, hp.text_bitmap("WERKSCHUTZ"), wear=.15, seed=4)
    hp.stamp(B, c, (.09, -.17, 1.86), (0, -1, 0), .05, PALE, hp.text_bitmap("KS"), wear=.25, seed=5)
    c[B.is_("strap", "pouch")] = INK * 1.8
    c[B.is_("pouch") & (((P[:, 2] * 40) % 1) < .15)] = INK
    c[B.is_("bone")] = BONE
    c[B.is_("claw")] = INK * 1.5
    c[B.is_("tooth")] = TOOTH
    hp.blood(B, c, V | Pn | F, 9, coverage=.69)
    H = F & (P[:, 2] > hz - .02)
    m = H & (P[:, 1] < -.085) & (P[:, 2] > hz + .01) & (P[:, 2] < hz + .1) & (np.abs(P[:, 0]) < .055)
    c[m] = OLD
    m = H & (P[:, 1] < -.04) & (np.abs(P[:, 0]) < .011) & (P[:, 2] > hz + .1)
    c[m] = INK                                                 # split forehead
    m = H & (P[:, 1] < -.05) & (np.abs(np.abs(P[:, 0]) - .05) < .022) & (np.abs(P[:, 2] - (hz + .16)) < .022)
    c[m] = INK
    _shoes(B, c, F, .12)
    return _finish(B, c, levels=9)


def boca(B, hz):
    P = B.P
    c = B.fill(PALE)
    F = B.is_("skin")
    _skin(B, c, F, PALE * .7, 5)
    Rg = B.is_("rags")
    c[F & (P[:, 2] > .6) & (P[:, 2] < 1.06) & (np.abs(P[:, 0]) < .2)] = INK
    c[Rg] = GOWN * .78
    hp.folds(B, c, Rg, 6)
    hp.grime(B, c, Rg, 8, .5)
    hp.stamp(B, c, (0, -.13, .95), (0, -1, 0), .05, INK * 2, hp.text_bitmap("SUBJ 31"), wear=.15, seed=6)
    # Inside of the head: wet crimson with hard ridges, darkest at the throat.
    H = F & (P[:, 2] > hz - .04)
    inner = H & (P[:, 1] < .03) & (B.N[:, 0] * P[:, 0] < 0)
    t = np.clip((P[:, 2] - hz) / .36 + (hp.noise(P, (6, 40, 40), 2) - .5) * .4, 0, .999)
    c[inner] = hp.ramp(t, [X("#090205"), OLD, BLOOD, CRIMSON])[inner]
    ridge = inner & (((P[:, 2] * 24) % 1) < .18)
    c[ridge] = c[ridge] * .5
    # Outside of the lobes: bruised skin, darkening to the rim of the cut.
    rim = H & ~inner & (np.abs(P[:, 0]) < .1) & (P[:, 1] < -.03)
    c[rim] = BRUISE
    c[B.is_("toothL", "toothR")] = TOOTH
    c[B.is_("claw")] = OLD
    m = F & (P[:, 2] > 1.02) & (P[:, 2] < hz - .1) & (np.abs(P[:, 0]) < .17) & (((P[:, 2] * 14) % 1) < .22)
    c[m] = c[m] * .66                                          # ribs
    hp.blood(B, c, F | Rg, 11, coverage=.69)
    m = F & (P[:, 2] > hz - .12) & (P[:, 2] < hz) & (np.abs(P[:, 0]) < .1)
    hp.blood(B, c, m, 13, coverage=.5)                        # the throat takes what the mouth drops
    _dead_extremities(B, c, F, hz - .6)
    c[F & (P[:, 2] < .1)] = INK * 2.2
    return _finish(B, c)


def rastrero(B, hz):
    P = B.P
    c = B.fill(FLESH)
    F = B.is_("skin")
    _skin(B, c, F, PALE * .68, 7)
    # Ribs and sternum painted onto the bare skin.
    m = F & (P[:, 2] > hz - .5) & (P[:, 2] < hz - .12) & (np.abs(P[:, 0]) < .17) & (((P[:, 2] * 15) % 1) < .28)
    c[m] = c[m] * .62
    Sh, Pn = B.is_("shirt"), B.is_("pants")
    c[F & (P[:, 2] > .35) & (P[:, 2] < 1.02) & (np.abs(P[:, 0]) < .24)] = INK
    c[F & (P[:, 2] > 1.0) & (P[:, 2] < 1.46) & (np.abs(P[:, 0]) < .34)] = INK
    # Test-subject gown: thin hard stripes.
    c[Sh] = GOWN * .8
    m = Sh & (((P[:, 0] * 22) % 1) < .22)
    c[m] = STAFF_DK
    c[Pn] = GUARD_DK
    hp.folds(B, c, Sh | Pn, 3)
    hp.grime(B, c, Sh | Pn, 2, .4)
    c[B.is_("bone")] = BONE
    c[B.is_("claw")] = OLD
    c[B.is_("tooth")] = TOOTH
    hp.blood(B, c, F | Sh | Pn, 12, coverage=.69)
    # Inverted face: chin on top, then the mouth, then the eyes under it.
    H = F & (P[:, 2] > hz)
    m = H & (P[:, 1] < -.05) & (P[:, 2] > hz + .1) & (P[:, 2] < hz + .165) & (np.abs(P[:, 0]) < .066)
    c[m] = OLD
    m = H & (P[:, 1] < -.05) & (np.abs(np.abs(P[:, 0]) - .045) < .017) & (np.abs(P[:, 2] - (hz + .045)) < .016)
    c[m] = INK
    _dead_extremities(B, c, F, hz - .75)
    c[F & (P[:, 2] < .1)] = INK * 2.2
    return _finish(B, c)


def _metal(B, c, mask, seed):
    P = B.P
    c[mask] = METAL * (.7 + .6 * hp.fbm(P, 16, 2, seed)[mask, None])
    m = mask & (hp.fbm(P, 11, 2, seed + 1) > .64)
    c[m] = RUST
    m = mask & (((P[:, 2] * 18) % 1) < .1)
    c[m] = c[m] * .45


def recycled(name, B, hz):
    """Same rules plus the laboratory's metal and the recycled clothing (D-130, D-152)."""
    P = B.P
    if name == "Mudo":
        c = mudo_nurse(B, hz)
    else:
        c = {"Alto": alto, "Boca": boca, "Rastrero": rastrero}[name](B, hz)
    M = B.is_("metal", "armmetal")
    _metal(B, c, M, 21)
    c[B.is_("rust")] = RUST
    return hp.posterize(B, c, levels=9)


def mudo_nurse(B, hz):
    P = B.P
    c = B.fill(PALE)
    F = B.is_("skin", "finger")
    _skin(B, c, F, PALE, 3)
    Sh, Sk, T = B.is_("shirt"), B.is_("skirt"), B.is_("trousers")
    c[Sh | Sk] = X("#8d8a82") * .82
    c[T] = X("#746a68") * .8
    hp.folds(B, c, Sh | Sk | T, 4)
    hp.grime(B, c, Sh | Sk | T, 6, .4)
    m = (Sh | Sk) & P[:, 1].__lt__(-.02) & (np.abs(P[:, 0]) < .012)
    c[m] = STAFF_DK
    m = Sh & (np.abs(P[:, 0]) > .2) & (np.abs(P[:, 2] - (hz - .28)) < .03)
    c[m] = CRIMSON
    hp.blood(B, c, Sh | Sk | T, 15, coverage=.71)
    # Bandaged head: overlapping diagonal wraps, dirty at the edges.
    H = F & (P[:, 2] > hz)
    band = (P[:, 2] * 34 + P[:, 0] * 11 + np.sin(np.arctan2(P[:, 1], P[:, 0]) * 3) * .6) % 1
    c[H] = np.where((band < .78)[:, None], PALE * 1.12, X("#4b3f3e"))[H]
    hp.blood(B, c, H, 16, coverage=.66)
    _dead_extremities(B, c, F, hz - .56)
    _shoes(B, c, F | T, .12)
    return _finish(B, c)
