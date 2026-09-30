---
inclusion: auto
name: camera-system
description: Rules for automatic isometric and over-the-shoulder camera behavior, transitions, framing and player orientation.
---

# Camera Design

The project may use:
- isometric exploration camera
- over-the-shoulder combat camera

## Primary Rule

Never abruptly change orientation in a way that causes the player to lose directional understanding.

## Transition Requirements

Transitions should preserve:
- player position awareness
- enemy awareness
- movement continuity
- input predictability

Use intentional interpolation rather than unexplained cuts unless scene design requires a cut.

## Input Consistency

Changing perspective must not silently redefine movement direction in a confusing way.

Movement must remain predictable during and immediately after transitions.

## Combat Perspective

Over-the-shoulder mode must activate from an explicit gameplay condition.

Do not continuously switch because an enemy briefly crosses a trigger.

Use a state machine, hysteresis, minimum-state duration, cooldown, or another explicit anti-oscillation rule.
