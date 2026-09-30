# Project Foundation — Requirements

Status: Done

## Objective

Establish the Unity project conventions, folder boundaries, bootstrap scene and test foundation without introducing gameplay beyond what later specs require.

## Requirements

## R1 - Project structure

WHEN project-owned assets are created THE SYSTEM SHALL place them under `Assets/_Project/` according to the steering structure.

## R2 - Input foundation

WHEN gameplay input is introduced THE PROJECT SHALL use Unity Input System rather than the legacy Input API.

## R3 - Test foundation

WHEN pure gameplay logic is introduced THE PROJECT SHALL provide an EditMode testing location and PlayMode testing location.

## R4 - Bootstrap

WHEN the demo starts THE PROJECT SHALL enter a deterministic bootstrap / entry flow whose exact first playable scene remains defined by game-flow requirements.

## R5 - Dependency hygiene

WHEN third-party assets are imported THE PROJECT SHALL keep them isolated from project-owned source code.

## Acceptance Rule

A requirement is complete only when its observable behavior is implemented and verified.

Unresolved gameplay decisions must remain `TBD` rather than being inferred.
