using Heartbeat.AI;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Recycle
{
    /// <summary>
    /// The body of a dead enemy (S-09, M-20). Every unfinished body can be raised once by the lab; finishing it
    /// (ground strike or discharge spike) burns its last trace of identity. Recycled enemies and boss-room bodies
    /// cannot be recycled, so the finisher prompt never targets them. The timer/rise lives in RecycleSystem (E-08).
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class Corpse : MonoBehaviour
    {
        [Tooltip("Bodies in the boss room are never recycled (M-20).")]
        [SerializeField] private bool neverRecycle;

        private EnemyHealth health;

        public bool IsDown => health != null && !health.IsAlive;
        public bool IsFinished { get; private set; }
        public float DiedAt { get; private set; } = -1f;

        /// <summary>True when the lab could still raise this body: down, not finished, not already recycled.</summary>
        public bool CanBeRecycled => IsDown && !IsFinished && !neverRecycle && !health.IsRecycled;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            health.Died += () => DiedAt = Time.time;
        }

        public void Finish(string how)
        {
            if (!CanBeRecycled) return;
            IsFinished = true;
            HbLog.Info("Recycle", $"{name} finished by {how}; it will not be recycled");
        }

        /// <summary>Debug / tests: pretend the body died <paramref name="seconds"/> ago.</summary>
        public void DebugAge(float seconds) => DiedAt = Time.time - seconds;

        /// <summary>Called when the lab raises the body (E-08) so it becomes a living, recycled enemy again.</summary>
        public void OnRaised()
        {
            DiedAt = -1f;
        }
    }
}
