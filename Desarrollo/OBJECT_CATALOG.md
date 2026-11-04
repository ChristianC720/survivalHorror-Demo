# Object catalog (props per floor)

Source: `GDD/00 - Ideas de objetos por sala.md` (revised 2026-10-31), cross-checked with `GDD/04 - Contenido/02 - Niveles.md` (archetype themes) and `GDD/05 - Arte y Audio/03 - Entornos.md`.

Legend:
- **Kind**: naming only since D-156: both `PROP` and `PH` are Blender models (`Blender3DAssets/Scripts/env_models.py`).
- **Cover**: the prop blocks enemy line of sight and the player can crouch behind it (gets a `Cover` marker and a collider ≥ 1.0 m high).
- **Archetypes**: where the room builder should prefer it (Cor = Corridor, Sto = Storage, Opn = Open, Sur = Surveillance, Hck = Hack, Ver = Vertical, Key = key rooms).
- Sizes are W × H × D in metres, pivot at the floor center of the footprint, on the 0.5 m sub-grid.

## Shared (all floors) — `Prefabs/Props/Shared/`
| Prefab | Kind | Size | Cover | Archetypes | Notes |
|---|---|---|---|---|---|
| `PROP_Shared_Speaker` | PROP | 0.4 × 0.5 × 0.3 | – | all (wall, y 2.6) | PA horn. Also the audio emitter of The Mother's lines |
| `PROP_Shared_CableBundle` | PROP | 2.0 × 0.15 × 0.15 | – | Cor, Ver | Runs along walls/ceiling |
| `PROP_Shared_Pipe_2m` | PROP | 2.0 × 0.3 × 0.3 | – | Cor, Ver | Straight; plus `_Elbow` |
| `PH_Shared_Corpse` | PH | 0.6 × 0.3 × 1.8 | – | Sto, Opn | Dead assimilated, clothed, no gore (D-85). Static décor, never a real enemy corpse |
| `PROP_Shared_TrashPile` | PROP | 1.0 × 0.4 × 1.0 | – | Cor, Sto | Low, does not block sight |
| `PROP_Shared_MetalCrate` | PROP | 1.0 × 1.0 × 1.0 | ✔ | Sto, Opn | Stackable (0.5 m variant too) |
| `PROP_Shared_VitalisPoster` | PROP | 0.8 × 1.1 × 0.02 | – | all, TUT | Quad with text; tutorial posters show the bound key |
| `PROP_Shared_Grate` | PROP | 1.0 × 0.02 × 1.0 | – | Cor, Ver | Floor or wall vent |
| `PROP_Shared_Puddle` | PROP | 1.5 × 0.01 × 1.0 | – | all | Decal-like quad; footstep surface `Wet` |

## Floor 1 — Maintenance (sodium amber) — `Prefabs/Props/F1/`
| Prefab | Kind | Size | Cover | Archetypes |
|---|---|---|---|---|
| `PH_F1_Generator` | PH | 2.0 × 2.0 × 1.5 | ✔ | Opn, Hck (power switch room) |
| `PH_F1_Boiler` | PH | 1.5 × 3.0 × 1.5 | ✔ | Opn (boiler room) |
| `PROP_F1_WaterTank` | PROP | Ø 2.0 × 2.5 | ✔ | Opn, Sto |
| `PH_F1_Forklift` | PH | 1.2 × 2.2 × 2.5 | ✔ | Sto, Opn |
| `PH_F1_GantryCrane` | PH | 6.0 × 0.6 × 0.6 (at y 4.5) | – | Ver, Opn (6 m rooms only) |
| `PH_F1_WaterPump` | PH | 1.0 × 1.0 × 1.5 | ✔ | Cor, Opn |
| `PROP_F1_PressureValve` | PROP | 0.4 × 0.4 × 0.2 | – | Cor, Hck |
| `PROP_F1_ThickPipe` | PROP | 2.0 × 0.8 × 0.8 | ✔ (low) | Cor (pipe tunnel), Ver |
| `PROP_F1_Workbench` | PROP | 2.0 × 0.9 × 0.8 | ✔ (crouched) | Sto, Sur |
| `PROP_F1_Tools` | PROP | 0.6 × 0.1 × 0.3 | – | Sto (on benches) |
| `PROP_F1_Gauge` | PROP | 0.3 × 0.3 × 0.1 | – | Hck, Sur (water / pressure / power variants) |
| `PROP_F1_BigContainer` | PROP | 2.0 × 2.5 × 6.0 | ✔ | Sto, Opn |
| `PROP_F1_ToolShelf` | PROP | 0.6 × 2.2 × 2.0 | ✔ | Sto (tool depot) |

## Floor 2 — Illegal clinic (cold fluorescent) — `Prefabs/Props/F2/`
| Prefab | Kind | Size | Cover | Archetypes |
|---|---|---|---|---|
| `PROP_F2_LabChair` | PROP | 0.6 × 1.0 × 0.6 | – | Sur, Opn |
| `PH_F2_HumanCapsule` | PH | 1.0 × 2.2 × 1.0 | ✔ | Opn, Key |
| `PH_F2_MonsterCapsule` | PH | 1.4 × 2.6 × 1.4 | ✔ | Opn, Key |
| `PH_F2_ResonanceCapsule` | PH | 1.2 × 1.4 × 2.6 | ✔ | Opn |
| `PROP_F2_Desk` | PROP | 1.6 × 0.8 × 0.8 | ✔ (crouched) | Sur, Sto (records) |
| `PROP_F2_Console` | PROP | 1.0 × 1.2 × 0.6 | ✔ (crouched) | Sur (nurse station), Hck |
| `PROP_F2_ResearchBed` | PROP | 1.0 × 0.8 × 2.0 | – | Opn (waiting room), Cor |
| `PROP_F2_LabCoatRack` | PROP | 0.5 × 1.8 × 0.5 | – | Sto, Cor |
| `PROP_F2_SurgeryTable` | PROP | 0.9 × 0.9 × 2.0 | – | Hck (operating theatre lock) |
| `PROP_F2_SurgicalTools` | PROP | 0.6 × 0.1 × 0.4 | – | Hck (on trays) |
| `PROP_F2_SurgicalLamp` | PROP | Ø 0.8 (at y 2.4) | – | Hck — also the room's light source |
| `PROP_F2_Screen` | PROP | 1.8 × 1.8 × 0.05 | ✔ (sight only) | Opn, Cor (patient screens) |
| `PROP_F2_IVCart` | PROP | 0.5 × 1.8 × 0.5 | – | Cor |
| `PROP_F2_FilingCabinet` | PROP | 0.5 × 1.4 × 0.6 | ✔ | Sto (pharmacy & records) |
| `PROP_F2_SampleFridge` | PROP | 0.8 × 2.0 × 0.7 | ✔ | Sto |

## Floor 3 — Core (crimson) — `Prefabs/Props/F3/`
| Prefab | Kind | Size | Cover | Archetypes |
|---|---|---|---|---|
| `PH_F3_CryoTank` | PH | Ø 1.6 × 3.0 | ✔ | Sto (vial store), Opn |
| `PH_F3_Transformer` | PH | 1.5 × 2.2 × 1.5 | ✔ | Opn, Hck. The Mother's 3 boss transformers reuse it with a `WeakPoint` |
| `PROP_F3_CoolingColumn` | PROP | Ø 1.0 × 6.0 | ✔ | Ver (catwalks over the core), Opn |
| `PROP_F3_ContainmentConsole` | PROP | 1.2 × 1.2 × 0.8 | ✔ (crouched) | Sur (monitor room), Hck (data node) |
| `PH_F3_BrokenCapsule` | PH | 1.2 × 2.2 × 1.2 | ✔ | Opn (containment room) |
| `PROP_F3_PAHorn` | PROP | 0.6 × 0.6 × 0.8 | – | all F3, Mother room |
| `PROP_F3_HeavyCabling` | PROP | 2.0 × 0.4 × 0.6 | – (trip-free, no collider) | Cor (cable gallery), Ver |

## Key items and story objects (not props) — `Prefabs/Items/`
`ITEM_Heal`, `ITEM_AmmoBlister`, `ITEM_Spike`, `ITEM_KeyMandatory`, `ITEM_KeyOptional`, `ITEM_Document`, `ITEM_EmiliaDocument`, `ITEM_Confiscated`, `ITEM_Ring`, `INT_PowerSwitch`, `INT_HackPanel`, `INT_PersonalTray` (the revolver tray in the capsule row). Visual descriptions: `GDD/04 - Contenido/04 - Objetos y Coleccionables.md`. All are simple primitive builds (`PROP`-quality) with the silhouette rules from that file (e.g. the heal kit's handle, the mandatory key's stair icon).

## Boss — `Prefabs/PH/`
`PH_TheMother` (cryo-archive "cathedral shelf" on a crane, ≈ 8 × 10 × 4), `PH_TheMother_CollectorArm`, `PH_TheMother_HostileCamera`, `PROP_F3_MixingTable` (hack node host ×3).

## CRT monitors (D-157)

`PROP_F1_CRT` (steel), `PROP_F2_CRT` (yellowed plastic), both set on desks and crates; `PROP_F3_CRTBank` (floor-standing rack of six, one red). Placement: `Scripts/Editor/RoomDress.cs`.
