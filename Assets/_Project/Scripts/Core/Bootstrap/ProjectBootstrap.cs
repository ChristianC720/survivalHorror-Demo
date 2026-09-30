using System;
using Project.Core.Input;
using Project.Core.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core.Bootstrap
{
    /// <summary>
    /// Application composition root. Lives only in the Bootstrap scene, persists for the
    /// application lifetime and owns shared infrastructure (currently input only).
    /// Consumers receive dependencies explicitly from this root; there is no static access point.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ProjectBootstrap : MonoBehaviour
    {
        // TBD (Spec 10 Game Flow): replace the temporary Sandbox entry scene with the real first playable scene.
        [SerializeField] private string _nextSceneName = "Sandbox";

        private readonly BootstrapSceneValidator _sceneValidator = new BootstrapSceneValidator();
        private ProjectInputActions _input;
        private bool _isInitialized;

        /// <summary>True once this instance has validated its configuration and taken ownership of infrastructure.</summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>Input wrapper owned by this root. Action maps start disabled; consumers enable what they need.</summary>
        public ProjectInputActions Input => _input;

        private void Awake()
        {
            if (HasInitializedDuplicate())
            {
                Debug.LogError(
                    $"Duplicate {nameof(ProjectBootstrap)} detected in scene '{gameObject.scene.name}'. " +
                    $"Only the Bootstrap scene may contain a composition root. Destroying the duplicate.",
                    this);
                Destroy(gameObject);
                return;
            }

            try
            {
                _sceneValidator.Validate(_nextSceneName, GetBuildScenePaths(), gameObject.scene.buildIndex);
            }
            catch (InvalidOperationException exception)
            {
                Debug.LogException(exception, this);
                enabled = false;
                return;
            }

            DontDestroyOnLoad(gameObject);
            _input = new ProjectInputActions();
            _isInitialized = true;
        }

        private void Start()
        {
            if (!_isInitialized)
            {
                return;
            }

            // sceneLoaded fires after the loaded scene's Awake/OnEnable and before its Start,
            // so the entry point is initialized before it checks its own state.
            SceneManager.sceneLoaded += OnSceneLoaded;
            var loadOperation = SceneManager.LoadSceneAsync(_nextSceneName, LoadSceneMode.Single);
            if (loadOperation == null)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Debug.LogError($"{nameof(ProjectBootstrap)} failed to start loading scene '{_nextSceneName}'.", this);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != _nextSceneName)
            {
                return;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            InitializeEntryPoint(scene);
        }

        private void InitializeEntryPoint(Scene scene)
        {
            SceneEntryPoint entryPoint = null;
            var count = 0;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.TryGetComponent(out SceneEntryPoint candidate))
                {
                    entryPoint = candidate;
                    count++;
                }
            }

            if (count != 1)
            {
                Debug.LogError(
                    $"Scene '{scene.name}' must have exactly one root-level {nameof(SceneEntryPoint)}, found {count}.", this);
                return;
            }

            try
            {
                entryPoint.Initialize(_input);
            }
            catch (InvalidOperationException exception)
            {
                Debug.LogException(exception, entryPoint);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (_input == null)
            {
                return;
            }

            _input.Dispose();
            _input = null;
        }

        private bool HasInitializedDuplicate()
        {
            // One-time startup guard, not dependency resolution.
            var roots = FindObjectsByType<ProjectBootstrap>();
            foreach (var root in roots)
            {
                if (root != this && root.IsInitialized)
                {
                    return true;
                }
            }

            return false;
        }

        private static string[] GetBuildScenePaths()
        {
            var paths = new string[SceneManager.sceneCountInBuildSettings];
            for (var i = 0; i < paths.Length; i++)
            {
                paths[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }

            return paths;
        }
    }
}
