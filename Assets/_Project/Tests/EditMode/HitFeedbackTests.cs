using Heartbeat.Core;
using Heartbeat.Render;
using Heartbeat.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests.EditMode
{
    /// <summary>
    /// Hit feedback on enemies (C-72): the reticle marker grows in and shrinks out, each kind of hit has its own cue, and
    /// a shot enemy flashes and then gets its own materials back.
    /// </summary>
    public class HitFeedbackTests
    {
        private GameObject root;
        private Material own, flash;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("HitFlashTest");
            own = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            flash = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(own);
            Object.DestroyImmediate(flash);
        }

        [Test]
        public void MarkerGrowsInHoldsAndShrinksOut()
        {
            ReticleFeedback.MarkerPose(0f, out float s0, out float a0);
            ReticleFeedback.MarkerPose(0.4f, out float s1, out float a1);
            ReticleFeedback.MarkerPose(0.99f, out float s2, out float a2);
            Assert.That(s0, Is.LessThan(s1), "grows when it appears");
            Assert.That((s1, a1), Is.EqualTo((1f, 1f)));
            Assert.That(a0, Is.EqualTo(1f));
            Assert.That(s2, Is.LessThan(s1), "shrinks while it fades");
            Assert.That(a2, Is.LessThan(0.05f));
        }

        [Test]
        public void EachHitKindHasItsOwnCue()
        {
            Assert.That(Sfx.HitCue(false, false), Is.EqualTo(Sfx.HitBody));
            Assert.That(Sfx.HitCue(true, false), Is.EqualTo(Sfx.HitWeakPoint));
            Assert.That(Sfx.HitCue(true, true), Is.EqualTo(Sfx.HitKill));
            Assert.That(Sfx.HitCue(false, true), Is.EqualTo(Sfx.HitKill));
        }

        [Test]
        public void FlashSwapsMaterialsAndStopRestoresThem()
        {
            var body = Mesh(root, 2);
            var limb = Mesh(new GameObject("Limb"), 1);
            limb.transform.SetParent(root.transform);
            var outlined = Mesh(new GameObject("Hull"), 1);
            outlined.transform.SetParent(root.transform);
            var other = new MaterialPropertyBlock();
            other.SetColor("_BaseColor", Color.green);
            outlined.SetPropertyBlock(other);

            var hit = Flash();
            hit.Play(0.1f);
            Assert.That(hit.Flashing, Is.True);
            Assert.That(body.sharedMaterials, Is.EqualTo(new[] { flash, flash }));
            Assert.That(limb.sharedMaterial, Is.SameAs(flash));
            Assert.That(outlined.sharedMaterial, Is.SameAs(own), "a renderer someone else drives is left alone");

            hit.Play(0.1f);   // a second shot mid-flash must not save the flash as the original
            hit.Stop();
            Assert.That(hit.Flashing, Is.False);
            Assert.That(body.sharedMaterials, Is.EqualTo(new[] { own, own }));
            Assert.That(limb.sharedMaterial, Is.SameAs(own));
            Assert.That(body.HasPropertyBlock(), Is.False);
            Assert.That(outlined.HasPropertyBlock(), Is.True);
        }

        [Test]
        public void ReducedFlashIsDimmer()
        {
            var body = Mesh(root, 1);
            var hit = Flash();
            hit.Play(0.1f, reduced: true);
            var block = new MaterialPropertyBlock();
            body.GetPropertyBlock(block);
            Assert.That(block.GetColor("_BaseColor").r, Is.LessThan(0.5f));
            hit.Stop();
        }

        private HitFlash Flash()
        {
            var hit = root.AddComponent<HitFlash>();
            var so = new SerializedObject(hit);
            so.FindProperty("flashMaterial").objectReferenceValue = flash;
            so.ApplyModifiedPropertiesWithoutUndo();
            return hit;
        }

        private MeshRenderer Mesh(GameObject go, int slots)
        {
            var r = go.AddComponent<MeshRenderer>();
            var mats = new Material[slots];
            for (int i = 0; i < slots; i++) mats[i] = own;
            r.sharedMaterials = mats;
            return r;
        }
    }
}
