using Heartbeat.Level;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class HackSequenceTests
    {
        [Test]
        public void WrongInput_RestartsSequence() // acceptance M-13
        {
            var s = new HackSequence(new[] { HackDir.Up, HackDir.Left, HackDir.Down, HackDir.Right });
            Assert.AreEqual(HackStep.Correct, s.Input(HackDir.Up));
            Assert.AreEqual(HackStep.Correct, s.Input(HackDir.Left));
            Assert.AreEqual(HackStep.Error, s.Input(HackDir.Up));
            Assert.AreEqual(0, s.Progress);
            Assert.AreEqual(1, s.Errors);
        }

        [Test]
        public void CorrectChain_Completes()
        {
            var s = new HackSequence(new[] { HackDir.Down, HackDir.Down });
            s.Input(HackDir.Down);
            Assert.AreEqual(HackStep.Complete, s.Input(HackDir.Down));
            Assert.IsTrue(s.IsComplete);
        }

        [TestCase(HackDifficulty.QuickSeal, 2, 3)]
        [TestCase(HackDifficulty.Door, 4, 5)]
        [TestCase(HackDifficulty.Main, 7, 8)]
        public void Lengths_MatchGdd(HackDifficulty d, int min, int max)
        {
            var rng = new System.Random(1);
            for (int i = 0; i < 200; i++)
            {
                int n = HackSequence.Generate(d, rng).Steps.Count;
                Assert.That(n, Is.InRange(min, max));
            }
        }
    }
}
