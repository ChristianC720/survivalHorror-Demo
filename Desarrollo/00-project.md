# E-00 Project setup

**Goal:** a clean Unity 6.6 URP project with the official packages, folders, asmdefs, layers and the final pixel look, so that every later system is built and judged with the real render.

**GDD sources:** `07 - Tecnología/02 - Motor y Herramientas.md`, `03 - Requisitos Técnicos.md`, `05 - Arte y Audio/01 - Dirección de Arte.md` (D-81 render).

## Tasks
### `E00-cleanup` ✅
Legacy tagged `legacy-prototype` and removed. CLAUDE.md, Desarrollo/, Aura skills.

### `E00-packages` ✅
1. Install with `install_unity_package`: `com.unity.cinemachine` (3.x), `com.unity.timeline`, `com.unity.animation.rigging`, `com.unity.localization`, `com.unity.test-framework`. AI Navigation, Input System and URP are already present. Check with `list_packages`.
2. Import TMP Essential Resources (uGUI 2.6 contains TMP): scratchpad `execute_script` calling `TMPro.TMP_PackageResourceImporter.ImportResources(true, false, false)`, or the menu `Window/TextMeshPro/Import TMP Essential Resources`.
3. Create the folder tree from CLAUDE.md "Layout". Add asmdefs: `Heartbeat.Runtime` (refs: Unity.InputSystem, Unity.Cinemachine, Unity.AI.Navigation, Unity.Localization, Unity.TextMeshPro, Unity.Timeline, Unity.RenderPipelines.Universal.Runtime, Unity.RenderPipelines.Core.Runtime), `Heartbeat.Editor` (Editor only), `Heartbeat.Tests.EditMode` (test assembly, refs Runtime).
4. Layers and tags from `KIT_SPEC.md` §6 (scratchpad `execute_script` on `ProjectSettings/TagManager.asset` via `SerializedObject`). Physics matrix: `RoomVolume` collides only with `Player` and `Enemy`; `Ceiling` with nothing except `Player`.
5. Player Settings: company "Heartbeat", product "Identity Dissolution", Win64 only, Input System only (`activeInputHandler = 1`), Linear colour space, Enter Play Mode Options with domain reload disabled (hence `GameEvents.ResetAll`).
6. Set the default `PC_RPAsset` to Forward+ and delete `Mobile_*` assets if unused.

### `E00-render` ✅
1. `Scripts/Render/PixelationFeature.cs`: a URP `ScriptableRendererFeature` that renders the camera at ~540p (height from `RenderConfig` SO: 540; width by aspect), applies ordered dithering (4×4 Bayer, strength param) and colour quantization, and upscales with point filtering. Use the RenderGraph API (URP 17): a `RecordRenderGraph` pass that blits to a low-res `TextureHandle` and back.
2. Shader `Render/Dither.shader` (HLSL, URP Blit-compatible).
3. Add the feature to `PC_Renderer` (done: `HeartbeatPixelation`, settings on the feature: target height 540, 32 colour steps, dither 0.6, after post-processing so overlay UI stays sharp). Surveillance and degradation drive it at runtime through `PixelationFeature.HeightOverride`. CRT effects get their own feature in E-03 and E-08.
   Note: the editor Game view is ~538 px tall, so at 540 the effect only shows at real resolutions. To check it in the editor, lower `targetHeight` temporarily.
4. Texture import preset: Point filter, no mipmaps, max 256 (props) / 512 (key items).

## Acceptance
- `check_compile_errors` clean. `list_packages` shows all packages.
- A test scene with a cube and a spotlight renders pixelated at 540 lines with visible dithering in the light falloff (`capture_editor_screenshot` in Game view at 1920×1080).
- Frame time with the feature < 1 ms on the dev machine (`get_worst_cpu_frames`).
