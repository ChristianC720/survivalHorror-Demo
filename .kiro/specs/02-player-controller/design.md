# Player Controller — Technical Design

Status: Done

## Design Goal

Implement predictable player locomotion suitable for exploration and later combat/camera integration.

## Architectural Boundaries

- Keep domain/gameplay logic independent from presentation where practical.
- Use explicit ownership and dependencies.
- Do not use scene searches as a normal dependency mechanism.
- Do not store unrelated global mutable state.
- Integrate with other features through explicit contracts/events.

## Components (`Project.Player`, assembly `Project.Runtime`)

- `MovementSettings` — ScriptableObject with all tuning values and `Validate()`.
- `PlayerMotor` — plain C#. Converts `MotorInput`, `MovementFrame`, grounded state and `deltaTime` into a `MotorResult` (displacement, velocity, facing). No dependency on Input System or `CharacterController`. Accepts an optional facing target so camera/combat can override facing later.
- `IMovementReferenceFrame` / `TransformReferenceFrame` — supplies the camera's flattened forward/right. No `Camera.main`.
- `ReferenceFrameLock` — keeps the captured frame while input intent is unchanged.
- `IPlayerMoveInput` / `PlayerMoveInput` — read-only adapter over `ProjectInputActions.Player` (`Move`, `Run`).
- `PlayerController` — thin `MonoBehaviour` driving the `CharacterController`; one `Update`; disabled until `Initialize`.

## Scene wiring (`Project.Core.Scenes.SceneEntryPoint`)

- Each gameplay scene has exactly one root-level `SceneEntryPoint`.
- `ProjectBootstrap` subscribes to `SceneManager.sceneLoaded`, finds the single entry point among the loaded scene's root objects and calls `Initialize(input)`.
- The entry point builds the adapter and reference frame, initializes the player and enables the `Player` action map. It disables the map when destroyed.
- Playing a gameplay scene without Bootstrap logs one error and leaves the player disabled. No fallback input.

## Integration

Displacement uses the average velocity over each step, split at the instant speed reaches its target. This is exact for constant acceleration and keeps results frame-rate independent.

## Data and State

`MovementSettings` is configuration only. Runtime state lives in `PlayerMotor`, `ReferenceFrameLock` and `PlayerController`.

## Integration Constraints

- Follow `.kiro/steering/tech.md`.
- Follow `.kiro/steering/structure.md`.
- Follow `.kiro/steering/csharp-standards.md` for C# files.
- Follow `.kiro/steering/camera-system.md` for reference-frame behavior.

## Error / Invalid-State Handling

Invalid `MovementSettings` throws in `Validate()` with the field name. Missing entry-point references throw on `Initialize`.

## Testing Strategy

EditMode: settings validation, motor, facing, gravity, reference frame and lock, input asset.
PlayMode: controller with fake input, adapter with `InputTestFixture`, end-to-end Bootstrap → Sandbox keypress tests.

## Open Decisions

Final tuning values, gamepad, arrow keys, slopes and stairs are `TBD`.
