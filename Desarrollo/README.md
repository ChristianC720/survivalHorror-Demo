# Desarrollo — implementation guides

English guides that turn the GDD (`GDD/`, Spanish) into build steps for Claude Code and Aura. Read `CLAUDE.md` first.

**Equipo:** para entender y modificar el proyecto en Unity, empieza por [GUIA_EQUIPO_UNITY.md](GUIA_EQUIPO_UNITY.md) (en español).

| Guide | Epic | Milestone |
|---|---|---|
| [00-project.md](00-project.md) | E-00 Project setup, packages, render | H1 |
| [01-core.md](01-core.md) | E-01 Bootstrap, state machine, events, scenes, pause | H1 |
| [02-player.md](02-player.md) | E-02 Input contexts, movement, stamina, health | H1 |
| [03-camera.md](03-camera.md) | E-03 OTS camera, aim, surveillance | H1 / H3 |
| [04-combat.md](04-combat.md) | E-04 Revolver, shove, spike, finisher | H2 |
| [05-ai.md](05-ai.md) | E-05 Perception, alert states, 4 enemy types | H2 / H5 |
| [06-inventory-interaction.md](06-inventory-interaction.md) | E-06 Inventory, pickup, documents, flashlight, hacking, doors | H2 |
| [07-generation.md](07-generation.md) | E-07 Floor graph, validator, builder, NavMesh | H3 |
| [08-cycle-persistence.md](08-cycle-persistence.md) | E-08 Death, reconstruction, save, recycling, degradation | H3 / H5 |
| [09-boss.md](09-boss.md) | E-09 The Mother | H5 |
| [10-ui.md](10-ui.md) | E-10 Arm terminal, menus, subtitles, options, localization | H4 |
| [11-audio.md](11-audio.md) | E-11 AudioCue, mixer, signals | H4 |
| [12-art-render.md](12-art-render.md) | E-12 Pixel render, lighting, materials, placeholders | H1 / H4 |
| [13-quality-build.md](13-quality-build.md) | E-13 Tests, profiling, build | all / H6 |
| [14-levels.md](14-levels.md) | E-14 Kits, rooms, tutorial, Mother room, intro | H3 / H5 |
| [15-texts.md](15-texts.md) | E-15 Documents, bestiary, posters, string tables | H5 |

Reference sheets: [KIT_SPEC.md](KIT_SPEC.md) · [OBJECT_CATALOG.md](OBJECT_CATALOG.md) · [PLACEHOLDERS.md](PLACEHOLDERS.md) · [AUDIO_CUES.md](AUDIO_CUES.md) · [EVENTS.md](EVENTS.md) · [ROADMAP.md](ROADMAP.md)

## Guide format
Each guide has: **Goal** · **GDD sources** (M/S/D ids) · **Deliverables** (files) · **Tasks** (each one = one git-flow feature branch `E##-<task>`) · **Acceptance** (from `GDD/09 - Calidad/04 - Criterios de Aceptación.md`) · **Playtest** (what `run_play_mode_sequence` must prove).

Numbers in the guides are pointers. The authoritative values are in the GDD, and in code they come from the `Balance` ScriptableObjects.
