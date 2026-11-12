using System.Linq;
using Heartbeat.Combat;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>
    /// Hitboxes must ride the skeleton (C-63): one per weighted bone, children of the bone, on the Hitbox layer, solid,
    /// with the weak point on the zone the GDD names. Coverage itself is EnemyHitboxCoverageTests.
    /// </summary>
    public class EnemyHitboxTests
    {
        // GDD Enemigos: El Alto the bent head (and its jaw), La Boca the open mouth between the two head halves,
        // El Mudo the torso, El Rastrero the head with the hanging jaw.
        [TestCase("Alto", "head,jaw"), TestCase("Boca", "head_L,head_R"), TestCase("Mudo", "chest"), TestCase("Rastrero", "Head")]
        public void WeakPointAndBodyFollowTheBones(string enemy, string weakBones)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Characters/Enemy_{enemy}.prefab");
            var smr = prefab.GetComponentInChildren<SkinnedMeshRenderer>(true);
            int layer = LayerMask.NameToLayer("Hitbox");
            var hitboxes = prefab.GetComponentsInChildren<Collider>(true).Where(c => c.gameObject != prefab).ToArray();

            Assert.That(hitboxes.All(c => c.gameObject.layer == layer && !c.isTrigger), "every child collider is a solid Hitbox");
            Assert.That(hitboxes.All(c => smr.bones.Contains(c.transform.parent)), "every hitbox is a child of a bone");
            Assert.That(hitboxes.OfType<MeshCollider>().All(m => m.convex && m.sharedMesh != null), "hull hitboxes are convex");
            Assert.That(hitboxes.Length, Is.GreaterThanOrEqualTo(20), "head, neck, torso, pelvis, arms, hands, legs and feet");

            var weak = prefab.GetComponentsInChildren<WeakPoint>(true).Select(w => w.transform.parent.name).OrderBy(n => n);
            Assert.That(weak, Is.EqualTo(weakBones.Split(',').OrderBy(n => n)));
        }
    }
}
