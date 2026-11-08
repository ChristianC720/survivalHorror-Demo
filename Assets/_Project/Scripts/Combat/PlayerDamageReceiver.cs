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

        private void OnEnable() => GameEvents.PlayerAttacked += OnAttacked;
        private void OnDisable() => GameEvents.PlayerAttacked -= OnAttacked;

        private void OnAttacked(AttackInfo attack)
        {
            if (health.Model.IsDead) return;
            if (!attack.IsGrab)
            {
                health.ApplyHit(attack.HitsToKill);
                return;
            }

            if (inventory.Model.TryConsume(ItemKind.Spike))
            {
                GrabsCountered++;
                HbLog.Info("Combat", "Grab countered: discharge spike consumed");
                if (attack.Source is IGrabber grabber) grabber.OnGrabCountered(balance.spikeStunSeconds);
                GameEvents.RaiseGrabResolved(true);
                return;
            }

            if (health.State == HealthState.Critical && !attack.NeverKills)
            {
                health.ApplyGrabWithoutSpike();
                HbLog.Info("Combat", "Grabbed in Critical without a spike -> death");
                GameEvents.RaiseGrabResolved(false);
                return;
            }
            health.ApplyGrabWithoutSpike(attack.NeverKills);
            HbLog.Info("Combat", $"Grabbed without a spike -> {health.State}");
            GameEvents.RaiseGrabResolved(false);
        }
    }
}
