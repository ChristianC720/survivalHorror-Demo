using Heartbeat.AI;
using Heartbeat.Combat;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests.EditMode
{
    /// <summary>
    /// Weak-point damage (C-74): a weak-point hit deals the CombatBalance multiplier (2) times a normal hit, measured
    /// against the real enemy definitions. Mixed hits add up.
    /// </summary>
    public class WeakPointDamageTests
    {
        private GameObject root;
        private CombatBalance balance;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("WeakPointDamageTest");
            balance = AssetDatabase.LoadAssetAtPath<CombatBalance>("Assets/_Project/ScriptableObjects/Balance/CombatBalance.asset");
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        private EnemyHealth Enemy(string n, bool recycled = false)
        {
            var go = new GameObject(n);
            go.transform.SetParent(root.transform);
            var health = go.AddComponent<EnemyHealth>();
            var so = new SerializedObject(health);
            so.FindProperty("definition").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<EnemyDefinition>($"Assets/_Project/ScriptableObjects/Enemies/ED_{n}.asset");
            so.ApplyModifiedPropertiesWithoutUndo();
            health.Revive(recycled);
            return health;
        }

        private void Hit(EnemyHealth e, bool weak) =>
            e.ReceiveShot(new ShotHit(e.transform.position, Vector3.forward, weak, balance.ShotDamage(weak)));

        private int WeakHitsToKill(EnemyHealth e)
        {
            int n = 0;
            while (e.IsAlive && n < 10) { Hit(e, true); n++; }
            return n;
        }

        [Test]
        public void BalanceDoublesWeakPointDamage()
        {
            Assert.That(balance.weakPointDamageMultiplier, Is.EqualTo(2f));
            Assert.That(balance.ShotDamage(false), Is.EqualTo(1f));
            Assert.That(balance.ShotDamage(true), Is.EqualTo(2f));
        }

        [TestCase("Alto", 4, 2)]
        [TestCase("Boca", 3, 2)]
        [TestCase("Mudo", 3, 2)]
        [TestCase("Rastrero", 2, 1)]
        public void WeakPointHitsOnly(string enemy, int shotsToKill, int weakHits)
        {
            var e = Enemy(enemy);
            Assert.That(e.ShotsToKill, Is.EqualTo(shotsToKill), "shotsToKill does not change");
            Assert.That(WeakHitsToKill(e), Is.EqualTo(weakHits));
        }

        [TestCase("Alto")]
        [TestCase("Boca")]
        [TestCase("Mudo")]
        [TestCase("Rastrero")]
        public void RecycledDiesWithOneWeakPointHit(string enemy) => Assert.That(WeakHitsToKill(Enemy(enemy, recycled: true)), Is.EqualTo(1));

        [Test]
        public void MixedHitsAddUp()
        {
            var alto = Enemy("Alto");
            Hit(alto, true);
            Hit(alto, false);
            Assert.That(alto.IsAlive, Is.True, "one weak point and one normal hit is 3 of 4");
            Hit(alto, false);
            Assert.That(alto.IsAlive, Is.False, "one weak point and two normal hits kill El Alto");
            Assert.That(alto.DamageTaken, Is.EqualTo(4f));
        }

        [Test]
        public void NormalHitsStillCountOne()
        {
            var alto = Enemy("Alto");
            for (int i = 0; i < 3; i++) Hit(alto, false);
            Assert.That(alto.IsAlive, Is.True);
            Hit(alto, false);
            Assert.That(alto.IsAlive, Is.False);
        }

        [Test]
        public void DiedFiresOnceAndDeadIgnoresHits()
        {
            var boca = Enemy("Boca");
            int died = 0, shots = 0;
            boca.Died += () => died++;
            boca.Shot += _ => shots++;
            Hit(boca, true);
            Hit(boca, true);
            Hit(boca, true);
            Assert.That((died, shots), Is.EqualTo((1, 2)));
            Assert.That(boca.DamageTaken, Is.EqualTo(4f), "overkill on the last hit is kept, later hits are ignored");
        }

        [Test]
        public void ReviveClearsDamage()
        {
            var mudo = Enemy("Mudo");
            Hit(mudo, true);
            Hit(mudo, true);
            mudo.Revive(true);
            Assert.That(mudo.IsAlive, Is.True);
            Assert.That(mudo.DamageTaken, Is.EqualTo(0f));
        }
    }
}
