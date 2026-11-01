using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>The 6 playable archetypes (GDD 04 - Contenido/02 - Niveles, D-69). Key rooms use None.</summary>
    public enum RoomArchetype { None, Corridor, Storage, Open, Surveillance, Hack, Vertical }

    /// <summary>Role of a room in a floor (S-06, D-33).</summary>
    public enum RoomRole { Procedural, Secondary, Final, CapsuleRow, Tutorial, Mother }

    /// <summary>Socket direction pointing OUT of the room, in room-local axes (KIT_SPEC §4).</summary>
    public enum Facing { North, East, South, West }

    [Serializable]
    public struct SocketData
    {
        public Vector2Int cell;
        public Facing facing;
        public bool allowLocked;
    }

    /// <summary>
    /// Metadata of one hand-built room prefab, filled by the "Heartbeat/Rooms/Refresh Definitions" editor menu from
    /// the prefab itself (sockets, size, spawns). The generator only reads these assets (pure data, testable).
    /// </summary>
    [CreateAssetMenu(menuName = "Heartbeat/Room Definition", fileName = "RD_")]
    public sealed class RoomDefinition : ScriptableObject
    {
        public string id;
        public GameObject prefab;
        [Range(1, 3)] public int floor = 1;
        public RoomArchetype archetype;
        public RoomRole role;
        [Tooltip("Tutorial order 1-4 (D-70); 0 otherwise.")] public int tutorialIndex;
        [Tooltip("Size in 2 m cells.")] public Vector2Int size = new Vector2Int(4, 4);
        public List<SocketData> sockets = new List<SocketData>();
        public bool hasCamera;
        [Tooltip("Final rooms of floors 1-2 may start without power (D-72).")] public bool canBeUnpowered;
        public int enemySpawns;
        public int itemSpawns;
        [Tooltip("Tag of each item spawn, in hierarchy order (the builder maps index -> SpawnPoint).")]
        public List<ItemSpawnTag> itemSpawnTags = new List<ItemSpawnTag>();
        [Tooltip("Weight when picking procedural rooms.")] public float weight = 1f;
    }
}
