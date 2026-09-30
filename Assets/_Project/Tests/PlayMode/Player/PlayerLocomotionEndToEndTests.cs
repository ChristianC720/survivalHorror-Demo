using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Project.Core.Bootstrap;
using Project.Core.Scenes;
using Project.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Project.Tests.PlayMode.Player
{
    /// <summary>
    /// Full chain: simulated keyboard → Bootstrap-owned input → SceneEntryPoint → PlayerController in Sandbox.
    /// </summary>
    public sealed class PlayerLocomotionEndToEndTests : InputTestFixture
    {
        private const string BootstrapSceneName = "Bootstrap";
        private const string SandboxSceneName = "Sandbox";
        private const float TimeoutSeconds = 10f;
        private const float EndToEndTolerance = 0.02f;
        private const float DirectionToleranceDegrees = 2f;

        // Provisional defaults from DefaultMovementSettings.
        private const float WalkSpeed = 2.5f;
        private const float RunSpeed = 4.5f;
        private const float Acceleration = 18f;

        private Keyboard _keyboard;

        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
            InputSystem.AddDevice<Mouse>();
        }

        public override void TearDown()
        {
            foreach (var root in Object.FindObjectsByType<ProjectBootstrap>())
            {
                Object.DestroyImmediate(root.gameObject);
            }

            base.TearDown();
        }

        [UnityTest]
        public IEnumerator Bootstrap_InitializesEntryPointBeforeStart_AndEnablesPlayerMap()
        {
            yield return LoadThroughBootstrap();

            // SceneEntryPoint.Start logs an error when not initialized; none may have been logged.
            LogAssert.NoUnexpectedReceived();
            var entry = FindEntryPoint();
            Assert.That(entry.IsInitialized, Is.True);
            Assert.That(entry.Player.enabled, Is.True);
            Assert.That(FindBootstrap().Input.Player.enabled, Is.True);
        }

        [UnityTest]
        public IEnumerator HoldingW_MovesAlongCameraForward_AtAnalyticDistance()
        {
            yield return LoadThroughBootstrap();
            var player = FindEntryPoint().Player;
            var start = player.transform.position;

            Press(_keyboard.wKey);
            var elapsed = 0f;
            yield return Hold(1f, t => elapsed = t);

            var displacement = Flat(player.transform.position - start);
            Assert.That(Vector3.Angle(displacement, CameraForward()), Is.LessThanOrEqualTo(DirectionToleranceDegrees));
            var expected = DistanceFromRest(WalkSpeed, elapsed);
            Assert.That(displacement.magnitude, Is.EqualTo(expected).Within(expected * EndToEndTolerance));
        }

        [UnityTest]
        public IEnumerator HoldingWAndLeftShift_RunsAtRunSpeed()
        {
            yield return LoadThroughBootstrap();
            var player = FindEntryPoint().Player;

            PressKeys(Key.W, Key.LeftShift);
            yield return Hold(0.5f, null);
            Assert.That(_keyboard.wKey.isPressed && _keyboard.leftShiftKey.isPressed, Is.True);

            yield return AssertSteadySpeed(player, RunSpeed);
        }

        [UnityTest]
        public IEnumerator HoldingWAndD_MovesDiagonallyAtWalkSpeed()
        {
            yield return LoadThroughBootstrap();
            var player = FindEntryPoint().Player;

            PressKeys(Key.W, Key.D);
            yield return Hold(0.5f, null);
            Assert.That(_keyboard.wKey.isPressed && _keyboard.dKey.isPressed, Is.True);

            var start = player.transform.position;
            yield return AssertSteadySpeed(player, WalkSpeed);

            var forward = CameraForward();
            var diagonal = (forward + Vector3.Cross(Vector3.up, forward)).normalized;
            Assert.That(Vector3.Angle(Flat(player.transform.position - start), diagonal),
                Is.LessThanOrEqualTo(DirectionToleranceDegrees));
        }

        [UnityTest]
        public IEnumerator ReleasingW_StopsThePlayer()
        {
            yield return LoadThroughBootstrap();
            var player = FindEntryPoint().Player;

            Press(_keyboard.wKey);
            yield return Hold(0.5f, null);
            Release(_keyboard.wKey);
            yield return Hold(0.3f, null);

            var stopped = player.transform.position;
            yield return Hold(0.2f, null);

            Assert.That(player.IsMoving, Is.False);
            Assert.That(Vector3.Distance(Flat(player.transform.position), Flat(stopped)), Is.LessThan(1e-4f));
        }

        [UnityTest]
        public IEnumerator CameraTurnWhileHeld_KeepsDirection_UntilInputIsReleased()
        {
            yield return LoadThroughBootstrap();
            var player = FindEntryPoint().Player;
            var cameraTransform = FindCamera();
            var originalForward = CameraForward();

            Press(_keyboard.wKey);
            yield return Hold(0.3f, null);

            cameraTransform.rotation = Quaternion.AngleAxis(90f, Vector3.up) * cameraTransform.rotation;
            var turnedForward = CameraForward();
            Assert.That(Vector3.Angle(originalForward, turnedForward), Is.EqualTo(90f).Within(0.1f));

            var heldStart = player.transform.position;
            yield return Hold(0.5f, null);
            Assert.That(Vector3.Angle(Flat(player.transform.position - heldStart), originalForward),
                Is.LessThanOrEqualTo(DirectionToleranceDegrees), "Held input must keep the captured frame.");

            Release(_keyboard.wKey);
            yield return Hold(0.3f, null);

            var repressStart = player.transform.position;
            Press(_keyboard.wKey);
            yield return Hold(0.5f, null);
            Assert.That(Vector3.Angle(Flat(player.transform.position - repressStart), turnedForward),
                Is.LessThanOrEqualTo(DirectionToleranceDegrees), "New input must follow the current camera.");
        }

        [UnityTest]
        public IEnumerator SandboxWithoutBootstrap_LogsOneErrorAndPlayerStaysDisabled()
        {
            LogAssert.Expect(LogType.Error, new Regex("was not initialized by ProjectBootstrap"));
            SceneManager.LoadScene(SandboxSceneName, LoadSceneMode.Single);
            yield return null;
            yield return null;

            var entry = FindEntryPoint();
            var player = entry.Player;
            var start = player.transform.position;

            Press(_keyboard.wKey);
            yield return Hold(0.3f, null);

            Assert.That(entry.IsInitialized, Is.False);
            Assert.That(player.enabled, Is.False);
            Assert.That(player.transform.position, Is.EqualTo(start));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator UnloadingSandbox_DisablesPlayerMap()
        {
            yield return LoadThroughBootstrap();
            var input = FindBootstrap().Input;
            Assert.That(input.Player.enabled, Is.True);

            var placeholder = SceneManager.CreateScene("EndToEndPlaceholder");
            SceneManager.SetActiveScene(placeholder);
            yield return SceneManager.UnloadSceneAsync(SandboxSceneName);

            Assert.That(input.Player.enabled, Is.False);
        }

        /// <summary>Sends one full keyboard state with all keys held, as real hardware does.</summary>
        private void PressKeys(params Key[] keys)
        {
            InputSystem.QueueStateEvent(_keyboard, new UnityEngine.InputSystem.LowLevel.KeyboardState(keys));
            InputSystem.Update();
        }

        private IEnumerator LoadThroughBootstrap()
        {
            SceneManager.LoadScene(BootstrapSceneName, LoadSceneMode.Single);
            yield return null;

            var deadline = Time.realtimeSinceStartup + TimeoutSeconds;
            while (!IsReady())
            {
                if (Time.realtimeSinceStartup > deadline)
                {
                    Assert.Fail($"Timed out waiting for '{SandboxSceneName}' to be initialized through Bootstrap.");
                }

                yield return null;
            }

            yield return Hold(0.2f, null); // let the player settle onto the floor
        }

        private static bool IsReady()
        {
            if (SceneManager.GetActiveScene().name != SandboxSceneName)
            {
                return false;
            }

            var entries = Object.FindObjectsByType<SceneEntryPoint>();
            return entries.Length == 1 && entries[0].IsInitialized;
        }

        private static IEnumerator AssertSteadySpeed(PlayerController player, float expectedSpeed)
        {
            var start = player.transform.position;
            var elapsed = 0f;
            yield return Hold(0.5f, t => elapsed = t);

            var speed = Flat(player.transform.position - start).magnitude / elapsed;
            Assert.That(speed, Is.EqualTo(expectedSpeed).Within(expectedSpeed * EndToEndTolerance));
        }

        /// <summary>Waits at least <paramref name="seconds"/>; reports the exact accumulated frame time.</summary>
        private static IEnumerator Hold(float seconds, System.Action<float> onElapsed)
        {
            var elapsed = 0f;
            while (elapsed < seconds)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            onElapsed?.Invoke(elapsed);
        }

        private static float DistanceFromRest(float targetSpeed, float duration)
        {
            var accelerationTime = targetSpeed / Acceleration;
            if (duration <= accelerationTime)
            {
                return 0.5f * Acceleration * duration * duration;
            }

            return 0.5f * Acceleration * accelerationTime * accelerationTime + targetSpeed * (duration - accelerationTime);
        }

        private static SceneEntryPoint FindEntryPoint()
        {
            var entries = Object.FindObjectsByType<SceneEntryPoint>();
            Assert.That(entries.Length, Is.EqualTo(1));
            return entries[0];
        }

        private static ProjectBootstrap FindBootstrap()
        {
            var roots = Object.FindObjectsByType<ProjectBootstrap>();
            Assert.That(roots.Length, Is.EqualTo(1));
            return roots[0];
        }

        private static Transform FindCamera()
        {
            var cameras = Object.FindObjectsByType<Camera>();
            Assert.That(cameras.Length, Is.EqualTo(1));
            return cameras[0].transform;
        }

        private static Vector3 CameraForward()
        {
            return Flat(FindCamera().forward).normalized;
        }

        private static Vector3 Flat(Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }
    }
}
