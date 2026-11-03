"""Procedural sculpting: a signed distance volume built from anatomy primitives, meshed with surface nets.

The result is one closed surface, so no face can be culled from the wrong side and clothing cannot show through. The
full-resolution volume is kept after decimation: it gives every texel the sculpt's normal, occlusion and crease lines,
which is how a low-poly mesh keeps its muscles and face. Blender axes: facing -Y, +X is the character's left.
"""
import bmesh
import bpy
import numpy as np

SKIN, MOUTH, SHIRT, PANTS, VEST, BANDAGE = range(6)


class Volume:
    def __init__(self, lo, hi, h=.005):
        self.h = h
        self.lo = np.array(lo, dtype=np.float32)
        self.shape = tuple(int(np.ceil((b - a) / h)) + 1 for a, b in zip(lo, hi))
        self.d = np.full(self.shape, 1.0, dtype=np.float32)
        self.mat = np.zeros(self.shape, dtype=np.uint8)
        self.gx = None

    def _box(self, a, b, pad):
        """Index ranges and coordinates of the sub-grid around world box a..b."""
        i0 = np.maximum(np.floor((np.minimum(a, b) - pad - self.lo) / self.h).astype(int), 0)
        i1 = np.minimum(np.ceil((np.maximum(a, b) + pad - self.lo) / self.h).astype(int) + 1, self.shape)
        if (i1 <= i0).any():
            return None
        sl = tuple(slice(p, q) for p, q in zip(i0, i1))
        ax = [self.lo[k] + self.h * np.arange(i0[k], i1[k], dtype=np.float32) for k in range(3)]
        return sl, ax

    def _apply(self, sl, f, k, mat):
        cur = self.d[sl]
        if mat is not None:
            self.mat[sl][f < cur + k * .25] = mat
        if k <= 0:
            self.d[sl] = np.minimum(cur, f)
        else:
            q = np.clip(.5 + .5 * (cur - f) / k, 0, 1)
            self.d[sl] = cur * (1 - q) + f * q - k * q * (1 - q)

    def _carve(self, sl, f, k):
        cur = self.d[sl]
        if k <= 0:
            self.d[sl] = np.maximum(cur, -f)
        else:
            q = np.clip(.5 - .5 * (cur + f) / k, 0, 1)
            self.d[sl] = cur * (1 - q) - f * q + k * q * (1 - q)

    @staticmethod
    def _grid(ax):
        return np.meshgrid(*ax, indexing="ij", sparse=True)

    def _limb_field(self, a, b, ra, rb, sy, pad):
        a, b = np.array(a, dtype=np.float32), np.array(b, dtype=np.float32)
        r = max(ra, rb) * max(1, sy)
        box = self._box(a, b, r + pad)
        if box is None:
            return None, None
        sl, ax = box
        X, Y, Z = self._grid(ax)
        ab = b - a
        l2 = max(float(ab @ ab), 1e-9)
        t = np.clip(((X - a[0]) * ab[0] + (Y - a[1]) * ab[1] + (Z - a[2]) * ab[2]) / l2, 0, 1)
        dy = (Y - (a[1] + t * ab[1])) / sy
        f = np.sqrt((X - (a[0] + t * ab[0])) ** 2 + dy ** 2 + (Z - (a[2] + t * ab[2])) ** 2) - (ra + (rb - ra) * t)
        return sl, f * min(1, sy)

    def _ell_field(self, c, r, pad, tilt=0.0):
        c = np.array(c, dtype=np.float32)
        r = np.array(r, dtype=np.float32)
        ext = np.full(3, float(r.max()))
        box = self._box(c - ext, c + ext, pad)
        if box is None:
            return None, None
        sl, ax = box
        X, Y, Z = self._grid(ax)
        x, y, z = X - c[0], Y - c[1], Z - c[2]
        if tilt:  # around X: positive leans the top backward (+Y)
            ct, st = np.cos(tilt), np.sin(tilt)
            y, z = y * ct - z * st, y * st + z * ct
        k0 = np.sqrt((x / r[0]) ** 2 + (y / r[1]) ** 2 + (z / r[2]) ** 2)
        k1 = np.sqrt((x / r[0] ** 2) ** 2 + (y / r[1] ** 2) ** 2 + (z / r[2] ** 2) ** 2)
        return sl, k0 * (k0 - 1) / np.maximum(k1, 1e-6)

    def limb(self, a, b, ra, rb=None, k=.015, mat=None, sy=1.0):
        """Round cone a..b. sy flattens the section front to back."""
        rb = ra if rb is None else rb
        sl, f = self._limb_field(a, b, ra, rb, sy, k + 4 * self.h)
        if sl is not None:
            self._apply(sl, f, k, mat)

    def ball(self, c, r, k=.015, mat=None, tilt=0.0):
        sl, f = self._ell_field(c, r, k + 4 * self.h, tilt)
        if sl is not None:
            self._apply(sl, f, k, mat)

    def cut_limb(self, a, b, ra, rb=None, k=.006, sy=1.0, mat=None):
        rb = ra if rb is None else rb
        sl, f = self._limb_field(a, b, ra, rb, sy, k + 4 * self.h)
        if sl is not None:
            self._carve(sl, f, k)
            if mat is not None:
                self.mat[sl][f < 2 * self.h] = mat

    def cut_ball(self, c, r, k=.006, tilt=0.0, mat=None):
        sl, f = self._ell_field(c, r, k + 4 * self.h, tilt)
        if sl is not None:
            self._carve(sl, f, k)
            if mat is not None:
                self.mat[sl][f < 2 * self.h] = mat

    def axes(self):
        return [self.lo[k] + self.h * np.arange(self.shape[k], dtype=np.float32) for k in range(3)]

    def cloth(self, select, thick, mat, folds=None):
        """Shell over everything so far. select(P) gives 0..1 coverage (it tapers the hem), folds(P) adds thickness, so
        creases and torn edges are real geometry over the closed body."""
        idx = np.nonzero(np.abs(self.d) < thick + .03)
        P = np.stack(idx, axis=1).astype(np.float32) * self.h + self.lo
        t = select(P) * (thick if folds is None else thick + folds(P))
        hit = t > 0
        self.d[idx[0][hit], idx[1][hit], idx[2][hit]] -= t[hit]
        m = self.mat[idx]
        m[t > thick * .3] = mat
        self.mat[idx] = m

    def front_y(self, x, z, y0=-.45, y1=.12):
        """First surface met walking back from the front at (x, z): where to root a tooth so it never floats."""
        ys = np.arange(y0, y1, self.h * .5, dtype=np.float32)
        P = np.stack((np.full_like(ys, x), ys, np.full_like(ys, z)), axis=1)
        inside = np.nonzero(self.sample(self.d, P) < 0)[0]
        return float(ys[inside[0]]) if len(inside) else y1

    def freeze(self):
        """Gradients for the bake. Call after the last edit."""
        self.gx, self.gy, self.gz = np.gradient(self.d, self.h)

    def _idx(self, P):
        q = (P - self.lo) / self.h
        i = np.clip(np.floor(q).astype(np.int64), 0, np.array(self.shape) - 2)
        return i, np.clip(q - i, 0, 1)

    def sample(self, field, P):
        i, r = self._idx(P)
        out = 0
        for a in (0, 1):
            wx = r[:, 0] if a else 1 - r[:, 0]
            for b in (0, 1):
                wy = r[:, 1] if b else 1 - r[:, 1]
                for c in (0, 1):
                    wz = r[:, 2] if c else 1 - r[:, 2]
                    out = out + field[i[:, 0] + a, i[:, 1] + b, i[:, 2] + c] * wx * wy * wz
        return out

    def material(self, P):
        """Material at each point, taken from the nearest solid voxel around it."""
        i, r = self._idx(P)
        best = np.full(len(P), 1e9, dtype=np.float32)
        out = np.zeros(len(P), dtype=np.uint8)
        for a in (0, 1):
            for b in (0, 1):
                for c in (0, 1):
                    x, y, z = i[:, 0] + a, i[:, 1] + b, i[:, 2] + c
                    v = np.abs(self.d[x, y, z])
                    m = v < best
                    best[m] = v[m]
                    out[m] = self.mat[x, y, z][m]
        return out


def surface_nets(V):
    """Closed quad mesh of the zero level of V.d. Returns (verts, quads); quads wind outward."""
    d = V.d
    inside = d < 0
    nx, ny, nz = d.shape
    occ = np.zeros((nx - 1, ny - 1, nz - 1), dtype=np.uint8)
    for a in (0, 1):
        for b in (0, 1):
            for c in (0, 1):
                occ += inside[a:nx - 1 + a, b:ny - 1 + b, c:nz - 1 + c]
    cells = np.argwhere((occ > 0) & (occ < 8))
    ids = np.full(occ.shape, -1, dtype=np.int32)
    ids[tuple(cells.T)] = np.arange(len(cells))
    # A cell's vertex is the mean of the zero crossings on its 12 edges.
    corners = np.array([(a, b, c) for a in (0, 1) for b in (0, 1) for c in (0, 1)])
    pairs = [(i, j) for i in range(8) for j in range(i + 1, 8) if np.abs(corners[i] - corners[j]).sum() == 1]
    vals = np.stack([d[cells[:, 0] + c[0], cells[:, 1] + c[1], cells[:, 2] + c[2]] for c in corners], axis=1)
    total = np.zeros((len(cells), 3), dtype=np.float32)
    count = np.zeros(len(cells), dtype=np.float32)
    for i, j in pairs:
        a, b = vals[:, i], vals[:, j]
        hit = (a < 0) != (b < 0)
        t = np.where(hit, a / np.where(hit, a - b, 1), 0)
        total[hit] += (corners[i] + (corners[j] - corners[i]) * t[:, None])[hit]
        count[hit] += 1
    verts = (cells + total / count[:, None]) * V.h + V.lo
    quads = []
    # One quad for each grid edge crossing the surface, around the 4 cells sharing that edge.
    for axis in range(3):
        u, w = (axis + 1) % 3, (axis + 2) % 3
        lo = [slice(1, n - 1) for n in d.shape]
        hi = list(lo)
        lo[axis], hi[axis] = slice(1, d.shape[axis] - 2), slice(2, d.shape[axis] - 1)
        cross = inside[tuple(lo)] != inside[tuple(hi)]
        p = np.argwhere(cross) + 1
        if not len(p):
            continue
        flip = ~inside[p[:, 0], p[:, 1], p[:, 2]]
        def cell(du, dw):
            q = p.copy()
            q[:, u] -= du
            q[:, w] -= dw
            return ids[q[:, 0], q[:, 1], q[:, 2]]
        c00, c10, c11, c01 = cell(1, 1), cell(0, 1), cell(0, 0), cell(1, 0)
        q = np.stack((c00, c10, c11, c01), axis=1)
        q[flip] = q[flip][:, ::-1]
        quads.append(q)
    quads = np.concatenate(quads)
    return verts, quads[(quads >= 0).all(axis=1)]


def to_mesh(name, verts, quads, target, keep=None, collection=None):
    """Mesh decimated to `target` tris. keep(verts) gives each vertex a share class; each class gets its own budget, so
    the face receives triangles the limbs could not take from it. Weighted collapse wrecked thin limbs, hence the regions."""
    me = bpy.data.meshes.new(name)
    me.from_pydata(verts.tolist(), [], quads.tolist())
    me.update()
    ob = bpy.data.objects.new(name + "_hi", me)
    (collection or bpy.context.scene.collection).objects.link(ob)
    vl = bpy.context.view_layer
    vl.objects.active = ob
    ob.select_set(True)
    w = keep(verts) if keep else np.ones(len(verts))
    fw = w[quads].max(axis=1)
    shares = {1.0: .3, .5: .2, .25: .5} if keep else {1.0: 1.0}
    with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
        bpy.ops.object.mode_set(mode="EDIT")
        bpy.ops.mesh.select_mode(type="FACE")
        for cls, share in shares.items():
            bm = bmesh.from_edit_mesh(me)
            bm.faces.ensure_lookup_table()
            for f in bm.faces:
                f.select_set(False)
            sel = np.nonzero(fw == cls)[0]
            for i in sel:
                bm.faces[int(i)].select_set(True)
            bmesh.update_edit_mesh(me)
            count = len(sel) * 2
            if count:
                bpy.ops.mesh.decimate(ratio=min(1.0, target * share / count))
            # Indices changed: re-derive the classes from the survivors for the next region.
            bm = bmesh.from_edit_mesh(me)
            bm.faces.ensure_lookup_table()
            bm.verts.ensure_lookup_table()
            bm.verts.index_update()
            cur = np.array([v.co[:] for v in bm.verts], dtype=np.float32)
            vw = keep(cur) if keep else np.ones(len(cur))
            fw = np.array([max(vw[v.index] for v in f.verts) for f in bm.faces])
        bm = bmesh.from_edit_mesh(me)
        for f in bm.faces:
            f.select_set(True)
        bmesh.update_edit_mesh(me)
        n = sum(len(f.verts) - 2 for f in bm.faces)
        if n > target:
            bpy.ops.mesh.decimate(ratio=target / n)
        bpy.ops.object.mode_set(mode="OBJECT")
    bpy.data.objects.remove(ob)
    return me


def bake_fields(B, V):
    """Adds to the Bake what the sculpt knows at each texel: Ns (sculpt normal), ao, crease, mat."""
    P = B.P
    n = np.stack((V.sample(V.gx, P), V.sample(V.gy, P), V.sample(V.gz, P)), axis=1)
    n /= np.maximum(np.linalg.norm(n, axis=1, keepdims=True), 1e-6)
    B.Ns = n
    occ = np.zeros(len(P), dtype=np.float32)
    w = 0
    for dist, wt in ((.012, 1.0), (.03, .8), (.07, .55), (.14, .35)):
        occ += np.clip(1 - V.sample(V.d, P + n * dist) / dist, 0, 1) * wt
        w += wt
    B.ao = 1 - occ / w
    # Sharp creases: how fast the normal turns nearby. Negative is a valley, positive a ridge.
    s = V.h * 1.5
    B.crease = np.zeros(len(P), dtype=np.float32)
    for ax in np.eye(3, dtype=np.float32):
        B.crease += V.sample(V.d, P + ax * s) + V.sample(V.d, P - ax * s)
    B.crease = (B.crease - 6 * V.sample(V.d, P)) / (s * 6)
    B.mat = V.material(P)
    return B
