using System.IO;
using Heartbeat.Core;
using Heartbeat.Cycle;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class TelemetryRecorderTests
    {
        [Test]
        public void OnlyWithTheArgument()
        {
            Assert.IsTrue(TelemetryRecorder.Requested(new[] { "game.exe", "-showcase", "-telemetry" }));
            Assert.IsFalse(TelemetryRecorder.Requested(new[] { "game.exe", "-showcase" }));
        }

        [Test]
        public void WritesOneJsonLinePerEvent_AndStopsListeningWhenDisposed()
        {
            var w = new StringWriter();
            float t = 0f;
            var rec = new TelemetryRecorder(w, () => t += 1f);
            GameEvents.RaiseFloorBuilt(2, 99);
            GameEvents.RaisePlaySubStateChanged(PlaySubState.Exploration, PlaySubState.Surveillance);
            GameEvents.RaisePlaySubStateChanged(PlaySubState.Surveillance, PlaySubState.Exploration);
            GameEvents.RaisePlayerDied(DeathCause.Health);
            rec.Dispose();
            GameEvents.RaisePlayerDied(DeathCause.Health);

            var lines = w.ToString().Trim().Split('\n');
            Assert.AreEqual(6, lines.Length, w.ToString());
            StringAssert.Contains("\"event\":\"session_start\"", lines[0]);
            StringAssert.Contains("\"event\":\"floor_start\",\"floor\":2", lines[1]);
            StringAssert.Contains("\"seed\":99", lines[1]);
            StringAssert.Contains("surveillance_on", lines[2]);
            StringAssert.Contains("surveillance_off", lines[3]);
            StringAssert.Contains("\"cause\":\"Health\"", lines[4]);
            StringAssert.Contains("session_end", lines[5]);
        }

        [Test]
        public void LogsGrabsAndPickUps_ToCountSpikes() // C-64
        {
            var w = new StringWriter();
            var rec = new TelemetryRecorder(w, () => 0f);
            GameEvents.RaiseItemPickedUp("Spike", 1);
            GameEvents.RaiseGrabResolved(true);
            GameEvents.RaiseGrabResolved(false);
            rec.Dispose();

            var lines = w.ToString().Trim().Split('\n');
            Assert.AreEqual(5, lines.Length, w.ToString());
            StringAssert.Contains("\"event\":\"item_picked\"", lines[1]);
            StringAssert.Contains("\"kind\":\"Spike\",\"amount\":1", lines[1]);
            StringAssert.Contains("\"event\":\"grab\"", lines[2]);
            StringAssert.Contains("\"spike\":true", lines[2]);
            StringAssert.Contains("\"spike\":false", lines[3]);
        }
    }
}
