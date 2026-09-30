# Spec Dependency Map

The recommended implementation sequence is dependency-driven rather than strictly linear.

```text
01 Project Foundation
    │
    ├── 02 Player Controller
    │      ├── 03 Interaction System
    │      └── 04 Camera System
    │
    ├── 05 Inventory System
    │
    └── 06 Health / Damage / Death
           │
           ├── 07 Combat System
           │      └── 08 Enemy AI
           │
           └── 10 Game Flow
                   │
                   ├── 09 Procedural Room Generation
                   ├── 11 Clone Respawn System
                   │      └── 12 Run History Terminal
                   └── 13 Settings Menu (mostly independent)

14 Visual Presentation can evolve in parallel after the core loop is stable.

15 Vertical Slice integrates all MUST systems.
```

## Execution Guidance

Do not start a dependent spec if its required contract is still unstable unless the work can be isolated safely.
