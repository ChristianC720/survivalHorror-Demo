using Heartbeat.Core;
using Heartbeat.InputRouting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Lukas's locomotion (M-06, M-09): crouch 0.8 / walk 1.5 / run 4 m/s, health multipliers, stamina drain while
    /// running, movement noise pulses. Camera-relative movement. No jump, no dodge (Controles).
    /// Other systems set <see cref="IsAiming"/> and <see cref="IsSurveilling"/>; this class never reads devices.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerBalance balance;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerHealth health;
        [Tooltip("Placeholder mesh root (PH_Lukas). Used for crouch / injured posture until the animator exists.")]
        [SerializeField] private Transform visual;
        [Tooltip("Camera used for camera-relative movement. Defaults to Camera.main at start.")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private HeartbeatInput.GameplayActions actions;
        private bool hasInput;
        private float verticalVelocity;
        private float noiseTimer;

        public bool IsCrouched { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsAiming { get; set; }
        public bool IsSurveilling { get; set; }
        /// <summary>When true (hacking, cinematics, heal animation lock) the character does not move.</summary>
        public bool MovementLocked { get; set; }
        public float CurrentSpeed { get; private set; }
        /// <summary>Camera used for camera-relative movement (set by the OTS camera rig).</summary>
        public Transform CameraTransform { get => cameraTransform; set => cameraTransform = value; }
        public Vector3 Velocity { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            controller.height = balance.standHeight;
            controller.center = new Vector3(0f, balance.standHeight * 0.5f, 0f);
        }

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
            if (Services.TryGet<InputRouter>(out var router))
            {
                actions = router.Actions.Gameplay;
                actions.Crouch.performed += OnCrouch;
                hasInput = true;
            }
            else HbLog.Error("Player", "InputRouter not registered (is Bootstrap loaded?)");
        }

        private void OnDestroy()
        {
            if (hasInput) actions.Crouch.performed -= OnCrouch;
        }

        private void OnCrouch(InputAction.CallbackContext _)
        {
            if (IsCrouched && !CanStandUp()) return;
            SetCrouched(!IsCrouched);
        }

        private void Update()
        {
            Vector2 move = hasInput && !MovementLocked ? actions.Move.ReadValue<Vector2>() : Vector2.zero;
            bool wantsRun = hasInput && actions.Sprint.IsPressed();

            Vector3 dir = CameraRelative(move);
            bool moving = dir.sqrMagnitude > 0.0001f;

            IsRunning = moving && wantsRun && !IsAiming && stamina.Model.CanRun;
            if (IsRunning && IsCrouched && CanStandUp()) SetCrouched(false);   // running stands Lukas up
            if (IsRunning) stamina.Model.Drain(balance.sprintCostPerSecond, Time.deltaTime);

            float speed = BaseSpeed() * HealthMultiplier();
            CurrentSpeed = moving ? speed * Mathf.Clamp01(move.magnitude) : 0f;

            Vector3 horizontal = dir.normalized * CurrentSpeed;
            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += balance.gravity * Time.deltaTime;
            Velocity = horizontal + Vector3.up * verticalVelocity;
            controller.Move(Velocity * Time.deltaTime);

            if (moving && !IsAiming)
            {
                var target = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, balance.turnSpeed * Time.deltaTime);
            }

            EmitMovementNoise(moving);
            ApplyPosture();
        }

        private float BaseSpeed()
        {
            if (IsSurveilling) return balance.surveillanceSpeed;
            if (IsRunning) return balance.runSpeed;
            if (IsCrouched) return balance.crouchSpeed;
            return IsAiming ? balance.walkSpeed * balance.aimSpeedMultiplier : balance.walkSpeed;
        }

        private float HealthMultiplier()
        {
            switch (health.State)
            {
                case HealthState.Injured: return balance.injuredSpeedMultiplier;
                case HealthState.Critical: return balance.criticalSpeedMultiplier;
                default: return 1f;
            }
        }

        private Vector3 CameraRelative(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f) return Vector3.zero;
            Vector3 fwd = cameraTransform != null ? cameraTransform.forward : transform.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;
            fwd.y = 0f; right.y = 0f;
            fwd.Normalize(); right.Normalize();
            return fwd * input.y + right * input.x;
        }

        private void EmitMovementNoise(bool moving)
        {
            noiseTimer -= Time.deltaTime;
            if (!moving || IsCrouched || noiseTimer > 0f) return;   // crouched = 0 noise (M-09)
            noiseTimer = balance.noiseInterval;
            float radius = IsRunning ? balance.runNoiseRadius : balance.walkNoiseRadius;
            GameEvents.RaiseNoiseEmitted(new NoiseInfo(transform.position, radius, IsRunning ? NoiseSource.Running : NoiseSource.Footsteps));
        }

        private void SetCrouched(bool crouched)
        {
            IsCrouched = crouched;
            float h = crouched ? balance.crouchHeight : balance.standHeight;
            controller.height = h;
            controller.center = new Vector3(0f, h * 0.5f, 0f);
            HbLog.Dev("Player", crouched ? "Crouch" : "Stand");
        }

        private bool CanStandUp()
        {
            float r = controller.radius * 0.95f;
            Vector3 bottom = transform.position + Vector3.up * (balance.crouchHeight - r);
            Vector3 top = transform.position + Vector3.up * (balance.standHeight - r);
            return !Physics.CheckCapsule(bottom, top, r, LayerMask.GetMask("Environment"), QueryTriggerInteraction.Ignore);
        }

        /// <summary>Placeholder posture until the animator exists: crouch squash and a forward lean when hurt.</summary>
        private void ApplyPosture()
        {
            if (visual == null) return;
            float yScale = IsCrouched ? balance.crouchHeight / balance.standHeight : 1f;
            float lean = health.State == HealthState.Critical ? 18f : health.State == HealthState.Injured ? 8f : 0f;
            visual.localScale = Vector3.Lerp(visual.localScale, new Vector3(1f, yScale, 1f), 12f * Time.deltaTime);
            visual.localRotation = Quaternion.Slerp(visual.localRotation, Quaternion.Euler(lean, 0f, 0f), 6f * Time.deltaTime);
        }
    }
}
