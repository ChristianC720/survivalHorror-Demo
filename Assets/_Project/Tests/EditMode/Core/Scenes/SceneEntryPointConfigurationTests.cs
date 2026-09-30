using System.Linq;
using NUnit.Framework;
using Project.Core.Scenes;
using Project.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Project.Tests.EditMode.Core.Scenes
{
    public sealed class SceneEntryPointConfigurationTests
    {
        [Test]
        public void GameplayBuildScenes_HaveExactlyOneConfiguredRootEntryPoint()
        {
            var gameplayScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled && scene.path != PlayFromBootstrap.BootstrapScenePath)
                .Select(scene => scene.path)
                .ToArray();
            Assert.That(gameplayScenes, Is.Not.Empty);

            foreach (var path in gameplayScenes)
            {
                var alreadyLoaded = SceneManager.GetSceneByPath(path).isLoaded;
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try
                {
                    var entryPoints = scene.GetRootGameObjects()
                        .Select(root => root.GetComponent<SceneEntryPoint>())
                        .Where(component => component != null)
                        .ToArray();
                    Assert.That(entryPoints.Length, Is.EqualTo(1), $"Scene '{path}' must have one root {nameof(SceneEntryPoint)}.");

                    var nested = scene.GetRootGameObjects()
                        .SelectMany(root => root.GetComponentsInChildren<SceneEntryPoint>(true))
                        .Count();
                    Assert.That(nested, Is.EqualTo(1), $"Scene '{path}' must not contain nested entry points.");

                    var serialized = new SerializedObject(entryPoints[0]);
                    Assert.That(serialized.FindProperty("_player").objectReferenceValue, Is.Not.Null, $"'{path}': player not set.");
                    Assert.That(serialized.FindProperty("_movementReference").objectReferenceValue, Is.Not.Null,
                        $"'{path}': movement reference not set.");
                }
                finally
                {
                    if (!alreadyLoaded)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
            }
        }
    }
}
