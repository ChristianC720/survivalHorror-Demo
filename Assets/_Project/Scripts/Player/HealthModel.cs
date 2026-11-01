using System;

namespace Heartbeat.Player
{
    public enum HealthState { Fine, Injured, Critical, Dead }

    /// <summary>
    /// Hidden health split into 3 equal bands (M-05, D-119). No regeneration.
    /// A hit removes max/hitsToKill (Alto and Mudo 2, Boca 3, Rastrero 4).
    /// A heal raises to the top of the next state, or to full when Fine with damage.
    /// A grab without a spike drops to the top of the lower state; in Critical it kills.
    /// </summary>
    public sealed class HealthModel
    {
        public float Max { get; }
        public float Value { get; private set; }
        public HealthState State => StateFor(Value);
        public bool IsDead => Value <= 0f;

        public event Action<HealthState> StateChanged;

        public HealthModel(float max)
        {
            Max = max;
            Value = max;
        }

        private float Band => Max / 3f;

        public HealthState StateFor(float value)
        {
            if (value <= 0f) return HealthState.Dead;
            if (value > Band * 2f + 0.0001f) return HealthState.Fine;
            if (value > Band + 0.0001f) return HealthState.Injured;
            return HealthState.Critical;
        }

        public void ApplyHit(int hitsToKill)
        {
            if (IsDead || hitsToKill <= 0) return;
            SetValue(Value - Max / hitsToKill);
        }

        public void ApplyGrabWithoutSpike()
        {
            if (IsDead) return;
            switch (State)
            {
                case HealthState.Fine: SetValue(Band * 2f); break;     // top of Injured
                case HealthState.Injured: SetValue(Band); break;       // top of Critical
                default: SetValue(0f); break;                          // Critical: death (Reglas y Estados)
            }
        }

        /// <summary>Not allowed at full health (D-119).</summary>
        public bool CanHeal => !IsDead && Value < Max - 0.0001f;

        public bool TryHeal()
        {
            if (!CanHeal) return false;
            switch (State)
            {
                case HealthState.Critical: SetValue(Band * 2f); break; // → top of Injured
                default: SetValue(Max); break;                         // Injured → top of Fine = full; Fine with damage → full
            }
            return true;
        }

        public void Restore() => SetValue(Max);

        public void Kill() => SetValue(0f);

        /// <summary>Restores hidden health (floor carry-over, autosave).</summary>
        public void SetValue(float v)
        {
            var before = State;
            Value = Math.Max(0f, Math.Min(Max, v));
            var after = State;
            if (after != before) StateChanged?.Invoke(after);
        }
    }
}
