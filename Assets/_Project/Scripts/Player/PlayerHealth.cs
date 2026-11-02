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

        /// <summary>Debug console / scripted playtests: hits and grabs still land (events, interrupts) but do no damage.
        /// Resets with the Player on every scene load; compiled out of release builds.</summary>
        public bool DebugInvulnerable { get; set; }

        public void ApplyHit(int hitsToKill)
        {
            if (!Invulnerable) Model.ApplyHit(hitsToKill);
            Hit?.Invoke();
        }

        public void ApplyGrabWithoutSpike()
        {
            if (!Invulnerable) Model.ApplyGrabWithoutSpike();
            Hit?.Invoke();
        }

#if UNITY_EDITOR || DEBUG
        private bool Invulnerable => DebugInvulnerable;
#else
        private bool Invulnerable => false;
#endif

        private void OnStateChanged(HealthState state)
        {
            HbLog.Info("Health", $"State -> {state}");
            GameEvents.RaiseHealthStateChanged((int)state);
            if (state == HealthState.Dead) GameEvents.RaisePlayerDied(DeathCause.Health);
        }
    }
}
