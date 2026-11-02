using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Recycle
{
    /// <summary>
    /// The lab raises every unfinished body once (S-09, M-20, D-99): ~3 s of audible convulsion as a warning, then the
    /// body stands up ~40 s after death as a recycled enemy (25% faster, 2 shots / 1 for a Rastrero, metal pieces).
    /// Finished bodies, recycled enemies and boss-room bodies never rise. Scene service next to the AlertDirector.
    /// </summary>
    public sealed class RecycleSystem : MonoBehaviour
    {
        [SerializeField] private float riseAfterSeconds = 40f;
        [SerializeField] private float warningSeconds = 3f;
        [Tooltip("Material of the placeholder metal pieces on recycled enemies.")]
        [SerializeField] private Material metal;

        private readonly List<Corpse> corpses = new List<Corpse>();
        private readonly HashSet<Corpse> warned = new HashSet<Corpse>();

        public int Pending => corpses.Count;

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

        private void Update()
        {
            if (Suspended) return;
            for (int i = corpses.Count - 1; i >= 0; i--)
            {
                var c = corpses[i];
                if (c == null || !c.CanBeRecycled) { warned.Remove(c); corpses.RemoveAt(i); continue; }
                float age = Time.time - c.DiedAt;
                if (age >= riseAfterSeconds - warningSeconds && warned.Add(c))
                {
                    HbLog.Info("Recycle", $"{c.name}: convulsing (warning)");
                    GameEvents.RaiseRecycleWarning(c.transform.position);
                }
                if (age >= riseAfterSeconds)
                {
                    corpses.RemoveAt(i);
                    warned.Remove(c);
                    Raise(c);
                }
            }
        }

        private void Raise(Corpse c)
        {
            c.GetComponent<EnemyHealth>().Revive(recycled: true);
            c.GetComponent<EnemyBrain>().OnRecycled();
            c.OnRaised();
            AddMetalPieces(c.transform);
            HbLog.Info("Recycle", $"{c.name} rises recycled");
            GameEvents.RaiseEnemyRecycled(c.GetComponent<EnemyBrain>());
        }

        /// <summary>Placeholder for the metal pieces The Mother adds (PLACEHOLDERS.md: PH_Recycled_Parts).</summary>
        private void AddMetalPieces(Transform enemy)
        {
            var mat = metal;
            for (int i = 0; i < 3; i++)
            {
                var piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(piece.GetComponent<Collider>());
                piece.name = "PH_Recycled_Part";
                piece.transform.SetParent(enemy, false);
                piece.transform.localScale = new Vector3(0.18f, 0.08f, 0.12f);
                piece.transform.localPosition = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(0.6f, 1.6f), 0.2f);
                piece.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f));
                if (mat != null) piece.GetComponent<Renderer>().sharedMaterial = mat;
            }
        }

        /// <summary>Debug / tests: age every pending body so it rises within <paramref name="seconds"/>.</summary>
        public void DebugFastForward(float seconds)
        {
            foreach (var c in corpses) c.DebugAge(riseAfterSeconds - seconds);
        }
    }
}
