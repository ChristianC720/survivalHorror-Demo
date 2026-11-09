using UnityEditor;
using UnityEditor.SceneManagement;

namespace Heartbeat.Sandbox
{
    /// <summary>
    /// Makes Play open Sandbox_Gallery whatever scene is open, on this machine only (EditorPrefs), and lets the gallery hand over to
    /// the real game: Play again through Bootstrap and the main menu, then back to the gallery when that Play stops.
    /// Editor-only, like the rest of this assembly: a build never starts here.
    /// </summary>
    [InitializeOnLoad]
    public static class GalleryStart
    {
        private const string MenuPath = "Heartbeat/Sandbox/Play Starts in Gallery";
        private const string Enabled = "Heartbeat.Sandbox.PlayStartsInGallery";
        // SessionState survives the domain reload of entering and leaving Play.
        private const string Pending = "Heartbeat.Sandbox.GamePending", Running = "Heartbeat.Sandbox.GameRunning";
        private const string BootstrapScene = "Assets/_Project/Scenes/Bootstrap.unity";

        static GalleryStart()
        {
            EditorApplication.playModeStateChanged += OnPlayMode;
            EditorApplication.delayCall += Apply;
        }

        private static void Apply()
        {
            string scene = SessionState.GetBool(Running, false) ? BootstrapScene : EditorPrefs.GetBool(Enabled, false) ? GalleryTools.ScenePath : null;
            EditorSceneManager.playModeStartScene = scene == null ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(scene);
        }

        /// <summary>From the gallery's panel: plays one cinematic through the real game (CinematicPreview), then comes back.</summary>
        public static void PlayCinematic(string id)
        {
            SessionState.SetString("Heartbeat.Sandbox.Cinematic", id);
            PlayGame();
        }

        /// <summary>From the gallery's panel: stops the gallery and plays the game from its real start.</summary>
        public static void PlayGame()
        {
            SessionState.SetBool(Pending, true);
            EditorApplication.isPlaying = false;
        }

        private static void OnPlayMode(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredEditMode) return;
            if (SessionState.GetBool(Pending, false))
            {
                SessionState.SetBool(Pending, false);
                SessionState.SetBool(Running, true);
                Apply();
                EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
                return;
            }
            SessionState.SetBool(Running, false);
            Apply();
            if (SessionState.GetBool("Heartbeat.Sandbox.ReturnToGallery", false))
            {
                SessionState.SetBool("Heartbeat.Sandbox.ReturnToGallery", false);
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GalleryTools.ScenePath);
                EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
            }
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            EditorPrefs.SetBool(Enabled, !EditorPrefs.GetBool(Enabled, false));
            Apply();
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleCheck()
        {
            Menu.SetChecked(MenuPath, EditorPrefs.GetBool(Enabled, false));
            return true;
        }
    }
}
