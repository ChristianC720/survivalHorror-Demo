# Inventory System — Requirements

Status: Draft

## Objective

Implement the confirmed six-slot inventory while separating item definitions from runtime state and UI.

## Requirements

## R1 - Capacity

WHEN the inventory initializes THE SYSTEM SHALL provide exactly six inventory slots.

## R2 - Add item

WHEN a valid item is acquired and a compatible slot is available THE SYSTEM SHALL add it to inventory.

## R3 - Full inventory

WHEN no compatible slot is available THE SYSTEM SHALL reject acquisition without destroying the world item.

## R4 - Remove item

WHEN an item is consumed, discarded, or otherwise removed THE SYSTEM SHALL free its occupied slot.

## R5 - Data separation

THE SYSTEM SHALL separate immutable/static item definition data from runtime item instances.

## R6 - UI independence

Inventory domain logic SHALL NOT depend on Unity UI.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
