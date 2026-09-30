---
inclusion: auto
name: testing
description: Testing standards for gameplay systems, procedural generation, inventory, combat, run logic and regression fixes.
---

# Testing Strategy

Use:
- `Assets/_Project/Tests/EditMode/`
- `Assets/_Project/Tests/PlayMode/`

## EditMode

Prefer EditMode tests for:
- inventory rules
- damage calculations
- generation algorithms
- seeded procedural behavior
- state machines
- run-state logic
- validators
- pure domain logic

## PlayMode

Use PlayMode tests for:
- GameObject lifecycle
- scene integration
- physics
- animation integration
- camera behavior
- Input System integration

## Regression Rule

When fixing a reproducible defect:
1. reproduce it
2. add a regression test when practical
3. implement the fix
4. verify existing behavior
