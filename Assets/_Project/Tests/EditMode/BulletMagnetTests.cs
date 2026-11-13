using Heartbeat.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Heartbeat.Tests.EditMode
{
    /// <summary>
    /// Light bullet magnet (C-70): a miss within 0.25 m of a live enemy hitbox and 2 degrees of the ray bends to it,
    /// never through a wall, and a straight hit never changes. Distances are from the hitbox surface.
    /// </summary>
    public class BulletMagnetTests
    {
        private const float Radius = 0.25f, MaxDegrees = 2f, Range = 60f;
        private GameObject root;
        private int mask, hitboxLayer;

        private sealed class Target : MonoBehaviour, IDamageable
        {
            public bool alive = true;
            public bool IsAlive => alive;
            public void ReceiveShot(in ShotHit hit) { }
        }

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("BulletMagnetTest");
            mask = LayerMask.GetMask("Environment", "Hitbox");
            hitboxLayer = LayerMask.NameToLayer("Hitbox");
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        /// <summary>A 0.5 m cube hitbox whose left face is <paramref name="gap"/> metres right of the ray at <paramref name="z"/>.</summary>
        private Target Enemy(float z, float gap)
        {
            var enemy = new GameObject("Enemy", typeof(Target));
            enemy.transform.SetParent(root.transform);
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.layer = hitboxLayer;
            box.transform.SetParent(enemy.transform);
            box.transform.localScale = Vector3.one * 0.5f;
            box.transform.position = new Vector3(gap + 0.25f, 0f, z);
            return enemy.GetComponent<Target>();
        }

        private bool Shoot(out RaycastHit hit)
        {
            Physics.SyncTransforms();   // auto-sync is off in this project
            return BulletMagnet.Resolve(Vector3.zero, Vector3.forward, Vector3.zero, Range, Radius, MaxDegrees, mask, hitboxLayer,
                out hit, out _);
        }

        [Test]
        public void MissByPointTwoMetresHits()
        {
            var enemy = Enemy(10f, 0.2f);
            Assert.That(Shoot(out var hit), Is.True);
            Assert.That(hit.collider.GetComponentInParent<IDamageable>(), Is.SameAs(enemy));
        }

        [Test]
        public void MissByPointFourMetresMisses()
        {
            Enemy(10f, 0.4f);
            Assert.That(Shoot(out _), Is.False);
        }

        [Test]
        public void WallBetweenChestAndEnemyMisses()
        {
            Enemy(10f, 0.2f);
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);   // covers the enemy, not the straight ray
            wall.layer = LayerMask.NameToLayer("Environment");
            wall.transform.SetParent(root.transform);
            wall.transform.localScale = new Vector3(1f, 1f, 0.2f);
            wall.transform.position = new Vector3(0.6f, 0f, 8f);   // x 0.1 to 1.1: clear of the straight ray, across the bent one
            Assert.That(Shoot(out _), Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WallInFrontOfTheCameraStopsTheSearch(bool wall)
        {
            // The chest sits left of the camera, so its ray to the enemy clears a wall that only the camera sees.
            Enemy(10f, 0.2f);
            if (wall)
            {
                var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
                w.layer = LayerMask.NameToLayer("Environment");
                w.transform.SetParent(root.transform);
                w.transform.localScale = new Vector3(0.4f, 1f, 0.2f);
                w.transform.position = new Vector3(0.15f, 0f, 5f);
            }
            Physics.SyncTransforms();
            Assert.That(BulletMagnet.Resolve(Vector3.zero, Vector3.forward, new Vector3(-0.6f, 0f, 0f), Range, Radius, MaxDegrees,
                mask, hitboxLayer, out var hit, out _), Is.True);
            Assert.That(hit.collider.gameObject.layer, Is.EqualTo(wall ? LayerMask.NameToLayer("Environment") : hitboxLayer));
        }

        [Test]
        public void BendLargerThanTwoDegreesMisses()
        {
            Enemy(3f, 0.2f);   // 0.2 m at 3 m is 3.8 degrees
            Assert.That(Shoot(out _), Is.False);
        }

        [Test]
        public void DeadEnemyDoesNotPull()
        {
            Enemy(10f, 0.2f).alive = false;
            Assert.That(Shoot(out _), Is.False);
        }

        [Test]
        public void StraightHitIsUnchanged()
        {
            Enemy(10f, 0.2f);
            var straight = Enemy(12f, -0.25f);   // centred on the ray
            Assert.That(Shoot(out var hit), Is.True);
            Assert.That(hit.collider.GetComponentInParent<IDamageable>(), Is.SameAs(straight));
        }

        [Test]
        public void NearestHitboxToTheRayWins()
        {
            Enemy(10f, 0.2f);
            var closer = Enemy(15f, 0.1f);
            Assert.That(Shoot(out var hit), Is.True);
            Assert.That(hit.collider.GetComponentInParent<IDamageable>(), Is.SameAs(closer));
        }
    }
}
