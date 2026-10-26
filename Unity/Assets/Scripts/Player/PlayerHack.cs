using System.Collections.Generic;
using Heartbeat.Config;
using Heartbeat.Items;
using Heartbeat.Level;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>Flecha de una secuencia de hackeo.</summary>
    public enum HackArrow
    {
        Arriba = 0,
        Abajo = 1,
        Izquierda = 2,
        Derecha = 3,
    }

    /// <summary>
    /// Hackeo por secuencias. Con E sobre un panel se inicia la sesión: el jugador queda inmóvil
    /// (la cámara OTS sigue viva, el juego NO se pausa) y debe introducir la secuencia de flechas
    /// con WASD/flechas. Un fallo la reinicia, recibir un golpe la cancela y E la abandona.
    /// E sobre una hoja cerrada levanta el sello o avisa de la llave.
    /// Corre antes que <see cref="PlayerFinisher"/> y <see cref="PlayerPickup"/> para que la E
    /// sepa a quién ceder. Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    [DefaultExecutionOrder(-11)]
    public class PlayerHack : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private PlayerMotor motor;
        [SerializeField] private PlayerHealth health;

        [Header("Depuración")]
        [Tooltip("Registra el inicio, el fallo y el resultado de cada hackeo en consola.")]
        [SerializeField] private bool logEvents = true;

        private readonly List<HackArrow> sequence = new List<HackArrow>();

        private HackPanel panelTarget;
        private SealableDoor doorTarget;
        private HackPanel activePanel;
        private int index;
        private float errorUntil;
        private bool hacking;

        /// <summary>Hay una secuencia en curso.</summary>
        public bool IsHacking => hacking;

        /// <summary>Hay un panel o una hoja cerrada apuntados y al alcance.</summary>
        public bool HasTarget => panelTarget != null || doorTarget != null;

        /// <summary>Flechas de la secuencia en curso.</summary>
        public IReadOnlyList<HackArrow> Sequence => sequence;

        /// <summary>Flechas ya introducidas correctamente.</summary>
        public int SequenceIndex => index;

        /// <summary>Se acaba de fallar: la UI lo resalta un instante.</summary>
        public bool IsWrongFlash => Time.time < errorUntil;

        /// <summary>La secuencia en curso es de un panel (si no, se está abriendo una hoja).</summary>
        public bool HasActivePanel => activePanel != null;

        /// <summary>Acción del panel en curso, para el rótulo de la UI.</summary>
        public string ActivePanelName => activePanel != null ? activePanel.ActionName : string.Empty;

        /// <summary>Distancia al centro de pantalla del objetivo actual (o infinito).</summary>
        public float CenterScore
        {
            get
            {
                if (panelTarget != null) return ViewportCenterDistance(panelTarget.transform.position);
                if (doorTarget != null) return ViewportCenterDistance(doorTarget.OpeningCenter);

                return float.MaxValue;
            }
        }

        /// <summary>Aviso "[E] ..." del objetivo actual (o null).</summary>
        public string Prompt
        {
            get
            {
                if (panelTarget != null) return "[E] " + panelTarget.ActionName;
                if (doorTarget == null) return null;

                if (doorTarget.IsLocked) return "[E] Puerta cerrada con llave";
                if (doorTarget.IsSealed) return "[E] Levantar el sello";
                return "[E] Puerta cerrada · hay que hackearla";
            }
        }

        private void Awake()
        {
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>();
            if (playerRoot == null) playerRoot = transform;
            if (motor == null) motor = GetComponent<PlayerMotor>();
            if (health == null) health = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            if (health != null) health.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            if (health != null) health.Damaged -= OnDamaged;

            if (hacking) Abort("componente desactivado");
        }

        private void OnDamaged(int hits)
        {
            Abort("golpe recibido");
        }

        private void Update()
        {
            DetectTarget();

            if (!hacking) return;

            if (health != null && health.IsDead)
            {
                Abort("jugador muerto");
                return;
            }

            ReadSequenceInput();
        }

        /// <summary>Distancia (viewport 0-1) del punto al centro de la pantalla.</summary>
        public float ViewportCenterDistance(Vector3 worldPosition)
        {
            if (viewCamera == null) return float.MaxValue;

            Vector3 viewport = viewCamera.WorldToViewportPoint(worldPosition);
            if (viewport.z <= 0f) return float.MaxValue;

            return new Vector2(viewport.x - 0.5f, viewport.y - 0.5f).magnitude;
        }

        /// <summary>El panel o la hoja apuntados ganan la E al objeto indicado si están más centrados.</summary>
        public bool HasPriorityOver(WorldItem item)
        {
            if (!HasTarget) return false;
            if (item == null) return true;

            return CenterScore < ViewportCenterDistance(item.transform.position);
        }

        /// <summary>E sobre el objetivo: inicia la secuencia del panel o interactúa con la hoja.</summary>
        public bool TryStart()
        {
            if (hacking) return false;
            if (panelTarget != null) return StartPanel(panelTarget);

            if (doorTarget != null)
            {
                doorTarget.Interact();
                return true;
            }

            return false;
        }

        /// <summary>Cancela la secuencia en curso y devuelve el movimiento al jugador.</summary>
        public void Abort(string reason)
        {
            if (!hacking) return;

            End();
            if (logEvents) Debug.Log($"[Hackeo] Cancelado: {reason}.");
        }

        private bool StartPanel(HackPanel panel)
        {
            sequence.Clear();

            int length = panel.RollSequenceLength();
            for (int i = 0; i < length; i++) sequence.Add((HackArrow)Random.Range(0, 4));

            index = 0;
            errorUntil = 0f;
            activePanel = panel;
            hacking = true;

            if (motor != null) motor.SetMovementLocked(true);

            if (logEvents) Debug.Log($"[Hackeo] {panel.ActionName}: secuencia de {length} flechas (WASD/flechas). E abandona.");
            return true;
        }

        private void Complete()
        {
            HackPanel panel = activePanel;
            End();

            if (panel == null) return;
            if (logEvents) Debug.Log($"[Hackeo] Secuencia correcta en {panel.name}.");

            panel.ApplyEffect();
        }

        private void End()
        {
            hacking = false;
            activePanel = null;
            index = 0;
            errorUntil = 0f;
            sequence.Clear();

            if (motor != null) motor.SetMovementLocked(false);
        }

        private void ReadSequenceInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (sequence.Count == 0 || index >= sequence.Count) return;

            HackArrow pressed;
            if (!TryReadArrow(keyboard, out pressed)) return;

            if (pressed == sequence[index])
            {
                index++;
                if (index >= sequence.Count) Complete();
                return;
            }

            index = 0;
            errorUntil = Time.time + (config != null ? config.HackErrorFlashDuration : 0f);

            if (logEvents) Debug.Log($"[Hackeo] Flecha equivocada ({pressed}): la secuencia vuelve a empezar.");
        }

        private static bool TryReadArrow(Keyboard keyboard, out HackArrow arrow)
        {
            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            {
                arrow = HackArrow.Arriba;
                return true;
            }

            if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                arrow = HackArrow.Abajo;
                return true;
            }

            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                arrow = HackArrow.Izquierda;
                return true;
            }

            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                arrow = HackArrow.Derecha;
                return true;
            }

            arrow = HackArrow.Arriba;
            return false;
        }

        private void DetectTarget()
        {
            if (hacking) return;

            panelTarget = null;
            doorTarget = null;

            if (config == null || viewCamera == null) return;
            if (health != null && health.IsDead) return;

            Transform reference = playerRoot != null ? playerRoot : transform;

            // Primero los paneles: son el objetivo principal.
            float best = config.HackAimRadius;
            IReadOnlyList<HackPanel> panels = HackPanel.All;

            for (int i = 0; i < panels.Count; i++)
            {
                HackPanel panel = panels[i];
                if (panel == null || !panel.IsUsable) continue;
                if (Vector3.Distance(reference.position, panel.transform.position) > config.HackRange) continue;

                float score = ViewportCenterDistance(panel.transform.position);
                if (score > best) continue;

                best = score;
                panelTarget = panel;
            }

            if (panelTarget != null) return;

            // Si no hay panel, la hoja cerrada más centrada (sello, llave o puerta sin hackear).
            best = config.HackAimRadius;
            IReadOnlyList<SealableDoor> doors = SealableDoor.All;

            for (int i = 0; i < doors.Count; i++)
            {
                SealableDoor door = doors[i];
                if (door == null || !door.IsClosed) continue;
                if (Vector3.Distance(reference.position, door.OpeningCenter) > config.HackRange) continue;

                float score = ViewportCenterDistance(door.OpeningCenter);
                if (score > best) continue;

                best = score;
                doorTarget = door;
            }
        }
    }
}
