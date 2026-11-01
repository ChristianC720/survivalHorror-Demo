# Placeholder registry (Blender models pending)

Every `PH_` prefab is a primitive composite at real scale that stands in for a Blender model (owner: Bryan Iván Navarrete Canché, art/animation). Swap procedure: replace the children under `PH_<Name>/Mesh` with the imported model and keep the root, its pivot, colliders and scripts. Gameplay scripts live on the parent prefab (e.g. `Enemy_Alto` contains `PH_Alto`), so swapping never breaks references.

Rules
- Root pivot at the feet / floor center; scale (1,1,1); children hold the primitives under a `Mesh` child.
- Root carries `PlaceholderInfo` (intended model, triangle budget, notes).
- Colour = role colour from the palette (`GDD/05 - Arte y Audio/01 - Dirección de Arte.md`): Lukas teal, assimilated purple-grey with iridescent purple, The Mother crimson, props base warm grey.
- Mark the facing direction with a small cube on the +Z side (characters) so orientation is readable.

| Prefab | Stands for | Real size (W×H×D m) | Primitive build | Tri budget (final) | Rig / anim needed | Status |
|---|---|---|---|---|---|---|
| `PH_Lukas` | Lukas R-H-07 (teal overalls, arm terminal on left forearm) | 0.5 × 1.78 × 0.3 | capsule body, sphere head, box arm terminal (left) | 5–8k | Humanoid; Mixamo locomotion, aim, reload, shove, grab, injured/critical gaits | placeholder done |
| `PH_Lukas_Civil` | Lukas in the intro (civilian clothes) | same | same, grey | 5–8k | same | pending |
| `PH_Alto` | El Alto | 0.8 × 2.5 × 0.6 | tall capsule, long box arms | ≤ 6k | Long reach attack, grab | pending |
| `PH_Rastrero` | El Rastrero | 0.6 × 1.0 × 1.4 | horizontal capsule, low | ≤ 5k | Fast quadruped crawl, flank, lunge | pending |
| `PH_Boca` | La Boca (blind, screams) | 0.7 × 1.9 × 0.5 | capsule, large sphere jaw | ≤ 5k | Scream, walk | pending |
| `PH_Mudo` | El Mudo (still decoy, grabs) | 0.6 × 1.8 × 0.4 | capsule, slumped | ≤ 5k | Idle-still, activation, grab | pending |
| `PH_Recycled_Parts` | Metal pieces added to recycled enemies | – | small grey boxes parented to enemy | ≤ 500 | – | pending |
| `PH_TheMother` | Cryo-archive shelf on a crane | 8 × 10 × 4 | stacked boxes + cylinders, crimson inner light | ≤ 20k | Crane movement, phase states | pending |
| `PH_TheMother_CollectorArm` | Collector arm | 1 × 1 × 6 | box segments | ≤ 4k | Patrol, sweep, grab | pending |
| `PH_TheMother_HostileCamera` | Hostile camera | 0.4 × 0.4 × 0.6 | box + cylinder lens | ≤ 1k | Pan / focus | pending |
| `PH_Shared_Corpse` | Static dead assimilated | 0.6 × 0.3 × 1.8 | flattened capsule | ≤ 3k | – | pending |
| `PH_F1_Generator`, `PH_F1_Boiler`, `PH_F1_Forklift`, `PH_F1_GantryCrane`, `PH_F1_WaterPump` | Floor 1 machines | see `OBJECT_CATALOG.md` | boxes / cylinders | ≤ 2k each | – | pending |
| `PH_F2_HumanCapsule`, `PH_F2_MonsterCapsule`, `PH_F2_ResonanceCapsule` | Floor 2 capsules | see catalog | cylinder + box base | ≤ 2k each | – | pending |
| `PH_F3_CryoTank`, `PH_F3_Transformer`, `PH_F3_BrokenCapsule` | Floor 3 machines | see catalog | cylinders / boxes | ≤ 2k each | – | pending |
| `PH_Revolver` | Kessler revolver (ceramic frame, green round counter) | 0.25 × 0.15 × 0.05 | boxes; counter is a TMP 3D text | ≤ 1.5k | – | pending |
| `PH_Capsule_Reconstruction` | Reconstruction capsule (capsule row) | 1.0 × 2.2 × 1.0 | cylinder + glass box | ≤ 2k | open/close | pending |
| `PH_Intro_House`, `PH_Intro_Elevator` | Intro set pieces | room-sized | kit-like boxes | – | – | pending |

Add a row whenever you create a new `PH_` prefab.
