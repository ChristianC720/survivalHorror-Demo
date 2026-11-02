# Placeholder registry (Blender models pending)

Every `PH_` prefab is a primitive composite at real scale that stands in for a Blender model (production plan: `BLENDER_PLAN.md`). Swap procedure: replace the children under `PH_<Name>/Mesh` with the imported model and keep the root, its pivot, colliders and scripts. Gameplay scripts live on the parent prefab (e.g. `Enemy_Alto` contains `PH_Alto`), so swapping never breaks references.

Rules
- Root pivot at the feet / floor center; scale (1,1,1); children hold the primitives under a `Mesh` child.
- Root carries `PlaceholderInfo` (intended model, triangle budget, notes).
- Colour = role colour from the palette (`GDD/05 - Arte y Audio/01 - Dirección de Arte.md`): Lukas teal, assimilated purple-grey with iridescent purple, The Mother crimson, props base warm grey.
- Mark the facing direction with a small cube on the +Z side (characters) so orientation is readable.

| Prefab | Stands for | Real size (W×H×D m) | Primitive build | Tri budget (final) | Rig / anim needed | Status |
|---|---|---|---|---|---|---|
| `PH_Lukas` | Lukas R-H-07 (teal overalls, arm terminal on left forearm) | 0.75 × 1.78 × 0.37 | capsule body, sphere head, box arm terminal (left) | ≤ 8k | Humanoid, all clips in Blender: locomotion, aim, reload, shove, grab, injured/critical gaits | placeholder done |
| `PH_Lukas_Civil` | Lukas in the intro (civilian clothes) | same | same, grey | 5–8k | same | pending |
| `PH_Alto` | El Alto | 0.8 × 2.5 × 0.6 | tall capsule, long box arms | ~4–8k | Long reach attack, grab | placeholder done (Enemy_Alto) |
| `PH_Rastrero` | El Rastrero | 0.6 × 1.0 × 1.4 | horizontal capsule, low | ~4–8k | Fast quadruped crawl, flank, lunge | placeholder done (Enemy_Rastrero) |
| `PH_Boca` | La Boca: blind test subject, two-lobed split head, vertical mouth (emaciated only when recycled) | 0.6 × 1.9 × 0.5 (hunched) | skin boxes + crimson vertical mouth slit, rags | ~4–8k | Scream, listen tilt, shamble/lurch | placeholder done (`Enemy_Boca`) |
| `PH_Mudo` | El Mudo: faceless, grey Vitalis staff clothing (nurse, bandages and face plate only when recycled) | 0.55 × 1.8 × 0.35 | dirty-white boxes, metal face plate | ~4–8k | Frozen idle, spasm activation, burst run, grab | placeholder done (`Enemy_Mudo`) |
| `PH_Recycled_Parts` | Metal pieces added to recycled enemies | – | small grey boxes parented to enemy | ≤ 500 | – | pending |
| `PH_TheMother` | Cryo-archive shelf on a crane | 8 × 10 × 4 | stacked boxes + cylinders, crimson inner light | ~15k | Crane movement, phase states | pending |
| `PH_TheMother_CollectorArm` | Collector arm | 1 × 1 × 6 | box segments | ≤ 4k | Patrol, sweep, grab | pending |
| `PH_TheMother_HostileCamera` | Hostile camera | 0.4 × 0.4 × 0.6 | box + cylinder lens | ≤ 1k | Pan / focus | pending |
| `PH_Shared_Corpse` | Static dead assimilated | 0.6 × 0.3 × 1.8 | flattened capsule | ~2–4k | – | pending |
| `PH_F1_Generator`, `PH_F1_Boiler`, `PH_F1_Forklift`, `PH_F1_GantryCrane`, `PH_F1_WaterPump` | Floor 1 machines | see `OBJECT_CATALOG.md` | boxes / cylinders | ≤ 2k each | – | Generator, Boiler done; others pending |
| `PH_F2_HumanCapsule`, `PH_F2_MonsterCapsule`, `PH_F2_ResonanceCapsule` | Floor 2 capsules | see catalog | cylinder + box base | ≤ 2k each | – | pending |
| `PH_F3_CryoTank`, `PH_F3_Transformer`, `PH_F3_BrokenCapsule` | Floor 3 machines | see catalog | cylinders / boxes | ≤ 2k each | – | pending |
| `PH_Revolver` | Kessler revolver (ceramic frame, green round counter) | 0.25 × 0.15 × 0.05 | boxes; counter is a TMP 3D text | ≤ 1.5k | – | placeholder done (inside Player prefab, WeaponSocket) |
| `PH_Capsule_Reconstruction` | Reconstruction capsule (capsule row) | 1.0 × 2.2 × 1.0 | cylinder + glass box | ≤ 2k | open/close | placeholder done |
| `PH_Intro_House`, `PH_Intro_Elevator` | Intro set pieces | room-sized | kit-like boxes | – | – | placeholder done: `Set_House`, `Set_Elevator`, `Set_Elevator2`, `Set_NonexistentFloor` in `Intro.unity` (F1/F2 kit shells + primitive table, chair, photo) |
| `PH_KesslerGuard_Corpse` | Assimilated Kessler guard lying in the tutorial depot with the revolver (D-74) | 1.8 m lying | dark capsule + assimilated head | ≤ 5k | static | placeholder done (`ROOM_F1_Storage_A`) |
| `PH_Lukas_Mudo` | Lukas turned into a Mudo, standing far away on the floor that does not exist (Assimilation ending, D-61) | 0.55 × 1.8 × 0.35 | assimilated capsule + metal face plate | ≤ 6k | frozen idle | placeholder done (`Cinematics.unity`) |
| `PH_Intro_Decoy` | The Mudo decoy at the end of the floor that does not exist (D-60) | 0.55 × 1.8 × 0.35 | dirty-white capsule, no AI | – | frozen idle | placeholder done (`Intro.unity`) |

Add a row whenever you create a new `PH_` prefab.

## UI placeholders
| Item | Intended asset | Current placeholder | Status |
|---|---|---|---|
| Title logo | "Identity Dissolution" logotype (D-90) | bold TMP text in `MenuView.ShowTitle` | pending art |
| Fonts | VT323 (data), IBM Plex Sans (menus), OpenDyslexic (option) | imported (OFL, licences next to the files in `UI/Fonts/`), dynamic TMP assets via `UI/Resources/UiFontSet` | done |
| Main menu backdrop | capsule-row room with the facility terminal next to Lukas's capsule (D-90) | flat black screen with green text | pending (E12-f1-lighting) |
| Credits roll | END table credits | hard-coded lines in `MenuView.ShowCredits` | pending (E15) |
| Arm terminal | world-space canvas on Lukas's forearm + screen mirror | screen-space panel only | pending Lukas model |
| The Mother (crane archive, collector arm, cryo core) | cryogenic sample archive on a crane with a collector arm and an opening core (D-65) | `ROOM_Mother`: CryoTank ×1.6 core, box arm + claw, cube cameras with red lights | pending model + core-opening animation |
