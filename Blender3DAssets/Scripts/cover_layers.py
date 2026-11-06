"""Turns the cover scene of Portada.blend into named layers on the 1920x1080 canvas: fondo, personaje, título; the guide template is the camera's background image.
Lukas and the title stay inside the safe zone x 620-1300, y 200-880; the title between y 200 and 620; no text but the title.
Run inside Blender with Portada.blend open: build(), then bounds() checks the rules in canvas pixels."""
import os

import bpy
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Vector

ROOT = r"C:\Users\Chris\Heartbeat"
COVER = ROOT + r"\Blender3DAssets\Cover"
LUKAS_BLEND = ROOT + r"\Blender3DAssets\Blends\Characters\CH_Lukas_RH07.blend"
W, H = 1920, 1080
CAM_Y = -5.0                        # further back than the old render, so all of Lukas fits above y=880
SHIFT_PX = 66                       # the picture rises this many pixels
TITLE_BOX = (630, 210, 1290, 392)   # x0, y0, x1, y1 in canvas pixels
PLANE_DIST = .2                     # the logo rides this far in front of the camera
LOGO = ROOT + r"\Blender3DAssets\Logo\identity-dissolution-logo-horizontal.png"
LUKAS = ("PJ_Rig", "PJ_Rig_Place", "CH_Lukas_RH07", "CH_Lukas_Civil", "CH_Lukas_Hair", "WeaponGrip", "WeaponGrip_Barrel", "WeaponGrip_Up")
# Props with words painted on them.
LETTERED = ("PROP_F2_Console", "PROP_F3_ContainmentConsole", "PROP_Shared_VitalisPoster", "PROP_F1_WaterTank", "CH_F1_Boiler", "PROP_F2_SampleFridge", "CH_F3_Transformer", "PROP_Shared_MetalCrate")
LUKAS_FILL = 130     # watts of the area light on his back: the camera moved away, so he needs more to read against the dark


def layer(name):
    c = bpy.data.collections.get(name) or bpy.data.collections.new(name)
    if name not in bpy.context.scene.collection.children:
        bpy.context.scene.collection.children.link(c)
    return c


def move(ob, target):
    for c in list(ob.users_collection):
        c.objects.unlink(ob)
    target.objects.link(ob)


def to_camera(cam, px, py):
    """Local camera position of a canvas pixel on the plane PLANE_DIST in front of the camera."""
    width = PLANE_DIST * cam.data.sensor_width / cam.data.lens
    return Vector(((px - W / 2 + cam.data.shift_x * W) / W * width, (H / 2 - py + cam.data.shift_y * W) / W * width, -PLANE_DIST))


def add_hair():
    if "CH_Lukas_Hair" in bpy.data.objects:
        return
    with bpy.data.libraries.load(LUKAS_BLEND, link=False) as (src, dst):
        dst.objects = ["CH_Lukas_Hair"]
    hair, rig = dst.objects[0], bpy.data.objects["PJ_Rig"]
    bpy.data.collections["Cast"].objects.link(hair)
    hair.parent = rig
    hair.modifiers["Armature"].object = rig
    hair.data.materials[0] = bpy.data.objects["CH_Lukas_RH07"].data.materials[0]


def add_title(cam, target):
    """The horizontal logo on a plane, fitted to TITLE_BOX; it is the cover's only text."""
    for name in ("Titulo", "Logo"):
        old = bpy.data.objects.get(name)
        if old:
            bpy.data.objects.remove(old)
    image = bpy.data.images.load(LOGO, check_existing=True)
    image.reload()
    x0, y0, x1, y1 = TITLE_BOX
    scale = min((x1 - x0) / image.size[0], (y1 - y0) / image.size[1])
    w, h = image.size[0] * scale, image.size[1] * scale
    cx, cy = (x0 + x1) / 2, (y0 + y1) / 2
    a, b = to_camera(cam, cx - w / 2, cy - h / 2), to_camera(cam, cx + w / 2, cy + h / 2)
    me = bpy.data.meshes.new("Logo")
    me.from_pydata([(a.x, b.y, a.z), (b.x, b.y, a.z), (b.x, a.y, a.z), (a.x, a.y, a.z)], [], [(0, 1, 2, 3)])
    uv = me.uv_layers.new(name="UVMap")
    for i, c in enumerate(((0, 0), (1, 0), (1, 1), (0, 1))):
        uv.data[i].uv = c
    mat = bpy.data.materials.get("M_Logo") or bpy.data.materials.new("M_Logo")
    mat.use_nodes = True
    mat.surface_render_method = "BLENDED"
    nt = mat.node_tree
    nt.nodes.clear()
    tex = nt.nodes.new("ShaderNodeTexImage")
    tex.image = image
    emit, clear, mix, out = (nt.nodes.new(k) for k in ("ShaderNodeEmission", "ShaderNodeBsdfTransparent", "ShaderNodeMixShader", "ShaderNodeOutputMaterial"))
    nt.links.new(tex.outputs["Color"], emit.inputs["Color"])
    nt.links.new(tex.outputs["Alpha"], mix.inputs[0])
    nt.links.new(clear.outputs[0], mix.inputs[1])
    nt.links.new(emit.outputs[0], mix.inputs[2])
    nt.links.new(mix.outputs[0], out.inputs[0])
    me.materials.append(mat)
    ob = bpy.data.objects.new("Logo", me)
    target.objects.link(ob)
    ob.parent = cam


def add_guides(cam):
    """The template as the camera's background image: it shows only in camera view and never reaches the render."""
    old = bpy.data.objects.get("guias_svg")
    if old:
        bpy.data.objects.remove(old)
    layer_old = bpy.data.collections.get("guias")
    if layer_old:
        bpy.data.collections.remove(layer_old)
    cam.data.background_images.clear()
    cam.data.show_background_images = True
    bg = cam.data.background_images.new()
    bg.image = bpy.data.images.load(os.path.join(COVER, "utg-portada-guias.png"), check_existing=True)
    bg.display_depth = "FRONT"
    bg.frame_method = "FIT"
    bg.alpha = 1.0


def build():
    scene = bpy.context.scene
    cam = scene.camera
    cam.location.y = CAM_Y
    cam.data.shift_y = -SHIFT_PX / W
    scene.render.resolution_x, scene.render.resolution_y, scene.render.resolution_percentage = W, H, 100
    scene.display_settings.display_device = "sRGB"
    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    add_hair()

    root = scene.collection
    fondo, personaje, titulo = layer("fondo"), layer("personaje"), layer("título")
    for name in ("Set", "Lights", "Screens", "Cast"):
        old = bpy.data.collections.get(name)
        if old is None:
            continue
        if name == "Cast":
            for ob in list(old.objects):
                move(ob, personaje if ob.name in LUKAS else fondo)
            root.children.unlink(old)
            bpy.data.collections.remove(old)
        elif name in root.children:
            root.children.unlink(old)
            fondo.children.link(old)
    # Order of the outliner, bottom to top: fondo, personaje, título.
    for c in (fondo, personaje, titulo):
        root.children.unlink(c)
        root.children.link(c)

    for ob in bpy.data.objects:
        if ob.type == "FONT" or ob.name.split(".")[0] in LETTERED:
            ob.hide_render = ob.hide_viewport = True
    fill = min((o for o in bpy.data.objects if o.type == "LIGHT" and o.data.type == "AREA"), key=lambda o: (o.location - Vector((.2, -3.6, 3.6))).length)
    fill.data.energy = LUKAS_FILL
    add_title(cam, titulo)
    add_guides(cam)
    bpy.context.view_layer.update()
    return bounds()


def bounds():
    scene = bpy.context.scene
    dg = bpy.context.evaluated_depsgraph_get()
    out = {}
    for label, names in (("personaje", ("CH_Lukas_RH07", "CH_Lukas_Hair")), ("titulo", ("Logo",))):
        xs, ys = [], []
        for n in names:
            ev = bpy.data.objects[n].evaluated_get(dg)
            me = ev.to_mesh()
            for v in me.vertices:
                c = world_to_camera_view(scene, scene.camera, ev.matrix_world @ v.co)
                xs.append(c.x * W)
                ys.append((1 - c.y) * H)
            ev.to_mesh_clear()
        out[label] = [round(min(xs)), round(min(ys)), round(max(xs)), round(max(ys))]
    p, t = out["personaje"], out["titulo"]
    out["ok"] = p[0] >= 620 and p[2] <= 1300 and p[1] >= 200 and p[3] <= 880 and t[0] >= 620 and t[2] <= 1300 and t[1] >= 200 and t[3] <= 620
    return out


def render(path, title=False):
    """EEVEE, with the title layer left out unless asked for: the title goes on afterwards."""
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    bpy.data.collections["título"].hide_render = not title
    jpg = path.lower().endswith(".jpg")
    scene.render.image_settings.file_format = "JPEG" if jpg else "PNG"
    if jpg:
        scene.render.image_settings.quality = 95
    scene.render.filepath = path
    bpy.ops.render.render(write_still=True)
