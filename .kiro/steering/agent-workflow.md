---
inclusion: always
---

# Agent Workflow

Before modifying the project:

1. Inspect the current implementation.
2. Identify affected systems and dependencies.
3. Read the active spec.
4. Preserve established architecture.
5. Make the smallest coherent change.
6. Compile.
7. Run relevant tests.
8. Review warnings and errors.
9. Validate acceptance criteria.

## Scope Control

Do not implement unrelated features.

Do not silently change approved game design.

Do not create speculative abstractions for hypothetical future requirements.

Do not change public APIs unnecessarily.

## Missing Requirements

When a requirement is undefined:
- mark it as `TBD`
- isolate the assumption
- avoid turning the assumption into permanent architecture

## Refactoring

Refactoring must preserve observable behavior unless behavior change is explicitly required.

## Completion Definition

A task is not complete merely because code exists.

Verify:
- compilation
- references
- tests
- scene / prefab integration when applicable
- acceptance criteria
- absence of new avoidable warnings
