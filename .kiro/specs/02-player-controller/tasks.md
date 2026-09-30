# Player Controller — Tasks

Status: Done

## Implementation Checklist

- [x] 1. Define movement data/configuration
- [x] 2. Implement input adapter
- [x] 3. Implement movement motor
- [x] 4. Integrate collision/physics solution
- [x] 5. Expose locomotion state needed by camera/animation
- [x] 6. Add movement tests where practical
- [x] 7. Validate movement in target scene

## Definition of Done

- [x] Code compiles.
- [x] No new avoidable warnings are introduced.
- [x] Relevant tests pass.
- [x] Scene/prefab references are valid where applicable.
- [x] Requirements acceptance criteria are verified.
- [x] No unrelated feature scope was added.
- [x] Any remaining unknowns are explicitly marked `TBD`.

## Verification Record

Batchmode via `Tools/run-tests.ps1` (project assemblies only): EditMode 93/93, PlayMode 18/18. No `CS` warnings or errors.

Not yet verified by hand in the interactive editor: movement feel and camera placement.

| Req | Evidence |
|---|---|
| R1 | `PlayerMotorTests` (walk/run speeds, acceleration, deceleration, facing); end-to-end W, W+Shift, W+D, release tests. |
| R2 | Motor at 30/60/144 fps: constant speed ≤0.5%, acceleration/deceleration ≤1%, steady speed ≤0.5%, facing ≤1°, diagonal ≤0.1%. End-to-end distance ≤2%. |
| R3 | `ReferenceFrameTests` (locked when held, resampled on neutral or a >30° change); end-to-end camera-turn test. |
| R4 | `PlayerMoveInputTests` and end-to-end tests with `InputTestFixture`. Input is injected by `SceneEntryPoint`, with no global access. Direct Sandbox play logs one error and the player stays disabled. |
| R5 | `PlayerController` exposes `Velocity`, `Speed`, `IsMoving`, `IsGrounded`, `Facing` (`PlayerControllerPlayModeTests`). |

## Remaining TBD

- Final tuning values (current values are provisional).
- Gamepad, arrow keys, slopes and stairs.
- Real camera reference frame (Spec 04). The Sandbox camera is a placeholder.
