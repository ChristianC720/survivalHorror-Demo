using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartbeat.Sandbox
{
    /// <summary>Creates Sandbox_Gallery without touching the open scene, and stops a build that would ship any sandbox or test scene.</summary>
    public sealed class GalleryTools : IPreprocessBuildWithReport
    {
        public const string ScenePath = "Assets/_Project/Scenes/Tests/Sandbox_Gallery.unity";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var leaked = EditorBuildSettings.scenes.Where(s => s.enabled && (s.path.Contains("/Sandbox_") || s.path.Contains("/Scenes/Tests/"))).Select(s => s.path).ToArray();
            if (leaked.Length > 0) throw new BuildFailedException("[Heartbeat][Sandbox] Sandbox scenes must never ship: " + string.Join(", ", leaked));
        }

        [MenuItem("Heartbeat/Sandbox/Create Gallery Scene")]
        public static void CreateScene()
        {
            if (System.IO.File.Exists(ScenePath)) { Debug.Log("[Heartbeat][Sandbox] Gallery scene already exists"); return; }
            var active = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            var go = new GameObject("GalleryRig", typeof(Camera), typeof(AudioListener), typeof(GalleryRig));
            go.tag = "MainCamera";
            go.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            go.GetComponent<Camera>().backgroundColor = new Color(.02f, .02f, .025f);
            go.GetComponent<Camera>().farClipPlane = 2000f;
            EditorSceneManager.SaveScene(scene, ScenePath);
            if (active.IsValid()) SceneManager.SetActiveScene(active);
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}
