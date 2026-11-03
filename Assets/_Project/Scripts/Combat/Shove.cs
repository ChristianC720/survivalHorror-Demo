using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Combat
{
    /// <summary>
    /// Shove / butt strike (M-06): left click / RT without aiming. Costs 35% stamina (fails with a gasp otherwise),
    /// pushes and briefly staggers the enemy in front, gives no protection against damage.
    /// Over an unfinished body the same input is the ground strike instead (<see cref="PlayerFinisher"/>).
    /// </summary>
    public sealed class Shove : MonoBehaviour
    {
        [SerializeField] private CombatBalance balance;
        [SerializeField] private PlayerBalance playerBalance;
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerFinisher finisher;

        private InputRouter router;
        private float cooldown;
        private int enemyMask;

        public int Shoves { get; private set; }

        private void Start()
        {
            enemyMask = LayerMask.GetMask("Enemy");
            router = Services.Get<InputRouter>();
            router.Actions.Gameplay.Attack.performed += OnAttack;
        }

        private void OnDestroy()
        {
            if (router != null) router.Actions.Gameplay.Attack.performed -= OnAttack;
        }

        private void Update() => cooldown = Mathf.Max(0f, cooldown - Time.deltaTime);

        private void OnAttack(InputAction.CallbackContext _)
        {
            if (player.IsAiming || player.MovementLocked || cooldown > 0f) return;
            if (finisher != null && finisher.HasTarget) return;   // ground strike handles it
            TryShove();
        }

        public bool TryShove()
        {
            if (!stamina.TrySpend(playerBalance.shoveCost)) return false;
            cooldown = balance.shoveCooldown;
            Shoves++;

            Vector3 origin = transform.position + Vector3.up * 1.1f;
            var hits = Physics.SphereCastAll(origin, balance.shoveRadius, transform.forward, balance.shoveRange, enemyMask, QueryTriggerInteraction.Ignore);
            IStaggerable target = null;
            foreach (var h in hits)
            {
                target = h.collider.GetComponentInParent<IStaggerable>();
                if (target != null) break;
            }
            Sfx.Play(Sfx.Shove, origin);
            GameEvents.RaisePlayerGesture("Shove");
            if (target != null)
            {
                target.Stagger(transform.position, balance.shoveStaggerSeconds);
                HbLog.Info("Combat", "Shove hit");
            }
            else HbLog.Info("Combat", "Shove (no target)");
            return true;
        }
    }
}
