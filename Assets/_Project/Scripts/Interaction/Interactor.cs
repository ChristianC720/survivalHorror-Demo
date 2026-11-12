using Heartbeat.CameraSystem;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Inventory;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Interaction
{
    /// <summary>
    /// Finds the interactable Lukas is looking at (M-08) and runs it on E / A. Only what lies inside the camera's cone and
    /// in front of Lukas counts (a door at his back is never used); among those, the one closest to the centre of the
    /// view wins (D-27), which also decides between a body finisher and a door/item.
    /// Never pauses the game. Raises InteractTargetChanged for the diegetic E prompt (E-10).
    /// </summary>
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField] private float range = 1.8f;
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerBalance balance;

        private InputRouter router;
        private OtsCameraRig rig;
        private int mask;
        private readonly Collider[] buffer = new Collider[64];
        private int wallMask;
        private readonly HoldInteraction hold = new HoldInteraction();
        private IHoldInteractable holdTarget;
        private PlayerHealth health;

        public IInteractable Current { get; private set; }
        /// <summary>How centred the current target is (dot of view direction, 1 = dead centre).</summary>
        public float CurrentScore { get; private set; } = -1f;
        public PlayerInventory Inventory => inventory;
        public PlayerController Player => player;
        /// <summary>0..1 while Interact is held on a target with a hold action (the prompt draws it); 0 otherwise.</summary>
        public float HoldProgress => hold.Progress;

        private void Start()
        {
            mask = LayerMask.GetMask("Interactable");
            wallMask = LayerMask.GetMask("Environment");
            router = Services.Get<InputRouter>();
            Services.TryGet(out rig);
            router.Actions.Gameplay.Interact.performed += OnInteract;
            if (player.TryGetComponent(out health)) health.Hit += hold.Cancel;   // a hit cancels the hold
            gameObject.AddComponent<InteractableOutliner>().Init(this, balance.outlineRange);
        }

        private void OnEnable() => Services.Register(this);

        private void OnDisable() => Services.Unregister(this);

        private void OnDestroy()
        {
            if (router != null) router.Actions.Gameplay.Interact.performed -= OnInteract;
            if (health != null) health.Hit -= hold.Cancel;
        }

        private void Update()
        {
            float score = -1f;
            var best = player.MovementLocked ? null : FindBest(out score);
            if (best == null) score = -1f;
            if (!ReferenceEquals(best, Current))
            {
                Current = best;
                GameEvents.RaiseInteractTargetChanged(best as MonoBehaviour);
                HbLog.Dev("Interact", best == null ? "No target" : $"Target {((MonoBehaviour)best).name}");
            }
            CurrentScore = score;
            if (hold.Active) TickHold();
        }

        private void TickHold()
        {
            // Out of range, looking away, movement locked or the target no longer offers it: cancelled, nothing runs.
            if (!ReferenceEquals(holdTarget, Current) || !holdTarget.CanHold) { hold.Cancel(); return; }
            switch (hold.Tick(Time.deltaTime, router.Actions.Gameplay.Interact.IsPressed()))
            {
                case HoldResult.Tap:
                    Use(Current);
                    break;
                case HoldResult.Completed:
                    HbLog.Info("Interact", $"Hold {((MonoBehaviour)Current).name}");
                    GameEvents.RaisePlayerGesture(GestureFor(Current));
                    holdTarget.HoldComplete(this);
                    break;
            }
        }

        /// <summary>Score of any world point as seen from the camera (used by the finisher to compare).</summary>
        public float ViewScore(Vector3 point)
        {
            Transform view = rig != null ? rig.Brain.transform : transform;
            Vector3 to = point - view.position;
            return Vector3.Dot(view.forward, to.normalized);
        }

        /// <summary>Doors are Environment themselves: a hit right next to the target still counts as seeing it.</summary>
        public static bool HasLineOfSight(Vector3 eye, Vector3 point, int walls) =>
            !Physics.Linecast(eye, point, out var hit, walls, QueryTriggerInteraction.Ignore) || Vector3.Distance(hit.point, point) < 0.6f;

        private IInteractable FindBest(out float bestScore)
        {
            bestScore = Mathf.Cos(balance.interactConeDegrees * Mathf.Deg2Rad);
            IInteractable best = null;
            Transform view = rig != null ? rig.Brain.transform : transform;
            Vector3 ahead = Vector3.ProjectOnPlane(view.forward, Vector3.up).normalized;
            int n = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * 0.9f, range, buffer, mask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < n; i++)
            {
                var it = buffer[i].GetComponentInParent<IInteractable>();
                if (it == null || !it.CanInteract(this)) continue;
                // The camera sits behind Lukas, so something behind him can still be near the centre of the view.
                if (Vector3.Dot(it.InteractionPoint - transform.position, ahead) < 0f) continue;
                if (!HasLineOfSight(transform.position + Vector3.up * 1.2f, it.InteractionPoint, wallMask)) continue;
                float s = ViewScore(it.InteractionPoint);
                if (s > bestScore) { bestScore = s; best = it; }
            }
            return best;
        }

        private void OnInteract(InputAction.CallbackContext _)
        {
            if (Current == null || player.MovementLocked) return;
            if (Services.TryGet(out PauseService pause) && (pause.IsReading || pause.ReadingEndedFrame == Time.frameCount)) return;   // that E closes the reading
            if (router.Context == InputContext.OverBody && Services.TryGet<Combat.PlayerFinisher>(out var f) && f.HasTarget) return;
            if (Current is IHoldInteractable h && h.CanHold)
            {
                // Decided on release or after the hold time (TickHold): a tap interacts, a full hold runs the hold action.
                holdTarget = h;
                hold.Begin(SystemsBalance.Current.sealTapSeconds, SystemsBalance.Current.sealHoldSeconds);
                return;
            }
            Use(Current);
        }

        private void Use(IInteractable target)
        {
            HbLog.Info("Interact", $"Interact {((MonoBehaviour)target).name}");
            GameEvents.RaisePlayerGesture(GestureFor(target));
            target.Interact(this);
        }

        private string GestureFor(IInteractable target)
        {
            if (target is DocumentPickup || target is UI.TutorialPoster) return "Examine";
            float height = target.InteractionPoint.y - transform.position.y;
            if (height < 0.6f) return "Pick_Floor";
            string kind = target.GetType().Name;
            if (kind.Contains("Door")) return "Use_Door";
            if (kind.Contains("Switch")) return "PowerSwitch";
            return height > 1.2f ? "Pick_Shelf" : "Use_Panel";
        }
    }
}
