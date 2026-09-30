# Kiro Project Configuration

This folder contains the operational context used by Kiro for the Unity survival-horror / roguelite demo.

## Purpose

The project separates three kinds of authority:

1. **Game Design Document (GDD)** — authoritative source for approved game design, narrative and player experience.
2. **Steering** — rules that define how Kiro should reason, structure and implement changes.
3. **Specs** — executable feature contracts containing requirements, technical design and implementation tasks.

## Rules

- Steering files must not invent gameplay or narrative decisions.
- Missing design information must remain `TBD` until explicitly approved.
- Specs must remain feature-scoped.
- A spec is not considered complete until its acceptance criteria are verified.
- The current production target is a playable PC demo / vertical slice.

## Structure

```text
.kiro/
├── steering/
└── specs/
```

## Recommended workflow

```text
GDD decision
    ↓
Feature requirements
    ↓
Technical design
    ↓
Implementation tasks
    ↓
Code + tests
    ↓
Acceptance validation
```
