using System;
using Heartbeat.Combat;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.AI
{
    /// <summary>Enemy toughness counted in revolver shots (M-04, D-66). Weak-point hits interrupt but do not add damage.</summary>
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyDefinition definition;

        private int shotsTaken;

        public bool IsRecycled { get; private set; }
        public bool IsAlive => shotsTaken < ShotsToKill;
        public int ShotsToKill => IsRecycled ? definition.recycledShotsToKill : definition.shotsToKill;
        public int ShotsTaken => shotsTaken;

        public event Action<ShotHit> Shot;
        public event Action Died;

        public void ReceiveShot(in ShotHit hit)
        {
            if (!IsAlive) return;
            shotsTaken++;
            HbLog.Info("Enemy", $"{name} shot {shotsTaken}/{ShotsToKill}{(hit.WeakPoint ? " weak point" : "")}");
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
            shotsTaken = 0;
        }
    }
}
