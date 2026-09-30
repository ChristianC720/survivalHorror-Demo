using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Editor
{
    /// <summary>
    /// Editor-only: forces Play Mode to start from the Bootstrap scene so the composition root
    /// always exists. Toggle via <c>Project/Play From Bootstrap</c> (stored per user in EditorPrefs).
    /// Unity Test Framework PlayMode runs are left untouched.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayFromBootstrap
    {
        public const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        public const string EnabledPrefKey = "Project.PlayFromBootstrap.Enabled";

        private const string MenuPath = "Project/Play From Bootstrap";

        // Test Framework saves its PlayMode init scene as "Assets/InitTestScene<guid>.unity".
        private const string TestRunnerInitScenePrefix = "Assets/InitTestScene";

        static PlayFromBootstrap()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.delayCall += Apply;
        }

        public static bool IsEnabled
        {
            get => EditorPrefs.GetBool(EnabledPrefKey, true);
            set
            {
                EditorPrefs.SetBool(EnabledPrefKey, value);
                Apply();
            }
        }

        /// <summary>Synchronizes <see cref="EditorSceneManager.playModeStartScene"/> with the current toggle state.</summary>
        public static void Apply()
        {
            EditorSceneManager.playModeStartScene = IsEnabled ? LoadBootstrapScene() : null;
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            IsEnabled = !IsEnabled;
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, IsEnabled);
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    if (IsTestRunnerScene(SceneManager.GetActiveScene()))
                    {
                        EditorSceneManager.playModeStartScene = null;
                    }
                    else
                    {
                        Apply();
                    }

                    break;
                case PlayModeStateChange.EnteredEditMode:
                    Apply();
                    break;
            }
        }

        private static bool IsTestRunnerScene(Scene scene)
        {
            return scene.IsValid() && scene.path.StartsWith(TestRunnerInitScenePrefix, System.StringComparison.Ordinal);
        }

        private static SceneAsset LoadBootstrapScene()
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);
            if (scene == null)
            {
                Debug.LogError($"{nameof(PlayFromBootstrap)}: Bootstrap scene not found at '{BootstrapScenePath}'. " +
                               $"Disable '{MenuPath}' or restore the scene.");
            }

            return scene;
        }
    }
}
