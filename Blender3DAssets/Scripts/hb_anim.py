"""Clip authoring helpers for the Heartbeat humanoid rig (D-151: every clip is made in Blender).

Poses are written as armature-space Euler degrees applied at each bone's rest frame:
  +X lean/fold forward (top goes to -Y, which is the way characters face), -X raises a hanging arm forward,
  +Y rolls towards the character's left, +Z yaws towards the character's left... see `rot`.
Keys are baked to every frame at 30 fps, so the FBX carries exactly what was reviewed.
"""
import math
import bpy
from mathutils import Euler, Quaternion, Vector

FPS = 30


def rig_action_bones(rig):
    return {b.name: b for b in rig.pose.bones}


def fcurves_of(action):
    return [fc for l in action.layers for s in l.strips for cb in s.channelbags for fc in cb.fcurves]


class Sampler:
    """Reads a source action at any frame, including the fractional frames left by the 24 to 30 fps retime."""

    def __init__(self, action):
        self.q = {}
        self.loc = {}
        self.start, self.end = action.frame_range
        for fc in fcurves_of(action):
            if '"' not in fc.data_path:
                continue
            bone = fc.data_path.split('"')[1]
            if fc.data_path.endswith("rotation_quaternion"):
                self.q.setdefault(bone, {})[fc.array_index] = fc
            elif fc.data_path.endswith("location"):
                self.loc.setdefault(bone, {})[fc.array_index] = fc

    def rot(self, bone, f):
        c = self.q.get(bone)
        if not c:
            return Quaternion()
        return Quaternion([c[i].evaluate(f) if i in c else (1 if i == 0 else 0) for i in range(4)])

    def location(self, bone, f):
        c = self.loc.get(bone)
        if not c:
            return Vector()
        return Vector([c[i].evaluate(f) if i in c else 0 for i in range(3)])


def smooth(t):
    return t * t * (3 - 2 * t)


class Author:
    def __init__(self, rig):
        self.rig = rig
        self.rest = {b.name: b.bone.matrix_local.to_quaternion() for b in rig.pose.bones}
        for b in rig.pose.bones:
            b.rotation_mode = "QUATERNION"

    def rot(self, bone, e):
        """Armature-space Euler (degrees) expressed in the bone's local frame."""
        r = Euler([math.radians(a) for a in e], "XYZ").to_quaternion()
        b = self.rest[bone]
        return b.inverted() @ r @ b

    def _pose_at(self, keys, f):
        """Interpolates the {frame: {bone: euler}} keys into {bone: local quaternion} at frame f."""
        frames = sorted(keys)
        if f <= frames[0]:
            a = b = frames[0]
            t = 0
        elif f >= frames[-1]:
            a = b = frames[-1]
            t = 0
        else:
            b = next(x for x in frames if x >= f)
            a = frames[frames.index(b) - 1]
            t = smooth((f - a) / (b - a)) if b != a else 0
        out, loc = {}, Vector()
        names = set(keys[a]) | set(keys[b])
        for n in names:
            if n == "loc":
                la = Vector(keys[a].get("loc", (0, 0, 0)))
                lb = Vector(keys[b].get("loc", (0, 0, 0)))
                loc = la.lerp(lb, t)
                continue
            qa = self.rot(n, keys[a].get(n, (0, 0, 0)))
            qb = self.rot(n, keys[b].get(n, (0, 0, 0)))
            out[n] = qa.slerp(qb, t)
        return out, loc

    def clip(self, name, frames, keys=None, base=None, warp=None, swing=None, loop=False):
        """Creates (or replaces) action `name`.

        keys:  {frame: {bone: (x, y, z), "loc": (x, y, z)}}; a single key means a constant offset.
        base:  source action name, sampled across the whole clip and looped over `frames`.
        warp:  fn(t in 0..1) -> t, to hold or hurry part of a base cycle (limps).
        swing: {bone: factor}: scales the base motion around its average, to lengthen or shorten a stride.
        """
        keys = keys or {1: {}}
        rig = self.rig
        src = Sampler(bpy.data.actions[base]) if base else None
        means = {}
        if src and swing:
            for bone in swing:
                acc = Quaternion((0, 0, 0, 0))
                n = 12
                first = None
                for i in range(n):
                    q = src.rot(bone, src.start + (src.end - src.start) * i / n)
                    first = first or q
                    if q.dot(first) < 0:
                        q = -q
                    acc = Quaternion([acc[j] + q[j] for j in range(4)])
                acc.normalize()
                means[bone] = acc
        old = bpy.data.actions.get(name)
        if old:
            bpy.data.actions.remove(old)
        act = bpy.data.actions.new(name)
        act.use_fake_user = True
        rig.animation_data_create()
        rig.animation_data.action = act
        hips_rest = self.rest["Hips"]
        for f in range(1, frames + 1):
            t = (f - 1) / max(frames - 1, 1)
            pose, loc = self._pose_at(keys, f)
            for b in rig.pose.bones:
                q = Quaternion()
                l = Vector()
                if src:
                    tt = warp(t) if warp else t
                    sf = src.start + (src.end - src.start) * tt
                    q = src.rot(b.name, sf)
                    if b.name in means:
                        if q.dot(means[b.name]) < 0:
                            q = -q
                        axis, angle = (means[b.name].inverted() @ q).to_axis_angle()
                        q = means[b.name] @ Quaternion(axis, angle * swing[b.name])
                    if b.name == "Hips":
                        l = src.location("Hips", sf)
                if b.name in pose:
                    q = q @ pose[b.name]
                if b.name == "Hips" and loc.length > 0:
                    l = l + hips_rest.inverted() @ loc
                b.rotation_quaternion = q
                b.location = l
                b.keyframe_insert("rotation_quaternion", frame=f, group=b.name)
                if b.name == "Hips":
                    b.keyframe_insert("location", frame=f, group=b.name)
        act.use_frame_range = True
        act.frame_start = 1
        act.frame_end = frames
        act.use_cyclic = loop
        return act


def grip(side, amount=1.0):
    """Closes a hand. Fingers curl towards the palm, which faces +X on the right hand and -X on the left."""
    n, s = ("Right", -1) if side == "R" else ("Left", 1)
    return {
        n + "Fingers": (0, 70 * amount * s, 0),
        n + "IndexProximal": (0, 45 * amount * s, 0),
        n + "IndexDistal": (0, 40 * amount * s, 0),
        n + "ThumbProximal": (0, 25 * amount * s, 0),
        n + "ThumbDistal": (0, 25 * amount * s, 0),
    }
