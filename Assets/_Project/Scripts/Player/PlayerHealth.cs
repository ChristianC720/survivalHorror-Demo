using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>MonoBehaviour adapter for <see cref="HealthModel"/> (M-05, S-03).</summary>
    public sealed class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private PlayerBalance balance;

        public HealthModel Model { get; private set; }
        public HealthState State => Model.State;

        private void Awake()
        {
            Model = new HealthModel(balance.maxHealth);
            Model.StateChanged += OnStateChanged;
        }

        private void OnDestroy() => Model.StateChanged -= OnStateChanged;

        public void ApplyHit(int hitsToKill) => Model.ApplyHit(hitsToKill);

        public void ApplyGrabWithoutSpike() => Model.ApplyGrabWithoutSpike();

        private void OnStateChanged(HealthState state)
        {
            HbLog.Info("Health", $"State -> {state}");
            GameEvents.RaiseHealthStateChanged((int)state);
            if (state == HealthState.Dead) GameEvents.RaisePlayerDied(DeathCause.Health);
        }
    }
}
