# Vertical Slice Integration — Requirements

Status: Draft

## Objective

Integrate the approved demo systems into a coherent playable experience and verify the actual end-to-end player loop.

## Requirements

## R1 - Playable loop

THE DEMO SHALL provide a complete playable loop from entry into a run through the currently approved completion or defeat state.

## R2 - Systems integration

The vertical slice SHALL integrate player movement, interaction, camera, inventory, damage/death, combat, enemy AI, procedural rooms and run flow to the extent required by the approved demo.

## R3 - Restart integrity

WHEN the player starts another run THE DEMO SHALL reset run-scoped state correctly.

## R4 - Debuggability

The demo build SHALL expose sufficient logs/diagnostics in development builds to reproduce critical run-generation or lifecycle defects.

## R5 - Quality gate

The slice SHALL compile without blocking errors and SHALL pass the agreed acceptance checklist before being considered complete.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
