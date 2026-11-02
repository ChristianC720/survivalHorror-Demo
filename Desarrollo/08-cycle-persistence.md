# E-08 Death cycle and persistence (S-07, S-08, S-09)

**Goal:** dying is part of the loop: Lukas is rebuilt in the capsule row with the next subject code, loses his items, keeps his knowledge, and the floor changes. Progress is saved in 3 slots.

**GDD sources:** S-07, S-08, S-09, M-16, M-17, M-18, M-20; `02 - Diseño/07 - Reglas y Estados del Juego.md` (exit table, counter), `07 - Tecnología/04 - Datos y Persistencia.md`, `06 - Experiencia/04 - Guardado y Configuración.md`; D-30, D-32, D-35, D-39, D-63, D-74, D-99.

## Deliverables
`Scripts/Save/SaveService.cs`, `SaveData.cs` (slot: mode, floor, attempt seeds, inventory, death counter, subject code, degradation, registry ids, faults patched), `GlobalSave.cs` (settings, brightness calibrated), JSON in `Application.persistentDataPath/saves/slot{1..3}.json` with atomic write (temp + replace) and version field. `Scripts/Cycle/DeathCycle.cs`, `DeathCounter.cs` (pure), `Degradation.cs`, `SubjectCode.cs`. `Scripts/Recycle/RecycleSystem.cs`, `Corpse.cs`. `Scripts/Level/FaultPoint.cs` (M-17).

## Rules
- Death → save first (counter, registry) → discard the autosave → empty the inventory → regenerate floors → reconstruction cinematic (full after the intro, short afterwards) → capsule row of floor 1 with the revolver **unloaded** on the personal tray. Subject code +1 (R-H-07 → R-H-08 …).
- "Restart" from pause counts as a death. Quitting during an alert or hunt counts as a full death (exit table).
- **Counter (M-18, Desirable):** 5 consecutive deaths → Assimilation ending, then credits. Only reaching floor 2's final key room resets it. Deaths in the first attempt (tutorial) and the scripted intro death do not count.
- Degradation: CRT corruption and vocal distortion grow per death, capped; never hides information.
- Editor play-from-scene (Floor, Sandbox_*) uses the hidden dev slot `slot_3.json` (`CycleController.DevSlot`), recreated every Play and never "last used"; real slots are only touched through Bootstrap/MainMenu.
- Autosave: in the final key rooms of floors 1 and 2, only if no enemy is hunting (pending until the hunt ends). Floor 3: none.
- **Recycling (M-20):** every dead enemy rises once after ~40 s with a ~3 s warning, unless finished. A recycled enemy uses the recycled stats.
- **Machine faults (M-17, Desirable):** discoverable shortcuts that The Mother patches after 1–2 uses; the record stays in the terminal.
- "New game" is the only thing that clears the registry.

## Tasks
`E08-save-service` · `E08-death-cycle` · `E08-arm-terminal-data` · `E08-recycling` · `E08-exit-cost` · `E08-endings` (H5) · `E08-degradation-assimilation` (H5) · `E08-machine-faults` (H5).

## Acceptance
- M-16: die with 3 items and 2 documents → after the short cinematic Lukas is in the floor 1 capsule with no items, both documents and the next R-H code.
- M-18: counter at 4 after floor 2, die again → Assimilation, then credits.
- M-20 row. EditMode: save round-trip, counter rules, corrupted-file fallback.
