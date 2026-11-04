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
        [Tooltip("Room lights allowed at once; the flashlight and Lukas's rim light make 16 (budget).")]
        [SerializeField] private int maxRoomLights = 14;
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

        private RoomInstance RoomAt(Vector3 p)
        {
            foreach (var r in rooms)
                if (r != null && r.TryGetComponent(out Collider c))
                {
                    var b = c.bounds;
                    if (p.x >= b.min.x && p.x <= b.max.x && p.z >= b.min.z && p.z <= b.max.z) return r;
                }
            return null;
        }

        public void Apply(RoomInstance room)
        {
            // Ignore rooms of a floor being torn down (their triggers can still fire during a rebuild).
            if (room == null || !rooms.Contains(room)) return;
            current = room;
            int liveEnemies = 0;
            bool LiveRoom(RoomInstance r) => r == room || room.Neighbours.Contains(r);
            var candidates = new List<(Light light, float priority)>();
            Vector3 centre = room.transform.position;
            foreach (var r in rooms)
            {
                bool live = LiveRoom(r);
                float d = r == room ? -1f : Vector3.Distance(centre, r.transform.position);
                foreach (var l in r.GetComponentsInChildren<Light>(true))
                {
                    l.enabled = false;
                    if (live && r.IsPowered) candidates.Add((l, d));
                }
            }

            // Enemies by where they are now, not the room they spawned in: a pursuer that followed Lukas used to
            // vanish next to him (and reappear when he stepped back). Searching/hunting enemies are never switched off.
            foreach (var list in enemies.Values)
                foreach (var e in list)
                {
                    if (e == null) continue;
                    var brain = e.GetComponent<AI.EnemyBrain>();
                    bool pursuing = brain != null && brain.Alert >= AlertState.Search;
                    var at = RoomAt(e.transform.position);
                    bool on = pursuing || at == null || LiveRoom(at);
                    e.SetActive(on);
                    if (on) liveEnemies++;
                }

            LitCount = ShadowedCount = 0;
            foreach (var c in candidates.OrderBy(c => c.priority))
            {
                // A light without shadows crosses walls and lights the wrong room, so past the shadow budget a light stays off.
                if (LitCount >= maxRoomLights || ShadowedCount >= maxShadowedRoomLights) break;
                c.light.enabled = true;
                LitCount++;
                c.light.shadows = LightShadows.Hard;
                ShadowedCount++;
            }
            HbLog.Info("Rooms", $"Active around {room.name}: {liveEnemies} enemies, lights={LitCount} shadowed={ShadowedCount}{(room.IsPowered ? "" : " (no power)")}");
        }
    }
}
