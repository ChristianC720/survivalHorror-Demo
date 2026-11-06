using Heartbeat.Save;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Heartbeat.Render
{
    /// <summary>Applies Options → Video (resolution, screen mode, V-Sync, FPS limit). Shadows are read by RoomActivation.</summary>
    public static class VideoSettings
    {
        public static void Apply(GlobalData g)
        {
            var native = Screen.currentResolution;
            int w = g.resolutionWidth > 0 ? g.resolutionWidth : native.width;
            int h = g.resolutionHeight > 0 ? g.resolutionHeight : native.height;
            var mode = g.screenMode == 1 ? FullScreenMode.Windowed : g.screenMode == 2 ? FullScreenMode.FullScreenWindow : FullScreenMode.ExclusiveFullScreen;
#if !UNITY_EDITOR
            // In the editor the Game view owns the size.
            if (Screen.width != w || Screen.height != h || Screen.fullScreenMode != mode) Screen.SetResolution(w, h, mode);
#endif
            QualitySettings.vSyncCount = g.vsync ? 1 : 0;
            Application.targetFrameRate = g.fpsLimit > 0 ? g.fpsLimit : -1;
        }

        /// <summary>
        /// The pixel look (D-81) keeps ~540 lines, so the scene is rendered at that height and point-upscaled: same image,
        /// a fraction of the pixels to shade (SSAO, lights, dither). Called by PixelationFeature whenever the screen height changes.
        /// </summary>
        public static void ApplyRenderScale(int screenHeight)
        {
            if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urp) || screenHeight <= 0) return;
            urp.renderScale = Mathf.Clamp(PixelationFeature.TargetHeight / (float)screenHeight, 0.1f, 1f);   // URP's own limits
            urp.upscalingFilter = UpscalingFilterSelection.Point;
        }
    }
}
