# E-03 Camera and surveillance (S-01)

**Goal:** over-the-shoulder camera with aiming, and the isometric surveillance view with noise, delay, hidden attention cost and lockout.

**GDD sources:** M-01, M-02, M-03 (`02 - Mecánicas.md`), S-01 (`03 - Sistemas.md`), D-23, D-24, D-28.

## Deliverables
`Scripts/Camera/CameraRig.cs` (Cinemachine 3), `AimController.cs`, `SurveillanceSystem.cs`, `SurveillanceCamera.cs` (on room `CameraMount`), `AttentionMeter.cs` (pure), `Balance/CameraBalance.asset`, `Render/CRTSurveillance` material/feature.

## Tasks
### `E03-ots-camera`
1. `CinemachineCamera` + `CinemachineThirdPersonFollow` (right shoulder) for exploration, a second one with closer offset and lower FOV for aiming. Camera collision with `CinemachineDeoccluder` against `Environment`.
2. Aim (hold right click / LT): blend to the aim camera, show the reticle, slow to aim speed, no running, flashlight cone narrows (M-12).
3. Lukas's silhouette always separates from the background (art rule): later a rim light; for now check in screenshots.

### `E03-surveillance` (H3, needs rooms with `CameraMount`)
1. Only in rooms whose `RoomDefinition.hasCamera` (all key rooms + ~1/3 procedural). `Q` toggles. The view shows the current room only (same floor), isometric orthographic from the mount, ~0.3 s delay (render-texture ring buffer of frames), CRT noise. Ceiling layer hidden.
2. While viewing, Lukas can only stand still or walk very slowly (input context `Surveillance`).
3. `AttentionMeter` (pure, hidden): fills while viewing (~5 s to full), decays when closed. Full → `AttentionFull(playerPos)` → S-02 sends enemies. Shots also add a little (D-24).
4. Signal stages driven by `HighestAlertChanged`: Search → signal degrades; Hunt (or the player takes damage) → instant cut, `Q` locked until the hunt ends or Lukas crosses a door (M-03, D-23).

## Acceptance
- M-01 and M-02/M-03 rows of the acceptance table.
- Playtest: hold aim → log `[Heartbeat][Camera] Aim on`, reticle visible (capture). Surveillance: press Q in a camera room → `[Heartbeat][Surveillance] Open room=<id>`; force a hunt → `Cut reason=Hunt` and Q does nothing.
