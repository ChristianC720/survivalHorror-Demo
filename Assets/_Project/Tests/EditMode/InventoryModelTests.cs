using Heartbeat.Inventory;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class InventoryModelTests
    {
        [Test]
        public void Ammo_StacksTo12PerSlot_ThenOpensNewSlot()
        {
            var inv = new InventoryModel();
            for (int i = 0; i < 4; i++) Assert.AreEqual(0, inv.Add(ItemKind.Ammo, 4)); // 16 rounds
            Assert.AreEqual(16, inv.Count(ItemKind.Ammo));
            Assert.AreEqual(4, inv.FreeSlots);
            Assert.AreEqual(12, inv[0].Count);
            Assert.AreEqual(4, inv[1].Count);
        }

        [Test]
        public void Heals_DoNotStack()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Heal, 2);
            Assert.AreEqual(4, inv.FreeSlots);
        }

        [Test]
        public void Full_ReturnsLeftover_AndKeepsItemsOnFloor()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Heal, 6);
            Assert.IsTrue(inv.IsFull);
            Assert.AreEqual(4, inv.Add(ItemKind.Ammo, 4));
            Assert.AreEqual(0, inv.SpaceFor(ItemKind.KeyMandatory, 1));
        }

        [Test]
        public void PartialAmmoSlot_TakesPartOfBlister()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Heal, 5);
            inv.Add(ItemKind.Ammo, 10);
            Assert.AreEqual(2, inv.Add(ItemKind.Ammo, 4));
            Assert.AreEqual(12, inv.Count(ItemKind.Ammo));
        }

        [Test]
        public void ConsumeAmmo_EmptiestSlotFirst_FreesSlot()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Ammo, 14); // 12 + 2
            Assert.IsTrue(inv.TryConsume(ItemKind.Ammo, 2));
            Assert.AreEqual(5, inv.FreeSlots);
            Assert.AreEqual(12, inv.Count(ItemKind.Ammo));
        }

        [Test]
        public void Consume_IsAllOrNothing()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Ammo, 3);
            Assert.IsFalse(inv.TryConsume(ItemKind.Ammo, 4));
            Assert.AreEqual(3, inv.Count(ItemKind.Ammo));
        }

        [Test]
        public void KeyReplacesSlot_WhenFull()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Heal, 6);
            var old = inv.Replace(2, ItemKind.KeyMandatory, 1);
            Assert.AreEqual(ItemKind.Heal, old.Kind);
            Assert.IsTrue(inv.Has(ItemKind.KeyMandatory));
            Assert.AreEqual(5, inv.Count(ItemKind.Heal));
        }

        [Test]
        public void Clear_LosesEverything()
        {
            var inv = new InventoryModel();
            inv.Add(ItemKind.Spike, 1);
            inv.Add(ItemKind.Ammo, 8);
            inv.Clear();
            Assert.AreEqual(6, inv.FreeSlots);
        }

        [Test]
        public void ChangedEvent_FiresOnAddAndConsume()
        {
            var inv = new InventoryModel();
            int n = 0;
            inv.Changed += () => n++;
            inv.Add(ItemKind.Spike, 1);
            inv.TryConsume(ItemKind.Spike);
            Assert.AreEqual(2, n);
        }
    }
}
