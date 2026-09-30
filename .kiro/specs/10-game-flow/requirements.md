# Game Flow — Requirements

Status: Draft

## Objective

Own the high-level lifecycle of entering a run, playing, dying, ending/restarting and returning to non-run states.

## Requirements

## R1 - Explicit states

THE APPLICATION SHALL represent high-level game flow with explicit states.

## R2 - Start run

WHEN a run begins THE SYSTEM SHALL initialize run-scoped systems in a deterministic order.

## R3 - Player death

WHEN player death is confirmed THE SYSTEM SHALL transition into the approved run-end/restart flow.

## R4 - Restart

WHEN a new run is started after death THE SYSTEM SHALL not leak mutable state from the previous run unless explicitly persisted.

## R5 - Ownership

Scene loading and run orchestration SHALL be owned by game flow rather than arbitrary gameplay components.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
