using Heartbeat.Player;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>The arm terminal sits on the forearm whatever the rig's rest pose.</summary>
    public class LukasTerminalPanelTests
    {
        private static Vector3 Panel(string fbx)
        {
            var go = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Models/Characters/{fbx}.fbx"));
            try
            {
                var a = go.GetComponent<Animator>();
                return LukasAttachments.TerminalPanel(go.transform, a.GetBoneTransform(HumanBodyBones.LeftLowerArm), a.GetBoneTransform(HumanBodyBones.LeftHand), out _, out _);
            }
            finally { Object.DestroyImmediate(go); }
        }

        [Test]
        public void FirstRig_KeepsTheMeasuredPanel() =>
            Assert.That(Vector3.Distance(Panel("CH_Lukas_RH07_LegacyAnims"), new Vector3(-0.355f, 0.99f, -0.045f)), Is.LessThan(0.01f));

        [Test]
        public void NewRig_PanelStaysOnTheForearm()
        {
            var go = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Models/Characters/CH_Lukas_RH07.fbx"));
            var a = go.GetComponent<Animator>();
            Vector3 elbow = a.GetBoneTransform(HumanBodyBones.LeftLowerArm).position, wrist = a.GetBoneTransform(HumanBodyBones.LeftHand).position;
            Object.DestroyImmediate(go);
            Vector3 p = Panel("CH_Lukas_RH07"), seg = wrist - elbow;
            float k = Mathf.Clamp01(Vector3.Dot(p - elbow, seg) / seg.sqrMagnitude);
            Assert.That(Vector3.Distance(p, elbow + seg * k), Is.LessThan(0.06f));
        }
    }
}
