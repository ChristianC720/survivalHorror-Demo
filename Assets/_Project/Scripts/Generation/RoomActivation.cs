using System.Collections.Generic;
using System.Linq;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Only the current room and its neighbours are lit (S-06, performance budget: ≤ 16 lights, ≤ 4 shadowed,
    /// 12-art-render.md). Lights are lit nearest first (current room, then neighbours by distance) up to the budget.
    /// Unpowered rooms stay dark until PowerRestored (D-72). Rendering is cut to what the player can see: rooms more
    /// than <see cref="visibleDepth"/> doors away, and the enemies standing in them, are not drawn. Enemies are never
    /// switched off, so they keep patrolling and hunting out of sight (owner request 2026-11-05).
    /// </summary>
    public sealed class RoomActivation : MonoBehaviour
    {
        [Tooltip("Room lights allowed at once; the flashlight and Lukas's rim light make 16 (budget).")]
        [SerializeField] private int maxRoomLights = 14;
        [Tooltip("Room lights with shadows; the flashlight is the 4th shadowed light (budget 4).")]
        [SerializeField] private int maxShadowedRoomLights = 3;
        [Tooltip("Rooms drawn, in doors from the current one: 2 keeps the dark room behind a neighbour's doorway visible.")]
        [SerializeField] private int visibleDepth = 2;
        [Tooltip("Seconds between checks of which enemies stand in a drawn room.")]
        [SerializeField] private float enemyCheckInterval = 0.2f;

        private readonly List<RoomInstance> rooms = new List<RoomInstance>();
        private readonly Dictionary<RoomInstance, List<GameObject>> enemies = new Dictionary<RoomInstance, List<GameObject>>();
        private readonly Dictionary<GameObject, Renderer[]> renderers = new Dictionary<GameObject, Renderer[]>();
        private readonly HashSet<RoomInstance> visible = new HashSet<RoomInstance>();
        private RoomInstance current;
        private float nextEnemyCheck;

        public int LitCount { get; private set; }
        public int ShadowedCount { get; private set; }

        public void Register(RoomInstance room, GameObject enemy)
        {
            if (!enemies.TryGetValue(room, out var list)) enemies[room] = list = new List<GameObject>();
            list.Add(enemy);
        }

        public void SetRooms(IEnumerable<RoomInstance> all) { rooms.Clear(); rooms.AddRange(all); renderers.Clear(); visible.Clear(); }

        private void OnEnable()
        {
            GameEvents.RoomEntered += OnRoomEntered;
            GameEvents.PowerRestored += OnPowerRestored;
            GameEvents.OptionsChanged += ReadShadowQuality;
            ReadShadowQuality();
        }

        private void OnDisable()
        {
            GameEvents.RoomEntered -= OnRoomEntered;
            GameEvents.PowerRestored -= OnPowerRestored;
            GameEvents.OptionsChanged -= ReadShadowQuality;
        }

        // Options → Video: High is the whole budget; each step down costs one shadowed light. Applies from the next room.
        private int shadowBudget;

        private void ReadShadowQuality()
        {
            int quality = Services.TryGet(out Save.SaveService saves) ? saves.LoadGlobal().shadowQuality : 2;
            shadowBudget = Mathf.Max(1, maxShadowedRoomLights - (2 - quality));
        }

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

            // Rooms past the visible depth are walls the player cannot see through: skip drawing them.
            visible.Clear();
            var frontier = new List<RoomInstance> { room };
            for (int depth = 0; depth <= visibleDepth && frontier.Count > 0; depth++)
            {
                var next = new List<RoomInstance>();
                foreach (var r in frontier)
                    if (r != null && visible.Add(r)) next.AddRange(r.Neighbours);
                frontier = next;
            }
            foreach (var r in rooms)
                if (r != null) SetDrawn(r.gameObject, visible.Contains(r));
            liveEnemies = UpdateEnemyVisibility();

            LitCount = ShadowedCount = 0;
            foreach (var c in candidates.OrderBy(c => c.priority))
            {
                // A light without shadows crosses walls and lights the wrong room, so past the shadow budget a light stays off.
                if (LitCount >= maxRoomLights || ShadowedCount >= shadowBudget) break;
                c.light.enabled = true;
                LitCount++;
                c.light.shadows = LightShadows.Hard;
                ShadowedCount++;
            }
            HbLog.Info("Rooms", $"Active around {room.name}: {liveEnemies} enemies, lights={LitCount} shadowed={ShadowedCount}{(room.IsPowered ? "" : " (no power)")}, drawn rooms={visible.Count}/{rooms.Count}");
        }

        private void Update()
        {
            // Enemies walk between rooms, so their visibility is rechecked on a timer rather than only on room changes.
            if (current == null || Time.time < nextEnemyCheck) return;
            nextEnemyCheck = Time.time + enemyCheckInterval;
            UpdateEnemyVisibility();
        }

        /// <summary>Draws the enemies standing in a drawn room (or outside every room); returns how many are drawn.</summary>
        private int UpdateEnemyVisibility()
        {
            int drawn = 0;
            foreach (var list in enemies.Values)
                foreach (var e in list)
                {
                    if (e == null) continue;
                    var at = RoomAt(e.transform.position);
                    bool on = at == null || visible.Contains(at);
                    SetDrawn(e, on);
                    if (on) drawn++;
                }
            return drawn;
        }

        // forceRenderingOff leaves Renderer.enabled to its owners (flicker, doors) and keeps the object fully alive.
        private void SetDrawn(GameObject go, bool on)
        {
            if (!renderers.TryGetValue(go, out var list)) renderers[go] = list = go.GetComponentsInChildren<Renderer>(true);
            foreach (var r in list)
                if (r != null) r.forceRenderingOff = !on;
        }
    }
}
