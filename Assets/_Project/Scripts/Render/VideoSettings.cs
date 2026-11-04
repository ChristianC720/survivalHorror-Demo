using Heartbeat.Save;
using UnityEngine;

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
    }
}
