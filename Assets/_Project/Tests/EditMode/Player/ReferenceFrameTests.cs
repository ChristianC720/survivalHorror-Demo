using System;
using NUnit.Framework;
using Project.Player;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Tests.EditMode.Player
{
    public sealed class ReferenceFrameTests
    {
        private const float Deadzone = 0.1f;
        private const float Threshold = 30f;

        private static readonly MovementFrame North = new MovementFrame(Vector3.forward);
        private static readonly MovementFrame East = new MovementFrame(Vector3.right);

        private GameObject _cameraObject;

        [TearDown]
        public void TearDown()
        {
            if (_cameraObject != null)
            {
                Object.DestroyImmediate(_cameraObject);
            }
        }

        [Test]
        public void Lock_HeldInput_KeepsFrameAcrossCameraTurn()
        {
            var frameLock = new ReferenceFrameLock(Deadzone, Threshold);

            frameLock.Resolve(Vector2.up, North);
            var resolved = frameLock.Resolve(Vector2.up, East);

            AssertFrame(resolved, North);
        }

        [Test]
        public void Lock_NeutralInput_ClearsCaptureAndResamples()
        {
            var frameLock = new ReferenceFrameLock(Deadzone, Threshold);
            frameLock.Resolve(Vector2.up, North);

            frameLock.Resolve(new Vector2(0f, Deadzone * 0.5f), East);
            Assert.That(frameLock.HasCapture, Is.False);

            AssertFrame(frameLock.Resolve(Vector2.up, East), East);
        }

        [Test]
        public void Lock_DirectionChangeAboveThreshold_Resamples()
        {
            var frameLock = new ReferenceFrameLock(Deadzone, Threshold);
            frameLock.Resolve(Vector2.up, North);

            AssertFrame(frameLock.Resolve(Rotate(Vector2.up, 31f), East), East);
        }

        [Test]
        public void Lock_DirectionChangeBelowThreshold_KeepsFrame()
        {
            var frameLock = new ReferenceFrameLock(Deadzone, Threshold);
            frameLock.Resolve(Vector2.up, North);

            AssertFrame(frameLock.Resolve(Rotate(Vector2.up, 29f), East), North);
        }

        [Test]
        public void Lock_InvalidConfiguration_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceFrameLock(1f, Threshold));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceFrameLock(Deadzone, 0f));
        }

        [Test]
        public void TransformFrame_AxisAlignedCamera_ProducesExpectedAxes()
        {
            var frame = CreateCameraFrame(Quaternion.Euler(30f, 90f, 0f));

            AssertVector(frame.Current.Forward, Vector3.right);
            AssertVector(frame.Current.Right, Vector3.back);
        }

        [Test]
        public void TransformFrame_TopDownCamera_UsesUpVector()
        {
            var frame = CreateCameraFrame(Quaternion.Euler(90f, 0f, 0f));

            AssertVector(frame.Current.Forward, Vector3.forward);
            AssertVector(frame.Current.Right, Vector3.right);
        }

        [Test]
        public void TransformFrame_NullTransform_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TransformReferenceFrame(null));
        }

        [Test]
        public void MovementFrame_VerticalForward_Throws()
        {
            Assert.Throws<ArgumentException>(() => new MovementFrame(Vector3.up));
        }

        private TransformReferenceFrame CreateCameraFrame(Quaternion rotation)
        {
            _cameraObject = new GameObject("TestCamera");
            _cameraObject.transform.rotation = rotation;
            return new TransformReferenceFrame(_cameraObject.transform);
        }

        private static Vector2 Rotate(Vector2 vector, float degrees)
        {
            var radians = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }

        private static void AssertFrame(MovementFrame actual, MovementFrame expected)
        {
            AssertVector(actual.Forward, expected.Forward);
            AssertVector(actual.Right, expected.Right);
        }

        private static void AssertVector(Vector3 actual, Vector3 expected)
        {
            Assert.That(Vector3.Distance(actual, expected), Is.LessThan(1e-4f), $"Expected {expected}, got {actual}.");
        }
    }
}
