using System;
using UnityEngine;

namespace Project.Player
{
    /// <summary>
    /// Pure locomotion model: converts intent, a movement frame and a time step into displacement,
    /// velocity and facing. Has no dependency on input devices or Unity physics components.
    /// </summary>
    /// <remarks>
    /// Displacement uses the average velocity over each step, split at the instant speed reaches its
    /// target. This is exact under constant acceleration, so results do not depend on frame rate.
    /// </remarks>
    public sealed class PlayerMotor
    {
        private readonly MovementSettings _settings;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        private float _facingYaw;

        public PlayerMotor(MovementSettings settings) : this(settings, Vector3.forward)
        {
        }

        public PlayerMotor(MovementSettings settings, Vector3 initialFacing)
        {
            _settings = settings != null ? settings : throw new ArgumentNullException(nameof(settings));
            var flat = new Vector3(initialFacing.x, 0f, initialFacing.z);
            _facingYaw = flat.sqrMagnitude > 0f ? YawOf(flat) : 0f;
        }

        public Vector3 HorizontalVelocity => _horizontalVelocity;

        public float VerticalVelocity => _verticalVelocity;

        public Vector3 Facing => DirectionOf(_facingYaw);

        /// <summary>Advances the motor by <paramref name="deltaTime"/> seconds.</summary>
        /// <param name="facingTarget">
        /// Optional facing override (e.g. future camera/combat). When null the motor faces its movement direction.
        /// </param>
        public MotorResult Step(MotorInput input, MovementFrame frame, bool isGrounded, float deltaTime,
            Vector3? facingTarget = null)
        {
            if (deltaTime < 0f || float.IsNaN(deltaTime))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime), deltaTime, "Delta time must be >= 0.");
            }

            var move = FilterInput(input.Move);
            var desiredDirection = frame.ToWorld(move);
            var targetSpeed = input.Run ? _settings.RunSpeed : _settings.WalkSpeed;
            var desiredVelocity = desiredDirection * targetSpeed;

            var horizontalDisplacement = StepHorizontal(desiredVelocity, deltaTime);
            var verticalDisplacement = StepVertical(isGrounded, deltaTime);
            StepFacing(facingTarget ?? desiredDirection, deltaTime);

            return new MotorResult(
                horizontalDisplacement + Vector3.up * verticalDisplacement,
                _horizontalVelocity + Vector3.up * _verticalVelocity,
                Facing);
        }

        private Vector2 FilterInput(Vector2 move)
        {
            var magnitude = move.magnitude;
            if (magnitude < _settings.InputDeadzone || magnitude <= 0f)
            {
                return Vector2.zero;
            }

            return magnitude > 1f ? move / magnitude : move;
        }

        private Vector3 StepHorizontal(Vector3 desiredVelocity, float deltaTime)
        {
            var start = _horizontalVelocity;
            var delta = desiredVelocity - start;
            var distanceToTarget = delta.magnitude;
            if (distanceToTarget <= 0f)
            {
                return start * deltaTime;
            }

            var rate = desiredVelocity.sqrMagnitude > start.sqrMagnitude
                ? _settings.Acceleration
                : _settings.Deceleration;
            var timeToTarget = distanceToTarget / rate;

            if (timeToTarget >= deltaTime)
            {
                _horizontalVelocity = start + delta * (rate * deltaTime / distanceToTarget);
                return (start + _horizontalVelocity) * (0.5f * deltaTime);
            }

            _horizontalVelocity = desiredVelocity;
            return (start + desiredVelocity) * (0.5f * timeToTarget) + desiredVelocity * (deltaTime - timeToTarget);
        }

        private float StepVertical(bool isGrounded, float deltaTime)
        {
            if (isGrounded)
            {
                _verticalVelocity = _settings.GroundedVelocity;
                return _verticalVelocity * deltaTime;
            }

            var start = _verticalVelocity;
            _verticalVelocity = start + _settings.Gravity * deltaTime;
            return (start + _verticalVelocity) * (0.5f * deltaTime);
        }

        private void StepFacing(Vector3 target, float deltaTime)
        {
            var flat = new Vector3(target.x, 0f, target.z);
            if (flat.sqrMagnitude <= 1e-8f)
            {
                return;
            }

            _facingYaw = Mathf.MoveTowardsAngle(_facingYaw, YawOf(flat), _settings.TurnSpeed * deltaTime);
        }

        private static float YawOf(Vector3 flatDirection)
        {
            return Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg;
        }

        private static Vector3 DirectionOf(float yaw)
        {
            var radians = yaw * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radians), 0f, Mathf.Cos(radians));
        }
    }
}
