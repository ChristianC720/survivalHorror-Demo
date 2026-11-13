using System;
using Heartbeat.Combat;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.AI
{
    /// <summary>Enemy toughness counted in normal revolver shots (M-04, D-66). A hit adds its <see cref="ShotHit.Damage"/>: weak points count double and still interrupt (C-74).</summary>
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyDefinition definition;

        // Fractional damage (shove, C-77) that sums exactly to the limit must kill despite float rounding.
        private const float Tolerance = 0.001f;

        private float damageTaken;

        public bool IsRecycled { get; private set; }
        public bool IsAlive => damageTaken < ShotsToKill - Tolerance;
        public int ShotsToKill => IsRecycled ? definition.recycledShotsToKill : definition.shotsToKill;
        /// <summary>Damage taken so far, in normal-shot units.</summary>
        public float DamageTaken => damageTaken;

        public event Action<ShotHit> Shot;
        public event Action Died;

        public void ReceiveShot(in ShotHit hit)
        {
            if (!IsAlive) return;
            damageTaken += hit.Damage;
            HbLog.Info("Enemy", $"{name} shot {damageTaken:0.##}/{ShotsToKill}{(hit.WeakPoint ? " weak point" : "")}");
            Shot?.Invoke(hit);
            if (!IsAlive)
            {
                HbLog.Info("Enemy", $"{name} killed");
                Died?.Invoke();
            }
        }

        /// <summary>Debug console / tests: kill outright.</summary>
        public void DebugKill()
        {
            while (IsAlive) ReceiveShot(new ShotHit(transform.position + Vector3.up, Vector3.forward, false));
        }

        /// <summary>Called by the recycling system when the lab raises the body (M-20).</summary>
        public void Revive(bool recycled)
        {
            IsRecycled = recycled;
            damageTaken = 0f;
        }
    }
}
