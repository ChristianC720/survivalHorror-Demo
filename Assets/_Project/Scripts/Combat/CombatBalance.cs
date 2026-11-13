using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>Revolver, shove, spike and finisher values (M-04, M-06, M-11, M-20). Initial values, subject to balance.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Combat", fileName = "CombatBalance")]
    public sealed class CombatBalance : ScriptableObject
    {
        [Header("Revolver (M-04, D-74)")]
        [Tooltip("Drum size.")] public int drumSize = 6;
        [Tooltip("Seconds between shots (recoil).")] public float secondsBetweenShots = 0.8f;
        [Tooltip("Seconds to load one round.")] public float reloadSecondsPerRound = 0.7f;
        [Tooltip("Max spread cone (degrees) when moving / after a shot.")] public float maxSpreadDegrees = 5f;
        [Tooltip("Seconds standing still to fully close the spread.")] public float spreadCloseSeconds = 1f;
        [Tooltip("Spread added by each shot (0..1).")] public float spreadPerShot = 0.6f;
        public float range = 60f;
        [Tooltip("Shots alert enemies within this radius (M-09).")] public float shotNoiseRadius = 20f;
        [Tooltip("A shot that misses bends to a live enemy hitbox this close to its ray, metres (C-70).")] public float bulletMagnetRadius = 0.25f;
        [Tooltip("Largest bend of that magnet, degrees (C-70).")] public float bulletMagnetMaxDegrees = 2f;
        [Tooltip("Muzzle flash light and particles, seconds (C-71).")] public float muzzleFlashSeconds = 0.06f;
        [Tooltip("A bullet mark on a wall fades out over this many seconds (C-71).")] public float bulletMarkSeconds = 10f;
        [Tooltip("Bullet marks at once; the oldest is recycled (C-71).")] public int bulletMarkMax = 20;

        [Header("Shove (M-06)")]
        public float shoveRange = 1.6f;
        public float shoveRadius = 0.6f;
        [Tooltip("Stagger applied to the shoved enemy, seconds.")] public float shoveStaggerSeconds = 1.2f;
        public float shoveCooldown = 0.6f;

        [Header("Spike (M-11) and finisher (M-20)")]
        [Tooltip("Stun on an enemy that grabbed Lukas when the spike fires.")] public float spikeStunSeconds = 2f;
        [Tooltip("Distance to a corpse to enter the OverBody context (D-27).")] public float finisherRange = 1.5f;
        [Tooltip("Spike finisher duration (silent).")] public float spikeFinisherSeconds = 1.5f;
        [Tooltip("Ground strike duration.")] public float groundStrikeSeconds = 2f;
        [Tooltip("Ground strike noise radius.")] public float groundStrikeNoiseRadius = 10f;
    }
}
