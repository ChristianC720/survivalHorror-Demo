using UnityEngine;

namespace Heartbeat.AI
{
    public enum EnemyType { Alto, Rastrero, Boca, Mudo }

    /// <summary>
    /// One asset per enemy type (GDD 04 - Contenido/03 - Enemigos y NPC, D-66, D-76). Initial values, subject to balance.
    /// </summary>
    [CreateAssetMenu(menuName = "Heartbeat/Enemy Definition", fileName = "ED_")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        public EnemyType type;

        [Header("Toughness (D-66)")]
        [Tooltip("Revolver shots to kill.")] public int shotsToKill = 4;
        [Tooltip("Shots to kill once recycled (M-20).")] public int recycledShotsToKill = 2;
        [Tooltip("Hits needed to kill Lukas.")] public int hitsToKillLukas = 2;

        [Header("Movement (m/s)")]
        public float patrolSpeed = 1.2f;
        public float huntSpeed = 3f;
        [Tooltip("Recycled enemies are 25% faster (M-20).")] public float recycledSpeedMultiplier = 1.25f;

        [Header("Perception")]
        [Tooltip("0 = blind (La Boca).")] public float sightRange = 15f;
        [Tooltip("Full sight cone angle, degrees.")] public float sightAngle = 110f;
        public float hearingRange = 6f;
        [Tooltip("Scales the radius of every noise this enemy hears (La Boca hears far, D-170).")] public float hearingMultiplier = 1f;
        public bool reactsToLight = true;
        public AI.AlertTuning alert = AI.AlertTuning.Default;

        [Header("Attack")]
        public float attackRange = 2.2f;
        [Tooltip("Telegraph before the hit lands.")] public float attackWindup = 0.6f;
        public float attackCooldown = 1.6f;
        public bool canGrab;
        [Range(0f, 1f)] public float grabChance = 0.35f;
        public float grabRange = 1.4f;
        [Tooltip("C-64: Grab clip time where the hands settle on Lukas; the grab windup plays the clip up to here. Measured from the clip (EnemyGrabClipTests).")]
        public float grabCloseSeconds = 0.94f;
        [Tooltip("C-64: clip seconds the hands hold Lukas after closing.")] public float grabHoldSeconds = 0.85f;
        [Tooltip("C-64: distance from this enemy's root to Lukas's root while held, so the hands rest on him.")]
        public float grabAnchorDistance = 0.85f;

        [Header("Behaviour")]
        [Tooltip("Rastrero: circle to the flank before striking.")] public bool flanks;
        [Tooltip("Lateral offset used while flanking, m.")] public float flankOffset = 3f;
        [Tooltip("Patrol radius around the spawn point, m.")] public float patrolRadius = 6f;
        [Tooltip("Stagger after a weak-point hit, s (interrupts attacks and grabs).")] public float weakPointStagger = 0.6f;

        [Header("La Boca (D-64)")]
        [Tooltip("Screams on entering Search/Hunt: nearby enemies search towards her.")] public bool screams;
        public float screamRadius = 18f;
        public float screamCooldown = 8f;
        [Tooltip("Blind hunters: a noise this close while already searching starts a hunt (fraction of hearing range).")]
        [Range(0f, 1f)] public float blindHuntFraction = 0.6f;

        [Header("El Mudo (D-53, D-60)")]
        [Tooltip("Does not patrol: stands still until Lukas is this close or lights it.")] public bool dormant;
        public float activationRange = 3f;
        [Tooltip("Moves in bursts while hunting: seconds moving / seconds still (0 = continuous).")]
        public float burstSeconds;
        public float burstPauseSeconds = 0.7f;
    }
}
