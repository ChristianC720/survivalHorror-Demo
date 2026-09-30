---
inclusion: auto
name: persistence
description: Persistence, run history, save-data versioning and clone-respawn data lifecycle rules.
---

# Persistence and Run Data

## Separation

Keep persistent profile data separate from current-run transient state.

## Current Run

Run-specific state may include:
- seed
- generated layout identity
- player runtime state
- collected resources
- run outcome

Exact persisted fields remain subject to the active feature spec.

## Versioning

Persistent formats should expose a version when the demo begins storing data across application sessions.

## Failure Handling

Corrupt or incompatible save data must fail safely.

Do not silently overwrite recoverable user data without an explicit migration / fallback strategy.

## Narrative Terminal

The terminal may read historical run information, but presentation requirements remain defined by its spec and GDD.
