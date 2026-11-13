using System.Linq;
using Heartbeat.Combat;
using Heartbeat.Recycle;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>
    /// C-76: the OverBody context reaches the whole lying body. For each enemy in the last frame of its death clip, with
    /// the hitboxes disabled as on death, Lukas 1 m from the head, the torso and the feet, facing them, gets the body.
    /// 1.6 m from every part, all around the body, he does not.
    /// </summary>
    public class FinisherReachTests
    {
        private const float Range = 1.5f;   // CombatBalance.finisherRange
        private static readonly Vector3 Away = new(0f, -5000f, 0f);

        [TestCase("Alto"), TestCase("Boca"), TestCase("Mudo"), TestCase("Rastrero")]
        public void OverBodyReachesHeadTorsoAndFeet(string enemy)
        {
            var go = DeadBody(enemy, out var corpse, out var smr);
            try
            {
                Vector3 head = Floor(Bone(smr, "head"));
                Vector3 torso = Floor(Bone(smr, "chest"));
                Vector3 feet = Floor(Bone(smr, "foot_l", "leftfoot"));
                Vector3 axis = (head - feet).normalized;
                Vector3 side = Vector3.Cross(Vector3.up, axis);

                Assert.That(Reached(corpse, head + axis, head), Is.True, $"{enemy}: not reached beside the head");
                Assert.That(Reached(corpse, torso + side, torso), Is.True, $"{enemy}: not reached beside the torso");
                Assert.That(Reached(corpse, feet - axis, feet), Is.True, $"{enemy}: not reached beside the feet");
            }
            finally { Object.DestroyImmediate(go); }
        }

        [TestCase("Alto"), TestCase("Boca"), TestCase("Mudo"), TestCase("Rastrero")]
        public void OverBodyStopsAt1_6mFromEveryPart(string enemy)
        {
            var go = DeadBody(enemy, out var corpse, out _);
            var baked = new Mesh();
            try
            {
                var vertices = go.GetComponentsInChildren<SkinnedMeshRenderer>().SelectMany(skin =>
                {
                    skin.BakeMesh(baked, true);
                    var toWorld = skin.transform.localToWorldMatrix * Matrix4x4.Scale(Inverse(skin.transform.lossyScale));
                    return baked.vertices.Select(v => toWorld.MultiplyPoint3x4(v)).ToArray();
                }).ToArray();
                Vector3 centre = Floor(vertices.Aggregate(Vector3.zero, (a, v) => a + v) / vertices.Length);

                for (int i = 0; i < 24; i++)
                {
                    Vector3 dir = Quaternion.Euler(0f, i * 15f, 0f) * Vector3.forward;
                    Vector3 near = Out(vertices, centre, dir, 1.4f);
                    Vector3 far = Out(vertices, centre, dir, 1.6f);
                    Assert.That(Reached(corpse, near, centre), Is.True, $"{enemy} {i * 15}°: not reached 1.4 m from the body");
                    Assert.That(Reached(corpse, far, centre), Is.False, $"{enemy} {i * 15}°: reached 1.6 m from the body");
                }
            }
            finally { Object.DestroyImmediate(go); Object.DestroyImmediate(baked); }
        }

        [Test]
        public void WithTwoBodiesTheOneMostInFrontWins()
        {
            var ahead = Box("Ahead", new Vector3(0f, 0f, 1.2f));
            var aside = Box("Aside", new Vector3(0.8f, 0f, 0.8f));
            try
            {
                var pick = PlayerFinisher.Pick(new[] { aside, ahead }, Vector3.zero, Vector3.forward, Range, out _);
                Assert.That(pick, Is.SameAs(ahead));
            }
            finally { Object.DestroyImmediate(ahead.gameObject); Object.DestroyImmediate(aside.gameObject); }
        }

        private static bool Reached(Corpse corpse, Vector3 lukas, Vector3 lookAt)
        {
            Vector3 forward = lookAt - lukas; forward.y = 0f;
            return PlayerFinisher.Pick(new[] { corpse }, lukas, forward, Range, out _) == corpse;
        }

        /// <summary>From the centre outward until the closest vertex of the body is <paramref name="distance"/> away.</summary>
        private static Vector3 Out(Vector3[] vertices, Vector3 centre, Vector3 dir, float distance)
        {
            for (float r = 0f; r < 10f; r += 0.01f)
            {
                Vector3 p = centre + dir * r;
                if (vertices.Min(v => (v - p).sqrMagnitude) >= distance * distance) return p;
            }
            Assert.Fail("body too large");
            return centre;
        }

        private static GameObject DeadBody(string enemy, out Corpse corpse, out SkinnedMeshRenderer smr)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Characters/Enemy_{enemy}.prefab");
            var go = Object.Instantiate(prefab, Away, Quaternion.identity);
            var animator = go.GetComponentInChildren<Animator>();
            var controller = (AnimatorController)animator.runtimeAnimatorController;
            var death = (AnimationClip)controller.layers[0].stateMachine.states.First(s => s.state.name == "Death").state.motion;
            death.SampleAnimation(animator.gameObject, death.length);
            foreach (var c in go.GetComponentsInChildren<Collider>(true))
                if (c.gameObject != go) c.enabled = false;   // as EnemyBrain.OnDied leaves them
            corpse = go.GetComponent<Corpse>();
            smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
            return go;
        }

        private static Corpse Box(string name, Vector3 at)
        {
            var go = new GameObject(name);
            go.transform.position = at;
            go.AddComponent<BoxCollider>().size = new Vector3(0.4f, 0.3f, 0.4f);
            return go.AddComponent<Corpse>();
        }

        private static Vector3 Bone(SkinnedMeshRenderer smr, params string[] names) =>
            smr.bones.First(b => names.Any(n => b.name.ToLowerInvariant().Contains(n))).position;

        private static Vector3 Floor(Vector3 p) => new(p.x, Away.y, p.z);

        private static Vector3 Inverse(Vector3 s) => new(1f / s.x, 1f / s.y, 1f / s.z);
    }
}
