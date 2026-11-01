---
description: Assemble a room prefab (ROOM_/KEY_/TUT_) by hand from KIT_ pieces and props on the 2 m grid, with DoorSockets, spawns, one light tint, RoomVolume and optional surveillance CameraMount. Use when creating or editing rooms for the procedural floors.
argument-hint: "<room name, e.g. ROOM_F1_Storage_A> <W>x<D cells> [notes]"
---

# Build a room prefab

Read first: `Desarrollo/KIT_SPEC.md` §2–4, `Desarrollo/14-levels.md` (room list and theme), `Desarrollo/OBJECT_CATALOG.md` (props for this floor and archetype).

Steps
1. Open `Sandbox_Kit` (save the current scene first). Clear any previous room instance at the origin.
2. Plan on paper first and write it in the chat: size W×D cells, socket edges (≥ 2, on different edges; corridors on opposite edges), cover layout, spawns, light position. Get the plan visible before placing anything.
3. Create root `<Name>` at (0,0,0), scale 1, with children `Geometry`, `Props`, `Lights`, `Sockets`, `Spawns`, `Cover`, and `CameraMount` if surveillance is wanted.
4. Geometry: place `KIT_<F>_Floor_2x2` at every (2i, 0, 2j); `KIT_<F>_Ceiling_2x2` likewise; walls on every boundary edge **except socket edges**, using the placement table (South (2k+1,0,0) rot 0 · North (2k+1,0,2D) rot 180 · West (0,0,2k+1) rot 90 · East (2W,0,2k+1) rot 270). Use `place_asset_in_scene` + `set_transform`; parent under `Geometry`.
5. Sockets: for each socket edge create an empty `Socket_<Facing>_<k>` at the edge midpoint (same position as the missing wall), rotation so +Z points **out** of the room (South 180, North 0, West 270, East 90), add `DoorSocket` (once the component exists).
6. Props from the catalog, on the 0.5 m sub-grid, under `Props`; add `Cover` markers on cover props. Keep a 2 m clear lane between sockets.
7. Spawns: `EnemySpawn` (≤ 2, ≥ 6 m from sockets), `ItemSpawn` (tagged: Budget/Document/Emilia/Confiscated/Key/Switch), `PlayerSpawn` only in entry rooms.
8. Lights: exactly one tint (F1 amber sodium, F2 cold fluorescent, F3 crimson); 1–2 lights, max 1 shadowed.
9. `RoomVolume`: BoxCollider trigger covering the interior (inset 0.3), layer `RoomVolume`; `RoomInstance` on the root.
10. `create_prefab` into `Assets/_Project/Prefabs/Rooms/<F>/`. Run the menu `Heartbeat/Rooms/Refresh Definition` (via a scratchpad `execute_script` calling `EditorApplication.ExecuteMenuItem`) and fix every reported violation.
11. Verify with `capture_scene_object` (top view + eye level from each socket). `save_scene`.

Report: prefab path, size, sockets, spawns, props used, screenshots reviewed.
