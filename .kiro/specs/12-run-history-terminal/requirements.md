# Run History Terminal — Requirements

Status: Draft

## Objective

Provide an interactable terminal capable of presenting approved information from previous runs without coupling UI to raw persistence storage.

## Requirements

## R1 - Interaction

WHEN the player interacts with the terminal THE SYSTEM SHALL open the approved terminal presentation.

## R2 - Data source

THE TERMINAL SHALL request historical run data through an explicit read contract.

## R3 - Missing data

WHEN no run history exists THE TERMINAL SHALL present a valid empty state.

## R4 - Decoupling

Terminal UI SHALL NOT directly parse or own persistence files.

## R5 - Unknown content

The exact fields, narrative wording and amount of run information displayed SHALL remain `TBD` until approved.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
