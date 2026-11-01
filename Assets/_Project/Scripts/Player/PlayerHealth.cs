using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>MonoBehaviour adapter for <see cref="HealthModel"/> (M-05, S-03).</summary>
    public sealed class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private PlayerBalance balance;

        public HealthModel Model { get; private set; }

        /// <summary>Raised on every damaging hit or grab (cancels heal animations, interrupts hacking).</summary>
        public event System.Action Hit;
        public HealthState State => Model.State;

        private void Awake()
        {
            Model = new HealthModel(balance.maxHealth);
            Model.StateChanged += OnStateChanged;
        }

        private void OnDestroy() => Model.StateChanged -= OnStateChanged;

        public void ApplyHit(int hitsToKill)
        {
            Model.ApplyHit(hitsToKill);
            Hit?.Invoke();
        }

        public void ApplyGrabWithoutSpike()
        {
            Model.ApplyGrabWithoutSpike();
            Hit?.Invoke();
        }

        private void OnStateChanged(HealthState state)
        {
            HbLog.Info("Health", $"State -> {state}");
            GameEvents.RaiseHealthStateChanged((int)state);
            if (state == HealthState.Dead) GameEvents.RaisePlayerDied(DeathCause.Health);
        }
    }
}
