"""Closes skinned meshes without joining body parts: drops zero-area islands, joins twin rims (the lip of a mouth and its
lining) and caps the small holes left. Refuses a cap wide enough to span two body parts.
drop_foreign_weights() repairs vertices pulled by a far bone (a hand tied to the thigh).
Run inside Blender: close(ob) returns a report and changes the mesh only with apply=True."""
from collections import defaultdict

import bmesh
import numpy as np

MAX_CAP_SPAN = .25       # metres: anything wider would bridge parts rather than close a hole
TWIN_RIM_GAP = .035      # metres between the centres of two rims of the same length


def counts(bm):
    return dict(open=sum(len(e.link_faces) == 1 for e in bm.edges), over=sum(len(e.link_faces) > 2 for e in bm.edges), faces=len(bm.faces))


def _remove_zero_islands(bm):
    bm.faces.ensure_lookup_table()
    seen, dead = set(), []
    for f in bm.faces:
        if f.index in seen:
            continue
        st, isl = [f], []
        seen.add(f.index)
        while st:
            x = st.pop()
            isl.append(x)
            for e in x.edges:
                for g in e.link_faces:
                    if g.index not in seen:
                        seen.add(g.index)
                        st.append(g)
        if sum(x.calc_area() for x in isl) < 2e-6:
            dead += isl
    bmesh.ops.delete(bm, geom=dead, context="FACES")
    return len(dead)


def _boundary_cycles(bm):
    adj = defaultdict(list)
    for e in bm.edges:
        if len(e.link_faces) == 1:
            adj[e.verts[0]].append(e)
            adj[e.verts[1]].append(e)
    seen, comps = set(), []
    for s in adj:
        if s in seen:
            continue
        st, vs, es = [s], [], set()
        seen.add(s)
        while st:
            v = st.pop()
            vs.append(v)
            for e in adj[v]:
                es.add(e)
                o = e.other_vert(v)
                if o not in seen:
                    seen.add(o)
                    st.append(o)
        comps.append((vs, list(es), all(len(adj[v]) == 2 for v in vs)))
    return comps


def _centre(vs):
    return sum((v.co for v in vs), vs[0].co * 0) / len(vs)


def _span(vs):
    a = np.array([v.co[:] for v in vs])
    return float((a.max(0) - a.min(0)).max())


def _orient(face):
    """Cap faces wind against the face across their rim edge, so the normal points out like its neighbours."""
    for l in face.loops:
        for g in l.edge.link_faces:
            if g is not face:
                other = next(x for x in g.loops if x.edge is l.edge)
                if other.vert is l.vert:
                    bmesh.ops.reverse_faces(bm_of(face), faces=[face])
                return


_bm = None


def bm_of(_face):
    return _bm


def _paint_cap(face, uvl):
    """A cap takes the material and the colour of the skin around it."""
    for l in face.loops:
        for g in l.edge.link_faces:
            if g is not face:
                face.material_index = g.material_index
                c = sum((x[uvl].uv for x in g.loops), g.loops[0][uvl].uv * 0) / len(g.loops)
                for x in face.loops:
                    x[uvl].uv = c
                return


def _ordered(vs, es):
    """Vertices of an open boundary chain, from one end to the other."""
    links = defaultdict(list)
    for e in es:
        links[e.verts[0]].append(e.verts[1])
        links[e.verts[1]].append(e.verts[0])
    ends = [v for v in vs if len(links[v]) == 1]
    if len(ends) != 2:
        return None
    path, prev = [ends[0]], None
    while path[-1] is not ends[1]:
        nxt = [o for o in links[path[-1]] if o is not prev]
        if len(nxt) != 1:
            return None
        prev = path[-1]
        path.append(nxt[0])
    return path


def _close_chain(bm, vs, es):
    """An open chain ends on edges shared by other sheets: its ends are closed by a face over the chain, or for a
    single loose edge by a triangle to the vertex both of its ends already connect to."""
    path = _ordered(vs, es)
    if not path:
        return None
    if len(path) == 2:
        a, b = path
        around_a = {e.other_vert(a) for e in a.link_edges}
        shared = [e.other_vert(b) for e in b.link_edges if e.other_vert(b) in around_a]
        if not shared:
            return None
        path = [a, b, min(shared, key=lambda v: (v.co - a.co).length)]
    try:
        return bm.faces.new(path)
    except ValueError:      # that face already exists
        return None


def _split_cycles(es):
    """Boundary shaped like a bow tie: walks it into the separate loops that touch at a vertex."""
    free = defaultdict(list)
    for e in es:
        free[e.verts[0]].append(e)
        free[e.verts[1]].append(e)
    cycles = []
    for start in list(free):
        while free[start]:
            path, v = [start], start
            while free[v]:
                e = free[v].pop()
                o = e.other_vert(v)
                free[o].remove(e)
                if o in path:
                    cycles.append(path[path.index(o):])
                    del path[path.index(o) + 1:]
                    v = o
                else:
                    path.append(o)
                    v = o
    return [c for c in cycles if len(c) >= 3]


def _join_twin_rims(bm, new):
    joined = 0
    comps = [c for c in _boundary_cycles(bm) if c[2] and len(c[0]) >= 8]
    used = set()
    for i, a in enumerate(comps):
        for j, b in enumerate(comps):
            if j <= i or i in used or j in used or len(a[0]) != len(b[0]):
                continue
            if (_centre(a[0]) - _centre(b[0])).length < TWIN_RIM_GAP:
                r = bmesh.ops.bridge_loops(bm, edges=a[1] + b[1])
                new += [f for f in r["faces"]]
                used |= {i, j}
                joined += 1
    return joined


def close(ob, apply=False):
    global _bm
    me = ob.data
    bm = bmesh.new()
    bm.from_mesh(me)
    _bm = bm
    uvl = bm.loops.layers.uv.active
    rep = dict(name=ob.name, before=counts(bm))
    rep["zero_area_faces"] = _remove_zero_islands(bm)
    new = []
    rep["rims_joined"] = _join_twin_rims(bm, new)
    capped, refused = 0, []
    for vs, es, simple in _boundary_cycles(bm):
        if _span(vs) > MAX_CAP_SPAN:
            refused.append(dict(n=len(vs), span=round(_span(vs), 3)))
            continue
        if simple:
            r = bmesh.ops.holes_fill(bm, edges=es, sides=0)
            new += r["faces"]
            capped += len(r["faces"])
        else:
            f = _close_chain(bm, vs, es)
            if f:
                new.append(f)
                capped += 1
            elif all(sum(v in e.verts for e in es) % 2 == 0 for v in vs):
                for c in _split_cycles(es):
                    try:
                        new.append(bm.faces.new(c))
                        capped += 1
                    except ValueError:      # that face already exists
                        pass
    rep["caps"], rep["refused"] = capped, refused
    bm.faces.ensure_lookup_table()
    for f in new:
        if f.is_valid:
            _orient(f)
            _paint_cap(f, uvl)
    bmesh.ops.triangulate(bm, faces=[f for f in new if f.is_valid and len(f.verts) > 4])
    rep["after"] = counts(bm)
    if apply:
        bm.to_mesh(me)
        me.update()
    bm.free()
    _bm = None
    return rep


def _segment_distance(p, a, b):
    ab = b - a
    t = np.clip(((p - a) * ab).sum(-1) / max((ab * ab).sum(), 1e-9), 0, 1)
    return np.linalg.norm(p - (a + t[..., None] * ab), axis=-1)


def drop_foreign_weights(ob, rig, ratio=3.0, floor=.07, apply=False):
    """Removes a bone's pull on a vertex when that bone is far away and another bone is right next to it."""
    me = ob.data
    wm = np.array(ob.matrix_world.inverted() @ rig.matrix_world)
    names = [g.name for g in ob.vertex_groups]
    p = np.array([v.co[:] for v in me.vertices])
    segs = {}
    for n in names:
        b = rig.data.bones.get(n)
        if b:
            segs[n] = ((wm[:3, :3] @ np.array(b.head_local)) + wm[:3, 3], (wm[:3, :3] @ np.array(b.tail_local)) + wm[:3, 3])
    dist = {n: _segment_distance(p, *s) for n, s in segs.items()}
    near = np.min(np.stack(list(dist.values())), axis=0)
    nearest = np.array(list(dist))[np.argmin(np.stack(list(dist.values())), axis=0)]
    fixed, dropped = 0, defaultdict(int)
    for v in me.vertices:
        bad = [g for g in v.groups if names[g.group] in dist and g.weight > 1e-4 and dist[names[g.group]][v.index] > max(near[v.index] * ratio, floor)]
        if not bad:
            continue
        fixed += 1
        for g in bad:
            dropped[names[g.group]] += 1
        if apply:
            for gn in [names[g.group] for g in bad]:
                ob.vertex_groups[gn].remove([v.index])
            left = [g for g in v.groups if g.weight > 1e-4]
            total = sum(g.weight for g in left)
            if total <= 1e-4:
                ob.vertex_groups[str(nearest[v.index])].add([v.index], 1.0, "REPLACE")
            else:
                for g in left:
                    g.weight /= total
    return dict(vertices=fixed, dropped=dict(sorted(dropped.items(), key=lambda x: -x[1])[:12]))


def move_past_joint(ob, rig, parent, child, past=.08, apply=False):
    """Vertices along a child bone but tied to its parent stay behind when the limb moves: their weight goes to the child."""
    me = ob.data
    wm = np.array(ob.matrix_world.inverted() @ rig.matrix_world)
    seg = lambda n: ((wm[:3, :3] @ np.array(rig.data.bones[n].head_local)) + wm[:3, 3], (wm[:3, :3] @ np.array(rig.data.bones[n].tail_local)) + wm[:3, 3])
    p = np.array([v.co[:] for v in me.vertices])
    a, b = seg(child)
    along = ((p - a) * (b - a)).sum(-1) / ((b - a) * (b - a)).sum()
    closer = _segment_distance(p, a, b) < _segment_distance(p, *seg(parent))
    gp, gc = ob.vertex_groups[parent], ob.vertex_groups[child]
    moved = 0
    for v in me.vertices:
        w = next((g.weight for g in v.groups if g.group == gp.index), 0)
        if w > .02 and closer[v.index] and along[v.index] > past:
            moved += 1
            if apply:
                gc.add([v.index], w, "ADD")
                gp.remove([v.index])
    return moved


def smooth_around_joint(ob, rig, bone, radius=.16, iterations=4, keep=4):
    """Averages weights with the neighbours near a joint, so chest and arm stop tearing apart at the armpit."""
    me = ob.data
    joint = ob.matrix_world.inverted() @ rig.matrix_world @ rig.data.bones[bone].head_local
    near = {v.index for v in me.vertices if (v.co - joint).length < radius}
    links = defaultdict(set)
    for e in me.edges:
        a, b = e.vertices
        links[a].add(b)
        links[b].add(a)
    w = {v.index: {g.group: g.weight for g in v.groups if g.weight > 1e-4} for v in me.vertices}
    for _ in range(iterations):
        nxt = {}
        for i in near:
            mix = defaultdict(float)
            for g, x in w[i].items():
                mix[g] += x * .5
            share = .5 / max(len(links[i]), 1)
            for n in links[i]:
                for g, x in w[n].items():
                    mix[g] += x * share
            nxt[i] = mix
        w.update(nxt)
    for i in near:
        top = dict(sorted(w[i].items(), key=lambda t: -t[1])[:keep])
        total = sum(top.values())
        for g in list(ob.vertex_groups):
            if g.index in top:
                g.add([i], top[g.index] / total, "REPLACE")
            else:
                g.remove([i])
    return len(near)
