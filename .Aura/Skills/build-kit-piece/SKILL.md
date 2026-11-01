---
description: Build one modular kit piece (floor tile, wall, doorway, ceiling, ramp, platform, column) by hand in the Sandbox_Kit scene with grid snapping and an exact pivot, then save it as a prefab. Use when creating or fixing KIT_ prefabs.
argument-hint: "<piece name, e.g. KIT_F1_Wall_2m>"
---

# Build a kit piece

Spec: `Desarrollo/KIT_SPEC.md` §1–2 (read the table row for this piece first).

Steps
1. `get_unity_editor_state`. If the active scene is not `Assets/_Project/Scenes/Sandbox_Kit.unity`, save the current scene, then `open_scene` it (create it with `create_scene` in `Assets/_Project/Scenes` with `add_to_editor=false` if it does not exist).
2. Make sure snapping is on: move 2 m (kit) / 0.5 m, rotate 90°. If unsure, run a scratchpad `execute_script` that sets `EditorSnapSettings.gridSnapEnabled = true; EditorSnapSettings.move = new Vector3(2,2,2); EditorSnapSettings.rotate = 90`.
3. Create an empty root named exactly like the piece at world (0,0,0), rotation 0, **scale (1,1,1)**. The root's position *is* the pivot.
4. Add primitive children (Cube, Cylinder) and set `set_transform` with exact local values so the geometry matches the extent in the spec (e.g. a wall's cube: local position (0, 1.5, 0.15), scale (2, 3, 0.3)). No child may exceed the spec extent.
5. Materials: use or create `Materials/<F>/MAT_<F>_<Surface>` (`create_pbr_material`, URP/Lit, smoothness ≤ 0.2, palette colour from `Desarrollo/12-art-render.md`), then `assign_material`.
6. Keep the primitive colliders (walls, floors); remove the collider on purely decorative children.
7. Run the menu `Heartbeat/Kit/Finish Kit Pieces In Scene` (scratchpad `execute_script` calling `Heartbeat.EditorTools.KitPieceTools.FinishAllInScene()`). It sets layers (`Environment`, or `Ceiling` for `_Ceiling` pieces) and static flags on every `KIT_*` root and prints each piece's bounds next to its spec extent. Fix any mismatch.
8. `create_prefab` into `Assets/_Project/Prefabs/Kit/<F>/<Name>.prefab`.
9. Verify: the bounds report from step 7, and a visual check. `capture_scene_object` frames poorly on single pieces, so enter Play Mode and use `run_play_mode_sequence` captures with `from` + `look_at`, or assemble a 2×2 test room with the wall placement table.
10. Delete the scene instance unless you are about to build a room with it, then `save_scene`.

Report: prefab path, extent measured, any deviation from the spec.
