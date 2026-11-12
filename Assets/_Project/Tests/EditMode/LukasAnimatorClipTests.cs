using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>C-60: overwriting the Lukas FBX must leave every state of his controllers with a clip.</summary>
    public class LukasAnimatorClipTests
    {
        private const string Dir = "Assets/_Project/Models/Characters/";

        private static IEnumerable<(string state, AnimationClip clip)> Clips(string state, Motion motion)
        {
            if (motion is BlendTree tree)
                return tree.children.SelectMany(c => Clips(state, c.motion));
            return new[] { (state, motion as AnimationClip) };
        }

        [TestCase("AC_Lukas")]
        [TestCase("AC_Cine_LukasIdle")]
        [TestCase("AC_Cine_LukasTerminal")]
        public void EveryState_HasAHumanoidClip(string controller)
        {
            var ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(Dir + controller + ".controller");
            var states = ac.layers.SelectMany(l => l.stateMachine.states).Select(s => s.state).Where(s => s.name != "Empty").ToList();
            Assert.That(states, Is.Not.Empty);
            foreach (var (state, clip) in states.SelectMany(s => Clips(s.name, s.motion)))
            {
                Assert.That(clip, Is.Not.Null, state + " has no clip");
                Assert.That(clip.isHumanMotion && !clip.empty && clip.length > 0.1f, state + " plays an unusable clip: " + clip.name);
            }
        }

        [Test]
        public void TimedStates_UseTheClipNamedAfterThem()
        {
            // PlayerAnimator.LengthOf finds a gesture's length by the clip name "AN_Lukas_<state>".
            var ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(Dir + "AC_Lukas.controller");
            foreach (var layer in ac.layers.Where(l => l.name == "Upper" || l.name == "Full"))
                foreach (var s in layer.stateMachine.states.Select(s => s.state).Where(s => s.name != "Empty"))
                    Assert.That(s.motion.name, Is.EqualTo("AN_Lukas_" + s.name), layer.name + "/" + s.name);
        }
    }
}
