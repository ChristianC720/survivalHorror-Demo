using System;

namespace Heartbeat.Player
{
    /// <summary>
    /// Flashlight charge (M-12, D-71, D-124): ~120 s of continuous use; after ~3 s off it recharges by itself and
    /// fills 0→100% in ~30 s; at 0 it flickers, turns off and cannot be turned on for ~10 s. Pure logic.
    /// </summary>
    public sealed class FlashlightModel
    {
        private readonly float drainSeconds;
        private readonly float rechargeDelay;
        private readonly float rechargeSeconds;
        private readonly float lockSeconds;

        private float sinceOff;
        private float lockLeft;

        public bool IsOn { get; private set; }
        /// <summary>0..1</summary>
        public float Charge { get; private set; } = 1f;
        public bool IsLocked => lockLeft > 0f;

        public event Action Depleted;

        public FlashlightModel(float drainSeconds = 120f, float rechargeDelay = 3f, float rechargeSeconds = 30f, float lockSeconds = 10f)
        {
            this.drainSeconds = drainSeconds;
            this.rechargeDelay = rechargeDelay;
            this.rechargeSeconds = rechargeSeconds;
            this.lockSeconds = lockSeconds;
        }

        /// <summary>F / D-pad up. Returns the new state; turning on fails while locked or empty.</summary>
        public bool Toggle()
        {
            if (IsOn) { IsOn = false; sinceOff = 0f; return false; }
            if (IsLocked || Charge <= 0f) return false;
            IsOn = true;
            return true;
        }

        public void ForceOff()
        {
            if (!IsOn) return;
            IsOn = false;
            sinceOff = 0f;
        }

        public void Tick(float dt)
        {
            if (lockLeft > 0f) lockLeft = Math.Max(0f, lockLeft - dt);

            if (IsOn)
            {
                Charge = Math.Max(0f, Charge - dt / drainSeconds);
                if (Charge <= 0f)
                {
                    IsOn = false;
                    sinceOff = 0f;
                    lockLeft = lockSeconds;
                    Depleted?.Invoke();
                }
                return;
            }

            sinceOff += dt;
            if (sinceOff >= rechargeDelay) Charge = Math.Min(1f, Charge + dt / rechargeSeconds);
        }
    }
}
