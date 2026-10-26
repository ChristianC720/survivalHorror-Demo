using Heartbeat.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Motor del jugador sobre CharacterController: cápsula de 1.78 m, agacharse con Ctrl izquierdo
    /// (alternar), correr manteniendo Shift izquierdo y aviso de ruido a la IA.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerNoiseEmitter noise;

        [Tooltip("Apuntar reduce la velocidad y bloquea correr.")]
        [SerializeField] private PlayerAim aim;

        [Tooltip("El estado de salud reduce la velocidad (Herido 15 %, Crítico 30 %).")]
        [SerializeField] private PlayerHealth health;

        [Tooltip("Mientras la vista de vigilancia está activa no se corre y se camina muy lento.")]
        [SerializeField] private SurveillanceView surveillance;

        [Tooltip("Malla de la cápsula; se estira/encoge con la postura.")]
        [SerializeField] private Transform visual;

        [Tooltip("Transform que define hacia dónde se mueve el jugador (la cámara). Vacío = el propio.")]
        [SerializeField] private Transform movementReference;

        [Header("Depuración")]
        [SerializeField] private bool logStance = true;

        private const string ActionMapName = "Player";

        private CharacterController controller;
        private InputAction moveAction;
        private InputAction sprintAction;
        private InputAction crouchAction;

        private readonly Collider[] overlapBuffer = new Collider[8];
        private float verticalVelocity;
        private float currentSpeed;
        private bool isCrouched;
        private bool movementLocked;

        public bool IsCrouched => isCrouched;
        public bool IsRunning { get; private set; }

        /// <summary>Shift izquierdo pulsado, aunque la vista de vigilancia bloquee correr.</summary>
        public bool WantsSprint { get; private set; }

        public float CurrentSpeed => currentSpeed;
        public bool IsGrounded => controller != null && controller.isGrounded;

        /// <summary>Bloqueo externo de movimiento (hackeo): la cámara OTS sigue viva.</summary>
        public bool IsMovementLocked => movementLocked;

        /// <summary>Bloquea el desplazamiento sin tocar la postura ni la gravedad.</summary>
        public void SetMovementLocked(bool value)
        {
            movementLocked = value;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            if (stamina == null) stamina = GetComponent<PlayerStamina>();
            if (noise == null) noise = GetComponent<PlayerNoiseEmitter>();
            if (aim == null) aim = GetComponent<PlayerAim>();
            if (health == null) health = GetComponent<PlayerHealth>();
            if (surveillance == null) surveillance = GetComponent<SurveillanceView>();
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("[Player] Falta GameConfig en PlayerMotor.", this);
                return;
            }

            if (controller.radius != config.PlayerRadius)
            {
                controller.radius = config.PlayerRadius;
            }

            ApplyStance(instant: true);
        }

        private void OnEnable()
        {
            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null)
            {
                Debug.LogError($"[Player] El asset de input no tiene el mapa '{ActionMapName}'.", this);
                return;
            }

            moveAction = map.FindAction("Move", throwIfNotFound: false);
            sprintAction = map.FindAction("Sprint", throwIfNotFound: false);
            crouchAction = map.FindAction("Crouch", throwIfNotFound: false);

            moveAction?.Enable();
            sprintAction?.Enable();

            if (crouchAction != null)
            {
                crouchAction.Enable();
                crouchAction.performed += OnCrouchPerformed;
            }
        }

        private void OnDisable()
        {
            if (crouchAction != null)
            {
                crouchAction.performed -= OnCrouchPerformed;
                crouchAction.Disable();
            }

            moveAction?.Disable();
            sprintAction?.Disable();

            moveAction = null;
            sprintAction = null;
            crouchAction = null;
        }

        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            SetCrouched(!isCrouched);
        }

        private void Update()
        {
            if (config == null || controller == null) return;

            Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            if (input.sqrMagnitude > 1f) input.Normalize();

            if (movementLocked) input = Vector2.zero;

            bool moving = input.sqrMagnitude > 0.01f;
            bool sprintHeld = !movementLocked && sprintAction != null && sprintAction.IsPressed();
            WantsSprint = sprintHeld;

            bool sprintBlocked = aim != null && aim.BlocksSprint;

            // En la vista de vigilancia no se puede correr (pulsar Shift la cancela).
            bool heldBySurveillance = surveillance != null && surveillance.IsActive;

            IsRunning = false;
            if (!isCrouched && !sprintBlocked && !heldBySurveillance && moving && sprintHeld && stamina != null)
            {
                IsRunning = stamina.TrySpendSprint(Time.deltaTime);
            }

            float speed = isCrouched
                ? config.CrouchSpeed
                : (IsRunning ? config.RunSpeed : config.WalkSpeed);

            if (heldBySurveillance) speed *= config.SurveillanceSpeedMultiplier;

            if (aim != null) speed *= aim.SpeedMultiplier;

            if (health != null)
            {
                speed *= health.SpeedMultiplier;
                if (health.IsDead) speed = 0f;
            }

            Vector3 motion = BuildMotion(input);
            currentSpeed = motion.magnitude * speed;

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += config.Gravity * Time.deltaTime;

            Vector3 velocity = motion * speed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            if (noise != null) noise.SetState(isCrouched, IsRunning, moving);
        }

        private Vector3 BuildMotion(Vector2 input)
        {
            Transform reference = movementReference != null ? movementReference : transform;

            // Sólo el giro horizontal: la cámara mira arriba/abajo y eso no debe inclinar el movimiento.
            Quaternion yawRotation = Quaternion.Euler(0f, reference.eulerAngles.y, 0f);
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;

            Vector3 motion = right * input.x + forward * input.y;
            return motion.sqrMagnitude > 1f ? motion.normalized : motion;
        }

        /// <summary>Alterna la postura. Agacharse siempre se permite; levantarse sólo si hay hueco.</summary>
        public void SetCrouched(bool value)
        {
            if (isCrouched == value) return;

            if (!value && !HasHeadroom())
            {
                Debug.Log("[Player] No puede levantarse: techo encima.");
                return;
            }

            isCrouched = value;
            ApplyStance(instant: true);

            if (logStance) Debug.Log(isCrouched ? "[Player] Agachado." : "[Player] De pie.");
        }

        private void ApplyStance(bool instant)
        {
            if (config == null || controller == null) return;

            float height = isCrouched ? config.CrouchHeight : config.PlayerHeight;

            controller.height = height;
            controller.center = new Vector3(0f, height * 0.5f, 0f);
            controller.stepOffset = 0.3f;

            if (visual != null)
            {
                visual.localPosition = new Vector3(0f, height * 0.5f, 0f);
                visual.localScale = new Vector3(config.PlayerRadius * 2f, height * 0.5f, config.PlayerRadius * 2f);
            }
        }

        private bool HasHeadroom()
        {
            if (config == null || controller == null) return true;

            float radius = controller.radius * 0.95f;
            Vector3 bottom = transform.position + Vector3.up * (radius + 0.02f);
            Vector3 top = transform.position + Vector3.up * (config.PlayerHeight - radius - 0.02f);

            int count = Physics.OverlapCapsuleNonAlloc(bottom, top, radius, overlapBuffer, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                Collider other = overlapBuffer[i];
                if (other == null) continue;
                if (other.transform == transform || other.transform.IsChildOf(transform)) continue;
                return false;
            }

            return true;
        }
    }
}
