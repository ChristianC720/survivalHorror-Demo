# E-04 Combat (S-03)

**Goal:** a revolver that never has enough ammo, a shove that costs stamina, the discharge spike, and finishing corpses.

**GDD sources:** M-04, M-06, M-11, M-20, S-03; item details in `04 - Contenido/04 - Objetos y Coleccionables.md` (D-71, D-74, D-122).

## Deliverables
`Scripts/Combat/Revolver.cs`, `RevolverModel.cs` (pure: drum 6, round-by-round reload ~0.7 s, cancellable, ~0.8 s between shots), `Shove.cs`, `GrabResolver.cs`, `Finisher.cs`, `IDamageable.cs`, `WeakPoint.cs`, `Balance/CombatBalance.asset`, `PH/PH_Revolver` with a TMP counter on the frame (diegetic ammo, D-86).

## Tasks
- `E04-revolver`: hitscan from the camera centre while aiming (layers `Enemy`, `Hitbox`, `Environment`). Shot noise ~20 m (`NoiseEmitted`) + attention bump. Dry fire on empty. Reload takes rounds from the inventory (S-05) one by one. The revolver always comes back unloaded after reconstruction. Enemy hitboxes (C-63) are generated per weighted bone by `HitboxBuilder` (`Heartbeat/Rebuild Enemy Hitboxes`): box, capsule or convex hull (`Prefabs/Characters/Hitboxes/HB_*.asset`) covering the bind pose and the clips; `Hit_<bone>` and `WeakPoint_<bone>`, layer `Hitbox`. `WeakPoint.At(collider, point)` decides a weak-point hit; `Revolver` calls `Physics.SyncTransforms()` before the rays. `BulletMagnet.Resolve` (C-70) resolves the shot: camera ray for the aim point, bullet from Lukas's chest; a shot that lands on no hitbox bends to the nearest live enemy hitbox within `bulletMagnetRadius` (0.25 m) of the camera ray and `bulletMagnetMaxDegrees` (2) of its direction, only up to the wall the camera sees and only if the chest ray reaches that enemy clean. `WeakPoint.At` uses the chest ray's real hit. `EnemyHitboxCoverageTests` checks the coverage. Shot effects (C-71): `ShotEffects` on the player plays `VFX_MuzzleFlash` at `Muzzle` on `ShotFired` (`muzzleFlashSeconds`) and, on `ShotImpact` (raised only when the hit has no `IDamageable` in its parents, so no enemy alive or dead), `VFX_BulletImpact` at the hit point facing the normal: sparks, dust and a mark that fades over `bulletMarkSeconds`, at most `bulletMarkMax` at once, oldest recycled, cleared on `FloorBuilt`. Both go through `Heartbeat.Render.VfxPool` (per prefab cap, ring recycling, `VfxLifetime`), reusable for other effects. Prefabs and URP materials live in `Assets/_Project/VFX/`.
- `E04-shove`: left click without aiming. Costs 35 % stamina (`StaminaModel.TrySpend`); fails with a gasp otherwise. Short stagger on `IStaggerable`.
- `E04-spike-grab`: when an enemy grabs, `GrabResolver` checks the inventory: if a spike is present it is consumed automatically, the enemy is stunned 2 s and Lukas takes no damage (M-11). Otherwise the grab damage applies (by type).
- `E04-finisher`: near a corpse (context `OverBody`): left click = ground strike (stamina, makes noise, `GroundStrikeNoise`); `E` = spike finisher (~1.5 s, silent, consumes the spike). Both prevent recycling. `E` is not shown on corpses that cannot be recycled.

## Acceptance
- M-04: 6 loaded rounds, 4 hits on an Alto → it dies, the counter shows 2, enemies within ~20 m alerted.
- M-11, M-20 rows.
- EditMode: `RevolverModel` drum and reload tests.

## Revolver ownership (D-74, E13-bugfix)
`Revolver.Owned` hides the model and blocks aim, fire and reload until Lukas picks it up: `ITEM_Revolver` (`RevolverPickup`) lies by `PH_KesslerGuard_Corpse` in the tutorial depot (first attempt, tutorial room 2) and on the capsule-row tray on floor 1 after every reconstruction. Ownership travels in the floor-start and final-room saves (`AttemptSave.revolverOwned`); reconstruction leaves it on the tray. Sandboxes keep `ownedAtStart` = true; the Intro's Lukas has no revolver.
