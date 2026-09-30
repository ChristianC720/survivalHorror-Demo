# Player Controller — Requirements

Status: Done

## Objective

Implement predictable player locomotion suitable for exploration and later combat/camera integration.

## Approved Movement Model

- Camera-relative movement: directional input maps onto the camera's flattened forward/right axes.
- Keyboard: WASD. Diagonal input is normalized.
- Walk (default, slow-paced) and Run (held with Left Shift). No stamina.
- The character turns toward its movement direction at a limited turn speed.
- Gravity with ground snapping on flat floors.

### Provisional tuning (not GDD canon)

These are initial values stored in `MovementSettings`. They are editable and must not be treated as gameplay constants.

| Setting | Value |
|---|---|
| Walk speed | 2.5 m/s |
| Run speed | 4.5 m/s |
| Acceleration | 18 m/s² |
| Deceleration | 24 m/s² |
| Turn speed | 540 °/s |
| Gravity | −20 m/s² |
| Grounded velocity | −2 m/s |
| Input deadzone | 0.1 |
| Direction-change threshold | 30° |
| Capsule height / radius | 1.8 m / 0.35 m |
| Capsule center | (0, 0.9, 0) |
| Step offset / skin width | 0.2 m / 0.035 m |

## Requirements

## R1 - Movement

WHEN directional input is provided THE PLAYER SHALL move relative to the current movement reference frame at walk speed, or at run speed while Run is held, accelerating and decelerating at the configured rates and turning toward the movement direction.

## R2 - Frame independence

WHEN frame rate changes THE PLAYER SHALL preserve equivalent movement speed within expected tolerances.

Tolerances: constant-speed distance ≤ 0.5%; acceleration/deceleration ≤ 1%; steady speed ≤ 0.5%; facing ≤ 1°; diagonal magnitude ≤ 0.1%; PlayMode end-to-end distance ≤ 2%.

## R3 - Camera compatibility

WHEN the camera changes between approved perspectives THE PLAYER SHALL preserve predictable directional control.

The reference frame captured when movement starts stays locked while input direction stays within the direction-change threshold. Neutral input (below the deadzone) or a larger direction change resamples the current frame.

## R4 - Input source

WHEN movement input is read THE PLAYER SHALL use Unity Input System actions injected through the scene entry point. No global or static input access.

## R5 - State exposure

WHEN other gameplay systems need player locomotion state THE PLAYER CONTROLLER SHALL expose it through an intentional API (`Velocity`, `Speed`, `IsMoving`, `IsGrounded`, `Facing`) rather than scene searches.

## Out of scope / TBD

- OUT: jump, crouch, stamina, sprint burst, aiming, animation, locomotion events.
- TBD: gamepad, slopes and stairs, final tuning values.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
