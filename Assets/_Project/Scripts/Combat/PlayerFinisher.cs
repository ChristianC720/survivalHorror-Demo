using System.Collections;
using System.Collections.Generic;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Inventory;
using Heartbeat.Player;
using Heartbeat.Recycle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Finishing bodies (M-20, D-27). At ~1.5 m looking at an unfinished body Lukas enters the OverBody context:
    /// left click = ground strike (25% stamina, ~2 s, audible), E = discharge spike (~1.5 s, silent, consumes the spike).
    /// Both stop the lab from recycling the body. Bodies that cannot be recycled never trigger the context.
    /// </summary>
    public sealed class PlayerFinisher : MonoBehaviour
    {
        [SerializeField] private CombatBalance balance;
        [SerializeField] private PlayerBalance playerBalance;
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerInventory inventory;

        private InputRouter router;
        private readonly List<Corpse> candidates = new();
        private Vector3 targetPoint;

        public Corpse Target { get; private set; }
        public bool HasTarget => Target != null;
        public bool IsFinishing { get; private set; }

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Start()
        {
            router = Services.Get<InputRouter>();
            router.Actions.Gameplay.Attack.performed += OnAttack;
            router.Actions.Gameplay.Interact.performed += OnInteract;
        }

        private void OnDestroy()
        {
            if (router == null) return;
            router.Actions.Gameplay.Attack.performed -= OnAttack;
            router.Actions.Gameplay.Interact.performed -= OnInteract;
        }

        private void Update()
        {
            if (IsFinishing) return;
            Target = player.IsAiming ? null : FindTarget();
            // D-27: if a door, panel or item is more centred in the view than the body, E/click go to it instead.
            if (Target != null && Services.TryGet<Interaction.Interactor>(out var interactor) && interactor.Current != null
                && interactor.CurrentScore > interactor.ViewScore(targetPoint))
                Target = null;

            var ctx = router.Context;
            if (Target != null && ctx == InputContext.Exploration) router.SetContext(InputContext.OverBody);
            else if (Target == null && ctx == InputContext.OverBody) router.SetContext(InputContext.Exploration);
        }

        private Corpse FindTarget()
        {
            candidates.Clear();
            foreach (var c in Corpse.All)
                if (c.CanBeRecycled) candidates.Add(c);
            return Pick(candidates, transform.position, transform.forward, balance.finisherRange, out targetPoint);
        }

        /// <summary>
        /// C-76: the body Lukas finishes. Any part of the lying body within <paramref name="range"/> and roughly in front
        /// counts (head, torso or feet), not only the root where the enemy died. With two bodies near, the one most in
        /// front wins. <paramref name="point"/> is the part of the chosen body most in front.
        /// </summary>
        public static Corpse Pick(IReadOnlyList<Corpse> corpses, Vector3 from, Vector3 forward, float range, out Vector3 point)
        {
            Corpse best = null;
            float bestDot = 0.5f;   // must be roughly in front of Lukas
            point = from;
            foreach (var c in corpses)
            {
                if (!c.TryFacingPoint(from, forward, range, out var p, out float dot)) continue;
                if (dot > bestDot) { bestDot = dot; best = c; point = p; }
            }
            return best;
        }

        private void OnAttack(InputAction.CallbackContext _)
        {
            if (!HasTarget || IsFinishing || player.IsAiming) return;
            if (!stamina.TrySpend(playerBalance.groundStrikeCost)) return;   // gasp logged by stamina
            StartCoroutine(FinishRoutine(Target, balance.groundStrikeSeconds, "ground strike", noisy: true));
        }

        private void OnInteract(InputAction.CallbackContext _)
        {
            if (!HasTarget || IsFinishing) return;
            if (!inventory.Model.TryConsume(ItemKind.Spike))
            {
                HbLog.Info("Combat", "No discharge spike: E does nothing over the body");
                return;
            }
            Sfx.Play(Sfx.SpikeDischarge, transform.position);   // audible to the player, emits no AI noise
            StartCoroutine(FinishRoutine(Target, balance.spikeFinisherSeconds, "discharge spike", noisy: false));
        }

        private IEnumerator FinishRoutine(Corpse corpse, float seconds, string how, bool noisy)
        {
            IsFinishing = true;
            player.MovementLocked = true;
            HbLog.Info("Combat", $"Finisher started: {how}");
            GameEvents.RaisePlayerGesture(noisy ? "GroundStrike" : "Finisher");
            if (noisy)
                GameEvents.RaiseNoiseEmitted(new NoiseInfo(corpse.transform.position, balance.groundStrikeNoiseRadius, NoiseSource.GroundStrike));
            yield return new WaitForSeconds(seconds);
            corpse.Finish(how);
            player.MovementLocked = false;
            IsFinishing = false;
        }
    }
}
