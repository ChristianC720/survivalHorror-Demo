"""Lukas civil (intro) and the 4 foreign-body textures (D-43). Run with Player.blend open; call make()."""
import os

import bmesh
import bpy
import numpy as np

ROOT = r"C:\Users\Chris\Heartbeat"
TEXDIR = ROOT + r"\Assets\_Project\Models\Characters\Textures"

JACKET = np.array([.30, .34, .29], dtype=np.float32)    # desaturated green-grey field jacket (GDD Personajes)
TROUSERS = np.array([.12, .12, .13], dtype=np.float32)
SKIN = np.array([.62, .47, .38], dtype=np.float32)
FLESH = np.array([.40, .34, .40], dtype=np.float32)     # the laboratory's piece: another body's skin
SEAM = np.array([.13, .07, .09], dtype=np.float32)


def load(name):
    im = bpy.data.images[name]
    w, h = im.size
    return np.array(im.pixels[:], dtype=np.float32).reshape(h, w, 4), w, h


def raster(uvs, w, h):
    pts = np.array(uvs) * [w, h]
    x0, y0 = np.maximum(np.floor(pts.min(0)).astype(int), 0)
    x1, y1 = np.ceil(pts.max(0)).astype(int)
    x1, y1 = min(x1, w - 1), min(y1, h - 1)
    yy, xx = np.mgrid[y0:y1 + 1, x0:x1 + 1]
    ins = np.zeros(xx.shape, bool)
    for i in range(1, len(pts) - 1):
        a, b, c = pts[0], pts[i], pts[i + 1]
        d1 = (xx - b[0]) * (a[1] - b[1]) - (yy - b[1]) * (a[0] - b[0])
        d2 = (xx - c[0]) * (b[1] - c[1]) - (yy - c[1]) * (b[0] - c[0])
        d3 = (xx - a[0]) * (c[1] - a[1]) - (yy - a[1]) * (c[0] - a[0])
        ins |= ~(((d1 < 0) | (d2 < 0) | (d3 < 0)) & ((d1 > 0) | (d2 > 0) | (d3 > 0)))
    return y0, x0, ins


def poly_bone(ob, m, p):
    names = {g.index: g.name for g in ob.vertex_groups}
    score = {}
    for i in p.vertices:
        for g in m.vertices[i].groups:
            score[names[g.group]] = score.get(names[g.group], 0) + g.weight
    return max(score, key=score.get) if score else ""


def save(arr, name):
    im = bpy.data.images.get(name) or bpy.data.images.new(name, arr.shape[1], arr.shape[0])
    im.pixels.foreach_set(arr.ravel())
    im.filepath_raw = rf"{TEXDIR}\{name}.png"
    im.file_format = "PNG"
    im.save()
    return im


def make_civil(body, A, w, h):
    old = bpy.data.objects.get("CH_Lukas_Civil")
    if old:
        bpy.data.objects.remove(old)
    ob = body.copy()
    ob.data = body.data.copy()
    ob.name = ob.data.name = "CH_Lukas_Civil"
    body.users_collection[0].objects.link(ob)
    m = ob.data
    uv = m.uv_layers.active.data
    T = A.copy()
    drop = []
    for p in m.polygons:
        uvs = [uv[l].uv[:] for l in p.loop_indices]
        c = p.center
        # Reconstruction hardware does not exist yet: back tubes and arm terminal go away (GDD: arms clean).
        if (c.y > .085 and 1.05 < c.z < 1.52 and abs(c.x) < .17) or (.335 < c.x < .385 and -.01 < c.y < .1 and .9 < c.z < 1.05):
            px = A[min(int(uvs[0][1] * h), h - 1), min(int(uvs[0][0] * w), w - 1)]
            if abs(px[0] - px[1]) < .07 and abs(px[1] - px[2]) < .09:
                drop.append(p.index)
                continue
        bone = poly_bone(ob, m, p)
        y0, x0, ins = raster(uvs, w, h)
        reg = T[y0:y0 + ins.shape[0], x0:x0 + ins.shape[1], :3]
        r, g, b = reg[:, :, 0], reg[:, :, 1], reg[:, :, 2]
        skin_like = (r > g * 1.08) & (g > b * 1.05) & (r > .32)
        hair_like = (r < .36) & (r > g * 1.08) & (g > b * 1.05)
        if "Hand" in bone or "Fingers" in bone or "Index" in bone or "Thumb" in bone:
            reg[ins] = np.where(skin_like[ins][:, None], reg[ins], SKIN)  # bare hands, no gloves
            continue
        if bone in ("Head",):
            continue                                                       # face and hair stay
        if bone in ("LeftFoot", "RightFoot", "LeftToes", "RightToes"):
            continue
        if c.z < .3:
            continue
        cloth = ins & ~skin_like
        reg[cloth] = JACKET if c.z > .82 else TROUSERS
        # Keep a little of the painted line work so folds still read.
        lines = ins & (r + g + b < .36) & ~skin_like
        reg[lines] = (JACKET if c.z > .82 else TROUSERS) * .55
    bm = bmesh.new()
    bm.from_mesh(m)
    bm.faces.ensure_lookup_table()
    bmesh.ops.delete(bm, geom=[bm.faces[i] for i in drop], context="FACES")
    bm.to_mesh(m)
    bm.free()
    im = save(T, "CH_Lukas_Civil_Albedo")
    mat = bpy.data.materials.get("M_Lukas_Civil") or bpy.data.materials.new("M_Lukas_Civil")
    mat.use_nodes = True
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    return ob, len(drop)


def make_variants(body, A, w, h):
    """Cumulative: each reconstruction shows one more foreign piece; the last is the ring hand."""
    m = body.data
    uv = m.uv_layers.active.data
    steps = [
        ("RightLowerArm",),
        ("Neck",),
        ("LeftHand", "LeftThumbProximal", "LeftThumbDistal", "LeftIndexProximal", "LeftIndexDistal", "LeftFingers"),
        ("RightHand", "RightThumbProximal", "RightThumbDistal", "RightIndexProximal", "RightIndexDistal", "RightFingers"),
    ]
    T = A.copy()
    for k, bones in enumerate(steps):
        for p in m.polygons:
            bone = poly_bone(body, m, p)
            # The jaw patch of step 2 reaches up into the head from the neck.
            jaw = k == 1 and bone == "Head" and p.center.z < 1.6 and p.center.y < .0
            if bone not in bones and not jaw:
                continue
            y0, x0, ins = raster([uv[l].uv[:] for l in p.loop_indices], w, h)
            reg = T[y0:y0 + ins.shape[0], x0:x0 + ins.shape[1], :3]
            r, g, b = reg[:, :, 0], reg[:, :, 1], reg[:, :, 2]
            skin = ins & (r > g * 1.08) & (g > b * 1.05) & (r > .32)
            reg[skin] = FLESH * (reg[skin].mean(axis=1, keepdims=True) / .47)
        save(T, f"CH_Lukas_RH07_Var{k + 1}")


def make():
    body = bpy.data.objects["CH_Lukas_RH07"]
    A, w, h = load("PJ_Personaje_Albedo")
    make_variants(body, A, w, h)
    civil, dropped = make_civil(body, A, w, h)
    civil.data.calc_loop_triangles()
    bpy.ops.wm.save_mainfile()
    return {"civil_tris": len(civil.data.loop_triangles), "dropped": dropped}
