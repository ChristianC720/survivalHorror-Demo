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
    /// Finds the interactable Lukas is looking at (M-08) and runs it on E / A. Among candidates within range, the one
    /// closest to the centre of the view wins (D-27), which also decides between a body finisher and a door/item.
    /// Never pauses the game. Raises InteractTargetChanged for the diegetic E prompt (E-10).
    /// </summary>
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField] private float range = 1.8f;
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerInventory inventory;

        private InputRouter router;
        private OtsCameraRig rig;
        private int mask;
        private readonly Collider[] buffer = new Collider[16];

        public IInteractable Current { get; private set; }
        /// <summary>How centred the current target is (dot of view direction, 1 = dead centre).</summary>
        public float CurrentScore { get; private set; } = -1f;
        public PlayerInventory Inventory => inventory;
        public PlayerController Player => player;

        private void Start()
        {
            mask = LayerMask.GetMask("Interactable");
            router = Services.Get<InputRouter>();
            Services.TryGet(out rig);
            router.Actions.Gameplay.Interact.performed += OnInteract;
        }

        private void OnEnable() => Services.Register(this);

        private void OnDisable() => Services.Unregister(this);

        private void OnDestroy()
        {
            if (router != null) router.Actions.Gameplay.Interact.performed -= OnInteract;
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
        }

        /// <summary>Score of any world point as seen from the camera (used by the finisher to compare).</summary>
        public float ViewScore(Vector3 point)
        {
            Transform view = rig != null ? rig.Brain.transform : transform;
            Vector3 to = point - view.position;
            return Vector3.Dot(view.forward, to.normalized);
        }

        private IInteractable FindBest(out float bestScore)
        {
            bestScore = 0.3f;   // must be roughly in view
            IInteractable best = null;
            int n = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * 0.9f, range, buffer, mask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < n; i++)
            {
                var it = buffer[i].GetComponentInParent<IInteractable>();
                if (it == null || !it.CanInteract(this)) continue;
                float s = ViewScore(it.InteractionPoint);
                if (s > bestScore) { bestScore = s; best = it; }
            }
            return best;
        }

        private void OnInteract(InputAction.CallbackContext _)
        {
            if (Current == null || player.MovementLocked) return;
            if (router.Context == InputContext.OverBody && Services.TryGet<Combat.PlayerFinisher>(out var f) && f.HasTarget) return;
            HbLog.Info("Interact", $"Interact {((MonoBehaviour)Current).name}");
            Current.Interact(this);
        }
    }
}
