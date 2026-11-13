using System.Collections;
using System.Linq;
using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.Level;
using Heartbeat.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>
    /// C-69: holding Interact on a closed door opens the quick seal hack (2-3 inputs). Completing it seals the door and
    /// raises DoorSealed (telemetry door_sealed); failing it (a hit) leaves the door closed and unsealed.
    /// </summary>
    public sealed class DoorSealHackTests
    {
        [UnityTest]
        public IEnumerator HoldOnClosedDoor_OpensQuickSealHack_SuccessSeals_FailureLeavesItClosed()
        {
            yield return SceneManager.LoadSceneAsync("Floor", LoadSceneMode.Single);
            for (float t = 0f; t < 11f; t += Time.deltaTime) yield return null;   // reconstruction card

            var player = Object.FindAnyObjectByType<PlayerController>();
            var interactor = player.GetComponentInChildren<Interactor>();
            var hack = Object.FindAnyObjectByType<PlayerHack>();
            var health = player.GetComponent<PlayerHealth>();
            var door = Object.FindObjectsByType<DoorLeaf>().OrderBy(d => d.name).First();
            door.Configure(DoorLock.None);
            door.DebugSetState("Closed");

            int sealedEvents = 0;
            System.Action<MonoBehaviour> onSealed = _ => sealedEvents++;
            GameEvents.DoorSealed += onSealed;
            try
            {
                // 1. The completed hold opens the hack; the door is not sealed yet.
                Assert.IsTrue(door.CanHold);
                door.HoldComplete(interactor);
                Assert.IsTrue(hack.IsHacking, "holding Interact opens the seal hack");
                Assert.That(hack.Current.Steps.Count, Is.InRange(2, 3), "quick seal hack: 2 or 3 arrows");
                Assert.AreEqual(DoorState.Closed, door.State);
                Assert.AreEqual(0, sealedEvents);

                // 2. Completing it seals the door.
                hack.DebugSolve();
                yield return null;
                Assert.IsFalse(hack.IsHacking);
                Assert.AreEqual(DoorState.Sealed, door.State, "a completed hack seals the door");
                Assert.AreEqual(1, sealedEvents);

                // 3. Failing it (a hit cancels the hack) leaves the door closed and unsealed.
                door.DebugSetState("Closed");
                door.HoldComplete(interactor);
                Assert.IsTrue(hack.IsHacking);
                health.DebugInvulnerable = true;
                health.ApplyHit(100);
                yield return null;
                Assert.IsFalse(hack.IsHacking, "a hit cancels the hack");
                Assert.AreEqual(DoorState.Closed, door.State, "a failed hack does not seal the door");
                Assert.AreEqual(1, sealedEvents);
            }
            finally
            {
                GameEvents.DoorSealed -= onSealed;
                if (health != null) health.DebugInvulnerable = false;
            }
        }
    }
}
