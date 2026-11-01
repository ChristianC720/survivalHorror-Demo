# Kit, room and socket specification

Applies to every kit piece, room prefab and the runtime `FloorBuilder`. These rules make hand-built pieces line up exactly when the generator joins rooms. **If a piece breaks a rule here, fix the piece, not the generator.**

GDD sources: `GDD/04 - Contenido/02 - Niveles.md` (archetypes, themes), `GDD/05 - Arte y Audio/03 - Entornos.md` (2 m modular kit), `GDD/02 - Diseño/03 - Sistemas.md` S-06.

## 1. Units and grid
| Measure | Value |
|---|---|
| Grid cell | **2 m × 2 m** (XZ) |
| Floor top | y = 0 |
| Wall height | 3.0 m (vertical rooms: 6.0 m, two stacked wall pieces) |
| Wall thickness | 0.3 m, **inset inside the room's own cells** |
| Doorway opening | 2.0 m wide × 2.2 m high (one full cell edge) |
| Lintel | 0.8 m high (y 2.2→3.0) × 0.3 m |
| Player capsule | 0.35 m radius × 1.8 m high (fits 2.0 × 2.2) |
| Prop sub-grid | 0.5 m |

Aura snapping (run before building, in the Sandbox scene): move snap 2 m for kit pieces, 0.5 m for props, rotation snap 90° (props 15°). Use `scene_view_functions` or a scratchpad `execute_script` that sets `EditorSnapSettings.move`, `EditorSnapSettings.rotate`, `EditorSnapSettings.gridSnapEnabled = true`.

Axes: +X east, +Z north, +Y up. All rotations are multiples of 90° around Y for kit pieces.

## 2. Kit pieces
All kit pieces: static, `MeshRenderer` + `BoxCollider`/`MeshCollider`, layer `Environment` (ceilings: `Ceiling`), no scripts. Built from Unity primitives (scaled children) under an empty root whose **pivot is the reference point below**. Children carry the scale; **the root is always scale (1,1,1)**.

| Piece | Name | Root pivot | Local extent at rotation 0 |
|---|---|---|---|
| Floor tile | `KIT_<F>_Floor_2x2` | min corner of the cell, top face at y=0 | x 0→2, z 0→2, y −0.2→0 |
| Ceiling tile | `KIT_<F>_Ceiling_2x2` | min corner, at y=3 | x 0→2, z 0→2, y 3→3.2 · layer `Ceiling` |
| Wall | `KIT_<F>_Wall_2m` | **bottom center of the outer face** (on the room boundary) | x −1→1, y 0→3, z 0→0.3 (local +Z points **into** the room) |
| Wall high | `KIT_<F>_Wall_2m_High` | same | y 0→6 |
| Doorway | `KIT_<F>_Doorway_2m` | same as wall | lintel only: x −1→1, y 2.2→3, z 0→0.3 |
| Window wall | `KIT_<F>_WallWindow_2m` | same as wall | wall with 1.2 × 0.8 opening at y 1.2 (surveillance rooms) |
| Ramp | `KIT_Shared_Ramp_2x4` | min corner, bottom | x 0→2, z 0→4, rises 0→1.5 (≈20.6°) |
| Platform | `KIT_Shared_Platform_2x2` | min corner, at y=1.5 | x 0→2, z 0→2, top at 1.5, rails 1.0 high on open sides |
| Column | `KIT_<F>_Column` | center of cell corner | 0.4 × 3 × 0.4 |

Wall placement inside a room of W × D cells (room origin at its min corner):

| Edge | Position for cell index k | Rotation Y |
|---|---|---|
| South (z = 0) | (2k+1, 0, 0) | 0 |
| North (z = 2D) | (2k+1, 0, 2D) | 180 |
| West (x = 0) | (0, 0, 2k+1) | 90 |
| East (x = 2W) | (2W, 0, 2k+1) | 270 |

Corners: the inset walls overlap in the 0.3 × 0.3 corner square, which is intended (no gap and no corner piece needed). Adjacent rooms therefore have a 0.6 m double wall between them, and doorways are 0.6 m deep.

Each floor theme has its own kit (`F1`, `F2`, `F3`) with **identical geometry** and different materials. Build `F1` first, then duplicate the prefabs (`duplicate_asset`) and swap materials for `F2` and `F3`.

## 3. Room prefabs

**Shell vs design (decision 2026-11-01).** The repetitive shell of a room — floor and ceiling tiles, perimeter walls, and the doorway/wall at each socket — is assembled at runtime by `RoomShell` from the hand-built kit pieces (`KitSet_F<n>`). Everything that makes a room *designed* is placed by hand with Aura in the prefab: sockets, props and cover, interior walls, ramps and platforms, lights, spawns, camera mount. Reason: the shell is identical for every room and costs ~70 tool calls per room by hand; the result is the same geometry and follows the user's instruction to assemble the hand-built pieces with a runtime script. Use the `Preview Shell` context menu on `RoomShell` (or the scratchpad `PreviewShell`) to see the walls while dressing a room; previews are `DontSave`.
Naming: `ROOM_<F>_<Archetype>_<Variant>` (procedural) and `KEY_<F>_<Secondary|Final>_<Variant>`, plus `KEY_F1_CapsuleRow`, `TUT_F1_<1..4>`, `KEY_F3_Mother`.
Archetypes: `Corridor`, `Storage`, `Open`, `Surveillance`, `Hack`, `Vertical`.

Root `ROOM_…` (pivot = room min corner at y=0, scale 1, layer `RoomVolume`) with `RoomInstance`, `RoomShell` (cellsX, cellsZ, kit, tall, windows) and a trigger `BoxCollider` covering the interior. Hierarchy:
```
ROOM_F1_Storage_A            RoomInstance (ref to RoomDefinition), RoomVolume (BoxCollider trigger, layer RoomVolume)
├─ (Shell)                   built at runtime by RoomShell — never saved in the prefab
├─ Geometry                  optional hand-placed interior kit pieces (inner walls, columns, ramps, platforms)
├─ Props                     PROP_/PH_ prefabs on the 0.5 m sub-grid
├─ Lights                    exactly one light tint per room (warm amber F1 / cold F2 / crimson F3)
├─ Sockets                   DoorSocket × n
├─ Spawns                    EnemySpawn, ItemSpawn (tags: Budget, Document, Emilia, Confiscated, Key, Switch), PlayerSpawn
├─ Cover                     empty markers on cover props (AI and sight tests)
└─ CameraMount               (optional) surveillance camera pose + SurveillanceCamera
```
Rules:
- Size in whole cells. Footprint ≤ 8 × 8 cells (16 m) except `KEY_F3_Mother`.
- The `RoomVolume` box covers the interior exactly (inset 0.3) from y 0 to the wall height.
- **Leave socket edges empty** in `Geometry`: the builder fills each socket with a doorway (if connected) or a wall (if not).
- Procedural rooms have ≥ 2 sockets on different edges (corridors: on opposite edges); key rooms (leaves) may have 1. Sockets sit on cells that do not hold walls of the vertical room's upper level.
- Collectibles only stand out under light (art rule): no emissive pickups.
- Enemy spawns ≥ 4 m from every socket (6 m was impossible in rooms with 3–4 sockets). Two enemy spawns max per room (GDD max 2 active per room).
- Room metadata goes in a `RoomDefinition` asset (`ScriptableObjects/Rooms/`): archetype, floor, `RoomRole` (Procedural / Secondary / Final / CapsuleRow / Tutorial / Mother), size in cells, socket list (auto-filled by the `Heartbeat/Rooms/Refresh Definition` editor menu), `hasCamera`, `canBeUnpowered`, prefab reference.

## 4. DoorSocket
- Transform at the **midpoint of the boundary edge**, y = 0, **forward (+Z) pointing out of the room**.
- Fields: `Vector2Int cell`, `Facing facing` (N/E/S/W, derived), `bool allowLocked` (the socket can receive a key door or a hack panel).
- Alignment: to connect room B to room A, the builder rotates B so `B.socket.forward == -A.socket.forward`, then translates B so both socket positions coincide. Because walls are inset, the rooms meet boundary to boundary without overlapping.
- At a connected pair the builder spawns **one** `DoorLeaf` (prefab `Prefabs/Level/Door_2m`) centered on the shared point, plus a doorway lintel in each room. `DoorLeaf` variants: plain, key-locked (mandatory / optional key), hack-sealable, power-locked.
- Unused sockets get `KIT_<F>_Wall_2m`.

## 5. Layout grid at floor level
The generator works on an integer grid in **cells**. A placed room occupies a rectangle of cells after rotation; rooms may never overlap. All room positions are multiples of 2 m, so every piece is on-grid in world space too.

## 6. Layers and tags
Layers: `Environment` (8), `Ceiling` (9, hidden from the surveillance camera), `Player` (10), `Enemy` (11), `Interactable` (12), `RoomVolume` (13, triggers; queries use `QueryTriggerInteraction.Ignore`), `Corpse` (14), `Hitbox` (15).
Tags: only `Player`. Everything else is found through components.

## 7. Checklist before saving a kit piece or room
- [ ] Root scale (1,1,1); pivot exactly as in the tables (check with `get_game_object_info`).
- [ ] All child positions are on the 2 m grid (kit) or the 0.5 m sub-grid (props).
- [ ] Layers set; colliders present on walls, floors and cover props.
- [ ] `capture_scene_object` top view and player-height view taken and reviewed.
- [ ] Room: `RoomDefinition` refreshed; socket count ≥ 2; one light tint.
