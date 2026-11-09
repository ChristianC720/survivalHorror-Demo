using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.Inventory;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Resolves enemy attacks on Lukas (S-03). Hits remove health by the attacker's hits-to-kill. Grabs consume the
    /// discharge spike automatically and stun the grabber 2 s with no damage (M-11); without a spike, Lukas drops one
    /// health state, or dies if Critical.
    /// </summary>
    public sealed class PlayerDamageReceiver : MonoBehaviour
    {
        [SerializeField] private CombatBalance balance;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private PlayerInventory inventory;

        public int GrabsCountered { get; private set; }
        private bool armWarned;

        /// <summary>D-169: true when this grab leaves Lukas at the edge; remembers a spared grab in Critical.</summary>
        public static bool Spares(ref bool warned, bool neverKills, HealthState state)
        {
            bool spared = neverKills && !warned;
            if (neverKills && state == HealthState.Critical) warned = true;
            return spared;
        }

        private void OnEnable() => GameEvents.PlayerAttacked += OnAttacked;
        private void OnDisable() => GameEvents.PlayerAttacked -= OnAttacked;

        private void OnAttacked(AttackInfo attack)
        {
            if (health.Model.IsDead) return;
            if (!attack.IsGrab)
            {
                health.ApplyHit(attack.HitsToKill * (Cycle.CycleController.StoryMode ? SystemsBalance.Current.storyHitsMultiplier : 1));   // D-116
                return;
            }

            if (health.State != HealthState.Critical) armWarned = false;   // healed since the last arm grab
            if (inventory.Model.TryConsume(ItemKind.Spike))
            {
                armWarned = false;
                GrabsCountered++;
                HbLog.Info("Combat", "Grab countered: discharge spike consumed");
                if (attack.Source is IGrabber grabber) grabber.OnGrabCountered(balance.spikeStunSeconds);
                GameEvents.RaiseGrabResolved(true);
                return;
            }

            // D-169: the collector arm leaves Lukas at the edge the first time in Critical; the next grab before healing
            // or using a spike kills him.
            bool spared = Spares(ref armWarned, attack.NeverKills, health.State);
            if (health.State == HealthState.Critical && !spared)
            {
                health.ApplyGrabWithoutSpike();
                HbLog.Info("Combat", "Grabbed in Critical without a spike -> death");
                GameEvents.RaiseGrabResolved(false);
                return;
            }
            health.ApplyGrabWithoutSpike(spared);
            HbLog.Info("Combat", $"Grabbed without a spike -> {health.State}");
            GameEvents.RaiseGrabResolved(false);
        }
    }
}
