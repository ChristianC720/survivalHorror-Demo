using Heartbeat.Player;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class FlashlightModelTests
    {
        private static void Run(FlashlightModel f, float seconds) { for (float t = 0; t < seconds; t += 0.1f) f.Tick(0.1f); }

        [Test]
        public void Depletes_After120s_ThenLockedFor10s() // acceptance M-12
        {
            var f = new FlashlightModel();
            Assert.IsTrue(f.Toggle());
            Run(f, 119f);
            Assert.IsTrue(f.IsOn);
            bool depleted = false;
            f.Depleted += () => depleted = true;
            Run(f, 1.2f);
            Assert.IsTrue(depleted);
            Assert.IsFalse(f.IsOn);
            Assert.IsFalse(f.Toggle(), "F does not turn it on while locked");
            Run(f, 9f);
            Assert.IsTrue(f.IsLocked);
            Run(f, 1.2f);
            Assert.IsFalse(f.IsLocked);
        }

        [Test]
        public void Recharges_After3sOff_FullIn30s()
        {
            var f = new FlashlightModel();
            f.Toggle();
            Run(f, 60f);                       // 50%
            f.Toggle();                        // off
            Run(f, 2.9f);
            Assert.AreEqual(0.5f, f.Charge, 0.01f);
            Run(f, 15.2f);                     // +~50%
            Assert.AreEqual(1f, f.Charge, 0.02f);
        }

        [Test]
        public void DepletedLight_RechargesFrom0To100InAbout33s()
        {
            var f = new FlashlightModel();
            f.Toggle();
            Run(f, 121f);
            Run(f, 33.2f);
            Assert.AreEqual(1f, f.Charge, 0.01f);
        }
    }
}
