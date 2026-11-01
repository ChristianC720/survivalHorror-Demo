# E-12 Art and render (placeholders)

**Goal:** the final look (low-poly with modern light, pixelated render) on placeholder geometry, with readable darkness.

**GDD sources:** `05 - Arte y Audio/01 - Dirección de Arte.md` (palette, rules), `02 - Personajes y Animación.md`, `03 - Entornos.md`, reference images in `GDD/10 - Referencia/` (palettes PaletaBase / Carmesi / Morados / Verde).

## Palette (placeholder material colours)
| Role | Colour |
|---|---|
| Base structure (~70 %) | warm greys with a plum tint, e.g. `#3A3438`, `#4B4448`, `#2A2629` |
| Vitalis / danger / The Mother | crimson `#922235` |
| Lukas and terminal | teal, e.g. `#2F7F7A` |
| Cosmic / assimilated sheen | purple, e.g. `#5B3A78` |
| Facility screens | phosphor green `#59FF8C` |
| Floor 1 sodium light | amber `#FFB347` |
| Flashlight only | warm white `#FFF2DE` |
Sample the exact values from the palette PNGs with a scratchpad script if needed.

## Rules
- Colour is never the only signal. One light tint per room. Pickups only stand out under light. Lukas's silhouette always separates from the background. Degradation never hides information.
- No baked lighting (rooms are procedural): realtime lights, ambient almost black (`RenderSettings.ambientMode = Flat`, very dark plum), no directional light inside the facility.
- Materials: `URP/Lit` via `create_pbr_material`, smoothness low; point filtering on any texture.

## Tasks
- `E12-sandbox-kit` (H1): `Sandbox_Kit` scene, snapping set, F1 kit pieces (see `14-levels.md`, `KIT_SPEC.md`).
- `E12-f1-lighting` (H4): light templates per floor (`Prefabs/Level/Light_F1_Sodium`: downward spot 140°, intensity 18, range 9), ≤ 16 active lights, ≤ 4 with shadows. `RoomActivation` lights the current room then neighbours by distance up to 15 room lights (+ flashlight), gives hard shadows only to ≤ 3 lights of the current room, and keeps unpowered rooms dark until `PowerRestored`. Log: `[Heartbeat][Rooms] Active around <room>: … lights=N shadowed=M`.
- Player brightness (D-106) is a gain in the PixelDither pass (`_HbBrightness`, `Heartbeat.Render.Brightness`).
- Placeholders as they are needed by other epics (`PLACEHOLDERS.md`).
