using NUnit.Framework;
using Project.Core.Input;

namespace Project.Tests.PlayMode.Core.Input
{
    public sealed class ProjectInputActionsPlayModeTests
    {
        [Test]
        public void Wrapper_ExposesRequiredMapsDisabledByDefault()
        {
            var inputActions = new ProjectInputActions();
            try
            {
                Assert.That(inputActions.Player.Get(), Is.Not.Null);
                Assert.That(inputActions.UI.Get(), Is.Not.Null);
                Assert.That(inputActions.Player.enabled, Is.False);
                Assert.That(inputActions.UI.enabled, Is.False);
            }
            finally
            {
                inputActions.Dispose();
            }
        }

        [Test]
        public void Wrapper_Dispose_DoesNotThrow()
        {
            var inputActions = new ProjectInputActions();

            Assert.DoesNotThrow(inputActions.Dispose);
        }
    }
}
