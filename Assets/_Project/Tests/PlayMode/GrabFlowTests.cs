using System.Collections;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Combat;
using Heartbeat.Inventory;
using Heartbeat.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>
    /// C-64: El Alto's grab on a built floor. The windup pulls Lukas to the anchor facing the Alto (stood up if crouched),
    /// holds him, and lets go on the NavMesh without crossing a wall. Without a spike Lukas drops a state; with one the
    /// spike is spent and the Alto is stunned; a stagger (weak-point hit) frees him with no damage.
    /// </summary>
    public sealed class GrabFlowTests
    {
        private PlayerController player;
        private PlayerHealth health;
        private PlayerInventory inventory;
        private EnemyBrain alto;
        private Vector3? home;

        [UnityTest]
        public IEnumerator AltoGrab_PullsHoldsAndReleases_WithAndWithoutSpike()
        {
            yield return SceneManager.LoadSceneAsync("Floor", LoadSceneMode.Single);
            for (float t = 0f; t < 11f; t += Time.deltaTime) yield return null;   // reconstruction card

            player = Object.FindAnyObjectByType<PlayerController>();
            health = player.GetComponent<PlayerHealth>();
            inventory = player.GetComponent<PlayerInventory>();
            var brains = Object.FindObjectsByType<EnemyBrain>();
            alto = brains.FirstOrDefault(b => b.Definition.type == EnemyType.Alto);
            if (alto == null) Assert.Inconclusive("no Alto on this floor");
            foreach (var b in brains) if (b != alto) b.gameObject.SetActive(false);

            // 1. Crouched, no spike: pulled to the anchor, stood up, drops one state.
            yield return Stage(crouched: true, spikes: 0);
            var before = health.State;
            Vector3 start = player.transform.position;
            alto.DebugStartGrab();
            Assert.IsTrue(player.Grabbed, "the windup grabs Lukas");
            Assert.IsFalse(player.IsCrouched, "a grab stands Lukas up");
            yield return Seconds(alto.Definition.attackWindup + 0.1f);
            AssertHeldAtAnchor();
            Assert.Greater((int)health.State, (int)before, "a grab without a spike drops one state");
            yield return Released(start);
            // The cooldown runs after the hands let go, so Lukas gets the full attackCooldown to step away.
            for (float t = 0f; t < alto.Definition.attackCooldown - 0.2f; t += Time.deltaTime)
            {
                Assert.IsFalse(alto.IsWindingUp, $"the Alto winds up again {t:0.00} s after letting go");
                yield return null;
            }

            // 2. One spike: spent, the Alto is stunned, Lukas is free and unhurt.
            yield return Stage(crouched: false, spikes: 1);
            before = health.State;
            start = player.transform.position;
            alto.DebugStartGrab();
            yield return Seconds(alto.Definition.attackWindup + 0.1f);
            Assert.IsFalse(inventory.Model.Has(ItemKind.Spike), "the spike is spent");
            Assert.IsTrue(alto.IsStaggered, "the spike stuns the Alto");
            Assert.AreEqual(before, health.State, "no damage with a spike");
            yield return Released(start);

            // 3. A weak-point stagger during the windup frees Lukas before the hands close.
            yield return Stage(crouched: false, spikes: 0);
            before = health.State;
            start = player.transform.position;
            alto.DebugStartGrab();
            yield return Seconds(0.3f);
            alto.Stagger(player.transform.position, alto.Definition.weakPointStagger);
            yield return null;
            Assert.IsFalse(player.Grabbed, "a weak-point stagger interrupts the grab");
            Assert.AreEqual(before, health.State, "an interrupted grab does no damage");
            yield return Released(start);
        }

        /// <summary>Lukas 1,5 m in front of the Alto, both on the NavMesh, healthy and with the given spikes.</summary>
        private IEnumerator Stage(bool crouched, int spikes)
        {
            yield return Seconds(2.2f);   // let any stun and cooldown run out
            // The hunting Alto may have grabbed Lukas on its own meanwhile; a hold blocks the next windup, so end it.
            alto.Stagger(alto.transform.position, 0.05f);
            yield return Seconds(0.1f);
            health.Model.SetValue(health.Model.Max);
            while (inventory.Model.TryConsume(ItemKind.Spike)) { }
            for (int i = 0; i < spikes; i++) inventory.Model.Add(ItemKind.Spike, 1);

            // Every stage from the Alto's first spot: a hunting Alto can wander next to a wall between stages (C-66).
            if (home == null) home = alto.transform.position;
            Vector3 a = home.Value;
            for (int i = 0; i < 16; i++)
            {
                Vector3 dir = Quaternion.Euler(0f, i * 22.5f, 0f) * Vector3.forward;
                if (!NavMesh.SamplePosition(a + dir * 1.5f, out var hit, 0.2f, NavMesh.AllAreas)) continue;
                if (Physics.Linecast(a + Vector3.up, hit.position + Vector3.up, LayerMask.GetMask("Environment"))) continue;
                player.DebugTeleport(hit.position.x, hit.position.y, hit.position.z, Quaternion.LookRotation(-dir).eulerAngles.y);
                player.DebugCrouch(crouched);
                alto.DebugWarp(a.x, a.y, a.z, Quaternion.LookRotation(dir).eulerAngles.y);
                yield return null;
                yield break;
            }
            Assert.Inconclusive("no free spot 1,5 m from the Alto");
        }

        private void AssertHeldAtAnchor()
        {
            Assert.IsTrue(player.Grabbed, "held after the hands close");
            Vector3 to = alto.transform.position - player.transform.position; to.y = 0f;
            Assert.AreEqual(alto.Definition.grabAnchorDistance, to.magnitude, 0.15f, "Lukas at the anchor");
            Assert.Less(Vector3.Angle(player.transform.forward, to), 20f, "Lukas faces the Alto");
        }

        private IEnumerator Released(Vector3 start)
        {
            for (float t = 0f; t < 2f && player.Grabbed; t += Time.deltaTime) yield return null;
            Assert.IsFalse(player.Grabbed, "released");
            Vector3 p = player.transform.position;
            Assert.IsTrue(NavMesh.SamplePosition(p, out _, 0.3f, NavMesh.AllAreas), $"Lukas off the NavMesh at {p}");
            Assert.IsFalse(Physics.Linecast(start + Vector3.up, p + Vector3.up, LayerMask.GetMask("Environment")), "Lukas crossed a wall");
        }

        private static IEnumerator Seconds(float s)
        {
            for (float t = 0f; t < s; t += Time.deltaTime) yield return null;
        }
    }
}
