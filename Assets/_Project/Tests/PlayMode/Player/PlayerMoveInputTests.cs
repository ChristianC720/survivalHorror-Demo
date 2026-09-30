using NUnit.Framework;
using Project.Core.Input;
using Project.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Tests.PlayMode.Player
{
    public sealed class PlayerMoveInputTests : InputTestFixture
    {
        private Keyboard _keyboard;
        private ProjectInputActions _actions;

        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
            _actions = new ProjectInputActions();
        }

        public override void TearDown()
        {
            _actions.Dispose();
            base.TearDown();
        }

        [Test]
        public void WAndLeftShift_AreReadThroughAdapter()
        {
            _actions.Player.Enable();
            var adapter = new PlayerMoveInput(_actions.Player);

            Press(_keyboard.wKey);
            Press(_keyboard.leftShiftKey);

            Assert.That(adapter.Move, Is.EqualTo(Vector2.up));
            Assert.That(adapter.IsRunHeld, Is.True);
        }

        [Test]
        public void Adapter_DoesNotEnableMap()
        {
            var adapter = new PlayerMoveInput(_actions.Player);

            Press(_keyboard.wKey);

            Assert.That(_actions.Player.enabled, Is.False);
            Assert.That(adapter.Move, Is.EqualTo(Vector2.zero));
        }
    }
}
