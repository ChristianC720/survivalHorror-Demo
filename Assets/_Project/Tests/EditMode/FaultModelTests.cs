using System.Linq;
using Heartbeat.Level;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class FaultModelTests
    {
        private const string Door = "FAULT.DOOR_BACKDOOR", Mixer = "FAULT.MIXER_BYPASS";

        private static FaultModel New() => new FaultModel(2, 2, 0.5f);

        [Test]
        public void UndiscoveredFault_IsNotUsable()
        {
            Assert.IsNull(New().UsableFor(HackDifficulty.Door, _ => false));
        }

        [Test]
        public void DiscoveredFault_OnlyMatchesItsHackKind()
        {
            var m = New();
            Assert.AreEqual(Door, m.UsableFor(HackDifficulty.Door, id => id == Door));
            Assert.IsNull(m.UsableFor(HackDifficulty.Main, id => id == Door));
            Assert.IsNull(m.UsableFor(HackDifficulty.QuickSeal, _ => true));
        }

        [Test]
        public void PatchedAfterTwoUses_AndPersistsThroughExport() // M-17 / D-09
        {
            var m = New();
            m.Use(Door);
            Assert.AreEqual(1, m.UsesLeft(Door));
            m.Use(Door);
            Assert.IsTrue(m.IsPatched(Door));
            Assert.IsNull(m.UsableFor(HackDifficulty.Door, _ => true));

            var reloaded = New();
            reloaded.Import(m.Export());
            Assert.IsTrue(reloaded.IsPatched(Door));
        }

        [Test]
        public void Death_PatchesEveryDiscoveredFault_OnlyThose()
        {
            var m = New();
            m.PatchDiscovered(id => id == Mixer);
            Assert.IsTrue(m.IsPatched(Mixer));
            Assert.IsFalse(m.IsPatched(Door));
            CollectionAssert.AreEquivalent(new[] { Mixer }, m.Export().ToArray());
        }

        [Test]
        public void ShortLengths()
        {
            var m = New();
            Assert.AreEqual(2, m.ShortLength(HackDifficulty.Door, 5));
            Assert.AreEqual(4, m.ShortLength(HackDifficulty.Main, 7));
            Assert.AreEqual(4, m.ShortLength(HackDifficulty.Main, 8));
        }

        [Test]
        public void Truncate_KeepsAtLeastOneInputToEnter()
        {
            var s = new HackSequence(new[] { HackDir.Up, HackDir.Up, HackDir.Down, HackDir.Left, HackDir.Right });
            s.Input(HackDir.Up); s.Input(HackDir.Up); s.Input(HackDir.Down);
            s.Truncate(2);
            Assert.AreEqual(4, s.Steps.Count);
            Assert.IsFalse(s.IsComplete);
            Assert.AreEqual(HackStep.Complete, s.Input(HackDir.Left));
        }
    }
}
