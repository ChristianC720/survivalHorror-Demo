using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>
    /// Hoja de puerta con bisagra. Mientras está bloqueada su collider impide el paso.
    /// Las reglas de llave/inventario se conectan más adelante desde <see cref="TryOpen"/>.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Header("Estado")]
        [Tooltip("Si está bloqueada, TryOpen registra ACCESS DENIED y no se abre.")]
        [SerializeField] private bool isLocked = true;

        [Tooltip("La llave obligatoria del piso abre esta puerta (SalaClaveFinal).")]
        [SerializeField] private bool requiresLevelKey = true;

        [Header("Animación")]
        [SerializeField] private float openAngle = 95f;
        [SerializeField] private float openSpeed = 220f;

        private bool isOpen;
        private float currentAngle;

        public bool IsLocked => isLocked;
        public bool IsOpen => isOpen;
        public bool RequiresLevelKey => requiresLevelKey;

        public bool TryOpen()
        {
            if (isOpen) return true;

            if (isLocked)
            {
                Debug.Log($"[Door] ACCESS DENIED — {name} requiere la llave obligatoria del piso.");
                return false;
            }

            isOpen = true;
            foreach (Collider leafCollider in GetComponentsInChildren<Collider>())
            {
                leafCollider.enabled = false;
            }
            Debug.Log($"[Door] {name} abierta.");
            return true;
        }

        public void Unlock()
        {
            if (!isLocked) return;
            isLocked = false;
            Debug.Log($"[Door] {name} desbloqueada.");
        }

        public void Lock()
        {
            isLocked = true;
        }

        private void Update()
        {
            if (!isOpen) return;

            currentAngle = Mathf.MoveTowards(currentAngle, openAngle, openSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
        }
    }
}
