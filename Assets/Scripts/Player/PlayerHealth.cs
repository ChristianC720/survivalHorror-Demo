using System;
using Heartbeat.Config;
using Heartbeat.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>Tres estados de salud, sin regeneración.</summary>
    public enum HealthState
    {
        /// <summary>Bien: velocidad completa.</summary>
        Good = 0,

        /// <summary>Herido: velocidad reducida.</summary>
        Hurt = 1,

        /// <summary>Crítico: velocidad muy reducida y el siguiente golpe mata.</summary>
        Critical = 2
    }

    /// <summary>Tipo de ataque enemigo; cada uno resta un número configurable de golpes.</summary>
    public enum EnemyHitType
    {
        Light = 0,
        Heavy = 1,
        Brutal = 2
    }

    /// <summary>
    /// Salud del jugador como reserva de "golpes". Sin regeneración: sólo la cura
    /// sube un estado (1.5 s de canalización, cancelable sin consumirse al recibir
    /// un golpe). En Crítico cualquier golpe mata.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private InputActionAsset inputActions;

        [Tooltip("Cámara que vibra al recibir daño.")]
        [SerializeField] private ThirdPersonCamera cameraRig;

        [Tooltip("Si hay inventario, las curas salen de sus casillas.")]
        [SerializeField] private PlayerInventory inventory;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";

        private InputAction healAction;
        private int currentHits;
        private int healCharges;
        private float healTimer;
        private bool isHealing;

        public HealthState State { get; private set; }
        public bool IsDead { get; private set; }
        public int CurrentHits => currentHits;
        public int MaxHits => config != null ? config.PlayerMaxHits : 0;
        public float Health01 => MaxHits > 0 ? (float)currentHits / MaxHits : 0f;
        public int HealCharges => inventory != null ? inventory.CountOf(ItemType.Heal) : healCharges;
        public bool IsHealing => isHealing;

        public float HealProgress01 => config != null && config.HealDuration > 0f
            ? Mathf.Clamp01(healTimer / config.HealDuration)
            : 0f;

        /// <summary>No se puede usar la cura estando Bien, muerto, canalizando o sin curas.</summary>
        public bool CanHeal => !IsDead && !isHealing && HealCharges > 0 && State != HealthState.Good;

        /// <summary>Multiplicador de velocidad según el estado.</summary>
        public float SpeedMultiplier
        {
            get
            {
                if (config == null) return 1f;

                switch (State)
                {
                    case HealthState.Hurt: return config.HurtSpeedMultiplier;
                    case HealthState.Critical: return config.CriticalSpeedMultiplier;
                    default: return 1f;
                }
            }
        }

        /// <summary>Golpes recibidos.</summary>
        public event Action<int> Damaged;

        /// <summary>Cambio de estado (también al morir).</summary>
        public event Action<HealthState> StateChanged;

        /// <summary>La cura terminó y subió un estado.</summary>
        public event Action<HealthState> Healed;

        public event Action Died;

        private void Awake()
        {
            if (cameraRig == null) cameraRig = GetComponentInChildren<ThirdPersonCamera>();

            if (config != null)
            {
                currentHits = config.PlayerMaxHits;
                healCharges = config.PlayerStartHeals;
            }

            State = EvaluateState(currentHits);
            isHealing = false;
        }

        private void OnEnable()
        {
            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            healAction = map.FindAction("Heal", throwIfNotFound: false);
            if (healAction == null)
            {
                Debug.LogWarning("[Salud] Falta la acción 'Heal' en el mapa Player.", this);
                return;
            }

            healAction.Enable();
            healAction.performed += OnHealPerformed;
        }

        private void OnDisable()
        {
            if (healAction != null)
            {
                healAction.performed -= OnHealPerformed;
                healAction.Disable();
            }
            healAction = null;
        }

        private void Update()
        {
            if (!isHealing) return;
            if (config == null) return;

            healTimer += Time.deltaTime;
            if (healTimer < config.HealDuration) return;

            CompleteHeal();
        }

        private void OnHealPerformed(InputAction.CallbackContext context)
        {
            TryStartHeal();
        }

        /// <summary>Recibe un golpe enemigo del tipo indicado.</summary>
        public void ApplyHit(EnemyHitType type)
        {
            ApplyHit(HitsFor(type), type);
        }

        /// <summary>
        /// Baja exactamente un estado de salud (lo usa el agarre enemigo): Bien → Herido → Crítico → muere.
        /// No pasa por la tabla de golpes: el estado es el que manda.
        /// </summary>
        public bool DropOneState()
        {
            if (IsDead || config == null) return false;

            if (isHealing) CancelHeal();

            if (State == HealthState.Critical)
            {
                if (logEvents) Debug.Log("[Salud] Agarre en crítico: muere.");
                Die();
                return true;
            }

            HealthState target = State == HealthState.Good ? HealthState.Hurt : HealthState.Critical;
            float ratio = target == HealthState.Hurt ? config.PlayerHurtRatio : config.PlayerCriticalRatio;

            int hits = Mathf.Clamp(Mathf.FloorToInt(MaxHits * ratio), 1, MaxHits);
            int lost = Mathf.Max(1, currentHits - hits);
            currentHits = hits;

            SetState(EvaluateState(currentHits));
            ShakeCamera();

            if (logEvents)
            {
                Debug.Log($"[Salud] Agarre: baja un estado (-{lost}) → {StateName(State)}, {currentHits}/{MaxHits} golpes.");
            }

            Damaged?.Invoke(lost);
            return true;
        }

        /// <summary>Recibe un golpe que resta el número de golpes indicado.</summary>
        public void ApplyHit(int hits)
        {
            ApplyHit(hits, EnemyHitType.Light);
        }

        /// <summary>Recibe un golpe concreto, sin pasar por la tabla de tipos.</summary>
        public void ApplyHit(int hits, EnemyHitType type)
        {
            if (IsDead || config == null) return;

            // Un golpe cancela la cura en curso sin consumir la carga.
            if (isHealing) CancelHeal();

            if (State == HealthState.Critical)
            {
                if (logEvents) Debug.Log($"[Salud] Golpe {type} en crítico: muere.");
                Die();
                return;
            }

            int applied = Mathf.Max(1, hits);
            currentHits = Mathf.Max(0, currentHits - applied);

            SetState(EvaluateState(currentHits));
            ShakeCamera();

            if (logEvents)
            {
                Debug.Log($"[Salud] Golpe {type} (-{applied}): quedan {currentHits}/{MaxHits} golpes → {StateName(State)}.");
            }

            Damaged?.Invoke(applied);

            if (currentHits <= 0) Die();
        }

        /// <summary>Inicia la cura. Devuelve false si no se puede usar.</summary>
        public bool TryStartHeal()
        {
            if (config == null) return false;

            if (IsDead) return false;
            if (State == HealthState.Good)
            {
                if (logEvents) Debug.Log("[Salud] La cura no se puede usar estando Bien.");
                return false;
            }

            if (isHealing) return false;

            // Las curas salen del inventario cuando existe; si no, del contador propio.
            if (HealCharges <= 0)
            {
                if (logEvents) Debug.Log("[Salud] Sin curas.");
                return false;
            }

            isHealing = true;
            healTimer = 0f;
            if (logEvents) Debug.Log($"[Salud] Curando ({StateName(State)})... {config.HealDuration:F1} s.");
            return true;
        }

        /// <summary>Cancela la cura en curso sin gastar la carga.</summary>
        public void CancelHeal()
        {
            if (!isHealing) return;

            isHealing = false;
            healTimer = 0f;
            if (logEvents) Debug.Log("[Salud] Cura cancelada (no se consume).");
        }

        private void CompleteHeal()
        {
            isHealing = false;
            healTimer = 0f;

            if (inventory != null) inventory.ConsumeOne(ItemType.Heal);
            else healCharges = Mathf.Max(0, healCharges - 1);

            HealthState target = State == HealthState.Critical ? HealthState.Hurt : HealthState.Good;

            // Sube hasta el primer valor de golpes que cae dentro del estado superior.
            int hits = Mathf.FloorToInt(MaxHits * config.PlayerHurtRatio);
            if (target == HealthState.Good) hits += 1;

            currentHits = Mathf.Clamp(hits, 1, MaxHits);
            SetState(target);

            if (logEvents)
            {
                Debug.Log($"[Salud] Curada: {StateName(State)}, {currentHits}/{MaxHits} golpes, quedan {HealCharges} curas.");
            }

            Healed?.Invoke(State);
        }

        private void Die()
        {
            if (IsDead) return;

            IsDead = true;
            currentHits = 0;
            isHealing = false;
            healTimer = 0f;

            ShakeCamera();

            if (logEvents) Debug.Log("[Salud] Muerto.");
            Died?.Invoke();
        }

        private void ShakeCamera()
        {
            if (cameraRig == null || config == null) return;

            cameraRig.Shake(config.DamageShakeMagnitude, config.DamageShakeAngle, config.DamageShakeDuration);
        }

        private int HitsFor(EnemyHitType type)
        {
            if (config == null) return 1;

            switch (type)
            {
                case EnemyHitType.Heavy: return Mathf.Max(1, config.HitHeavyHits);
                case EnemyHitType.Brutal: return Mathf.Max(1, config.HitBrutalHits);
                default: return Mathf.Max(1, config.HitLightHits);
            }
        }

        private HealthState EvaluateState(int hits)
        {
            if (config == null || MaxHits <= 0) return HealthState.Good;

            float ratio = (float)hits / MaxHits;
            if (ratio <= config.PlayerCriticalRatio) return HealthState.Critical;
            if (ratio <= config.PlayerHurtRatio) return HealthState.Hurt;
            return HealthState.Good;
        }

        private void SetState(HealthState value)
        {
            if (State == value) return;

            State = value;
            StateChanged?.Invoke(State);
        }

        private static string StateName(HealthState state)
        {
            switch (state)
            {
                case HealthState.Hurt: return "Herido";
                case HealthState.Critical: return "Crítico";
                default: return "Bien";
            }
        }
    }
}
