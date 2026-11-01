using Heartbeat.Combat;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class RevolverModelTests
    {
        private int ammo;
        private bool Take() { if (ammo <= 0) return false; ammo--; return true; }

        private static RevolverModel New(int loaded = 0) => new RevolverModel(6, 0.8f, 0.7f, 1f, 0.6f, loaded);

        [Test]
        public void StartsUnloaded_DryFires()
        {
            var r = New();
            Assert.AreEqual(FireResult.DryFire, r.TryFire());
        }

        [Test]
        public void Reload_LoadsOneRoundEvery07s_UntilFull()
        {
            ammo = 16;
            var r = New();
            Assert.IsTrue(r.BeginReload(true));
            r.Tick(0.69f, Take, false);
            Assert.AreEqual(0, r.Loaded);
            r.Tick(0.02f, Take, false);
            Assert.AreEqual(1, r.Loaded);
            for (int i = 0; i < 60; i++) r.Tick(0.1f, Take, false);
            Assert.AreEqual(6, r.Loaded);
            Assert.IsFalse(r.IsReloading);
            Assert.AreEqual(10, ammo);
        }

        [Test]
        public void CancelReload_KeepsLoadedRounds()
        {
            ammo = 12;
            var r = New();
            r.BeginReload(true);
            r.Tick(1.5f, Take, false); // 2 rounds
            r.CancelReload();
            Assert.AreEqual(2, r.Loaded);
            Assert.IsFalse(r.IsReloading);
        }

        [Test]
        public void CannotFireWhileReloading()
        {
            ammo = 12;
            var r = New(3);
            r.BeginReload(true);
            Assert.AreEqual(FireResult.Reloading, r.TryFire());
        }

        [Test]
        public void ShotInterval_IsEnforced()
        {
            var r = New(6);
            Assert.AreEqual(FireResult.Fired, r.TryFire());
            r.Tick(0.5f, Take, false);
            Assert.AreEqual(FireResult.Cooldown, r.TryFire());
            r.Tick(0.31f, Take, false);
            Assert.AreEqual(FireResult.Fired, r.TryFire());
            Assert.AreEqual(4, r.Loaded);
        }

        [Test]
        public void FourShotsOnAlto_LeaveTwo() // acceptance M-04 counter part
        {
            var r = New(6);
            for (int i = 0; i < 4; i++) { r.TryFire(); r.Tick(0.8f, Take, false); }
            Assert.AreEqual(2, r.Loaded);
        }

        [Test]
        public void Spread_ClosesInOneSecondStill_OpensWhenMoving()
        {
            var r = New(6);
            r.TryFire();
            Assert.Greater(r.Spread, 0f);
            r.Tick(1f, Take, false);
            Assert.AreEqual(0f, r.Spread, 0.001f);
            r.Tick(0.1f, Take, true);
            Assert.AreEqual(1f, r.Spread, 0.001f);
        }

        [Test]
        public void ReloadStopsWhenAmmoRunsOut()
        {
            ammo = 2;
            var r = New();
            r.BeginReload(true);
            r.Tick(5f, Take, false);
            Assert.AreEqual(2, r.Loaded);
            Assert.IsFalse(r.IsReloading);
        }
    }
}
