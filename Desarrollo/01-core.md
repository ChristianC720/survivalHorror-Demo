# E-01 Core

**Goal:** the persistent backbone: service registry, event bus, game state machine, scene loading and pause.

**GDD sources:** `07 - Tecnología/01 - Arquitectura Técnica.md` (D-107, D-108), `02 - Diseño/07 - Reglas y Estados del Juego.md` (states, transitions, global rules), `06 - Experiencia/01 - UX y Flujo del Jugador.md`.

## Deliverables
`Scripts/Core/`: `Bootstrap.cs`, `Services.cs`, `GameEvents.cs` (see `EVENTS.md`), `GameStateMachine.cs` + `GameStateId.cs` + state classes, `SceneLoader.cs`, `PauseService.cs`, `HbLog.cs`, `GameConfig.cs` (root SO referencing every Balance SO + active preset).
Scenes: `Bootstrap`, `MainMenu`, `Intro`, `Floor`, `Cinematics` in `_Project/Scenes`, in that build order.

## Tasks
### `E01-core`
1. `Services`: static `Register<T>(T)`, `Get<T>()`, `TryGet<T>()`, `Clear()`. Only `Bootstrap` registers. This is the single allowed global.
2. `Bootstrap` (in the `Bootstrap` scene, `DontDestroyOnLoad`): creates and registers `GameStateMachine`, `SceneLoader`, `SaveService`, `SettingsService`, `AudioService`, `InputRouter`, `PauseService`, `EventSystem`. Calls `GameEvents.ResetAll()` in `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]`.
3. **Editor convenience:** `BootstrapLoader` with `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` loads `Bootstrap` additively when you press Play in any other scene, so `Floor` or `Sandbox_*` can be played directly.
4. `GameStateMachine`: states from Reglas y Estados del Juego. Top level: `Boot, BrightnessCalibration, Title, MainMenu, Intro, Loading, Playing, Paused, FloorTransition, Death, Reconstruction, Victory, FinalDecision, Ending, Credits`. Playing sub-states: `Exploring, Inventory, Hacking, Surveillance, Cinematic`. Each state: `Enter/Exit/Tick`, allowed transitions as a table, invalid transition = error log. Raises `GameStateChanged`. Pure C# core (`StateMachineCore<T>`) with EditMode tests.
5. `SceneLoader`: async load/unload with a fade, never shows a loading screen during floor transitions (M-15: transition ≥ 5 s, staged as a cinematic).

### `E01-pause`
`PauseService` is the only code that writes `Time.timeScale`. Pause is allowed only from `Playing`. Pause menu (built in E-10) has Resume / Options / Restart (counts as death, confirmation) / Quit to menu (confirmation; quitting during an alert or hunt counts as a full death, see the exit table in Reglas y Estados). In the intro, pause has no Restart (D-63).

## Acceptance
- Play from `Floor` directly → Bootstrap loads, `[Heartbeat][Core] Services ready` logged.
- EditMode: state-machine transition table tests (valid and invalid).
- Esc toggles pause; `Time.timeScale` is 0 only while paused; opening the inventory leaves it at 1.
