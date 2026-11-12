using Heartbeat.Interaction;
using Heartbeat.Level;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    /// <summary>C-59: on a closed door a tap of Interact opens it and a 2 s hold seals it.</summary>
    public sealed class HoldInteractionTests
    {
        private const float Tap = 0.25f, Hold = 2f;

        private static HoldInteraction Begun()
        {
            var h = new HoldInteraction();
            h.Begin(Tap, Hold);
            return h;
        }

        [Test]
        public void ShortPress_IsATap_SoTheDoorOpens()
        {
            var h = Begun();
            Assert.AreEqual(HoldResult.None, h.Tick(0.1f, true));
            Assert.AreEqual(HoldResult.Tap, h.Tick(0.02f, false));
            Assert.IsFalse(h.Active);
        }

        [Test]
        public void HeldTwoSeconds_Completes_SoTheDoorSeals()
        {
            var h = Begun();
            Assert.AreEqual(HoldResult.None, h.Tick(1.9f, true));
            Assert.AreEqual(0.95f, h.Progress, 0.001f);
            Assert.AreEqual(HoldResult.Completed, h.Tick(0.2f, true));
            Assert.IsFalse(h.Active);
            Assert.AreEqual(HoldResult.None, h.Tick(1f, true), "one press completes once");
        }

        [Test]
        public void ReleasedAt1_9Seconds_Cancels_NeitherSealsNorOpens()
        {
            var h = Begun();
            h.Tick(1.9f, true);
            Assert.AreEqual(HoldResult.Cancelled, h.Tick(0.02f, false));
        }

        [Test]
        public void Cancel_StopsTheHold()
        {
            var h = Begun();
            h.Tick(1f, true);
            h.Cancel();   // out of range or hit
            Assert.AreEqual(0f, h.Progress);
            Assert.AreEqual(HoldResult.None, h.Tick(5f, true));
        }

        [TestCase(DoorLock.None, DoorState.Closed, true)]
        [TestCase(DoorLock.None, DoorState.Open, false)]
        [TestCase(DoorLock.None, DoorState.Sealed, false)]
        [TestCase(DoorLock.MandatoryKey, DoorState.Closed, false)]
        [TestCase(DoorLock.OptionalKey, DoorState.Closed, false)]
        [TestCase(DoorLock.NoPower, DoorState.Closed, false)]
        [TestCase(DoorLock.Tutorial, DoorState.Closed, false)]
        public void OnlyAClosedUnlockedDoor_OffersTheSeal(DoorLock l, DoorState s, bool expected) =>
            Assert.AreEqual(expected, DoorLeaf.CanSeal(l, s));

        [Test]
        public void BalanceAsset_HoldsTheSealTimes()
        {
            var b = UnityEditor.AssetDatabase.LoadAssetAtPath<Core.SystemsBalance>("Assets/_Project/ScriptableObjects/Balance/SystemsBalance.asset");
            Assert.AreEqual(2f, b.sealHoldSeconds);
            Assert.AreEqual(0.25f, b.sealTapSeconds);
        }
    }
}
