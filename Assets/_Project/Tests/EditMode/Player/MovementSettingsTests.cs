using System;
using NUnit.Framework;
using Project.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Project.Tests.EditMode.Player
{
    public sealed class MovementSettingsTests
    {
        private const string DefaultAssetPath = "Assets/_Project/ScriptableObjects/Player/DefaultMovementSettings.asset";

        private MovementSettings _settings;

        [SetUp]
        public void SetUp()
        {
            _settings = ScriptableObject.CreateInstance<MovementSettings>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_settings);
        }

        [Test]
        public void DefaultAsset_ExistsAndIsValid()
        {
            var asset = AssetDatabase.LoadAssetAtPath<MovementSettings>(DefaultAssetPath);

            Assert.That(asset, Is.Not.Null, $"Missing '{DefaultAssetPath}'.");
            Assert.DoesNotThrow(asset.Validate);
        }

        [Test]
        public void DefaultAsset_HasProvisionalValues()
        {
            var asset = AssetDatabase.LoadAssetAtPath<MovementSettings>(DefaultAssetPath);

            Assert.That(asset.WalkSpeed, Is.EqualTo(2.5f));
            Assert.That(asset.RunSpeed, Is.EqualTo(4.5f));
            Assert.That(asset.Acceleration, Is.EqualTo(18f));
            Assert.That(asset.Deceleration, Is.EqualTo(24f));
            Assert.That(asset.TurnSpeed, Is.EqualTo(540f));
            Assert.That(asset.Gravity, Is.EqualTo(-20f));
            Assert.That(asset.GroundedVelocity, Is.EqualTo(-2f));
            Assert.That(asset.InputDeadzone, Is.EqualTo(0.1f));
            Assert.That(asset.DirectionChangeThreshold, Is.EqualTo(30f));
            Assert.That(asset.CapsuleHeight, Is.EqualTo(1.8f));
            Assert.That(asset.CapsuleRadius, Is.EqualTo(0.35f));
            Assert.That(asset.CapsuleCenter, Is.EqualTo(new Vector3(0f, 0.9f, 0f)));
            Assert.That(asset.StepOffset, Is.EqualTo(0.2f));
            Assert.That(asset.SkinWidth, Is.EqualTo(0.035f));
        }

        [Test]
        public void NewInstance_DefaultsAreValid()
        {
            Assert.DoesNotThrow(_settings.Validate);
        }

        [TestCase("_walkSpeed", 0f)]
        [TestCase("_walkSpeed", -1f)]
        [TestCase("_runSpeed", 0f)]
        [TestCase("_runSpeed", 2f)]
        [TestCase("_acceleration", 0f)]
        [TestCase("_deceleration", -1f)]
        [TestCase("_turnSpeed", 0f)]
        [TestCase("_gravity", 0f)]
        [TestCase("_gravity", 5f)]
        [TestCase("_groundedVelocity", 0.5f)]
        [TestCase("_inputDeadzone", -0.1f)]
        [TestCase("_inputDeadzone", 1f)]
        [TestCase("_directionChangeThreshold", 0f)]
        [TestCase("_directionChangeThreshold", 180f)]
        [TestCase("_capsuleRadius", 0f)]
        [TestCase("_capsuleHeight", 0.5f)]
        [TestCase("_stepOffset", 2f)]
        [TestCase("_skinWidth", 0f)]
        public void Validate_InvalidValue_ThrowsNamingField(string fieldName, float value)
        {
            // OnValidate logs the same problem in the editor when the value is applied.
            LogAssert.ignoreFailingMessages = true;
            SetFloat(fieldName, value);
            LogAssert.ignoreFailingMessages = false;

            var exception = Assert.Throws<InvalidOperationException>(_settings.Validate);
            StringAssert.Contains(fieldName, exception.Message);
        }

        private void SetFloat(string fieldName, float value)
        {
            var serialized = new SerializedObject(_settings);
            var property = serialized.FindProperty(fieldName);
            Assert.That(property, Is.Not.Null, $"Field '{fieldName}' not found.");
            property.floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
