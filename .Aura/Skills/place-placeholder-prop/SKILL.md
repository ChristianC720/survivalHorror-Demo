---
description: Create a PH_ (Blender placeholder) or PROP_ prefab from Unity primitives at real scale with the pivot at the floor center, and register placeholders in Desarrollo/PLACEHOLDERS.md. Use whenever a character, enemy, boss part or prop model is needed.
argument-hint: "<prefab name, e.g. PH_F1_Boiler or PH_Alto>"
---

# Placeholder or prop prefab

1. Look up the object in `Desarrollo/OBJECT_CATALOG.md` (size, cover, kind) or `Desarrollo/PLACEHOLDERS.md` (characters/boss). If it is missing, add a row first.
2. In `Sandbox_Kit` (save the current scene first), create root `<Name>` at (0,0,0), scale 1. Pivot = floor center of the footprint (characters: between the feet, facing +Z).
3. Child `Mesh` holding primitives that match the real size and a readable silhouette (e.g. the heal kit's handle, the Alto's long arms). Add a small cube on the +Z side of characters to show facing.
4. Colours from the palette (`Desarrollo/12-art-render.md`). Use shared materials; do not create one material per prop.
5. Colliders: one simple collider on the root sized to the footprint. Cover props: collider ≥ 1.0 m high + a `Cover` marker child.
6. For `PH_`: add `PlaceholderInfo` on the root (intended model, triangle budget, owner Bryan) once the component exists.
7. `create_prefab` into `Prefabs/PH/` (PH_ characters/boss), `Prefabs/Props/<F>/` (props) or `Prefabs/Items/`.
8. `capture_scene_object`, then delete the scene instance and `save_scene`.
9. Update the row in `Desarrollo/PLACEHOLDERS.md` (status: placeholder done).
