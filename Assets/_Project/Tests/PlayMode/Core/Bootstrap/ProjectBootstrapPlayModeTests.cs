using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Project.Core.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Project.Tests.PlayMode.Core.Bootstrap
{
    public sealed class ProjectBootstrapPlayModeTests
    {
        private const string BootstrapSceneName = "Bootstrap";
        private const string SandboxSceneName = "Sandbox";
        private const string DontDestroyOnLoadSceneName = "DontDestroyOnLoad";
        private const float TransitionTimeoutSeconds = 10f;

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var root in Object.FindObjectsByType<ProjectBootstrap>())
            {
                Object.Destroy(root.gameObject);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Bootstrap_InitializesAndTransitionsToSandbox()
        {
            yield return LoadBootstrapAndWaitForSandbox();

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(SandboxSceneName));

            var roots = Object.FindObjectsByType<ProjectBootstrap>();
            Assert.That(roots.Length, Is.EqualTo(1));
            Assert.That(roots[0].IsInitialized, Is.True);
            Assert.That(roots[0].Input, Is.Not.Null);
            Assert.That(roots[0].gameObject.scene.name, Is.EqualTo(DontDestroyOnLoadSceneName));
        }

        [UnityTest]
        public IEnumerator DuplicateRoot_AfterTransition_IsDestroyedAndOriginalSurvives()
        {
            yield return LoadBootstrapAndWaitForSandbox();
            var original = Object.FindObjectsByType<ProjectBootstrap>()[0];

            LogAssert.Expect(LogType.Error, new Regex($"Duplicate ProjectBootstrap detected in scene '{SandboxSceneName}'"));
            new GameObject("DuplicateBootstrap").AddComponent<ProjectBootstrap>();
            yield return null;

            var roots = Object.FindObjectsByType<ProjectBootstrap>();
            Assert.That(roots.Length, Is.EqualTo(1));
            Assert.That(roots[0], Is.SameAs(original));
            Assert.That(original.IsInitialized, Is.True);
            Assert.That(original.Input, Is.Not.Null);
        }

        private static IEnumerator LoadBootstrapAndWaitForSandbox()
        {
            SceneManager.LoadScene(BootstrapSceneName, LoadSceneMode.Single);

            // LoadScene completes on the next frame; a previous test may have left Sandbox active.
            yield return null;

            var deadline = Time.realtimeSinceStartup + TransitionTimeoutSeconds;
            while (!HasTransitionedToSandbox())
            {
                if (Time.realtimeSinceStartup > deadline)
                {
                    Assert.Fail($"Timed out after {TransitionTimeoutSeconds}s waiting for '{SandboxSceneName}' to become active.");
                }

                yield return null;
            }
        }

        private static bool HasTransitionedToSandbox()
        {
            if (SceneManager.GetActiveScene().name != SandboxSceneName || SceneManager.GetSceneByName(BootstrapSceneName).isLoaded)
            {
                return false;
            }

            foreach (var root in Object.FindObjectsByType<ProjectBootstrap>())
            {
                if (root.IsInitialized)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
