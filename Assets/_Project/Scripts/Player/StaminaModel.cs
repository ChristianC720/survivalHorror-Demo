using System;

namespace Heartbeat.Player
{
    /// <summary>
    /// One stamina bar for running, shoving and the ground-strike finisher (M-06). Pure logic: time is passed in.
    /// Values are percentages (0..100).
    /// </summary>
    public sealed class StaminaModel
    {
        public const float Max = 100f;

        private readonly float regenDelay;
        private readonly float regenPerSecond;
        private readonly float exhaustionSeconds;

        private float sinceSpend;
        private float exhaustedLeft;

        public float Value { get; private set; } = Max;
        public bool IsExhausted => exhaustedLeft > 0f;
        public float Normalized => Value / Max;

        public event Action Exhausted;

        public StaminaModel(float regenDelay, float regenPerSecond, float exhaustionSeconds)
        {
            this.regenDelay = regenDelay;
            this.regenPerSecond = regenPerSecond;
            this.exhaustionSeconds = exhaustionSeconds;
            sinceSpend = regenDelay;
        }

        /// <summary>Shove / ground strike: all or nothing. Fails while exhausted or if there is not enough.</summary>
        public bool TrySpend(float amount)
        {
            if (IsExhausted || Value < amount) return false;
            Spend(amount);
            return true;
        }

        public bool CanRun => !IsExhausted && Value > 0f;

        /// <summary>Continuous drain while running. Reaching 0 triggers exhaustion.</summary>
        public void Drain(float perSecond, float dt)
        {
            if (IsExhausted) return;
            Spend(perSecond * dt);
        }

        public void Tick(float dt)
        {
            if (exhaustedLeft > 0f)
            {
                exhaustedLeft = Math.Max(0f, exhaustedLeft - dt);
            }
            sinceSpend += dt;
            if (sinceSpend >= regenDelay && Value < Max)
                Value = Math.Min(Max, Value + regenPerSecond * dt);
        }

        public void Refill()
        {
            Value = Max;
            exhaustedLeft = 0f;
            sinceSpend = regenDelay;
        }

        private void Spend(float amount)
        {
            Value = Math.Max(0f, Value - amount);
            sinceSpend = 0f;
            if (Value <= 0f && exhaustedLeft <= 0f)
            {
                exhaustedLeft = exhaustionSeconds;
                Exhausted?.Invoke();
            }
        }
    }
}
