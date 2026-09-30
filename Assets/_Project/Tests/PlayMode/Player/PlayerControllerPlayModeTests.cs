#if UNITY_EDITOR
using System;
using System.Collections;
using NUnit.Framework;
using Project.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Project.Tests.PlayMode.Player
{
    /// <summary>Controller integration with the real Player prefab and a fake input source.</summary>
    public sealed class PlayerControllerPlayModeTests
    {
        private const string PrefabPath = "Assets/_Project/Prefabs/Player/Player.prefab";
        private const float SteadySpeedTolerance = 0.005f;

        private GameObject _floor;
        private GameObject _player;

        [SetUp]
        public void SetUp()
        {
            _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _floor.transform.position = new Vector3(0f, -0.5f, 0f);
            _floor.transform.localScale = new Vector3(20f, 1f, 20f);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null, $"Missing '{PrefabPath}'.");
            _player = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_player);
            Object.Destroy(_floor);
        }

        [UnityTest]
        public IEnumerator Uninitialized_StaysDisabledAndDoesNotMove()
        {
            var controller = _player.GetComponent<PlayerController>();
            var start = _player.transform.position;

            for (var i = 0; i < 10; i++)
            {
                yield return null;
            }

            Assert.That(controller.enabled, Is.False);
            Assert.That(controller.IsInitialized, Is.False);
            Assert.That(_player.transform.position, Is.EqualTo(start));
        }

        [Test]
        public void Initialize_NullDependencies_Throw()
        {
            var controller = _player.GetComponent<PlayerController>();
            var frame = new FixedFrame(Vector3.forward);

            Assert.Throws<ArgumentNullException>(() => controller.Initialize(null, frame));
            Assert.Throws<ArgumentNullException>(() => controller.Initialize(new FakeInput(), null));
            Assert.That(controller.enabled, Is.False);
        }

        [Test]
        public void Prefab_AppliesCapsuleSettings()
        {
            var characterController = _player.GetComponent<CharacterController>();

            Assert.That(characterController.height, Is.EqualTo(1.8f));
            Assert.That(characterController.radius, Is.EqualTo(0.35f));
            Assert.That(characterController.center, Is.EqualTo(new Vector3(0f, 0.9f, 0f)));
            Assert.That(characterController.stepOffset, Is.EqualTo(0.2f));
            Assert.That(characterController.skinWidth, Is.EqualTo(0.035f));
        }

        [UnityTest]
        public IEnumerator Initialized_WithForwardInput_WalksGroundedAtWalkSpeed()
        {
            var controller = _player.GetComponent<PlayerController>();
            var input = new FakeInput();
            controller.Initialize(input, new FixedFrame(Vector3.forward));

            yield return WaitSeconds(0.2f); // settle onto the floor
            input.Move = Vector2.up;
            yield return WaitSeconds(1f);

            Assert.That(controller.IsGrounded, Is.True);
            Assert.That(controller.IsMoving, Is.True);
            Assert.That(controller.Speed, Is.EqualTo(2.5f).Within(2.5f * SteadySpeedTolerance));
            Assert.That(Vector3.Angle(controller.Facing, Vector3.forward), Is.LessThanOrEqualTo(1f));
            Assert.That(_player.transform.position.z, Is.GreaterThan(2f));
        }

        private static IEnumerator WaitSeconds(float seconds)
        {
            var elapsed = 0f;
            while (elapsed < seconds)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }
        }

        private sealed class FakeInput : IPlayerMoveInput
        {
            public Vector2 Move { get; set; }

            public bool IsRunHeld { get; set; }
        }

        private sealed class FixedFrame : IMovementReferenceFrame
        {
            public FixedFrame(Vector3 forward)
            {
                Current = new MovementFrame(forward);
            }

            public MovementFrame Current { get; }
        }
    }
}
#endif
