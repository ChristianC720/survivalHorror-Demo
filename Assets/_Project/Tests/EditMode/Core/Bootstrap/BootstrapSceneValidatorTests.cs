using System;
using NUnit.Framework;
using Project.Core.Bootstrap;

namespace Project.Tests.EditMode.Core.Bootstrap
{
    public sealed class BootstrapSceneValidatorTests
    {
        private const string BootstrapPath = "Assets/_Project/Scenes/Bootstrap.unity";
        private const string SandboxPath = "Assets/_Project/Scenes/Sandbox.unity";

        private BootstrapSceneValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new BootstrapSceneValidator();
        }

        [Test]
        public void Validate_ValidConfiguration_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                _validator.Validate("Sandbox", new[] { BootstrapPath, SandboxPath }, 0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Validate_MissingNextSceneName_Throws(string nextSceneName)
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _validator.Validate(nextSceneName, new[] { BootstrapPath, SandboxPath }, 0));

            StringAssert.Contains("next scene name is not configured", exception.Message);
        }

        [Test]
        public void Validate_NextSceneNotInBuild_Throws()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _validator.Validate("Missing", new[] { BootstrapPath, SandboxPath }, 0));

            StringAssert.Contains("'Missing'", exception.Message);
            StringAssert.Contains("not in the build scene list", exception.Message);
        }

        [Test]
        public void Validate_SceneNameMatchIsCaseSensitive()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _validator.Validate("sandbox", new[] { BootstrapPath, SandboxPath }, 0));
        }

        [TestCase(1)]
        [TestCase(-1)]
        public void Validate_BootstrapNotAtBuildIndexZero_Throws(int bootstrapBuildIndex)
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _validator.Validate("Sandbox", new[] { SandboxPath, BootstrapPath }, bootstrapBuildIndex));

            StringAssert.Contains("build index 0", exception.Message);
        }

        [Test]
        public void Validate_NextSceneIsBootstrapItself_Throws()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _validator.Validate("Bootstrap", new[] { BootstrapPath, SandboxPath }, 0));

            StringAssert.Contains("must not be the bootstrap scene", exception.Message);
        }

        [Test]
        public void Validate_NullBuildScenePaths_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate("Sandbox", null, 0));
        }
    }
}
