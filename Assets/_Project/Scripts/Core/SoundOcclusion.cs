using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>Counts the walls and closed doors on the straight line between two points (feeds <see cref="OcclusionModel"/>).</summary>
    public static class SoundOcclusion
    {
        private static readonly RaycastHit[] hits = new RaycastHit[16];
        private static readonly HashSet<ISoundOccluder> seenDoors = new HashSet<ISoundOccluder>();
        private static int mask = -1;

        public static void Count(Vector3 from, Vector3 to, out int walls, out int doors)
        {
            walls = doors = 0;
            if (mask < 0) mask = LayerMask.GetMask("Environment", "Interactable");
            Vector3 d = to - from;
            float len = d.magnitude;
            if (len < 0.01f) return;
            int n = Physics.RaycastNonAlloc(from, d / len, hits, len, mask, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, 0, n, HitByDistance.Instance);
            seenDoors.Clear();
            float lastWall = float.NegativeInfinity;
            for (int i = 0; i < n; i++)
            {
                var o = hits[i].collider.GetComponentInParent<ISoundOccluder>();
                if (o == null || !o.Occludes) continue;
                if (o.IsDoor) { if (seenDoors.Add(o)) doors++; continue; }
                if (hits[i].distance - lastWall > OcclusionModel.SameWallGap) walls++;
                lastWall = hits[i].distance;
            }
        }

        private sealed class HitByDistance : IComparer<RaycastHit>
        {
            public static readonly HitByDistance Instance = new HitByDistance();
            public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
        }
    }
}
