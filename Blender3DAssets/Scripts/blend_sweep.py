"""One .blend per game model, in Blender3DAssets/Blends with the same folders as Assets/_Project/Models.

Each file is the FBX the game uses, re-imported: mesh, UVs, materials and, on characters, rig, weights and clips. Textures are linked
by relative path, from the same PNGs the game reads, and shown with the same material. Run sweep() after any export.
ModelsNew already holds the team's own files. To change a generated model for good, edit its builder (env_models, team_models,
hb_props) or edit the .blend and re-export over the FBX: the next run of a builder overwrites hand edits on that FBX.
"""
import os
import re

import bpy

ROOT = r"C:\Users\Chris\Heartbeat"
MODELS = ROOT + r"\Assets\_Project\Models"
OUT = ROOT + r"\Blender3DAssets\Blends"


def _guid_map():
    guids = {}
    for folder, _, files in os.walk(MODELS):
        for f in files:
            if f.endswith(".png.meta"):
                m = re.search(r"^guid: (\w+)", open(os.path.join(folder, f), encoding="utf-8").read(), re.M)
                if m:
                    guids[m.group(1)] = os.path.join(folder, f[:-5])
    return guids


def _maps(mat_path, guids):
    """(albedo, emission) paths from a Unity material."""
    s = open(mat_path, encoding="utf-8").read()
    def tex(prop):
        m = re.search(prop + r":\s*\n\s*m_Texture: \{[^}]*guid: (\w+)", s)
        return guids.get(m.group(1)) if m else None
    return tex("_BaseMap"), tex("_EmissionMap")


def _unity_materials(rel, name):
    """The Unity materials on this model's slots, in slot order."""
    folder = rel.split(os.sep)[0]
    p = MODELS + "\\"
    if folder == "Enemies":
        short = name.replace("CH_", "")
        if short == "Alto_V1":
            return [p + r"Enemies\M_CH_Alto_V1.mat", p + r"Enemies\M_CH_Alto_V1_Cloth.mat"]
        return [x for x in (p + rf"Enemies\M_{short}.mat", p + rf"Enemies\M_{short}_Cloth.mat") if os.path.exists(x)]
    if folder == "Statics":
        return [p + rf"Statics\M_{name}.mat"]
    if folder == "Characters":
        return [p + r"Characters\M_Lukas_RH07.mat"]
    if name == "CH_Revolver":
        return [p + r"Props\M_Revolver.mat"]
    if name == "CH_Item_Terminal":
        return [p + r"Props\M_Herramientas.mat"]
    return [p + r"Props\M_Palette.mat"]


def _image(path, directory):
    im = bpy.data.images.load(path, check_existing=True)
    im.filepath = bpy.path.relpath(path, start=directory)
    return im


def _shade(mat, albedo, emission, directory):
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    bsdf = next((n for n in nodes if n.type == "BSDF_PRINCIPLED"), None)
    if bsdf is None:
        return
    for n in [n for n in nodes if n.type == "TEX_IMAGE"]:
        nodes.remove(n)
    if albedo:
        t = nodes.new("ShaderNodeTexImage")
        t.image = _image(albedo, directory)
        t.interpolation = "Closest"
        mat.node_tree.links.new(t.outputs["Color"], bsdf.inputs["Base Color"])
    if emission:
        t = nodes.new("ShaderNodeTexImage")
        t.image = _image(emission, directory)
        t.interpolation = "Closest"
        t.location.y = -300
        mat.node_tree.links.new(t.outputs["Color"], bsdf.inputs["Emission Color"])
        bsdf.inputs["Emission Strength"].default_value = 1.0


def sweep(only=None):
    guids = _guid_map()
    tex = MODELS + r"\Env\Textures"
    done, missing = [], []
    for folder, _, files in os.walk(MODELS):
        if "ModelsNew" in folder:
            continue
        for f in sorted(files):
            if not f.endswith(".fbx") or (only and only not in f):
                continue
            path = os.path.join(folder, f)
            name = f[:-4]
            rel = os.path.relpath(path, MODELS)
            target = os.path.join(OUT, rel[:-4] + ".blend")
            directory = os.path.dirname(target)
            os.makedirs(directory, exist_ok=True)
            bpy.ops.wm.read_homefile(use_empty=True)
            with bpy.context.temp_override(window=bpy.context.window_manager.windows[0]):
                bpy.ops.import_scene.fbx(filepath=path, use_anim=True, automatic_bone_orientation=False)
            meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
            unity = _unity_materials(rel, name)
            for o in meshes:   # statics reach the FBX without a material
                if not o.data.materials:
                    o.data.materials.append(bpy.data.materials.new(os.path.basename(unity[0])[:-4]))
            slots = [m for o in meshes for m in o.data.materials if m]
            i = 0
            for m in slots:
                base = m.name.split(".")[0]
                if base == "M_Lukas_Civil":
                    albedo, emission = _maps(MODELS + r"\Characters\M_Lukas_Civil.mat", guids)
                    _shade(m, albedo, emission, directory)
                    continue
                if base == "MAT_Glass":
                    m.use_nodes = True
                    continue
                if base.startswith("M_Env_"):
                    floor = base[len("M_Env_"):]
                    _shade(m, tex + rf"\T_Env_{floor}.png", tex + rf"\T_Env_{floor}_Emission.png", directory)
                    continue
                source = unity[min(i, len(unity) - 1)] if unity else None
                i += 1
                if source and os.path.exists(source):
                    albedo, emission = _maps(source, guids)
                    _shade(m, albedo, emission, directory)
                    if not albedo:
                        missing.append(f"{name}: {os.path.basename(source)} has no base map")
                else:
                    missing.append(f"{name}: no material for {base}")
            bpy.ops.wm.save_as_mainfile(filepath=target, relative_remap=False, compress=True)
            done.append(name)
    return dict(count=len(done), missing=missing)
