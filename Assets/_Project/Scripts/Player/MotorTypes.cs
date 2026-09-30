using UnityEngine;

namespace Project.Player
{
    /// <summary>Locomotion intent for a single motor step. Independent of any input technology.</summary>
    public readonly struct MotorInput
    {
        public MotorInput(Vector2 move, bool run)
        {
            Move = move;
            Run = run;
        }

        /// <summary>x = right, y = forward, relative to the movement frame.</summary>
        public Vector2 Move { get; }

        public bool Run { get; }
    }

    /// <summary>Outcome of a single motor step.</summary>
    public readonly struct MotorResult
    {
        public MotorResult(Vector3 displacement, Vector3 velocity, Vector3 facing)
        {
            Displacement = displacement;
            Velocity = velocity;
            Facing = facing;
        }

        /// <summary>World-space displacement to apply for this step.</summary>
        public Vector3 Displacement { get; }

        /// <summary>Intended velocity at the end of the step (horizontal and vertical).</summary>
        public Vector3 Velocity { get; }

        /// <summary>Flat, unit-length facing direction at the end of the step.</summary>
        public Vector3 Facing { get; }
    }
}
