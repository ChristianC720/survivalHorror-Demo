using Heartbeat.Core;
using NUnit.Framework;
using UnityEngine;

namespace Heartbeat.Tests
{
    public class PauseReadingTests
    {
        [TearDown] public void TearDown() => PauseService.ResetTime();

        [Test]
        public void Reading_StopsTime_WithoutPauseState_AndRestoresIt()
        {
            var states = new GameStateMachine();
            var pause = new PauseService(states);
            var before = states.Current;

            pause.BeginReading();
            Assert.IsTrue(pause.IsReading);
            Assert.AreEqual(0f, Time.timeScale);
            Assert.IsTrue(AudioListener.pause);
            Assert.IsFalse(pause.IsPaused, "No pause menu or state change (D-159)");
            Assert.AreEqual(before, states.Current);

            pause.EndReading();
            Assert.IsFalse(pause.IsReading);
            Assert.AreEqual(1f, Time.timeScale);
            Assert.IsFalse(AudioListener.pause);
            Assert.AreEqual(Time.frameCount, pause.ReadingEndedFrame);
        }

        [Test]
        public void EndReading_WithoutBegin_DoesNotTouchTime()
        {
            var pause = new PauseService(new GameStateMachine());
            Time.timeScale = 0.5f;
            pause.EndReading();
            Assert.AreEqual(0.5f, Time.timeScale);
            Assert.AreEqual(-1, pause.ReadingEndedFrame);
        }
    }
}
