# E-09 The Mother (S-10, M-19)

**Goal:** a 3-phase boss built only from mechanics the player already knows, followed by the final decision.

**GDD sources:** M-19, S-10, `04 - Contenido/02 - Niveles.md` §Sala de The Mother, `03 - Historia y Mundo/01 - Premisa y Trama.md` (endings), D-28, D-51, D-62, D-65, D-80, D-114, D-115, D-117.

## Deliverables
Room prefab `KEY_F3_Mother` (E-14), `Scripts/Boss/MotherBoss.cs` (phase controller), `CollectorArm.cs`, `HackNode.cs` (reuses `HackPanel`), `HostileCamera.cs`, `CapsuleRelease.cs`, `Transformer.cs` (`WeakPoint`, 2 shots), `FinalDecision.cs`, `Balance/BossBalance.asset`, Timelines `TL_Mother_Fall`, `TL_Awakening`, `TL_Ending_*`.

## Phases
1. **Nodes:** hack 3 nodes on the mixing tables while the collector arm patrols (it can kill in every phase).
2. **Hostile cameras:** each camera focus (`HostileCameraFocused`) releases one assimilated from an archive capsule: **4 in total** (2 Altos, 2 Rastreros), max 2 active. Cryo fog is visual only. The player moves between blind spots (camera motor cue). Ends when the 4 are dead and Lukas reaches the core; if Lukas arrives early, the remaining ones are released.
3. **Transformers:** the cryo core opens and exposes 3 transformers (2 shots each). Destroying the last one drops The Mother.
Ammo in the room: 20 fixed rounds (12 at blind spots for phase 2, 8 next to the core for phase 3); 0–1 heals.
The Mother's protocol lines per phase as subtitles + `CUE_Vocal_Mother`.

After the fall: Awakening (lab heartbeat speeds up) → with 6 Emilia documents, a choice terminal/exit; without them, the Basic Relief ending. Endings: Assimilation (M-18), Basic Relief, Full Relief, Ambiguous.

## Acceptance
M-19: enter with no ammo → hack 3 nodes, kill the 4 with the room's 12 rounds, reach the core → phase 3 starts with 8 rounds next to it and 3 transformers of 2 shots each; destroying them drops The Mother. Death in the room returns to the start of the cycle.
