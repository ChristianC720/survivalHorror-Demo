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
    /// collider (capsule or box, whichever is smaller) that wraps the bind-pose vertices it drives, 2 cm outward, so
    /// the hitbox rides the animation. Finger bones fold into their hand. The weak-point bones from the GDD get the
    /// <see cref="WeakPoint"/> marker on their own collider. Idempotent: old Hit_* / WeakPoint_* objects are replaced.
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
        private const float SharedWeight = 0.4f;  // a vertex also widens every other bone that moves it this much (joints)
        private const float MaxPiece = 0.25f;     // a fit longer than this is halved...
        private const int MaxDepth = 3;           // ...at most three times: up to 8 colliders per bone
        private const float WeakExtra = 0.02f;    // weak points reach 2 cm past their neighbours, so they win the shared seams

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

                var weakIdx = new HashSet<int>(Enumerable.Range(0, bones.Length).Where(b => weakBones.Contains(bones[b].name)));
                var points = new List<Vector3>[bones.Length];
                void Add(int bone, float w, Vector3 v)
                {
                    if (bone < 0 || w <= 0f) return;
                    int o = owner[bone];
                    (points[o] ??= new List<Vector3>()).Add(bind[o].MultiplyPoint3x4(v));   // bind pose, in the bone's space
                }
                for (int i = 0; i < verts.Length; i++)
                {
                    var w = weights[i];
                    Add(w.boneIndex0, 1f, verts[i]);   // the dominant bone always owns the vertex
                    if (weakIdx.Contains(w.boneIndex0)) continue;   // a weak-point vertex never widens a neighbour over it
                    if (w.weight1 >= SharedWeight) Add(w.boneIndex1, w.weight1, verts[i]);
                    if (w.weight2 >= SharedWeight) Add(w.boneIndex2, w.weight2, verts[i]);
                    if (w.weight3 >= SharedWeight) Add(w.boneIndex3, w.weight3, verts[i]);
                }

                var sb = new StringBuilder($"{enemy}:");
                int count = 0;
                for (int b = 0; b < bones.Length; b++)
                {
                    if (points[b] == null || points[b].Count == 0) continue;
                    bool weak = weakBones.Contains(bones[b].name);
                    float margin = (Margin + (weak ? WeakExtra : 0f)) / Mathf.Max(1e-4f, bones[b].lossyScale.x);
                    var pieces = new List<Shape>();
                    Split(points[b], margin, 0, pieces);
                    for (int k = 0; k < pieces.Count; k++)
                    {
                        string name = (weak ? "WeakPoint_" : "Hit_") + bones[b].name + (k == 0 ? "" : "_" + k);
                        var go = new GameObject(name) { layer = layer };
                        go.transform.SetParent(bones[b], false);
                        sb.Append($" {name}({Apply(go, pieces[k])})");
                        if (weak) go.AddComponent<WeakPoint>();
                        count++;
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root, path);
                return $"{sb} = {count} colliders";
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
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

        /// <summary>
        /// Fits the points, and halves them along the longest side of the fit while a piece is larger than
        /// <see cref="MaxPiece"/>: one box over a whole shoulder or a crawling head leaves corners far from the skin.
        /// </summary>
        private static void Split(List<Vector3> pts, float margin, int depth, List<Shape> into)
        {
            var shape = Best(pts, margin);
            var size = shape.Capsule ? CapsuleSize(shape) : shape.Size;
            int axis = size.x >= size.y && size.x >= size.z ? 0 : size.y >= size.z ? 1 : 2;
            if (depth >= MaxDepth || size[axis] <= MaxPiece || pts.Count < 16) { into.Add(shape); return; }
            var inv = Quaternion.Inverse(shape.Frame);
            var sorted = pts.OrderBy(p => (inv * p)[axis]).ToList();
            int mid = sorted.Count / 2;
            Split(sorted.GetRange(0, mid), margin, depth + 1, into);
            Split(sorted.GetRange(mid, sorted.Count - mid), margin, depth + 1, into);
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
