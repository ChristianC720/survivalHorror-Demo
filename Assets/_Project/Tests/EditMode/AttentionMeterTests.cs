using Heartbeat.CameraSystem;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class AttentionMeterTests
    {
        [Test]
        public void FillsInFiveSecondsOfViewing_ThenFiresOnce() // D-24
        {
            var m = new AttentionMeter();
            int fired = 0;
            m.Full += () => fired++;
            for (int i = 0; i < 49; i++) m.Tick(0.1f, true);
            Assert.AreEqual(0, fired);
            for (int i = 0; i < 2; i++) m.Tick(0.1f, true);
            Assert.AreEqual(1, fired);
            Assert.Less(m.Value, 0.1f);
        }

        [Test]
        public void DrainsInFifteenSeconds_WhenClosed()
        {
            var m = new AttentionMeter();
            for (int i = 0; i < 40; i++) m.Tick(0.1f, true);   // 0.8
            for (int i = 0; i < 121; i++) m.Tick(0.1f, false); // 12.1 s
            Assert.AreEqual(0f, m.Value, 0.001f);
        }

        [Test]
        public void ShotsAddAttention()
        {
            var m = new AttentionMeter();
            m.Add(0.15f);
            Assert.AreEqual(0.15f, m.Value, 0.0001f);
        }
    }
}
