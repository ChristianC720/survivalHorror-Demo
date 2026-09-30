using System;
using UnityEngine;

namespace Project.Player
{
    /// <summary>
    /// Player locomotion tuning. All defaults are provisional tuning values, not GDD canon.
    /// </summary>
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Project/Player/Movement Settings")]
    public sealed class MovementSettings : ScriptableObject
    {
        [Header("Speed")]
        [SerializeField, Tooltip("m/s")] private float _walkSpeed = 2.5f;
        [SerializeField, Tooltip("m/s, used while Run is held")] private float _runSpeed = 4.5f;
        [SerializeField, Tooltip("m/s²")] private float _acceleration = 18f;
        [SerializeField, Tooltip("m/s²")] private float _deceleration = 24f;

        [Header("Facing")]
        [SerializeField, Tooltip("deg/s")] private float _turnSpeed = 540f;

        [Header("Vertical")]
        [SerializeField, Tooltip("m/s², must be negative")] private float _gravity = -20f;
        [SerializeField, Tooltip("m/s applied while grounded, must be <= 0")] private float _groundedVelocity = -2f;

        [Header("Input")]
        [SerializeField, Tooltip("Input magnitude below this is neutral")] private float _inputDeadzone = 0.1f;
        [SerializeField, Tooltip("Input direction change (deg) that resamples the movement reference frame")]
        private float _directionChangeThreshold = 30f;

        [Header("Character Controller")]
        [SerializeField] private float _capsuleHeight = 1.8f;
        [SerializeField] private float _capsuleRadius = 0.35f;
        [SerializeField] private Vector3 _capsuleCenter = new Vector3(0f, 0.9f, 0f);
        [SerializeField] private float _stepOffset = 0.2f;
        [SerializeField] private float _skinWidth = 0.035f;

        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public float TurnSpeed => _turnSpeed;
        public float Gravity => _gravity;
        public float GroundedVelocity => _groundedVelocity;
        public float InputDeadzone => _inputDeadzone;
        public float DirectionChangeThreshold => _directionChangeThreshold;
        public float CapsuleHeight => _capsuleHeight;
        public float CapsuleRadius => _capsuleRadius;
        public Vector3 CapsuleCenter => _capsuleCenter;
        public float StepOffset => _stepOffset;
        public float SkinWidth => _skinWidth;

        /// <summary>Throws <see cref="InvalidOperationException"/> naming the first invalid field.</summary>
        public void Validate()
        {
            var error = GetValidationError();
            if (error != null)
            {
                throw new InvalidOperationException($"{nameof(MovementSettings)} '{name}': {error}");
            }
        }

        private string GetValidationError()
        {
            if (!(_walkSpeed > 0f)) return $"{nameof(_walkSpeed)} must be > 0.";
            if (!(_runSpeed > 0f)) return $"{nameof(_runSpeed)} must be > 0.";
            if (_runSpeed < _walkSpeed) return $"{nameof(_runSpeed)} must be >= {nameof(_walkSpeed)}.";
            if (!(_acceleration > 0f)) return $"{nameof(_acceleration)} must be > 0.";
            if (!(_deceleration > 0f)) return $"{nameof(_deceleration)} must be > 0.";
            if (!(_turnSpeed > 0f)) return $"{nameof(_turnSpeed)} must be > 0.";
            if (!(_gravity < 0f)) return $"{nameof(_gravity)} must be < 0.";
            if (!(_groundedVelocity <= 0f)) return $"{nameof(_groundedVelocity)} must be <= 0.";
            if (!(_inputDeadzone >= 0f && _inputDeadzone < 1f)) return $"{nameof(_inputDeadzone)} must be in [0, 1).";
            if (!(_directionChangeThreshold > 0f && _directionChangeThreshold < 180f))
                return $"{nameof(_directionChangeThreshold)} must be in (0, 180).";
            if (!(_capsuleRadius > 0f)) return $"{nameof(_capsuleRadius)} must be > 0.";
            if (_capsuleHeight < 2f * _capsuleRadius) return $"{nameof(_capsuleHeight)} must be >= 2 * {nameof(_capsuleRadius)}.";
            if (!(_stepOffset >= 0f)) return $"{nameof(_stepOffset)} must be >= 0.";
            if (_stepOffset > _capsuleHeight) return $"{nameof(_stepOffset)} must be <= {nameof(_capsuleHeight)}.";
            if (!(_skinWidth > 0f)) return $"{nameof(_skinWidth)} must be > 0.";
            return null;
        }

        private void OnValidate()
        {
            var error = GetValidationError();
            if (error != null)
            {
                Debug.LogError($"{nameof(MovementSettings)} '{name}': {error}", this);
            }
        }
    }
}
