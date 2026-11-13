# Audio cue registry (placeholders)

Source: `GDD/05 - Arte y Audio/05 - Audio y Música.md`, S-11 in `GDD/02 - Diseño/03 - Sistemas.md`, M-10. Owner of the final audio: Lian Daniel Sierra Vera.

## How placeholders work
- Every sound is an `AudioCue` ScriptableObject in `Assets/_Project/ScriptableObjects/Audio/` named `CUE_<Group>_<Name>`.
- Fields: `AudioClip[] clips` (**empty for now**), `AudioMixerGroup bus`, `CuePriority priority` (Signal = 1, Combat = 2, Ambience = 3, Music = 4), `bool spatial`, `float volume`, `Vector2 pitchRange`, `float maxDistance`, `bool loop`, `string debugCaption`.
- `AudioService.Play(cue, position)` always raises `GameEvents.CuePlayed(cue, position)` and logs `[Heartbeat][Audio] <cue name>` (editor and development builds only). Playtests assert on that line. If the clip list is empty, a development build lists `debugCaption` in the top-right corner (`UI.SoundCaptionView`). Player-facing sound captions (option, D-102) come from the `CAP` table, not from `debugCaption`; add a `CAP.*` row for every new Signal cue. This lets QA "hear" the placeholder.
- Gameplay never touches the service: event-driven cues are mapped in `AudioService.OnEnable` (ShotFired, alerts, autosave…); local feedback uses `Sfx.Play(Sfx.DoorOpen, pos)` / `Sfx.Play2D(...)` (Core), which raises `GameEvents.CueRequested`. Loops owned by audio components use `AudioService.StartLoop/Stop`. `Sfx.Stop(cue)` (raises `GameEvents.CueStopRequested`) fades out every playing instance of a cue in 0.2 s: sequences use it so a long one-shot does not run over the next sound (elevator hum and fail, capsule reconstruction).
- The `AudioService` component sits on the `Bootstrap` object with every cue in its `cues` list. A new cue must be added there (re-run the cue wiring script or drag it in), or it logs `Unknown cue`.
- Final audio = drop clips into the existing cue assets. No code changes.

## Mixer (`Assets/_Project/Audio/Mixer/Heartbeat.mixer`)
`Master` → `Signals` (P1) · `PlayerCombat` (P2) · `Ambience` (P3, includes lab heartbeat) · `Music` (P4) · `Voices` (vocal sounds + captions). A P1 cue ducks `Ambience` and `Music` (−8 dB, 0.6 s hold, 1.5 s release; fields on `AudioService`). The duck is done in code on the exposed params, not with a Duck Volume effect, because mixer effects cannot be created through a public API. The Options volumes map to the exposed parameters `vol_master`, `vol_music`, `vol_sfx`, `vol_ambience`, `vol_voices` (+ `vol_signals`, not in Options) through `AudioService.ApplyVolumes(GlobalData)`, applied on start.

## Cue list
| Cue | Bus / P | Spatial | Loop | Trigger | GDD |
|---|---|---|---|---|---|
| `CUE_Alert_Suspicion` | Signals / 1 | ✔ | – | enemy enters Suspicion (highest state only) | M-10, D-23 |
| `CUE_Alert_Search` | Signals / 1 | ✔ | – | enemy enters Search | M-10 |
| `CUE_Alert_Hunt` | Signals / 1 | ✔ | – | enemy enters Hunt | M-10 |
| `CUE_Heartbeat_Player` | Signals / 1 | – | ✔ | rate driven by nearest-threat distance (occluded by sealed doors) | S-11 |
| `CUE_Heartbeat_Lab` | Ambience / 3 | – | ✔ | always after first reconstruction; grows with deaths and Critical | D-64, D-95 |
| `CUE_Boca_Scream` | Signals / 1 | ✔ | – | La Boca screams | D-64 |
| `CUE_Step_Metal` / `_Water` / `_Debris` | Player&Combat / 2 | ✔ | – | footstep by surface (player and enemies) | M-10 |
| `CUE_Recycle_Warning` | Signals / 1 | ✔ | – | ~3 s convulsion before a corpse rises (~40 s) | M-20, D-99 |
| `CUE_Lukas_Gasp` | Player&Combat / 2 | – | – | not enough stamina | M-06 |
| `CUE_Revolver_Shot` | Player&Combat / 2 | ✔ | – | shot (noise ~20 m) | M-04, D-74 |
| `CUE_Revolver_ReloadRound` | Player&Combat / 2 | ✔ | – | each round loaded | M-04 |
| `CUE_Revolver_DryFire` | Player&Combat / 2 | ✔ | – | empty drum | M-04 |
| `CUE_Shove` | Player&Combat / 2 | ✔ | – | shove / butt strike | M-06 |
| `CUE_Spike_Discharge` | Player&Combat / 2 | ✔ | – | spike fires (grab or finisher) | M-11, M-20 |
| `CUE_Hack_Input` / `CUE_Hack_Error` / `CUE_Hack_Success` | Signals / 1 | ✔ | – | hacking | M-13 |
| `CUE_Power_Startup` | Signals / 1 | ✔ | – | power switch (alerts nearby enemies) | D-72 |
| `CUE_HostileCamera_Motor` | Signals / 1 | ✔ | ✔ | boss phase 2 camera turning | D-28 |
| `CUE_Terminal_On` / `CUE_Terminal_Register` | Player&Combat / 2 | – | – | arm terminal opens / item registered | M-08 |
| `CUE_Capsule_Reconstruction` | Signals / 1 | ✔ | – | reconstruction: played once by the shot that shows the capsule (intro, short reconstruction, The Mother) and stopped with `Sfx.Stop` at the cut; the `Reconstructed` event plays nothing | D-39 |
| `CUE_Transit_Lockdown` | Signals / 1 | ✔ | – | stairs/elevator locked during a hunt | D-96 |
| `CUE_Autosave_Sync` | Signals / 1 | – | – | "SYNCHRONIZING…" | D-30 |
| `CUE_Door_Open` / `CUE_Door_Locked` / `CUE_Door_Seal` / `CUE_Door_SealBreak` | Player&Combat / 2 | ✔ | – | doors | M-14 |
| `CUE_Enemy_Attack` / `CUE_Enemy_Grab` / `CUE_Enemy_Death` | Player&Combat / 2 | ✔ | – | enemy combat | S-02, S-03 |
| `CUE_Pickup` | Player&Combat / 2 | – | – | item picked up | M-07 |
| `CUE_Heal_Use` | Player&Combat / 2 | – | – | heal animation (~1.5 s) | M-05 |
| `CUE_Flashlight_Toggle` / `CUE_Flashlight_Flicker` | Player&Combat / 2 | – | – | flashlight | M-12 |
| `CUE_Arm_Hydraulic` | Signals / 1 | ✔ | – | collector arm warns ~1 s before grabbing | D-98 |
| `CUE_HostileCamera_Lock` | Signals / 1 | ✔ | – | hostile camera locks its red light on Lukas | D-28 |
| `CUE_Transformer_Hit` / `CUE_Transformer_Destroyed` | Player&Combat / 2 | ✔ | – | phase 3 transformer shot / destroyed | D-80 |
| `CUE_Boss_NodeHacked` | Signals / 1 | ✔ | – | phase 1 node goes offline | M-19 |
| `CUE_Surveillance_Static` | Signals / 1 | – | ✔ | while surveillance view is open | M-02 |
| `CUE_Amb_F1` / `CUE_Amb_F2` / `CUE_Amb_F3` / `CUE_Amb_Mother` | Ambience / 3 | – | ✔ | floor ambience drones | S-11 |
| `CUE_Vocal_Lukas` / `CUE_Vocal_Mother` / `CUE_Vocal_Emilia` | Voices / 2 | – | – | vocal sound accompanying a subtitle line | D-52 |
| `CUE_Music_Title` (El latido, title) · `CUE_Music_Menu` (Emilia) · `CUE_Music_VoiceNote` (Emilia, intro) · `CUE_Music_Mother_P1..P3` · `CUE_Music_Ending_*` · `CUE_Music_Awakening` | Music / 4 | – | ✔ | menu, boss phases, endings | D-87 |
| `CUE_Intro_PhoneMessage` | PlayerCombat / 2 | – | – | Emilia's scheduled voice note arrives (intro house) | D-53 |
| `CUE_Elevator_Hum` | PlayerCombat / 2 | – | – | intro elevator descends; the second elevator rises on its own | D-53 |
| `CUE_Elevator_Fail` | PlayerCombat / 2 | – | – | intro elevator jolts and loses power | D-53 |
| `CUE_Elevator_Fall` | PlayerCombat / 2 | – | – | intro elevator distorts and drops (cinematic) | D-53, D-46 |
| `CUE_Elevator_Impact` | PlayerCombat / 2 | – | – | intro elevator stops at the floor that does not exist | D-53 |
| `CUE_Intro_Attack` | PlayerCombat / 2 | – | – | attack from behind, cut to black (scripted death) | D-53, D-63 |
| `CUE_Rastrero_Behind` | PlayerCombat / 2 | ✔ | – | tutorial tunnel (first attempt): a Rastrero drags itself behind the far door when Lukas is within 4 m (`TutorialCue`) | D-70 |
| `CUE_Ending_BeatLukas` / `CUE_Ending_BeatLab` | Ambience / 3 | – | – | one-shot heartbeats of the endings: Ambiguous (out of phase, then one) and Full relief (second act: three shots of the one-beat `BeatLab` clip, 1.3 s apart, each ~4 dB softer, as in the original 7 s file) | D-61 |
