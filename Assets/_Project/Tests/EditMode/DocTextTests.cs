using Heartbeat.Narrative;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class DocTextTests
    {
        [Test]
        public void Corrupt_HidesMarkedPassage_KeepsTheRest() // D-127
        {
            string r = DocText.Render("Tempo 41 bpm. <c>Lukas made tea.</c> End.", reveal: false);
            StringAssert.StartsWith("Tempo 41 bpm. <color=", r);
            StringAssert.DoesNotContain("Lukas", r);
            StringAssert.EndsWith(" End.", r);
        }

        [Test]
        public void Reveal_ShowsCleanText() // D-55
        {
            Assert.AreEqual("A Lukas B", DocText.Render("A <c>Lukas</c> B", reveal: true));
        }

        [Test]
        public void Glitch_IsDeterministic()
        {
            Assert.AreEqual(DocText.Render("<c>Emilia</c>", false), DocText.Render("<c>Emilia</c>", false));
        }
    }
}
