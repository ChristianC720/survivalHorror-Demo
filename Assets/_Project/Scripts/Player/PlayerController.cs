using Heartbeat.Core;
using Heartbeat.InputRouting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Lukas's locomotion (M-06, M-09): crouch 1 / walk 1.8 / run 4.5 m/s, health multipliers, stamina drain while
    /// running, movement noise pulses. Camera-relative movement. No jump, no dodge (Controles).
    /// Other systems set <see cref="IsAiming"/> and <see cref="IsSurveilling"/>; this class never reads devices.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerBalance balance;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerHealth health;
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
            // Depenetrating from an enemy shoved Lukas through walls: bodies never push him, BlockedByEnemies stops him instead.
            controller.excludeLayers = LayerMask.GetMask("Enemy", "Corpse", "Hitbox");
            enemyMask = LayerMask.GetMask("Enemy");
            wallMask = LayerMask.GetMask("Environment");
            groundMask = ~LayerMask.GetMask("Player", "Enemy", "Corpse", "Hitbox", "Interactable", "RoomVolume");
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
                actions.Crouch.canceled += OnCrouchReleased;
                GameEvents.OptionsChanged += ReadOptions;
                ReadOptions();
                hasInput = true;
            }
            else HbLog.Error("Player", "InputRouter not registered (is Bootstrap loaded?)");
        }

        private void OnDestroy()
        {
            if (!hasInput) return;
            GameEvents.OptionsChanged -= ReadOptions;
            InputAction crouch;
            try { crouch = actions.Crouch; }
            catch (System.NullReferenceException) { return; }   // input wrapper torn down first (leaving Play mode, recompile)
            if (crouch == null) return;
            crouch.performed -= OnCrouch;
            crouch.canceled -= OnCrouchReleased;
        }

        private bool crouchToggle = true;

        private void ReadOptions()
        {
            if (Services.TryGet(out Save.SaveService saves)) crouchToggle = saves.LoadGlobal().crouchToggle;
        }

        /// <summary>Hold mode (Options ▸ Controls): releasing the key stands Lukas up when there is room.</summary>
        private void OnCrouchReleased(InputAction.CallbackContext _)
        {
            if (!crouchToggle && IsCrouched && CanStandUp()) SetCrouched(false);
        }

        private void OnCrouch(InputAction.CallbackContext _)
        {
            if (!crouchToggle) { if (!IsCrouched) SetCrouched(true); return; }
            if (IsCrouched && !CanStandUp()) return;
            SetCrouched(!IsCrouched);
        }

        private void Update()
        {
            Vector2 move = MovementLocked ? Vector2.zero : DebugMove ?? (hasInput ? actions.Move.ReadValue<Vector2>() : Vector2.zero);
            bool wantsRun = DebugMove.HasValue ? DebugRun : hasInput && actions.Sprint.IsPressed();

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
            horizontal = BlockedByEnemies(horizontal);
            Velocity = horizontal + Vector3.up * verticalVelocity;
            Vector3 before = transform.position;
            controller.Move(Velocity * Time.deltaTime);
            if (!GuardTunnelling(before)) GuardFallOut();

            // Lukas always faces the camera (look input), not only when aiming: the flashlight never points into his
            // own body and movement strafes / backpedals relative to the view.
            if (!IsAiming && !MovementLocked && cameraTransform != null)
            {
                Vector3 look = cameraTransform.forward; look.y = 0f;
                if (look.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(look), balance.turnSpeed * Time.deltaTime);
            }

            EmitMovementNoise(moving);
        }

        // Safety net, not balance: any physics tunnelling out of the floor would otherwise fall forever (blank screen).
        private const float FallOutDepth = 10f;
        private Vector3 lastGrounded;
        private bool hasGrounded;

        private const float VoidProbe = 40f, EnemyGap = 0.05f;
        private int enemyMask, wallMask, groundMask;
        private readonly Collider[] nearEnemies = new Collider[8];

        /// <summary>Removes the part of the movement that walks into a living enemy; going through CharacterController.Move keeps walls solid.</summary>
        private Vector3 BlockedByEnemies(Vector3 move)
        {
            int n = Physics.OverlapSphereNonAlloc(controller.bounds.center, controller.radius + EnemyGap, nearEnemies, enemyMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < n; i++)
            {
                Vector3 away = transform.position - nearEnemies[i].transform.position;
                away.y = 0f;
                if (away.sqrMagnitude < 0.0001f) continue;
                away.Normalize();
                float into = Vector3.Dot(move, away);
                if (into < 0f) move -= away * into;
            }
            return move;
        }

        /// <summary>Lukas's centre can never legally cross a wall in one Move: whatever pushed him, undo it.</summary>
        private bool GuardTunnelling(Vector3 before)
        {
            Vector3 chest = Vector3.up * (controller.height * 0.5f);
            if (!Physics.Linecast(before + chest, transform.position + chest, wallMask, QueryTriggerInteraction.Ignore)) return false;
            HbLog.Warn("Player", $"Pushed through a wall at {transform.position}: back to {before}");
            DebugTeleport(before.x, before.y, before.z, transform.eulerAngles.y);
            return true;
        }

        private void GuardFallOut()
        {
            if (controller.isGrounded) { lastGrounded = transform.position; hasGrounded = true; return; }
            bool overVoid = !Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, VoidProbe, groundMask, QueryTriggerInteraction.Ignore);
            if (!hasGrounded || (!overVoid && transform.position.y > lastGrounded.y - FallOutDepth)) return;
            HbLog.Warn("Player", $"Fell out of the level at {transform.position}: back to {lastGrounded}");
            verticalVelocity = 0f;
            DebugTeleport(lastGrounded.x, lastGrounded.y, lastGrounded.z, transform.eulerAngles.y);
        }

        /// <summary>Tests: when set, replaces the Move input (camera-relative stick) and <see cref="DebugRun"/> the Sprint input.</summary>
        public Vector2? DebugMove { get; set; }
        public bool DebugRun { get; set; }

        /// <summary>Tests: crouch or stand up as the Crouch input would.</summary>
        public void DebugCrouch(bool on) => SetCrouched(on);

        /// <summary>Debug console / tests: teleport Lukas and face yaw degrees.</summary>
        public void DebugTeleport(float x, float y, float z, float yaw)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.Euler(0f, yaw, 0f));
            controller.enabled = true;
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
    }
}
