using System;

namespace Heartbeat.Inventory
{
    /// <summary>The 5 slot-taking item types (M-07, D-71). Revolver, flashlight, ring and records take no slot.</summary>
    public enum ItemKind { None, Heal, Ammo, Spike, KeyMandatory, KeyOptional }

    [Serializable]
    public struct InventorySlot
    {
        public ItemKind Kind;
        public int Count;
        public bool IsEmpty => Kind == ItemKind.None || Count <= 0;
        public static readonly InventorySlot Empty = new InventorySlot { Kind = ItemKind.None, Count = 0 };
    }

    /// <summary>
    /// 6 fixed slots in real time (M-07). Per-slot caps: heal 1, spike 1, key 1, ammo 12 rounds (D-37, D-71).
    /// Pure logic: no Unity types, EditMode-tested.
    /// </summary>
    public sealed class InventoryModel
    {
        public const int SlotCount = 6;
        public const int AmmoPerSlot = 12;

        private readonly InventorySlot[] slots = new InventorySlot[SlotCount];

        public event Action Changed;

        public InventorySlot this[int index] => slots[index];

        public static int CapFor(ItemKind kind) => kind == ItemKind.Ammo ? AmmoPerSlot : kind == ItemKind.None ? 0 : 1;

        public int FreeSlots
        {
            get { int n = 0; foreach (var s in slots) if (s.IsEmpty) n++; return n; }
        }

        public bool IsFull => FreeSlots == 0;

        public int Count(ItemKind kind)
        {
            int n = 0;
            foreach (var s in slots) if (s.Kind == kind) n += s.Count;
            return n;
        }

        public bool Has(ItemKind kind) => Count(kind) > 0;

        /// <summary>How many of <paramref name="amount"/> would fit right now.</summary>
        public int SpaceFor(ItemKind kind, int amount)
        {
            int cap = CapFor(kind), space = 0;
            foreach (var s in slots)
            {
                if (s.IsEmpty) space += cap;
                else if (s.Kind == kind) space += cap - s.Count;
                if (space >= amount) return amount;
            }
            return Math.Min(space, amount);
        }

        /// <summary>Adds up to <paramref name="amount"/>, topping up partial slots first. Returns the leftover that did not fit.</summary>
        public int Add(ItemKind kind, int amount)
        {
            if (kind == ItemKind.None || amount <= 0) return amount;
            int cap = CapFor(kind);
            int left = amount;
            for (int i = 0; i < SlotCount && left > 0; i++)
            {
                if (slots[i].Kind != kind || slots[i].Count >= cap) continue;
                int put = Math.Min(cap - slots[i].Count, left);
                slots[i].Count += put;
                left -= put;
            }
            for (int i = 0; i < SlotCount && left > 0; i++)
            {
                if (!slots[i].IsEmpty) continue;
                int put = Math.Min(cap, left);
                slots[i] = new InventorySlot { Kind = kind, Count = put };
                left -= put;
            }
            if (left != amount) Changed?.Invoke();
            return left;
        }

        /// <summary>Consumes <paramref name="amount"/> (all or nothing). Ammo is taken from the emptiest slot first to free slots.</summary>
        public bool TryConsume(ItemKind kind, int amount = 1)
        {
            if (amount <= 0) return true;
            if (Count(kind) < amount) return false;
            int left = amount;
            while (left > 0)
            {
                int best = -1;
                for (int i = 0; i < SlotCount; i++)
                    if (slots[i].Kind == kind && slots[i].Count > 0 && (best < 0 || slots[i].Count < slots[best].Count)) best = i;
                int take = Math.Min(slots[best].Count, left);
                slots[best].Count -= take;
                left -= take;
                if (slots[best].Count == 0) slots[best] = InventorySlot.Empty;
            }
            Changed?.Invoke();
            return true;
        }

        /// <summary>Empties a slot (discard) and returns what it held so it can be dropped in the world.</summary>
        public InventorySlot RemoveSlot(int index)
        {
            var s = slots[index];
            if (s.IsEmpty) return InventorySlot.Empty;
            slots[index] = InventorySlot.Empty;
            Changed?.Invoke();
            return s;
        }

        /// <summary>Full inventory + key item: replace a slot with the key (M-07 edge case). Returns what was replaced.</summary>
        public InventorySlot Replace(int index, ItemKind kind, int count)
        {
            var old = slots[index];
            slots[index] = new InventorySlot { Kind = kind, Count = Math.Min(count, CapFor(kind)) };
            Changed?.Invoke();
            return old;
        }

        /// <summary>Death / restart: everything is lost (M-16).</summary>
        public void Clear()
        {
            for (int i = 0; i < SlotCount; i++) slots[i] = InventorySlot.Empty;
            Changed?.Invoke();
        }

        public InventorySlot[] Snapshot() => (InventorySlot[])slots.Clone();

        public void Restore(InventorySlot[] snapshot)
        {
            for (int i = 0; i < SlotCount; i++) slots[i] = i < snapshot.Length ? snapshot[i] : InventorySlot.Empty;
            Changed?.Invoke();
        }
    }
}
