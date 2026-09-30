using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Project.Core.Bootstrap;
using Project.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Project.Tests.EditMode.Core.Bootstrap
{
    /// <summary>
    /// Validates the committed Bootstrap scene and build configuration (not the runtime behavior).
    /// </summary>
    public sealed class BootstrapSceneConfigurationTests
    {
        private const string NextSceneFieldName = "_nextSceneName";

        [Test]
        public void Bootstrap_IsEnabledAtBuildIndexZero()
        {
            var scenes = EditorBuildSettings.scenes;

            Assert.That(scenes, Is.Not.Empty);
            Assert.That(scenes[0].path, Is.EqualTo(PlayFromBootstrap.BootstrapScenePath));
            Assert.That(scenes[0].enabled, Is.True);
        }

        [Test]
        public void BootstrapScene_ContainsExactlyOneCompositionRoot()
        {
            WithSceneOpen(PlayFromBootstrap.BootstrapScenePath, scene =>
                Assert.That(FindRoots(scene).Count, Is.EqualTo(1)));
        }

        [Test]
        public void BootstrapScene_NextSceneIsValidAgainstBuildSettings()
        {
            var enabledScenePaths = GetEnabledBuildScenePaths();
            var bootstrapIndex = Array.IndexOf(enabledScenePaths, PlayFromBootstrap.BootstrapScenePath);

            WithSceneOpen(PlayFromBootstrap.BootstrapScenePath, scene =>
            {
                var root = FindRoots(scene).Single();
                var nextSceneName = new SerializedObject(root).FindProperty(NextSceneFieldName).stringValue;

                Assert.DoesNotThrow(() =>
                    new BootstrapSceneValidator().Validate(nextSceneName, enabledScenePaths, bootstrapIndex));
            });
        }

        [Test]
        public void NonBootstrapBuildScenes_DoNotContainCompositionRoot()
        {
            foreach (var path in GetEnabledBuildScenePaths().Where(path => path != PlayFromBootstrap.BootstrapScenePath))
            {
                WithSceneOpen(path, scene =>
                    Assert.That(FindRoots(scene), Is.Empty, $"Scene '{path}' must not contain a {nameof(ProjectBootstrap)}."));
            }
        }

        private static string[] GetEnabledBuildScenePaths()
        {
            return EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        }

        private static List<ProjectBootstrap> FindRoots(Scene scene)
        {
            return scene.GetRootGameObjects()
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<ProjectBootstrap>(true))
                .ToList();
        }

        private static void WithSceneOpen(string path, Action<Scene> assertion)
        {
            var alreadyLoaded = SceneManager.GetSceneByPath(path).isLoaded;
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            try
            {
                assertion(scene);
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
