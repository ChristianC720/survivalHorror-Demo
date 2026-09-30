---
inclusion: auto
name: unity-performance
description: Performance rules for profiling, pooling, physics, procedural generation, AI, rendering and frequently executed Unity code.
---

# Performance

Optimize based on measurement.

Avoid obviously pathological designs even before profiling.

## Avoid

- scene-wide searches every frame
- repeated `GetComponent` calls in hot loops
- avoidable LINQ allocations in per-frame code
- frequent instantiate/destroy cycles for reusable objects
- unnecessary physics queries
- uncontrolled garbage generation
- rebuilding procedural state without need

## Prefer

- cached references
- object pooling when reuse frequency justifies it
- spatial filtering
- event-driven updates
- controlled AI tick rates
- profiler-backed decisions

## Rule

Do not sacrifice architectural clarity for speculative micro-optimization.
