---
inclusion: always
---

# Project Structure

Gameplay code is organized by feature / domain rather than by technical type.

## Recommended Unity structure

```text
Assets/
├── _Project/
│   ├── Art/
│   ├── Audio/
│   ├── Materials/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Player/
│   │   ├── Camera/
│   │   ├── Interaction/
│   │   ├── Inventory/
│   │   ├── Combat/
│   │   ├── Enemies/
│   │   ├── ProceduralGeneration/
│   │   ├── Runs/
│   │   ├── Persistence/
│   │   ├── UI/
│   │   └── Presentation/
│   ├── ScriptableObjects/
│   ├── Settings/
│   │   ├── Input/
│   │   └── Rendering/
│   └── Tests/
│       ├── EditMode/
│       └── PlayMode/
└── ThirdParty/
```

Editor-only Unity tooling lives in `Assets/_Project/Scripts/Editor/`.

Repository-level automation (scripts that run outside Unity, e.g. `run-tests.ps1`) lives in `/Tools/` at the repository root.

Unity-generated output of that automation (test results, logs) goes to `/TestResults/`, which is not versioned.

## Ownership

Project-owned content belongs under `Assets/_Project/`.

Third-party content must remain isolated.

Do not modify third-party source code unless explicitly requested.

## Feature organization

A feature may contain:
- runtime implementation
- interfaces
- domain models
- configuration
- adapters
- tests

Avoid generic folders such as `Managers`, `Models`, or `Interfaces` unless they represent a real architectural boundary.

## Namespaces

Namespaces should follow project structure.

Examples:
- `Project.Player`
- `Project.Inventory`
- `Project.Combat`
- `Project.ProceduralGeneration`
- `Project.Runs`

## Assembly Definitions

Introduce assembly definitions only when they provide meaningful compile-time or dependency boundaries.

Avoid excessive fragmentation during early prototyping.

Current assemblies:
- `Project.Runtime` — `Assets/_Project/Scripts/`; must not reference `UnityEditor`.
- `Project.Editor` — `Assets/_Project/Scripts/Editor/`; Editor platform only.
- `Project.Tests.EditMode` — `Assets/_Project/Tests/EditMode/`.
- `Project.Tests.PlayMode` — `Assets/_Project/Tests/PlayMode/`.
