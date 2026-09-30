---
inclusion: auto
name: gameplay-architecture
description: Architectural rules for implementing or refactoring gameplay systems and communication between Unity features.
---

# Gameplay Architecture

## Principle

Gameplay features should be independently understandable and loosely coupled.

Prefer explicit feature boundaries over cross-feature implementation access.

## Communication

Use direct references when:
- ownership is clear
- the dependency is local
- lifecycle is obvious

Use events when:
- multiple systems react to the same occurrence
- the publisher should not know subscribers

Use ScriptableObject event channels only when they solve a concrete decoupling need.

Do not introduce a global event bus without a specific architectural reason.

## Configuration vs Runtime State

ScriptableObjects are primarily assets and configuration.

Do not store mutable run-specific state in persistent ScriptableObject assets unless the lifecycle and reset behavior are explicitly controlled.

Example separation:

```text
ItemDefinition (asset configuration)
        ↓
InventoryItem (runtime instance)
        ↓
Inventory (runtime owner)
```

## Managers

Do not create a manager merely because several objects use a system.

Before introducing a manager/service, define:
- ownership
- lifetime
- responsibility
- dependencies
- public API
