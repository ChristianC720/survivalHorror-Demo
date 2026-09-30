# Combat System — Technical Design

Status: Draft

## Design Goal

Implement the minimum combat loop required for the demo while preserving player vulnerability and avoiding assumptions about unconfirmed weapons.

## Architectural Boundaries

- Keep domain/gameplay logic independent from presentation where practical.
- Use explicit ownership and dependencies.
- Do not use scene searches as a normal dependency mechanism.
- Do not store unrelated global mutable state.
- Integrate with other features through explicit contracts/events.

## Proposed Components

- Combat state/controller
- Attack abstraction
- Target/damage resolution
- Combat-camera signal
- Weapon-specific implementations only after approval

## Data and State

State ownership must be explicit.

Persisted state, run-scoped state and configuration assets must not be conflated.

## Integration Constraints

- Follow `.kiro/steering/tech.md`.
- Follow `.kiro/steering/structure.md`.
- Follow `.kiro/steering/csharp-standards.md` for C# files.
- Follow any specialized steering automatically applicable to this feature.

## Error / Invalid-State Handling

Invalid configuration should fail explicitly during development rather than produce silent undefined behavior.

## Testing Strategy

Prefer EditMode tests for pure logic and PlayMode tests for Unity lifecycle, physics, camera, input and scene integration.

## Open Decisions

Any detail not present in `requirements.md` or the approved GDD is `TBD`.
