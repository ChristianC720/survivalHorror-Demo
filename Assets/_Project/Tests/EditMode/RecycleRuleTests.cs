using Heartbeat.Recycle;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class RecycleRuleTests
    {
        [Test] public void BeforeRiseTime_NeverRises() => Assert.IsFalse(RecycleSystem.ShouldRise(39f, false, 40f, 20f));
        [Test] public void OutOfView_RisesOnTime() => Assert.IsTrue(RecycleSystem.ShouldRise(40f, false, 40f, 20f));
        [Test] public void Seen_Waits() => Assert.IsFalse(RecycleSystem.ShouldRise(55f, true, 40f, 20f));   // D-166
        [Test] public void Seen_RisesAfterMaxWait() => Assert.IsTrue(RecycleSystem.ShouldRise(60f, true, 40f, 20f));
    }
}
