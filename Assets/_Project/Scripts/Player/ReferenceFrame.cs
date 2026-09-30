using System;
using UnityEngine;

namespace Project.Player
{
    /// <summary>Supplies the frame that directional input is interpreted in (typically the active camera).</summary>
    public interface IMovementReferenceFrame
    {
        MovementFrame Current { get; }
    }

    /// <summary>
    /// Reference frame from a Transform's flattened forward. Falls back to its flattened up
    /// when the transform looks almost straight down.
    /// </summary>
    public sealed class TransformReferenceFrame : IMovementReferenceFrame
    {
        private const float MinimumFlatLength = 0.01f;

        private readonly Transform _transform;

        public TransformReferenceFrame(Transform transform)
        {
            _transform = transform != null ? transform : throw new ArgumentNullException(nameof(transform));
        }

        public MovementFrame Current
        {
            get
            {
                if (_transform == null)
                {
                    throw new InvalidOperationException("Movement reference transform was destroyed.");
                }

                var forward = Flatten(_transform.forward);
                if (forward.magnitude < MinimumFlatLength)
                {
                    forward = Flatten(_transform.up);
                }

                if (forward.magnitude < MinimumFlatLength)
                {
                    throw new InvalidOperationException(
                        $"Transform '{_transform.name}' does not define a usable horizontal movement frame.");
                }

                return new MovementFrame(forward);
            }
        }

        private static Vector3 Flatten(Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }
    }

    /// <summary>
    /// Keeps the movement frame captured when movement starts while input intent stays the same,
    /// so camera perspective changes do not silently redefine a held direction.
    /// </summary>
    public sealed class ReferenceFrameLock
    {
        private readonly float _deadzone;
        private readonly float _directionChangeThreshold;
        private bool _hasCapture;
        private MovementFrame _capturedFrame;
        private Vector2 _capturedDirection;

        public ReferenceFrameLock(float deadzone, float directionChangeThresholdDegrees)
        {
            if (deadzone < 0f || deadzone >= 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(deadzone), deadzone, "Deadzone must be in [0, 1).");
            }

            if (directionChangeThresholdDegrees <= 0f || directionChangeThresholdDegrees >= 180f)
            {
                throw new ArgumentOutOfRangeException(nameof(directionChangeThresholdDegrees),
                    directionChangeThresholdDegrees, "Threshold must be in (0, 180).");
            }

            _deadzone = deadzone;
            _directionChangeThreshold = directionChangeThresholdDegrees;
        }

        public bool HasCapture => _hasCapture;

        public MovementFrame Resolve(Vector2 input, MovementFrame current)
        {
            var magnitude = input.magnitude;
            if (magnitude < _deadzone || magnitude <= 0f)
            {
                _hasCapture = false;
                return current;
            }

            var direction = input / magnitude;
            if (!_hasCapture || Vector2.Angle(_capturedDirection, direction) > _directionChangeThreshold)
            {
                _hasCapture = true;
                _capturedFrame = current;
                _capturedDirection = direction;
            }

            return _capturedFrame;
        }
    }
}
