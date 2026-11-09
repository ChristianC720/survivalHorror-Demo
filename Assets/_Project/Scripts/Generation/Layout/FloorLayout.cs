using System.Collections.Generic;
using Heartbeat.AI;
using Heartbeat.Inventory;
using Heartbeat.Level;
using UnityEngine;

namespace Heartbeat.Generation.Layout
{
    public sealed class PlacedRoom
    {
        public int Index;
        public RoomTemplate Template;
        /// <summary>World cell of the rotated footprint's min corner.</summary>
        public Vector2Int Origin;
        /// <summary>Quarter turns (Unity yaw = 90·Rotation).</summary>
        public int Rotation;
        public int Depth;
        public bool Empty;
        /// <summary>2-4 for the fixed tutorial rooms of the first attempt (D-70), 0 otherwise.</summary>
        public int TutorialIndex;
        /// <summary>Behind a door that needs an optional key (D-79: no guaranteed minimum here).</summary>
        public bool BehindOptionalDoor;
        /// <summary>Surveillance camera (D-25): every key room and ~1/3 of the procedural rooms.</summary>
        public bool HasCamera;
        public readonly bool[] SocketUsed;

        public PlacedRoom(RoomTemplate t) { Template = t; SocketUsed = new bool[t.Sockets.Count]; }

        public Vector2Int Size => GridMath.RotatedSize(Template.Size, Rotation);

        public bool Contains(Vector2Int cell) =>
            cell.x >= Origin.x && cell.y >= Origin.y && cell.x < Origin.x + Size.x && cell.y < Origin.y + Size.y;

        public Vector2Int SocketCell(int i) => Origin + GridMath.RotateCell(Template.Sockets[i].cell, Template.Size, Rotation);
        public Facing SocketFacing(int i) => GridMath.Rotate(Template.Sockets[i].facing, Rotation);
        /// <summary>The cell just outside the socket.</summary>
        public Vector2Int SocketOutside(int i) => SocketCell(i) + GridMath.Dir(SocketFacing(i));
    }

    public sealed class Connection
    {
        public int A, SocketA, B, SocketB;
        public DoorLock Lock;
    }

    public sealed class ItemPlacement
    {
        public int Room, Spawn;
        public ItemKind Kind;          // None for documents / switch
        public int Amount;
        public ItemSpawnTag Tag;       // what the spawn ends up holding
    }

    public sealed class EnemyPlacement
    {
        public int Room, Spawn;
        public EnemyType Type;
    }

    /// <summary>Result of the generator: pure data the FloorBuilder turns into GameObjects.</summary>
    public sealed class FloorLayout
    {
        public int Floor;
        public int Seed;
        public int Attempts;
        public readonly List<PlacedRoom> Rooms = new List<PlacedRoom>();
        public readonly List<Connection> Connections = new List<Connection>();
        public readonly List<ItemPlacement> Items = new List<ItemPlacement>();
        public readonly List<EnemyPlacement> Enemies = new List<EnemyPlacement>();
        public int EntryRoom;
        public int FinalRoom = -1;
        public int KeyRoom = -1;
        public int SwitchRoom = -1;
        public bool FinalUnpowered;
        public readonly List<int> EmiliaRooms = new List<int>();

        public IEnumerable<int> Neighbours(int room)
        {
            foreach (var c in Connections)
            {
                if (c.A == room) yield return c.B;
                else if (c.B == room) yield return c.A;
            }
        }

        public int Count(ItemKind kind)
        {
            int n = 0;
            foreach (var i in Items) if (i.Kind == kind) n += i.Amount;
            return n;
        }
    }
}
