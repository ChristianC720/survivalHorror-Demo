---
inclusion: always
---

# Technology Stack

## Engine

Unity 6.x. The exact editor version is recorded in `ProjectSettings/ProjectVersion.txt`; package versions in `Packages/packages-lock.json`.

Enter Play Mode Options are disabled, so Domain Reload and Scene Reload stay enabled. Do not add static-state reset infrastructure unless Domain Reload is deliberately disabled later.

Do not introduce deprecated Unity APIs or dependencies incompatible with the installed project version.

## Language

C#.

## Runtime Principles

Prefer:
- modern C#
- strong typing
- explicit state ownership
- composition over inheritance
- event-driven communication where appropriate
- ScriptableObjects for configuration/data where appropriate
- dependency inversion between gameplay systems

Avoid:
- global mutable state
- excessive static classes
- `GameObject.Find`
- runtime scene-wide lookup as dependency resolution
- `SendMessage`
- string-based messaging
- deep `MonoBehaviour` inheritance trees
- generic god-managers
- hidden runtime dependencies

## Input

Use Unity Input System. Active Input Handling is set to the Input System only.

Do not implement new gameplay using the legacy Input API.

Input actions live in `Assets/_Project/Settings/Input/ProjectInputActions.inputactions`, with a generated `Project.Core.Input.ProjectInputActions` wrapper. The wrapper is owned by `ProjectBootstrap` and passed explicitly to consumers. Do not register project-wide actions or use `InputSystem.actions` as a global lookup.

Each gameplay scene has exactly one root-level `SceneEntryPoint`. `ProjectBootstrap` hands it the input after the scene loads; the entry point enables the `Player` map after successful initialization and disables it on teardown. Gameplay components only read injected input and never enable or disable maps.

Bound actions: `Player/Move` (WASD) and `Player/Run` (Left Shift, held).

The generated wrapper only regenerates when scripts compile. When adding actions, add them to the asset before writing code that uses the new generated members.

Input System test utilities (`InputTestFixture`) are exposed through manifest `testables` and referenced only by `Project.Tests.PlayMode`. `Tools/run-tests.ps1` runs project test assemblies only.

Control schemes: Keyboard & Mouse. Gamepad support is `TBD`.

## Physics

Gameplay logic must not depend on frame rate.

Use the correct timestep for physics-sensitive behavior.

## Version Control

Use Git + Git LFS. Unity Version Control (Plastic / collab-proxy) is not used.

Use Git LFS for large binary assets. Tracked patterns and LF line-ending rules are defined in `.gitattributes`.

Unity uses Visible Meta Files and Force Text asset serialization.

Never commit:
- `Library/`
- `Temp/`
- `Logs/`
- `Obj/`
- `UserSettings/`

Commit Unity `.meta` files.

## Target Platform

Primary target:
- PC (Windows Standalone). The only quality level is `PC`, which uses the URP asset in `Assets/_Project/Settings/Rendering/`.

Other platforms are outside demo scope unless explicitly approved.

## Performance Philosophy

Avoid premature optimization, but do not create obviously pathological runtime designs.

Profile before performing complex optimization.
