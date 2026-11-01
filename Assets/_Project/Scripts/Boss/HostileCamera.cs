using Heartbeat.AI;
using Heartbeat.Audio;
using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Boss
{
    /// <summary>
    /// Phase 2 hostile camera (D-28, D-98): sweeps, shows a red light and motor sound; ~1 s on Lukas locks and
    /// releases an assimilated towards him.
    /// </summary>
    public sealed class HostileCamera : MonoBehaviour
    {
        [SerializeField] private BossBalance balance;
        [SerializeField] private Light redLight;

        private bool active;
        private float baseYaw, clock, focus, cooldown;
        private int envMask;
        private AudioSource motor;

        public bool IsActive => active;

        private void Awake()
        {
            baseYaw = transform.eulerAngles.y;
            envMask = LayerMask.GetMask("Environment");
            if (redLight != null) redLight.enabled = false;
        }

        public void SetActive(bool on)
        {
            active = on;
            if (redLight != null) { redLight.enabled = on; redLight.intensity = 2f; }
            if (!Services.TryGet(out AudioService audio)) return;
            if (on && motor == null && audio.TryGetCue("CUE_HostileCamera_Motor", out var cue)) motor = audio.StartLoop(cue, transform);
            if (!on && motor != null) { audio.Stop(motor); motor = null; }
        }

        private void OnDisable() => SetActive(false);

        private void Update()
        {
            if (!active) return;
            float dt = Time.deltaTime;
            cooldown -= dt;
            clock += dt;
            float half = balance.cameraSweepHalfAngle;
            float period = 4f * half / balance.cameraSweepDegreesPerSecond;
            float yaw = baseYaw + Mathf.PingPong(clock / period * 4f * half, 2f * half) - half;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yaw, 0f);

            bool sees = Services.TryGet(out PlayerController p) && Sees(p.transform.position + Vector3.up * 1.2f);
            focus = sees && cooldown <= 0f ? focus + dt : 0f;
            if (redLight != null) redLight.intensity = focus > 0f ? 8f : 2f;
            if (focus >= balance.cameraLockSeconds)
            {
                focus = 0f;
                cooldown = balance.cameraCooldown;
                Sfx.Play("CUE_HostileCamera_Lock", transform.position);
                Services.Get<MotherDirector>().CameraLocked(this, p.transform.position);
            }
        }

        private bool Sees(Vector3 target)
        {
            Vector3 to = target - transform.position;
            float d = to.magnitude;
            if (d > balance.cameraRange) return false;
            Vector3 flatFwd = transform.forward; flatFwd.y = 0f;
            Vector3 flatTo = to; flatTo.y = 0f;
            if (Vector3.Angle(flatFwd, flatTo) > balance.cameraConeHalfAngle) return false;
            return !Physics.Raycast(transform.position, to / d, d - 0.3f, envMask, QueryTriggerInteraction.Ignore);
        }
    }
}
