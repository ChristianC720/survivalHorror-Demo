"""Paint rules for the assimilated. The sheets in Image_References give the forms and values; the method is Signalis's: limited
ramps, hard dark lines, flat colour, graphic marks. Shading comes from the full-resolution sculpt (hb_sculpt.bake_fields), so
the low-poly mesh keeps its anatomy in any light. Facing -Y, +X is the character's left."""
import numpy as np

import hb_paint as hp
from hb_sculpt import BANDAGE, MOUTH, PANTS, SHIRT, SKIN, VEST

X = hp.hex_col
INK = X("#0f0607")
SEPIA = X("#1d0f10")
CRIMSON = X("#922235")
BLOOD = X("#51111f")
OLD = X("#270a16")
TOOTH = X("#cfc8ac")
BONE = X("#b0a68e")
METAL = X("#6d6a68")
RUST = X("#5c2f1c")
STAFF = X("#6d6866")
STAFF_DK = X("#332b2c")
GUARD = X("#2c343a")
GUARD_DK = X("#191d21")
GOWN = X("#8d8a82")
# Skin ramp from the sheets, a little dimmer for dark rooms: sepia shadow, flesh, bone highlight.
FLESH = [X("#2a1a1a"), X("#5a4038"), X("#8c7363"), X("#b39e88"), X("#d5c8b2")]
MUDO_SKIN = [X("#2b211f"), X("#5c4c44"), X("#8f7c6c"), X("#b5a48f"), X("#d3c7b1")]
ALTO_SKIN = [X("#180c20"), X("#452a55"), X("#8a7384"), X("#b9a896"), X("#ded3bd")]   # violet shadows keep D-152's iridescence
MOUTH_RAMP = [X("#080204"), OLD, BLOOD, X("#7a1a2a"), CRIMSON]
LIGHT = np.array((.35, -.55, .76), dtype=np.float32)
LIGHT /= np.linalg.norm(LIGHT)


def _ellipse(P, x, z, rx, rz):
    return ((np.abs(P[:, 0]) - x) / rx) ** 2 + ((P[:, 2] - z) / rz) ** 2 < 1


def _value(B):
    """0..1 painted value: soft key light plus the sculpt's occlusion."""
    return np.clip((B.Ns @ LIGHT * .5 + .5) * .72 + B.ao * .36 - .2, 0, .999)


def _ramp(t, cols):
    """Smooth through the ramp; posterize() later cuts it into hard bands."""
    cols = np.asarray(cols, dtype=np.float32)
    f = t * (len(cols) - 1)
    i = np.clip(f.astype(int), 0, len(cols) - 2)
    k = (f - i)[:, None]
    return cols[i] * (1 - k) + cols[i + 1] * k


def _lines(B, c, mask, valley=.03, strength=.72, col=SEPIA):
    """Ink where the sculpt creases, and a thin light edge on ridges: the drawn line of the sheets."""
    k = np.clip((-B.crease - valley) / .1, 0, 1)
    idx = mask & (k > 0)
    c[idx] = c[idx] * (1 - k[idx, None] * strength) + col * k[idx, None] * strength
    r = mask & (B.crease > .11)
    c[r] = c[r] * .86 + np.array((1, .96, .86), dtype=np.float32) * .14


def _skin(B, c, mask, ramp, seed):
    t = _value(B) + (hp.fbm(B.P, 9, 2, seed) - .5) * .08
    c[mask] = _ramp(np.clip(t, 0, .999), ramp)[mask]
    # Bruised blotches and veins, thin and few.
    v = (np.abs(hp.noise(B.P, (24, 24, 7), seed + 2) - .5) < .012) & (hp.noise(B.P, 5, seed + 3) > .55) & mask
    c[v] = c[v] * .68 + X("#4a2b3a") * .32
    _lines(B, c, mask)


def _cloth(B, c, mask, col, seed, grime=.3):
    k = (_value(B) * 1.15 + .12)[:, None]
    c[mask] = (np.asarray(col, dtype=np.float32) * k)[mask]
    hp.grime(B, c, mask, seed, grime)
    _lines(B, c, mask, valley=.05, strength=.6)


def _gashes(B, c, region, seed, count, size=(.018, .085)):
    """Open wounds: a dark slit, a raw rim, one run down from it. Each aligns near vertical, as in the sheets."""
    idx = np.nonzero(region)[0]
    if not len(idx):
        return
    rng = np.random.default_rng(seed)
    P = B.P
    for ci in rng.choice(idx, size=count):
        o = P[ci]
        a = rng.uniform(-.5, .5)
        rel = P - o
        along = rel[:, 2] * np.cos(a) + rel[:, 0] * np.sin(a)
        l = size[1] * rng.uniform(.6, 1.25)
        w = size[0] * rng.uniform(.6, 1.2)
        across = (rel[:, 0] * np.cos(a) - rel[:, 2] * np.sin(a)) * .8 + rel[:, 1] * .6
        lay = np.abs(rel @ B.Ns[ci]) < .03
        e = (along / l) ** 2 + (across / w) ** 2 + (hp.noise(P, 60, seed) - .5) * .6
        rim = (e < 1) & region & lay
        core = (e < .32) & region & lay
        c[rim] = c[rim] * .2 + CRIMSON * .35 + BLOOD * .45
        c[core] = OLD * .7
        # Runs: thin, broken, downward.
        run = (along < -l * .5) & (along > -l * rng.uniform(2.2, 4.2)) & (np.abs(across - np.sin(along * 30) * .004) < .0042) & region & lay & (hp.noise(P, (9, 9, 30), seed + 1) > .3)
        c[run] = c[run] * .3 + BLOOD * .7


def _mouth(B, c, mask, seed=2):
    t = np.clip(B.ao ** 2 * .72 + (B.Ns @ LIGHT) * .1 + (hp.noise(B.P, 22, seed) - .5) * .14, 0, .999)
    c[mask] = _ramp(t, MOUTH_RAMP)[mask]
    _lines(B, c, mask, valley=.04, strength=.8, col=INK)
    wet = mask & (B.Ns @ LIGHT > .72) & (hp.noise(B.P, 40, seed + 3) > .62) & (B.ao > .55)
    c[wet] = c[wet] * .55 + X("#d58a8a") * .45


def _hard(B, c):
    """Teeth, claws, spines, cables, metal: flat colours with the face normal for hard facets."""
    k = (np.clip(B.N @ LIGHT * .5 + .5, 0, 1) * .55 + .55)[:, None]
    tooth = B.is_("tooth", "toothL", "toothR")
    c[tooth] = (TOOTH * k)[tooth]
    root = tooth & (hp.noise(B.P, 60, 3) > .68)
    c[root] = BLOOD
    claw = B.is_("claw", "toeclaw")
    c[claw] = (X("#2a1816") * k)[claw]
    bone = B.is_("bone")
    c[bone] = (BONE * k)[bone]
    tip = bone & (hp.noise(B.P, 30, 8) > .6)
    c[tip] = BLOOD * 1.1
    cable = B.is_("cable")
    c[cable] = (X("#2a2c2e") * k)[cable]
    metal = B.is_("metal", "armmetal")
    c[metal] = (METAL * k)[metal]
    streak = metal & (hp.noise(B.P, (34, 34, 5), 6) > .62)
    c[streak] = c[streak] * .68 + RUST * .32
    c[B.is_("rivet")] = (X("#9a9894") * k)[B.is_("rivet")]
    c[B.is_("pouch")] = (X("#1d1c1b") * k)[B.is_("pouch")]


def _finish(B, c, levels=24):
    return hp.posterize(B, c, levels=levels)


def _masks(B):
    org = B.is_("skin")
    return org, org & (B.mat == SKIN), org & (B.mat == MOUTH), org & (B.mat == SHIRT), org & (B.mat == PANTS), org & (B.mat == VEST), org & (B.mat == BANDAGE)


def _dirty_hands(B, c, F, ramp_seed, wrist_z):
    """Hands and feet stained dark, from the sheets of the three worst-off."""
    n = hp.fbm(B.P, 7, 2, ramp_seed)
    # The edge wanders with noise, so no limb ends in a painted cuff.
    edge = (hp.noise(B.P, 11, ramp_seed + 1) - .5) * .22
    m = F & ((B.P[:, 2] < .1 + edge) | ((B.P[:, 2] < wrist_z + edge) & (np.abs(B.P[:, 0]) > .18))) & (n > .36)
    c[m] = c[m] * .5 + SEPIA * .34 + BLOOD * .16


def mudo(B, hz, shirt=STAFF, trousers=STAFF_DK, ramp=MUDO_SKIN, label="VITALIS", tag="VK-0417", gashes=3):
    P = B.P
    c = B.fill(ramp[2])
    org, F, M, S, T, _, _ = _masks(B)
    _skin(B, c, F | M, ramp, 3)
    _cloth(B, c, S, shirt, 6)
    _cloth(B, c, T, trousers, 7)
    # Placket, belt, chest tag, sleeve band: the uniform reads from a few hard marks.
    front = P[:, 1] < -.02
    m = S & front & (np.abs(P[:, 0]) < .006)
    c[m] = c[m] * .45
    m = S & front & (np.abs(P[:, 0]) < .006) & (((P[:, 2] + .02) % .085) < .012)
    c[m] = ramp[3]
    c[(S | T) & (np.abs(P[:, 2] - 1.0) < .016) & (np.abs(P[:, 0]) < .17)] = INK * 1.7
    m = S & (np.abs(P[:, 0]) > .2) & (np.abs(P[:, 2] - (hz - .28)) < .025)
    c[m] = CRIMSON
    tagc = (.085, -.11, hz - .26)
    hp.stamp(B, c, tagc, (0, -1, .1), (.075, .04), X("#c4bfae"), np.ones((5, 8), dtype=bool))
    hp.stamp(B, c, tagc, (0, -1, .1), (.066, .015), INK, hp.text_bitmap(tag))
    hp.stamp(B, c, (0, .115, hz - .23), (0, 1, 0), .04, shirt * .4, hp.text_bitmap(label), wear=.15, seed=2)
    hp.blood(B, c, S | T, 8, coverage=.74)
    _gashes(B, c, (F | S) & (P[:, 2] < hz - .02), 4, gashes)
    _hard(B, c)
    return _finish(B, c)


def alto(B, hz):
    P = B.P
    c = B.fill(ALTO_SKIN[2])
    org, F, M, S, T, V, Bd = _masks(B)
    _skin(B, c, F, ALTO_SKIN, 3)
    _mouth(B, c, M)
    _cloth(B, c, V, GUARD, 7)
    _cloth(B, c, T, GUARD_DK, 8)
    m = V & (((P[:, 2] * 12) % 1) < .06)                       # quilted vest rows
    c[m] = c[m] * .55
    m = T & (np.abs(np.abs(P[:, 0]) - .17) < .009)              # service stripe
    c[m] = CRIMSON * .8
    hp.stamp(B, c, (0, .13, 1.72), (0, 1, 0), .05, X("#9d9a90"), hp.text_bitmap("WERKSCHUTZ"), wear=.2, seed=4)
    hp.stamp(B, c, (.09, -.14, 1.85), (0, -1, 0), .03, X("#9d9a90"), hp.text_bitmap("KS"), wear=.25, seed=5)
    _gashes(B, c, F & (P[:, 2] < hz - .18), 9, 22)
    _gashes(B, c, V | T, 10, 5)
    hp.blood(B, c, V | T, 9, coverage=.72)
    # Blood from the mouth down the chin, from the split brow down the face.
    drip = F & (P[:, 2] > hz - .1) & (P[:, 2] < hz + .03) & (P[:, 1] < -.05) & (np.abs(P[:, 0]) < .045) & (hp.noise(P, (70, 70, 8), 3) > .45)
    c[drip] = BLOOD
    drip = F & (P[:, 2] > hz + .1) & (P[:, 1] < -.07) & (np.abs(P[:, 0] + .006 * np.sin(P[:, 2] * 40)) < .006)
    c[drip] = BLOOD
    # Empty sockets: black, the darkest thing on a pale head.
    m = F & (P[:, 1] < -.06) & (P[:, 2] > hz + .1) & _ellipse(P, .038, hz + .142, .027, .021)
    c[m] = INK
    _dirty_hands(B, c, F, 5, hz - 1.0)
    _hard(B, c)
    return _finish(B, c)


def boca(B, hz):
    P = B.P
    c = B.fill(FLESH[2])
    org, F, M, S, T, V, Bd = _masks(B)
    _skin(B, c, F, FLESH, 5)
    _mouth(B, c, M)
    _cloth(B, c, T, GOWN * .7, 8, .5)
    _cloth(B, c, Bd, X("#b6ae96"), 3, .4)
    hp.stamp(B, c, (0, -.11, .95), (0, -1, 0), .03, INK * 2, hp.text_bitmap("SUBJ 31"), wear=.15, seed=6)
    # Raw edge of the cut, and what the mouth drops on the chest.
    fan = np.abs(P[:, 0]) < .012 + np.clip(hz + .02 - P[:, 2], 0, 1) * .1 * (.2 + 1.3 * hp.noise(P, (30, 30, 7), 2))
    spill = F & (P[:, 1] < -.03) & (P[:, 2] < hz + .02) & (P[:, 2] > hz - .34 - hp.noise(P, (40, 40, 3), 5) * .14) & fan
    c[spill] = c[spill] * .3 + BLOOD * .7
    _gashes(B, c, F & (P[:, 2] < hz), 11, 30)
    _dirty_hands(B, c, F, 7, hz - .7)
    _hard(B, c)
    return _finish(B, c)


def rastrero(B, hz):
    P = B.P
    c = B.fill(FLESH[2])
    org, F, M, S, T, V, Bd = _masks(B)
    _skin(B, c, F, FLESH, 7)
    _mouth(B, c, M)
    # Striped subject shirt, pale blue on white as in the sheet, dulled: the one cold accent on the body.
    pale, blue = X("#b5b6ae"), X("#5d7382")
    t = (_value(B) * 1.2 + .1)[:, None]
    stripe = ((P[:, 0] * 30) % 1) < .42
    c[S] = (np.where(stripe[:, None], blue, pale) * t)[S]
    hp.grime(B, c, S, 2, .55)
    _lines(B, c, S, valley=.05, strength=.6)
    _cloth(B, c, T, X("#232c31"), 3, .4)
    # Lips raw only next to the maw; the outside of the jaw stays skin.
    lip = F & (P[:, 1] < -.22) & (B.ao < .7) & (hp.noise(P, 34, 4) > .35)
    c[lip] = c[lip] * .4 + BLOOD * .6
    _gashes(B, c, F & (P[:, 1] > -.13) & (P[:, 2] < hz), 12, 14)
    _gashes(B, c, F & (P[:, 1] <= -.13) & (P[:, 2] < hz - .1), 16, 4)
    hp.blood(B, c, S | T, 17, coverage=.8)
    _dirty_hands(B, c, F, 9, hz - .9)
    _hard(B, c)
    return _finish(B, c)


def mudo_nurse(B, hz):
    P = B.P
    c = B.fill(MUDO_SKIN[2])
    org, F, M, S, T, V, Bd = _masks(B)
    _skin(B, c, F | M, MUDO_SKIN, 3)
    _cloth(B, c, S, X("#8d8a82") * .9, 6, .45)
    m = S & (P[:, 1] < -.02) & (np.abs(P[:, 0]) < .006)
    c[m] = STAFF_DK
    m = S & (np.abs(P[:, 0]) > .2) & (np.abs(P[:, 2] - (hz - .28)) < .025)
    c[m] = CRIMSON
    hp.blood(B, c, S, 15, coverage=.76)
    _cloth(B, c, Bd, X("#b6ae96"), 3, .35)
    hp.blood(B, c, Bd, 16, coverage=.68)
    _gashes(B, c, F & (P[:, 2] < hz - .02), 4, 3)
    _hard(B, c)
    return _finish(B, c)


def recycled(name, B, hz):
    """Same rules plus the laboratory's metal and the recycled clothing (D-130, D-152)."""
    if name == "Mudo":
        return mudo_nurse(B, hz)
    return {"Alto": alto, "Boca": boca, "Rastrero": rastrero}[name](B, hz)
