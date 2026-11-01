using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.CameraSystem
{
    /// <summary>
    /// Over-the-shoulder camera and aiming (M-01, S-01). Lives on the PlayerCamera prefab; finds Lukas through
    /// Services. Look input rotates a yaw/pitch target; holding Aim raises the aim camera's priority, slows Lukas,
    /// turns him to the camera yaw and shows the reticle. Running cancels aim (Controles).
    /// </summary>
    public sealed class OtsCameraRig : MonoBehaviour
    {
        [SerializeField] private CameraBalance balance;
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineCamera exploreCamera;
        [SerializeField] private CinemachineCamera aimCamera;
        [SerializeField] private GameObject reticle;

        private PlayerController player;
        private InputRouter router;
        private Transform target;
        private float yaw;
        private float pitch;
        private bool aiming;

        public bool IsAiming => aiming;
        public Transform Target => target;
        public float Yaw => yaw;
        public CinemachineBrain Brain => brain;

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Start()
        {
            router = Services.Get<InputRouter>();
            if (!Services.TryGet(out player))
            {
                HbLog.Error("Camera", "No PlayerController registered; camera rig disabled");
                enabled = false;
                return;
            }

            target = new GameObject("CameraTarget").transform;
            target.SetParent(player.transform, false);
            target.localPosition = Vector3.up * balance.targetHeight;
            yaw = player.transform.eulerAngles.y;

            Configure(exploreCamera, balance.exploreShoulder, balance.exploreDistance, balance.exploreFov, 10);
            Configure(aimCamera, balance.aimShoulder, balance.aimDistance, balance.aimFov, 5);
            brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, balance.aimBlendSeconds);
            player.CameraTransform = brain.transform;
            if (reticle != null) reticle.SetActive(false);
        }

        private void Configure(CinemachineCamera cam, Vector3 shoulder, float distance, float fov, int priority)
        {
            cam.Target.TrackingTarget = target;
            cam.Lens.FieldOfView = fov;
            cam.Priority.Value = priority;
            var follow = cam.GetComponent<CinemachineThirdPersonFollow>();
            follow.ShoulderOffset = shoulder;
            follow.CameraDistance = distance;
            follow.VerticalArmLength = 0f;
            follow.CameraSide = 1f;
            follow.AvoidObstacles.Enabled = true;
            follow.AvoidObstacles.CollisionFilter = LayerMask.GetMask("Environment");
            follow.AvoidObstacles.CameraRadius = 0.15f;
        }

        private void LateUpdate()
        {
            if (Time.timeScale <= 0f) return;
            var gameplay = router.Actions.Gameplay;

            if (gameplay.Look.enabled) ApplyLook(gameplay.Look);
            UpdateAim(gameplay.Aim.enabled && gameplay.Aim.IsPressed() && !player.IsRunning && !player.MovementLocked);

            float h = player.IsCrouched ? balance.crouchTargetHeight : balance.targetHeight;
            target.localPosition = Vector3.Lerp(target.localPosition, Vector3.up * h, 10f * Time.deltaTime);
            target.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void ApplyLook(InputAction look)
        {
            Vector2 v = look.ReadValue<Vector2>();
            bool stick = look.activeControl != null && look.activeControl.device is Gamepad;
            float sens = aiming ? balance.aimMouseSensitivity : balance.mouseSensitivity;
            Vector2 delta = stick ? v / 300f * balance.stickDegreesPerSecond * Time.deltaTime : v * sens;
            yaw += delta.x;
            pitch += balance.invertY ? delta.y : -delta.y;
            pitch = Mathf.Clamp(pitch, balance.minPitch, balance.maxPitch);
        }

        private void UpdateAim(bool wantsAim)
        {
            if (wantsAim)
                player.transform.rotation = Quaternion.Euler(0f, yaw, 0f);   // Lukas faces where he aims

            if (wantsAim == aiming) return;
            aiming = wantsAim;
            player.IsAiming = aiming;
            aimCamera.Priority.Value = aiming ? 20 : 5;
            if (reticle != null) reticle.SetActive(aiming);

            var ctx = router.Context;
            if (aiming && (ctx == InputContext.Exploration || ctx == InputContext.OverBody)) router.SetContext(InputContext.Aiming);
            else if (!aiming && ctx == InputContext.Aiming) router.SetContext(InputContext.Exploration);

            GameEvents.RaiseAimChanged(aiming);
            HbLog.Info("Camera", aiming ? "Aim on" : "Aim off");
        }

        /// <summary>Ray from the screen centre (used by the revolver, E-04).</summary>
        public Ray AimRay() => new Ray(brain.transform.position, brain.transform.forward);
    }
}
