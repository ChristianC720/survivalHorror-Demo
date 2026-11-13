# E-06 Inventory and interaction (S-05, M-07, M-08, M-12, M-13, M-14)

**Goal:** a 6-slot real-time inventory, interaction with `E`, documents registered in the arm terminal, the flashlight, sequence hacking and sealable doors.

**GDD sources:** M-07, M-08, M-12, M-13, M-14, S-05; `04 - Contenido/04 - Objetos y Coleccionables.md`; `06 - Economía y Recursos.md`.

## Deliverables
`Scripts/Inventory/InventoryModel.cs` (pure), `PlayerInventory.cs`, `ItemDefinition.cs` SO + `Items/ID_*.asset`; `Scripts/Interaction/Interactor.cs` (sphere/raycast probe, `QueryTriggerInteraction.Ignore`), `IInteractable.cs`, `WorldItem.cs`, `DocumentPickup.cs`, `ConfiscatedObject.cs`; `Scripts/Narrative/Registry.cs` (terminal records: documents, Emilia docs n/6, confiscated, bestiary, rules, faults); `Scripts/Player/Flashlight.cs` + `FlashlightModel.cs`; `Scripts/Level/HackPanel.cs` + `HackSequence.cs` (pure); `Scripts/Level/DoorLeaf.cs`, `SealableDoor.cs`.

## Rules (start values)
- 6 slots. Per-slot caps: Heal 1, Ammo 12 rounds (picked up in blisters of 4), Spike 1, Key 1. Documents, ring and confiscated objects take no slot.
- Full inventory: a key can replace another item (prompt). Discard drops the item to the floor.
- Using an item never pauses (heal ~1.5 s, cancelled by a hit without consuming).
- Flashlight: ~120 s of charge; recharges after ~3 s off, 0→100 % in ~30 s; at 0 it flickers, turns off and is locked ~10 s (D-124).
- Hacking: direction sequences of 2–3 (quick), 4–5 (door / panel), 7–8 (hard). One mistake restarts the sequence with an error beep. Lukas cannot move. Never pauses.
- Sealing (C-59): on a closed door with no key lock, with power and outside the tutorial, a tap of Interact (< `sealTapSeconds` 0.25 s) opens it and holding Interact `sealHoldSeconds` 2 s seals it; releasing in between cancels. The prompt shows both options and a progress bar. The hold cancels when the door stops being the target, Lukas is hit or the door changes state; Lukas can move. Logic in `Interaction/HoldInteraction.cs` (pure) and `IHoldInteractable` on `DoorLeaf` (no longer an `IHackTarget`); values in `SystemsBalance`. A hunting enemy breaks the seal in `sealBreakSeconds` 30 s. The seal also breaks if Lukas crosses back. Telemetry logs `door_sealed`.

## Tasks
`E06-inventory` · `E06-interaction` · `E06-documents-registry` · `E06-flashlight` · `E06-hacking` · `E06-doors-seal`. Each pure model gets EditMode tests.

## Acceptance
- M-07: inventory open with a hunting enemy, heal (~1.5 s) → the enemy keeps attacking; if it hits during the animation the heal is not consumed.
- M-08: at ~1.5 m from an Emilia document, `E` → `REGISTERED`, the file count goes to n/6, and the game never pauses.
- M-12, M-13, M-14 rows.
