using Heartbeat.AI;
using Heartbeat.Combat;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests.EditMode
{
    /// <summary>
    /// Shove damage (C-77): a shove that lands deals 0.6 of a normal shot, adds up with shots, can kill,
    /// and raises no Shot event (no hit feedback). Measured against the real El Alto definition (4 shots).
    /// </summary>
    public class ShoveDamageTests
    {
        private GameObject root;
        private CombatBalance balance;
        private EnemyHealth alto;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("ShoveDamageTest");
            balance = AssetDatabase.LoadAssetAtPath<CombatBalance>("Assets/_Project/ScriptableObjects/Balance/CombatBalance.asset");
            alto = root.AddComponent<EnemyHealth>();
            var so = new SerializedObject(alto);
            so.FindProperty("definition").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<EnemyDefinition>("Assets/_Project/ScriptableObjects/Enemies/ED_Alto.asset");
            so.ApplyModifiedPropertiesWithoutUndo();
            alto.Revive(false);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        private void Shove(int n) { for (int i = 0; i < n; i++) alto.ReceiveShove(balance.shoveDamageMultiplier); }
        private void Shoot(int n) { for (int i = 0; i < n; i++) alto.ReceiveShot(new ShotHit(Vector3.zero, Vector3.forward, false, balance.ShotDamage(false))); }

        [Test]
        public void BalanceShoveIsSixTenthsOfAShot() => Assert.That(balance.shoveDamageMultiplier, Is.EqualTo(0.6f));

        [Test]
        public void SixShovesLeaveElAltoAlive()
        {
            Assert.That(alto.ShotsToKill, Is.EqualTo(4));
            Shove(6);
            Assert.That(alto.IsAlive, Is.True);
            Assert.That(alto.DamageTaken, Is.EqualTo(3.6f).Within(0.0001f));
        }

        [Test]
        public void SevenShovesKillElAlto()
        {
            int died = 0;
            alto.Died += () => died++;
            Shove(7);
            Assert.That(alto.IsAlive, Is.False);
            Assert.That(died, Is.EqualTo(1), "a shove kill follows the same Died flow as a shot kill");
        }

        [Test]
        public void ShoveAndShotsAddUp()
        {
            Shove(1);
            Shoot(3);
            Assert.That(alto.IsAlive, Is.True, "1 shove + 3 shots = 3.6 of 4");
            Shoot(1);
            Assert.That(alto.IsAlive, Is.False);
        }

        [Test]
        public void ShoveRaisesNoShotEvent()
        {
            int shots = 0;
            alto.Shot += _ => shots++;
            Shove(7);
            Assert.That(shots, Is.EqualTo(0), "no hit flash, splatter or brain reaction to a shot");
        }

        [Test]
        public void ShoveOnDeadEnemyDoesNothing()
        {
            int died = 0;
            alto.Died += () => died++;
            Shoot(4);
            Shove(2);
            Assert.That(died, Is.EqualTo(1));
            Assert.That(alto.DamageTaken, Is.EqualTo(4f).Within(0.0001f));
        }
    }
}
