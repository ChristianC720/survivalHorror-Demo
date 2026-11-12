using System.Collections;
using System.Collections.Generic;
using Heartbeat.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>C-61: walking, running, aiming or crouched, in the 8 directions, Lukas's hips never turn more than 45 degrees away from his chest.</summary>
    public sealed class LukasTwistTests
    {
        private const float MaxTwist = 45f, SampleSeconds = 0.7f;

        [UnityTest]
        public IEnumerator HipsNeverTwistMoreThan45DegreesFromChest_InTheEightDirections()
        {
            yield return SceneManager.LoadSceneAsync("Floor", LoadSceneMode.Single);
            // The reconstruction card at the start puts Lukas back on the start point after about 10 s.
            for (float t = 0f; t < 11f; t += Time.deltaTime) yield return null;

            var player = Object.FindAnyObjectByType<PlayerController>();
            var animator = player.GetComponentInChildren<Animator>();
            var health = player.GetComponent<PlayerHealth>();
            var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.transform.position = new Vector3(0f, -200.5f, 0f);
            slab.transform.localScale = new Vector3(400f, 1f, 400f);
            slab.layer = LayerMask.NameToLayer("Environment");

            var modes = new (string name, bool aim, bool crouch, bool run, HealthState state)[]
            {
                ("walk", false, false, false, HealthState.Fine),
                ("run", false, false, true, HealthState.Fine),
                ("aim", true, false, false, HealthState.Fine),
                ("crouch", false, true, false, HealthState.Fine),
                ("crouch+aim", true, true, false, HealthState.Fine),
                ("walk injured", false, false, false, HealthState.Injured),
                ("walk critical", false, false, false, HealthState.Critical),
                ("aim injured", true, false, false, HealthState.Injured),
                ("aim critical", true, false, false, HealthState.Critical),
                ("crouch+aim critical", true, true, false, HealthState.Critical),
            };
            var failures = new List<string>();
            float worst = 0f;
            try
            {
                foreach (var m in modes)
                {
                    health.Model.SetValue(ValueFor(health.Model, m.state));
                    player.IsAiming = m.aim;
                    player.DebugCrouch(m.crouch);
                    player.DebugRun = m.run;
                    for (int i = 0; i < 8; i++)
                    {
                        float yaw = player.CameraTransform != null ? player.CameraTransform.eulerAngles.y : player.transform.eulerAngles.y;
                        player.DebugTeleport(0f, -199.9f, 0f, yaw);
                        float a = i * 45f * Mathf.Deg2Rad;
                        player.DebugMove = new Vector2(Mathf.Sin(a), Mathf.Cos(a));
                        Vector3 start = player.transform.position;
                        float max = 0f;
                        for (float t = 0f; t < SampleSeconds; t += Time.deltaTime)
                        {
                            yield return null;
                            max = Mathf.Max(max, PlayerAnimator.HipChestTwist(animator));
                        }
                        float moved = Vector3.Distance(start, player.transform.position);
                        worst = Mathf.Max(worst, max);
                        Debug.Log($"[Heartbeat][Test] C-61 {m.name} {i * 45}°: twist {max:0.0}°");
                        if (moved < 0.3f) failures.Add($"{m.name} {i * 45}°: did not move ({moved:0.00} m)");
                        if (max > MaxTwist) failures.Add($"{m.name} {i * 45}°: twist {max:0.0}°");
                    }
                }
            }
            finally
            {
                player.DebugMove = null;
                player.DebugRun = false;
                player.IsAiming = false;
                player.DebugCrouch(false);
                health.Model.Restore();
            }
            Debug.Log($"[Heartbeat][Test] C-61 worst hip-chest twist {worst:0.0}°");
            Assert.IsEmpty(failures, string.Join("\n", failures));
        }

        private static float ValueFor(HealthModel model, HealthState state)
        {
            for (float v = model.Max; v > 0f; v -= model.Max * 0.01f)
                if (model.StateFor(v) == state) return v;
            Assert.Fail("no health value gives " + state);
            return model.Max;
        }
    }
}
