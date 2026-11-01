using Heartbeat.Boss;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class MotherFightTests
    {
        private static MotherFight ToCameras()
        {
            var f = new MotherFight();
            f.NodeHacked(); f.NodeHacked(); f.NodeHacked();
            return f;
        }

        [Test]
        public void ThreeNodes_StartPhase2()
        {
            var f = new MotherFight();
            f.NodeHacked(); f.NodeHacked();
            Assert.AreEqual(MotherPhase.Nodes, f.Phase);
            f.NodeHacked();
            Assert.AreEqual(MotherPhase.Cameras, f.Phase);
        }

        [Test]
        public void Releases_AreCappedAtTwoActive_AndFourTotal() // D-115
        {
            var f = ToCameras();
            Assert.IsTrue(f.TryRelease());
            Assert.IsTrue(f.TryRelease());
            Assert.IsFalse(f.TryRelease(), "max 2 active");
            f.AssimilatedKilled();
            Assert.IsTrue(f.TryRelease());
            f.AssimilatedKilled(); f.AssimilatedKilled();
            Assert.IsTrue(f.TryRelease());
            Assert.IsFalse(f.TryRelease(), "4 total");
        }

        [Test]
        public void Phase2_NeedsAllKilledAndCore()
        {
            var f = ToCameras();
            for (int i = 0; i < 4; i++) { if (f.Active >= 2) f.AssimilatedKilled(); f.TryRelease(); }
            while (f.Killed < 4) f.AssimilatedKilled();
            Assert.AreEqual(MotherPhase.Cameras, f.Phase, "core not reached yet");
            f.ReachedCore();
            Assert.AreEqual(MotherPhase.Transformers, f.Phase);
        }

        [Test]
        public void ReachingCoreEarly_ReleasesTheRest_NeverBlocks() // D-117
        {
            var f = ToCameras();
            f.ReachedCore();
            Assert.AreEqual(2, f.PendingAtCore);
            f.TryRelease(); f.TryRelease();
            Assert.AreEqual(0, f.PendingAtCore);
            f.AssimilatedKilled(); f.AssimilatedKilled();
            Assert.AreEqual(2, f.PendingAtCore);
        }

        [Test]
        public void ThreeTransformers_Defeat()
        {
            var f = ToCameras();
            f.ReachedCore();
            for (int i = 0; i < 4; i++) { f.TryRelease(); f.AssimilatedKilled(); }
            Assert.AreEqual(MotherPhase.Transformers, f.Phase);
            f.TransformerDestroyed(); f.TransformerDestroyed();
            Assert.AreEqual(MotherPhase.Transformers, f.Phase);
            f.TransformerDestroyed();
            Assert.AreEqual(MotherPhase.Defeated, f.Phase);
        }
    }
}
