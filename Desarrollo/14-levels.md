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
German ambience posters (`POST.AMBIENT_*`, D-58, never translated, `tutorialOnly` off) hang in `ROOM_F1_Hack_A`, `ROOM_F1_Storage_A` (and their F2/F3 variants), `ROOM_F2_Open_B` and `ROOM_F3_Corridor_B`. Vitalis safety posters show the bound key (`IN CASE OF DARKNESS: PRESS [F]`): `PROP_Shared_VitalisPoster` + `TutorialPoster` (POST table, actions as `Map/Action` list), placed by hand in `KEY_F1_CapsuleRow` (interact, terminal), `ROOM_F1_Storage_A` (flashlight, inventory), `ROOM_F1_Corridor_A` (sprint) and `ROOM_F1_Open_A` (stealth, combat). `FloorBuilder` keeps them only in the first-attempt tutorial rooms. Unlit photoluminescent paper so they read in the dark; English in both languages (D-89), German closing line in italics (D-58).

## Intro (`Intro` scene, D-53, D-63)
House → outside Sektor V-7 → hatch (first hack, on the phone) → elevator (cinematic, distorts and falls) → the floor that does not exist → figure in the distance → attack from behind → full reconstruction cinematic. Built with the kit plus `PH_Intro_*`; minimal HUD; pause without Restart; the death does not count.
- Implementation: `IntroDirector` (on `_IntroRoot`) runs House → Exterior → Elevator → NonexistentFloor → Reconstruction and logs `[Heartbeat][Intro] Stage <name>`. Sets are `RoomShell` kit shells placed by hand (built at start with every socket open); the hatches are door blocks slid by the director. The hatch is a `HackPanel` (QuickSeal) whose view is titled as Lukas's phone (`UI.HACK.PHONE_TITLE`, D-63); the elevator panel is `IntroElevatorPanel` (Descend, then Repair after the failure). The attack fires within 2.5 m of `AttackPoint` or 25 s after arrival. The reconstruction is 5 capsule flashes with cuts to black, then The Mother's terminal labels; `CycleController.FinishIntro` registers the damaged voice note (personal effects) and the ring, sets `introDone` and loads floor 1 (first attempt), where Lukas says the ring line. New game / Continue on a slot without `introDone` plays the Intro first.
- shortcut: Lukas wears his maintenance PH (`PH_Lukas_Civil` pending), the revolver and the arm terminal are hidden in the Intro scene; the house is a kit room with primitive furniture.

## The Mother room
Central PA room, "machine only". Mixing tables with 3 hack nodes, archive capsules (4 releases), hostile cameras with blind spots marked for level design, the cryo core with 3 transformer sockets, ammo spawn points (12 at blind spots, 8 at the core).


## Floors 2 and 3 (H5 state)
- Kits `KIT_F2_*` / `KIT_F3_*` are Aura prefab variants of the F1 pieces (same geometry and pivots) with the floor materials (`MAT_F2_Floor/Tile/Trim`, `MAT_F3_Floor/Wall/Trim`), grouped in `KitSet_F2` / `KitSet_F3`. Light templates: `Light_F2_Fluorescent` (cold white), `Light_F3_Crimson`.
- Rooms `ROOM_F2_*`, `KEY_F2_*`, `ROOM_F3_*`, `KEY_F3_*` are **prefab variants of the hand-built F1 layouts**, re-dressed: floor kit set, light tint, F1-specific props swapped for the floor's props at the same footprint (`Prefabs/Props/F2|F3`). F3 rooms carry one extra enemy spawn (placed with Aura) so the core holds its larger enemy budget. All 1 000-seed real-library tests pass for F2/F3 full and showcase.
- Unique layouts built by hand with Aura: `ROOM_F2_Open_B` (clinic waiting room: rows of lab chairs, patient screens, nurse station, bed and IV cart; 5×5, sockets S/N/E) and `ROOM_F3_Corridor_B` (cable gallery: trip-free heavy cabling along both walls, containment console, PA horn; 2×8, sockets S/N/W), `ROOM_F2_Hack_B` (operating theatre: surgery table under the surgical lamp that lights the room, instrument tray, IV cart, sample fridge and filing cabinets; 4×4, sockets S/E/N) and `ROOM_F3_Open_B` (containment room: three broken capsules, cryo tank, transformer, containment console with a document; 5×5, sockets S/N/W), `ROOM_F2_Corridor_B` (patient corridor: beds and IV carts along one wall, patient screen; 2×7), `ROOM_F2_Storage_B` (pharmacy and records: filing-cabinet wall and aisle, sample fridges, desk with a document; 5×5, sockets S/W) and `ROOM_F3_Storage_B` (cryogenic vial store: four cryo tanks, broken capsule, console with a document; 5×5, sockets S/E), `ROOM_F3_Hack_B` (data node: two facing containment consoles, transformers cabled to them, PA horn; 4×4, sockets S/W/N).
- shortcut: F2/F3 reuse the F1 layouts; more floor-specific layouts (catwalks over the core — OBJECT_CATALOG archetype notes) are pending level design (Victor) and drop in as new room prefabs + RoomDefinitions without code changes.
