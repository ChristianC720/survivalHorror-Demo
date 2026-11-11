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

        [Test]
        public void GermanKey_MatchesTheDeTable() // C-18
        {
            Assert.AreEqual("DE.ORDNUNG_IST_DAS_HALBE_LEBEN", DocText.GermanKey("Ordnung ist das halbe Leben."));
            Assert.AreEqual("DE.KAMERAUEBERWACHUNG_AKTIV", DocText.GermanKey("Kameraüberwachung aktiv."));
            Assert.AreEqual("DE.NICHTS_DARF_ZURUECKBLEIBEN", DocText.GermanKey("Nichts darf zurückbleiben."));
        }

        [Test]
        public void GermanLines_KeepGerman_AndGetASubtitle() // C-18
        {
            string raw = "Do not hum.\n<i>Ordnung ist das halbe Leben.</i>\n<i>(breathing)</i> end";
            string s = DocText.WithGermanSubtitles(raw, de => de.StartsWith("Ordnung") ? "Order is half of life." : null);
            StringAssert.Contains("<i>Ordnung ist das halbe Leben.</i>\n<size=75%>", s);
            StringAssert.Contains("Order is half of life.", s);
            StringAssert.Contains("<i>(breathing)</i> end", s);
        }
    }
}
