using System;
using Heartbeat.Config;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>
    /// Resistencia 0-100 del jugador: correr gasta 10 %/s, la regeneración arranca
    /// 3 s después del último esfuerzo y llegar a 0 provoca 2 s de agotamiento.
    /// Empujar y rematar cortan la regeneración llamando a <see cref="ReportExertion"/>.
    /// </summary>
    public class PlayerStamina : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        [Tooltip("Registra en consola los cambios de estado relevantes.")]
        [SerializeField] private bool logChanges = true;

        private float current;
        private float timeSinceExertion;
        private float exhaustionTimer;
        private bool wasExhausted;
        private bool wasRegenerating;

        /// <summary>Resistencia actual (0-100).</summary>
        public float Current => current;

        public float Max => config != null ? config.StaminaMax : 100f;

        /// <summary>0-1, útil para la UI.</summary>
        public float Normalized => Max > 0f ? Mathf.Clamp01(current / Max) : 0f;

        /// <summary>Agotado: no puede correr ni empujar.</summary>
        public bool IsExhausted => exhaustionTimer > 0f;

        public bool CanSprint => !IsExhausted && current > 0f;

        public bool CanPush => !IsExhausted;

        public event Action Exhausted;
        public event Action Recovered;

        private void Awake()
        {
            current = Max;
        }

        /// <summary>
        /// Consume resistencia por correr. Devuelve false si no puede correr
        /// (agotado o sin resistencia), para que el motor caiga a caminar.
        /// </summary>
        public bool TrySpendSprint(float deltaTime)
        {
            if (!CanSprint || config == null) return false;

            current = Mathf.Max(0f, current - config.StaminaDrainPerSecond * deltaTime);
            timeSinceExertion = 0f;

            if (current <= 0f) BeginExhaustion();
            return true;
        }

        /// <summary>
        /// Marca un esfuerzo puntual (empujar, rematar): reinicia el retardo de regeneración.
        /// </summary>
        public void ReportExertion()
        {
            timeSinceExertion = 0f;
        }

        /// <summary>
        /// Intenta gastar resistencia de golpe (empujón, remate). Devuelve false si no llega.
        /// </summary>
        public bool TrySpend(float amount)
        {
            if (amount <= 0f) return true;
            if (config == null || current < amount) return false;

            current -= amount;
            timeSinceExertion = 0f;

            if (current <= 0f) BeginExhaustion();
            return true;
        }

        private void BeginExhaustion()
        {
            if (config == null) return;

            exhaustionTimer = config.ExhaustionDuration;
            timeSinceExertion = 0f;
            if (logChanges) Debug.Log($"[Stamina] Agotado: {config.ExhaustionDuration:F1} s sin poder correr ni empujar.");
            Exhausted?.Invoke();
        }

        private void Update()
        {
            if (config == null) return;

            if (exhaustionTimer > 0f)
            {
                exhaustionTimer -= Time.deltaTime;

                // Mientras está agotado se trata como esfuerzo continuo: ni corre ni regenera.
                timeSinceExertion = 0f;

                if (exhaustionTimer <= 0f)
                {
                    exhaustionTimer = 0f;
                    if (logChanges) Debug.Log("[Stamina] Agotamiento terminado.");
                    Recovered?.Invoke();
                }
            }
            else
            {
                timeSinceExertion += Time.deltaTime;
            }

            bool regenerating = CanRegenerate();
            if (regenerating)
            {
                current = Mathf.Min(Max, current + config.StaminaRegenPerSecond * Time.deltaTime);
            }

            if (logChanges && regenerating != wasRegenerating)
            {
                wasRegenerating = regenerating;
                if (regenerating) Debug.Log($"[Stamina] Regenerando desde {current:F0}.");
            }

            wasExhausted = IsExhausted;
        }

        private bool CanRegenerate()
        {
            return !IsExhausted
                && current < Max
                && timeSinceExertion >= config.StaminaRegenDelay;
        }
    }
}
