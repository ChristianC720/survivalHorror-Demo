# GameEvents catalog

`Heartbeat.Core.GameEvents` is a static class of C# events, one per row of "Relaciones entre sistemas" (`GDD/02 - Diseño/03 - Sistemas.md` §Relaciones). Each event has a `Raise…` method and is cleared in `GameEvents.ResetAll()`, which `Bootstrap` calls on domain reload (needed with Enter Play Mode Options). **Add new events here first, then in code.**

| Event | Payload | From → To | GDD |
|---|---|---|---|
| `FloorBuilt` | `FloorContext ctx` (floor index, seed, rooms, camera rooms, final room) | S-06 → S-01, S-02, S-05, S-09, S-10, S-07 | S-06 |
| `RoomEntered` / `RoomExited` | `RoomInstance room` | Level → S-01, S-02, S-06 (activation) | S-06 |
| `FinalRoomReached` | `int floor` | S-06 → S-07 (autosave), S-08 (counter reset on floor 2) | D-30, D-35 |
| `FloorExitRequested` | `int floor` | Level → S-08 / state machine | M-15 |
| `AttentionFull` | `Vector3 playerPos` | S-01 → S-02 | D-24 |
| `HighestAlertChanged` | `AlertState state` | S-02 → S-01, S-11, S-07 | M-03, M-10 |
| `EnemyAlertChanged` | `MonoBehaviour enemy (EnemyBrain), AlertState state, float distance` | S-02 → S-11 | S-11 |
| `HuntChanged` | `bool hunting` (first enemy enters Hunt / last one leaves) | S-02 → S-01 (cut + lock), S-07 (pending autosave, exit cost), Level (transit lockdown) | M-03, D-96 |
| `PlayerAttacked` | `AttackInfo info` | S-02 → S-03 | S-03 |
| `PlayerGesture` | `string gesture` (animation name: Shove, Use_Door, Heal...) | S-03, Interaction, S-05 → PlayerAnimator | D-151 |
| `GrabStarted` | `MonoBehaviour grabber` (`EnemyBrain`; grab windup: Lukas is pulled to the hands, a hack is cancelled) | S-02 → S-03, Level | M-11 |
| `GrabResolved` | `bool spikeUsed` (grabs arrive as `PlayerAttacked` with `IsGrab`) | S-03 → UI, S-11 | M-11 |
| `HealthStateChanged` | `HealthState state` | S-03 → UI, Player (posture), S-11 | M-05 |
| `PlayerDied` | `DeathCause cause` | S-03 → S-08 | S-08 |
| `EnemyKilled` | `MonoBehaviour enemy (EnemyBrain), Vector3 corpsePos` | S-03 → S-09 | S-09 |
| `EnemyRecycled` | `EnemyBrain enemy` | S-09 → S-02 | M-20 |
| `RecycleWarning` | `Corpse corpse` | S-09 → S-11 | D-99 |
| `GroundStrikeNoise` | `Vector3 pos, float radius` | S-09 → S-02 | M-20 |
| `NoiseEmitted` | `NoiseInfo info` (pos, radius, source) | Player/Combat → S-02 | M-09 |
| `StaminaExhausted` | – | S-04 → S-03, S-11 | M-06 |
| `InventoryChanged` | `InventorySnapshot snap` | S-05 → UI, S-07 | M-07 |
| `ItemRegistered` | `RegistryEntry entry` (document, Emilia doc, confiscated, bestiary, rule, fault) | Interaction → S-07, UI, S-11 | M-08 |
| `InventoryCleared` | – | S-08 → S-05 | S-08 |
| `DegradationChanged` | `int level` | S-08 → S-01 (CRT), S-11 (heartbeat/voice), UI | M-18 |
| `Reconstructed` | `int subjectCode` (R-H-07, 08…) | S-08 → S-07, UI | M-16 |
| `AutosaveRequested` / `Saved` | `SaveReason reason` | S-07 | D-30 |
| `EndingTriggered` | `EndingId id` | S-08 / S-10 → state machine, S-11 | Premisa y Trama |
| `HostileCameraFocused` | `Vector3 point` | S-10 → S-02 | D-28 |
| `HostileCameraTurning` | `bool turning` | S-10 → S-11 | D-28 |
| `BossPhaseChanged` | `int phase` | S-10 → S-11, UI | M-19 |
| `BossDefeated` | – | S-10 → S-08 | M-19 |
| `PowerRestored` | `RoomInstance room` | Level → S-02 (noise), S-06 | D-72 |
| `FaultUsed` / `FaultPatched` | `FaultId id` | Level → S-07 | M-17 |
| `CueRequested` | `string cueId, Vector3? pos` (null = 2D), via `Sfx.Play` | any → S-11 | – |
| `SubtitleRequested` | `string speaker, string subKey, object[] args`, via `Subtitles.Say` | any → UI (SubtitleView plays `CUE_Vocal_<Speaker>`) | D-52 |
| `CuePlayed` | `string cueId, Vector3? pos` | S-11 → debug captions, tests | – |
| `AimChanged` | `bool aiming` | Camera → UI, Flashlight | M-01 |
| `ShotFired` | `Vector3 origin` | Combat → S-01 (attention), S-11 | M-04 |
| `InventoryChanged` | – | S-05 → UI, S-07 | M-07 |
| `DoorSealed` | `MonoBehaviour door` (`DoorLeaf`, sealed by completing the quick seal hack opened with a 0.5 s Interact hold, C-69; local telemetry) | Level → S-11 | M-12 |
| `ItemPickedUp` | `string kind, int amount` (a `WorldItem` picked up; local telemetry counts spikes found) | Interaction → S-11 | M-07 |
| `GameStateChanged` | `GameStateId from, GameStateId to` | state machine → all | Reglas y Estados |
| `InputContextChanged` | `InputContext ctx` | InputRouter → UI prompts | Controles |
