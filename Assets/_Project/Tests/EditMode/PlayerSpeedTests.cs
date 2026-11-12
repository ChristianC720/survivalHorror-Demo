using System.Linq;
using Heartbeat.AI;
using Heartbeat.Audio;
using Heartbeat.Player;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>Lukas's speeds live in three assets that must agree: PlayerBalance, AC_Lukas's blend trees and AudioBalance.</summary>
    public sealed class PlayerSpeedTests
    {
        private const string Balance = "Assets/_Project/ScriptableObjects/Balance/";
        private static PlayerBalance Player => AssetDatabase.LoadAssetAtPath<PlayerBalance>(Balance + "PlayerBalance.asset");

        private static BlendTree Tree(string name) =>
            AssetDatabase.LoadAllAssetsAtPath("Assets/_Project/Models/Characters/AC_Lukas.controller").OfType<BlendTree>().Single(t => t.name == name);

        [TestCase("Fine", 1f)]
        [TestCase("Injured", -1f)]
        [TestCase("Critical", -2f)]
        public void LocomotionTreeThresholdsMatchBalance(string tree, float which)
        {
            var b = Player;
            float mult = which > 0f ? 1f : which > -1.5f ? b.injuredSpeedMultiplier : b.criticalSpeedMultiplier;
            var motions = Tree(tree).children;
            Assert.AreEqual(b.walkSpeed * mult, motions[1].threshold, 0.01f, "walk");
            Assert.AreEqual(b.runSpeed * mult, motions[2].threshold, 0.01f, "run");
        }

        [Test]
        public void CrouchTreeThresholdMatchesBalance() =>
            Assert.AreEqual(Player.crouchSpeed, Tree("Crouch").children[1].threshold, 0.01f);

        [Test]
        public void CrouchedStepsStaySilent() =>   // crouch = 0 noise: FootstepEmitter mutes below minStepSpeed
            Assert.Less(Player.crouchSpeed, AssetDatabase.LoadAssetAtPath<AudioBalance>(Balance + "AudioBalance.asset").minStepSpeed);

        [TestCase("Alto")]
        [TestCase("Rastrero")]
        public void HunterCatchesLukasOnceStaminaIsGone(string enemy)
        {
            var def = AssetDatabase.LoadAssetAtPath<EnemyDefinition>($"Assets/_Project/ScriptableObjects/Enemies/ED_{enemy}.asset");
            Assert.Greater(def.huntSpeed, Player.walkSpeed);
        }
    }
}
