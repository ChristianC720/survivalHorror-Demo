using System.Linq;
using Heartbeat.AI;
using Heartbeat.Combat;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>
    /// C-64: only El Alto and El Mudo grab, with the GDD values (40 %, 1,6 m). Their EnemyDefinition grab timing and
    /// anchor match the Grab clip: at grabCloseSeconds the hands are on Lukas's shoulders (shoulder height, just in
    /// front of his root at the anchor) and they stay there for grabHoldSeconds. The Grab state follows the Rate
    /// parameter so the windup can speed it up.
    /// </summary>
    public class EnemyGrabTests
    {
        private const float ShoulderMin = 1.3f, ShoulderMax = 1.75f;   // Lukas's shoulders sit at ~1,36 m
        private const float HandsBehindRoot = 0.2f, HoldDrift = 0.12f;
        private static readonly Vector3 Away = new(0f, -5000f, 0f);

        private static EnemyDefinition Def(string n) => AssetDatabase.LoadAssetAtPath<EnemyDefinition>($"Assets/_Project/ScriptableObjects/Enemies/ED_{n}.asset");

        [TestCase("Alto"), TestCase("Mudo")]
        public void Grabbers_UseTheGddValues(string n)
        {
            var d = Def(n);
            Assert.IsTrue(d.canGrab);
            Assert.AreEqual(0.4f, d.grabChance, 1e-4f);
            Assert.AreEqual(1.6f, d.grabRange, 1e-4f);
            Assert.AreEqual(0.7f, d.attackWindup, 1e-4f);
        }

        [TestCase("Boca"), TestCase("Rastrero")]
        public void BocaAndRastrero_NeverGrab(string n)
        {
            Assert.IsFalse(Def(n).canGrab);
            var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>($"Assets/_Project/Models/Enemies/AC_{n}.controller");
            Assert.IsFalse(ctrl.layers[0].stateMachine.states.Any(s => s.state.name == "Grab"));
        }

        [TestCase("Alto"), TestCase("Mudo")]
        public void GrabClip_HandsRestOnLukasAtTheAnchor_ForTheWholeHold(string n)
        {
            var d = Def(n);
            var go = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Characters/Enemy_{n}.prefab"), Away, Quaternion.identity);
            try
            {
                var anim = go.GetComponentInChildren<Animator>();
                var clip = anim.runtimeAnimatorController.animationClips.First(c => c.name == $"AN_{n}_Grab");
                var bones = go.GetComponentsInChildren<Transform>(true);
                Transform l = bones.First(b => b.name == "hand_L"), r = bones.First(b => b.name == "hand_R");
                Assert.LessOrEqual(d.grabCloseSeconds + d.grabHoldSeconds, clip.length);

                Vector3 closed = Vector3.zero;
                for (int i = 0; i <= 10; i++)
                {
                    float t = d.grabCloseSeconds + d.grabHoldSeconds * i / 10f;
                    clip.SampleAnimation(anim.gameObject, t);
                    Vector3 hands = go.transform.InverseTransformPoint((l.position + r.position) * 0.5f);
                    if (i == 0) closed = hands;
                    Assert.That(hands.y, Is.InRange(ShoulderMin, ShoulderMax), $"{n} t={t:0.00}: hands at {hands.y:0.00} m, not at shoulder height");
                    float behindLukasRoot = d.grabAnchorDistance - hands.z;   // Lukas faces the enemy at the anchor
                    Assert.That(behindLukasRoot, Is.InRange(0f, HandsBehindRoot), $"{n} t={t:0.00}: hands {hands.z:0.00} m out, Lukas at {d.grabAnchorDistance:0.00} m");
                    Assert.Less(Vector3.Distance(hands, closed), HoldDrift, $"{n} t={t:0.00}: hands left Lukas during the hold");
                }
            }
            finally { Object.DestroyImmediate(go); }
        }

        [TestCase("Alto"), TestCase("Mudo")]
        public void GrabState_FollowsRate(string n)
        {
            var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>($"Assets/_Project/Models/Enemies/AC_{n}.controller");
            var grab = ctrl.layers[0].stateMachine.states.First(s => s.state.name == "Grab").state;
            Assert.IsTrue(grab.speedParameterActive);
            Assert.AreEqual("Rate", grab.speedParameter);
        }

        [Test]
        public void Pull_EasesFromTheStartToTheAnchor()
        {
            Assert.AreEqual(1.6f, GrabPull.Distance(1.6f, 0.86f, 0f), 1e-4f);
            Assert.AreEqual(0.86f, GrabPull.Distance(1.6f, 0.86f, 1f), 1e-4f);
            Assert.AreEqual(1.23f, GrabPull.Distance(1.6f, 0.86f, 0.5f), 1e-3f);
            Assert.Greater(GrabPull.Distance(1.6f, 0.86f, 0.1f), 1.55f, "slow start");
        }
    }
}
