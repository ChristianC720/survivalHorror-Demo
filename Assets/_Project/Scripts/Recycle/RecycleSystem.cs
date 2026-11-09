using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Recycle
{
    /// <summary>
    /// The lab raises every unfinished body once (S-09, M-20, D-99, D-166): the body convulses with an audible warning,
    /// then stands up where it fell ~40 s after death as a recycled enemy (25% faster, 2 shots / 1 for a Rastrero).
    /// It only rises out of Lukas's view; if he keeps looking, it waits at most <see cref="SystemsBalance.recycleMaxWaitSeconds"/>.
    /// Finished bodies, recycled enemies and boss-room bodies never rise. Scene service next to the AlertDirector.
    /// </summary>
    public sealed class RecycleSystem : MonoBehaviour
    {
        private readonly List<Corpse> corpses = new List<Corpse>();
        private readonly HashSet<Corpse> warned = new HashSet<Corpse>();
        private int envMask;

        public int Pending => corpses.Count;

        private void Awake() => envMask = LayerMask.GetMask("Environment");

        private void OnEnable()
        {
            Services.Register(this);
            GameEvents.EnemyKilled += OnEnemyKilled;
        }

        private void OnDisable()
        {
            Services.Unregister(this);
            GameEvents.EnemyKilled -= OnEnemyKilled;
        }

        private void OnEnemyKilled(MonoBehaviour enemy, Vector3 pos)
        {
            var c = enemy.GetComponent<Corpse>();
            if (c != null && c.CanBeRecycled && !corpses.Contains(c)) corpses.Add(c);
        }

        /// <summary>The Mother's room is only machine: no bodies rise there (D-28).</summary>
        public bool Suspended { get; set; }

        /// <summary>D-166: past the rise time the body waits while it is seen, up to <paramref name="maxWait"/> more seconds.</summary>
        public static bool ShouldRise(float age, bool seen, float riseAfter, float maxWait) =>
            age >= riseAfter && (!seen || age >= riseAfter + maxWait);

        private void Update()
        {
            if (Suspended) return;
            var b = SystemsBalance.Current;
            for (int i = corpses.Count - 1; i >= 0; i--)
            {
                var c = corpses[i];
                if (c == null || !c.CanBeRecycled)
                {
                    if (c != null) c.SetConvulsing(false);
                    warned.Remove(c);
                    corpses.RemoveAt(i);
                    continue;
                }
                float age = Time.time - c.DiedAt;
                if (age >= b.recycleAfterSeconds - b.recycleWarningSeconds && warned.Add(c))
                {
                    HbLog.Info("Recycle", $"{c.name}: convulsing (warning)");
                    c.SetConvulsing(true);
                    GameEvents.RaiseRecycleWarning(c.transform.position);
                }
                if (ShouldRise(age, SeenByLukas(c), b.recycleAfterSeconds, b.recycleMaxWaitSeconds))
                {
                    corpses.RemoveAt(i);
                    warned.Remove(c);
                    Raise(c, age - b.recycleAfterSeconds);
                }
            }
        }

        /// <summary>In the gameplay camera's frame with no wall in between.</summary>
        private bool SeenByLukas(Corpse c)
        {
            var cam = Camera.main;
            if (cam == null) return false;
            Vector3 p = c.transform.position + Vector3.up * 0.4f;
            Vector3 v = cam.WorldToViewportPoint(p);
            if (v.z <= 0f || v.x < 0f || v.x > 1f || v.y < 0f || v.y > 1f) return false;
            return !Physics.Linecast(cam.transform.position, p, envMask, QueryTriggerInteraction.Ignore);
        }

        private void Raise(Corpse c, float waited)
        {
            c.GetComponent<EnemyHealth>().Revive(recycled: true);
            c.GetComponent<EnemyBrain>().OnRecycled();
            c.OnRaised();
            HbLog.Info("Recycle", $"{c.name} rises recycled (waited {waited:0.0} s for Lukas to look away)");
            GameEvents.RaiseEnemyRecycled(c.GetComponent<EnemyBrain>());
        }

        /// <summary>Debug / tests: age every pending body so it rises within <paramref name="seconds"/>.</summary>
        public void DebugFastForward(float seconds)
        {
            foreach (var c in corpses) c.DebugAge(SystemsBalance.Current.recycleAfterSeconds - seconds);
        }
    }
}
