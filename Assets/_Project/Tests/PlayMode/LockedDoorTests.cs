using System.Collections;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Generation;
using Heartbeat.Level;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>C-11: enemies never open or cross a key-locked door; only Lukas unlocks it.</summary>
    public sealed class LockedDoorTests
    {
        [UnityTest]
        public IEnumerator HuntingEnemy_NextToKeyLockedDoor_NeitherOpensNorCrossesIt()
        {
            yield return SceneManager.LoadSceneAsync("Floor", LoadSceneMode.Single);
            yield return null;
            var builder = Object.FindAnyObjectByType<FloorBuilder>();
            builder.Clear();
            yield return null;
            Assert.IsTrue(builder.Build(1, 4243, false, false, false));
            yield return null;
            yield return null;

            var locked = builder.Doors.First(d => d.Lock == DoorLock.MandatoryKey);
            Assert.IsFalse(locked.EnemyCanReach, "a locked door is never a target for enemies");
            Assert.IsFalse(locked.TryOpenByEnemy(), "an enemy cannot open a key-locked door");
            Assert.IsTrue(locked.GetComponentsInChildren<NavMeshObstacle>(true).Any(o => o.enabled), "a locked door carves the NavMesh");

            // A hunting enemy right in front of it for 2 s: the door stays shut.
            var enemy = Object.FindObjectsByType<EnemyBrain>(FindObjectsSortMode.None).First();
            var p = locked.transform.position + locked.transform.forward * 1f;
            enemy.DebugWarp(p.x, p.y, p.z, 0f);
            enemy.DebugForceHunt();
            for (float t = 0f; t < 2f; t += Time.deltaTime) yield return null;
            Assert.AreEqual(DoorState.Closed, locked.State);

            // Contrast: an ordinary closed door opens for the same enemy.
            var open = builder.Doors.First(d => d.Lock == DoorLock.None && !d.EnemyBarrier && d.State == DoorState.Closed);
            Assert.IsTrue(open.TryOpenByEnemy());
        }
    }
}
