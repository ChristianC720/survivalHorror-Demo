using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>
    /// Balance numbers of systems that are created at runtime and cannot be wired in a scene (recycling, attention,
    /// alert scope, seals, Story mode). Registered by Bootstrap; <see cref="Current"/> falls back to the defaults so
    /// sandbox scenes without Bootstrap still work. Initial values, subject to balance.
    /// </summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Systems", fileName = "SystemsBalance")]
    public sealed class SystemsBalance : ScriptableObject
    {
        [Header("Recycling (M-20, D-99, D-166)")]
        [Tooltip("Seconds after death when the body rises.")] public float recycleAfterSeconds = 40f;
        [Tooltip("The body convulses this long before rising.")] public float recycleWarningSeconds = 3f;
        [Tooltip("If Lukas keeps looking at the body, it waits at most this long, then rises anyway.")] public float recycleMaxWaitSeconds = 20f;

        private static SystemsBalance fallback;

        public static SystemsBalance Current =>
            Services.TryGet(out SystemsBalance b) ? b : fallback != null ? fallback : fallback = CreateInstance<SystemsBalance>();
    }
}
