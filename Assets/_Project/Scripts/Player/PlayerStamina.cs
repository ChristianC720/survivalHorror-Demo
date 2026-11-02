using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>MonoBehaviour adapter for <see cref="StaminaModel"/> (S-04).</summary>
    public sealed class PlayerStamina : MonoBehaviour
    {
        [SerializeField] private PlayerBalance balance;

        public StaminaModel Model { get; private set; }

        private void Awake()
        {
            Model = new StaminaModel(balance.regenDelay, balance.regenPerSecond, balance.exhaustionSeconds);
            Model.Exhausted += OnExhausted;
        }

        private void OnDestroy() => Model.Exhausted -= OnExhausted;

        private void Update() => Model.Tick(Time.deltaTime);

        /// <summary>Shove / ground strike. Plays the gasp cue on failure (E-11 hooks the cue).</summary>
        public bool TrySpend(float amount)
        {
            if (Model.TrySpend(amount)) return true;
            HbLog.Info("Stamina", $"Not enough stamina ({Model.Value:0}% < {amount:0}%) -> gasp");
            Sfx.Play("CUE_Lukas_Gasp", transform.position);   // M-06: the shove does not happen and Lukas gasps
            return false;
        }

        private static void OnExhausted()
        {
            HbLog.Info("Stamina", "Exhausted");
            GameEvents.RaiseStaminaExhausted();
        }
    }
}
