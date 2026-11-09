using System.Linq;
using Heartbeat.Combat;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>Hitboxes must ride the skeleton: a shot aimed at the animated head has to find the weak point.</summary>
    public class EnemyHitboxTests
    {
        [TestCase("Alto"), TestCase("Boca"), TestCase("Mudo"), TestCase("Rastrero")]
        public void WeakPointAndBodyFollowTheBones(string enemy)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Characters/Enemy_{enemy}.prefab");
            var weak = prefab.GetComponentInChildren<WeakPoint>(true);
            Assert.That(weak.transform.parent.name.ToLowerInvariant(), Is.EqualTo("head"));
            var body = prefab.GetComponentsInChildren<CapsuleCollider>(true).Where(c => c.name.StartsWith("Hit_")).ToArray();
            Assert.That(body.Length, Is.GreaterThanOrEqualTo(11));
            int layer = LayerMask.NameToLayer("Hitbox");
            Assert.That(body.All(c => c.gameObject.layer == layer && !c.isTrigger));
        }
    }
}
