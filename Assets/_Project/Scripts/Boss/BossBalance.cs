using UnityEngine;

namespace Heartbeat.Boss
{
    /// <summary>The Mother encounter values (M-19, D-28, D-65, D-80, D-98, D-115). Initial, subject to balance.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Boss", fileName = "BossBalance")]
    public sealed class BossBalance : ScriptableObject
    {
        [Header("Collector arm (D-65, D-98)")]
        [Tooltip("Crane speed on its rails, m/s (Lukas running outpaces it).")] public float armSpeed = 1.6f;
        [Tooltip("Grab reach, m.")] public float armReach = 2.2f;
        [Tooltip("Light + hydraulic warning before the grab, s.")] public float armWarning = 2.2f;
        public float armCooldown = 7f;
        [Tooltip("Stun when the discharge spike counters the grab (M-11).")] public float armStunSeconds = 2f;
        [Tooltip("Phase 1: the arm leaves its node patrol to chase Lukas within this distance.")] public float armChaseRadius = 6f;

        [Header("Hostile cameras, phase 2 (D-28, D-98)")]
        public float cameraSweepDegreesPerSecond = 20f;
        public float cameraSweepHalfAngle = 60f;
        public float cameraRange = 12f;
        public float cameraConeHalfAngle = 20f;
        [Tooltip("Focus time before the red light locks and an assimilated is released, s.")] public float cameraLockSeconds = 1f;
        public float cameraCooldown = 5f;

        [Header("Assimilated release, phase 2 (D-65, D-115, D-117)")]
        public int releasesTotal = 4;
        public int maxActive = 2;
        [Tooltip("Rounds in the room's blind spots for phase 2 (3 blisters).")] public int phase2Ammo = 12;
        [Tooltip("Dark cold haze: hides distance, never washes the image out or hides the M-10 signals.")]
        public float fogDensity = 0.045f;
        public Color fogColor = new Color(0.09f, 0.1f, 0.115f);

        [Header("Room transformation (D-59)")]
        public float speakerPulse = 0.04f;
        public float speakerBeatsPerSecond = 1.2f;
        public float plateFallDegreesPerSecond = 150f;

        [Header("Transformers, phase 3 (D-80)")]
        public int transformerShots = 2;
        [Tooltip("Rounds next to the core: 6 for the transformers + 2 margin.")] public int phase3Ammo = 8;

        [Header("Final decision (D-61)")]
        [Tooltip("Without the 6 Emilia documents Lukas examines the terminal automatically after this delay, s.")]
        public float autoDecisionSeconds = 6f;
    }
}
