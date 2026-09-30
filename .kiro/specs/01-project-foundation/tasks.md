# Project Foundation — Tasks

Status: Done

## Implementation Checklist

- [x] 1. Create project-owned folder structure
- [x] 2. Configure Input System foundation
- [x] 3. Create bootstrap / entry scene
- [x] 4. Create tests folder structure
- [x] 5. Configure serialization / version-control-safe project settings
- [x] 6. Verify clean compilation and no avoidable warnings

## Definition of Done

- [x] Code compiles.
- [x] No new avoidable warnings are introduced.
- [x] Relevant tests pass.
- [x] Scene/prefab references are valid where applicable.
- [x] Requirements acceptance criteria are verified.
- [x] No unrelated feature scope was added.
- [x] Any remaining unknowns are explicitly marked `TBD`.

## Verification Record

Unity 6000.6.3f1, Input System 1.20.0, Test Framework 1.8.0, URP 17.6.0.

- Batchmode via `Tools/run-tests.ps1`: EditMode 26/26, PlayMode 4/4, no `CS` warnings, no missing-reference or render-pipeline errors.
- The user manually verified that the project opens in the editor and the Console shows no errors.

| Req | Evidence |
|---|---|
| R1 | Project content under `Assets/_Project/`; `structure.md` updated. |
| R2 | Active Input Handling = Input System only; `ProjectInputActions` asset and generated wrapper; no project-wide actions (`ProjectInputActionsTests`). |
| R3 | `Tests/EditMode` and `Tests/PlayMode` asmdefs; boundaries checked by `AssemblyHygieneTests`. |
| R4 | `Bootstrap` at build index 0 loads `Sandbox` (`BootstrapSceneConfigurationTests`, `ProjectBootstrapPlayModeTests`); exactly one composition root, duplicates destroyed. |
| R5 | Asset packs isolated in `Assets/ThirdParty/` with GUIDs preserved. |

## Remaining TBD / follow-ups

- First playable scene: `TBD` (Spec 10). `Sandbox` is temporary.
- Gamepad control scheme: `TBD`.
- Git remote not attached: `github.com/ChristianC720/survivalHorror-Demo` requires authentication, state unverified.
- First commit: to be made by the user.
- Run `git lfs install` in the repository before the first push.
