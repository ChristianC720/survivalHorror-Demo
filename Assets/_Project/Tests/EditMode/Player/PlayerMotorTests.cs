using System;
using NUnit.Framework;
using Project.Player;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Tests.EditMode.Player
{
    /// <summary>
    /// Frame-rate independence and locomotion behavior of the pure motor.
    /// Expected values are derived analytically from the provisional default settings.
    /// </summary>
    public sealed class PlayerMotorTests
    {
        private const float ConstantSpeedTolerance = 0.005f;
        private const float AccelerationTolerance = 0.01f;
        private const float SteadySpeedTolerance = 0.005f;
        private const float FacingToleranceDegrees = 1f;
        private const float DiagonalTolerance = 0.001f;

        private static readonly int[] FrameRates = { 30, 60, 144 };
        private static readonly MovementFrame WorldFrame = new MovementFrame(Vector3.forward);
        private static readonly MotorInput Forward = new MotorInput(Vector2.up, false);
        private static readonly MotorInput ForwardRun = new MotorInput(Vector2.up, true);
        private static readonly MotorInput Neutral = new MotorInput(Vector2.zero, false);

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

        [TestCaseSource(nameof(FrameRates))]
        public void ConstantSpeed_OneSecond_CoversWalkSpeedDistance(int fps)
        {
            var motor = new PlayerMotor(_settings);
            Simulate(motor, Forward, fps, 1f); // reach steady speed

            var distance = Simulate(motor, Forward, fps, 1f).z;

            AssertRelative(distance, _settings.WalkSpeed, ConstantSpeedTolerance);
        }

        [TestCaseSource(nameof(FrameRates))]
        public void AccelerationFromRest_OneSecond_MatchesAnalytic(int fps)
        {
            var distance = Simulate(new PlayerMotor(_settings), Forward, fps, 1f).z;

            AssertRelative(distance, AnalyticDistanceFromRest(_settings.WalkSpeed, 1f), AccelerationTolerance);
        }

        [TestCaseSource(nameof(FrameRates))]
        public void AccelerationFromRest_PureAccelerationPhase_MatchesAnalytic(int fps)
        {
            // Run reaches 4.5 m/s after 0.25 s, so 1/6 s is entirely inside the acceleration phase.
            const float duration = 1f / 6f;
            var distance = Simulate(new PlayerMotor(_settings), ForwardRun, fps, duration).z;

            AssertRelative(distance, 0.5f * _settings.Acceleration * duration * duration, AccelerationTolerance);
        }

        [TestCaseSource(nameof(FrameRates))]
        public void Deceleration_FromWalk_MatchesAnalyticStoppingDistance(int fps)
        {
            var motor = new PlayerMotor(_settings);
            Simulate(motor, Forward, fps, 1f);

            var distance = Simulate(motor, Neutral, fps, 1f).z;
            var expected = _settings.WalkSpeed * _settings.WalkSpeed / (2f * _settings.Deceleration);

            AssertRelative(distance, expected, AccelerationTolerance);
            Assert.That(motor.HorizontalVelocity.magnitude, Is.EqualTo(0f).Within(1e-5f));
        }

        [TestCaseSource(nameof(FrameRates))]
        public void SteadySpeed_WalkAndRun_MatchSettings(int fps)
        {
            var walk = new PlayerMotor(_settings);
            Simulate(walk, Forward, fps, 1f);
            AssertRelative(walk.HorizontalVelocity.magnitude, _settings.WalkSpeed, SteadySpeedTolerance);

            var run = new PlayerMotor(_settings);
            Simulate(run, ForwardRun, fps, 1f);
            AssertRelative(run.HorizontalVelocity.magnitude, _settings.RunSpeed, SteadySpeedTolerance);
        }

        [Test]
        public void DiagonalInput_IsNormalized()
        {
            var motor = new PlayerMotor(_settings);
            Simulate(motor, new MotorInput(new Vector2(1f, 1f), false), 60, 1f);

            AssertRelative(motor.HorizontalVelocity.magnitude, _settings.WalkSpeed, DiagonalTolerance);
        }

        [Test]
        public void InputBelowDeadzone_DoesNotAccelerate()
        {
            var motor = new PlayerMotor(_settings);
            var tiny = new MotorInput(new Vector2(0f, _settings.InputDeadzone * 0.5f), false);

            var displacement = Simulate(motor, tiny, 60, 1f);

            Assert.That(new Vector2(displacement.x, displacement.z).magnitude, Is.EqualTo(0f));
        }

        [Test]
        public void ReleasingRun_DeceleratesToWalkSpeed()
        {
            var motor = new PlayerMotor(_settings);
            Simulate(motor, ForwardRun, 60, 1f);

            Simulate(motor, Forward, 60, 1f);

            AssertRelative(motor.HorizontalVelocity.magnitude, _settings.WalkSpeed, SteadySpeedTolerance);
        }

        [Test]
        public void MovementFollowsFrame()
        {
            var frame = new MovementFrame(new Vector3(1f, -1f, 1f));
            var motor = new PlayerMotor(_settings);

            var displacement = Step(motor, Forward, frame, 60, 1f);

            Assert.That(Vector3.Angle(Flat(displacement), frame.Forward), Is.LessThan(0.01f));
        }

        [TestCaseSource(nameof(FrameRates))]
        public void Facing_Turns180DegreesAtTurnSpeed(int fps)
        {
            var motor = new PlayerMotor(_settings, Vector3.forward);
            var backward = new MotorInput(Vector2.down, false);
            var halfTurnTime = 180f / _settings.TurnSpeed; // 1/3 s

            Simulate(motor, backward, fps, halfTurnTime * 0.5f);
            Assert.That(Vector3.Angle(motor.Facing, Vector3.forward), Is.EqualTo(90f).Within(FacingToleranceDegrees));

            Simulate(motor, backward, fps, halfTurnTime * 0.5f);
            Assert.That(Vector3.Angle(motor.Facing, Vector3.back), Is.LessThanOrEqualTo(FacingToleranceDegrees));
        }

        [Test]
        public void Facing_HoldsWithoutInput()
        {
            var motor = new PlayerMotor(_settings, Vector3.right);

            Simulate(motor, Neutral, 60, 1f);

            Assert.That(Vector3.Angle(motor.Facing, Vector3.right), Is.LessThanOrEqualTo(FacingToleranceDegrees));
        }

        [Test]
        public void Facing_OverrideTargetIsRespected()
        {
            var motor = new PlayerMotor(_settings, Vector3.forward);

            for (var i = 0; i < 60; i++)
            {
                motor.Step(Forward, WorldFrame, true, 1f / 60f, Vector3.left);
            }

            Assert.That(Vector3.Angle(motor.Facing, Vector3.left), Is.LessThanOrEqualTo(FacingToleranceDegrees));
        }

        [TestCaseSource(nameof(FrameRates))]
        public void Airborne_FallMatchesFreeFall(int fps)
        {
            var motor = new PlayerMotor(_settings);
            const float duration = 0.5f;

            var displacement = Step(motor, Neutral, WorldFrame, fps, duration, isGrounded: false);

            AssertRelative(displacement.y, 0.5f * _settings.Gravity * duration * duration, AccelerationTolerance);
        }

        [Test]
        public void Grounded_VerticalVelocityEqualsGroundedVelocity()
        {
            var motor = new PlayerMotor(_settings);
            Step(motor, Neutral, WorldFrame, 60, 0.5f, isGrounded: false);

            var result = motor.Step(Neutral, WorldFrame, true, 1f / 60f);

            Assert.That(result.Velocity.y, Is.EqualTo(_settings.GroundedVelocity));
            Assert.That(result.Displacement.y, Is.EqualTo(_settings.GroundedVelocity / 60f).Within(1e-6f));
        }

        [Test]
        public void NegativeDeltaTime_Throws()
        {
            var motor = new PlayerMotor(_settings);

            Assert.Throws<ArgumentOutOfRangeException>(() => motor.Step(Forward, WorldFrame, true, -0.1f));
        }

        [Test]
        public void NullSettings_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PlayerMotor(null));
        }

        private static float AnalyticDistanceFromRest(float targetSpeed, float duration)
        {
            // Uses the default acceleration of 18 m/s².
            const float acceleration = 18f;
            var accelerationTime = targetSpeed / acceleration;
            return 0.5f * acceleration * accelerationTime * accelerationTime
                   + targetSpeed * (duration - accelerationTime);
        }

        private static Vector3 Simulate(PlayerMotor motor, MotorInput input, int fps, float duration)
        {
            return Step(motor, input, WorldFrame, fps, duration);
        }

        private static Vector3 Step(PlayerMotor motor, MotorInput input, MovementFrame frame, int fps, float duration,
            bool isGrounded = true)
        {
            var steps = Mathf.RoundToInt(duration * fps);
            Assert.That(Mathf.Abs(steps - duration * fps), Is.LessThan(1e-3f), "Duration must be a whole number of frames.");

            var total = Vector3.zero;
            var deltaTime = 1f / fps;
            for (var i = 0; i < steps; i++)
            {
                total += motor.Step(input, frame, isGrounded, deltaTime).Displacement;
            }

            return isGrounded ? Flat(total) : total;
        }

        private static Vector3 Flat(Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        private static void AssertRelative(float actual, float expected, float relativeTolerance)
        {
            Assert.That(actual, Is.EqualTo(expected).Within(Mathf.Abs(expected) * relativeTolerance),
                $"Expected {expected} ±{relativeTolerance:P1}, got {actual}.");
        }
    }
}
