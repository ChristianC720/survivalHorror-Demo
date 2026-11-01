using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>Lukas's balance values. Initial values from the GDD, subject to balance.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Player", fileName = "PlayerBalance")]
    public sealed class PlayerBalance : ScriptableObject
    {
        [Header("Movement (M-06, D-76)")]
        [Tooltip("Crouched speed, m/s (D-76).")] public float crouchSpeed = 0.8f;
        [Tooltip("Walking speed, m/s (D-76).")] public float walkSpeed = 1.5f;
        [Tooltip("Running speed, m/s (D-76).")] public float runSpeed = 4f;
        [Tooltip("Speed multiplier while Injured (M-05: -15%).")] public float injuredSpeedMultiplier = 0.85f;
        [Tooltip("Speed multiplier while Critical (M-05: -30%).")] public float criticalSpeedMultiplier = 0.70f;
        [Tooltip("Speed multiplier while aiming (M-01: slower; value to balance).")] public float aimSpeedMultiplier = 0.6f;
        [Tooltip("Speed while the surveillance view is open (M-02: still or very slow).")] public float surveillanceSpeed = 0.4f;
        [Tooltip("Turn speed of the body towards the move direction, degrees/s.")] public float turnSpeed = 540f;
        public float gravity = -20f;
        [Tooltip("Capsule height standing / crouched.")] public float standHeight = 1.8f;
        public float crouchHeight = 1.2f;

        [Header("Stamina (M-06, D-37, D-123)")]
        [Tooltip("Stamina cost per second while running (10%/s).")] public float sprintCostPerSecond = 10f;
        [Tooltip("Shove / butt strike cost (35%).")] public float shoveCost = 35f;
        [Tooltip("Ground-strike finisher cost (25%).")] public float groundStrikeCost = 25f;
        [Tooltip("Seconds without spending before regeneration starts (3 s).")] public float regenDelay = 3f;
        [Tooltip("Regeneration per second (25%/s: 0→100 in ~4 s).")] public float regenPerSecond = 25f;
        [Tooltip("Exhaustion lock at 0 (2 s).")] public float exhaustionSeconds = 2f;

        [Header("Health (M-05, D-66, D-119)")]
        [Tooltip("Hidden health split in 3 equal bands (Fine/Injured/Critical).")] public float maxHealth = 90f;
        [Tooltip("Heal animation length, s (M-07: ~1.5 s).")] public float healSeconds = 1.5f;

        [Header("Noise (M-09)")]
        [Tooltip("Walking: low noise radius, m.")] public float walkNoiseRadius = 3f;
        [Tooltip("Running: area pulse radius, m.")] public float runNoiseRadius = 10f;
        [Tooltip("Seconds between movement noise pulses.")] public float noiseInterval = 0.5f;
    }
}
