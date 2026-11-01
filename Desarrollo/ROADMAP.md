# Roadmap and task list

Target: **full configuration**, with the showcase as a `GameConfig` preset (×0.75 budgets, 6–8 + 2 + 1 rooms). GDD dates: H1 10-26 · H2 11-01 · H3 11-08 · H4 11-11 · H5 11-15 · tests 11-16→19 · H6 **2026-11-20** (`GDD/08 - Producción/03 - Roadmap.md`). The rebuild started 2026-10-31, so the schedule is compressed. Follow the cut order in `GDD/08 - Producción/01 - Alcance.md` if time runs out, and never cut the "never cut" list.

Each checkbox is one git-flow feature: `git flow feature start <id>`.

## H1 — Technical base
- [x] `E00-cleanup` — legacy prototype tagged and removed; docs, skills
- [x] `E00-packages` — Cinemachine 3, Timeline, Animation Rigging, Localization, Test Framework, TMP essentials; asmdefs; folders; layers (KIT_SPEC §6)
- [x] `E00-render` — pixelated ~540p Renderer Feature + dithering, Forward+, quality tiers
- [x] `E01-core` — `Bootstrap`, `Services`, `GameEvents`, `GameStateMachine`, `SceneLoader`, scenes in build
- [ ] `E01-pause` — pause (only `timeScale` user), confirmations
- [ ] `E02-input` — `Heartbeat.inputactions` with 8 context maps, `InputRouter`, fixed gamepad map
- [ ] `E02-player-motor` — `PH_Lukas`, CharacterController movement (crouch 0.8 / walk 1.5 / run 4 m/s), noise emitter
- [ ] `E02-stamina-health` — pure `StaminaModel`, `HealthModel` (Fine/Injured/Critical) + tests
- [ ] `E03-ots-camera` — Cinemachine OTS + aim (M-01)
- [ ] `E12-sandbox-kit` — `Sandbox_Kit` scene, snapping, F1 kit pieces (with Aura)

## H2 — Combat prototype
- [ ] `E04-revolver` · `E04-shove` · `E04-spike-grab` · `E04-finisher`
- [ ] `E05-perception` · `E05-alert-director` · `E05-alto` · `E05-rastrero`
- [ ] `E06-inventory` (pure model + tests) · `E06-interaction` · `E06-documents-registry` · `E06-flashlight` · `E06-hacking` · `E06-doors-seal`

## H3 — MVP
- [ ] `E14-f1-rooms` — 6 F1 archetype rooms + 2–3 F1 secondary key rooms + F1 final + capsule row + 4 tutorial rooms
- [ ] `E07-layout-graph` · `E07-validator` (1 000 seeds) · `E07-builder` · `E07-navmesh-activation` · `E07-budget`
- [ ] `E08-death-cycle` · `E08-save-service` · `E08-arm-terminal-data` · `E08-recycling` · `E08-exit-cost`
- [ ] `E03-surveillance` — iso view, signal stages, hidden attention, lockout (M-02, M-03)

## H4 — Vertical slice
- [ ] `E10-terminal-ui` · `E10-diegetic-hud` · `E10-subtitles` · `E10-main-menu` · `E10-options` · `E10-localization`
- [ ] `E11-audio-service` · `E11-mixer` · `E11-signals-heartbeats`
- [ ] `E12-f1-lighting` — one tint per room, ≤ 16 lights / ≤ 4 shadowed

## H5 — Content complete
- [ ] `E14-f2-kit-rooms` · `E14-f3-kit-rooms` · `E05-boca` · `E05-mudo`
- [ ] `E14-mother-room` · `E09-phase1-nodes` · `E09-phase2-cameras` · `E09-phase3-transformers` · `E09-final-decision`
- [ ] `E14-intro` · `E08-endings` · `E08-degradation-assimilation` (M-18) · `E08-machine-faults` (M-17)
- [ ] `E15-documents` · `E15-bestiary` · `E15-posters` · `E15-es-table`
- [ ] `E07-story-mode-showcase` — Story mode (+1 minimum) and showcase preset

## H6 — Release
- [ ] `E13-profiling` · `E13-bugfix` · `E13-build` → tag `v1.0-demo`
