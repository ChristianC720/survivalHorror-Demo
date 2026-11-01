using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Generation.Layout
{
    /// <summary>Pure description of a room for the generator (built from a RoomDefinition, or by hand in tests).</summary>
    public sealed class RoomTemplate
    {
        public string Id;
        public RoomRole Role;
        public RoomArchetype Archetype;
        public Vector2Int Size;
        public List<SocketData> Sockets = new List<SocketData>();
        public bool HasCamera;
        public bool CanBeUnpowered;
        public int EnemySpawns;
        public List<ItemSpawnTag> ItemSpawns = new List<ItemSpawnTag>();
        public int TutorialIndex;
        public float Weight = 1f;
        /// <summary>Back-reference for the runtime builder (null in tests).</summary>
        public RoomDefinition Definition;

        public static RoomTemplate From(RoomDefinition d) => new RoomTemplate
        {
            Id = d.id,
            Role = d.role,
            Archetype = d.archetype,
            Size = d.size,
            Sockets = new List<SocketData>(d.sockets),
            HasCamera = d.hasCamera,
            CanBeUnpowered = d.canBeUnpowered,
            EnemySpawns = d.enemySpawns,
            ItemSpawns = new List<ItemSpawnTag>(d.itemSpawnTags),
            TutorialIndex = d.tutorialIndex,
            Weight = d.weight,
            Definition = d,
        };
    }

    /// <summary>Grid maths for quarter-turn rotations (clockwise seen from above, i.e. Unity yaw 90·r).</summary>
    public static class GridMath
    {
        public static Vector2Int Dir(Facing f) => f == Facing.North ? Vector2Int.up : f == Facing.South ? Vector2Int.down : f == Facing.East ? Vector2Int.right : Vector2Int.left;

        public static Facing Rotate(Facing f, int r) => (Facing)(((int)f + r) % 4);

        public static Facing Opposite(Facing f) => (Facing)(((int)f + 2) % 4);

        public static Vector2Int RotatedSize(Vector2Int size, int r) => r % 2 == 0 ? size : new Vector2Int(size.y, size.x);

        /// <summary>Local cell → cell inside the rotated footprint (origin at the rotated footprint's min corner).</summary>
        public static Vector2Int RotateCell(Vector2Int c, Vector2Int size, int r)
        {
            switch (r & 3)
            {
                case 0: return c;
                case 1: return new Vector2Int(c.y, size.x - 1 - c.x);
                case 2: return new Vector2Int(size.x - 1 - c.x, size.y - 1 - c.y);
                default: return new Vector2Int(size.y - 1 - c.y, c.x);
            }
        }
    }
}
