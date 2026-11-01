---
description: Project rules and map for Identity Dissolution (Heartbeat). Use at the start of any task in this Unity project, or when unsure where a design value, convention or file lives.
argument-hint: "[topic, e.g. 'events' or 'rooms']"
---

# Heartbeat context

You are working on **Identity Dissolution** (codename Heartbeat), a third-person survival horror roguelite in Unity 6000.6.6f1 / URP 17.

Read these before acting (they are short):
1. `CLAUDE.md` (project root): architecture rules, layout, naming, verify loop, git rules.
2. `Desarrollo/README.md`: index of the per-epic guides; open only the guide for your task.
3. Reference sheets when relevant: `Desarrollo/KIT_SPEC.md` (grid, pivots, sockets), `Desarrollo/OBJECT_CATALOG.md` (props), `Desarrollo/PLACEHOLDERS.md`, `Desarrollo/AUDIO_CUES.md`, `Desarrollo/EVENTS.md`, `Desarrollo/ROADMAP.md` (task list).

Design source of truth: `GDD/` (Spanish). If code and GDD disagree, the GDD wins. Grep `GDD/10 - Referencia/03 - Registro de Decisiones.md` for a `D-xx` id instead of reading it whole.

Hard rules (summary):
- No singletons outside `Bootstrap` services (`Services.Get<T>()`). Cross-system calls go through `GameEvents`.
- Every balance number in a ScriptableObject. Pure logic in plain C# with EditMode tests.
- Only Pause touches `Time.timeScale`. Inventory, terminal and hacking never pause.
- Input only through `InputRouter` and the `Heartbeat.inputactions` context maps; never `Keyboard.current`.
- No permanent HUD. In-game text from Localization keys; German never translated.
- Placeholders: `PH_` primitive composites for Blender models, `AudioCue` assets with empty clips.
- Kit pieces and rooms are built by hand on the 2 m grid with exact pivots. Never generate them with a script.
- Save the scene before batch operations; opening another scene discards unsaved changes.
- After any C# change: check compile errors, then logs. Log lines are prefixed `[Heartbeat][<System>]`.
- Git: git-flow feature branch per task; no AI co-author lines in commits.

If an argument was given, open the guide or sheet that matches it and summarise the relevant rules before starting.
