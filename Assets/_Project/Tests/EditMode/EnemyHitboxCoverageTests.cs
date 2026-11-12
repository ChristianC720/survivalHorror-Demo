using System.Collections.Generic;
using System.Linq;
using Heartbeat.Boss;
using Heartbeat.Combat;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>
    /// C-63: the shot hitboxes must cover the whole animated body and nothing far from it. For each enemy and five
    /// poses, 200 rays aimed at points of the baked mesh surface must land on a hitbox no later than the mesh itself
    /// (98 % or more), and 200 rays that pass 15 to 40 cm from the mesh must hit nothing. Rays at the weak-point zone
    /// must land on a WeakPoint.
    /// </summary>
    public class EnemyHitboxCoverageTests
    {
        private const int Rays = 200;
        private const float MinHitRate = 0.98f;
        private const float MinWeakRate = 0.7f;   // regression floor, not a target: 75 to 98 % measured (El Alto's grab lowest); the rest sit under an arm or a hand
        private static readonly Vector3 Away = new(0f, -5000f, 0f);   // clear of whatever scene is open

        // Rest, walk, attack, grab and stunned. La Boca does not grab: its scream opens the mouth. El Rastrero does not grab either: hunt.
        private static readonly Dictionary<string, string[]> Poses = new()
        {
            { "Alto", new[] { "Idle", "Move", "Attack", "Grab", "Stunned" } },
            { "Boca", new[] { "Idle", "Move", "Attack", "Scream", "Stunned" } },
            { "Mudo", new[] { "Idle", "Move", "Attack", "Grab", "Stunned" } },
            { "Rastrero", new[] { "Idle", "Move", "Attack", "Hunt", "Stunned" } },
        };

        private static int HitboxMask => LayerMask.GetMask("Hitbox");

        [TestCase("Alto"), TestCase("Boca"), TestCase("Mudo"), TestCase("Rastrero")]
        public void HitboxesCoverTheAnimatedMeshAndNothingElse(string enemy)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Characters/Enemy_{enemy}.prefab");
            var go = Object.Instantiate(prefab, Away, Quaternion.identity);
            var probe = new GameObject("MeshProbe") { layer = LayerMask.NameToLayer("Ignore Raycast") };
            var probeCollider = probe.AddComponent<MeshCollider>();
            var baked = new Mesh();
            try
            {
                var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
                var animator = go.GetComponentInChildren<Animator>();
                var controller = (AnimatorController)animator.runtimeAnimatorController;
                var states = controller.layers[0].stateMachine.states.ToDictionary(s => s.state.name, s => s.state);
                var weakBones = new HashSet<Transform>(go.GetComponentsInChildren<WeakPoint>().Select(w => w.transform.parent));
                var boneIndex = smr.bones.Select((b, i) => (b, i)).ToDictionary(x => x.b, x => x.i);
                var weakIdx = new HashSet<int>(weakBones.Where(boneIndex.ContainsKey).Select(b => boneIndex[b]));
                Assert.That(weakBones.Count, Is.GreaterThan(0), "no weak point");
                var bindPose = smr.bones.Select(b => b.position).ToArray();
                var rng = new System.Random(63);
                var report = new List<string>();
                var failures = new List<string>();

                foreach (var pose in Poses[enemy])
                {
                    var clip = (AnimationClip)states[pose].motion;
                    clip.SampleAnimation(animator.gameObject, clip.length * 0.5f);
                    float moved = smr.bones.Select((b, i) => Vector3.Distance(b.position, bindPose[i])).Max();
                    Assert.That(moved, Is.GreaterThan(0.05f), $"{enemy} {pose}: the clip did not move the skeleton");

                    smr.BakeMesh(baked, true);
                    probe.transform.SetPositionAndRotation(smr.transform.position, smr.transform.rotation);
                    probeCollider.sharedMesh = null;
                    probeCollider.sharedMesh = baked;
                    Physics.SyncTransforms();

                    var toWorld = smr.transform.localToWorldMatrix * Matrix4x4.Scale(Inverse(smr.transform.lossyScale));
                    var surface = new Surface(baked, toWorld, rng);
                    var weightsByVertex = smr.sharedMesh.boneWeights;

                    int covered = 0, aimed = 0;
                    for (int tries = 0; aimed < Rays && tries < Rays * 20; tries++)
                    {
                        surface.Sample(out var p, out var normal, out _);
                        var ray = new Ray(p + normal * 5f, -normal);
                        if (!probeCollider.Raycast(ray, out var meshHit, 6f)) continue;   // grazing triangle: draw again
                        aimed++;
                        if (FirstHitbox(ray, go, out var boxHit) && boxHit.distance <= meshHit.distance + 0.005f) covered++;
                    }

                    // The weak-point zone: open surface (nothing of the mesh in front) owned by a weak bone.
                    var weakSurface = new Surface(baked, toWorld, rng, v0 => weakIdx.Contains(weightsByVertex[v0].boneIndex0));
                    int weakHit = 0, weakTried = 0;
                    for (int tries = 0; weakTried < Rays && tries < Rays * 20; tries++)
                    {
                        weakSurface.Sample(out var p, out var normal, out _);
                        var ray = new Ray(p + normal * 5f, -normal);
                        if (!probeCollider.Raycast(ray, out var meshHit, 6f) || Vector3.Distance(meshHit.point, p) > 0.01f) continue;
                        weakTried++;
                        if (FirstHitbox(ray, go, out var boxHit) && WeakPoint.At(boxHit.collider, boxHit.point) != null) weakHit++;   // the rule the revolver applies
                    }

                    int falseHits = 0, nearRays = 0;
                    var strays = new List<string>();
                    Vector3 centre = surface.Centre;
                    for (int tries = 0; nearRays < Rays && tries < Rays * 200; tries++)
                    {
                        Vector3 origin = centre + RandomOnSphere(rng) * 5f;
                        surface.Sample(out var p, out _, out _);
                        Vector3 aim = p + RandomOnSphere(rng) * Mathf.Lerp(0.15f, 0.4f, (float)rng.NextDouble());
                        var ray = new Ray(origin, (aim - origin).normalized);
                        float d = surface.DistanceToRay(ray, 10f, 0.15f);
                        if (d < 0.15f || d > 0.4f) continue;
                        nearRays++;
                        if (FirstHitbox(ray, go, out var stray)) { falseHits++; strays.Add($"{stray.collider.name} {d:0.00} m"); }
                    }

                    float rate = covered / (float)Rays;
                    float weakRate = weakHit / (float)Mathf.Max(1, weakTried);
                    report.Add($"{enemy} {pose}: covered {covered}/{Rays}, weak {weakHit}/{weakTried}, near-miss hits {falseHits}/{nearRays} {string.Join(", ", strays)}");
                    if (aimed < Rays || nearRays < Rays || weakTried < Rays) failures.Add($"{enemy} {pose}: could not place the rays");
                    if (rate < MinHitRate || weakRate < MinWeakRate || falseHits > 0) failures.Add(report[^1]);
                }
                TestContext.WriteLine(string.Join("\n", report));
                Assert.That(failures, Is.Empty, string.Join("\n", report));
            }
            finally
            {
                Object.DestroyImmediate(go);
                Object.DestroyImmediate(probe);
                Object.DestroyImmediate(baked);
            }
        }

        /// <summary>
        /// The Mother: her transformers are the only thing a round can damage. Shots at their surface must reach them.
        /// From 1.5 m: farther out, the rays on the inner side start behind the core, where Lukas cannot stand.
        /// </summary>
        [Test]
        public void MotherTransformersTakeTheShotsAimedAtThem()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Rooms/F3/ROOM_Mother.prefab");
            var room = Object.Instantiate(prefab, Away, Quaternion.identity);
            try
            {
                Physics.SyncTransforms();
                int mask = LayerMask.GetMask("Environment", "Hitbox");
                var rng = new System.Random(63);
                var transformers = room.GetComponentsInChildren<BossTransformer>(true);
                Assert.That(transformers.Length, Is.GreaterThan(0));
                foreach (var t in transformers)
                {
                    var mf = t.GetComponentsInChildren<MeshFilter>(true).OrderByDescending(f => f.sharedMesh.bounds.size.sqrMagnitude).First();
                    var surface = new Surface(mf.sharedMesh, mf.transform.localToWorldMatrix, rng);
                    var own = t.GetComponentsInChildren<Collider>(true);
                    int hits = 0, aimed = 0;
                    for (int tries = 0; aimed < Rays && tries < Rays * 20; tries++)
                    {
                        surface.Sample(out var p, out var normal, out _);
                        Vector3 origin = p + normal * 1.5f;
                        // Undersides of the insulators face down into the body: no shot can start inside the transformer.
                        if (own.Any(c => c.ClosestPoint(origin) == origin)) continue;
                        aimed++;
                        if (Physics.Raycast(origin, -normal, out var h, 1.7f, mask, QueryTriggerInteraction.Ignore)
                            && h.collider.GetComponentInParent<IDamageable>() == (IDamageable)t)
                            hits++;
                    }
                    Assert.That(aimed, Is.EqualTo(Rays), $"{t.name}: could not place the rays");
                    Assert.That(hits / (float)Rays, Is.GreaterThanOrEqualTo(MinHitRate), $"{t.name}: {hits}/{Rays}");
                }
            }
            finally { Object.DestroyImmediate(room); }
        }

        private static bool FirstHitbox(Ray ray, GameObject enemy, out RaycastHit hit) =>
            Physics.Raycast(ray, out hit, 10f, HitboxMask, QueryTriggerInteraction.Ignore) && hit.collider.transform.IsChildOf(enemy.transform);

        private static Vector3 Inverse(Vector3 s) => new(1f / s.x, 1f / s.y, 1f / s.z);

        private static Vector3 RandomOnSphere(System.Random rng)
        {
            while (true)
            {
                var v = new Vector3((float)rng.NextDouble() * 2f - 1f, (float)rng.NextDouble() * 2f - 1f, (float)rng.NextDouble() * 2f - 1f);
                float m = v.sqrMagnitude;
                if (m > 1e-4f && m <= 1f) return v / Mathf.Sqrt(m);
            }
        }

        /// <summary>Area-weighted sampling of a mesh surface in world space, plus a dense point cloud for distances.</summary>
        private sealed class Surface
        {
            private readonly Vector3[] v;
            private readonly int[] tris;
            private readonly float[] cumulative;
            private readonly System.Random rng;
            private Vector3[] cloud;
            public Vector3 Centre { get; }

            /// <param name="keep">Optional filter on a triangle's first vertex: only those triangles are sampled.</param>
            public Surface(Mesh mesh, Matrix4x4 toWorld, System.Random rng, System.Func<int, bool> keep = null)
            {
                this.rng = rng;
                v = mesh.vertices.Select(x => toWorld.MultiplyPoint3x4(x)).ToArray();
                var all = mesh.triangles;
                tris = keep == null ? all : Enumerable.Range(0, all.Length / 3).Where(t => keep(all[t * 3])).SelectMany(t => new[] { all[t * 3], all[t * 3 + 1], all[t * 3 + 2] }).ToArray();
                cumulative = new float[tris.Length / 3];
                float total = 0f;
                for (int t = 0; t < cumulative.Length; t++)
                {
                    total += Vector3.Cross(v[tris[t * 3 + 1]] - v[tris[t * 3]], v[tris[t * 3 + 2]] - v[tris[t * 3]]).magnitude * 0.5f;
                    cumulative[t] = total;
                }
                var b = new Bounds(v[0], Vector3.zero);
                foreach (var p in v) b.Encapsulate(p);
                Centre = b.center;
            }

            public void Sample(out Vector3 point, out Vector3 normal, out int vertex)
            {
                float r = (float)rng.NextDouble() * cumulative[^1];
                int t = System.Array.BinarySearch(cumulative, r);
                if (t < 0) t = ~t;
                t = Mathf.Min(t, cumulative.Length - 1);
                Vector3 a = v[tris[t * 3]], b = v[tris[t * 3 + 1]], c = v[tris[t * 3 + 2]];
                float u = (float)rng.NextDouble(), w = (float)rng.NextDouble();
                if (u + w > 1f) { u = 1f - u; w = 1f - w; }
                point = a + (b - a) * u + (c - a) * w;
                normal = Vector3.Cross(b - a, c - a).normalized;
                vertex = tris[t * 3];
            }

            public float DistanceToRay(Ray ray, float length, float stopBelow = 0f)
            {
                if (cloud == null)
                {
                    var list = new List<Vector3>(v);
                    for (int i = 0; i < 15000; i++) { Sample(out var p, out _, out _); list.Add(p); }
                    cloud = list.ToArray();
                }
                float best = float.MaxValue;
                foreach (var p in cloud)
                {
                    float s = Mathf.Clamp(Vector3.Dot(p - ray.origin, ray.direction), 0f, length);
                    best = Mathf.Min(best, (ray.origin + ray.direction * s - p).sqrMagnitude);
                    if (best < stopBelow * stopBelow) break;   // already too close: the caller rejects the ray
                }
                return Mathf.Sqrt(best);
            }
        }
    }
}
