using Heartbeat.Audio;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class MixMathTests
    {
        [Test]
        public void LinearToDb_Ends()
        {
            Assert.AreEqual(0f, MixMath.LinearToDb(1f), 0.001f);
            Assert.AreEqual(MixMath.SilentDb, MixMath.LinearToDb(0f));
            Assert.AreEqual(-6.02f, MixMath.LinearToDb(0.5f), 0.01f);
        }

        [Test]
        public void Duck_HoldsThenReleases()
        {
            Assert.AreEqual(-8f, MixMath.Duck(-0.1f, -8f, 1.5f));
            Assert.AreEqual(-4f, MixMath.Duck(0.75f, -8f, 1.5f), 0.001f);
            Assert.AreEqual(0f, MixMath.Duck(2f, -8f, 1.5f));
        }
    }
}
