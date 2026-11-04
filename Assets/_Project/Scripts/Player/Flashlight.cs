using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Player
{
    /// <summary>
    /// Lukas's flashlight (M-12): F / D-pad up toggles; beam narrows while aiming (M-01); enemies inside the beam with a
    /// clear line are marked lit (alert x2, M-12; La Boca is blind). The only warm-white light source (art direction).
    /// Charge readout lives in the arm terminal (E-10).
    /// </summary>
    public sealed class Flashlight : MonoBehaviour
    {
        [SerializeField] private Light spot;
        [SerializeField] private PlayerController player;
        [SerializeField] private float exploreAngle = 55f;
        [SerializeField] private float aimAngle = 28f;
        [SerializeField] private float range = 14f;
        [SerializeField] private float intensity = 18f;

        private InputRouter router;
        private AlertDirector director;
        private CameraSystem.OtsCameraRig rig;
        private int envMask;
        private float flickerTimer;
        private bool steadyFlicker;

        public FlashlightModel Model { get; } = new FlashlightModel();
        public bool IsOn => Model.IsOn;

        private void Start()
        {
            envMask = LayerMask.GetMask("Environment");
            router = Services.Get<InputRouter>();
            router.Actions.Gameplay.Flashlight.performed += OnToggle;
            Model.Depleted += OnDepleted;
            spot.range = range;
            Apply();
        }

        private void OnDestroy()
        {
            if (router != null) router.Actions.Gameplay.Flashlight.performed -= OnToggle;
        }

        private void OnToggle(InputAction.CallbackContext _)
        {
            bool before = Model.IsOn;
            bool now = Model.Toggle();
            if (!before && !now) HbLog.Info("Flashlight", Model.IsLocked ? "Locked (depleted)" : "Empty");
            else HbLog.Info("Flashlight", now ? "On" : "Off");
            Sfx.Play2D(Sfx.FlashlightToggle);
            Apply();
        }

        private void OnDepleted()
        {
            HbLog.Info("Flashlight", "Depleted: flicker and off, locked ~10 s");
            flickerTimer = 0.8f;
            steadyFlicker = Services.TryGet(out Save.SaveService saves) && saves.LoadGlobal().reduceFlashes;
            Sfx.Play2D(Sfx.FlashlightFlicker);
        }

        private void Update()
        {
            Model.Tick(Time.deltaTime);
            if (flickerTimer > 0f)
            {
                flickerTimer -= Time.deltaTime;
                spot.enabled = flickerTimer > 0f && (steadyFlicker || Random.value > 0.5f);   // reduce flashes: just goes out
            }
            else Apply();

            if (rig != null || Services.TryGet(out rig))
                spot.transform.rotation = Quaternion.Slerp(spot.transform.rotation, Quaternion.LookRotation(rig.Brain.transform.forward), 12f * Time.deltaTime);
            spot.spotAngle = Mathf.Lerp(spot.spotAngle, player.IsAiming ? aimAngle : exploreAngle, 10f * Time.deltaTime);
            MarkLitEnemies();
        }

        private void Apply()
        {
            spot.enabled = Model.IsOn;
            spot.intensity = intensity;
        }

        private void MarkLitEnemies()
        {
            if (director == null && !Services.TryGet(out director)) return;
            Vector3 origin = spot.transform.position;
            Vector3 fwd = spot.transform.forward;
            float half = spot.spotAngle * 0.5f;
            foreach (var e in director.Enemies)
            {
                if (e == null) continue;
                bool lit = false;
                if (Model.IsOn && e.IsActive)
                {
                    Vector3 target = e.transform.position + Vector3.up * 1.2f;
                    Vector3 to = target - origin;
                    float d = to.magnitude;
                    lit = d <= range && Vector3.Angle(fwd, to) <= half
                          && !Physics.Raycast(origin, to / d, d, envMask, QueryTriggerInteraction.Ignore);
                }
                e.IsLit = lit;
            }
        }

        /// <summary>Debug / tests.</summary>
        public void DebugSetOn(bool on)
        {
            if (on != Model.IsOn) Model.Toggle();
            Apply();
        }
    }
}
