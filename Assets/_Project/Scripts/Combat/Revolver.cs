using Heartbeat.CameraSystem;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Inventory;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Combat
{
    /// <summary>
    /// The Kessler revolver (M-04). Fires only while aiming (left click / RT); R / X reloads round by round from the
    /// inventory. Hitscan from the screen centre with the current spread. Every shot is a 20 m noise (M-09).
    /// Ammo state is diegetic: the counter on the frame (<see cref="RevolverCounter"/>), no HUD.
    /// </summary>
    public sealed class Revolver : MonoBehaviour
    {
        [SerializeField] private CombatBalance balance;
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerHealth health;
        [Tooltip("Muzzle used for the noise origin and debug lines.")]
        [SerializeField] private Transform muzzle;

        private InputRouter router;
        private OtsCameraRig rig;
        private int shotMask;
        private bool hitThisFrame;
        private System.Func<bool> takeRound;

        public RevolverModel Model { get; private set; }

        private void Awake()
        {
            Model = new RevolverModel(balance.drumSize, balance.secondsBetweenShots, balance.reloadSecondsPerRound,
                balance.spreadCloseSeconds, balance.spreadPerShot);
            shotMask = LayerMask.GetMask("Environment", "Enemy", "Hitbox");
        }

        private void Start()
        {
            router = Services.Get<InputRouter>();
            Services.TryGet(out rig);
            router.Actions.Gameplay.Attack.performed += OnAttack;
            router.Actions.Gameplay.Reload.performed += OnReload;
            if (health != null) health.Hit += OnHit;
        }

        private void OnDestroy()
        {
            if (router == null) return;
            router.Actions.Gameplay.Attack.performed -= OnAttack;
            router.Actions.Gameplay.Reload.performed -= OnReload;
            if (health != null) health.Hit -= OnHit;
        }

        private void Update()
        {
            bool unsteady = player.CurrentSpeed > 0.1f || player.IsRunning || hitThisFrame;
            hitThisFrame = false;
            Model.Tick(Time.deltaTime, takeRound ??= TakeRound, unsteady);   // cached: no delegate allocation per frame
        }

        private void OnAttack(InputAction.CallbackContext _)
        {
            if (!player.IsAiming) return;   // not aiming: the shove handles Attack (Shove.cs)
            if (Model.IsReloading)
            {
                Model.CancelReload();
                HbLog.Info("Revolver", $"Reload cancelled, loaded={Model.Loaded}");
                return;
            }
            Fire();
        }

        private void OnReload(InputAction.CallbackContext _)
        {
            if (Model.BeginReload(inventory.Model.Has(ItemKind.Ammo)))
                HbLog.Info("Revolver", "Reload started");
        }

        private bool TakeRound()
        {
            if (!inventory.Model.TryConsume(ItemKind.Ammo)) return false;
            Sfx.Play(Sfx.RevolverReloadRound, transform.position);
            return true;
        }

        private void OnHit() => hitThisFrame = true;

        private const float ChestHeight = 1.4f;   // inside Lukas's capsule: the shot never starts inside a wall

        public FireResult Fire()
        {
            float spreadBeforeShot = Model.Spread;   // the recoil of this shot only affects the next one
            var result = Model.TryFire();
            switch (result)
            {
                case FireResult.DryFire:
                    HbLog.Info("Revolver", "Dry fire (click)");
                    Sfx.Play(Sfx.RevolverDryFire, transform.position);
                    return result;
                case FireResult.Fired:
                    break;
                default:
                    return result;
            }

            Ray ray = rig != null ? rig.AimRay() : new Ray(muzzle.position, muzzle.forward);
            float spreadDeg = balance.maxSpreadDegrees * spreadBeforeShot;
            Vector2 jitter = Random.insideUnitCircle * Mathf.Tan(spreadDeg * Mathf.Deg2Rad);
            Vector3 dir = (ray.direction + rayRight(ray) * jitter.x + Vector3.Cross(ray.direction, rayRight(ray)) * -jitter.y).normalized;

            // The camera (over the right shoulder) only decides WHERE Lukas aims; the bullet leaves his chest. Behind
            // cover on his left the camera could see past it and the shot landed through the wall (playtest).
            Vector3 chest = transform.position + Vector3.up * ChestHeight;
            float skip = Mathf.Max(0f, Vector3.Dot(chest - ray.origin, dir));   // ignore what is between camera and Lukas
            Vector3 camOrigin = ray.origin + dir * skip;
            Vector3 aimPoint = Physics.Raycast(camOrigin, dir, out var aimHit, balance.range, shotMask, QueryTriggerInteraction.Ignore)
                ? aimHit.point : camOrigin + dir * balance.range;
            Vector3 toAim = aimPoint - chest;
            float aimDist = toAim.magnitude;

            string hitName = "nothing";
            if (aimDist > 0.01f && Physics.Raycast(chest, toAim / aimDist, out var hit, aimDist + 0.05f, shotMask, QueryTriggerInteraction.Ignore))
            {
                dir = toAim / aimDist;
                hitName = hit.collider.name;
                var weak = hit.collider.GetComponent<WeakPoint>();
                var target = weak != null ? weak.Owner : hit.collider.GetComponentInParent<IDamageable>();
                if (target != null && target.IsAlive) target.ReceiveShot(new ShotHit(hit.point, dir, weak != null));
                Debug.DrawLine(chest, hit.point, Color.yellow, 0.5f);
            }

            Vector3 origin = muzzle != null ? muzzle.position : transform.position;
            GameEvents.RaiseNoiseEmitted(new NoiseInfo(origin, balance.shotNoiseRadius, NoiseSource.Shot));
            GameEvents.RaiseShotFired(origin);
            HbLog.Info("Revolver", $"Shot -> {hitName}, loaded={Model.Loaded}, spread={spreadBeforeShot:0.00}");
            return result;
        }

        private static Vector3 rayRight(Ray r) => Vector3.Cross(Vector3.up, r.direction).normalized;

        /// <summary>Reconstruction hands the revolver back unloaded (D-74).</summary>
        public void Unload() => Model.Unload();
    }
}
