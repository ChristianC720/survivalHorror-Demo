using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>Enemy alert states (M-10, D-23). Only Hunt counts as "in alert" for the surveillance cut.</summary>
    public enum AlertState { Patrol, Suspicion, Search, Hunt }

    /// <summary>An enemy attack or grab aimed at Lukas (S-02 → S-03).</summary>
    public readonly struct AttackInfo
    {
        public readonly int HitsToKill;
        public readonly bool IsGrab;
        public readonly Vector3 From;
        public readonly MonoBehaviour Source;

        public AttackInfo(int hitsToKill, bool isGrab, Vector3 from, MonoBehaviour source)
        {
            HitsToKill = hitsToKill;
            IsGrab = isGrab;
            From = from;
            Source = source;
        }
    }
}
