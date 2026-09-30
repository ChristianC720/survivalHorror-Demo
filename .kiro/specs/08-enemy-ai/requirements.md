# Enemy AI — Requirements

Status: Draft

## Objective

Provide a deterministic, debuggable enemy behavior foundation that can support stronger-than-player threats without building unnecessary complexity.

## Requirements

## R1 - State model

THE ENEMY SHALL use an explicit behavior/state model rather than unrelated boolean flags.

## R2 - Perception

WHEN the player satisfies the approved perception rules THE ENEMY SHALL transition to the appropriate state.

## R3 - Loss of target

WHEN perception conditions cease THE ENEMY SHALL follow an explicit loss-of-target rule.

## R4 - Performance

Enemy sensing and decision logic SHALL avoid unnecessary per-frame scene-wide searches.

## R5 - Combat integration

WHEN an enemy attack resolves THE ENEMY SHALL use the shared damage system.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
