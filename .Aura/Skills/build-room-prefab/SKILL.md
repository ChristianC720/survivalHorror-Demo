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
4. Root components: `RoomInstance`, `RoomShell` (set `cellsX`, `cellsZ`, `kit` = `ScriptableObjects/Rooms/KitSet_F<n>`, `tall` for 6 m rooms), `BoxCollider` (isTrigger, center (W, 1.5, D), size (2W-0.6, 3, 2D-0.6)), layer `RoomVolume`. Do NOT place floor/ceiling/perimeter walls: `RoomShell` builds them at runtime. Only interior kit pieces (inner walls, columns, ramps, platforms) go under `Geometry`.
5. Sockets: `place_asset_in_scene` `Prefabs/Level/SOCKET.prefab` as `Socket_<Facing>_<k>` at the edge midpoint (South (2k+1,0,0) · North (2k+1,0,2D) · West (0,0,2k+1) · East (2W,0,2k+1)), rotation so +Z points **out** of the room (South 180, North 0, West 270, East 90).
6. Props from the catalog, on the 0.5 m sub-grid, under `Props`; add `Cover` markers on cover props. Keep a 2 m clear lane between sockets.
7. Spawns: `Prefabs/Level/SP_Enemy` (≤ 2, ≥ 6 m from sockets), `SP_Item` (`itemTag`: Budget/Document/Emilia/Confiscated/Key/Switch), `SP_Player` only in entry rooms.
8. Lights: `Prefabs/Level/Light_F<n>_*` — exactly one tint; 1–2 lights, max 1 shadowed.
9. Aura cannot set `Vector2Int` fields; that is why RoomShell uses `cellsX`/`cellsZ`.
10. `create_prefab` into `Assets/_Project/Prefabs/Rooms/<F>/`. Run `Heartbeat.EditorTools.RoomDefinitionRefresher.RefreshAll()` (scratchpad `execute_script`) and fix every reported violation; it writes `RD_<name>` and the `RoomLibrary`.
11. Verify with `capture_scene_object` (top view + eye level from each socket). `save_scene`.

Report: prefab path, size, sockets, spawns, props used, screenshots reviewed.
