---
description: Scaffold a new gameplay system (S-xx) following the project architecture - folder, namespace, balance ScriptableObject, pure model with EditMode tests, MonoBehaviour adapter and GameEvents entries. Use when starting any new system or component group.
argument-hint: "<System folder> <S-xx or M-xx id>"
---

# Scaffold a system

1. Read the system/mechanic in `GDD/02 - Diseño/03 - Sistemas.md` / `02 - Mecánicas.md` and the epic guide in `Desarrollo/`. List its rules and numbers.
2. Events: add the needed rows to `Desarrollo/EVENTS.md`, then to `Assets/_Project/Scripts/Core/GameEvents.cs` (event + `Raise` method + reset in `ResetAll`).
3. Balance: `Assets/_Project/Scripts/<System>/<System>Balance.cs` (`[CreateAssetMenu(menuName = "Heartbeat/Balance/<System>")]`), every number with a `[Tooltip]` citing the GDD id. Create the asset in `ScriptableObjects/Balance/` and reference it from `GameConfig`.
4. Pure model: `<Name>Model.cs`, plain C#, no UnityEngine dependencies beyond math types, deterministic (time passed in as a parameter).
5. Tests: `Assets/_Project/Tests/EditMode/<Name>ModelTests.cs` covering the GDD rules and the acceptance row.
6. Adapter: `<Name>.cs` MonoBehaviour with `[SerializeField] private` refs, reads input via `InputRouter`, raises events, logs `[Heartbeat][<System>]`.
7. `check_compile_errors` → run the EditMode tests → `get_unity_logs`.
8. Namespace `Heartbeat.<System>`, asmdef `Heartbeat.Runtime`. No singletons, no `Find*` in Update, no `Time.timeScale`.
