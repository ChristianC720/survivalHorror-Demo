using System;
using UnityEngine;

namespace Project.Player
{
    /// <summary>
    /// Horizontal (XZ) basis that directional input is mapped onto. Forward and Right are unit length and flat.
    /// </summary>
    public readonly struct MovementFrame
    {
        private const float MinimumFlatLength = 1e-4f;

        public MovementFrame(Vector3 forward)
        {
            var flat = new Vector3(forward.x, 0f, forward.z);
            if (flat.sqrMagnitude < MinimumFlatLength * MinimumFlatLength)
            {
                throw new ArgumentException("Movement frame forward must have a horizontal component.", nameof(forward));
            }

            Forward = flat.normalized;
            Right = Vector3.Cross(Vector3.up, Forward);
        }

        public Vector3 Forward { get; }

        public Vector3 Right { get; }

        /// <summary>Maps a 2D input (x = right, y = forward) into world space.</summary>
        public Vector3 ToWorld(Vector2 input)
        {
            return Right * input.x + Forward * input.y;
        }
    }
}
