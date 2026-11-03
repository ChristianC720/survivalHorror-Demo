# Blender model production plan

Plan for replacing the `PH_` placeholders with real models, closed with the project owner on 2026-11-02. Sources: `GDD/05 - Arte y Audio/02 - Personajes y Animación.md`, `GDD/03 - Historia y Mundo/03 - Personajes.md`, `GDD/04 - Contenido/04 - Objetos y Coleccionables.md`, `OBJECT_CATALOG.md`, `PLACEHOLDERS.md`, decisions D-151 and D-152. Where they disagree, the GDD wins.

The reference sheets in `Blender3DAssets/Image_References/` were generated with a free image model and their captions are unreliable. They are a loose guide; the GDD, the game's style and this plan win over them.

## 1. Specification

| Topic | Rule |
|---|---|
| Scale | 1 unit = 1 m; Lukas 1.78 m |
| Pivot | Feet / floor center, scale (1,1,1) |
| Skeleton | One shared Unity Humanoid hierarchy (`PJ_Rig` bone names plus fingers: thumb, index, one bone for the other three) for Lukas, Alto, Boca, Mudo. Extra bones only where needed (Boca jaw lobes), with their clips kept separate. Rastrero and The Mother have their own |
| Triangles | Lukas ≤8k (no minimum) · enemies ~4–8k · The Mother ~15k · corpses ~2–4k · machines ≤2k · revolver ≤1.5k |
| Textures | Characters 512 px · The Mother 1024 px · props 256 px · key items 512 px; point filter, no mipmaps |
| Style | Flat shading, one material per model, painted texture with dark edge lines, no PBR maps. Everything follows Lukas's look |
| Clips | 30 fps, in place (no root motion), stride matched to `PlayerBalance`: walk 1.5, run 4, crouch 0.8 m/s; ×0.85 injured, ×0.70 critical |
| Animation source | Everything in Blender; no Mixamo (D-151) |
| Format | FBX from Blender into `Assets/_Project/Models/<Group>/`; `.blend` sources stay in `Blender3DAssets/` (Git LFS) |
| Names | Models `CH_<Character>_<Variant>`, clips `AN_<Character>_<Action>` |
| Swap | Replace the children of `PH_<Name>/Mesh`; keep the root, pivot, colliders and scripts |

## 2. Method per asset

| Asset | Method |
|---|---|
| The 4 enemies | Planned: Aura generation. **Actual: Blender by code** (`Scripts/hb_monster.py`, `build_enemy.py`, `enemy_clips.py`), because Aura answered that 3D generation is not in the current plan and the image subscription has expired |
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
| Alto | ~2.5 m, hunched, arms to the floor, Werkschutz vest and torn straps, iridescent purple on bare skin |
| Boca | Two-lobed split head, more body mass than the sheet; jaw opens with own bones |
| Rastrero | Always on four legs, head turned 180° with chin up, horizontal toothed mouth, no vertical mouth |
| Mudo | Faceless, grey Vitalis staff clothing, reads as a corpse or a still person |
| All normals | No plates or riveted metal; bone spines and thin cables allowed. Dark palette blood (`#270a16`, `#51111f`), muted skin, fewer stains |
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
- **Enemy look.** Built by code, so they are simpler than the reference sheets: smooth limbs, paint per face instead of painted detail, little facial detail. Silhouette, height and clips follow the GDD. Worth a look in-game.
- **Enemy rig type.** They use Unity's Generic rig, not Humanoid: each has its clips on its own proportions, and the crawl exceeds the Humanoid muscle limits. The bone hierarchy is still the shared one.
- **Hand-made clips.** All clips are generated by code from key poses. Review them in `Blender3DAssets/Review` and in play; anything stiff can be retouched in the scripts.
- **Not placed in rooms.** New prefabs with no room yet: `PH_F1_Forklift`, `PH_F1_GantryCrane`, `PH_F1_WaterPump`, `PH_F2_MonsterCapsule`, `PH_F2_ResonanceCapsule`, `PH_Shared_Corpse`, `PH_Shared_Corpse_Subject`, `PROP_F3_MixingTable` (the boss room nodes use its model directly). Room layouts are hand-made work, so they were left alone.
- **Lukas civil.** Recoloured from the R-H-07 mesh: tubes and terminal removed, but the belt, wrist bands and knee pads remain, so it is not yet the field jacket of the reference.
- **Play coverage.** Tested: Lukas's movement, crouch, aim and critical gait; enemies' Idle, Scream and recycled swap; a floor loading without errors. Not played: the intro, the boss fight, the endings, body variants across real deaths. Those were checked by renders or not at all.
- **Release tags.** `v1.1-demo` and later are not tagged, because no build was requested with these models.
