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
Win64, IL2CPP or Mono (Mono if IL2CPP is not installed), scenes in order Bootstrap, MainMenu, Intro, Floor, Cinematics. Release flow: `git flow release start H6` → build → tag `v1.0-demo` → finish into `main`.
