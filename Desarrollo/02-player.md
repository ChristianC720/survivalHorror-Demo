# E-02 Player

**Goal:** Lukas moves, crouches, runs, makes noise, tires and gets hurt, with input routed by context.

**GDD sources:** `02 - Diseño/04 - Controles.md`, M-05, M-06, M-09 in `02 - Mecánicas.md`, S-03/S-04 in `03 - Sistemas.md`, D-104, D-119, D-120.

## Deliverables
`Input/Heartbeat.inputactions` (+ generated C# wrapper via `generate_input_action_wrapper_code`), `Scripts/Input/InputRouter.cs`, `InputContext.cs`; `Scripts/Player/PlayerController.cs`, `PlayerMotor.cs`, `PlayerNoise.cs`, `PlayerPosture.cs`; `Scripts/Combat/HealthModel.cs` + `PlayerHealth.cs`; `Scripts/Player/StaminaModel.cs` + `PlayerStamina.cs`; `Balance/PlayerBalance.asset`; prefab `Characters/Player` with `Models/Characters/CH_Lukas_RH07.fbx` (`CH_Lukas_Civil.fbx` in the Intro).

## Tasks
### `E02-input`
1. Build the asset with Aura's input tools (`create_input_action_asset`, `add_action_map`, `add_action`, `add_bindings`, `add_composite_binding`, `add_control_scheme`): control schemes `KeyboardMouse`, `Gamepad`. Maps and actions exactly as the Controles table: Exploration (Move, Look, Sprint, Crouch, Interact, Flashlight, Surveillance, Terminal, Pause, Aim, Attack, Reload), Aiming (Look, Fire, Reload, StopAim…), Inventory, Hacking (Up/Down/Left/Right, Cancel), Surveillance (Exit, small Move), OverBody (Stomp, Spike), Menu (Navigate, Submit, Cancel), Cinematic (Skip). No jump, no dodge. No duplicate binding inside one context.
2. `InputRouter.SetContext(InputContext)` enables only that map (plus Pause where valid), raises `InputContextChanged`. Gameplay reads actions through the router, never through the devices.
3. Rebinding support (keyboard and mouse only, D-120): save overrides JSON through `SettingsService`.

### `E02-player-motor`
1. `CH_Lukas_RH07` (C-60, see PLACEHOLDERS.md) inside the `Player` prefab: `CharacterController` (r 0.35, h 1.8), `PlayerController`, layer `Player`, tag `Player`.
2. Speeds from `PlayerBalance`: crouch 1, walk 1.8, run 4.5 m/s; Injured −15 %; Critical drags (value in the GDD). Camera-relative movement. Gravity, step offset 0.3.
3. `PlayerNoise`: crouched = 0 noise; walking a small radius; running and shooting alert (radii from balance). Raises `NoiseEmitted`.
4. `PlayerPosture`: drives a placeholder posture cue (lean/scale on the PH mesh, later the animator) from the health state.

### `E02-stamina-health`
- `StaminaModel` (pure): one bar for running, shoving and finishing. `TrySpend(amount)`, regen delay and rate, 2 s exhaustion at 0, raises `StaminaExhausted`. Shove costs 35 % (acceptance M-06).
- `HealthModel` (pure): `Fine → Injured → Critical → Dead`, no regeneration, hits by enemy type from `EnemyDefinition` (Alto kills in 2 hits, Rastrero in 4, Boca 3, Mudo 2). Heal restores one state (in Fine with damage → full). The heal animation is cancelled by a hit and does not consume the item.
- EditMode tests for both.

## Acceptance
- M-05: hit by an Alto in Fine → Injured, 15 % slower, no recovery after 60 s.
- M-06: stamina 30 %, shove (35 %) → no shove + `CUE_Lukas_Gasp`.
- M-09 (partial): crouched motion emits no `NoiseEmitted`.
