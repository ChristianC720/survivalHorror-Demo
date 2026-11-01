using UnityEngine;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Blanco de pruebas mientras no existe la IA: cuenta golpes, registra cada impacto
    /// y acepta empujones. Sirve para validar el revólver y el empujón en el nivel.
    /// </summary>
    public class DamageDummy : MonoBehaviour, IDamageable, IStaggerable
    {
        [Header("Configuración")]
        [Tooltip("Golpes que aguanta antes de caer.")]
        [SerializeField] private int maxHits = 4;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private Vector3 startPosition;

        /// <summary>Golpes recibidos.</summary>
        public int Hits { get; private set; }

        public bool IsDead => Hits >= maxHits;

        private void Awake()
        {
            startPosition = transform.position;
        }

        public void ApplyDamage(int amount, Vector3 point)
        {
            if (IsDead) return;

            Hits += Mathf.Max(1, amount);
            if (logEvents)
            {
                Debug.Log($"[Blanco] {name}: golpe en {point} → {Hits}/{maxHits}.");
            }

            if (IsDead && logEvents) Debug.Log($"[Blanco] {name}: abatido.");
        }

        public void ApplyStagger(Vector3 direction, float strength)
        {
            // Desplazamiento simple para que el empujón sea observable.
            transform.position += direction * strength * 0.1f;
            if (logEvents)
            {
                Debug.Log($"[Blanco] {name}: empujado {strength:F1} hacia {direction} (desde {startPosition}).");
            }
        }
    }
}
