using Project.Core.Input;
using UnityEngine;

namespace Project.Player
{
    /// <summary>Movement intent source consumed by <see cref="PlayerController"/>.</summary>
    public interface IPlayerMoveInput
    {
        /// <summary>x = right, y = forward.</summary>
        Vector2 Move { get; }

        bool IsRunHeld { get; }
    }

    /// <summary>
    /// Read-only adapter over the Input System <c>Player</c> map. Never enables or disables actions;
    /// the scene entry point owns the map lifecycle.
    /// </summary>
    public sealed class PlayerMoveInput : IPlayerMoveInput
    {
        private readonly ProjectInputActions.PlayerActions _actions;

        public PlayerMoveInput(ProjectInputActions.PlayerActions actions)
        {
            _actions = actions;
        }

        public Vector2 Move => _actions.Move.ReadValue<Vector2>();

        public bool IsRunHeld => _actions.Run.IsPressed();
    }
}
