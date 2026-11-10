using System;
using System.Collections.Generic;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Inventory;
using Heartbeat.Level;
using Random = System.Random;

namespace Heartbeat.Generation.Layout
{
    /// <summary>
    /// Fills a grown layout with its floor budget (D-17, D-37, D-70, D-72, D-76, D-79): mandatory key, power switch,
    /// Emilia documents, optional-key doors and keys, heals/ammo/spikes (guaranteed minimum never behind an optional
    /// door), context documents, confiscated objects and enemies (max 2 per room, ~1/3 of rooms empty).
    /// </summary>
    public static class BudgetDistributor
    {
        public static void Distribute(FloorLayout layout, FloorGenParams p, Random rng)
        {
            var taken = new HashSet<(int room, int spawn)>();
            var secondaries = layout.Rooms.Where(r => r.Template.Role == RoomRole.Secondary).ToList();

            // Mandatory key, Emilia documents and the power switch live in secondary key rooms (D-36, D-52, D-72).
            if (secondaries.Count > 0)
            {
                var keyRoom = secondaries[rng.Next(secondaries.Count)];
                layout.KeyRoom = keyRoom.Index;
                Place(layout, taken, keyRoom, ItemSpawnTag.Key, ItemKind.KeyMandatory, 1, ItemSpawnTag.Key, rng);

                foreach (var r in secondaries.OrderBy(_ => rng.Next()).Take(2))
                {
                    layout.EmiliaRooms.Add(r.Index);
                    Place(layout, taken, r, ItemSpawnTag.Emilia, ItemKind.None, 0, ItemSpawnTag.Emilia, rng);
                }

                bool canUnpower = layout.FinalRoom >= 0 && layout.Rooms[layout.FinalRoom].Template.CanBeUnpowered && !p.FirstAttempt;
                if (canUnpower && rng.NextDouble() < p.UnpoweredChance)
                {
                    layout.FinalUnpowered = true;
                    var sw = secondaries[rng.Next(secondaries.Count)];
                    layout.SwitchRoom = sw.Index;
                    Place(layout, taken, sw, ItemSpawnTag.Switch, ItemKind.None, 0, ItemSpawnTag.Switch, rng);
                }
            }

            // Optional-key doors on procedural leaves (never entry, tutorial or key rooms).
            int optional = p.OptionalKeys.Pick(rng);
            var leaves = layout.Rooms.Where(r => r.Template.Role == RoomRole.Procedural && r.TutorialIndex == 0 && r.Index != layout.EntryRoom
                                                 && layout.Neighbours(r.Index).Count() == 1).OrderBy(_ => rng.Next()).Take(optional).ToList();
            foreach (var leaf in leaves)
            {
                var c = layout.Connections.First(x => x.A == leaf.Index || x.B == leaf.Index);
                c.Lock = DoorLock.OptionalKey;
                leaf.BehindOptionalDoor = true;
            }

            var reachable = layout.Rooms.Where(r => !r.BehindOptionalDoor).ToList();
            foreach (var _ in leaves) PlaceAny(layout, taken, reachable, ItemKind.KeyOptional, 1, rng);

            // Tutorial depot (room 2) holds 1 heal + 1 blister on the first attempt, inside the floor 1 budget (D-70).
            int heals = p.Heals.Pick(rng);
            int b = p.BlisterRounds;
            int blisters = Math.Max(0, (p.Ammo.min + b * rng.Next(0, (p.Ammo.max - p.Ammo.min) / b + 1)) / b);
            int spikes = p.Spikes.Pick(rng);
            var depot = layout.Rooms.FirstOrDefault(r => r.TutorialIndex == 2);
            if (depot != null)
            {
                if (heals > 0 && Place(layout, taken, depot, ItemSpawnTag.Budget, ItemKind.Heal, 1, ItemSpawnTag.Budget, rng)) heals--;
                if (blisters > 0 && Place(layout, taken, depot, ItemSpawnTag.Budget, ItemKind.Ammo, b, ItemSpawnTag.Budget, rng)) blisters--;
            }

            // Guaranteed minimum first, only in rooms reachable without optional keys (D-79); the rest anywhere.
            var minHeals = Math.Max(0, p.Heals.min - (p.Heals.min > 0 && depot != null ? 1 : 0));
            var minBlisters = Math.Max(0, p.Ammo.min / b - (depot != null ? 1 : 0));
            var minSpikes = p.Spikes.min;
            for (int i = 0; i < heals; i++) PlaceAny(layout, taken, i < minHeals ? reachable : layout.Rooms, ItemKind.Heal, 1, rng);
            for (int i = 0; i < blisters; i++) PlaceAny(layout, taken, i < minBlisters ? reachable : layout.Rooms, ItemKind.Ammo, b, rng);
            for (int i = 0; i < spikes; i++) PlaceAny(layout, taken, i < minSpikes ? reachable : layout.Rooms, ItemKind.Spike, 1, rng);

            // Context documents and confiscated objects in procedural rooms (D-73, D-79).
            var proc = layout.Rooms.Where(r => r.Template.Role == RoomRole.Procedural).ToList();
            for (int i = 0, n = p.ContextDocs.Pick(rng); i < n; i++) PlaceTagged(layout, taken, proc, ItemSpawnTag.Document, rng);
            for (int i = 0, n = p.Confiscated.Pick(rng); i < n; i++) PlaceTagged(layout, taken, proc, ItemSpawnTag.Confiscated, rng);

            // Enemies: ~1/3 of procedural rooms are empty; max 2 per room (D-37); never in the entry or tutorial 2-3.
            var candidates = proc.Where(r => r.Index != layout.EntryRoom && (r.TutorialIndex == 0 || r.TutorialIndex == 4) && r.Template.EnemySpawns > 0)
                                 .OrderBy(_ => rng.Next()).ToList();
            int emptyCount = (int)Math.Round(candidates.Count * p.EmptyFraction);
            foreach (var r in candidates.Where(r => r.TutorialIndex == 0).Take(emptyCount)) r.Empty = true;

            int enemies = p.Enemies.Pick(rng);
            // D-118: never more than 2 per occupied room. The ~1/3 empty rooms are a target: if the occupied rooms cannot
            // hold the floor's minimum, empty rooms are re-occupied (if still short, the validator rejects → new seed).
            int Capacity() => candidates.Where(r => !r.Empty && r.TutorialIndex == 0).Sum(r => Math.Min(2, r.Template.EnemySpawns))
                              + (candidates.Any(r => r.TutorialIndex == 4) ? 1 : 0);
            foreach (var r in candidates.Where(r => r.Empty))
            {
                if (Capacity() >= enemies) break;
                r.Empty = false;
            }
            var tutorialBoiler = candidates.FirstOrDefault(r => r.TutorialIndex == 4);
            if (tutorialBoiler != null && enemies > 0)
            {
                layout.Enemies.Add(new EnemyPlacement { Room = tutorialBoiler.Index, Spawn = 0, Type = EnemyType.Rastrero });   // D-70: one real Rastrero
                enemies--;
            }
            var slots = new List<(int room, int spawn)>();
            foreach (var r in candidates.Where(r => !r.Empty && r.TutorialIndex == 0))
                for (int s = 0; s < Math.Min(2, r.Template.EnemySpawns); s++) slots.Add((r.Index, s));
            slots = slots.OrderBy(x => x.spawn).ThenBy(_ => rng.Next()).ToList();   // spread one per room before doubling up
            for (int i = 0; i < enemies && i < slots.Count; i++)
                layout.Enemies.Add(new EnemyPlacement { Room = slots[i].room, Spawn = slots[i].spawn, Type = p.EnemyTypes[rng.Next(p.EnemyTypes.Length)] });

            // Cameras (D-25): every key room and Surveillance room, then random procedural rooms up to a third.
            foreach (var r in layout.Rooms) r.HasCamera = r.Template.Role != RoomRole.Procedural || r.Template.Archetype == RoomArchetype.Surveillance;
            int cameras = proc.Count(r => r.HasCamera), cameraTarget = LayoutValidator.CameraTarget(layout);
            foreach (var r in proc.Where(r => !r.HasCamera).OrderBy(_ => rng.Next()))
            {
                if (cameras >= cameraTarget) break;
                r.HasCamera = true;
                cameras++;
            }
        }

        private static bool Place(FloorLayout layout, HashSet<(int, int)> taken, PlacedRoom room, ItemSpawnTag preferred, ItemKind kind, int amount, ItemSpawnTag result, Random rng)
        {
            var spawns = room.Template.ItemSpawns;
            var order = Enumerable.Range(0, spawns.Count).Where(i => !taken.Contains((room.Index, i)))
                .OrderBy(i => spawns[i] == preferred ? 0 : spawns[i] == ItemSpawnTag.Budget ? 1 : 2).ThenBy(_ => rng.Next()).ToList();
            if (order.Count == 0) return false;
            int s = order[0];
            taken.Add((room.Index, s));
            layout.Items.Add(new ItemPlacement { Room = room.Index, Spawn = s, Kind = kind, Amount = amount, Tag = result });
            return true;
        }

        private static void PlaceAny(FloorLayout layout, HashSet<(int, int)> taken, IList<PlacedRoom> rooms, ItemKind kind, int amount, Random rng)
        {
            foreach (var r in rooms.Where(r => r.Template.Role != RoomRole.CapsuleRow).OrderBy(_ => rng.Next()))
            {
                bool free = Enumerable.Range(0, r.Template.ItemSpawns.Count).Any(i => !taken.Contains((r.Index, i)) && r.Template.ItemSpawns[i] == ItemSpawnTag.Budget);
                if (free && Place(layout, taken, r, ItemSpawnTag.Budget, kind, amount, ItemSpawnTag.Budget, rng)) return;
            }
        }

        private static void PlaceTagged(FloorLayout layout, HashSet<(int, int)> taken, IList<PlacedRoom> rooms, ItemSpawnTag tag, Random rng)
        {
            foreach (var r in rooms.OrderBy(_ => rng.Next()))
            {
                bool has = Enumerable.Range(0, r.Template.ItemSpawns.Count).Any(i => !taken.Contains((r.Index, i)) && r.Template.ItemSpawns[i] == tag);
                if (has && Place(layout, taken, r, tag, ItemKind.None, 0, tag, rng)) return;
            }
        }
    }
}
