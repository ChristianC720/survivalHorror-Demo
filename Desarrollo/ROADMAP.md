# Roadmap and task list

Target: **full configuration**, with the showcase as a `GameConfig` preset (×0.75 budgets, 6–8 + 2 + 1 rooms). GDD dates: H1 10-26 · H2 11-01 · H3 11-08 · H4 11-11 · H5 11-15 · tests 11-16→19 · H6 **2026-11-20** (`GDD/08 - Producción/03 - Roadmap.md`). The rebuild started 2026-10-31, so the schedule is compressed. Follow the cut order in `GDD/08 - Producción/01 - Alcance.md` if time runs out, and never cut the "never cut" list.

Each checkbox is one git-flow feature: `git flow feature start <id>`.

## H1 — Technical base
- [x] `E00-cleanup` — legacy prototype tagged and removed; docs, skills
- [x] `E00-packages` — Cinemachine 3, Timeline, Animation Rigging, Localization, Test Framework, TMP essentials; asmdefs; folders; layers (KIT_SPEC §6)
- [x] `E00-render` — pixelated ~540p Renderer Feature + dithering, Forward+, quality tiers
- [x] `E01-core` — `Bootstrap`, `Services`, `GameEvents`, `GameStateMachine`, `SceneLoader`, scenes in build
- [x] `E01-pause` — pause (only `timeScale` user), confirmations
- [x] `E02-input` — `Heartbeat.inputactions` with 8 context maps, `InputRouter`, fixed gamepad map
- [x] `E02-player-motor` — `PH_Lukas`, CharacterController movement (crouch 0.8 / walk 1.5 / run 4 m/s), noise emitter
- [x] `E02-stamina-health` — pure `StaminaModel`, `HealthModel` (Fine/Injured/Critical) + tests
- [x] `E03-ots-camera` — Cinemachine OTS + aim (M-01)
- [x] `E12-sandbox-kit` — `Sandbox_Kit` scene, snapping, F1 kit pieces (with Aura) + shared ramp/platform

## H2 — Combat prototype
- [x] `E04-revolver` · `E04-shove` · `E04-spike-grab` · `E04-finisher`
- [x] `E05-ai-core` (perception, alert director, Alto, Rastrero) — merged as one feature
- [x] `E06-inventory` · `E06-interaction` · `E06-documents-registry` · `E06-flashlight` · `E06-hacking` · `E06-doors-seal` (all done)

## H3 — MVP
- [x] `E14-f1-rooms` — 6 F1 archetype rooms + 2 secondary + final + capsule row (tutorial reuses Storage/Corridor/Open, D-70)
- [x] `E07-layout-graph` · `E07-validator` (1 000 seeds) · `E07-budget`
- [x] `E07-builder` · `E07-navmesh-activation` (12-room floor + NavMesh in 365 ms)
- [x] `E08-death-cycle` · `E08-save-service` · `E08-arm-terminal-data` (registry persisted per slot)
- [x] `E08-recycling`
- [x] `E08-exit-cost` — exit table D-31, calm exit cost, first-attempt degradation (D-94)
- [x] `E03-surveillance` — iso view, signal stages, hidden attention, lockout (M-02, M-03)

## H4 — Vertical slice
- [x] `E10-terminal-ui` · `E10-diegetic-hud` · `E10-subtitles` · `E10-main-menu` · `E10-options` · `E10-localization` (shortcuts: screen-space terminal, flat menu backdrop, aim assist + dyslexia font stored only — see PLACEHOLDERS.md)
- [x] `E11-audio-service` · `E11-mixer` · `E11-signals-heartbeats` (Boca scream lands with E05-boca)
- [x] `E12-f1-lighting` — one tint per room, ≤ 16 lights / ≤ 4 shadowed (RoomActivation budget, power-aware)

## H5 — Content complete
- [x] `E14-f2-kit-rooms` · [x] `E14-f3-kit-rooms` (F1 layouts re-dressed; unique layouts pending) · [x] `E05-boca` · [x] `E05-mudo` (feature E05-boca-mudo)
- [x] `E14-mother-room` · `E09-phase1-nodes` · `E09-phase2-cameras` · `E09-phase3-transformers` · `E09-final-decision` (one feature: E09-mother)
- [x] `E14-intro` (house, phone hack, elevator fall, floor that does not exist, full reconstruction) · [x] `E08-endings` (card sequences; staged shots with E14-intro) · [x] `E08-degradation-assimilation` (M-18 CRT + vocal pitch) · [x] `E08-machine-faults` (M-17: door backdoor F1 + mixer bypass F3, manual [F] during a hack, D-150)
- [x] `E15-documents` · `E15-bestiary` (36 records, corruption, first-encounter unlock) · [x] `E15-posters` (7 Vitalis safety posters in the F1 tutorial rooms, bound key per device, first attempt only) · [x] `E15-es-table` (all GDD texts EN+ES)
- [x] `E07-story-mode-showcase` — Story mode (+1 minimum) and showcase preset (new slots get it with the `-showcase` launch argument)

## H6 — Release
- [x] `E13-profiling` (13-quality-build.md: per-cue log and revolver delegate GC fixed; floor build ≤ 200 ms)
- [~] `E13-bugfix` — done so far: revolver found on the Kessler guard / capsule tray (D-74), dark tutorial depot, tutorial tunnel Rastrero cue, shove gasp (M-06), unique documents per floor, Intro pause (no Restart, free quit), compiler warnings, localization line breaks, teardown NREs; plus D-61 ending details (Assimilation staged shot, Full relief second act, Ambiguous heartbeats), D-90 main menu in the capsule-row room, 11 floor-specific F2/F3 rooms (all archetypes on F3), German ambience posters (D-58), floor-2 finding (D-56), R-H line, reconstruction label, legible surveillance feed, rebindable hack keys, E prompt hidden outside Interact contexts, device-aware inventory hints
- [x] `E13-build` → tag `v1.0-demo` (Win64 Mono, 110 MB, Builds/v1.0-demo/IdentityDissolution.exe; boots to the main menu with no errors)
- [~] `E16-gdd-alignment` (2026-11-08 review): D-92, D-94, D-165..D-175 in code and GDD; pending: CP-01..CP-07 run, full playthrough, tag `v1.3-demo`
