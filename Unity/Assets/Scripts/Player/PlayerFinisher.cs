using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Config;
using Heartbeat.Items;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>
    /// Remate de enemigos caídos. Detecta el cuerpo más centrado en pantalla y dentro de
    /// <see cref="GameConfig.FinisherRange"/> y ofrece dos salidas: pisotón con el clic
    /// izquierdo (gasta el 25 % de la resistencia y hace ruido) o punzón con E
    /// (silencioso, consume un punzón). Un cuerpo rematado no se recicla nunca.
    /// Corre antes que <see cref="PlayerPickup"/> para que la E sepa a quién ceder.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class PlayerFinisher : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerNoiseEmitter noiseEmitter;

        [Header("Depuración")]
        [Tooltip("Registra la detección y el resultado de cada remate en consola.")]
        [SerializeField] private bool logEvents = true;

        private EnemyCorpse target;
        private EnemyCorpse actionTarget;
        private float actionTimer;
        private float actionDuration;
        private float wheezeUntil;
        private bool actionIsStomp;

        /// <summary>Cuerpo apuntado y al alcance (o null).</summary>
        public EnemyCorpse Target => target;

        public bool HasTarget => target != null;

        /// <summary>Distancia al centro de pantalla del cuerpo apuntado (o infinito).</summary>
        public float CenterScore => target != null ? ViewportCenterDistance(target.AimPoint) : float.MaxValue;

        /// <summary>Hay un remate en curso.</summary>
        public bool IsBusy => actionTimer > 0f;

        /// <summary>Avance 0-1 del remate en curso.</summary>
        public float ActionProgress01
        {
            get
            {
                if (!IsBusy || actionDuration <= 0f) return 0f;
                return Mathf.Clamp01(1f - actionTimer / actionDuration);
            }
        }

        /// <summary>El remate en curso es un pisotón (si no, punzón).</summary>
        public bool ActionIsStomp => actionIsStomp;

        /// <summary>Queda algún punzón para el remate silencioso.</summary>
        public bool HasSpike => inventory != null && inventory.CountOf(ItemType.Spike) > 0;

        private void Awake()
        {
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>();
            if (playerRoot == null) playerRoot = transform;
            if (health == null) health = GetComponent<PlayerHealth>();
            if (stamina == null) stamina = GetComponent<PlayerStamina>();
            if (inventory == null) inventory = GetComponent<PlayerInventory>();
            if (noiseEmitter == null) noiseEmitter = GetComponent<PlayerNoiseEmitter>();
        }

        private void Update()
        {
            DetectTarget();

            if (!IsBusy) return;

            actionTimer -= Time.deltaTime;
            if (actionTimer > 0f) return;

            Complete();
        }

        /// <summary>Distancia (viewport 0-1) del punto al centro de la pantalla.</summary>
        public float ViewportCenterDistance(Vector3 worldPosition)
        {
            if (viewCamera == null) return float.MaxValue;

            Vector3 viewport = viewCamera.WorldToViewportPoint(worldPosition);
            if (viewport.z <= 0f) return float.MaxValue;

            return new Vector2(viewport.x - 0.5f, viewport.y - 0.5f).magnitude;
        }

        /// <summary>El cuerpo apuntado gana la E al objeto indicado si está más centrado.</summary>
        public bool HasPriorityOver(WorldItem item)
        {
            if (target == null) return false;
            if (item == null) return true;

            return ViewportCenterDistance(target.AimPoint) < ViewportCenterDistance(item.transform.position);
        }

        /// <summary>Pisotón (clic izquierdo sin apuntar): ruidoso y con coste de resistencia.</summary>
        public bool TryStomp()
        {
            if (!CanAct("pisotón")) return false;
            if (target == null) return false;

            float cost = stamina != null ? stamina.Max * config.FinisherStompStaminaCost01 : 0f;

            if (stamina != null && !stamina.TrySpend(cost))
            {
                wheezeUntil = Time.time + config.FinisherWheezeLock;

                // El clic se consume igualmente: no debe degenerar en un empujón al aire.
                if (logEvents) Debug.Log($"[Remate] Jadeo: sin resistencia para el pisotón (necesita {cost:F0}, hay {stamina.Current:F0}).");
                return true;
            }

            if (stamina != null) stamina.ReportExertion();

            StartAction(target, config.FinisherStompDuration, true);
            if (logEvents) Debug.Log($"[Remate] Pisotón en marcha ({config.FinisherStompDuration:F1} s, {cost:F0} de resistencia).");
            return true;
        }

        /// <summary>Punzón (E): silencioso y consume un punzón del inventario.</summary>
        public bool TrySpike()
        {
            if (!CanAct("punzón")) return false;
            if (target == null) return false;

            if (inventory == null || !inventory.ConsumeOne(ItemType.Spike))
            {
                if (logEvents) Debug.Log("[Remate] Sin punzones: no hay remate silencioso.");
                return false;
            }

            StartAction(target, config.FinisherSpikeDuration, false);
            if (logEvents) Debug.Log($"[Remate] Punzón en marcha ({config.FinisherSpikeDuration:F1} s); queda(n) {inventory.CountOf(ItemType.Spike)}.");
            return true;
        }

        private bool CanAct(string actionName)
        {
            if (config == null) return false;
            if (health != null && health.IsDead) return false;
            if (IsBusy) return false;

            if (Time.time < wheezeUntil)
            {
                if (logEvents) Debug.Log($"[Remate] Sin aliento para el {actionName}.");
                return false;
            }

            return true;
        }

        private void StartAction(EnemyCorpse corpse, float duration, bool stomp)
        {
            actionTarget = corpse;
            actionDuration = Mathf.Max(0.01f, duration);
            actionTimer = actionDuration;
            actionIsStomp = stomp;
        }

        private void Complete()
        {
            EnemyCorpse corpse = actionTarget;
            bool stomp = actionIsStomp;

            actionTarget = null;
            actionTimer = 0f;
            actionDuration = 0f;

            if (corpse == null) return;

            Transform reference = playerRoot != null ? playerRoot : transform;
            if (Vector3.Distance(reference.position, corpse.Position) > config.FinisherRange)
            {
                if (logEvents) Debug.Log("[Remate] El cuerpo quedó fuera de alcance: el remate se pierde.");
                return;
            }

            if (!corpse.Finish(stomp)) return;

            // El pisotón se oye en toda la sala; el punzón no hace ruido.
            if (stomp && noiseEmitter != null) noiseEmitter.EmitPulse(config.FinisherStompNoiseRadius);
        }

        private void DetectTarget()
        {
            target = null;

            if (config == null || viewCamera == null) return;
            if (health != null && health.IsDead) return;
            if (IsBusy) return;

            IReadOnlyList<EnemyCorpse> corpses = EnemyCorpse.All;
            if (corpses.Count == 0) return;

            Transform reference = playerRoot != null ? playerRoot : transform;
            float best = float.MaxValue;

            for (int i = 0; i < corpses.Count; i++)
            {
                EnemyCorpse corpse = corpses[i];
                if (corpse == null || !corpse.CanBeFinished) continue;
                if (Vector3.Distance(reference.position, corpse.Position) > config.FinisherRange) continue;

                float score = ViewportCenterDistance(corpse.AimPoint);
                if (score > config.FinisherAimRadius) continue;
                if (score >= best) continue;

                best = score;
                target = corpse;
            }
        }
    }
}
