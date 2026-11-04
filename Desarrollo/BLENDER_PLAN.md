# Blender model production plan

Plan for replacing the `PH_` placeholders with real models, closed with the project owner on 2026-11-02. Sources: `GDD/05 - Arte y Audio/02 - Personajes y Animación.md`, `GDD/03 - Historia y Mundo/03 - Personajes.md`, `GDD/04 - Contenido/04 - Objetos y Coleccionables.md`, `OBJECT_CATALOG.md`, `PLACEHOLDERS.md`, decisions D-151 and D-152. Where they disagree, the GDD wins.

The reference sheets in `Blender3DAssets/Image_References/` were generated with a free image model and their captions are unreliable. The owner made them the guide for the enemies' forms (D-154, section 8); for everything else they stay a loose guide.

## 1. Specification

| Topic | Rule |
|---|---|
| Scale | 1 unit = 1 m; Lukas 1.78 m |
| Pivot | Feet / floor center, scale (1,1,1) |
| Skeleton | One shared Unity Humanoid hierarchy (`PJ_Rig` bone names plus fingers: thumb, index, one bone for the other three) for Lukas, Alto, Boca, Mudo. Extra bones only where needed (Boca jaw lobes), with their clips kept separate. Rastrero and The Mother have their own |
| Triangles | Lukas ≤8k (no minimum) · enemies ~11k (D-154) · The Mother ~15k · corpses ~3.5k · machines ≤2k · revolver ≤1.5k |
| Textures | Enemies and corpses 1024 px · Lukas 512 px · hard-surface models share a 2048 px atlas (The Mother 768 px, machines 384 px, items 128–256 px of it); point filter, no mipmaps |
| Style | Flat shading (enemies: smooth skin, flat hard parts), one material per model, painted texture with dark edge lines, no PBR maps. Everything follows Lukas's look and the Signalis method (section 7) |
| Clips | 30 fps, in place (no root motion), stride matched to `PlayerBalance`: walk 1.5, run 4, crouch 0.8 m/s; ×0.85 injured, ×0.70 critical |
| Animation source | Everything in Blender; no Mixamo (D-151) |
| Format | FBX from Blender into `Assets/_Project/Models/<Group>/`; `.blend` sources stay in `Blender3DAssets/` (Git LFS) |
| Names | Models `CH_<Character>_<Variant>`, clips `AN_<Character>_<Action>` |
| Swap | Replace the children of `PH_<Name>/Mesh`; keep the root, pivot, colliders and scripts |

## 2. Method per asset

| Asset | Method |
|---|---|
| The 4 enemies | Planned: Aura generation. **Actual: Blender by code** (`Scripts/hb_sculpt.py`, `hb_anatomy.py`, `enemy_sculpt.py`, `hb_monster.py`, `build_enemy.py`, `enemy_clips.py`; section 8), because Aura answered that 3D generation is not in the current plan and the image subscription has expired |
| Organic objects (plush rabbit) | Planned: Aura generation. Same limit: Blender by code |
| The Mother, machines, revolver, rigid items | Blender by code, exact measures |
| Corpses | Derived from Lukas's or the Mudo's body, with clothing per group |
| Out of scope | `KIT_*`, `PROP_*` and the intro sets stay primitive, except `PROP_F3_MixingTable` |

**Review.** The owner is absent during modeling and delegated the approval gates. Each gate gets an objective check (tris, height, weights, in-game capture) and evidence in `Blender3DAssets/Review/<model>/` (not versioned): 4 views, in-game capture, short clip previews. The owner reviews at the end and marks anything to redo.

## 3. Approved look

| Character | Decision |
|---|---|
| Lukas | Keep the model. Add merged low-poly curtain hair, round the top of the head, darken the boots with red edge, add a ring on the right hand. Keep the terminal on the outer forearm. Seams and overalls per GDD teal. Keep 512 px; thicken details only if the in-game test demands it |
| Light | 2 teal point lights (chest, forearm), 0.5 m range, no shadows, no gameplay effect, values in `PlayerBalance`. Not on civil Lukas |
| Alto | ~2.5 m, hunched, arms to the floor, torn Werkschutz vest and trousers, pale skin with purple only in its shadows, spines and cables |
| Boca | Two-lobed head split to the chest, deep crouch, spines, back plate and cables; lobes open with own bones |
| Rastrero | Always on four legs, bald eyeless head, huge hanging vertical jaw, striped rags and torn dark trousers, spines, back plate and cables |
| Mudo | Faceless, grey Vitalis staff clothing, reads as a corpse or a still person |
| All normals | Metal only where the sheet shows it (D-154); bone spines and thin cables allowed. Dark palette blood (`#270a16`, `#51111f`), muted skin, fewer stains |
| Recycled | Complete model per type on the same skeleton, swapped when the body rises (replaces `RecycleSystem.AddMetalPieces`). Metal comes from the laboratory |
| Body variants (D-43) | 4 cumulative: right forearm → neck and jaw patch → left hand → ring hand assimilated. Texture plus a slight shape change on one mesh |

## 4. Phases and status

Each model has its own branch (`E16-<model>`).

### v1.1-demo: Lukas and the 4 normal enemies
- [x] **Phase 0, pipeline:** export current Lukas, import as Humanoid, swap into `PH_Lukas`, test textures at 512 px. Record the export settings below.
- [x] **Phase 1, Lukas model:** hair, head, boots, ring, finger bones, teal seams, file cleanup, rename to `CH_Lukas_RH07`.
- [x] **Phase 2, Lukas locomotion:** derived from the existing clips: idle, walk (fine, injured, critical), run, crouch idle and walk, aim walk, turns.
- [x] **Phase 3, enemies:** Alto, Mudo, Boca, Rastrero with their clips (table below). Built by code: see section 2 and section 6.
- [x] **Phase 4, Lukas actions:** interaction (door, pickups, panel, terminal, hack, heal, examine, power switch), combat (aim, fire, reload, shove, ground strike, finisher), grabs (Alto, Mudo, The Mother's arm, spike release), hit and deaths.
- [x] **Unity:** Animators, teal lights, arm terminal screen, bone noise on the assimilated.
- [ ] **Unity, not done:** foot and hand IK (`Animation Rigging`), procedural Rastrero legs, enemy ragdolls. Bodies use a death clip.

| Enemy | Clips |
|---|---|
| Alto | ~8–10: patrol, hunt, long strike, grab, stunned, death |
| Mudo | ~6–8: still, burst, grab, pause, death |
| Boca | ~8–10: wander, hunt, bite, scream, stunned, death |
| Rastrero | ~6–8: base cycle for procedural IK, flank, claw, death |

### v1.2-demo: The Mother, recycled and civil
- [x] `CH_TheMother` with collector arm and hostile camera; `PROP_F3_MixingTable`; transformer. Static: no crane patrol or core-opening clips yet.
- [x] Recycled models (4), a second mesh on each enemy's rig, switched by `EnemyAnimator`.
- [x] `CH_Lukas_Civil` (shares Lukas's clips; no phone-hack or scripted-death clip yet), body variants (4, texture only, no shape change), `CH_Lukas_Mudo`, corpses.

### v1.3-demo: machines and objects
- [x] F1: Generator, Boiler, Forklift, GantryCrane, WaterPump, reconstruction capsule.
- [x] F2: HumanCapsule, MonsterCapsule, ResonanceCapsule. F3: CryoTank, BrokenCapsule.
- [x] Revolver and items: heal, ammo, spike, 2 keys, ring, document.
- [x] 12 confiscated objects, each on its `DocumentDefinition.model`.

## 5. Export settings

`Blender3DAssets/Scripts/export_fbx.py` is the only export path: selected rig and mesh, forward -Z, up Y, FBX scale all, no leaf bones, deform bones only, every `AN_` action baked at 30 fps, face smoothing.

- Unity builds the imported hierarchy from the first take at t=0, so the exporter adds a rest-pose take (`AN_000_Bind`) that sorts first.
- `ModelImportRules.ConfigureHumanoid(path)` maps the bones Unity's auto-mapping skips (Chest, fingers) and stores a T-pose, because the rest pose has the arms down. Re-run after adding bones.
- `ModelImportRules` also sets the 512 px point-filter import and in-place clips. Models face +Z in Unity.
- Clip stride is matched by the blend tree `timeScale` in `AnimatorBuilder`.
- Lukas's clips are generated by `Blender3DAssets/Scripts/lukas_clips.py` from the original `src_*` actions.

## 6. Open items for the owner

- **Aura plan.** 3D generation is not included in the plan and the image subscription has expired. If it is renewed, the enemies can be regenerated from `Image_References` and re-bound to the same skeleton, without touching the Animators.
- **Enemy look.** Sculpted by code after the sheets (section 8), so anatomy is coarser than the painted sheets. Height and skeleton are unchanged.
- **Enemy rig type.** They use Unity's Generic rig, not Humanoid: each has its clips on its own proportions, and the crawl exceeds the Humanoid muscle limits. The bone hierarchy is still the shared one.
- **Hand-made clips.** All clips are generated by code from key poses. Review them in `Blender3DAssets/Review` and in play; anything stiff can be retouched in the scripts.
- **Not placed in rooms.** New prefabs with no room yet: `PH_F1_Forklift`, `PH_F1_GantryCrane`, `PH_F1_WaterPump`, `PH_F2_MonsterCapsule`, `PH_F2_ResonanceCapsule`, `PH_Shared_Corpse`, `PH_Shared_Corpse_Subject`, `PROP_F3_MixingTable` (the boss room nodes use its model directly). Room layouts are hand-made work, so they were left alone.
- **Lukas civil.** Recoloured from the R-H-07 mesh: tubes and terminal removed, but the belt, wrist bands and knee pads remain, so it is not yet the field jacket of the reference.
- **Play coverage.** Tested: Lukas's movement, crouch, aim and critical gait; enemies' Idle, Scream and recycled swap; a floor loading without errors. Not played: the intro, the boss fight, the endings, body variants across real deaths. Those were checked by renders or not at all.
- **Release tags.** `v1.1-demo` and later are not tagged, because no build was requested with these models.

## 7. Signalis pass (2026-11-03)

The owner named *Signalis* (rose-engine, 2022) the main visual inspiration. GDD limits still apply: `Referencias e Inspiración` rules out copying its lore, communist aesthetic, concrete palette and characters, so this pass copies the method, not the content.

**Sources.** The 11 official Steam screenshots, studied directly (not stored in the repo). The fan wiki says the game moved from pixel art to 3D models rendered at low resolution in 2017. No source gave polygon counts, texture sizes or the render pipeline, so the points below are readings of those screenshots, not developer facts.

| What the screenshots show | What we do with it |
|---|---|
| Low polygons carry graphic design: stencils, hazard bands, data-matrix labels, numbered panels | Every machine and item has its own stencils, bands and labels, in German or as codes (D-58, D-62), never needed to play |
| Clean flat paint with hard shapes; wear only in a few spots | Small colour ramps per hue, no fine noise; chips only on edges |
| Dark bodies with one saturated accent, strong value contrast | One accent per uniform (crimson sleeve band, service stripe); pale skin against dark cloth |
| Blood is dark, in a few large shapes with thin runs | `hp.blood`: few blotches, with runs that follow gravity |
| Lit tiles and small screens are the only saturated light on a machine | Lamp fields and CRT panels, drawn into the emission map |
| Low internal resolution with big visible pixels | Already in the game (D-81, ~540p); textures keep point filter |

**Method.** `Scripts/hb_paint.py` gives each polygon an island at one density and returns every texel's 3D position, normal, part and edge distance, so paint works in model space and crosses islands with no cut.
- Enemies, recycled and statics: `Scripts/enemy_paint.py`, one 512 px texture per mesh.
- The Mother, machines, items, confiscated objects: `Scripts/hb_props.py`, one 2048 px atlas `T_Palette` (one material, still batches), plus `T_Palette_Emission`. Replaces the old 256 px palette.
- A stencil never goes under one texel per font pixel, so a word on a machine (about 85 px/m) must be short or it overflows its plate.
- `Scripts/review.py` renders the sheets into `Review/`, neutral and under a flashlight.

**Not in this pass.** `CH_Lukas_RH07` keeps its hand-painted texture, which already fits this look. `KIT_*`, `PROP_*` and the intro sets stay primitive (section 2).

## 8. Enemies after their reference sheets (2026-11-03)

The owner rejected the first models: low quality, flat faces, parts showing through. Two causes: URP culls back faces while the hand-built heads and cones had mixed winding (Blender renders hid it), and the bodies were Skin-modifier tubes with no anatomy.

**Method.** `hb_sculpt.py` sculpts a signed distance volume (5 mm voxels) from anatomy primitives in `hb_anatomy.py` and per-enemy forms in `enemy_sculpt.py`, then extracts one closed surface with surface nets.
- Clothing is a thickness added to the volume where a mask holds, so holes and hems are real and nothing can show through. Teeth, claws, spines, cables and metal stay hand-built and are re-wound outward on assembly (`hm.assemble`).
- Decimation has a budget per region: 30% head, 20% hands and feet, 50% the rest. Blender's weighted decimate collapsed thin limbs into spikes, so it is not used.
- The full volume is kept for the 1024 px bake: each texel gets the sculpt's normal, occlusion and crease lines (`hs.bake_fields`), so the anatomy survives on ~11k triangles in any light.
- The bone tables are unchanged, so clips, prefabs and avatars keep working. Postures stay in the clips. Boca's crouch and the Rastrero's arms and feet were changed in `enemy_clips.py`.

**Check.** Unity captures with real culling, front and back, normal and recycled; idle clips sampled with `AnimationMode`, reading bone heights.

## 9. The team's delivery (2026-11-03, D-155)

`Assets/_Project/Models/ModelsNew` holds the modeler's files. Do not edit them: `Scripts/team_models.py` opens them, never saves, and exports to the FBX files the prefabs already use. It replaces section 8 for Lukas, the 4 enemies and the items it lists.

| Delivered | Becomes | Added so it runs |
|---|---|---|
| Lukas (`Player.blend`) | `CH_Lukas_RH07` | 21 action clips from the previous rig (rest poses match); grip markers on the right hand; civil mesh and 4 body-variant textures |
| Monster1 | `CH_Mudo`, `CH_Mudo_Still` | Rig, weights, clips |
| Monster2 | `CH_Boca` | Idle clip |
| Monster3 | `CH_Alto` | Idle clip |
| Monster4 | `CH_Rastrero` | One mesh from its parts, UVs, 2048 px paint from its sheet, rig, weights, clips |
| `Pistola` | `CH_Revolver` | New revolver body on the delivered grip, trigger and guard |
| Other props | `CH_Item_*` | `Aturdidor` is exported but has no use in the GDD yet |

Every enemy also gets a `_Recycled` mesh on the delivered body (D-130): metal pieces on a second material slot, except the Boca, which only gets thinner.

Not delivered, so the code-built models stay: The Mother, machines, capsules, confiscated objects, ring, document, corpses, `CH_Lukas_Mudo`.

## 10. Environment by floor (2026-11-04, D-156)

The owner asked for identity per floor and no Unity primitives. `Scripts/env_models.py` builds 101 models; `EnvWire` (`Heartbeat ▸ Models ▸ Wire Environment Models`) swaps them in and keeps pivots, colliders, layers and scripts.

| Rule | How |
|---|---|
| One draw setup per floor | Atlas `T_Env_<floor>` (2048 px) plus emission, material `M_Env_<floor>` |
| No light leaks | Closed `core` block past each kit piece's edges; door frames close the leaf gap; `audit()` reports open edges and degenerate faces on every export (currently 0) |
| No texture waste | Faces nobody sees use `hidden`, which samples one dark texel |
| Lamps | No shadow casting, so a fixture never blocks its own light |
| Budgets | Kit up to 400 triangles (they repeat by the dozen); largest prop 946 |
| NavMesh | `FloorBuilder` bakes from render meshes, so Env, Props and Statics import as readable; players would fail otherwise |

Measured in `Floor` on floor 1: 71.6k triangles per frame, 18 set-pass calls, 42 shadow casters, 120 fps in the editor. 12 generated floors: 0 failures, 0 agents off the mesh.

Enemies: the delivered body plus a clothing shell in `team_models.add_clothing`, on the same weights, second material slot. Recycled forms are pending. Open questions for the owner: `DUDAS_MODELOS.md`.

## 11. Owner review (2026-11-04, D-157)

| Topic | What exists now |
|---|---|
| One `.blend` per model | `Scripts/blend_sweep.py` re-imports each game FBX into `Blender3DAssets/Blends/<same folders as Models>/`. Textures are linked by relative path. Run it after any export. A builder run overwrites hand edits on its FBX, so a change meant to last goes in the builder |
| Black triangles on walls | F2 and F3 kit prefabs still held F1's model, coplanar with their own. `EnvWire.DropOtherModels` removes it; `PrefabOverlapTests` fails if two renderers share bounds in a prefab or a floor shows another floor's model |
| Light through walls | `RoomActivation`: a light past the shadow budget stays off instead of lighting without shadows |
| Black upper walls | Ceiling lamps are 165/100 degree spots at lower intensity; ambient raised to (.05, .043, .048). Floors and ceilings cast no shadow |
| Room dressing | `RoomDress` (`Heartbeat/Rooms/Dress Rooms`): authored coordinates per layout, in a `Dress_F#` child. It reports solid props on spawns and sockets |
| More rooms | `FloorRedress.ExecuteF1` re-dresses the F2 B layouts as `ROOM_F1_*_B` |
| Door | `DoorLeaf.sideSlide` and `secondLeaf`; `Door_F2` has two leaves |
| Alto | `CH_Alto` has the improved clothing; `CH_Alto_V1` keeps the earlier one, visible in `Sandbox_Gallery` under Modelos sueltos |

Measured in `Floor`, floor 1, with three shadowed lights: 172k triangles per frame, 25 set-pass calls, 200 shadow casters, about 108 fps in the editor (was 71.6k, 18, 42 and 120 with one shadowed light).
