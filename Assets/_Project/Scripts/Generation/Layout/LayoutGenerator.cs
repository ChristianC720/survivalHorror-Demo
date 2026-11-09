using System;
using System.Collections.Generic;
using System.Linq;
using Heartbeat.Level;
using UnityEngine;
using Random = System.Random;

namespace Heartbeat.Generation.Layout
{
    /// <summary>
    /// Pure, seeded floor generator (S-06). Grows a graph of hand-built rooms on a cell grid by matching door sockets,
    /// then validates it and distributes the budget. Same seed + same params + same templates = same floor.
    /// Invalid results are retried with seed+1 (up to <see cref="MaxAttempts"/>).
    /// </summary>
    public static class LayoutGenerator
    {
        public const int MaxAttempts = 20;
        private const int PlaceTries = 60;

        public static FloorLayout Generate(FloorGenParams p, IReadOnlyList<RoomTemplate> templates)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                int seed = p.Seed + attempt;
                var layout = TryGenerate(p, templates, seed);
                if (layout == null) continue;
                BudgetDistributor.Distribute(layout, p, new Random(seed * 7919 + 13));
                if (LayoutValidator.Validate(layout, p, out _))
                {
                    layout.Attempts = attempt + 1;
                    return layout;
                }
            }
            return null;
        }

        /// <summary>Graph growth only (no budget). Returns null when the layout cannot be completed.</summary>
        public static FloorLayout TryGenerate(FloorGenParams p, IReadOnlyList<RoomTemplate> templates, int seed)
        {
            var rng = new Random(seed);
            var layout = new FloorLayout { Floor = p.Floor, Seed = seed };
            var occupied = new Dictionary<Vector2Int, int>();

            var procedural = templates.Where(t => t.Role == RoomRole.Procedural).ToList();
            var secondaries = templates.Where(t => t.Role == RoomRole.Secondary).ToList();
            var finals = templates.Where(t => t.Role == RoomRole.Final).ToList();
            if (procedural.Count == 0 || secondaries.Count == 0 || finals.Count == 0) return null;

            // 1. Entry: the capsule row on floor 1, the dead elevator's room below it (D-158).
            var entryRole = p.Floor == 1 ? RoomRole.CapsuleRow : RoomRole.Arrival;
            var entryT = templates.FirstOrDefault(t => t.Role == entryRole);
            entryT ??= Pick(procedural, rng, null);
            Add(layout, occupied, new PlacedRoom(entryT) { Origin = Vector2Int.zero, Rotation = 0, Depth = 0 });
            layout.EntryRoom = 0;

            // 2. Fixed tutorial chain on the first attempt (D-70).
            if (p.FirstAttempt)
            {
                for (int i = 0; i < p.TutorialRoomIds.Length; i++)
                {
                    var t = templates.FirstOrDefault(x => x.Id == p.TutorialRoomIds[i]);
                    if (t == null) return null;
                    var last = layout.Rooms[layout.Rooms.Count - 1];
                    if (!AttachFrom(layout, occupied, last, t, rng)) return null;
                    layout.Rooms[layout.Rooms.Count - 1].TutorialIndex = i + 2;
                }
            }

            // 3. Procedural rooms.
            int target = p.Procedural.Pick(rng) - (p.FirstAttempt ? p.TutorialRoomIds.Length : 0);   // D-70: the tutorial rooms count inside the range
            int placed = 0, guard = 0;
            while (placed < target && guard++ < target * PlaceTries)
            {
                var parent = PickOpenRoom(layout, rng, minDepth: 0);
                if (parent == null) return null;
                var t = Pick(procedural, rng, parent.Template.Archetype);
                if (AttachFrom(layout, occupied, parent, t, rng)) placed++;
            }
            if (placed < target) return null;

            // 4. Secondary key rooms at depth >= MinSecondaryDistance (leaves; the validator re-checks after loops).
            int secCount = Math.Min(p.Secondary.Pick(rng), Math.Max(2, secondaries.Count * 3));
            for (int i = 0; i < secCount; i++)
            {
                bool ok = false;
                for (int tries = 0; tries < PlaceTries && !ok; tries++)
                {
                    var parent = PickOpenRoom(layout, rng, minDepth: Math.Max(0, p.MinSecondaryDistance - 1));
                    if (parent == null) break;
                    ok = AttachFrom(layout, occupied, parent, secondaries[rng.Next(secondaries.Count)], rng, leaf: true);
                }
                if (!ok) return null;
            }

            // 5. Final room at the deepest open socket.
            var final = finals[rng.Next(finals.Count)];
            bool finalOk = false;
            foreach (var parent in layout.Rooms.Where(r => r.Template.Role == RoomRole.Procedural && HasOpenSocket(r))
                         .OrderByDescending(r => r.Depth).ThenBy(_ => rng.Next()))
            {
                if (AttachFrom(layout, occupied, parent, final, rng, leaf: true)) { finalOk = true; break; }
            }
            if (!finalOk) return null;
            layout.FinalRoom = layout.Rooms.Count - 1;
            layout.Connections[layout.Connections.Count - 1].Lock = DoorLock.MandatoryKey;

            // 6. Loops between facing open sockets (never into key rooms).
            AddLoops(layout, p.LoopChance, rng);
            return layout;
        }

        // ---------------------------------------------------------------------------------------------------

        private static RoomTemplate Pick(List<RoomTemplate> pool, Random rng, RoomArchetype? avoid)
        {
            float total = 0f;
            foreach (var t in pool) total += W(t, avoid);
            double r = rng.NextDouble() * total;
            foreach (var t in pool)
            {
                r -= W(t, avoid);
                if (r <= 0) return t;
            }
            return pool[pool.Count - 1];
        }

        private static float W(RoomTemplate t, RoomArchetype? avoid) => t.Weight * (avoid.HasValue && t.Archetype == avoid.Value ? 0.3f : 1f);

        private static bool HasOpenSocket(PlacedRoom r)
        {
            foreach (var u in r.SocketUsed) if (!u) return true;
            return false;
        }

        private static PlacedRoom PickOpenRoom(FloorLayout layout, Random rng, int minDepth)
        {
            var open = layout.Rooms.Where(r => r.Depth >= minDepth && HasOpenSocket(r)
                                               && r.Template.Role != RoomRole.Secondary && r.Template.Role != RoomRole.Final).ToList();
            if (open.Count == 0) return null;
            // Slight bias towards the frontier so floors branch instead of forming a single line.
            return rng.NextDouble() < 0.6 ? open[rng.Next(open.Count)] : open.OrderByDescending(r => r.Depth).First();
        }

        private static bool AttachFrom(FloorLayout layout, Dictionary<Vector2Int, int> occupied, PlacedRoom parent, RoomTemplate t, Random rng, bool leaf = false)
        {
            var parentSockets = Enumerable.Range(0, parent.SocketUsed.Length).Where(i => !parent.SocketUsed[i]).OrderBy(_ => rng.Next()).ToList();
            foreach (int ps in parentSockets)
            {
                Vector2Int target = parent.SocketOutside(ps);
                Facing need = GridMath.Opposite(parent.SocketFacing(ps));
                foreach (int rot in Enumerable.Range(0, 4).OrderBy(_ => rng.Next()))
                {
                    foreach (int cs in Enumerable.Range(0, t.Sockets.Count).OrderBy(_ => rng.Next()))
                    {
                        if (GridMath.Rotate(t.Sockets[cs].facing, rot) != need) continue;
                        Vector2Int origin = target - GridMath.RotateCell(t.Sockets[cs].cell, t.Size, rot);
                        var room = new PlacedRoom(t) { Origin = origin, Rotation = rot, Depth = parent.Depth + 1 };
                        if (!Fits(room, occupied)) continue;
                        Add(layout, occupied, room);
                        parent.SocketUsed[ps] = true;
                        room.SocketUsed[cs] = true;
                        layout.Connections.Add(new Connection { A = parent.Index, SocketA = ps, B = room.Index, SocketB = cs });
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool Fits(PlacedRoom r, Dictionary<Vector2Int, int> occupied)
        {
            var s = r.Size;
            for (int x = 0; x < s.x; x++)
                for (int y = 0; y < s.y; y++)
                    if (occupied.ContainsKey(r.Origin + new Vector2Int(x, y))) return false;
            return true;
        }

        private static void Add(FloorLayout layout, Dictionary<Vector2Int, int> occupied, PlacedRoom r)
        {
            r.Index = layout.Rooms.Count;
            layout.Rooms.Add(r);
            var s = r.Size;
            for (int x = 0; x < s.x; x++)
                for (int y = 0; y < s.y; y++)
                    occupied[r.Origin + new Vector2Int(x, y)] = r.Index;
        }

        private static void AddLoops(FloorLayout layout, float chance, Random rng)
        {
            var open = new List<(PlacedRoom room, int s)>();
            foreach (var r in layout.Rooms)
            {
                if (r.Template.Role == RoomRole.Final || r.Template.Role == RoomRole.Secondary) continue;
                for (int i = 0; i < r.SocketUsed.Length; i++) if (!r.SocketUsed[i]) open.Add((r, i));
            }
            for (int i = 0; i < open.Count; i++)
                for (int j = i + 1; j < open.Count; j++)
                {
                    var (ra, sa) = open[i];
                    var (rb, sb) = open[j];
                    if (ra == rb || ra.SocketUsed[sa] || rb.SocketUsed[sb]) continue;
                    if (ra.SocketOutside(sa) != rb.SocketCell(sb) || rb.SocketOutside(sb) != ra.SocketCell(sa)) continue;
                    if (rng.NextDouble() > chance) continue;
                    ra.SocketUsed[sa] = rb.SocketUsed[sb] = true;
                    layout.Connections.Add(new Connection { A = ra.Index, SocketA = sa, B = rb.Index, SocketB = sb });
                }
        }
    }
}
