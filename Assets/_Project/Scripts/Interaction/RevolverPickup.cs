using Heartbeat.Combat;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Interaction
{
    /// <summary>
    /// The Kessler revolver lying in the world (D-74): on the assimilated guard in the tutorial depot (first attempt),
    /// then on the personal-effects tray next to the capsule after every reconstruction on floor 1. Placed by hand in
    /// the room prefabs; <see cref="Generation.FloorBuilder"/> switches each one on only where it applies.
    /// </summary>
    public sealed class RevolverPickup : MonoBehaviour, IInteractable
    {
        public enum Spot { TutorialGuard, CapsuleTray }

        [SerializeField] private Spot spot = Spot.CapsuleTray;

        public Spot Where => spot;
        public string PromptKey => "UI.PROMPT.PICKUP";
        public Vector3 InteractionPoint => transform.position;

        private Revolver revolver;

        // Lukas may already carry it (floor start or final-room save): then the tray is empty.
        private void Update()
        {
            if (revolver == null && Services.TryGet(out Player.PlayerController pc)) pc.TryGetComponent(out revolver);
            if (revolver != null && revolver.Owned) gameObject.SetActive(false);
        }

        public bool CanInteract(Interactor who) => who.Player.TryGetComponent(out Revolver r) && !r.Owned;

        public void Interact(Interactor who)
        {
            if (!who.Player.TryGetComponent(out Revolver r)) return;
            r.SetOwned(true);
            Sfx.Play(Sfx.Pickup, transform.position);
            HbLog.Info("Revolver", $"Picked up ({spot}), loaded={r.Model.Loaded}");
            gameObject.SetActive(false);
        }
    }
}
