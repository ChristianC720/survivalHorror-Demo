using System;
using Heartbeat.AI;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// Bestiary entries unlock on the first encounter (D-48): the first time an enemy type notices Lukas nearby
    /// (Suspicion or higher within <see cref="EncounterRadius"/>), its clinical report is registered in the terminal.
    /// The recycled-subjects annex unlocks when the first corpse rises (M-20), and The Mother's entry the first time Lukas
    /// enters her room. Created by Bootstrap.
    /// </summary>
    public sealed class BestiaryUnlocker : IDisposable
    {
        public const float EncounterRadius = 15f;
        private readonly RegistryModel registry;

        public BestiaryUnlocker(RegistryModel registry)
        {
            this.registry = registry;
            GameEvents.EnemyAlertChanged += OnAlert;
            GameEvents.EnemyRecycled += OnRecycled;
            GameEvents.FloorBuilt += OnFloorBuilt;
        }

        public void Dispose()
        {
            GameEvents.EnemyAlertChanged -= OnAlert;
            GameEvents.EnemyRecycled -= OnRecycled;
            GameEvents.FloorBuilt -= OnFloorBuilt;
        }

        private void OnAlert(MonoBehaviour enemy, AlertState state, float distance)
        {
            if (state == AlertState.Patrol || distance > EncounterRadius || !(enemy is EnemyBrain brain) || brain.Definition == null) return;
            Unlock("BEST." + brain.Definition.type.ToString().ToUpperInvariant());
        }

        private void OnRecycled(MonoBehaviour _) => Unlock("BEST.RECYCLED");

        private void OnFloorBuilt(int floor, int _) { if (floor == Generation.FloorBuilder.MotherFloor) Unlock("BEST.MOTHER"); }

        private void Unlock(string id)
        {
            if (!registry.Register(id, RecordKind.Bestiary)) return;
            HbLog.Info("Registry", $"REGISTERED {id} (bestiary, first encounter)");
            GameEvents.RaiseItemRegistered(id, (int)RecordKind.Bestiary, true);
        }
    }
}
