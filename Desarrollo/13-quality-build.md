# E-13 Quality, profiling and build

**GDD sources:** `09 - Calidad/01..04`, `07 - Tecnología/03 - Requisitos Técnicos.md`.

## Testing
- **EditMode** (`Assets/_Project/Tests/EditMode`, NUnit): every pure model (state machine, stamina, health, revolver, inventory, hack sequence, perception accumulation, attention meter, layout generator, validator (1 000 seeds), budget, death counter, save round-trip). Run with `mcp-for-unity` `run_tests` (mode EditMode) and fix failures before committing.
- **Aura playtests**: each task ends with a `run_play_mode_sequence` that loads the relevant scene, drives input, waits, and checks the expected `[Heartbeat]` log lines with `get_unity_logs` + a screenshot. Keep a short note of the sequence in the commit message body.
- Critical tests CP-01..CP-07 from `09 - Calidad/02 - Pruebas.md` at each milestone.

## Debug tools (dev builds only)
`Scripts/Debug/DebugConsole.cs` (backquote key): `seed <n>`, `floor <1-3>`, `give <item> <n>`, `kill`, `god`, `docs all`, `ending <id>`, `alert <state>`. Shows the current seed (needed for bug reports).

## Performance budget
1080p 60 FPS on the recommended spec (GTX 1060). ≤ 6 active AI, ≤ 16 dynamic lights, ≤ 4 shadowed, floor generation + NavMesh ≤ 5 s, build ≤ 2 GB. Use `get_worst_cpu_frames` / `get_worst_gc_frames` and `list_objects_with_high_polygon_count`.

## Build (H6)
Win64, IL2CPP or Mono (Mono if IL2CPP is not installed), scenes in order Bootstrap, MainMenu, Intro, Floor, Cinematics. Release flow: `git flow release start H6` → build → tag `v0.N-demo` → finish into `main`. Builds are numbered 0.1, 0.2… in order (`bundleVersion` = N); the current one is `v0.5-demo`. The first build, tagged `v1.0-demo` until 2026-11-09, is `v0.1-demo`.

## Profiling log (E13-profiling, 2026-11-02, editor, Floor 1 seed 12345)
- Exploration ~170 FPS, surveillance view ~110 FPS in the editor (GPU wait dominates). Floor 1 built in 196 ms (budget 5 s). No mesh above 3 000 triangles. Rooms log ≤ 4 lights / 1 shadowed around the player.
- Per-frame GC came from logging every audio cue (each footstep: ~24 KB with stack trace) and from a delegate allocated by `Revolver.Update` every frame. Fixed: the cue log keeps its `[Heartbeat][Audio] CUE_X` format but is stripped from release builds; the reload delegate is cached.
- Remaining one-off hitches: first terminal open (~45 ms: TMP mesh generation of the log list) and entering surveillance (~12 ms). Acceptable for the demo; prewarm the terminal on floor load if they show in a release build.
