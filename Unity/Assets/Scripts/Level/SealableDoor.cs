using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Config;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>
    /// Hoja de puerta con bisagra que además se puede sellar. Cerrada bloquea el paso de todos;
    /// sellada sigue cerrada y sólo la levanta el jugador con E sobre la hoja, o un enemigo que
    /// la golpee durante <see cref="GameConfig.SealBreakDuration"/> segundos seguidos.
    /// Una puerta con llave no se abre, no se sella y no se rompe a golpes.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class SealableDoor : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;

        [Tooltip("Hoja que gira con la bisagra. Vacío = primer collider hijo.")]
        [SerializeField] private Transform leaf;

        [Header("Estado inicial")]
        [Tooltip("Empieza cerrada y bloqueando el paso.")]
        [SerializeField] private bool startsClosed;

        [Tooltip("Empieza con la llave obligatoria del piso: no se abre, ni se sella, ni se rompe.")]
        [SerializeField] private bool startsLocked;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private static readonly List<SealableDoor> Registry = new List<SealableDoor>();

        /// <summary>Puertas de la escena (registro estático, purgado en OnEnable).</summary>
        public static IReadOnlyList<SealableDoor> All => Registry;

        private Vector3 closedLocalPosition;
        private Collider[] leafColliders;
        private float currentAngle;
        private float breachTimer;
        private bool closed;
        private bool locked;
        private bool sealedDoor;

        /// <summary>La hoja está cerrada y bloquea el paso.</summary>
        public bool IsClosed => closed;

        /// <summary>La hoja está sellada por un panel.</summary>
        public bool IsSealed => sealedDoor;

        /// <summary>La puerta necesita la llave obligatoria del piso.</summary>
        public bool IsLocked => locked;

        /// <summary>Centro del vano con la hoja cerrada (punto de puntería y de golpes).</summary>
        public Vector3 OpeningCenter { get; private set; }

        /// <summary>Avance 0-1 de la ruptura a golpes (para depuración).</summary>
        public float BreachProgress01 => config != null
            ? Mathf.Clamp01(breachTimer / Mathf.Max(0.01f, config.SealBreakDuration))
            : 0f;

        private void Awake()
        {
            if (config == null) Debug.LogError("[Puerta] Falta GameConfig en SealableDoor.", this);

            if (leaf == null)
            {
                Collider child = GetComponentInChildren<Collider>(true);
                if (child != null) leaf = child.transform;
            }

            if (leaf != null)
            {
                closedLocalPosition = leaf.localPosition;
                OpeningCenter = transform.position + transform.rotation * closedLocalPosition;
                leafColliders = leaf.GetComponentsInChildren<Collider>(true);
            }
            else
            {
                OpeningCenter = transform.position;
                leafColliders = GetComponentsInChildren<Collider>(true);
            }

            locked = startsLocked;
            currentAngle = 0f;
            transform.localRotation = Quaternion.identity;
            SetClosed(startsClosed, instant: true);
        }

        private void OnEnable()
        {
            Registry.RemoveAll(item => item == null);
            if (!Registry.Contains(this)) Registry.Add(this);
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        private void Update()
        {
            AnimateLeaf();
            if (closed) TickBreach();
        }

        /// <summary>Sella la puerta: la hoja se cierra de golpe y los enemigos no pueden pasar.</summary>
        public void Seal()
        {
            if (locked)
            {
                if (logEvents) Debug.Log($"[Puerta] {name} no se puede sellar: tiene la llave obligatoria.");
                return;
            }

            if (sealedDoor)
            {
                if (logEvents) Debug.Log($"[Puerta] {name} ya está sellada.");
                return;
            }

            sealedDoor = true;
            breachTimer = 0f;
            SetClosed(true, instant: false);

            if (logEvents) Debug.Log($"[Puerta] {name} sellada: los enemigos no pasan hasta que la cruces o la rompan.");
        }

        /// <summary>Abre la puerta tras un hackeo correcto (puerta estándar).</summary>
        public void OpenByHack()
        {
            if (locked) return;

            sealedDoor = false;
            breachTimer = 0f;
            SetClosed(false, instant: false);

            if (logEvents) Debug.Log($"[Puerta] {name} abierta por el hackeo.");
        }

        /// <summary>
        /// Interacción con E sobre la hoja: levanta el sello, avisa de la llave o recuerda que
        /// hay que hackearla.
        /// </summary>
        public void Interact()
        {
            if (locked)
            {
                if (logEvents) Debug.Log($"[Puerta] ACCESS DENIED — {name} requiere la llave obligatoria del piso.");
                return;
            }

            if (!sealedDoor)
            {
                if (logEvents) Debug.Log($"[Puerta] {name} está cerrada: hay que hackearla.");
                return;
            }

            sealedDoor = false;
            breachTimer = 0f;
            SetClosed(false, instant: false);

            if (logEvents) Debug.Log($"[Puerta] Sello levantado en {name}.");
        }

        private void SetClosed(bool value, bool instant)
        {
            closed = value;

            if (leafColliders != null)
            {
                for (int i = 0; i < leafColliders.Length; i++)
                {
                    if (leafColliders[i] != null) leafColliders[i].enabled = value;
                }
            }

            if (!instant) return;

            currentAngle = value ? 0f : OpenAngle;
            transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
        }

        private float OpenAngle => config != null ? config.DoorOpenAngle : 95f;

        private float OpenSpeed => config != null ? config.DoorOpenSpeed : 220f;

        private void AnimateLeaf()
        {
            float target = closed ? 0f : OpenAngle;
            if (Mathf.Approximately(currentAngle, target)) return;

            currentAngle = Mathf.MoveTowards(currentAngle, target, OpenSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
        }

        private void TickBreach()
        {
            if (config == null) return;

            EnemyDirector director = EnemyDirector.Instance;
            if (director == null) return;

            if (locked) return;

            IReadOnlyList<EnemyAI> enemies = director.Enemies;
            float rangeSqr = config.SealBreakRange * config.SealBreakRange;

            bool pressing = false;
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyAI enemy = enemies[i];
                if (enemy == null) continue;
                if (!IsHunting(enemy.State)) continue;
                if ((enemy.transform.position - OpeningCenter).sqrMagnitude > rangeSqr) continue;

                pressing = true;
                break;
            }

            if (!pressing) return;

            breachTimer += Time.deltaTime;
            if (breachTimer < config.SealBreakDuration) return;

            Break();
        }

        private void Break()
        {
            sealedDoor = false;
            breachTimer = 0f;
            SetClosed(false, instant: false);

            if (logEvents) Debug.Log($"[Puerta] {name} rota a golpes: los enemigos han abierto paso.");
        }

        private static bool IsHunting(EnemyState state)
        {
            return state == EnemyState.Busqueda || state == EnemyState.Caza || state == EnemyState.Atacar;
        }
    }
}
