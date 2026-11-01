using Heartbeat.Combat;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.DebugTools
{
    /// <summary>Sandbox target: counts shots and dies after <see cref="shotsToKill"/> (Alto = 4). Not used in levels.</summary>
    public sealed class ShotDummy : MonoBehaviour, IDamageable
    {
        [SerializeField] private int shotsToKill = 4;

        public int Hits { get; private set; }
        public int WeakHits { get; private set; }
        public bool IsAlive => Hits < shotsToKill;

        public void ReceiveShot(in ShotHit hit)
        {
            Hits++;
            if (hit.WeakPoint) WeakHits++;
            HbLog.Info("Dummy", $"{name} hit {Hits}/{shotsToKill}{(hit.WeakPoint ? " (weak point)" : "")}");
            if (!IsAlive)
            {
                HbLog.Info("Dummy", $"{name} killed");
                transform.localScale = new Vector3(1f, 0.2f, 1f);
            }
        }
    }
}
