# CLAUDE.md — Identity Dissolution (codename Heartbeat)

Third-person survival horror with roguelite structure. Unity **6000.6.6f1**, URP 17 (Forward+), C#. Single-player PC demo.
The design source of truth is the Obsidian vault in `GDD/` (Spanish). The development guides in `Desarrollo/` (English) translate it into implementation steps. **If the code and the GDD disagree, the GDD wins** (and the GDD's own rule: the detailed section and `GDD/10 - Referencia/03 - Registro de Decisiones.md` win over summaries).

Status: rebuilt from scratch on 2026-10-31. The old prototype is only in git: `git show legacy-prototype:<path>` (e.g. `Assets/Scripts/AI/EnemyAI.cs`). Read it for ideas; never restore it wholesale.

## Where to look in the GDD
| Need | File |
|---|---|
| One-page overview | `GDD/00 - GDD Estructurado.md` |
| Mechanics M-01..M-20 (rules, numbers) | `GDD/02 - Diseño/02 - Mecánicas.md` |
| Systems S-01..S-11 + their events | `GDD/02 - Diseño/03 - Sistemas.md` |
| Game states and transitions | `GDD/02 - Diseño/07 - Reglas y Estados del Juego.md` |
| Controls per context | `GDD/02 - Diseño/04 - Controles.md` |
| Resource budget per floor | `GDD/02 - Diseño/06 - Economía y Recursos.md` |
| Floors, archetypes, tutorial, boss phases | `GDD/04 - Contenido/02 - Niveles.md` |
| Enemies | `GDD/04 - Contenido/03 - Enemigos y NPC.md`, `GDD/03 - Historia y Mundo/03 - Personajes.md` |
| Items | `GDD/04 - Contenido/04 - Objetos y Coleccionables.md` |
| Props per floor | `GDD/00 - Ideas de objetos por sala.md` → `Desarrollo/OBJECT_CATALOG.md` |
| All in-game text | `GDD/03 - Historia y Mundo/05 - Textos del Juego.md` |
| Architecture, scenes | `GDD/07 - Tecnología/01 - Arquitectura Técnica.md` |
| Save data | `GDD/07 - Tecnología/04 - Datos y Persistencia.md` |
| Acceptance criteria per mechanic | `GDD/09 - Calidad/04 - Criterios de Aceptación.md` |
| Tests CP-01..CP-07 | `GDD/09 - Calidad/02 - Pruebas.md` |

The decision log is 175 KB — grep it for a `D-xx` id instead of reading it whole.

## Architecture rules (non-negotiable)
- Scenes: `Bootstrap` (persistent services, first in build) · `MainMenu` · `Intro` · `Floor` (empty shell; rooms are instantiated by the generator) · `Cinematics` (additive). `Sandbox_*` scenes are for building and are never in the build.
- **No singletons outside `Bootstrap` services.** Services are reached through `Services.Get<T>()` (a registry populated by `Bootstrap`), never `FindObjectOfType` in `Update`.
- **Cross-system communication goes through `GameEvents`** (static C# events, one per row of the "Relaciones entre sistemas" table in Sistemas). The emitter never knows the listener. A system may *read* another system's state, but only changes it through a public method or an event.
- **Every balance number lives in a ScriptableObject** (`Assets/_Project/ScriptableObjects/Balance/`). No magic numbers in gameplay code. Valores are "initial, subject to balance".
- **Pure logic in plain C# classes** (no `MonoBehaviour`) so it is EditMode-testable: floor graph, validator, inventory rules, stamina, health states, save model, death counter, hack sequences.
- **Only Pause sets `Time.timeScale = 0`.** Inventory, terminal and hacking never pause.
- **Generation is deterministic**: same seed + same config = same floor. Log the seed on every floor build.
- Input: one `InputActionAsset` (`Assets/_Project/Input/Heartbeat.inputactions`) with one action map per context (Exploration, Aiming, Inventory, Hacking, Surveillance, OverBody, Menu, Cinematic). Only `InputRouter` enables/disables maps. **Never read `Keyboard.current` / `Mouse.current` in gameplay code.**
- Diegetic UI (D-86): no permanent HUD. On screen only: reticle, `E` prompt, subtitles, "SYNCHRONIZING…". Health is read from posture + arm terminal.
- Text: Unity Localization string tables, English (primary) + Spanish. **German strings are never translated** — keep them identical in both tables.
- Official Unity packages only (no Asset Store).

## Layout and naming
```
Assets/_Project/
  Scripts/<System>/          Core, Input, Player, Camera, Combat, AI, Inventory, Interaction, Level,
                             Generation, Cycle, Save, Recycle, Boss, Audio, UI, Narrative, Render, Debug
  Scripts/Editor/            editor-only tools (asmdef Heartbeat.Editor)
  Tests/EditMode/            asmdef Heartbeat.Tests.EditMode
  Prefabs/Kit/<Floor>/       modular kit pieces   (KIT_F1_Wall_2m, KIT_Shared_Floor_2x2 …)
  Prefabs/Rooms/<Floor>/     room prefabs         (ROOM_F1_Corridor_A, KEY_F1_Final …)
  Prefabs/Props/<Floor>/     props                (PROP_F1_Boiler …)
  Prefabs/PH/                Blender placeholders (PH_Lukas, PH_Alto, PH_TheMother …)
  Prefabs/Characters, Prefabs/Items, Prefabs/UI
  ScriptableObjects/{Balance,Enemies,Items,Rooms,Floors,Docs,Audio}
  Materials/<Floor>/, Scenes/, Timelines/, Localization/, UI/, Audio/Mixer/, Input/, Render/
```
- Namespaces `Heartbeat.<System>` (exceptions to avoid clashes with Unity types: `Heartbeat.InputRouting`, `Heartbeat.CameraSystem`); asmdef `Heartbeat.Runtime` for all runtime code.
- Code, identifiers, comments, logs: English. In-game text: Localization keys only.
- Floors: `F1` Maintenance (sodium amber), `F2` Illegal Clinic (cold fluorescent), `F3` Core (crimson). `Shared` = all floors.

## Grid, pivots and rooms (see `Desarrollo/KIT_SPEC.md`)
2 m cells, walls 3 m × 0.3 m, doorways 2 m × 2.2 m. Kit pieces are built **by hand with Aura** using grid snapping and exact pivots. Room prefabs are assembled by hand from kit pieces. At runtime the `FloorBuilder` joins room prefabs through `DoorSocket`s. Never generate kit pieces or rooms procedurally in an editor script.

## Placeholders
- **3D models that need Blender** (characters, enemies, The Mother, complex props): primitive composites at real scale, prefab `PH_<Name>`, pivot at the feet / floor center, root has `PlaceholderInfo` (intended model, owner, notes). Register each in `Desarrollo/PLACEHOLDERS.md`. Gameplay scripts go on the *parent* prefab, never on the PH mesh, so the mesh can be swapped.
- **Audio**: every sound is an `AudioCue` asset (clip may be null). Never put an `AudioClip` reference directly in gameplay code. Register cues in `Desarrollo/AUDIO_CUES.md`.
- **Cinematics**: Timelines with camera moves, text cards and fades over placeholder geometry.

## Tooling: Claude + Aura MCP
- Claude writes `.cs`, `.md`, `.asmdef`, `.inputactions` JSON with Write/Edit.
- **Aura (`mcp__aura-unity__*`) is the primary Unity interface**: `check_compile_errors`, `get_unity_logs`, `get_unity_editor_state`, `open_scene`/`save_scene`/`create_scene`, `create_game_object`, `set_transform`, `add_component`, `set_property`, `create_prefab`, `create_prefab_variant`, `create_pbr_material`/`assign_material`, `create_ui_element`/`set_rect_transform`/`set_ui_text`, input-action tools, `install_unity_package`, `execute_script`, `play_game`/`stop_game`/`run_play_mode_sequence`, `capture_scene_object`/`capture_editor_screenshot`/`capture_ui_canvas`.
- `mcp-for-unity` only for `run_tests` (EditMode) and `read_console` if Aura lacks it.
- One-off `execute_script` files go in the session scratchpad, **never** in the repo. Keep them small; they run on the main thread.

### Shared procedures (skills)
`.Aura/Skills/*/SKILL.md` are Claude-compatible procedures used by both Aura and Claude: `heartbeat-context`, `build-kit-piece`, `build-room-prefab`, `place-placeholder-prop`, `playtest-task`, `new-system`. Follow the matching one when doing that kind of work. Aura's always-on rules are in `.Aura/RULES.md`, to paste into Aura's Advanced Settings.

### Verify loop (every task)
1. `save_scene`, and commit if the next step is a batch operation (Aura only edits the open scene; `open_scene` discards unsaved changes).
2. Write or modify code → `check_compile_errors` → fix until clean → `get_unity_logs` (errors and warnings).
3. Scene/prefab work with Aura → `save_scene`.
4. EditMode tests for any pure logic touched.
5. Playtest: `run_play_mode_sequence` exercising the feature, then check the logs for the expected `[Heartbeat]` lines and capture a screenshot.
6. Commit on the task's feature branch.

`set_property` only reaches serialized fields: expose wiring as `[SerializeField] private`.

## Git (git-flow, AVH)
- `main` = releases, `develop` = integration. **One branch per backlog task**: `git flow feature start E07-floor-validator`. Finish into `develop` once verified.
- Milestone: `git flow release start H3` → tag `H3` → finish. Final tag `v1.0-demo`.
- Commit messages: imperative, prefixed with the epic: `E07: add LayoutValidator with 1000-seed test`.
- **Do not add `Co-Authored-By` or any AI attribution lines.** Local repository, do not push.
- LFS tracks images, audio, fonts, fbx/blend (see `.gitattributes`).

## Logging
Prefix runtime logs with `[Heartbeat][<System>]`. Playtests assert on these lines, so keep them stable. Debug-only logs go through `HbLog.Dev(...)` (stripped in release builds).
