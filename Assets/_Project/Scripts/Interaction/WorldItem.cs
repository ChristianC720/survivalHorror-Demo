using Heartbeat.Core;
using Heartbeat.Inventory;
using UnityEngine;

namespace Heartbeat.Interaction
{
    /// <summary>
    /// A slot item lying in the world (M-07, M-08): heal, ammo blister (4 rounds), spike, keys. Picking up adds what
    /// fits; the rest stays on the floor. With a full inventory a key can replace another item: that choice is made in
    /// the arm terminal; if the player declines, the key stays on the floor. Other items just report the full inventory.
    /// </summary>
    public sealed class WorldItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemKind kind = ItemKind.Ammo;
        [SerializeField, Min(1)] private int amount = 4;

        public ItemKind Kind => kind;
        public int Amount => amount;
        public string PromptKey => "UI.PROMPT.PICKUP";
        public Vector3 InteractionPoint => transform.position;

        public void Setup(ItemKind k, int count)
        {
            kind = k;
            amount = count;
        }

        public bool CanInteract(Interactor who) => amount > 0;

        public void Interact(Interactor who)
        {
            var inv = who.Inventory.Model;
            int left = inv.Add(kind, amount);
            int taken = amount - left;
            if (taken == 0)
            {
                bool key = kind == ItemKind.KeyMandatory || kind == ItemKind.KeyOptional;
                if (key && GameEvents.RaiseKeySwapOffered(kind, amount, () => { amount = 0; gameObject.SetActive(false); })) return;
                HbLog.Info("Interact", $"Inventory full: {kind} stays on the floor");
                GameEvents.RaiseInventoryFull(kind.ToString());
                return;
            }
            amount = left;
            Sfx.Play2D(Sfx.Pickup);
            GameEvents.RaiseNotice("UI.NOTICE.PICKED", UI.Loc.Get("UI.ITEM." + kind), taken);
            HbLog.Info("Interact", $"Picked up {kind} x{taken}{(left > 0 ? $" ({left} left on the floor)" : "")}");
            if (amount <= 0) gameObject.SetActive(false);
        }
    }
}
