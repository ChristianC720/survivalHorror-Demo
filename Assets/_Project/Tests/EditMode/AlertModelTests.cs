using Heartbeat.AI;
using Heartbeat.Core;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class AlertModelTests
    {
        private static AlertModel New() => new AlertModel(AlertTuning.Default);

        private static void Run(AlertModel m, float seconds, bool sees, float prox = 0.5f, bool lit = false)
        {
            for (float t = 0; t < seconds; t += 0.05f) m.Tick(0.05f, sees, prox, lit);
        }

        [Test]
        public void SeenLongEnough_GoesThroughEveryStateToHunt()
        {
            var m = New();
            var seen = new System.Collections.Generic.List<AlertState>();
            m.StateChanged += (a, b) => seen.Add(b);
            Run(m, 3f, true);
            CollectionAssert.AreEqual(new[] { AlertState.Suspicion, AlertState.Search, AlertState.Hunt }, seen);
        }

        [Test]
        public void Flashlight_DoublesAlertSpeed() // M-12
        {
            var dark = New(); var lit = New();
            Run(dark, 0.5f, true, 0.5f, false);
            Run(lit, 0.5f, true, 0.5f, true);
            Assert.AreEqual(dark.Level * 2f, lit.Level, 0.02f);
        }

        [Test]
        public void NotSeen_StaysPatrol() // M-09 crouched behind cover: no sight input
        {
            var m = New();
            Run(m, 10f, false);
            Assert.AreEqual(AlertState.Patrol, m.State);
        }

        [Test]
        public void Shot_JumpsToSearch_Footsteps_ToSuspicion()
        {
            var a = New(); a.Hear(NoiseSource.Shot);
            var b = New(); b.Hear(NoiseSource.Footsteps);
            Assert.AreEqual(AlertState.Search, a.State);
            Assert.AreEqual(AlertState.Suspicion, b.State);
        }

        [Test]
        public void Hunt_LostAfterFiveSecondsWithoutSight_ThenSearch()
        {
            var m = New();
            m.ForceHunt();
            Run(m, 4.5f, false);
            Assert.AreEqual(AlertState.Hunt, m.State);
            Run(m, 1f, false);
            Assert.AreEqual(AlertState.Search, m.State);
        }

        [Test]
        public void Search_DecaysBackToPatrol_Eventually()
        {
            var m = New();
            m.Hear(NoiseSource.Shot);
            Run(m, 20f, false);
            Assert.AreEqual(AlertState.Patrol, m.State);
        }

        [Test]
        public void AttentionFull_NeverLeavesPatrolEnemiesInPatrol_AndDoesNotLowerHunt() // S-02 acceptance, D-24
        {
            var patrol = New(); patrol.OnAttentionFull(2f);
            Assert.AreEqual(AlertState.Suspicion, patrol.State);   // D-98: suspicion first...
            Run(patrol, 1.5f, false, 0f);
            Assert.AreEqual(AlertState.Suspicion, patrol.State);   // ...without decaying back to patrol...
            Run(patrol, 1f, false, 0f);
            Assert.AreEqual(AlertState.Search, patrol.State);      // ...then search
            var hunting = New(); hunting.ForceHunt(); hunting.OnAttentionFull(2f);
            Assert.AreEqual(AlertState.Hunt, hunting.State);
        }

        [Test]
        public void CloseRange_FasterThanFar()
        {
            var close = New(); var far = New();
            Run(close, 0.4f, true, 1f);
            Run(far, 0.4f, true, 0f);
            Assert.Greater(close.Level, far.Level);
        }
    }
}
