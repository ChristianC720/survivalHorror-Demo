"""Concept-art pass over the cover scene of Portada.blend: one backlit figure, one monster in the fog, red/black/cyan only.
Fog hides the clutter (Silent Hill), the light cuts a silhouette (Resident Evil), the palette stays minimal (Signalis).
Only the game's own models. In Blender: apply(), save. In the background: render(base_png), then finish(base_png, out_jpg)
grades the render and lays the logo on top, so the painted filter never touches the letters.
Same safe zone as cover_layers: Lukas inside x 620-1300, y 200-880; title between y 200 and 620."""
import math
import os

import bpy
import numpy as np
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Euler, Vector

ROOT = r"C:\Users\Chris\Heartbeat"
LOGO = ROOT + r"\Blender3DAssets\Logo\identity-dissolution-logo-horizontal.png"
TITLE_BOX = (630, 210, 1290, 392)
W, H = 1920, 1080
RED = (1.0, .075, .055)     # deep red, not neon
EMBER = (1.0, .16, .05)
COLD = (.30, .85, 1.0)
LENS, CAM_Y, CAM_Z = 28.0, -6.64, 1.05      # long lens, far back and low: the set flattens into planes and Lukas towers
FEET_PX = 872                               # Lukas's feet on the canvas
HIDE = ("PROP_", "CH_F", "Txt", "Housing", "Panel", "ECG", "BackBoard", "Glass", "Mullion", "Header", "KIT_F2", "ColumnBand", "ColumnRib", "CH_Lamp_F2")
HIDE_CAST = ("CH_Lukas_Civil",)
# name, kind, location, target it aims at, watts, colour, size (spot: cone degrees)
LIGHTS = (
    ("CA_Contra", "SPOT", (.15, 3.4, 1.05), (.15, -3, .1), 2200, RED, 54),        # the backlight: cuts him out and throws his shadow at the camera
    ("CA_Foso", "AREA", (0, 5, -1.1), (0, 5, 3), 760, RED, 5),                   # the pit breathes red upwards
    ("CA_Fondo", "SPOT", (2.2, 15, 4.5), (1.7, 3.6, .8), 3000, EMBER, 22),            # shafts through the fog behind the monster
    ("CA_Alto", "SPOT", (2.0, 8.2, 1.9), (1.75, 3.6, 1.5), 1350, RED, 26),         # glow behind the Alto: it reads as a black shape
    ("CA_Mudo", "SPOT", (-2.6, 9.6, .6), (-2.6, 6.2, -.3), 1200, RED, 26),
    ("CA_Boca", "SPOT", (-2.7, 10.5, 2.8), (-2.7, 7.4, 2.2), 700, RED, 24),
    ("CA_Rastrero", "SPOT", (3.9, 4.4, 1.7), (2.6, 1.8, .8), 360, RED, 22),
    ("CA_Filo", "SPOT", (2.6, .6, 3.2), (0, -.9, 1.35), 420, COLD, 12),          # the only cold note: a thin edge on one shoulder
    ("CA_Relleno", "AREA", (0, -6, 3), (0, -.9, 1), 26, COLD, 5),                 # just enough to read the suit
)
# Each one stands on real ground, measured by ray casts in Portada.blend: deck z 0, back catwalk z 1.4, lower catwalk under it z -1.4, each also in clear view of the camera. All face Lukas.
LUKAS_AT = (0, -.9)
PLACE = {"Rig_Alto_Place": (1.75, 3.6, 0), "Rig_Mudo_Place": (-2.6, 6.2, -1.4), "Rig_Boca_Place": (-2.7, 7.4, 1.4), "Rig_Rastrero_Place": (2.6, 1.8, 0)}


def collection(name):
    c = bpy.data.collections.get(name) or bpy.data.collections.new(name)
    if name not in bpy.context.scene.collection.children:
        bpy.context.scene.collection.children.link(c)
    return c


def aim(ob, target):
    ob.rotation_euler = (Vector(target) - ob.location).to_track_quat("-Z", "Y").to_euler()


def add_lights():
    group = collection("concept_luces")
    for ob in bpy.data.objects:
        if ob.type == "LIGHT" and not ob.name.startswith("CA_"):
            ob.hide_render = ob.hide_viewport = True
    for name, kind, loc, target, watts, colour, size in LIGHTS:
        ob = bpy.data.objects.get(name)
        if ob is None:
            ob = bpy.data.objects.new(name, bpy.data.lights.new(name, kind))
            group.objects.link(ob)
        ob.location = loc
        aim(ob, target)
        ob.data.energy, ob.data.color = watts, colour
        if kind == "SPOT":
            ob.data.spot_size, ob.data.spot_blend, ob.data.shadow_soft_size = math.radians(size), .35, .12
        else:
            ob.data.size = size


def fog():
    """Uneven fog: thicker low down, torn by noise, so the light has something to paint on."""
    m = bpy.data.objects["Haze"].data.materials[0]
    m.use_nodes = True
    nt = m.node_tree
    nt.nodes.clear()
    coord, noise, ramp, low, sep, mul, vol, out = (nt.nodes.new(k) for k in (
        "ShaderNodeTexCoord", "ShaderNodeTexNoise", "ShaderNodeValToRGB", "ShaderNodeMapRange", "ShaderNodeSeparateXYZ", "ShaderNodeMath",
        "ShaderNodeVolumePrincipled", "ShaderNodeOutputMaterial"))
    noise.inputs["Scale"].default_value = .32
    noise.inputs["Detail"].default_value = 5
    noise.inputs["Roughness"].default_value = .55
    ramp.color_ramp.elements[0].position, ramp.color_ramp.elements[1].position = .34, .68
    ramp.color_ramp.elements[0].color = (.25, .25, .25, 1)
    sep_in = coord.outputs["Object"]
    nt.links.new(sep_in, noise.inputs["Vector"])
    nt.links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    nt.links.new(sep_in, sep.inputs[0])
    low.inputs["From Min"].default_value, low.inputs["From Max"].default_value = -.1, .4     # object space of the fog cube
    low.inputs["To Min"].default_value, low.inputs["To Max"].default_value = .05, .008
    nt.links.new(sep.outputs["Z"], low.inputs["Value"])
    mul.operation = "MULTIPLY"
    nt.links.new(ramp.outputs["Color"], mul.inputs[0])
    nt.links.new(low.outputs[0], mul.inputs[1])
    nt.links.new(mul.outputs[0], vol.inputs["Density"])
    vol.inputs["Color"].default_value = (.95, .92, .92, 1)
    vol.inputs["Anisotropy"].default_value = .38
    nt.links.new(vol.outputs[0], out.inputs["Volume"])


def compositor():
    """Painted edges and glow, in the render itself. The grade and the logo come after, in finish()."""
    scene = bpy.context.scene
    ng = bpy.data.node_groups.get("CoverComp")
    if ng:
        bpy.data.node_groups.remove(ng)
    ng = bpy.data.node_groups.new("CoverComp", "CompositorNodeTree")
    ng.interface.new_socket("Image", in_out="OUTPUT", socket_type="NodeSocketColor")
    rl, kuwa, glare, lens, out = (ng.nodes.new(k) for k in ("CompositorNodeRLayers", "CompositorNodeKuwahara", "CompositorNodeGlare", "CompositorNodeLensdist", "NodeGroupOutput"))
    rl.scene = scene
    kuwa.inputs["Size"].default_value = 5
    kuwa.inputs["Type"].default_value = "Anisotropic"
    kuwa.inputs["Sharpness"].default_value = .75
    for name, value in (("Type", "Bloom"), ("Threshold", .55), ("Strength", .7), ("Size", .85)):
        glare.inputs[name].default_value = value
    lens.inputs["Dispersion"].default_value = .012
    ng.links.new(rl.outputs["Image"], kuwa.inputs["Image"])
    ng.links.new(kuwa.outputs[0], glare.inputs["Image"])
    ng.links.new(glare.outputs["Image"], lens.inputs["Image"])
    ng.links.new(lens.outputs[0], out.inputs[0])
    scene.compositing_node_group = ng
    scene.render.use_compositing = True


def frame():
    scene = bpy.context.scene
    cam = scene.camera
    cam.data.lens = LENS
    cam.location = (.05, CAM_Y, CAM_Z)
    cam.rotation_euler = Euler((math.radians(90), 0, 0))
    cam.data.shift_y = 0
    bpy.context.view_layer.update()
    # Shift, not tilt: the verticals stay monumental. Solved by secant, the pixel slope of a shift is not obvious.
    s0, f0 = 0.0, bounds()["personaje"][3]
    s1 = .02
    for _ in range(4):
        cam.data.shift_y = s1
        bpy.context.view_layer.update()
        f1 = bounds()["personaje"][3]
        if abs(f1 - FEET_PX) < 1 or f1 == f0:
            break
        s0, f0, s1 = s1, f1, s1 + (FEET_PX - f1) * (s1 - s0) / (f1 - f0)


def apply():
    scene = bpy.context.scene
    for ob in bpy.data.objects:
        if ob.name.startswith(HIDE):
            ob.hide_render = ob.hide_viewport = True
        if ob.name.split(".")[0] in HIDE_CAST:
            ob.hide_render = ob.hide_viewport = True
    for name, loc in PLACE.items():
        place = bpy.data.objects[name]
        place.location = loc
        # The models face -Y at yaw 0.
        place.rotation_euler = (0, 0, math.atan2(LUKAS_AT[0] - loc[0], -(LUKAS_AT[1] - loc[1])))
        for ob in place.children_recursive:
            if ob.type == "MESH" and ob.name.split(".")[0] not in HIDE_CAST:
                ob.hide_render = ob.hide_viewport = False
    add_lights()
    fog()
    frame()
    compositor()
    world = scene.world.node_tree.nodes["Background"]
    world.inputs[0].default_value = (.002, .001, .002, 1)
    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    scene.render.engine = "CYCLES"
    scene.cycles.volume_bounces = 2
    scene.cycles.volume_step_rate = .6
    bpy.data.collections["título"].hide_render = True      # finish() lays the logo over the painted image
    return bounds()


def bounds():
    scene = bpy.context.scene
    dg = bpy.context.evaluated_depsgraph_get()
    xs, ys = [], []
    for n in ("CH_Lukas_RH07", "CH_Lukas_Hair"):
        ev = bpy.data.objects[n].evaluated_get(dg)
        me = ev.to_mesh()
        for v in me.vertices:
            c = world_to_camera_view(scene, scene.camera, ev.matrix_world @ v.co)
            xs.append(c.x * W)
            ys.append((1 - c.y) * H)
        ev.to_mesh_clear()
    p = [round(min(xs)), round(min(ys)), round(max(xs)), round(max(ys))]
    return {"personaje": p, "ok": p[0] >= 620 and p[2] <= 1300 and p[1] >= 200 and p[3] <= 880, "titulo_libre": p[1] > TITLE_BOX[3]}


def render(path, samples=64, percent=100):
    scene = bpy.context.scene
    prefs = bpy.context.preferences.addons["cycles"].preferences
    prefs.compute_device_type = "CUDA"      # OptiX kernel fails to compile on this machine
    prefs.get_devices()
    for d in prefs.devices:
        d.use = d.type == "CUDA"
    scene.cycles.device = "GPU"
    scene.cycles.samples = samples
    scene.cycles.use_denoising = True
    scene.cycles.denoiser = "OPENIMAGEDENOISE"
    scene.render.resolution_x, scene.render.resolution_y, scene.render.resolution_percentage = W, H, percent
    scene.render.image_settings.file_format = "PNG"
    scene.render.filepath = path
    bpy.ops.render.render(write_still=True)


def _read(path):
    im = bpy.data.images.load(path, check_existing=False)
    im.colorspace_settings.name = "Non-Color"      # pixels as stored: display-referred sRGB values
    w, h = im.size
    px = np.empty(w * h * 4, dtype=np.float32)
    im.pixels.foreach_get(px)
    bpy.data.images.remove(im)
    return px.reshape(h, w, 4)


def _resize(a, w, h):
    ys = np.linspace(0, a.shape[0] - 1, h).astype(int)
    xs = np.linspace(0, a.shape[1] - 1, w).astype(int)
    return a[ys][:, xs]


def finish(base, out, logo=True):
    """Grade: crushed blacks, a vignette that closes the corners, film grain. Then the logo, untouched by any of it."""
    a = _read(base)
    h, w = a.shape[:2]
    rgb = a[..., :3]
    rgb = np.clip((rgb - .03) / .97, 0, 1) ** 1.3     # deeper blacks: the light has to fight for each shape
    luma = rgb @ np.array((.2126, .7152, .0722), dtype=np.float32)
    shadows = np.clip(1 - luma * 3.2, 0, 1)[..., None]
    rgb = rgb * (1 - shadows * .12) + rgb.mean(-1, keepdims=True) * np.array((.92, 1.0, 1.1), dtype=np.float32) * shadows * .12
    y, x = np.mgrid[0:h, 0:w].astype(np.float32)
    d = np.sqrt(((x - w * .5) / (w * .62)) ** 2 + ((y - h * .47) / (h * .68)) ** 2)
    rgb *= (1 - np.clip((d - .52) / .75, 0, 1) ** 1.6 * .86)[..., None]
    rng = np.random.default_rng(7)      # same grain every time
    rgb += rng.normal(0, .011, (h, w, 1)).astype(np.float32) * (.45 + .55 * (1 - luma[..., None]))
    if logo:
        mark = _read(LOGO)
        x0, y0, x1, y1 = (round(v * w / W) for v in TITLE_BOX)
        scale = min((x1 - x0) / mark.shape[1], (y1 - y0) / mark.shape[0])
        mw, mh = round(mark.shape[1] * scale), round(mark.shape[0] * scale)
        mark = _resize(mark, mw, mh)
        left, bottom = (x0 + x1 - mw) // 2, h - (y0 + y1 + mh) // 2      # the pixel array runs bottom-up
        region = rgb[bottom:bottom + mh, left:left + mw]
        region[:] = region * (1 - mark[..., 3:]) + mark[..., :3] * mark[..., 3:]
    result = np.dstack((np.clip(rgb, 0, 1), np.ones((h, w, 1), dtype=np.float32)))
    im = bpy.data.images.new("_cover", w, h, alpha=False)
    im.colorspace_settings.name = "Non-Color"
    im.pixels.foreach_set(result.ravel())
    im.filepath_raw = out
    im.file_format = "JPEG" if out.lower().endswith(".jpg") else "PNG"
    bpy.context.scene.render.image_settings.quality = 95
    im.save()
    bpy.data.images.remove(im)
    return os.path.getsize(out)
