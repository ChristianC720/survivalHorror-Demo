using System;

namespace Heartbeat.CameraSystem
{
    /// <summary>
    /// The Mother's hidden attention (M-02, D-24): fills while the surveillance view is open (~5 s to full), shots add
    /// a little, and it drains slowly when closed. When full it fires once and restarts. Never shown on screen.
    /// </summary>
    public sealed class AttentionMeter
    {
        private readonly float fillSeconds;
        private readonly float drainSeconds;

        public float Value { get; private set; }
        public event Action Full;

        public AttentionMeter(float fillSeconds = 5f, float drainSeconds = 15f)
        {
            this.fillSeconds = fillSeconds;
            this.drainSeconds = drainSeconds;
        }

        public void Tick(float dt, bool viewing)
        {
            if (viewing) Add(dt / fillSeconds);
            else Value = Math.Max(0f, Value - dt / drainSeconds);
        }

        /// <summary>Shots and other loud acts feed The Mother's attention (D-24).</summary>
        public void Add(float amount)
        {
            Value += amount;
            if (Value < 1f) return;
            Value = 0f;
            Full?.Invoke();
        }
    }
}
