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

        [Header("Surveillance and The Mother's attention (M-02, D-23, D-24, D-98)")]
        [Tooltip("Seconds to fill the hidden attention meter while the view is open.")] public float attentionFillSeconds = 5f;
        [Tooltip("Seconds to drain it while the view is closed.")] public float attentionDrainSeconds = 15f;
        [Tooltip("Attention added by each shot (0..1).")] public float shotAttention = 0.15f;
        [Tooltip("When attention fills, nearby enemies stay suspicious this long, then search towards Lukas.")] public float attentionSuspicionSeconds = 2f;
        [Tooltip("Feed delay with no enemy searching.")] public float feedDelaySeconds = 0.3f;
        [Tooltip("Feed delay while an enemy in scope is searching (degraded signal).")] public float searchFeedDelaySeconds = 0.6f;

        [Header("Cycle and doors (D-39, D-97)")]
        [Tooltip("Floor transition cinematic, never shorter (D-39).")] [Min(5f)] public float floorTransitionSeconds = 5f;
        [Tooltip("Short reconstruction shot (D-172).")] public float shortReconstructionSeconds = 3f;
        [Tooltip("An enemy breaks a quick seal after this long (D-97).")] public float sealBreakSeconds = 30f;

        [Header("Story mode (D-103, D-116)")]
        [Tooltip("Each enemy needs this many times its hits to kill Lukas.")] public int storyHitsMultiplier = 2;
        [Tooltip("Sight alert gain is multiplied by this (slower detection).")] public float storyDetectionMultiplier = 0.5f;

        private static SystemsBalance fallback;

        public static SystemsBalance Current =>
            Services.TryGet(out SystemsBalance b) ? b : fallback != null ? fallback : fallback = CreateInstance<SystemsBalance>();
    }
}
