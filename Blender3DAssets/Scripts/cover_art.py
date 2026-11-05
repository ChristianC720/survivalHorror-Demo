"""Cover art (1920x1080, no title): Lukas from behind, looking at the three floors of the lab.
F1 sodium amber on top, F2 cold fluorescent on the right, F3 crimson in the pit. Built from the game's own .blend models.
Run inside Blender: exec(open(path).read()); build(); render(preview=True)
"""
import math
import os

import bpy
from mathutils import Euler, Matrix, Vector

HB = r"C:\Users\Chris\Heartbeat"
BLENDS = os.path.join(HB, "Blender3DAssets", "Blends")
OUT_DIR = os.path.join(HB, "Blender3DAssets", "Cover")
FONT = os.path.join(HB, "Assets", "_Project", "UI", "Fonts", "VT323-Regular.ttf")

AMBER = (1.0, 0.42, 0.08)
COLD = (0.45, 1.0, 0.86)
RED = (1.0, 0.03, 0.02)
CYAN = (0.098, 0.769, 0.863)
GREEN = (0.45, 0.95, 0.45)
UP = Matrix.Rotation(math.radians(90), 4, "X")   # the game's models are stored Y-up


def wipe():
    for o in list(bpy.data.objects):
        bpy.data.objects.remove(o)
    for block in (bpy.data.meshes, bpy.data.curves, bpy.data.lights, bpy.data.cameras, bpy.data.armatures, bpy.data.actions,
                  bpy.data.materials, bpy.data.images, bpy.data.fonts):
        for d in list(block):
            block.remove(d)
    for c in list(bpy.data.collections):
        bpy.data.collections.remove(c)


cache = {}


def coll(name):
    c = bpy.data.collections.get(name)
    if c is None:
        c = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(c)
    return c


def asset(rel):
    """Mesh objects of a static model, appended once and kept out of the scene."""
    if rel not in cache:
        with bpy.data.libraries.load(os.path.join(BLENDS, rel + ".blend"), link=False) as (a, b):
            b.objects = list(a.objects)
        cache[rel] = [o for o in b.objects if o is not None and o.type == "MESH"]
    return cache[rel]


def place(rel, x, y, z=0.0, yaw=0.0, s=1.0, group="Set"):
    """yaw 0: the model's forward looks at the camera (-Y)."""
    m = Matrix.Translation((x, y, z)) @ Matrix.Rotation(math.radians(yaw), 4, "Z") @ Matrix.Scale(s, 4) @ UP
    made = []
    for src in asset(rel):
        o = bpy.data.objects.new(os.path.basename(rel), src.data)
        o.matrix_world = m @ src.matrix_world
        coll(group).objects.link(o)
        made.append(o)
    return made


def floors(rel, x0, x1, y0, y1, z, group="Set"):
    x = x0
    while x < x1 - 0.01:
        y = y0
        while y < y1 - 0.01:
            place(rel, x + 2, y + 2, z, group=group)   # pivot at the far corner of the tile
            y += 2
        x += 2


def wall_x(rel, x0, x1, y, z, yaw=0.0, group="Set"):
    x = x0 + 1
    while x < x1 + 0.01:
        place(rel, x, y, z, yaw, group=group)
        x += 2


def wall_y(rel, y0, y1, x, z, yaw=90.0, group="Set"):
    y = y0 + 1
    while y < y1 + 0.01:
        place(rel, x, y, z, yaw, group=group)
        y += 2


def mat(name, color, emission=0.0, rough=0.6, metal=0.6):
    m = bpy.data.materials.get(name)
    if m:
        return m
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    b = m.node_tree.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = (*color, 1)
    b.inputs["Roughness"].default_value = rough
    b.inputs["Metallic"].default_value = metal
    if emission:
        b.inputs["Base Color"].default_value = (0, 0, 0, 1)
        b.inputs["Emission Color"].default_value = (*color, 1)
        b.inputs["Emission Strength"].default_value = emission
    return m


def tube(points, r, material, smooth=False, name="Pipe", group="Set"):
    c = bpy.data.curves.new(name, "CURVE")
    c.dimensions = "3D"
    c.bevel_depth = r
    c.bevel_resolution = 3
    c.resolution_u = 16
    if smooth:
        s = c.splines.new("BEZIER")
        s.bezier_points.add(len(points) - 1)
        for p, q in zip(s.bezier_points, points):
            p.co = q
            p.handle_left_type = p.handle_right_type = "AUTO"
    else:
        s = c.splines.new("POLY")
        s.points.add(len(points) - 1)
        for p, q in zip(s.points, points):
            p.co = (*q, 1)
    c.materials.append(material)
    o = bpy.data.objects.new(name, c)
    coll(group).objects.link(o)
    return o


def box(center, size, material, name="Box", group="Set"):
    me = bpy.data.meshes.new(name)
    hx, hy, hz = (v / 2 for v in size)
    vs = [(sx * hx, sy * hy, sz * hz) for sx in (-1, 1) for sy in (-1, 1) for sz in (-1, 1)]
    me.from_pydata(vs, [], [(0, 1, 3, 2), (4, 6, 7, 5), (0, 4, 5, 1), (2, 3, 7, 6), (0, 2, 6, 4), (1, 5, 7, 3)])
    me.materials.append(material)
    o = bpy.data.objects.new(name, me)
    o.location = center
    coll(group).objects.link(o)
    return o


def text(body, loc, size, material, yaw=0.0, group="Screens", align="LEFT"):
    c = bpy.data.curves.new("Txt", "FONT")
    c.body = body
    c.size = size
    c.align_x = align
    if os.path.exists(FONT):
        c.font = bpy.data.fonts.load(FONT, check_existing=True)
    c.materials.append(material)
    o = bpy.data.objects.new("Txt", c)
    o.location = loc
    o.rotation_euler = Euler((math.radians(90), 0, math.radians(yaw)))
    coll(group).objects.link(o)
    return o


def screen(center, w, h, color, strength, yaw=0.0, frame=0.06, group="Screens"):
    """Glowing panel in a dark housing. Local +X is its width; it looks towards -Y before the yaw."""
    housing = box(center, (w + frame * 2, 0.08, h + frame * 2), mat("Housing", (0.03, 0.03, 0.035)), "Housing", group)
    r = math.radians(yaw)
    fwd = Vector((math.sin(r), -math.cos(r), 0))
    panel = box(Vector(center) + fwd * 0.045, (w, 0.005, h), mat(f"Glow_{color}_{strength}", color, strength), "Panel", group)
    housing.rotation_euler.z = panel.rotation_euler.z = r
    return Vector(center) + fwd * 0.055, Vector((math.cos(r), math.sin(r), 0))


def ecg(origin, right, width, height, material):
    shape = [0, 0, .05, -.05, 0, 0, .9, -.5, .1, 0, 0, .18, .05, 0, 0, 0, .05, -.05, 0, 0, .9, -.5, .1, 0, 0, .18, 0]
    pts = [origin + right * (width * i / (len(shape) - 1)) + Vector((0, 0, v * height)) for i, v in enumerate(shape)]
    tube(pts, 0.009, material, name="ECG", group="Screens")


def light(kind, loc, color, energy, size=1.0, rot=(0, 0, 0), group="Lights", size_y=None):
    d = bpy.data.lights.new("L", kind)
    d.color = color
    d.energy = energy
    if kind == "AREA":
        d.shape = "RECTANGLE"
        d.size = size
        d.size_y = size_y if size_y is not None else size
    elif kind in ("POINT", "SPOT"):
        d.shadow_soft_size = size
    o = bpy.data.objects.new("L", d)
    o.location = loc
    o.rotation_euler = rot
    coll(group).objects.link(o)
    return o


def character(rel, rig_name, action_key, frame, loc, yaw, hide=(), s=1.0, group="Cast"):
    """Appends a rigged character and poses it with one of its clips."""
    with bpy.data.libraries.load(os.path.join(BLENDS, rel + ".blend"), link=False) as (a, b):
        b.objects = list(a.objects)
        b.actions = [n for n in a.actions if n.endswith(action_key)]
    rig = None
    for o in b.objects:
        if o is None:
            continue
        coll(group).objects.link(o)
        if o.name.startswith(rig_name) and o.type == "ARMATURE":
            rig = o
        if o.name.split(".")[0] in hide:
            o.hide_render = o.hide_viewport = True
    act = b.actions[0]
    rig.animation_data_create()
    rig.animation_data.action = act
    if act.slots:
        rig.animation_data.action_slot = act.slots[0]
    start, end = act.frame_range
    bpy.context.scene.frame_set(int(start + (end - start) * frame))
    rig["cover_frame"] = int(start + (end - start) * frame)
    # Keep the pose while other clips are used on other rigs: bake to pose bones by leaving one frame on a held action.
    rig.animation_data.action_extrapolation = "HOLD"
    holder = bpy.data.objects.new(rig_name + "_Place", None)
    coll(group).objects.link(holder)
    holder.matrix_world = Matrix.Translation(loc) @ Matrix.Rotation(math.radians(yaw), 4, "Z") @ Matrix.Scale(s, 4)
    rig.parent = holder
    return rig


def freeze(rig):
    """The whole scene shares one frame, so each rig's clip is cut to its chosen pose."""
    f = rig["cover_frame"]
    act = rig.animation_data.action
    for bag in act.layers[0].strips[0].channelbags:
        for fc in bag.fcurves:
            v = fc.evaluate(f)
            fc.keyframe_points.clear() if hasattr(fc.keyframe_points, "clear") else None
            fc.keyframe_points.insert(1, v)


def build():
    wipe()
    scene = bpy.context.scene
    scene.frame_set(1)

    steel = mat("Steel", (0.05, 0.055, 0.06), rough=0.5, metal=0.9)
    rust = mat("Rust", (0.12, 0.06, 0.035), rough=0.75, metal=0.5)
    cable = mat("Cable", (0.012, 0.012, 0.014), rough=0.5, metal=0.0)

    # ---------- The deck Lukas stands on: main ledge, left wing, right wing ----------
    floors("Env/F1/KIT_F1_Floor_2x2", -3, 1, -12, 0, 0)
    floors("Env/F1/KIT_F1_Floor_2x2", -11, -3, -12, 4, 0)
    floors("Env/F1/KIT_F1_Floor_2x2", 1, 11, -12, 6, 0)
    for ix in range(-12, 12):
        for iy in range(-12, 7):
            x, y = ix + 0.5, iy + 0.5
            if (-3 <= x <= 1 and y > 0) or (x < -3 and y > 4):
                continue
            place("Env/Shared/PROP_Shared_Grate", x, y, 0.005)

    # Railing round the pit
    def rail(a, b):
        a, b = Vector(a), Vector(b)
        for h in (1.05, 0.62):
            tube([(a.x, a.y, h), (b.x, b.y, h)], 0.03, steel, name="Rail")
        n = max(1, int((b - a).length / 1.3))
        for i in range(n + 1):
            p = a.lerp(b, i / n)
            tube([(p.x, p.y, 0), (p.x, p.y, 1.05)], 0.028, steel, name="Post")
    rail((-3, 0), (1, 0))
    rail((1, 0), (1, 6))
    rail((1, 6), (10, 6))
    rail((-3, 0), (-3, 4))
    rail((-3, 4), (-10, 4))

    # ---------- F3, crimson core: the pit ----------
    for z in (-9, -3):
        wall_x("Env/F3/KIT_F3_Wall_2m_High", -12, 12, 15, z)
    floors("Env/F3/KIT_F3_Floor_2x2", -12, 12, 9, 15, -3)
    wall_y("Env/F3/KIT_F3_Wall_2m_High", -1, 15, -11.5, -9, 90)
    wall_y("Env/F3/KIT_F3_Wall_2m_High", -1, 15, 11.5, -9, -90)
    for x in (-7.5, -3.8, 2.2, 6.2):
        place("Env/F3/PROP_F3_CoolingColumn", x, 13.4, -3)
    place("Props/CH_F3_CryoTank", -1.0, 12.8, -3)
    place("Props/CH_F3_Transformer", 0.9, 11.6, -3, 20)
    place("Env/F3/PROP_F3_ContainmentConsole", -1.6, 10.2, -3, 15)
    place("Env/F3/PROP_F3_CRTBank", 4.4, 13.6, -3)
    for i, x in enumerate((-9, -7, -5, -3, -1, 1, 3, 5, 7)):
        place("Env/F3/PROP_F3_HeavyCabling", x, 9.6 + (i % 2) * .4, -3, 8 * ((i % 3) - 1))
    # The glowing core wall below the rail
    box((-1, 14.6, -1.2), (6.5, 0.1, 3.2), mat("CoreGlow", RED, 5.0), "CoreGlow", "Lights")
    text("VITALIS", (-1.0, 14.5, -1.1), 1.0, mat("CoreText", (0.012, 0.0, 0.0), rough=0.8, metal=0.0), 0, align="CENTER")
    place("Env/Shared/PROP_Shared_VitalisPoster", -1.0, 14.45, -1.6, s=2.2)
    place("Env/F1/KIT_F1_Column", -4.6, 4.2, -9, s=1.0)
    catwalk_z = -1.5
    place("Env/Shared/CH_Env_Catwalk", -1, 6.2, catwalk_z)
    light("AREA", (-1, 10, -2.6), RED, 1000, 12, group="Lights", size_y=6)
    light("POINT", (-1, 4.5, -3.5), RED, 1300, 2)
    light("POINT", (3.5, 8, -2), RED, 350, 2)
    light("POINT", (-6, 8, -2), RED, 350, 2)

    # ---------- F2, cold clinic: the glass room on the right ----------
    z2 = 1.1
    floors("Env/F2/KIT_F2_Floor_2x2", 3, 11, 7, 13, z2)
    floors("Env/F2/KIT_F2_Ceiling_2x2", 3, 11, 7, 13, z2 + 3)
    frame = mat("GlassFrame", (0.03, 0.05, 0.05), rough=0.4)
    glass = bpy.data.materials.new("LabGlass")
    glass.use_nodes = True
    gb = glass.node_tree.nodes["Principled BSDF"]
    gb.inputs["Base Color"].default_value = (0.75, 1.0, 0.95, 1)
    gb.inputs["Transmission Weight"].default_value = 1.0
    gb.inputs["Roughness"].default_value = 0.03
    for x in (3, 5, 7, 9, 11):
        box((x, 7.2, z2 + 1.5), (0.09, 0.09, 3), frame, "Mullion")
    for y in (9.2, 11.2, 13):
        box((3, y, z2 + 1.5), (0.09, 0.09, 3), frame, "Mullion")
    box((7, 7.2, z2 + 0.28), (8, 0.1, 0.56), frame, "Parapet")
    box((3, 10.1, z2 + 0.28), (0.1, 5.8, 0.56), frame, "Parapet")
    box((7, 7.2, z2 + 2.95), (8, 0.12, 0.14), frame, "Header")
    box((3, 10.1, z2 + 2.95), (0.12, 5.8, 0.14), frame, "Header")
    box((7, 7.2, z2 + 1.7), (8, 0.02, 2.4), glass, "Glass")
    box((3, 10.1, z2 + 1.7), (0.02, 5.8, 2.4), glass, "Glass")
    wall_x("Env/F2/KIT_F2_Wall_2m", 3, 11, 13, z2)
    place("Env/F2/PROP_F2_SurgeryTable", 5.0, 10, z2, 80)
    place("Env/F2/PROP_F2_SurgicalLamp", 5.0, 10, z2)
    place("Env/F2/PROP_F2_SampleFridge", 3.9, 12.3, z2)
    place("Env/F2/PROP_F2_SampleFridge", 4.8, 12.3, z2)
    place("Env/F2/PROP_F2_Console", 7.6, 12, z2)
    place("Props/CH_F2_MonsterCapsule", 9.2, 11.2, z2)
    place("Env/F2/PROP_F2_IVCart", 6.6, 9.4, z2, 30)
    place("Env/F2/PROP_F2_LabChair", 7.3, 10.4, z2, 200)
    for x in (4.6, 6.8, 9):
        for y in (9, 11.5):
            place("Env/F2/CH_Lamp_F2", x, y, z2 + 2.72)
    light("AREA", (6.8, 10, z2 + 2.6), COLD, 190, 7, (0, 0, 0), size_y=5)
    light("POINT", (6.5, 5.5, z2 + 1.2), COLD, 220, 1.2)
    # Its supports, down into the pit
    for x in (3.4, 7, 10.6):
        place("Env/F3/KIT_F3_Column", x, 7.2, -3, s=1.4)
    # High catwalk with cold light over it
    place("Env/Shared/CH_Env_Catwalk", 7.2, 8.2, 6.3)
    for x in (5.2, 7.2, 9.2):
        place("Env/F2/CH_Lamp_F2", x, 9, 8.6)
    light("AREA", (7.2, 9, 8.5), COLD, 260, 6, size_y=2)

    # ---------- F1, sodium maintenance: the gallery on top ----------
    z1 = 4.5
    floors("Env/F1/KIT_F1_Floor_2x2", -11, 3, 7, 15, z1)
    floors("Env/F1/KIT_F1_Ceiling_2x2", -11, 3, 7, 15, z1 + 3.3)
    wall_x("Env/F1/KIT_F1_Wall_2m", -11, 3, 14.6, z1)
    wall_x("Env/F1/KIT_F1_Wall_2m", -11, 3, 14.6, z1 + 3)
    wall_y("Env/F1/KIT_F1_Wall_2m", 7, 15, 3, z1, -90)
    for x in (-8.5, -6.2, -3.9):
        place("Env/F1/PROP_F1_ToolShelf", x, 13.9, z1, 90)
    place("Env/F1/PROP_F1_WaterTank", -0.8, 13.1, z1)
    place("Props/CH_F1_Boiler", 1.5, 13.0, z1)
    place("Env/F1/PROP_F1_Workbench", -5.4, 11.8, z1)
    place("Env/F1/PROP_F1_Workbench", -2.8, 10.8, z1, 12)
    place("Env/F1/PROP_F1_Tools", -5.4, 11.8, z1 + 1.1)
    place("Env/Shared/PROP_Shared_MetalCrate", -9.2, 9.5, z1)
    place("Env/Shared/PROP_Shared_MetalCrate", 1.2, 9.0, z1, 25)
    for x in (-9, -6, -3, 0):
        for y in (9.2, 12.4):
            place("Env/F1/CH_Lamp_F1", x, y, z1 + 3.1)
            box((x, y, z1 + 3.0), (1.5, 0.12, 0.05), mat("SodiumBar", AMBER, 22), "Sodium", "Lights")
    light("AREA", (-4, 11, z1 + 2.9), AMBER, 2100, 12, size_y=7)
    light("POINT", (-4, 8.4, z1 + 1.2), AMBER, 520, 1.5)
    light("POINT", (1.2, 8.6, z1 + 1.2), AMBER, 200, 1)
    light("POINT", (-1.5, 5.8, 3.4), AMBER, 40, 0.6)   # picks the Boca out of the dark walkway
    for x in range(-11, 4):
        tube([(x, 7.1, z1), (x, 7.1, z1 + 1.05)], 0.025, steel, name="Post")
    tube([(-11, 7.1, z1 + 1.05), (3, 7.1, z1 + 1.05)], 0.03, steel, name="Rail")
    tube([(-11, 7.1, z1 + 0.6), (3, 7.1, z1 + 0.6)], 0.03, steel, name="Rail")
    box((-4, 7.2, z1 - 0.25), (14, 0.5, 0.5), rust, "Beam")

    # Walkway between the amber floor and the pit, with the enemy on it
    place("Env/Shared/CH_Env_Catwalk", -5, 7.4, 1.3)
    place("Env/Shared/CH_Env_Catwalk", -0.2, 7.4, 1.3)
    wall_x("Env/F1/KIT_F1_Wall_2m", -11, 3, 14.5, 1.3)

    # ---------- Structure and foreground ----------
    def column(x, y, w):
        box((x, y, 0.5), (w, w, 20), steel, "Column")
        for z in (0.06, 2.6, 5.2, 7.8):
            box((x, y, z), (w + 0.1, w + 0.1, 0.14), rust, "ColumnBand")
        for dx in (-w / 2 - 0.02, w / 2 + 0.02):
            box((x + dx, y - w / 2 - 0.01, 0.5), (0.05, 0.05, 20), rust, "ColumnRib")
    column(-2.75, 1.6, 0.95)   # the big column left of Lukas
    column(7.0, 4.0, 0.95)
    floors("Env/F1/KIT_F1_Ceiling_2x2", -13, 13, -12, 16, 10.4)
    for x in (-8, -3.5, 1, 5.5, 10):
        box((x, 5, 10.1), (0.5, 26, 0.5), rust, "Girder")
    for y in (-3, 4, 11):
        box((0, y, 9.8), (28, 0.4, 0.4), steel, "Girder")

    # Pipework around the big left column, as in the reference
    tube([(-5.8, 1.15, 2.72), (-3.42, 1.15, 2.72), (-3.42, 1.15, 0)], 0.13, rust, name="Pipe")
    tube([(-3.05, 1.0, 0), (-3.05, 1.0, 1.25), (-2.1, 1.0, 1.25), (-2.1, 1.0, 0)], 0.1, steel, name="Pipe")
    tube([(-2.3, 2.1, 7.5), (-2.3, 2.1, 3.9), (-1.2, 2.1, 3.9), (-1.2, 6.5, 3.9)], 0.13, rust, name="Pipe")
    tube([(-6, 3.4, 5.6), (-3.8, 3.4, 5.6), (-3.8, 3.4, 8.8)], 0.17, rust, name="Pipe")
    tube([(7, 3.1, 4.7), (4.9, 3.1, 4.7), (4.9, 7, 4.7)], 0.15, rust, name="Pipe")
    tube([(6.2, 3.3, 2.9), (6.2, 3.3, 8.2), (2.7, 6.2, 8.2)], 0.11, steel, name="Pipe")
    tube([(-10, 5, 8.8), (10, 5, 8.8)], 0.2, steel, name="Pipe")
    tube([(-10, 6.2, 8.3), (10, 6.2, 8.3)], 0.12, rust, name="Pipe")
    tube([(-10, 6.0, 7.7), (2.8, 6.0, 7.7), (5, 9, 7.7)], 0.08, steel, name="Pipe")

    # Hanging cables, the dark loops that frame the top
    tube([(-3.2, 0.6, 5.8), (-2.3, 0.6, 3.15), (-1.0, 0.6, 3.5), (-0.3, 0.6, 5.8)], 0.028, cable, True, "Cable")
    tube([(-2.9, 0.9, 5.8), (-1.9, 0.9, 3.75), (-1.2, 0.9, 5.8)], 0.02, cable, True, "Cable")
    tube([(1.5, 1.0, 5.8), (2.6, 1.0, 3.1), (4.1, 1.0, 3.25), (5.0, 1.0, 5.8)], 0.035, cable, True, "Cable")
    tube([(2.3, 0.7, 5.8), (3.3, 0.7, 3.75), (4.0, 0.7, 5.8)], 0.022, cable, True, "Cable")
    tube([(1.2, 0.6, 0.03), (2.2, -0.5, 0.05), (3.4, 0.2, 0.03), (4.6, -0.6, 0.05), (6.2, 0.2, 0.03)], 0.03, cable, True, "Cable")
    tube([(2.4, 3.6, 0.03), (3.4, 2.5, 0.05), (4.6, 3.0, 0.03), (6.4, 2.2, 0.05)], 0.028, cable, True, "Cable")

    # ---------- Terminals on the deck, left ----------
    place("Env/F2/PROP_F2_Console", -3.45, 0.75, 0, 8)
    place("Env/F2/PROP_F2_Console", -4.45, 0.9, 0, 8)
    box((-4.4, 2.12, 2.2), (3.2, 0.1, 4.4), mat("BackBoard", (0.03, 0.035, 0.038), rough=0.6), "BackBoard")

    # Vitalis wall screen
    o, r = screen((-4.25, 2.0, 3.3), 1.5, 0.92, (0.012, 0.09, 0.11), 1.0, 0)
    cyan_text = mat("TextCyan", CYAN, 9)
    text("VITALIS", o + Vector((-0.13, 0, -0.03)), 0.33, cyan_text, 0, align="CENTER")
    text("KONZERN", o + Vector((-0.13, 0, -0.24)), 0.17, cyan_text, 0, align="CENTER")
    # Data screen with the subject's pulse
    o, r = screen((-4.25, 2.0, 1.95), 1.5, 0.92, (0.015, 0.11, 0.09), 1.0, 0)
    text("SUBJECT R-H-07", o - r * 0.68 + Vector((0, 0, 0.3)), 0.11, mat("TextCold", COLD, 8), 0)
    ecg(o - r * 0.68 + Vector((0, 0, -0.1)), r, 1.36, 0.33, mat("LineCold", COLD, 9))
    light("POINT", (-4.2, 1.2, 1.9), COLD, 12, 0.5)
    light("POINT", (-4.2, 1.2, 3.3), CYAN, 16, 0.6)

    # ---------- Terminals on the deck, right ----------
    place("Env/F3/PROP_F3_ContainmentConsole", 4.75, 2.75, 0, -12)
    place("Env/F3/PROP_F3_CRTBank", 6.15, 3.15, 0, -12)
    board = box((5.55, 3.62, 1.3), (2.6, 0.1, 2.6), mat("BackBoard", (0.03, 0.035, 0.038), rough=0.6), "BackBoard")
    board.rotation_euler.z = math.radians(-12)
    o, r = screen((4.85, 3.35, 2.0), 1.05, 0.64, (0.13, 0.04, 0.005), 1.0, -12)
    t_amber = mat("TextAmber", AMBER, 8)
    text("LIGHT 12%\nDRUM  2/6", o - r * 0.44 + Vector((0, 0, 0.1)), 0.15, t_amber, -12)
    o, r = screen((6.0, 3.28, 2.0), 1.22, 0.64, (0.03, 0.002, 0.002), 1.0, -12)
    red_text = mat("TextRed", (1.0, 0.09, 0.05), 12)
    text("SYSTEM FAILURE", o + Vector((0, 0, 0.04)), 0.17, red_text, -12, align="CENTER")
    text("AUSSER BETRIEB", o + Vector((0, 0, -0.15)), 0.12, red_text, -12, align="CENTER")
    light("POINT", (4.8, 2.6, 2.0), AMBER, 16, 0.5)
    light("POINT", (6.0, 2.8, 2.0), (1, 0.1, 0.05), 16, 0.5)

    # ---------- The cast ----------
    lukas = character("Characters/CH_Lukas_RH07", "PJ_Rig", "AN_Lukas_Idle_Fine", 0.25, (0.0, -0.9, 0.0), 180.0, hide=("CH_Lukas_Civil",))
    crawler = character("Enemies/CH_Rastrero", "Rig_Rastrero", "AN_Rastrero_Crawl", 0.3, (2.5, 1.9, 0.0), -55.0)
    boca = character("Enemies/CH_Boca", "Rig_Boca", "AN_Boca_Scream", 0.5, (-1.5, 7.4, 1.3), 15.0)
    alto = character("Enemies/CH_Alto", "Rig_Alto", "AN_Alto_Idle", 0.3, (-1.75, 6.2, catwalk_z), 18.0)
    mudo = character("Enemies/CH_Mudo", "Rig_Mudo", "AN_Mudo_Still", 0.3, (1.9, 5.1, 0.0), -25.0)
    for rig in (lukas, crawler, boca, alto, mudo):
        freeze(rig)
    scene.frame_set(1)

    # Rim light that peels Lukas off the background: red from below, cold from the right, amber from the top left
    light("SPOT", (0.2, 1.2, -1.2), RED, 240, 0.4, (math.radians(-65), 0, 0))
    light("SPOT", (3.6, 0.6, 3.4), COLD, 170, 0.3, (math.radians(-70), 0, math.radians(-75)))
    light("SPOT", (-2.8, 1.0, 3.8), AMBER, 150, 0.3, (math.radians(-70), 0, math.radians(70)))
    light("AREA", (0.2, -3.6, 3.6), (0.5, 0.75, 0.8), 62, 3, (math.radians(25), 0, 0))
    # Low fill over the deck, so the foreground reads
    light("AREA", (-6, -4, 5), COLD, 26, 8, (math.radians(10), 0, 0))
    light("AREA", (6, -4, 5), (0.9, 0.6, 0.4), 20, 8, (math.radians(10), 0, 0))

    # ---------- Camera, world, render ----------
    cam_data = bpy.data.cameras.new("Cover")
    cam_data.lens = 20
    cam_data.sensor_width = 36
    cam = bpy.data.objects.new("Cover", cam_data)
    cam.location = (0.05, -4.3, 1.5)
    cam.rotation_euler = Euler((math.radians(90), 0, 0))
    scene.collection.objects.link(cam)
    scene.camera = cam

    world = bpy.data.worlds.get("Cover") or bpy.data.worlds.new("Cover")
    world.use_nodes = True
    world.node_tree.nodes["Background"].inputs[0].default_value = (0.004, 0.008, 0.008, 1)
    world.node_tree.nodes["Background"].inputs[1].default_value = 1.0
    scene.world = world

    # Haze: it is what makes the three lights read as three places
    haze = box((0, 2, 0.5), (32, 36, 24), bpy.data.materials.new("Haze"), "Haze", "Lights")
    hm = haze.data.materials[0]
    hm.use_nodes = True
    nt = hm.node_tree
    nt.nodes.clear()
    vol = nt.nodes.new("ShaderNodeVolumePrincipled")
    vol.inputs["Density"].default_value = 0.009
    vol.inputs["Color"].default_value = (0.9, 0.9, 0.9, 1)
    output = nt.nodes.new("ShaderNodeOutputMaterial")
    nt.links.new(vol.outputs[0], output.inputs["Volume"])

    return {"objects": len(bpy.data.objects), "lukas_height": round(max((o.dimensions.y for o in lukas.children), default=0), 2)}


def bloom(scene):
    """Glow of the screens and lamps in the haze, in the compositor."""
    ng = bpy.data.node_groups.get("CoverComp")
    if ng:
        bpy.data.node_groups.remove(ng)
    ng = bpy.data.node_groups.new("CoverComp", "CompositorNodeTree")
    ng.interface.new_socket("Image", in_out="OUTPUT", socket_type="NodeSocketColor")
    rl = ng.nodes.new("CompositorNodeRLayers")
    rl.scene = scene
    glare = ng.nodes.new("CompositorNodeGlare")
    out = ng.nodes.new("NodeGroupOutput")
    ng.links.new(rl.outputs["Image"], glare.inputs["Image"])
    ng.links.new(glare.outputs["Image"], out.inputs[0])
    for name, value in (("Type", "Bloom"), ("Threshold", 0.75), ("Strength", 0.55), ("Size", 0.75)):
        if name in glare.inputs:
            glare.inputs[name].default_value = value
    scene.compositing_node_group = ng
    scene.render.use_compositing = True


def render(preview=True, samples=None, name=None):
    scene = bpy.context.scene
    scene.render.engine = "CYCLES"
    prefs = bpy.context.preferences.addons["cycles"].preferences
    prefs.compute_device_type = "CUDA"   # OptiX kernel fails to compile on this machine
    prefs.get_devices()
    for d in prefs.devices:
        d.use = d.type == "CUDA"
    scene.cycles.device = "GPU"
    scene.cycles.samples = samples or (48 if preview else 512)
    scene.cycles.use_denoising = True
    scene.cycles.denoiser = "OPENIMAGEDENOISE"
    scene.cycles.max_bounces = 6
    scene.cycles.volume_bounces = 1
    scene.cycles.use_adaptive_sampling = True
    scene.render.resolution_x = 1920
    scene.render.resolution_y = 1080
    scene.render.resolution_percentage = 50 if preview else 100
    scene.render.image_settings.file_format = "PNG"
    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    scene.view_settings.exposure = 0.0
    bloom(scene)
    os.makedirs(OUT_DIR, exist_ok=True)
    path = os.path.join(OUT_DIR, name or ("preview.png" if preview else "Portada_1920x1080.png"))
    scene.render.filepath = path
    bpy.ops.render.render(write_still=True)
    return path
