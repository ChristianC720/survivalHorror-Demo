using System;
using System.Collections.Generic;
using System.Linq;
using Heartbeat.Inventory;
using Heartbeat.Level;
using UnityEngine;

namespace Heartbeat.Generation.Layout
{
    /// <summary>
    /// Rules every generated floor must satisfy (S-06, GDD 02 - Niveles, 06 - Economía): no overlaps; every room
    /// reachable; exactly one final room behind a mandatory-key door; mandatory key, power switch, Emilia documents and
    /// optional keys reachable without the final door or any optional door; guaranteed resource minimum reachable
    /// without optional keys; room counts inside the configured ranges; max 2 enemies per room.
    /// </summary>
    public static class LayoutValidator
    {
        public static bool Validate(FloorLayout l, FloorGenParams p, out List<string> errors)
        {
            errors = new List<string>();
            if (l == null) { errors.Add("null layout"); return false; }

            // Overlaps
            var cells = new HashSet<Vector2Int>();
            foreach (var r in l.Rooms)
                for (int x = 0; x < r.Size.x; x++)
                    for (int y = 0; y < r.Size.y; y++)
                        if (!cells.Add(r.Origin + new Vector2Int(x, y))) errors.Add($"overlap at room {r.Index}");

            // Connection consistency: sockets face each other on adjacent cells
            foreach (var c in l.Connections)
            {
                var a = l.Rooms[c.A]; var b = l.Rooms[c.B];
                if (a.SocketOutside(c.SocketA) != b.SocketCell(c.SocketB) || b.SocketOutside(c.SocketB) != a.SocketCell(c.SocketA))
                    errors.Add($"connection {c.A}-{c.B} sockets do not meet");
            }

            // Counts
            int proc = l.Rooms.Count(r => r.Template.Role == RoomRole.Procedural);   // tutorial rooms included (D-70)
            int sec = l.Rooms.Count(r => r.Template.Role == RoomRole.Secondary);
            int fin = l.Rooms.Count(r => r.Template.Role == RoomRole.Final);
            if (proc < p.Procedural.min || proc > p.Procedural.max) errors.Add($"{proc} procedural rooms outside {p.Procedural.min}-{p.Procedural.max}");
            if (sec < p.Secondary.min || sec > p.Secondary.max) errors.Add($"{sec} secondary rooms outside {p.Secondary.min}-{p.Secondary.max}");
            if (fin != 1 || l.FinalRoom < 0) errors.Add("needs exactly one final room");

            // Reachability
            var all = Reach(l, l.EntryRoom, c => true);
            if (all.Count != l.Rooms.Count) errors.Add($"{l.Rooms.Count - all.Count} rooms unreachable");

            var free = Reach(l, l.EntryRoom, c => c.Lock == DoorLock.None);
            if (l.FinalRoom >= 0)
            {
                var finalDoors = l.Connections.Where(c => c.A == l.FinalRoom || c.B == l.FinalRoom).ToList();
                if (finalDoors.Count != 1 || finalDoors[0].Lock != DoorLock.MandatoryKey) errors.Add("final room must have one mandatory-key door");
                if (free.Contains(l.FinalRoom)) errors.Add("final room reachable without the key");
            }
            if (l.KeyRoom < 0 || !free.Contains(l.KeyRoom)) errors.Add("mandatory key not reachable before the final door");
            if (!l.Items.Any(i => i.Kind == ItemKind.KeyMandatory)) errors.Add("mandatory key not placed");
            if (l.FinalUnpowered && (l.SwitchRoom < 0 || !free.Contains(l.SwitchRoom))) errors.Add("power switch not reachable");
            if (l.EmiliaRooms.Count != 2 || l.EmiliaRooms.Any(r => !free.Contains(r))) errors.Add("needs 2 reachable Emilia documents");
            if (l.Items.Count(i => i.Tag == ItemSpawnTag.Emilia) != 2) errors.Add("Emilia documents not placed");
            foreach (var k in l.Items.Where(i => i.Kind == ItemKind.KeyOptional))
                if (!free.Contains(k.Room)) errors.Add("optional key behind a locked door");
            int optionalDoors = l.Connections.Count(c => c.Lock == DoorLock.OptionalKey);
            if (l.Items.Count(i => i.Kind == ItemKind.KeyOptional) < optionalDoors) errors.Add("optional door without a key");

            // Guaranteed minimum reachable without optional keys (D-79)
            int Free(ItemKind k) => l.Items.Where(i => i.Kind == k && !l.Rooms[i.Room].BehindOptionalDoor).Sum(i => i.Amount);
            if (Free(ItemKind.Heal) < p.Heals.min) errors.Add($"heals {Free(ItemKind.Heal)} < min {p.Heals.min}");
            if (Free(ItemKind.Ammo) < p.Ammo.min) errors.Add($"ammo {Free(ItemKind.Ammo)} < min {p.Ammo.min}");
            if (Free(ItemKind.Spike) < p.Spikes.min) errors.Add($"spikes {Free(ItemKind.Spike)} < min {p.Spikes.min}");
            if (l.Count(ItemKind.Heal) > p.Heals.max || l.Count(ItemKind.Ammo) > p.Ammo.max || l.Count(ItemKind.Spike) > p.Spikes.max) errors.Add("budget over max");

            // Enemies
            if (l.Enemies.Count < p.Enemies.min || l.Enemies.Count > p.Enemies.max) errors.Add($"{l.Enemies.Count} enemies outside {p.Enemies.min}-{p.Enemies.max}");
            if (l.Enemies.GroupBy(e => e.Room).Any(g => g.Count() > 2)) errors.Add("more than 2 enemies in a room");
            if (l.Enemies.Any(e => e.Room == l.EntryRoom)) errors.Add("enemy in the entry room");

            // Secondary key rooms (documents) never near the spawn room (D-new): BFS distance, loops included.
            var dist = Distances(l, l.EntryRoom);
            if (l.FinalRoom >= 0 && dist.TryGetValue(l.FinalRoom, out int df) && df < p.MinFinalDistance) errors.Add($"final room only {df} doors from the entry (D-100)");
            // Cameras (D-25): every key room, at least a third of the procedural rooms.
            if (l.Rooms.Any(r => r.Template.Role != RoomRole.Procedural && !r.HasCamera)) errors.Add("key room without camera");
            if (l.Rooms.Count(r => r.Template.Role == RoomRole.Procedural && r.HasCamera) < CameraTarget(l)) errors.Add("fewer than a third of procedural rooms with camera");
            foreach (var r in l.Rooms.Where(r => r.Template.Role == RoomRole.Secondary))
                if (dist.TryGetValue(r.Index, out int d) && d < p.MinSecondaryDistance) errors.Add($"secondary room {r.Index} only {d} doors from the entry");

            return errors.Count == 0;
        }

        public static int CameraTarget(FloorLayout l) => (int)Math.Round(l.Rooms.Count(r => r.Template.Role == RoomRole.Procedural) / 3.0, MidpointRounding.AwayFromZero);

        public static Dictionary<int, int> Distances(FloorLayout l, int start)
        {
            var dist = new Dictionary<int, int> { [start] = 0 };
            var q = new Queue<int>();
            q.Enqueue(start);
            while (q.Count > 0)
            {
                int r = q.Dequeue();
                foreach (var c in l.Connections)
                {
                    int other = c.A == r ? c.B : c.B == r ? c.A : -1;
                    if (other >= 0 && !dist.ContainsKey(other)) { dist[other] = dist[r] + 1; q.Enqueue(other); }
                }
            }
            return dist;
        }

        public static HashSet<int> Reach(FloorLayout l, int start, System.Func<Connection, bool> passable)
        {
            var seen = new HashSet<int> { start };
            var q = new Queue<int>();
            q.Enqueue(start);
            while (q.Count > 0)
            {
                int r = q.Dequeue();
                foreach (var c in l.Connections)
                {
                    if (!passable(c)) continue;
                    int other = c.A == r ? c.B : c.B == r ? c.A : -1;
                    if (other >= 0 && seen.Add(other)) q.Enqueue(other);
                }
            }
            return seen;
        }
    }
}
