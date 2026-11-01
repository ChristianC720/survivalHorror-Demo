# E-14 Levels: kits, rooms, tutorial, intro, Mother room

**Goal:** all the hand-built geometry the generator needs, assembled **by hand with Aura** on the 2 m grid with exact pivots (`KIT_SPEC.md`), dressed with props from `OBJECT_CATALOG.md`.

**GDD sources:** `04 - Contenido/02 - Niveles.md`, `01 - Estructura del Contenido.md`, `05 - Arte y Audio/03 - Entornos.md`, `03 - Historia y Mundo/02 - Mundo.md` (setting), D-33, D-53, D-63, D-69, D-70, D-72.

## Workflow (Aura skills: `/build-kit-piece`, `/build-room-prefab`, `/place-placeholder-prop`)
1. Open `Sandbox_Kit` (never in the build). Set snapping (KIT_SPEC §1).
2. Build each kit piece under an empty root at the origin: `create_game_object` (primitive children) → `set_transform` with exact values → `create_pbr_material` / `assign_material` → `set_layer` → `create_prefab` into `Prefabs/Kit/<F>/`. Verify with `get_game_object_info` and `capture_scene_object`.
3. Build each room in the sandbox at the origin: place kit prefabs (`place_asset_in_scene` / `set_transform` using the wall placement table), leave socket edges empty, add `Sockets/DoorSocket` empties, `Spawns`, `Lights`, `CameraMount`, `RoomVolume` → `create_prefab` into `Prefabs/Rooms/<F>/` → run `Heartbeat/Rooms/Refresh Definition` → fix reported violations.
4. Capture a top view and a player-height view of every room before committing.

## Room list
| Floor | Procedural (6 archetypes, theme per `Niveles`) | Key rooms |
|---|---|---|
| F1 Maintenance | `ROOM_F1_Corridor_A` (pipe tunnel), `ROOM_F1_Storage_A` (tool depot), `ROOM_F1_Open_A` (boiler room), `ROOM_F1_Surveillance_A` (Kessler guard post), `ROOM_F1_Hack_A` (electrical panel), `ROOM_F1_Vertical_A` (ventilation ducts) | `KEY_F1_CapsuleRow`, `KEY_F1_Secondary_A/B/C`, `KEY_F1_Final` (stairs/elevator), `TUT_F1_1..4` |
| F2 Clinic | Corridor (patient corridor), Storage (pharmacy & records), Open (waiting room), Surveillance (nurse control), Hack (operating theatre lock), Vertical (lift shaft) | `KEY_F2_Secondary_A/B/C`, `KEY_F2_Final` (impossible photo) |
| F3 Core | Corridor (cable gallery), Storage (vial store), Open (containment room), Surveillance (monitor room), Hack (data node), Vertical (catwalks over the core) | `KEY_F3_Secondary_A/B/C`, `KEY_F3_MotherDoor`, `KEY_F3_Mother` |
Add `_B` variants of the procedural rooms if time allows. More variety per archetype beats more archetypes.

## Tutorial (first attempt only, D-70)
1. Capsule row: interact, open the terminal; ring on the floor (after the full cinematic); empty personal tray.
2. Dark depot (Storage): flashlight, pickup, inventory; dead Kessler guard with the revolver; 1 ammo blister + 1 heal.
3. Long tunnel (Corridor): running and stamina; a Rastrero is heard behind a door at the end.
4. Boiler room (Open): stealth, shove, shooting; 1 real Rastrero (counts toward the floor's enemies; its kill of Lukas does not add to the counter).
Vitalis safety posters show the bound key (`IN CASE OF DARKNESS: PRESS [F]`).

## Intro (`Intro` scene, D-53, D-63)
House → outside Sektor V-7 → hatch (first hack, on the phone) → elevator (cinematic, distorts and falls) → the floor that does not exist → figure in the distance → attack from behind → full reconstruction cinematic. Built with the kit plus `PH_Intro_*`; minimal HUD; pause without Restart; the death does not count.

## The Mother room
Central PA room, "machine only". Mixing tables with 3 hack nodes, archive capsules (4 releases), hostile cameras with blind spots marked for level design, the cryo core with 3 transformer sockets, ammo spawn points (12 at blind spots, 8 at the core).
