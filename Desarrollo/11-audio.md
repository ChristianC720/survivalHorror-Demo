# E-11 Audio (S-11) — placeholders

**Goal:** audio is gameplay information. The whole pipeline works with empty clips, so the final sounds drop in later without code changes.

**GDD sources:** `05 - Arte y Audio/05 - Audio y Música.md`, S-11, M-10. Cue list and mixer: `AUDIO_CUES.md`.

## Deliverables
`Scripts/Audio/AudioCue.cs` SO, `AudioService.cs` (pooled `AudioSource`s, 3D settings, priority voice stealing, ducking), `ThreatAudio.cs` (player heartbeat from nearest-threat distance, occluded by sealed doors), `LabHeartbeat.cs` (grows with deaths and Critical), `FootstepEmitter.cs` (surface by `PhysicsMaterial` name / `SurfaceTag`), `DebugCaptionView.cs`. Mixer asset `Audio/Mixer/Heartbeat.mixer` with groups and exposed volumes. All `CUE_*` assets from the registry.

## Tasks
- `E11-audio-service`: service + cue assets. Every play logs `[Heartbeat][Audio] CUE_X` and raises `CuePlayed`.
- `E11-mixer`: groups, ducking from the Signals group, exposed params bound to Options.
- `E11-signals-heartbeats`: alert cues (highest only), heartbeats, Boca scream, recycle warning, lockdown.

## Acceptance
- Playtest: walking on metal logs footstep cues; an enemy entering Hunt logs `CUE_Alert_Hunt` and the debug caption appears in development builds.
- No music during exploration.
