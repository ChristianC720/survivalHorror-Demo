# Combat System — Requirements

Status: Draft

## Objective

Implement the minimum combat loop required for the demo while preserving player vulnerability and avoiding assumptions about unconfirmed weapons.

## Requirements

## R1 - Enter combat

WHEN approved combat input/state conditions are met THE SYSTEM SHALL allow the player to perform the currently approved combat action set.

## R2 - Risk

THE COMBAT SYSTEM SHALL preserve the design principle that enemies are generally more dangerous than the player.

## R3 - Damage integration

WHEN an attack successfully resolves against a damageable target THE SYSTEM SHALL route damage through the health/damage contract.

## R4 - Camera integration

WHEN combat requires over-the-shoulder framing THE COMBAT SYSTEM SHALL signal camera state through an explicit contract rather than manipulate camera internals.

## R5 - Unknown content

Weapons, ammo rules, aiming model, reload behavior and hit rules not approved by the GDD SHALL remain `TBD`.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
