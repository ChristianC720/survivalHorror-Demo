using System;
using UnityEngine;

namespace Project.Player
{
    /// <summary>
    /// Unity integration for player locomotion: feeds injected input into <see cref="PlayerMotor"/> and
    /// applies the result through a <see cref="CharacterController"/>. Disabled until <see cref="Initialize"/>.
    /// Never owns or enables input action maps.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        // Technical threshold for reporting IsMoving, not a gameplay value.
        private const float MovingSpeedThreshold = 0.01f;

        [SerializeField] private MovementSettings _settings;

        private CharacterController _characterController;
        private PlayerMotor _motor;
        private ReferenceFrameLock _frameLock;
        private IPlayerMoveInput _input;
        private IMovementReferenceFrame _referenceFrame;

        /// <summary>Actual velocity applied by the character controller during the last step.</summary>
        public Vector3 Velocity { get; private set; }

        /// <summary>Horizontal speed in m/s.</summary>
        public float Speed { get; private set; }

        public bool IsMoving { get; private set; }

        public bool IsGrounded { get; private set; }

        /// <summary>Flat, unit-length facing direction.</summary>
        public Vector3 Facing { get; private set; }

        public bool IsInitialized => _input != null;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            enabled = false;
            Facing = FlatForward();

            if (_settings == null)
            {
                Debug.LogError($"{nameof(PlayerController)} on '{name}' has no {nameof(MovementSettings)} assigned.", this);
                return;
            }

            try
            {
                _settings.Validate();
            }
            catch (InvalidOperationException exception)
            {
                Debug.LogException(exception, this);
                return;
            }

            ApplyCapsule(_settings);
            _motor = new PlayerMotor(_settings, Facing);
            _frameLock = new ReferenceFrameLock(_settings.InputDeadzone, _settings.DirectionChangeThreshold);
        }

        /// <summary>Injects dependencies and starts locomotion.</summary>
        public void Initialize(IPlayerMoveInput input, IMovementReferenceFrame referenceFrame)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (referenceFrame == null) throw new ArgumentNullException(nameof(referenceFrame));
            if (_motor == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(PlayerController)} on '{name}' is not configured; see the earlier settings error.");
            }

            if (_input != null)
            {
                throw new InvalidOperationException($"{nameof(PlayerController)} on '{name}' is already initialized.");
            }

            _input = input;
            _referenceFrame = referenceFrame;
            enabled = true;
        }

        private void Update()
        {
            var move = _input.Move;
            var frame = _frameLock.Resolve(move, _referenceFrame.Current);
            var result = _motor.Step(new MotorInput(move, _input.IsRunHeld), frame, _characterController.isGrounded,
                Time.deltaTime);

            _characterController.Move(result.Displacement);
            transform.rotation = Quaternion.LookRotation(result.Facing, Vector3.up);

            Velocity = _characterController.velocity;
            Speed = new Vector3(Velocity.x, 0f, Velocity.z).magnitude;
            IsMoving = Speed > MovingSpeedThreshold;
            IsGrounded = _characterController.isGrounded;
            Facing = result.Facing;
        }

        private void ApplyCapsule(MovementSettings settings)
        {
            _characterController.height = settings.CapsuleHeight;
            _characterController.radius = settings.CapsuleRadius;
            _characterController.center = settings.CapsuleCenter;
            _characterController.stepOffset = settings.StepOffset;
            _characterController.skinWidth = settings.SkinWidth;
            // Small per-frame moves must never be dropped, or movement would depend on frame rate.
            _characterController.minMoveDistance = 0f;
        }

        private Vector3 FlatForward()
        {
            var forward = transform.forward;
            forward.y = 0f;
            return forward.sqrMagnitude > 0f ? forward.normalized : Vector3.forward;
        }
    }
}
