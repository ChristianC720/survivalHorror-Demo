---
description: Verify a finished task in Play Mode with run_play_mode_sequence, log assertions and screenshots, then report pass/fail against the GDD acceptance criterion. Use at the end of every gameplay task before committing.
argument-hint: "<task id, e.g. E04-revolver> [scene]"
---

# Playtest a task

1. Find the task's acceptance row in its guide (`Desarrollo/<NN>-*.md`) and in `GDD/09 - Calidad/04 - Criterios de Aceptación.md`. Write the Given / When / Then you will test.
2. `check_compile_errors` must be clean; `save_scene`.
3. Open the test scene (the guide names it; default `Floor`, or a `Sandbox_*` scene with the setup). Bootstrap auto-loads in Play Mode.
   Input injection relies on `Assets/_Project/Input/InputSystemSettings.asset` (editor input always goes to the Game view, background behaviour IgnoreFocus). If injected keys do nothing, check that asset first.
4. Build a `run_play_mode_sequence`: wait for `[Heartbeat][Core] Services ready`, drive the inputs the criterion needs (hold keys for durations, mouse buttons), add waits matching the GDD timings, take screenshots at the key moments.
5. After it ends: `get_unity_logs` (errors + the expected `[Heartbeat][...]` lines). Any exception = fail.
6. Look at the screenshots. Check the diegetic-UI rule: only reticle, `E` prompt, subtitles and the sync indicator may be on screen.
7. `stop_game` if still playing.
8. Report: PASS/FAIL per Given/When/Then, log excerpts, screenshot paths, and a fix proposal for failures. Do not commit on FAIL.
