using UnityEngine;

namespace Heartbeat.Render
{
    /// <summary>
    /// M-18 accumulated degradation as CRT corruption in the pixel pass (_HbDegradation). Grows with the level and is
    /// capped so it never hides information or the M-10 signals (D-29). Driven by GameEvents.DegradationChanged.
    /// </summary>
    public static class Degradation
    {
        public const int MaxLevel = Cycle.DeathCounter.MaxDegradation;
        public const float MaxStrength = 0.7f;   // cap (D-29)
        private static readonly int Id = Shader.PropertyToID("_HbDegradation");

        public static float Strength(int level) => Mathf.Clamp01(level / (float)MaxLevel) * MaxStrength;

        private static int current;
        private static bool reduced;

        public static void Apply(int level)
        {
            current = level;
            Shader.SetGlobalFloat(Id, Strength(level) * (reduced ? 0.5f : 1f));
        }

        /// <summary>Options "reduce CRT effects": softer, but the corruption still grows with the level.</summary>
        public static void SetReduced(bool on)
        {
            reduced = on;
            Apply(current);
        }
    }
}
