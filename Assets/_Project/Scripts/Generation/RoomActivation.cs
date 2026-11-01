using System.Collections.Generic;
using System.Linq;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Only the current room and its neighbours are live (S-06, performance budget: ≤ 6 active AI, ≤ 16 lights):
    /// enemies and lights of other rooms are switched off. Enemy state survives because only the GameObjects are toggled.
    /// </summary>
    public sealed class RoomActivation : MonoBehaviour
    {
        private readonly List<RoomInstance> rooms = new List<RoomInstance>();
        private readonly Dictionary<RoomInstance, List<GameObject>> enemies = new Dictionary<RoomInstance, List<GameObject>>();

        public void Register(RoomInstance room, GameObject enemy)
        {
            if (!enemies.TryGetValue(room, out var list)) enemies[room] = list = new List<GameObject>();
            list.Add(enemy);
        }

        public void SetRooms(IEnumerable<RoomInstance> all) { rooms.Clear(); rooms.AddRange(all); }

        private void OnEnable() => GameEvents.RoomEntered += OnRoomEntered;
        private void OnDisable() => GameEvents.RoomEntered -= OnRoomEntered;

        private void OnRoomEntered(MonoBehaviour m) => Apply(m as RoomInstance);

        public void Apply(RoomInstance current)
        {
            if (current == null) return;
            int liveEnemies = 0, liveLights = 0;
            foreach (var r in rooms)
            {
                bool live = r == current || current.Neighbours.Contains(r);
                foreach (var l in r.GetComponentsInChildren<Light>(true)) { l.enabled = live; if (live) liveLights++; }
                if (enemies.TryGetValue(r, out var list))
                    foreach (var e in list) if (e != null) { e.SetActive(live); if (live) liveEnemies++; }
            }
            HbLog.Dev("Rooms", $"Active around {current.name}: {liveEnemies} enemies, {liveLights} lights");
        }
    }
}
