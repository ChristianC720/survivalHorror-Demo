---
inclusion: fileMatch
fileMatchPattern: "Assets/**/*.cs"
---

# C# Development Standards

## General

Use modern idiomatic C#.

Prefer:
- `readonly` where possible
- explicit access modifiers
- small cohesive classes
- composition
- immutable data where practical
- explicit dependency injection
- cancellation-aware async code when asynchronous operations exist

Avoid:
- public mutable fields
- unnecessary singleton patterns
- Service Locator
- global mutable state
- reflection for ordinary gameplay logic

## Unity Serialization

Prefer:

```csharp
[SerializeField] private float _movementSpeed;
```

over public mutable fields.

Expose data publicly only when it is part of an intentional API.

## MonoBehaviour

`MonoBehaviour` should primarily handle Unity lifecycle and scene integration.

Move substantial pure gameplay/domain logic into plain C# classes when practical.

## Update Loops

Do not add `Update`, `FixedUpdate`, or `LateUpdate` without confirming that per-frame execution is required.

Avoid allocations in hot paths.

## Dependency Resolution

Prefer serialized references, explicit construction, factories, or explicit ownership.

Do not use scene searches as normal dependency resolution.

## Naming

PascalCase:
- types
- methods
- properties
- public members

camelCase:
- parameters
- local variables

`_camelCase`:
- private fields

Interfaces use an `I` prefix.

## Error Handling

Fail explicitly when invalid configuration makes a system unusable.

Do not silently swallow exceptions or impossible states.

Use assertions or validation for development-time invariants where appropriate.
