using System;
using Heartbeat.Config;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>Nivel de ruido que puede oír la IA.</summary>
    public enum NoiseLevel
    {
        Silent = 0,
        Low = 1,
        High = 2
    }

    /// <summary>Pulso de ruido: la IA lo escucha si está dentro de <see cref="Radius"/>.</summary>
    public struct NoisePulse
    {
        public readonly Vector3 Position;
        public readonly float Radius;
        public readonly NoiseLevel Level;
        public readonly float Time;

        public NoisePulse(Vector3 position, float radius, NoiseLevel level, float time)
        {
            Position = position;
            Radius = radius;
            Level = level;
            Time = time;
        }
    }

    /// <summary>
    /// Ruido que emite el jugador: agachado 0, caminar bajo, correr alto.
    /// Emite un pulso de radio configurable cada <c>noisePulseInterval</c> segundos
    /// mientras se mueve; la IA se suscribe a <see cref="Pulsed"/>.
    /// </summary>
    public class PlayerNoiseEmitter : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        [Tooltip("Registra cada pulso en consola.")]
        [SerializeField] private bool logPulses = true;

        private NoiseLevel level = NoiseLevel.Silent;
        private float radius;
        private float timer;

        /// <summary>Último pulso emitido, para consultas puntuales de la IA.</summary>
        public NoisePulse LastPulse { get; private set; }

        public NoiseLevel Level => level;
        public float CurrentRadius => radius;

        public event Action<NoisePulse> Pulsed;

        /// <summary>El motor informa el estado de cada frame.</summary>
        public void SetState(bool isCrouched, bool isRunning, bool isMoving)
        {
            if (config == null) return;

            if (!isMoving || isCrouched)
            {
                level = NoiseLevel.Silent;
                radius = isCrouched ? config.NoiseRadiusCrouch : 0f;
                return;
            }

            if (isRunning)
            {
                level = NoiseLevel.High;
                radius = config.NoiseRadiusRun;
            }
            else
            {
                level = NoiseLevel.Low;
                radius = config.NoiseRadiusWalk;
            }
        }

        private void Update()
        {
            if (config == null) return;

            if (level == NoiseLevel.Silent || radius <= 0f)
            {
                timer = 0f;
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f) return;

            timer = config.NoisePulseInterval;
            Emit();
        }

        private void Emit()
        {
            LastPulse = new NoisePulse(transform.position, radius, level, Time.time);
            if (logPulses)
            {
                Debug.Log($"[Ruido] {level} radio {radius:F1} m en {transform.position}");
            }
            Pulsed?.Invoke(LastPulse);
        }

        /// <summary>Pulso puntual con radio propio (disparos, golpes en el suelo, portazos).</summary>
        public void EmitPulse(float customRadius)
        {
            if (customRadius <= 0f) return;

            LastPulse = new NoisePulse(transform.position, customRadius, NoiseLevel.High, Time.time);
            if (logPulses)
            {
                Debug.Log($"[Ruido] Pulso radio {customRadius:F1} m en {transform.position}");
            }
            Pulsed?.Invoke(LastPulse);
        }
    }
}
