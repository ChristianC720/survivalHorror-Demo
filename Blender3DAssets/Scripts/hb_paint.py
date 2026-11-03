"""Paints low-poly models texel by texel from their 3D position (Signalis-style pass, BLENDER_PLAN.md section 7).

Each polygon gets its own island at one density. bake() returns every texel's position, normal, part and distance to the
polygon's edge, so paint rules work in model space: stencils, bands, seams and blood cross UV islands with no visible cut.
Colours are display values (PNG bytes / 255).
"""
import math

import bpy
import numpy as np

_FONT = {
    "A": "7C1211127C", "B": "7F49494936", "C": "3E41414122", "D": "7F4141221C", "E": "7F49494941", "F": "7F09090901",
    "G": "3E4149497A", "H": "7F0808087F", "I": "00417F4100", "J": "2040413F01", "K": "7F08142241", "L": "7F40404040",
    "M": "7F020C027F", "N": "7F0408107F", "O": "3E4141413E", "P": "7F09090906", "Q": "3E4151215E", "R": "7F09192946",
    "S": "4649494931", "T": "01017F0101", "U": "3F4040403F", "V": "1F2040201F", "W": "3F4038403F", "X": "6314081463",
    "Y": "0708700807", "Z": "6151494543", "0": "3E5149453E", "1": "00427F4000", "2": "4261514946", "3": "2141454B31",
    "4": "1814127F10", "5": "2745454539", "6": "3C4A494930", "7": "0171090503", "8": "3649494936", "9": "064949291E",
    "-": "0808080808", ".": "0060600000", "/": "2010080402", ":": "0036360000", " ": "0000000000",
}
_LUT = np.random.default_rng(1234).permutation(256)
_BAYER = np.array([[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]], dtype=np.float32) / 16 - .5


def hex_col(s):
    s = s.lstrip("#")
    return np.array([int(s[i:i + 2], 16) / 255 for i in (0, 2, 4)], dtype=np.float32)


def text_bitmap(text):
    """7 rows high, row 0 on top; 1 blank column between glyphs."""
    cols = []
    for ch in text.upper():
        g = _FONT.get(ch, _FONT[" "])
        cols += [int(g[i:i + 2], 16) for i in range(0, 10, 2)] + [0]
    return np.array([[(c >> r) & 1 for c in cols[:-1]] for r in range(7)], dtype=bool)


def _basis(n, align=None):
    n = np.asarray(n, dtype=np.float64)
    n = n / max(np.linalg.norm(n), 1e-9)
    if align is not None:
        u = align - n * (align @ n)
        u = u / max(np.linalg.norm(u), 1e-9)
    else:
        u = np.cross((0, 0, 1), n) if abs(n[2]) < .95 else np.array((1.0, 0, 0))
        u = u / np.linalg.norm(u)
    return u, np.cross(n, u)


class Bake:
    """Flat arrays, one row per texel: P, N, E (distance to the polygon's edge, metres), U, V (face-plane metres), part."""

    def __init__(self, tile, offset, atlas, density):
        self.tile, self.offset, self.atlas, self.density = tile, offset, atlas, density
        self.px, self.py, self.P, self.N, self.E, self.U, self.V, self.part, self.size = [], [], [], [], [], [], [], [], []
        self.names = []

    def freeze(self):
        for k in ("px", "py", "part"):
            setattr(self, k, np.concatenate(getattr(self, k)))
        for k in ("P", "N", "E", "U", "V", "size"):
            setattr(self, k, np.concatenate(getattr(self, k)).astype(np.float32))
        return self

    def is_(self, *names):
        idx = [self.names.index(n) for n in names if n in self.names]
        return np.isin(self.part, idx)

    @property
    def n(self):
        return len(self.px)

    def fill(self, col):
        return np.tile(np.asarray(col, dtype=np.float32), (self.n, 1))


def _pack(rects, tile, density, margin):
    """Shelf pack. rects: list of (w_m, h_m). Returns [(x, y, wpx, hpx)] by input index, or None when it does not fit."""
    size = [(max(2, math.ceil(w * density) + 1) + margin * 2, max(2, math.ceil(h * density) + 1) + margin * 2) for w, h in rects]
    order = sorted(range(len(rects)), key=lambda i: -size[i][1])
    x = y = row = 0
    out = [None] * len(rects)
    for i in order:
        w, h = size[i]
        if w > tile:
            return None
        if x + w > tile:
            y += row
            x = row = 0
        if y + h > tile:
            return None
        out[i] = (x, y, w, h)
        x += w
        row = max(row, h)
    return out


def bake(me, parts, tile, offset=(0, 0), atlas=None, margin=0, align_longest=False, edges=False, skip=()):
    """Writes the UVs of `me` and returns the Bake. parts[i] names polygon i; polygons in `skip` keep their UVs."""
    atlas = atlas or tile
    if not me.uv_layers:
        me.uv_layers.new(name="UVMap")
    uvl = me.uv_layers.active.data
    co = np.array([v.co[:] for v in me.vertices], dtype=np.float64)
    polys = [p for p in me.polygons if parts[p.index] not in skip]
    infos, rects = [], []
    for p in polys:
        vs = co[list(p.vertices)]
        align = None
        if align_longest:
            k = len(vs)
            e = [vs[(i + 1) % k] - vs[i] for i in range(k)]
            align = max(e, key=lambda a: a @ a)
        u, v = _basis(p.normal[:], align)
        x, y = vs @ u, vs @ v
        infos.append((p, u, v, np.array(p.normal[:]), x, y, vs))
        rects.append((x.max() - x.min(), y.max() - y.min()))
    lo, hi = 1.0, 4000.0
    best = None
    for _ in range(24):
        d = (lo * hi) ** .5
        pk = _pack(rects, tile, d, margin)
        if pk is None:
            hi = d
        else:
            lo, best = d, (d, pk)
    d, pk = best
    B = Bake(tile, offset, atlas, d)
    B.names = list(dict.fromkeys(parts))
    for (p, u, v, n, x, y, vs), (rx, ry, rw, rh) in zip(infos, pk):
        x0, y0 = x.min(), y.min()
        for k, li in enumerate(p.loop_indices):
            uvl[li].uv = ((offset[0] + rx + margin + (x[k] - x0) * d) / atlas, (offset[1] + ry + margin + (y[k] - y0) * d) / atlas)
        gy, gx = np.mgrid[0:rh, 0:rw]
        lx = (gx + .5 - margin) / d + x0
        ly = (gy + .5 - margin) / d + y0
        o = (vs[0] @ n) * n
        P = o + lx[..., None] * u + ly[..., None] * v
        if edges:
            q = np.stack((x, y), axis=1)
            e = np.full(lx.shape, 1e3)
            for i in range(len(q)):
                a, b = q[i], q[(i + 1) % len(q)]
                ab = b - a
                t = np.clip(((lx - a[0]) * ab[0] + (ly - a[1]) * ab[1]) / max(ab @ ab, 1e-12), 0, 1)
                e = np.minimum(e, np.hypot(lx - (a[0] + t * ab[0]), ly - (a[1] + t * ab[1])))
        else:
            e = np.zeros(lx.shape)
        count = rw * rh
        B.px.append((gx + rx + offset[0]).ravel())
        B.py.append((gy + ry + offset[1]).ravel())
        B.P.append(P.reshape(-1, 3))
        B.N.append(np.tile(n, (count, 1)))
        B.E.append(e.ravel())
        B.U.append(lx.ravel())
        B.V.append(ly.ravel())
        B.part.append(np.full(count, B.names.index(parts[p.index])))
        B.size.append(np.full(count, p.area))
    return B.freeze()


def noise(P, f=1.0, seed=0):
    """3D value noise in [0, 1]. f may be a scalar or per-axis, so f=(30, 30, 3) makes vertical streaks."""
    q = P * np.asarray(f, dtype=np.float32) + seed * 17.13
    i = np.floor(q).astype(np.int64)
    r = q - i
    r = r * r * (3 - 2 * r)

    def h(a, b, c):
        return _LUT[(_LUT[(_LUT[(i[:, 0] + a) & 255] + i[:, 1] + b) & 255] + i[:, 2] + c) & 255] / 255.0

    x00 = h(0, 0, 0) * (1 - r[:, 0]) + h(1, 0, 0) * r[:, 0]
    x10 = h(0, 1, 0) * (1 - r[:, 0]) + h(1, 1, 0) * r[:, 0]
    x01 = h(0, 0, 1) * (1 - r[:, 0]) + h(1, 0, 1) * r[:, 0]
    x11 = h(0, 1, 1) * (1 - r[:, 0]) + h(1, 1, 1) * r[:, 0]
    y0 = x00 * (1 - r[:, 1]) + x10 * r[:, 1]
    y1 = x01 * (1 - r[:, 1]) + x11 * r[:, 1]
    return (y0 * (1 - r[:, 2]) + y1 * r[:, 2]).astype(np.float32)


def fbm(P, f=1.0, octaves=3, seed=0):
    t, a, s = 0, .5, 0
    f = np.asarray(f, dtype=np.float32)
    for k in range(octaves):
        t = t + noise(P, f * 2 ** k, seed + k) * a
        s += a
        a *= .5
    return t / s


def ramp(t, cols):
    """Hard bands: t in [0, 1] picks one of `cols`, as a limited pixel-art ramp does."""
    cols = np.asarray(cols, dtype=np.float32)
    i = np.clip((np.asarray(t) * len(cols)).astype(int), 0, len(cols) - 1)
    return cols[i]


def mix(c, mask, col, a=1.0):
    c[mask] = c[mask] * (1 - a) + np.asarray(col, dtype=np.float32) * a


def blood(B, c, region, seed=0, coverage=.68, dark="#270a16", mid="#51111f"):
    """Dark dried blood: a few large blotches in `region` (a mask), with thin runs down from them."""
    P = B.P
    blot = fbm(P, 5, 2, seed)
    m = (blot > coverage) & region
    col = noise(P, (46, 46, 4), seed + 9)
    run = np.zeros(B.n, dtype=bool)
    for k in (.04, .09, .15, .22):
        up = P + np.array((0, 0, k), dtype=np.float32)
        run |= (fbm(up, 5, 2, seed) > coverage) & (col > .8 - k * .5)
    run &= region
    mix(c, run & ~m, hex_col(dark), .9)
    mix(c, m, hex_col(mid))
    mix(c, m & (fbm(P, 14, 2, seed + 3) > .56), hex_col(dark))
    return m | run


def grime(B, c, region, seed=0, amount=.3):
    """Low-frequency soil with downward streaks; only multiplies, so the palette hue survives."""
    n = fbm(B.P, (4, 4, 1.2), 2, seed)
    k = 1 - np.clip((n - .5) * 3, 0, 1) * amount
    c[region] *= k[region, None]


def folds(B, c, region, seed=0, f=(13, 13, 1.6), strength=.3):
    """Painted cloth folds: thin creases along gravity. Thresholding value noise makes blocks; an iso-line stays a line."""
    n = noise(B.P, f, seed)
    gate = noise(B.P, (4, 4, 4), seed + 4) > .46
    dark = (np.abs(n - .5) < .022) & gate & region
    c[dark] *= 1 - strength


def top_light(B, c, region=None, low=.8):
    """Baked soft top light, so volumes hold in the dark rooms."""
    k = low + (1 - low) * np.clip(B.N[:, 2] * .5 + .5, 0, 1)
    if region is None:
        c *= k[:, None]
    else:
        c[region] *= k[region, None]


def posterize(B, c, levels=7, dither=0.0):
    """Limited ramp per hue with ordered dither: the raw, hand-pixelled grain."""
    L = np.maximum(c.mean(axis=1), 1e-4)
    d = _BAYER[B.py % 4, B.px % 4] * dither
    q = np.floor(L * levels + .5 + d) / levels
    return np.clip(c * (q / L)[:, None], 0, 1)


def decal(B, center, normal, size, fn, up=None):
    """Plane projection. fn(s, t) -> mask, s and t in [0, 1], t = 1 on top. Returns (texel mask, s, t values on it)."""
    center = np.asarray(center, dtype=np.float32)
    n = np.asarray(normal, dtype=np.float32)
    n = n / np.linalg.norm(n)
    u, v = _basis(n)
    u, v = u.astype(np.float32), v.astype(np.float32)
    rel = B.P - center
    s = rel @ u / size[0] + .5
    t = rel @ v / size[1] + .5
    hit = (B.N @ n > .6) & (np.abs(rel @ n) < max(size) * .6) & (s >= 0) & (s < 1) & (t >= 0) & (t < 1)
    return hit, s, t, fn


def stamp(B, c, center, normal, size, col, bitmap, a=1.0, wear=0.0, seed=0):
    """Stencil a bool bitmap. Width follows from the aspect when size is a single height."""
    h, w = bitmap.shape
    if not isinstance(size, (tuple, list)):
        size = (size * w / h, size)
    # A bitmap under its own pixel size turns to noise: never go below 1 texel per bitmap pixel.
    k = max(1.0, h / max(size[1] * B.density, 1e-6))
    size = (size[0] * k, size[1] * k)
    hit, s, t, _ = decal(B, center, normal, size, None)
    if not hit.any():
        return size
    ix = np.clip((s[hit] * w).astype(int), 0, w - 1)
    iy = np.clip(((1 - t[hit]) * h).astype(int), 0, h - 1)
    on = bitmap[iy, ix]
    idx = np.nonzero(hit)[0][on]
    if wear:
        idx = idx[noise(B.P[idx], 60, seed) > wear]
    c[idx] = c[idx] * (1 - a) + np.asarray(col, dtype=np.float32) * a
    return size


def stripes(B, c, center, normal, size, col_a, col_b, pitch=.12, wear=.0, seed=0):
    """Hazard band, 45 degrees."""
    hit, s, t, _ = decal(B, center, normal, size, None)
    if not hit.any():
        return
    d = (s[hit] * size[0] + t[hit] * size[1]) / pitch
    on = (d % 1) < .5
    idx = np.nonzero(hit)[0]
    c[idx[on]] = col_a
    c[idx[~on]] = col_b
    if wear:
        w = idx[fbm(B.P[idx], 22, 2, seed) > wear]
        c[w] *= .55


def write(path, name, tile, layers, existing=None):
    """layers: list of (Bake, colours) onto one image of `tile` px. Returns the image."""
    img = existing if existing is not None else np.zeros((tile, tile, 4), dtype=np.float32)
    img[:, :, 3] = 1
    for B, c in layers:
        img[B.py, B.px, :3] = c
    save(path, name, img)
    return img


def save(path, name, img):
    im = bpy.data.images.get(name)
    h, w = img.shape[:2]
    if im is None or tuple(im.size) != (w, h):
        if im:
            bpy.data.images.remove(im)
        im = bpy.data.images.new(name, w, h)
    im.pixels.foreach_set(img.ravel())
    im.filepath_raw = path
    im.file_format = "PNG"
    im.save()
    return im
