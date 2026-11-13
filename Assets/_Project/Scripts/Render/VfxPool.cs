using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Render
{
    /// <summary>
    /// Reusable pool for one-shot visual effects (C-71). Each prefab keeps up to <c>cap</c> instances; once full, the
    /// oldest is recycled. Every spawn restarts the instance's <see cref="VfxLifetime"/>, which plays its particles, fades
    /// its plain renderers and hides it when the lifetime ends. Instances whose parent was destroyed are rebuilt.
    /// </summary>
    public sealed class VfxPool
    {
        private sealed class Ring
        {
            public readonly List<VfxLifetime> items = new();
            public int next;
        }

        private readonly Transform root;
        private readonly Dictionary<GameObject, Ring> rings = new();

        /// <param name="root">Parent of instances spawned without their own parent.</param>
        public VfxPool(Transform root) => this.root = root;

        public VfxLifetime Spawn(GameObject prefab, int cap, Vector3 position, Quaternion rotation, float lifetime,
            Transform parent = null)
        {
            if (prefab == null || cap <= 0) return null;
            if (!rings.TryGetValue(prefab, out var ring)) rings[prefab] = ring = new Ring();
            VfxLifetime fx;
            if (ring.items.Count < cap)
            {
                fx = Create(prefab);
                ring.items.Add(fx);
            }
            else
            {
                int i = ring.next;
                ring.next = (ring.next + 1) % cap;
                fx = ring.items[i];
                if (fx == null) ring.items[i] = fx = Create(prefab);   // its parent (a muzzle, a room) was destroyed
            }
            fx.transform.SetParent(parent != null ? parent : root, false);
            fx.transform.SetPositionAndRotation(position, rotation);
            fx.Restart(lifetime);
            return fx;
        }

        /// <summary>Hides every instance, for example when the floor is rebuilt and the walls they sat on are gone.</summary>
        public void Clear()
        {
            foreach (var ring in rings.Values)
                foreach (var fx in ring.items)
                    if (fx != null) fx.gameObject.SetActive(false);
        }

        public int ActiveCount(GameObject prefab)
        {
            if (!rings.TryGetValue(prefab, out var ring)) return 0;
            int n = 0;
            foreach (var fx in ring.items) if (fx != null && fx.gameObject.activeSelf) n++;
            return n;
        }

        private VfxLifetime Create(GameObject prefab)
        {
            var go = Object.Instantiate(prefab, root);
            go.SetActive(false);
            return go.TryGetComponent(out VfxLifetime fx) ? fx : go.AddComponent<VfxLifetime>();
        }
    }
}
