using Heartbeat.Combat;
using Heartbeat.Player;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class ArmGrabTests
    {
        [Test]
        public void CollectorArm_SparesOnceInCritical_ThenKills() // D-169
        {
            bool warned = false;
            Assert.IsTrue(PlayerDamageReceiver.Spares(ref warned, true, HealthState.Injured));
            Assert.IsFalse(warned);
            Assert.IsTrue(PlayerDamageReceiver.Spares(ref warned, true, HealthState.Critical));
            Assert.IsFalse(PlayerDamageReceiver.Spares(ref warned, true, HealthState.Critical));
        }

        private bool dummy;
        [Test] public void OtherGrabs_NeverSpare() => Assert.IsFalse(PlayerDamageReceiver.Spares(ref dummy, false, HealthState.Critical));
    }
}
