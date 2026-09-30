using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Project.Tests.EditMode.Core.Input
{
    /// <summary>
    /// Validates the committed input asset and project input settings.
    /// Wrapper lifecycle (construction / Dispose) is covered by PlayMode tests because the
    /// generated Dispose uses Object.Destroy, which is only valid in Play Mode.
    /// </summary>
    public sealed class ProjectInputActionsTests
    {
        private const string AssetPath = "Assets/_Project/Settings/Input/ProjectInputActions.inputactions";
        private const string ProjectWideActionsConfigKey = "com.unity.input.settings.actions";
        private const string KeyboardMouseScheme = "Keyboard&Mouse";

        private InputActionAsset _asset;

        [SetUp]
        public void SetUp()
        {
            _asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
            Assert.That(_asset, Is.Not.Null, $"Input asset not found at '{AssetPath}'.");
        }

        [TestCase("Player")]
        [TestCase("UI")]
        public void Asset_ContainsRequiredActionMap(string mapName)
        {
            Assert.That(_asset.FindActionMap(mapName, throwIfNotFound: false), Is.Not.Null);
        }

        [Test]
        public void Asset_ContainsOnlyKeyboardMouseControlScheme()
        {
            var schemeNames = _asset.controlSchemes.Select(scheme => scheme.name).ToArray();

            Assert.That(schemeNames, Is.EqualTo(new[] { KeyboardMouseScheme }));
        }

        [Test]
        public void PlayerMove_IsVector2BoundToWasd()
        {
            var move = _asset.FindAction("Player/Move", throwIfNotFound: false);
            Assert.That(move, Is.Not.Null);
            Assert.That(move.type, Is.EqualTo(InputActionType.Value));
            Assert.That(move.expectedControlType, Is.EqualTo("Vector2"));

            var paths = move.bindings.Where(binding => binding.isPartOfComposite).Select(binding => binding.path).ToArray();
            Assert.That(paths, Is.EquivalentTo(new[] { "<Keyboard>/w", "<Keyboard>/a", "<Keyboard>/s", "<Keyboard>/d" }));
            Assert.That(move.bindings.Where(binding => binding.isPartOfComposite).All(binding => binding.groups == KeyboardMouseScheme),
                Is.True);
        }

        [Test]
        public void PlayerRun_IsButtonBoundToLeftShift()
        {
            var run = _asset.FindAction("Player/Run", throwIfNotFound: false);
            Assert.That(run, Is.Not.Null);
            Assert.That(run.type, Is.EqualTo(InputActionType.Button));
            Assert.That(run.bindings.Select(binding => binding.path).ToArray(), Is.EqualTo(new[] { "<Keyboard>/leftShift" }));
        }

        [Test]
        public void ProjectWideActions_AreNotRegistered()
        {
            var isRegistered = EditorBuildSettings.TryGetConfigObject(ProjectWideActionsConfigKey, out InputActionAsset _);

            Assert.That(isRegistered, Is.False, "Project-wide actions must not be registered; input is owned by ProjectBootstrap.");
            Assert.That(InputSystem.actions, Is.Null);
        }
    }
}
