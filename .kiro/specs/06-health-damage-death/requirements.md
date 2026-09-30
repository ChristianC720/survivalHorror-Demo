# Health, Damage and Death — Requirements

Status: Draft

## Objective

Define reusable player/enemy damage handling and connect player death to the run lifecycle without embedding respawn logic in health components.

## Requirements

## R1 - Damage

WHEN valid damage is applied THE TARGET SHALL reduce health according to approved damage rules.

## R2 - Death state

WHEN health reaches the death threshold THE TARGET SHALL enter death exactly once.

## R3 - Player death

WHEN the player dies THE SYSTEM SHALL notify game-flow/run systems rather than directly reloading arbitrary scenes.

## R4 - Validation

WHEN invalid negative damage or impossible health configuration is supplied THE SYSTEM SHALL reject or explicitly validate it.

## R5 - Decoupling

Health logic SHALL NOT depend directly on UI, cloning, procedural generation, or scene loading.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
