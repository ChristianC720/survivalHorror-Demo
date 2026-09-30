using NUnit.Framework;
using Project.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Project.Tests.EditMode.Editor
{
    public sealed class PlayFromBootstrapTests
    {
        private bool _hadPref;
        private bool _originalPrefValue;
        private SceneAsset _originalStartScene;

        [SetUp]
        public void SetUp()
        {
            _hadPref = EditorPrefs.HasKey(PlayFromBootstrap.EnabledPrefKey);
            _originalPrefValue = EditorPrefs.GetBool(PlayFromBootstrap.EnabledPrefKey, true);
            _originalStartScene = EditorSceneManager.playModeStartScene;
        }

        [TearDown]
        public void TearDown()
        {
            if (_hadPref)
            {
                EditorPrefs.SetBool(PlayFromBootstrap.EnabledPrefKey, _originalPrefValue);
            }
            else
            {
                EditorPrefs.DeleteKey(PlayFromBootstrap.EnabledPrefKey);
            }

            EditorSceneManager.playModeStartScene = _originalStartScene;
        }

        [Test]
        public void Enabled_SetsPlayModeStartSceneToBootstrap()
        {
            PlayFromBootstrap.IsEnabled = true;

            var bootstrap = AssetDatabase.LoadAssetAtPath<SceneAsset>(PlayFromBootstrap.BootstrapScenePath);
            Assert.That(bootstrap, Is.Not.Null);
            Assert.That(EditorSceneManager.playModeStartScene, Is.EqualTo(bootstrap));
        }

        [Test]
        public void Disabled_ClearsPlayModeStartScene()
        {
            PlayFromBootstrap.IsEnabled = false;

            Assert.That(EditorSceneManager.playModeStartScene, Is.Null);
        }

        [Test]
        public void DefaultsToEnabled_WhenNoPreferenceStored()
        {
            EditorPrefs.DeleteKey(PlayFromBootstrap.EnabledPrefKey);

            Assert.That(PlayFromBootstrap.IsEnabled, Is.True);
        }
    }
}
