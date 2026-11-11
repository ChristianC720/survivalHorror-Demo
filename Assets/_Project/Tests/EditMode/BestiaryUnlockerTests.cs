using Heartbeat.Core;
using Heartbeat.Generation;
using Heartbeat.Narrative;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class BestiaryUnlockerTests
    {
        [Test]
        public void MotherEntry_UnlocksOnlyInHerRoom_Once()
        {
            var registry = new RegistryModel();
            var unlocker = new BestiaryUnlocker(registry);
            int raised = 0;
            System.Action<string, int, bool> count = (id, kind, isNew) => { if (id == "BEST.MOTHER") raised++; };
            GameEvents.ItemRegistered += count;
            try
            {
                GameEvents.RaiseFloorBuilt(3, 1);
                Assert.IsFalse(registry.Has("BEST.MOTHER"));
                GameEvents.RaiseFloorBuilt(FloorBuilder.MotherFloor, 0);
                GameEvents.RaiseFloorBuilt(FloorBuilder.MotherFloor, 0);
                Assert.IsTrue(registry.Has("BEST.MOTHER"));
                Assert.AreEqual(1, raised);
            }
            finally { GameEvents.ItemRegistered -= count; unlocker.Dispose(); }
        }
    }
}
