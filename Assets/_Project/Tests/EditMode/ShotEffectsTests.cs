using Heartbeat.Combat;
using Heartbeat.Render;
using NUnit.Framework;
using UnityEngine;

namespace Heartbeat.Tests.EditMode
{
    /// <summary>
    /// Shot effects (C-71): the pool caps each prefab and recycles the oldest instance, a bullet mark fades out over
    /// its lifetime, and only a hit on something without <see cref="IDamageable"/> shows the impact.
    /// </summary>
    public class ShotEffectsTests
    {
        private GameObject root, prefab;

        private sealed class Target : MonoBehaviour, IDamageable
        {
            public bool alive = true;
            public bool IsAlive => alive;
            public void ReceiveShot(in ShotHit hit) { }
        }

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("ShotEffectsTest");
            prefab = new GameObject("VFX_Test");
            prefab.transform.SetParent(root.transform);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        [Test]
        public void PoolCapsAndRecyclesOldest()
        {
            var pool = new VfxPool(root.transform);
            var first = pool.Spawn(prefab, 20, Vector3.zero, Quaternion.identity, 10f);
            for (int i = 1; i < 20; i++) pool.Spawn(prefab, 20, Vector3.right * i, Quaternion.identity, 10f);
            Assert.That(pool.ActiveCount(prefab), Is.EqualTo(20));

            var reused = pool.Spawn(prefab, 20, Vector3.up * 5f, Quaternion.identity, 10f);
            Assert.That(reused, Is.SameAs(first));
            Assert.That(reused.transform.position, Is.EqualTo(Vector3.up * 5f));
            Assert.That(pool.ActiveCount(prefab), Is.EqualTo(20));
        }

        [Test]
        public void PoolRebuildsInstanceWhoseParentWasDestroyed()
        {
            var pool = new VfxPool(root.transform);
            var parent = new GameObject("Muzzle");
            var a = pool.Spawn(prefab, 1, Vector3.zero, Quaternion.identity, 1f, parent.transform);
            Object.DestroyImmediate(parent);
            Assert.That(a == null, Is.True);
            var b = pool.Spawn(prefab, 1, Vector3.zero, Quaternion.identity, 1f);
            Assert.That(b != null && b.gameObject.activeSelf, Is.True);
        }

        [Test]
        public void ClearHidesEverything()
        {
            var pool = new VfxPool(root.transform);
            for (int i = 0; i < 3; i++) pool.Spawn(prefab, 20, Vector3.zero, Quaternion.identity, 10f);
            pool.Clear();
            Assert.That(pool.ActiveCount(prefab), Is.Zero);
        }

        [Test]
        public void MarkFadesOutOverItsLifetime()
        {
            Assert.That(VfxLifetime.Alpha(0f, 10f), Is.EqualTo(1f));
            Assert.That(VfxLifetime.Alpha(5f, 10f), Is.InRange(0.5f, 1f));
            Assert.That(VfxLifetime.Alpha(10f, 10f), Is.EqualTo(0f));
        }

        [Test]
        public void ImpactOnlyOnSurfacesWithoutDamageable()
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.SetParent(root.transform);
            Assert.That(ShotEffects.ShowsImpact(wall.GetComponent<Collider>()), Is.True);

            var enemy = new GameObject("Enemy", typeof(Target));
            enemy.transform.SetParent(root.transform);
            var hitbox = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            hitbox.transform.SetParent(enemy.transform);
            var collider = hitbox.GetComponent<Collider>();
            Assert.That(ShotEffects.ShowsImpact(collider), Is.False, "live enemy");
            enemy.GetComponent<Target>().alive = false;
            Assert.That(ShotEffects.ShowsImpact(collider), Is.False, "dead enemy");
        }
    }
}
