using Heartbeat.Core;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Inventory
{
    /// <summary>
    /// Lukas's 6-slot inventory (S-05). Never pauses. Using a heal takes ~1.5 s and is cancelled without consuming
    /// the item if Lukas is hit meanwhile (M-05, M-07). Registered as a scene service for other systems.
    /// </summary>
    public sealed class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private PlayerBalance balance;
        [SerializeField] private PlayerHealth health;

        private float healTimer = -1f;   // shared ~1.5 s action timer: heal or discard (M-07)
        private int pendingDiscard = -1;

        public InventoryModel Model { get; } = new InventoryModel();
        public bool IsUsingItem => healTimer >= 0f;
        public float UseProgress => IsUsingItem ? 1f - healTimer / balance.healSeconds : 0f;

        private void OnEnable()
        {
            Services.Register(this);
            Model.Changed += OnChanged;
            if (health != null) health.Hit += OnHit;
        }

        private void OnDisable()
        {
            Services.Unregister(this);
            Model.Changed -= OnChanged;
            if (health != null) health.Hit -= OnHit;
        }

        private void Update()
        {
            if (healTimer < 0f) return;
            healTimer -= Time.deltaTime;
            if (healTimer > 0f) return;
            healTimer = -1f;
            if (pendingDiscard >= 0) { FinishDiscard(); return; }
            if (health.Model.CanHeal && Model.TryConsume(ItemKind.Heal))
            {
                health.Model.TryHeal();
                HbLog.Info("Inventory", $"Heal used -> {health.State}");
            }
        }

        /// <summary>Use the item in a slot (left click / A on the slot).</summary>
        public bool TryUseSlot(int index)
        {
            var slot = Model[index];
            if (slot.IsEmpty || IsUsingItem) return false;
            switch (slot.Kind)
            {
                case ItemKind.Heal:
                    if (!health.Model.CanHeal)
                    {
                        HbLog.Info("Inventory", "Heal refused: health is full");
                        return false;
                    }
                    healTimer = balance.healSeconds;
                    HbLog.Info("Inventory", "Heal started");
                    GameEvents.RaisePlayerGesture("Heal");
                    Sfx.Play2D(Sfx.HealUse);
                    return true;
                default:
                    // Ammo is loaded with R, spikes trigger on grabs or finishers, keys are used on doors.
                    return false;
            }
        }

        /// <summary>Right click on a slot: after ~1.5 s the item drops at Lukas's feet as a pickup (M-07).</summary>
        public bool TryDiscardSlot(int index)
        {
            if (Model[index].IsEmpty || IsUsingItem) return false;
            pendingDiscard = index;
            healTimer = balance.healSeconds;
            HbLog.Info("Inventory", $"Discard started: {Model[index].Kind}");
            return true;
        }

        private void FinishDiscard()
        {
            var slot = Model.RemoveSlot(pendingDiscard);
            pendingDiscard = -1;
            if (slot.IsEmpty) return;
            if (Services.TryGet(out Generation.FloorBuilder builder))
                builder.DropItem(slot.Kind, slot.Count, transform.position + transform.forward * 0.5f);
            HbLog.Info("Inventory", $"Discarded {slot.Kind} x{slot.Count}");
        }

        public void TryUseFirst(ItemKind kind)
        {
            for (int i = 0; i < InventoryModel.SlotCount; i++)
                if (Model[i].Kind == kind) { TryUseSlot(i); return; }
        }

        private void OnHit()
        {
            if (!IsUsingItem) return;
            healTimer = -1f;
            pendingDiscard = -1;
            HbLog.Info("Inventory", "Item action cancelled by a hit (item kept)");
        }

        private void OnChanged()
        {
            HbLog.Dev("Inventory", Describe());
            GameEvents.RaiseInventoryChanged();
        }

        /// <summary>Debug console / playtests: give items by name (Heal, Ammo, Spike, KeyMandatory, KeyOptional).</summary>
        public int DebugGive(string kind, int count)
        {
            if (!System.Enum.TryParse(kind, true, out ItemKind k)) return -1;
            return Model.Add(k, count);
        }

        public string Describe()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < InventoryModel.SlotCount; i++)
            {
                var s = Model[i];
                sb.Append(s.IsEmpty ? "[ ]" : $"[{s.Kind} {s.Count}]");
            }
            return sb.ToString();
        }
    }
}
