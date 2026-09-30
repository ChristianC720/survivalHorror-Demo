using System;
using Project.Core.Input;
using Project.Player;
using UnityEngine;

namespace Project.Core.Scenes
{
    /// <summary>
    /// Single root-level entry point of a gameplay scene. Receives shared infrastructure from
    /// <c>ProjectBootstrap</c>, wires scene objects and owns the <c>Player</c> action-map lifecycle for the scene.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneEntryPoint : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;

        // TBD (Spec 04 Camera): replaced by the camera system's reference frame.
        [SerializeField] private Transform _movementReference;

        private ProjectInputActions _input;

        public bool IsInitialized => _input != null;

        public PlayerController Player => _player;

        public void Initialize(ProjectInputActions input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(SceneEntryPoint)} in '{gameObject.scene.name}' is already initialized.");
            }

            if (_player == null)
            {
                throw new InvalidOperationException($"{nameof(SceneEntryPoint)} in '{gameObject.scene.name}' has no player assigned.");
            }

            if (_movementReference == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(SceneEntryPoint)} in '{gameObject.scene.name}' has no movement reference assigned.");
            }

            _player.Initialize(new PlayerMoveInput(input.Player), new TransformReferenceFrame(_movementReference));
            _input = input;
            _input.Player.Enable();
        }

        private void Start()
        {
            if (!IsInitialized)
            {
                Debug.LogError(
                    $"{nameof(SceneEntryPoint)} in '{gameObject.scene.name}' was not initialized by ProjectBootstrap. " +
                    "Play from the Bootstrap scene (menu: Project/Play From Bootstrap). The player stays disabled.",
                    this);
            }
        }

        private void OnDestroy()
        {
            // The owner may already have disposed the input (its asset is destroyed with it).
            if (_input != null && _input.asset != null)
            {
                _input.Player.Disable();
            }

            _input = null;
        }
    }
}
