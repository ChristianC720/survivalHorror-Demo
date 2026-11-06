using Heartbeat.Player;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class StaminaModelTests
    {
        private static StaminaModel New() => new StaminaModel(regenDelay: 3f, regenPerSecond: 25f, exhaustionSeconds: 2f);

        [Test]
        public void Shove_At30Percent_Fails() // acceptance M-06
        {
            var s = New();
            s.TrySpend(70f);
            Assert.AreEqual(30f, s.Value, 0.001f);
            Assert.IsFalse(s.TrySpend(35f));
            Assert.AreEqual(30f, s.Value, 0.001f);
        }

        [Test]
        public void Regen_WaitsThreeSeconds_ThenFillsIn4s()
        {
            var s = New();
            s.TrySpend(100f);
            for (int i = 0; i < 29; i++) s.Tick(0.1f); // 2.9 s
            Assert.AreEqual(0f, s.Value, 0.001f);
            for (int i = 0; i < 41; i++) s.Tick(0.1f); // +4.1 s
            Assert.AreEqual(100f, s.Value, 0.001f);
        }

        [Test]
        public void ReachingZero_ExhaustsForTwoSeconds()
        {
            var s = New();
            bool fired = false;
            s.Exhausted += () => fired = true;
            s.Drain(10f, 10f);
            Assert.IsTrue(fired);
            Assert.IsTrue(s.IsExhausted);
            Assert.IsFalse(s.CanRun);
            s.Tick(1.9f);
            Assert.IsTrue(s.IsExhausted);
            s.Tick(0.2f);
            Assert.IsFalse(s.IsExhausted);
        }

        [Test]
        public void Running_Drains10PercentPerSecond()
        {
            var s = New();
            s.Drain(10f, 1f);
            Assert.AreEqual(90f, s.Value, 0.001f);
        }
    }

    public class HealthModelTests
    {
        [Test]
        public void AltoHit_FromFine_GoesInjured() // acceptance M-05
        {
            var h = new HealthModel(90f);
            h.ApplyHit(2);
            Assert.AreEqual(HealthState.Injured, h.State);
        }

        [TestCase(2), TestCase(3), TestCase(4)]
        public void HitsToKill_KillExactly(int hits)
        {
            var h = new HealthModel(90f);
            for (int i = 0; i < hits - 1; i++) h.ApplyHit(hits);
            Assert.IsFalse(h.IsDead);
            h.ApplyHit(hits);
            Assert.IsTrue(h.IsDead);
        }

        [Test]
        public void NotLethal_HitsAndGrabsStopAtCritical() // D-160 tutorial
        {
            var h = new HealthModel(90f) { Lethal = false };
            for (int i = 0; i < 6; i++) h.ApplyHit(2);
            h.ApplyGrabWithoutSpike();
            Assert.IsFalse(h.IsDead);
            Assert.AreEqual(HealthState.Critical, h.State);
            Assert.IsTrue(h.TryHeal());
            Assert.AreEqual(HealthState.Injured, h.State);
        }

        [Test]
        public void Rastrero_FourHits_StatesProgress()
        {
            var h = new HealthModel(90f);
            h.ApplyHit(4); // 67.5 Fine
            Assert.AreEqual(HealthState.Fine, h.State);
            h.ApplyHit(4); // 45 Injured
            Assert.AreEqual(HealthState.Injured, h.State);
            h.ApplyHit(4); // 22.5 Critical
            Assert.AreEqual(HealthState.Critical, h.State);
        }

        [Test]
        public void Heal_NotAllowedAtFull()
        {
            var h = new HealthModel(90f);
            Assert.IsFalse(h.TryHeal());
        }

        [Test]
        public void Heal_FineWithDamage_RestoresFull()
        {
            var h = new HealthModel(90f);
            h.ApplyHit(4);
            Assert.IsTrue(h.TryHeal());
            Assert.AreEqual(90f, h.Value, 0.001f);
        }

        [Test]
        public void Heal_Critical_GoesToTopOfInjured()
        {
            var h = new HealthModel(90f);
            h.ApplyHit(4); h.ApplyHit(4); h.ApplyHit(4);
            Assert.IsTrue(h.TryHeal());
            Assert.AreEqual(HealthState.Injured, h.State);
            Assert.AreEqual(60f, h.Value, 0.001f);
        }

        [Test]
        public void Grab_WithoutSpike_DropsOneState_AndKillsInCritical()
        {
            var h = new HealthModel(90f);
            h.ApplyGrabWithoutSpike();
            Assert.AreEqual(HealthState.Injured, h.State);
            h.ApplyGrabWithoutSpike();
            Assert.AreEqual(HealthState.Critical, h.State);
            h.ApplyGrabWithoutSpike();
            Assert.IsTrue(h.IsDead);
        }
    }
}
