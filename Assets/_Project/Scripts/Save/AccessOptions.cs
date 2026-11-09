using Heartbeat.Core;

namespace Heartbeat.Save
{
    /// <summary>
    /// Cached accessibility flags (D-106) for effects that check them every frame: reading the global save each time
    /// would hit the disk. Refreshed when Options are applied.
    /// </summary>
    public static class AccessOptions
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => loaded = false;

        private static bool loaded, flashes, motion;

        /// <summary>Lights and screens do not blink (failing lights, empty flashlight, discharge).</summary>
        public static bool ReduceFlashes { get { Load(); return flashes; } }

        /// <summary>No camera shake, softer sway.</summary>
        public static bool ReduceMotion { get { Load(); return motion; } }

        private static void Load()
        {
            if (loaded || !Services.TryGet(out SaveService saves)) return;
            var g = saves.LoadGlobal();
            flashes = g.reduceFlashes;
            motion = g.reduceMotion;
            loaded = true;
            GameEvents.OptionsChanged -= Invalidate;
            GameEvents.OptionsChanged += Invalidate;
        }

        private static void Invalidate() => loaded = false;
    }
}
