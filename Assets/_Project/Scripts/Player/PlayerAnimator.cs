using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Level;
using UnityEngine;

namespace Heartbeat.Player
{
    /// <summary>
    /// Drives Lukas's Animator from gameplay state. Listens only; no gameplay depends on it. Clips stay in place (D-151):
    /// the CharacterController moves the body and this class turns the legs towards the movement, because Lukas always
    /// faces the camera (D-141) and so walks sideways or backwards relative to his chest. Without aiming the whole body
    /// turns with the legs; only the aim keeps the torso on the camera, with the hips capped so they never twist more
    /// than 45 degrees away from the chest (C-61).
    /// </summary>
    public sealed class PlayerAnimator : MonoBehaviour
    {
        private const int RightGripLayer = 1, UpperLayer = 2, FullLayer = 3;
        private const string Empty = "Empty";

        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private Revolver revolver;
        [SerializeField] private PlayerHack hack;
        [Tooltip("Degrees per second at which the legs turn towards the movement.")]
        [SerializeField] private float legTurnSpeed = 360f;
        [Tooltip("While aiming, the most the legs turn away from the chest, degrees. The walk clip adds about 6 on top (C-61: 45 at most).")]
        [SerializeField] private float maxAimTwist = 30f;
        [Tooltip("Seconds for the torso to blend onto the camera when aiming starts or stops.")]
        [SerializeField] private float aimTwistBlend = 0.15f;

        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int DirId = Animator.StringToHash("Dir");
        private static readonly int HealthId = Animator.StringToHash("Health");
        private static readonly int CrouchId = Animator.StringToHash("Crouch");

        private InputRouter router;
        private Transform spine, chest;
        private float legYaw, aimWeight, upperBusy, fullBusy;
        private string upperState = Empty;
        private bool dead;

        private void OnEnable()
        {
            GameEvents.PlayerGesture += OnGesture;
            GameEvents.ShotFired += OnShot;
            GameEvents.PlayerAttacked += OnAttacked;
            GameEvents.GrabResolved += OnGrabResolved;
            GameEvents.PlayerDied += OnDied;
            GameEvents.Reconstructed += OnReconstructed;
        }

        private void OnDisable()
        {
            GameEvents.PlayerGesture -= OnGesture;
            GameEvents.ShotFired -= OnShot;
            GameEvents.PlayerAttacked -= OnAttacked;
            GameEvents.GrabResolved -= OnGrabResolved;
            GameEvents.PlayerDied -= OnDied;
            GameEvents.Reconstructed -= OnReconstructed;
        }

        private void Start()
        {
            Services.TryGet(out router);
            spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        }

        private void Update()
        {
            Vector3 local = transform.InverseTransformDirection(player.Velocity);
            local.y = 0f;
            float speed = local.magnitude;
            bool backwards = local.z < -0.1f * Mathf.Max(speed, 0.01f);

            animator.SetFloat(SpeedId, speed, 0.08f, Time.deltaTime);
            animator.SetFloat(DirId, backwards ? -1f : 1f);
            animator.SetInteger(HealthId, (int)health.State);
            animator.SetBool(CrouchId, player.IsCrouched);
            animator.SetLayerWeight(RightGripLayer, revolver != null && revolver.Owned ? 1f : 0f);

            // Legs lead towards the movement; walking backwards plays the cycle in reverse instead of turning around.
            float target = speed > 0.1f ? Mathf.Atan2(local.x, Mathf.Abs(local.z)) * Mathf.Rad2Deg * (backwards ? -1f : 1f) : 0f;
            if (player.IsAiming) target = Mathf.Clamp(target, -maxAimTwist, maxAimTwist);
            if (upperBusy > 0f || fullBusy > 0f) target = 0f;   // a shove or a hand gesture plays towards the camera, where it acts
            legYaw = Mathf.MoveTowards(legYaw, target, legTurnSpeed * Time.deltaTime);
            aimWeight = Mathf.MoveTowards(aimWeight, player.IsAiming ? 1f : 0f, Time.deltaTime / Mathf.Max(aimTwistBlend, 0.01f));

            upperBusy -= Time.deltaTime;
            fullBusy -= Time.deltaTime;
            if (dead || upperBusy > 0f) return;
            string wanted = WantedUpperState();
            if (wanted != upperState) PlayUpper(wanted, 0.18f);
        }

        private string WantedUpperState()
        {
            if (hack != null && hack.IsHacking) return "Hack_Loop";
            if (router != null && router.Context == InputContext.Inventory) return "Terminal_Hold";
            if (revolver != null && revolver.Model != null && revolver.Model.IsReloading) return "Reload_Round";
            return player.IsAiming ? "Aim" : Empty;
        }

        private void LateUpdate()
        {
            if (dead) return;
            if (player.IsAiming && chest != null && player.CameraTransform != null)
            {
                // The torso follows the crosshair up and down; the clip only knows "level".
                Vector3 look = player.CameraTransform.forward;
                float pitch = Vector3.SignedAngle(Vector3.ProjectOnPlane(look, transform.up), look, transform.right);
                chest.Rotate(transform.right, pitch, Space.World);
            }
            if (Mathf.Abs(legYaw) < 0.01f) { animator.transform.localRotation = Quaternion.identity; return; }
            // The whole model carries the yaw. Only while aiming do spine and chest undo it, capped at maxAimTwist, so the
            // shoulders and the aim stay on the camera without the hips turning away from the chest (C-61).
            animator.transform.localRotation = Quaternion.Euler(0f, legYaw, 0f);
            float undo = Mathf.Clamp(legYaw, -maxAimTwist, maxAimTwist) * aimWeight;
            if (Mathf.Abs(undo) < 0.01f) return;
            Vector3 up = transform.up;
            if (spine != null) spine.Rotate(up, -undo * 0.4f, Space.World);
            if (chest != null) chest.Rotate(up, -undo * 0.6f, Space.World);
        }

        /// <summary>Angle in degrees between the hip line and the shoulder line on the horizontal plane (C-61 test metric).</summary>
        public static float HipChestTwist(Animator animator)
        {
            Transform ll = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg), rl = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            Transform la = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm), ra = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Vector3 up = animator.transform.parent != null ? animator.transform.parent.up : Vector3.up;
            return Vector3.Angle(Vector3.ProjectOnPlane(rl.position - ll.position, up), Vector3.ProjectOnPlane(ra.position - la.position, up));
        }

        private void PlayUpper(string state, float fade, float busy = 0f)
        {
            animator.CrossFadeInFixedTime(state, fade, UpperLayer);
            upperState = state;
            upperBusy = busy;
        }

        private void PlayFull(string state, float fade)
        {
            animator.CrossFadeInFixedTime(state, fade, FullLayer);
            fullBusy = LengthOf(state);
            upperBusy = fullBusy;
        }

        private float LengthOf(string state)
        {
            foreach (var c in animator.runtimeAnimatorController.animationClips)
                if (c.name == "AN_Lukas_" + state) return c.length;
            return 0.5f;
        }

        private void OnGesture(string gesture)
        {
            if (dead) return;
            if (gesture == "GroundStrike" || gesture == "Finisher" || gesture == "Pick_Floor") PlayFull(gesture, 0.1f);
            else PlayUpper(gesture, 0.1f, LengthOf(gesture));
        }

        private void OnShot(Vector3 _)
        {
            if (!dead) PlayUpper("Fire", 0.02f, LengthOf("Fire"));
        }

        private void OnAttacked(AttackInfo attack)
        {
            if (!dead && !attack.IsGrab) PlayUpper("Hit", 0.04f, LengthOf("Hit"));
        }

        private void OnGrabResolved(bool spikeUsed)
        {
            if (!dead) PlayFull(spikeUsed ? "Grab_Release" : "Grab_Struggle", 0.06f);
        }

        private void OnDied(DeathCause cause)
        {
            if (cause != DeathCause.Health && cause != DeathCause.GrabInCritical) return;
            dead = true;
            animator.CrossFadeInFixedTime(Empty, 0.1f, UpperLayer);
            upperState = Empty;
            animator.CrossFadeInFixedTime(cause == DeathCause.Health ? "Death_Hit" : "Death_Grab", 0.08f, FullLayer);
        }

        private void OnReconstructed(int _)
        {
            dead = false;
            upperBusy = fullBusy = 0f;
            upperState = Empty;
            animator.CrossFadeInFixedTime(Empty, 0f, UpperLayer);
            animator.CrossFadeInFixedTime(Empty, 0f, FullLayer);
        }
    }
}
