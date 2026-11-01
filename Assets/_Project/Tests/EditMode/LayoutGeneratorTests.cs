using System.Collections.Generic;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Generation;
using Heartbeat.Generation.Layout;
using Heartbeat.Inventory;
using Heartbeat.Level;
using NUnit.Framework;
using UnityEngine;

namespace Heartbeat.Tests
{
    public class LayoutGeneratorTests
    {
        private static SocketData S(int x, int y, Facing f) => new SocketData { cell = new Vector2Int(x, y), facing = f, allowLocked = true };

        private static RoomTemplate T(string id, RoomRole role, RoomArchetype a, int w, int h, int enemies, ItemSpawnTag[] items, params SocketData[] sockets) =>
            new RoomTemplate { Id = id, Role = role, Archetype = a, Size = new Vector2Int(w, h), EnemySpawns = enemies, ItemSpawns = items.ToList(), Sockets = sockets.ToList(), CanBeUnpowered = role == RoomRole.Final };

        private static readonly ItemSpawnTag[] ProcItems = { ItemSpawnTag.Budget, ItemSpawnTag.Budget, ItemSpawnTag.Document, ItemSpawnTag.Confiscated };
        private static readonly ItemSpawnTag[] KeyItems = { ItemSpawnTag.Key, ItemSpawnTag.Emilia, ItemSpawnTag.Switch, ItemSpawnTag.Budget };

        /// <summary>A library shaped like the real F1 set (sizes and socket layouts from Desarrollo/14-levels.md).</summary>
        public static List<RoomTemplate> Library() => new List<RoomTemplate>
        {
            T("Capsules", RoomRole.CapsuleRow, RoomArchetype.None, 4, 3, 0, new[] { ItemSpawnTag.Budget }, S(1, 2, Facing.North), S(3, 1, Facing.East)),
            T("Corridor", RoomRole.Procedural, RoomArchetype.Corridor, 2, 5, 1, ProcItems, S(0, 0, Facing.South), S(1, 4, Facing.North), S(1, 2, Facing.East)),
            T("Storage", RoomRole.Procedural, RoomArchetype.Storage, 5, 5, 2, ProcItems, S(2, 0, Facing.South), S(4, 3, Facing.East), S(0, 1, Facing.West)),
            T("Open", RoomRole.Procedural, RoomArchetype.Open, 5, 5, 2, ProcItems, S(2, 0, Facing.South), S(2, 4, Facing.North), S(4, 2, Facing.East), S(0, 2, Facing.West)),
            T("Surveillance", RoomRole.Procedural, RoomArchetype.Surveillance, 3, 3, 1, ProcItems, S(1, 0, Facing.South), S(0, 1, Facing.West), S(2, 2, Facing.North)),
            T("Hack", RoomRole.Procedural, RoomArchetype.Hack, 3, 4, 1, ProcItems, S(1, 0, Facing.South), S(1, 3, Facing.North), S(2, 1, Facing.East)),
            T("Vertical", RoomRole.Procedural, RoomArchetype.Vertical, 4, 3, 2, ProcItems, S(0, 0, Facing.South), S(3, 2, Facing.North)),
            T("SecA", RoomRole.Secondary, RoomArchetype.None, 3, 3, 0, KeyItems, S(1, 0, Facing.South)),
            T("SecB", RoomRole.Secondary, RoomArchetype.None, 3, 3, 0, KeyItems, S(0, 1, Facing.West)),
            T("Final", RoomRole.Final, RoomArchetype.None, 4, 4, 0, new[] { ItemSpawnTag.Budget }, S(1, 0, Facing.South)),
        };

        public static FloorGenParams Full(int floor, int seed, bool first = false)
        {
            var p = new FloorGenParams { Floor = floor, Seed = seed, FirstAttempt = first && floor == 1, TutorialRoomIds = new[] { "Storage", "Corridor", "Open" } };
            switch (floor)
            {
                case 1: p.Heals = new IntRange(2, 3); p.Ammo = new IntRange(12, 16); p.Spikes = new IntRange(2, 3); p.OptionalKeys = new IntRange(1, 2); p.Enemies = new IntRange(6, 9); break;
                case 2: p.Heals = new IntRange(1, 2); p.Ammo = new IntRange(8, 12); p.Spikes = new IntRange(1, 2); p.OptionalKeys = new IntRange(0, 1); p.Enemies = new IntRange(9, 12); break;
                default: p.Heals = new IntRange(1, 1); p.Ammo = new IntRange(4, 8); p.Spikes = new IntRange(0, 1); p.OptionalKeys = new IntRange(0, 1); p.Enemies = new IntRange(12, 15); p.UnpoweredChance = 0f; break;
            }
            return p;
        }

        public static FloorGenParams Showcase(int floor, int seed)
        {
            var p = Full(floor, seed);
            p.Procedural = new IntRange(6, 8);
            p.Secondary = new IntRange(2, 2);
            switch (floor)
            {
                case 1: p.Heals = new IntRange(2, 2); p.Ammo = new IntRange(8, 12); p.Spikes = new IntRange(2, 2); p.OptionalKeys = new IntRange(0, 1); p.Enemies = new IntRange(5, 7); break;
                case 2: p.Heals = new IntRange(1, 1); p.Ammo = new IntRange(4, 8); p.Spikes = new IntRange(1, 2); p.Enemies = new IntRange(7, 9); break;
                default: p.Heals = new IntRange(1, 1); p.Ammo = new IntRange(4, 4); p.Spikes = new IntRange(0, 1); p.OptionalKeys = new IntRange(0, 0); p.Enemies = new IntRange(9, 11); break;
            }
            return p;
        }

        private static void RunSeeds(System.Func<int, FloorGenParams> make, int count = 1000)
        {
            var lib = Library();
            int maxAttempts = 0;
            for (int seed = 0; seed < count; seed++)
            {
                var p = make(seed * 101);
                var layout = LayoutGenerator.Generate(p, lib);
                Assert.IsNotNull(layout, $"seed {p.Seed} produced no valid layout");
                Assert.IsTrue(LayoutValidator.Validate(layout, p, out var errors), $"seed {p.Seed}: {string.Join("; ", errors)}");
                maxAttempts = Mathf.Max(maxAttempts, layout.Attempts);
            }
            Debug.Log($"[Heartbeat][Gen] {count} seeds OK, worst case {maxAttempts} attempts");
        }

        [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void Full_1000Seeds_AllValid(int floor) => RunSeeds(s => Full(floor, s));

        [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void Showcase_1000Seeds_AllValid(int floor) => RunSeeds(s => Showcase(floor, s));

        [Test]
        public void FirstAttempt_Floor1_1000Seeds_WithTutorial() => RunSeeds(s => Full(1, s, first: true));

        [Test]
        public void StoryMode_RaisesMinimums_1000Seeds()
        {
            RunSeeds(s => { var p = Full(1, s); p.ApplyStoryMode(); return p; });
        }

        [Test]
        public void SameSeed_SameLayout()
        {
            var a = LayoutGenerator.Generate(Full(1, 1234), Library());
            var b = LayoutGenerator.Generate(Full(1, 1234), Library());
            Assert.AreEqual(a.Rooms.Count, b.Rooms.Count);
            for (int i = 0; i < a.Rooms.Count; i++)
            {
                Assert.AreEqual(a.Rooms[i].Template.Id, b.Rooms[i].Template.Id);
                Assert.AreEqual(a.Rooms[i].Origin, b.Rooms[i].Origin);
                Assert.AreEqual(a.Rooms[i].Rotation, b.Rooms[i].Rotation);
            }
            Assert.AreEqual(a.Items.Count, b.Items.Count);
            Assert.AreEqual(a.Enemies.Count, b.Enemies.Count);
        }

        [Test]
        public void DifferentSeeds_GiveDifferentFloors()
        {
            var a = LayoutGenerator.Generate(Full(1, 1), Library());
            var b = LayoutGenerator.Generate(Full(1, 2), Library());
            bool same = a.Rooms.Count == b.Rooms.Count && a.Rooms.Zip(b.Rooms, (x, y) => x.Template.Id == y.Template.Id && x.Origin == y.Origin).All(v => v);
            Assert.IsFalse(same);
        }

        [Test]
        public void TutorialRooms_AreFixedAndInOrder_OnFirstAttempt()
        {
            var l = LayoutGenerator.Generate(Full(1, 77, first: true), Library());
            Assert.AreEqual("Capsules", l.Rooms[0].Template.Id);
            Assert.AreEqual("Storage", l.Rooms[1].Template.Id);
            Assert.AreEqual(2, l.Rooms[1].TutorialIndex);
            Assert.AreEqual("Corridor", l.Rooms[2].Template.Id);
            Assert.AreEqual("Open", l.Rooms[3].Template.Id);
            // Depot holds 1 heal + 1 blister; boiler room holds the real Rastrero (D-70)
            Assert.IsTrue(l.Items.Any(i => i.Room == 1 && i.Kind == ItemKind.Heal));
            Assert.IsTrue(l.Items.Any(i => i.Room == 1 && i.Kind == ItemKind.Ammo));
            Assert.IsTrue(l.Enemies.Any(e => e.Room == 3 && e.Type == EnemyType.Rastrero));
            Assert.IsFalse(l.FinalUnpowered, "first attempt never starts unpowered");
        }

        [Test]
        public void Floor3_NeverUnpowered()
        {
            for (int s = 0; s < 200; s++)
            {
                var l = LayoutGenerator.Generate(Full(3, s), Library());
                Assert.IsNotNull(l);
                Assert.IsFalse(l.FinalUnpowered);
            }
        }

        [Test]
        public void SomeFloors1And2_StartUnpowered_WithReachableSwitch()
        {
            int unpowered = 0;
            for (int s = 0; s < 300; s++)
            {
                var l = LayoutGenerator.Generate(Full(1, s), Library());
                if (!l.FinalUnpowered) continue;
                unpowered++;
                Assert.GreaterOrEqual(l.SwitchRoom, 0);
            }
            Assert.That(unpowered, Is.InRange(50, 160), "about 1/3 of floors");
        }

        [Test]
        public void Validator_RejectsFinalWithoutKeyDoor()
        {
            var p = Full(1, 5);
            var l = LayoutGenerator.Generate(p, Library());
            l.Connections.First(c => c.A == l.FinalRoom || c.B == l.FinalRoom).Lock = DoorLock.None;
            Assert.IsFalse(LayoutValidator.Validate(l, p, out var errors));
            Assert.IsTrue(errors.Any(e => e.Contains("final")));
        }

        [Test]
        public void SecondaryRooms_AreAtDepthTwoOrMore()
        {
            for (int s = 0; s < 200; s++)
                foreach (var r in LayoutGenerator.Generate(Full(2, s), Library()).Rooms.Where(r => r.Template.Role == RoomRole.Secondary))
                    Assert.GreaterOrEqual(r.Depth, 2);
        }
    }
}
