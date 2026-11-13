# E-07 Procedural floors (S-06)

**Goal:** each floor and each reconstruction assembles a new, always-valid floor from hand-built room prefabs, in ≤ 5 s including NavMesh.

**GDD sources:** S-06 (`03 - Sistemas.md`), M-15, `04 - Contenido/02 - Niveles.md` (composition, energy D-72, tutorial D-70), `06 - Economía y Recursos.md` (budgets D-17, D-37, D-76, D-79). Geometry rules: `KIT_SPEC.md`.

## Deliverables
`Scripts/Generation/`: `FloorConfig.cs` SO (+ `Floors/FC_F1|F2|F3_Full.asset`, `_Showcase.asset`), `RoomDefinition.cs` SO, `RoomLibrary.cs` SO (all definitions), `FloorLayout.cs` (pure: graph + cell occupancy), `LayoutGenerator.cs` (pure, seeded `System.Random`), `LayoutValidator.cs` (pure), `BudgetDistributor.cs` (pure), `FloorBuilder.cs` (MonoBehaviour: instantiation, sockets, doors, walls, spawns, NavMesh), `RoomActivation.cs`, `RoomInstance.cs`, `DoorSocket.cs`. Editor: `Scripts/Editor/RoomDefinitionRefresher.cs` (menu `Heartbeat/Rooms/Refresh Definition`: reads the prefab's sockets, size and spawns into its definition; validates KIT_SPEC rules and reports violations).

## Algorithm
1. **Seed:** from the save (attempt seed per floor). Logged: `[Heartbeat][Gen] floor=1 seed=12345`.
2. **Composition** (full): 8–12 procedural + 2–3 secondary key + 1 final key. Showcase: 6–8 + 2 + 1. ~1/3 of procedural rooms are flagged *empty* (no enemies). Floor 1 always starts with `KEY_F1_CapsuleRow`. First attempt of floor 1: the 4 tutorial rooms are fixed right after the capsule row (D-70).
3. **Growth:** start from the entry room; repeatedly pick an open socket, pick a compatible room definition (weighted, archetype variety, no same archetype three times in a row), try the 4 rotations, place it if its cell rectangle does not overlap. Key rooms are placed at graph depth ≥ 2 (secondaries) and at maximum depth (final). Optional loops: connect two open sockets that face each other on adjacent cells.
4. **Validation** (`LayoutValidator`): a valid path entry → final exists; the mandatory key sits in a secondary key room reachable without passing through the final door; no guaranteed minimum resource behind an optional-key door; power rule (~1/3 of floors 1–2 start with the final room unpowered, never in the first attempt of floor 1 or on floor 3; the switch goes into a secondary key room reachable first); every room reachable; Emilia documents: 2 per floor in secondary key rooms. Invalid → reseed deterministically (`seed+1`), maximum 20 tries, then error.
5. **Budget** (`BudgetDistributor`): per-floor resource ranges (heals, ammo, spikes, enemies) from `FloorConfig`, Story mode +1 to each minimum, scattered to `ItemSpawn`/`EnemySpawn` markers by tag; max 2 enemies per room; documents 1–2 and confiscated objects 1–2 per attempt preferring unregistered ones; an already-registered Emilia document becomes +1 of the scarcest resource.
6. **Build** (`FloorBuilder`): instantiate rooms, fill sockets (doorway + `DoorLeaf` variant or wall), spawn items and enemies, then `NavMeshSurface.BuildNavMesh()` on a single surface (collect objects = children, layer `Environment`). Raise `FloorBuilt`.
7. **Activation:** only the current room and its neighbours have active enemies, lights and props renderers (lights ≤ 16, ≤ 4 shadowed).

## Tasks
`E07-layout-graph` · `E07-validator` · `E07-builder` · `E07-navmesh-activation` · `E07-budget` · `E07-story-mode-showcase`.

## Tests (EditMode)
- `Generator_1000Seeds_AllValid` for each floor config (full and showcase): 1 000 seeds pass the validator with no exception.
- `SameSeed_SameLayout` (determinism).
- Budget totals stay within range for 1 000 seeds.
- PlayMode/Aura: build floor 1, measure the time → log `[Heartbeat][Gen] built in X ms` (≤ 5 000 ms).
- Showcase (fairs/demos): launch the build with `-showcase`; new slots then use the `FC_F*_Showcase` configs (`SlotData.showcase`). In the editor, `Heartbeat ▸ Launch Arguments ▸ -showcase` (and `-telemetry`) simulates the switch until the editor closes; Play in `Floor.unity` then builds the first floor with the Showcase config too.
