using System.Collections.Generic;
using System.Linq;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Only the current room and its neighbours are live (S-06, performance budget: ≤ 6 active AI, ≤ 16 lights,
    /// ≤ 4 shadowed, 12-art-render.md): enemies and lights of other rooms are switched off. Lights are lit nearest
    /// first (current room, then neighbours by distance) up to the budget, and only the current room's lights cast
    /// (hard) shadows. Unpowered rooms stay dark until PowerRestored (D-72). Enemy state survives because only the
    /// GameObjects are toggled.
    /// </summary>
    public sealed class RoomActivation : MonoBehaviour
    {
        [Tooltip("Room lights allowed at once; one more is left for Lukas's flashlight (budget 16).")]
        [SerializeField] private int maxRoomLights = 15;
        [Tooltip("Room lights with shadows; the flashlight is the 4th shadowed light (budget 4).")]
        [SerializeField] private int maxShadowedRoomLights = 3;

        private readonly List<RoomInstance> rooms = new List<RoomInstance>();
        private readonly Dictionary<RoomInstance, List<GameObject>> enemies = new Dictionary<RoomInstance, List<GameObject>>();
        private RoomInstance current;

        public int LitCount { get; private set; }
        public int ShadowedCount { get; private set; }

        public void Register(RoomInstance room, GameObject enemy)
        {
            if (!enemies.TryGetValue(room, out var list)) enemies[room] = list = new List<GameObject>();
            list.Add(enemy);
        }

        public void SetRooms(IEnumerable<RoomInstance> all) { rooms.Clear(); rooms.AddRange(all); }

        private void OnEnable() { GameEvents.RoomEntered += OnRoomEntered; GameEvents.PowerRestored += OnPowerRestored; }
        private void OnDisable() { GameEvents.RoomEntered -= OnRoomEntered; GameEvents.PowerRestored -= OnPowerRestored; }

        private void OnRoomEntered(MonoBehaviour m) => Apply(m as RoomInstance);

        /// <summary>The floor has one power switch; it powers the rooms that were dark (D-72).</summary>
        private void OnPowerRestored()
        {
            foreach (var r in rooms) r.IsPowered = true;
            Apply(current);
        }

        public void Apply(RoomInstance room)
        {
            // Ignore rooms of a floor being torn down (their triggers can still fire during a rebuild).
            if (room == null || !rooms.Contains(room)) return;
            current = room;
            int liveEnemies = 0;
            var candidates = new List<(Light light, float priority, bool own)>();
            Vector3 centre = room.transform.position;
            foreach (var r in rooms)
            {
                bool live = r == room || room.Neighbours.Contains(r);
                float d = r == room ? -1f : Vector3.Distance(centre, r.transform.position);
                foreach (var l in r.GetComponentsInChildren<Light>(true))
                {
                    l.enabled = false;
                    if (live && r.IsPowered) candidates.Add((l, d, r == room));
                }
                if (enemies.TryGetValue(r, out var list))
                    foreach (var e in list) if (e != null) { e.SetActive(live); if (live) liveEnemies++; }
            }

            LitCount = ShadowedCount = 0;
            foreach (var c in candidates.OrderBy(c => c.priority))
            {
                if (LitCount >= maxRoomLights) break;
                c.light.enabled = true;
                LitCount++;
                bool shadow = c.own && ShadowedCount < maxShadowedRoomLights;
                c.light.shadows = shadow ? LightShadows.Hard : LightShadows.None;
                if (shadow) ShadowedCount++;
            }
            HbLog.Info("Rooms", $"Active around {room.name}: {liveEnemies} enemies, lights={LitCount} shadowed={ShadowedCount}{(room.IsPowered ? "" : " (no power)")}");
        }
    }
}
