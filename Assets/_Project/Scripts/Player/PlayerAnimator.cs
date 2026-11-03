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
    /// faces the camera (D-141) and so walks sideways or backwards relative to his chest.
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

        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int DirId = Animator.StringToHash("Dir");
        private static readonly int HealthId = Animator.StringToHash("Health");
        private static readonly int CrouchId = Animator.StringToHash("Crouch");

        private InputRouter router;
        private Transform spine, chest;
        private float legYaw, upperBusy, fullBusy;
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
            legYaw = Mathf.MoveTowards(legYaw, target, legTurnSpeed * Time.deltaTime);

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
            // The hips carry the yaw; spine and chest undo it so the shoulders and the aim stay on the camera.
            animator.transform.localRotation = Quaternion.Euler(0f, legYaw, 0f);
            Vector3 up = transform.up;
            if (spine != null) spine.Rotate(up, -legYaw * 0.4f, Space.World);
            if (chest != null) chest.Rotate(up, -legYaw * 0.6f, Space.World);
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
