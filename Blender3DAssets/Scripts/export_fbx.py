"""Exports a rig and optional mesh with the settings proved in Phase 0 (BLENDER_PLAN.md section 5)."""
import bpy


def export(path, rig, mesh=None, keep_prefix="AN_"):
    """Only actions named `keep_prefix`* reach the FBX. Others are deleted from memory: reopen the file afterwards."""
    vl = bpy.context.view_layer
    for o in vl.objects:
        o.select_set(False)
    rig.hide_set(False)
    rig.select_set(True)
    kinds = {"ARMATURE"}
    if mesh is not None:
        mesh.hide_set(False)
        mesh.select_set(True)
        kinds.add("MESH")
    vl.objects.active = rig
    # The FBX default pose must be the bind pose, not a frame of whichever clip was assigned last.
    if rig.animation_data:
        rig.animation_data.action = None
    for b in rig.pose.bones:
        b.rotation_mode = "QUATERNION"
        b.rotation_quaternion = (1, 0, 0, 0)
        b.location = (0, 0, 0)
    # Unity builds the imported hierarchy from the first take at time 0, so the first take must be the rest pose.
    bind = bpy.data.actions.get(keep_prefix + "000_Bind") or bpy.data.actions.new(keep_prefix + "000_Bind")
    rig.animation_data_create()
    rig.animation_data.action = bind
    for b in rig.pose.bones:
        for f in (1, 2):
            b.keyframe_insert("rotation_quaternion", frame=f, group=b.name)
            b.keyframe_insert("location", frame=f, group=b.name)
    rig.animation_data.action = None
    vl.update()
    for a in [a for a in bpy.data.actions if not a.name.startswith(keep_prefix)]:
        bpy.data.actions.remove(a)
    win = bpy.context.window_manager.windows[0]
    with bpy.context.temp_override(window=win):
        bpy.ops.export_scene.fbx(
            filepath=path, use_selection=True, object_types=kinds, apply_unit_scale=True,
            apply_scale_options="FBX_SCALE_ALL", axis_forward="-Z", axis_up="Y", add_leaf_bones=False,
            use_armature_deform_only=True, primary_bone_axis="Y", secondary_bone_axis="X", bake_anim=True,
            bake_anim_use_all_actions=True, bake_anim_use_nla_strips=False,
            bake_anim_force_startend_keying=True, bake_anim_simplify_factor=0.0, mesh_smooth_type="FACE")
