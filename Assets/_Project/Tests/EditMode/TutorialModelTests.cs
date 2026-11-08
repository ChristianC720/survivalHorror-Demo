using Heartbeat.Tutorial;
using NUnit.Framework;

namespace Heartbeat.Tests.EditMode
{
    public class TutorialModelTests
    {
        [Test]
        public void ClearsOnlyWhenEveryActionOfTheRoomIsDone()
        {
            var m = new TutorialModel { Stage = 2 };
            int cleared = 0;
            m.StageCleared += s => cleared = s;
            Assert.IsTrue(m.Do(TutorialAction.Flashlight));
            Assert.AreEqual(0, cleared);
            Assert.AreEqual(TutorialAction.PickUp, m.Current);
            m.Do(TutorialAction.PickUp);
            Assert.AreEqual(2, cleared);
            Assert.IsNull(m.Current);
        }

        [Test]
        public void ActionsOutsideTheirRoomDoNotCount()
        {
            var m = new TutorialModel { Stage = 1 };
            Assert.IsFalse(m.Do(TutorialAction.Sprint));
            m.Stage = 3;
            Assert.AreEqual(TutorialAction.Sprint, m.Current);
        }

        [Test]
        public void TimeoutClearAlsoOpensTheRoomOnce()
        {
            var m = new TutorialModel { Stage = 4 };
            int fired = 0;
            m.StageCleared += _ => fired++;
            m.Clear(4);
            m.Clear(4);
            Assert.AreEqual(1, fired);
            Assert.IsNull(m.Current);
        }
    }
}
