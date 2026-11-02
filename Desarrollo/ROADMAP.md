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
- [ ] `E14-intro` · [x] `E08-endings` (card sequences; staged shots with E14-intro) · [x] `E08-degradation-assimilation` (M-18 CRT + vocal pitch) · [x] `E08-machine-faults` (M-17: door backdoor F1 + mixer bypass F3, manual [F] during a hack, D-150)
- [x] `E15-documents` · `E15-bestiary` (36 records, corruption, first-encounter unlock) · [ ] `E15-posters` · [x] `E15-es-table` (all GDD texts EN+ES)
- [x] `E07-story-mode-showcase` — Story mode (+1 minimum) and showcase preset (new slots get it with the `-showcase` launch argument)

## H6 — Release
- [ ] `E13-profiling` · `E13-bugfix` · `E13-build` → tag `v1.0-demo`
