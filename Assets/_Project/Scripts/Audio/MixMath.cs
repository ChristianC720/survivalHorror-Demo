using System;

namespace Heartbeat.Audio
{
    /// <summary>Pure mixer math (testable).</summary>
    public static class MixMath
    {
        public const float SilentDb = -80f;

        /// <summary>0..1 slider → dB. 0 = silent, 1 = 0 dB.</summary>
        public static float LinearToDb(float v) => v <= 0.0001f ? SilentDb : Math.Max(SilentDb, 20f * (float)Math.Log10(v));

        /// <summary>Duck offset in dB: full while the hold lasts (sinceHoldEnd &lt; 0), then a linear release to 0.</summary>
        public static float Duck(float sinceHoldEnd, float duckDb, float releaseSeconds)
        {
            if (sinceHoldEnd <= 0f) return duckDb;
            if (sinceHoldEnd >= releaseSeconds) return 0f;
            return duckDb * (1f - sinceHoldEnd / releaseSeconds);
        }
    }
}
