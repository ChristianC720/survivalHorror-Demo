---
inclusion: auto
name: procedural-generation
description: Rules for procedural rooms, key rooms, connectivity, seeds, generation validation and reproducibility.
---

# Procedural Generation

Generation should be deterministic for a given seed whenever practical.

## Required Properties

Every generated run must:
- contain a valid starting room
- maintain valid connectivity
- include required key rooms
- preserve progression-critical paths
- prevent unreachable mandatory content
- expose enough diagnostics to reproduce defects

## Architecture

Separate:
1. room definition data
2. generation algorithm
3. generation validation
4. scene / prefab instantiation
5. runtime room state

The generation algorithm should avoid direct dependence on instantiated GameObjects where practical.

## Validation Pipeline

Preferred flow:

```text
Generate
  ↓
Validate
  ↓
Accept
```

If generation can fail:

```text
Generate
  ↓
Validate
  ↓
Retry / Repair
  ↓
Accept
```

Never assume random output is valid.

## Seed

Expose the run seed for debugging and reproduction.
