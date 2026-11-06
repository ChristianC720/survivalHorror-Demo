"""Lukas's hair, modelled on the cover reference and Lukas.jpeg: a warm brown mass, parted at the front, with angular locks
sweeping back and down, flicked tips at ear level, and a thin crease at the back of the crown.
Rebuilds the object CH_Lukas_Hair in CH_Lukas_RH07.blend. Run inside Blender. Rigid on the Head bone so it never stretches."""
import bisect
import math
import bmesh
import bpy
from mathutils import Vector
from mathutils.bvhtree import BVHTree

# Flat browns in the atlas, identical in the RH07 albedo, its 4 variants and the civil albedo.
LIGHT, MAIN, SHADE, DARK = (0.8892, 0.0459), (0.6675, 0.772), (0.4199, 0.6152), (0.8662, 0.042)

# Silhouette rings of the mass: z, half width, back y, front y (from the sheet's back and side views, fitted to the head).
RINGS = [(1.808, .042, .015, -.065), (1.792, .068, .050, -.090), (1.762, .088, .080, -.112), (1.722, .102, .095, -.124),
         (1.682, .108, .100, -.125), (1.642, .110, .098, -.110), (1.602, .108, .090, -.090), (1.552, .094, .072, -.066)]
# Lower edge of the mass against the angle from the face (degrees): parting, temple, cheek, nape.
EDGE = [(0, 1.745), (10, 1.735), (25, 1.700), (35, 1.668), (42, 1.640), (50, 1.600), (60, 1.566), (120, 1.558), (180, 1.552)]
COLS, ROWS = 28, 7   # rows bunch up at the crown, where the dome curves
SKULL_GAP = .006
SQUARE = .67
CENTER = Vector((0, -.02, 1.665))

# Locks. Each: colour, then stations (angle from the back in degrees, z, width, height over the mass, push outwards).
# Width 0 at the last station makes the tip. Mirrored on both sides.
LOCKS = [
    (MAIN, [(176, 1.786, .040, 0, 0), (150, 1.752, .056, .008, 0), (122, 1.697, .060, .009, 0), (106, 1.632, .050, .008, .006), (101, 1.600, 0, 0, .024)]),   # parting to the ear flick
    (LIGHT, [(177, 1.778, .018, 0, 0), (163, 1.738, .026, .008, .001), (150, 1.694, .022, .007, .002), (146, 1.662, 0, 0, .003)]),                             # strand along the forehead
    (LIGHT, [(116, 1.776, .030, 0, 0), (96, 1.748, .062, .009, 0), (74, 1.676, .060, .009, 0), (60, 1.598, .052, .007, .004), (55, 1.566, 0, 0, .018)]),       # top of the head to the nape, the side sweep
    (SHADE, [(114, 1.668, .036, 0, 0), (106, 1.622, .040, .006, 0), (101, 1.572, .036, .005, .002), (99, 1.546, 0, 0, .006)]),                                 # under-layer down the neck
    (MAIN, [(78, 1.772, .030, 0, 0), (58, 1.726, .060, .008, 0), (42, 1.644, .060, .008, 0), (34, 1.578, .050, .006, .003), (31, 1.552, 0, 0, .010)]),         # upper back
    (LIGHT, [(28, 1.770, .030, 0, 0), (28, 1.712, .058, .007, 0), (24, 1.626, .060, .007, 0), (21, 1.564, .044, .005, .002), (20, 1.542, 0, 0, .006)]),         # back, outer
    (LIGHT, [(9, 1.778, .026, 0, 0), (10, 1.730, .048, .006, 0), (13, 1.640, .052, .006, 0), (15, 1.564, .040, .004, .001), (15, 1.542, 0, 0, .003)]),         # back, inner
]
CREASE = (DARK, [(-3, 1.798, .008, 0, 0), (-4, 1.752, .009, .005, 0), (-3, 1.700, .006, .003, 0), (-2, 1.668, 0, 0, 0)])  # the reference's one dark line


def table(t, x):
    xs = [p[0] for p in t]
    i = min(max(bisect.bisect_right(xs, x), 1), len(xs) - 1)
    (x0, y0), (x1, y1) = t[i - 1], t[i]
    return y0 + (y1 - y0) * (x - x0) / (x1 - x0)


def ring(z):
    rows = RINGS[::-1]                     # ascending z
    z = max(rows[0][0], min(rows[-1][0], z))
    i = min(max(bisect.bisect_right([r[0] for r in rows], z), 1), len(rows) - 1)
    a, b = rows[i - 1], rows[i]
    t = (z - a[0]) / (b[0] - a[0])
    return [a[k] + (b[k] - a[k]) * t for k in (1, 2, 3)]


def on_mass(deg, z, out=0.0):
    """Point of the mass at this angle from the back (+Y), positive towards +X."""
    half, back, front = ring(z)
    th = math.radians(deg)
    cy, b = (back + front) / 2, (back - front) / 2
    # Squarer than an ellipse: the head has hard corners at the back that an ellipse leaves poking out.
    sx, sy = math.sin(th), math.cos(th)
    p = Vector((math.copysign(abs(sx) ** SQUARE, sx) * half, cy + math.copysign(abs(sy) ** SQUARE, sy) * b, z))
    return p + (p - CENTER).normalized() * out


def head_bvh(body):
    gi = body.vertex_groups["Head"].index
    me = body.data
    on = {v.index for v in me.vertices if any(g.group == gi and g.weight > .5 for g in v.groups)}
    faces = [list(p.vertices) for p in me.polygons if all(v in on for v in p.vertices)]
    return BVHTree.FromPolygons([body.matrix_world @ v.co for v in me.vertices], faces)


def clear_skull(bvh, p, c=CENTER):
    """The rings are drawn by hand: push out any point that would sink into the head."""
    d = (p - c).normalized()
    hit, *_ = bvh.ray_cast(c + d * .5, -d)
    if hit and (hit - c).length + SKULL_GAP > (p - c).length:
        return c + d * ((hit - c).length + SKULL_GAP)
    return p


def paint(face, spot, uvl):
    for i, l in enumerate(face.loops):
        a = 2 * math.pi * i / len(face.loops)
        l[uvl].uv = (spot[0] + .001 * math.cos(a), spot[1] + .001 * math.sin(a))


def add_mass(bm, uvl, bvh):
    c = CENTER
    top = bm.verts.new(Vector((0, -.025, RINGS[0][0] + .002)))
    cols, inner = [], []
    for j in range(COLS):
        deg = 360 * j / COLS
        low = table(EDGE, abs(180 - deg))
        col = [bm.verts.new(clear_skull(bvh, on_mass(deg, RINGS[0][0] + (low - RINGS[0][0]) * ((i + 1) / ROWS) ** 1.7))) for i in range(ROWS)]
        h = col[-1].co
        # Closes the underside so the mass never looks hollow from below or beside the face.
        inner.append(bm.verts.new(c + (h - c).to_2d().to_3d() * .72 + Vector((0, 0, h.z - c.z + .03))))
        cols.append(col)
    for j in range(COLS):
        n = (j + 1) % COLS
        paint(bm.faces.new((top, cols[j][0], cols[n][0])), MAIN, uvl)
        for i in range(ROWS - 1):
            paint(bm.faces.new((cols[j][i], cols[j][i + 1], cols[n][i + 1], cols[n][i])), MAIN, uvl)
        paint(bm.faces.new((cols[j][-1], inner[j], inner[n], cols[n][-1])), DARK, uvl)
    paint(bm.faces.new(inner), DARK, uvl)      # closes the shell, inside the head where it never shows


def add_lock(bm, uvl, colour, stations, sign):
    pts = [on_mass(sign * s[0], s[1], s[4]) for s in stations]
    rows = []
    for k, s in enumerate(stations):
        along = pts[min(k + 1, len(pts) - 1)] - pts[max(k - 1, 0)]
        normal = (pts[k] - CENTER).normalized()
        side = along.cross(normal).normalized()
        if s[2] == 0:
            rows.append(bm.verts.new(pts[k]))
            continue
        sink = .012 if k == 0 else .003      # locks grow out of the mass instead of starting on top of it
        rows.append((bm.verts.new(pts[k] - side * s[2] / 2 - normal * sink), bm.verts.new(pts[k] + normal * (0 if k == 0 else s[3])),
                     bm.verts.new(pts[k] + side * s[2] / 2 - normal * sink)))
    paint(bm.faces.new(rows[0]), SHADE, uvl)
    for a, b in zip(rows, rows[1:]):
        if isinstance(b, tuple):
            paint(bm.faces.new((a[0], b[0], b[1], a[1])), colour, uvl)
            paint(bm.faces.new((a[1], b[1], b[2], a[2])), colour, uvl)
            paint(bm.faces.new((a[2], b[2], b[0], a[0])), DARK, uvl)
        else:
            paint(bm.faces.new((a[0], b, a[1])), colour, uvl)
            paint(bm.faces.new((a[1], b, a[2])), colour, uvl)
            paint(bm.faces.new((a[2], b, a[0])), DARK, uvl)


def make():
    body = bpy.data.objects["CH_Lukas_RH07"]
    rig = bpy.data.objects["PJ_Rig"]
    old = bpy.data.objects.get("CH_Lukas_Hair")
    if old:
        data = old.data
        bpy.data.objects.remove(old)
        bpy.data.meshes.remove(data)
    bm = bmesh.new()
    uvl = bm.loops.layers.uv.new("UVMap")
    add_mass(bm, uvl, head_bvh(body))
    for colour, stations in LOCKS:
        for sign in (1, -1):
            add_lock(bm, uvl, colour, stations, sign)
    add_lock(bm, uvl, CREASE[0], CREASE[1], 1)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    me = bpy.data.meshes.new("CH_Lukas_Hair")
    bm.to_mesh(me)
    bm.free()
    me.materials.append(body.data.materials[0])
    ob = bpy.data.objects.new("CH_Lukas_Hair", me)
    body.users_collection[0].objects.link(ob)
    ob.parent = rig
    ob.modifiers.new("Armature", "ARMATURE").object = rig
    ob.vertex_groups.new(name="Head").add(range(len(me.vertices)), 1.0, "REPLACE")
    return dict(verts=len(me.vertices), tris=sum(len(p.vertices) - 2 for p in me.polygons))
