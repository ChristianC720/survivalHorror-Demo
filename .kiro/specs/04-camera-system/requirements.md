# Camera System — Requirements

Status: Draft

## Objective

Implement stable transitions between isometric exploration and over-the-shoulder combat framing without confusing player orientation.

## Requirements

## R1 - Exploration

WHEN the player is in exploration camera state THE CAMERA SHALL use the approved isometric framing.

## R2 - Combat

WHEN an explicit combat-camera condition is satisfied THE CAMERA SHALL transition to the approved over-the-shoulder framing.

## R3 - Stability

WHEN the triggering condition fluctuates briefly THE CAMERA SHALL avoid rapid repeated perspective switching.

## R4 - Continuity

WHEN perspective changes THE SYSTEM SHALL preserve movement predictability and spatial awareness.

## R5 - Recovery

WHEN combat-camera conditions end THE CAMERA SHALL transition back using an explicit state rule.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
