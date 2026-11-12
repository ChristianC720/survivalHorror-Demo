using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartbeat.Combat;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Rebuilds the shot hitboxes of every enemy from its skinned mesh (C-63). Each weighted bone gets one child
    /// collider, so the hitbox rides the animation. It wraps, 2 cm outward, the vertices that bone leads, in the bind
    /// pose and in 10 frames of every clip but death (skin drifts off its bone at the joints): the smallest box or
    /// capsule, or a convex hull of those vertices when the box or capsule would leave a corner more than 10 cm off
    /// the skin (a shoulder, a crawling head). Finger bones fold into their hand. The weak-point bones from the GDD get
    /// the <see cref="WeakPoint"/> marker on their own collider. Idempotent: old Hit_* / WeakPoint_* objects and the
    /// hull meshes (Hitboxes/HB_*.asset) are replaced. Checked by EnemyHitboxCoverageTests.
    /// </summary>
    public static class HitboxBuilder
    {
        public const string PrefabFolder = "Assets/_Project/Prefabs/Characters/";

        // GDD Enemigos: El Alto the bent head, La Boca the open mouth (its head splits in two halves around it),
        // El Rastrero the head with the hanging jaw, El Mudo the torso.
        public static readonly Dictionary<string, string[]> WeakBones = new()
        {
            { "Alto", new[] { "head", "jaw" } },
            { "Boca", new[] { "head_L", "head_R" } },
            { "Mudo", new[] { "chest" } },
            { "Rastrero", new[] { "Head" } },
        };

        private const float Margin = 0.02f;       // order C-63: 2 cm outward
        private const float MaxGap = 0.10f;       // a box or capsule straying farther than this from the skin becomes a hull
        private const float WeakExtra = 0.02f;    // weak points reach 2 cm past their neighbours, so they win the shared seams
        private const int FramesPerClip = 10;

        private static readonly string[] FingerWords = { "finger", "thumb", "pinky", "ring_", "middle_", "index" };

        [MenuItem("Heartbeat/Rebuild Enemy Hitboxes")]
        public static void RebuildMenu() => Debug.Log(RebuildAll());

        public static string RebuildAll()
        {
            var sb = new StringBuilder();
            foreach (var kv in WeakBones) sb.AppendLine(Rebuild(kv.Key, kv.Value));
            AssetDatabase.SaveAssets();
            return sb.ToString();
        }

        private static string Rebuild(string enemy, string[] weakBones)
        {
            string path = $"{PrefabFolder}Enemy_{enemy}.prefab";
            string hullPath = $"{PrefabFolder}Hitboxes/HB_{enemy}.asset";
            AssetDatabase.DeleteAsset(hullPath);
            if (!AssetDatabase.IsValidFolder(PrefabFolder + "Hitboxes")) AssetDatabase.CreateFolder(PrefabFolder.TrimEnd('/'), "Hitboxes");
            var hullStore = new Mesh { name = $"HB_{enemy}" };
            AssetDatabase.CreateAsset(hullStore, hullPath);
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                int layer = LayerMask.NameToLayer("Hitbox");
                foreach (var c in root.GetComponentsInChildren<Collider>(true).Where(c => c.gameObject != root).ToArray())
                    if (c.gameObject.layer == layer) Object.DestroyImmediate(c.gameObject);

                var smr = root.GetComponentInChildren<SkinnedMeshRenderer>(true);
                var mesh = smr.sharedMesh;
                var bones = smr.bones;
                var bind = mesh.bindposes;
                var verts = mesh.vertices;
                var weights = mesh.boneWeights;

                int[] owner = new int[bones.Length];
                for (int b = 0; b < bones.Length; b++) owner[b] = FoldFinger(b, bones);

                // Each vertex belongs to the bone that leads it, in that bone's space, in the bind pose and in every
                // sampled frame of every clip: a skin vertex drifts off its bone at the joints, and the hitbox has to
                // hold it wherever the animation takes it.
                var lead = weights.Select(w => owner[w.boneIndex0]).ToArray();
                int moved = AdoptStrays(lead, weights, owner, verts.Select((v, i) => bind[lead[i]].MultiplyPoint3x4(v)).ToArray());
                var frames = new List<Vector3[]> { verts.Select((v, i) => bind[lead[i]].MultiplyPoint3x4(v)).ToArray() };
                frames.AddRange(SamplePoses(root, smr, lead));

                var sb = new StringBuilder($"{enemy} ({frames.Count - 1} frames, {moved} stray vertices to their second bone):");
                int count = 0;
                for (int b = 0; b < bones.Length; b++)
                {
                    var mine = Enumerable.Range(0, verts.Length).Where(i => lead[i] == b).ToList();
                    if (mine.Count == 0) continue;
                    bool weak = weakBones.Contains(bones[b].name);
                    float margin = (Margin + (weak ? WeakExtra : 0f)) / Mathf.Max(1e-4f, bones[b].lossyScale.x);
                    var m = bind[b];
                    var pts = frames.SelectMany(f => mine.Select(i => f[i])).ToList();
                    var near = new Bounds(pts[0], Vector3.zero);
                    foreach (var p in pts) near.Encapsulate(p);
                    near.Expand(1f);
                    var skin = verts.Select(v => m.MultiplyPoint3x4(v)).Where(near.Contains).ToArray();
                    string name = (weak ? "WeakPoint_" : "Hit_") + bones[b].name;
                    var go = new GameObject(name) { layer = layer };
                    go.transform.SetParent(bones[b], false);
                    var shape = Best(pts, margin);
                    if (Gap(shape, skin) <= MaxGap) sb.Append($" {name}({Apply(go, shape)})");
                    else
                    {
                        var hull = Hull(pts, margin + HullSag, out var centre);
                        go.transform.localPosition = centre;   // a WeakPoint's transform is the zone centre for aim assist
                        hull.name = name;
                        AssetDatabase.AddObjectToAsset(hull, hullStore);
                        var mc = go.AddComponent<MeshCollider>();
                        mc.sharedMesh = hull;
                        mc.convex = true;
                        sb.Append($" {name}(hull)");
                    }
                    if (weak) go.AddComponent<WeakPoint>();
                    count++;
                }
                PrefabUtility.SaveAsPrefabAsset(root, path);
                return $"{sb} = {count} colliders";
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private const float ClusterCell = 0.10f;
        private const float StrayShare = 0.02f;

        /// <summary>
        /// A few vertices painted to a bone from across the body (El Rastrero's left thigh holds 3 from the right hip)
        /// would stretch its hull over the gap. A detached group under 2 % of the bone's vertices goes to each
        /// vertex's second bone instead. Returns how many vertices moved.
        /// </summary>
        private static int AdoptStrays(int[] lead, BoneWeight[] weights, int[] owner, Vector3[] bindLocal)
        {
            int moved = 0;
            foreach (var bone in lead.Distinct().ToArray())
            {
                var mine = Enumerable.Range(0, lead.Length).Where(i => lead[i] == bone).ToList();
                foreach (var group in Clusters(mine, bindLocal).Skip(1))
                {
                    if (group.Count >= mine.Count * StrayShare) continue;
                    foreach (int i in group)
                        if (weights[i].weight1 > 0f && owner[weights[i].boneIndex1] != bone) { lead[i] = owner[weights[i].boneIndex1]; moved++; }
                }
            }
            return moved;
        }

        /// <summary>Groups a bone's vertices into pieces that touch (bind pose, 10 cm cells), largest first.</summary>
        private static List<List<int>> Clusters(List<int> vertices, Vector3[] bindLocal)
        {
            Vector3Int Cell(int i) => Vector3Int.FloorToInt(bindLocal[i] / ClusterCell);
            var byCell = vertices.GroupBy(Cell).ToDictionary(g => g.Key, g => g.ToList());
            var seen = new HashSet<Vector3Int>();
            var result = new List<List<int>>();
            foreach (var start in byCell.Keys)
            {
                if (!seen.Add(start)) continue;
                var cluster = new List<int>();
                var open = new Stack<Vector3Int>();
                open.Push(start);
                while (open.Count > 0)
                {
                    var c = open.Pop();
                    cluster.AddRange(byCell[c]);
                    for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    for (int z = -1; z <= 1; z++)
                    {
                        var n = c + new Vector3Int(x, y, z);
                        if (byCell.ContainsKey(n) && seen.Add(n)) open.Push(n);
                    }
                }
                result.Add(cluster);
            }
            return result.OrderByDescending(c => c.Count).ToList();   // the main piece keeps the bone's plain name
        }

        /// <summary>
        /// Samples every clip of the enemy's controller except death (a dead enemy's hitboxes are off): one array per
        /// frame with each vertex in the space of the bone that leads it. The prefab's pose is restored before saving.
        /// </summary>
        private static List<Vector3[]> SamplePoses(GameObject root, SkinnedMeshRenderer smr, int[] lead)
        {
            var result = new List<Vector3[]>();
            var animator = root.GetComponentInChildren<Animator>(true);
            if (animator == null || animator.runtimeAnimatorController == null) return result;
            var all = root.GetComponentsInChildren<Transform>(true);
            var rest = all.Select(t => (t.localPosition, t.localRotation, t.localScale)).ToArray();
            var bones = smr.bones;
            var baked = new Mesh();
            try
            {
                foreach (var clip in animator.runtimeAnimatorController.animationClips.Distinct())
                {
                    if (clip.name.Contains("Death")) continue;
                    for (int f = 0; f < FramesPerClip; f++)
                    {
                        clip.SampleAnimation(animator.gameObject, clip.length * f / (FramesPerClip - 1));
                        smr.BakeMesh(baked, true);
                        var v = baked.vertices;
                        var toWorld = smr.transform.localToWorldMatrix;
                        var frame = new Vector3[v.Length];
                        for (int i = 0; i < v.Length; i++) frame[i] = bones[lead[i]].InverseTransformPoint(toWorld.MultiplyPoint3x4(v[i]));
                        result.Add(frame);
                    }
                }
            }
            finally
            {
                for (int i = 0; i < all.Length; i++) { all[i].localPosition = rest[i].Item1; all[i].localRotation = rest[i].Item2; all[i].localScale = rest[i].Item3; }
                Object.DestroyImmediate(baked);
            }
            return result;
        }

        private static bool IsFinger(string name)
        {
            string n = name.ToLowerInvariant();
            return FingerWords.Any(n.Contains);
        }

        private static int FoldFinger(int b, Transform[] bones)
        {
            var t = bones[b];
            while (t != null && IsFinger(t.name)) t = t.parent;
            int i = t == null ? -1 : System.Array.IndexOf(bones, t);
            return i < 0 ? b : i;
        }

        private const int HullDirections = 120;   // at most 236 faces: PhysX keeps a convex hull under 256 polygons
        private const float HullSag = 0.015f;     // a hull from 120 support points sags up to this much between them on a broad back

        /// <summary>
        /// Convex hull of the points grown by the margin. Only the outermost point along each of 120 even directions
        /// is kept, pushed the margin further along that direction, so the hull stays under PhysX's polygon limit.
        /// The hull is built by PhysX when it cooks the collider.
        /// </summary>
        private static Mesh Hull(List<Vector3> pts, float margin, out Vector3 centre)
        {
            var bounds = new Bounds(pts[0], Vector3.zero);
            foreach (var p in pts) bounds.Encapsulate(p);
            centre = bounds.center;
            var grown = new HashSet<Vector3>();
            for (int i = 0; i < HullDirections; i++)
            {
                // Fibonacci sphere: even directions without clustering at the poles.
                float y = 1f - 2f * (i + 0.5f) / HullDirections, r = Mathf.Sqrt(1f - y * y), a = i * 2.399963f;
                var d = new Vector3(Mathf.Cos(a) * r, y, Mathf.Sin(a) * r);
                var far = pts[0];
                foreach (var p in pts) if (Vector3.Dot(p, d) > Vector3.Dot(far, d)) far = p;
                grown.Add(far - centre + d * margin);
            }
            var mesh = new Mesh();
            var verts = grown.ToList();
            mesh.SetVertices(verts);
            var tris = new int[(verts.Count - 2) * 3];   // a fan: only the vertices matter to convex cooking
            for (int t = 0; t < verts.Count - 2; t++) { tris[t * 3] = 0; tris[t * 3 + 1] = t + 1; tris[t * 3 + 2] = t + 2; }
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>How far the outer points of the shape (box corners, edges and faces; capsule rings and tips) lie from the nearest skin vertex.</summary>
        private static float Gap(Shape s, Vector3[] skin)
        {
            var probes = new List<Vector3>();
            if (s.Capsule)
            {
                int u = (s.Axis + 1) % 3, v = (s.Axis + 2) % 3;
                for (int e = -1; e <= 1; e++)
                for (int k = 0; k < 8; k++)
                {
                    var p = Vector3.zero;
                    p[s.Axis] = e * (s.Half - s.Radius);
                    float ang = k * Mathf.PI / 4f;
                    p[u] = Mathf.Cos(ang) * s.Radius;
                    p[v] = Mathf.Sin(ang) * s.Radius;
                    probes.Add(p);
                }
                var tip = Vector3.zero;
                tip[s.Axis] = s.Half;
                probes.Add(tip);
                probes.Add(-tip);
            }
            else
            {
                for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                for (int k = -1; k <= 1; k++)
                    if (i != 0 || j != 0 || k != 0) probes.Add(Vector3.Scale(new Vector3(i, j, k), s.Size * 0.5f));
            }
            float worst = 0f;
            foreach (var p in probes)
            {
                Vector3 q = s.Frame * (s.Centre + p);
                float best = float.MaxValue;
                foreach (var x in skin) best = Mathf.Min(best, (x - q).sqrMagnitude);
                worst = Mathf.Max(worst, best);
            }
            return Mathf.Sqrt(worst);
        }

        private static Vector3 CapsuleSize(Shape s)
        {
            var v = Vector3.one * (2f * s.Radius);
            v[s.Axis] = 2f * s.Half;
            return v;
        }

        /// <summary>The smallest of four candidates: a box or a capsule, aligned to the bone or to the points' principal axes.</summary>
        private static Shape Best(List<Vector3> pts, float margin)
        {
            var best = FitInFrame(pts, Quaternion.identity, margin);
            var pca = FitInFrame(pts, PrincipalAxes(pts), margin);
            return pca.Volume < best.Volume ? pca : best;
        }

        /// <summary>Puts the collider object at the shape's centre, so a WeakPoint's transform is the zone centre for aim assist.</summary>
        private static string Apply(GameObject go, Shape best)
        {
            go.transform.localRotation = best.Frame;
            go.transform.localPosition = best.Frame * best.Centre;
            if (best.Capsule)
            {
                var cap = go.AddComponent<CapsuleCollider>();
                cap.direction = best.Axis;
                cap.radius = best.Radius;
                cap.height = 2f * best.Half;
                return $"cap r={best.Radius:0.00} h={2f * best.Half:0.00}";
            }
            var box = go.AddComponent<BoxCollider>();
            box.size = best.Size;
            return $"box {best.Size.x:0.00}x{best.Size.y:0.00}x{best.Size.z:0.00}";
        }

        private struct Shape
        {
            public Quaternion Frame;
            public Vector3 Centre, Size;
            public bool Capsule;
            public int Axis;
            public float Radius, Half, Volume;
        }

        private static Shape FitInFrame(List<Vector3> world, Quaternion frame, float margin)
        {
            var inv = Quaternion.Inverse(frame);
            var pts = world.Select(p => inv * p).ToList();
            var bounds = new Bounds(pts[0], Vector3.zero);
            foreach (var p in pts) bounds.Encapsulate(p);
            Vector3 c = bounds.center, size = bounds.size + Vector3.one * (2f * margin);

            int axis = size.x >= size.y && size.x >= size.z ? 0 : size.y >= size.z ? 1 : 2;
            float r = 0f;
            foreach (var p in pts)
            {
                var d = p - c; d[axis] = 0f;
                r = Mathf.Max(r, d.magnitude);
            }
            r += margin;
            float half = r;
            foreach (var p in pts)
            {
                var d = p - c; float a = Mathf.Abs(d[axis]); d[axis] = 0f;
                float rad = Mathf.Min(d.magnitude + margin, r);
                half = Mathf.Max(half, a + margin + r - Mathf.Sqrt(Mathf.Max(0f, r * r - rad * rad)));
            }
            float capVol = Mathf.PI * r * r * (2f * half - 2f * r) + 4f / 3f * Mathf.PI * r * r * r;
            float boxVol = size.x * size.y * size.z;
            return new Shape
            {
                Frame = frame, Centre = c, Size = size, Capsule = capVol < boxVol, Axis = axis,
                Radius = r, Half = half, Volume = Mathf.Min(capVol, boxVol),
            };
        }

        /// <summary>Eigenvectors of the points' covariance (Jacobi sweeps on the 3x3 matrix), as a right-handed rotation.</summary>
        private static Quaternion PrincipalAxes(List<Vector3> pts)
        {
            Vector3 mean = Vector3.zero;
            foreach (var p in pts) mean += p;
            mean /= pts.Count;
            var a = new double[3, 3];
            foreach (var p in pts)
            {
                var d = p - mean;
                for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++) a[i, j] += d[i] * d[j];
            }
            var v = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            for (int sweep = 0; sweep < 50; sweep++)
            {
                for (int pI = 0; pI < 2; pI++)
                for (int q = pI + 1; q < 3; q++)
                {
                    if (System.Math.Abs(a[pI, q]) < 1e-12) continue;
                    double theta = 0.5 * System.Math.Atan2(2 * a[pI, q], a[q, q] - a[pI, pI]);
                    double cs = System.Math.Cos(theta), sn = System.Math.Sin(theta);
                    for (int k = 0; k < 3; k++)
                    {
                        double akp = a[k, pI], akq = a[k, q];
                        a[k, pI] = cs * akp - sn * akq;
                        a[k, q] = sn * akp + cs * akq;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        double apk = a[pI, k], aqk = a[q, k];
                        a[pI, k] = cs * apk - sn * aqk;
                        a[q, k] = sn * apk + cs * aqk;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        double vkp = v[k, pI], vkq = v[k, q];
                        v[k, pI] = cs * vkp - sn * vkq;
                        v[k, q] = sn * vkp + cs * vkq;
                    }
                }
            }
            var x = new Vector3((float)v[0, 0], (float)v[1, 0], (float)v[2, 0]).normalized;
            var y = new Vector3((float)v[0, 1], (float)v[1, 1], (float)v[2, 1]).normalized;
            if (x.sqrMagnitude < 0.5f || y.sqrMagnitude < 0.5f) return Quaternion.identity;
            return Quaternion.LookRotation(Vector3.Cross(x, y), y);
        }
    }
}
