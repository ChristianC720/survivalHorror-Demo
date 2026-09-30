---
inclusion: always
---

# Scope Control

The current production target is a playable vertical slice.

Every proposed feature or enhancement should be classified as:

- **MUST** — required for the demo to function.
- **SHOULD** — strongly improves the demo but is not required for basic completion.
- **COULD** — useful enhancement if schedule permits.
- **OUT** — outside current demo scope.

When implementing a MUST feature:
- do not automatically implement SHOULD or COULD work
- avoid generalized systems for hypothetical post-demo requirements
- prefer the simplest maintainable architecture that supports confirmed needs
- do not reduce code quality in the name of MVP scope

If a task materially increases schedule risk, identify the risk before expanding scope.
