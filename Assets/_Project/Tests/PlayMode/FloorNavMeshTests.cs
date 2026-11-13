using System.Collections;
using System.Linq;
using Heartbeat.Generation;
using Heartbeat.Generation.Layout;
using Heartbeat.Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>
    /// Integration test from the GDD (Pruebas 2.2): build a floor, place Lukas, and check a NavMesh path to the key and the final room.
    /// Also run with the Showcase preset (C-66): 6 to 8 procedural + 2 secondary + 1 final room, enemies in the preset's range.
    /// </summary>
    public sealed class FloorNavMeshTests
    {
        private const string FloorScene = "Floor";   // in the build settings

        [UnityTest]
        public IEnumerator BuiltFloor_HasNavMeshPathFromPlayerToKeyAndFinalRoom([Values(1, 2, 3)] int floor, [Values(4242, 1234, 98765, 31337, 555)] int seedBase, [Values(false, true)] bool showcase)
        {
            yield return SceneManager.LoadSceneAsync(FloorScene, LoadSceneMode.Single);
            yield return null;   // FloorSession.Start builds a random floor first
            var builder = Object.FindAnyObjectByType<FloorBuilder>();
            Assert.IsNotNull(builder, "FloorBuilder not found in the Floor scene");

            builder.Clear();
            yield return null;   // Destroy is deferred: the old floor's walls must be gone before the new bake
            Assert.IsTrue(builder.Build(floor, seedBase + floor, false, false, showcase), $"floor {floor} seed {seedBase + floor}: no valid layout");
            // Checked in the same frame as the bake: closed and locked doors carve the NavMesh from the next frame on,
            // and this test checks the built layout, not the door states.
            var layout = builder.Layout;
            if (showcase) AssertShowcase(builder, floor, seedBase + floor);
            var from = builder.Player.transform.position;
            Assert.GreaterOrEqual(layout.FinalRoom, 0, "no final room");
            // Room volume center, not the pivot: the pivot is the outer wall corner, and KEY_F2_Final's stairs model keeps the NavMesh more than 2 m from it.
            var finalCenter = builder.Rooms[layout.FinalRoom].GetComponent<BoxCollider>().bounds.center;
            finalCenter.y = builder.Rooms[layout.FinalRoom].transform.position.y;
            AssertPath(from, finalCenter, $"floor {floor} seed {seedBase + floor}: final room");

            var key = layout.Items.FirstOrDefault(i => i.Kind == ItemKind.KeyMandatory);
            Assert.IsNotNull(key, $"floor {floor}: no mandatory key");
            var spawns = builder.Rooms[key.Room].GetComponentsInChildren<SpawnPoint>(true).Where(s => s.kind == SpawnKind.Item).ToList();
            AssertPath(from, spawns[key.Spawn].transform.position, $"floor {floor} seed {seedBase + floor}: key");
        }

        private static void AssertShowcase(FloorBuilder builder, int floor, int seed)
        {
            var cfg = builder.ConfigFor(floor, true);
            StringAssert.EndsWith("Showcase", cfg.name, "Showcase preset not used");
            var rooms = builder.Layout.Rooms;
            int procedural = rooms.Count(r => r.Template.Role == RoomRole.Procedural);
            Assert.That(procedural, Is.InRange(6, 8), $"floor {floor} seed {seed}: procedural rooms");
            Assert.AreEqual(2, rooms.Count(r => r.Template.Role == RoomRole.Secondary), $"floor {floor} seed {seed}: secondary rooms");
            Assert.AreEqual(1, rooms.Count(r => r.Template.Role == RoomRole.Final), $"floor {floor} seed {seed}: final rooms");
            Assert.That(builder.Layout.Enemies.Count, Is.InRange(cfg.enemies.min, cfg.enemies.max), $"floor {floor} seed {seed}: enemies");
        }

        private static void AssertPath(Vector3 a, Vector3 b, string what)
        {
            Assert.IsTrue(NavMesh.SamplePosition(a, out var ha, 2f, NavMesh.AllAreas), $"{what}: start not on the NavMesh");
            Assert.IsTrue(NavMesh.SamplePosition(b, out var hb, 2f, NavMesh.AllAreas), $"{what}: target not on the NavMesh");
            var path = new NavMeshPath();
            NavMesh.CalculatePath(ha.position, hb.position, NavMesh.AllAreas, path);
            Assert.AreEqual(NavMeshPathStatus.PathComplete, path.status, $"{what}: no complete NavMesh path from {ha.position} to {hb.position}, stops at {(path.corners.Length > 0 ? path.corners[path.corners.Length - 1] : a)}");
        }
    }
}
