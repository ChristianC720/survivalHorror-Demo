using System;
using Heartbeat.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Estado de apuntado con clic derecho mantenido. Mientras apunta, el motor
    /// aplica la penalización de velocidad y bloquea correr, y la retícula se muestra.
    /// También calcula la dispersión de la retícula: se cierra en ~1 s quieto y se
    /// abre al moverse, correr o recibir un golpe.
    /// </summary>
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private PlayerMotor motor;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Depuración")]
        [SerializeField] private bool logAim = true;

        private const string ActionMapName = "Player";

        private InputAction aimAction;
        private float hitKick;

        /// <summary>Apuntando con clic derecho.</summary>
        public bool IsAiming { get; private set; }

        /// <summary>Dispersión 0 (cerrada) a 1 (abierta).</summary>
        public float Spread01 { get; private set; }

        /// <summary>Multiplicador de velocidad que aplica el motor al apuntar.</summary>
        public float SpeedMultiplier => IsAiming && config != null ? config.AimSpeedMultiplier : 1f;

        /// <summary>Apuntando no se puede correr.</summary>
        public bool BlocksSprint => IsAiming;

        public event Action<bool> AimChanged;
        public event Action<float> HitKick;

        private void Awake()
        {
            if (motor == null) motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable()
        {
            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            aimAction = map.FindAction("Aim", throwIfNotFound: false);
            aimAction?.Enable();
        }

        private void OnDisable()
        {
            aimAction?.Disable();
            aimAction = null;
            IsAiming = false;
        }

        private void Update()
        {
            if (config == null) return;

            UpdateAiming();
            UpdateSpread(Time.deltaTime);
        }

        private void UpdateAiming()
        {
            bool aiming = aimAction != null && aimAction.IsPressed();
            if (aiming == IsAiming) return;

            IsAiming = aiming;
            if (logAim) Debug.Log(aiming ? "[Aim] Apuntando." : "[Aim] Dejó de apuntar.");
            AimChanged?.Invoke(aiming);
        }

        private void UpdateSpread(float deltaTime)
        {
            float target = 0f;

            if (motor != null && motor.CurrentSpeed > 0.05f)
            {
                target = motor.IsRunning ? config.ReticleRunOpen : config.ReticleMoveOpen;
            }

            hitKick = Mathf.Max(0f, hitKick - config.ReticleHitRecovery * deltaTime);
            target = Mathf.Max(target, hitKick);

            if (target > Spread01)
            {
                Spread01 = Mathf.MoveTowards(Spread01, target, config.ReticleOpenSpeed * deltaTime);
            }
            else if (config.ReticleCloseDuration > 0f)
            {
                Spread01 = Mathf.MoveTowards(Spread01, target, deltaTime / config.ReticleCloseDuration);
            }
            else
            {
                Spread01 = target;
            }
        }

        /// <summary>Un golpe abre la retícula de golpe (lo llamará el sistema de daño).</summary>
        public void NotifyHit()
        {
            if (config == null) return;

            hitKick = Mathf.Max(hitKick, config.ReticleHitOpen);
            HitKick?.Invoke(hitKick);
        }
    }
}
