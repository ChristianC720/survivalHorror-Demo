using System;
using Heartbeat.Core;

namespace Heartbeat.AI
{
    /// <summary>Tuning for <see cref="AlertModel"/>. Comes from EnemyDefinition.</summary>
    [Serializable]
    public struct AlertTuning
    {
        public float sightGainPerSecond;     // level gained per second when seen at close range
        public float farSightFactor;         // fraction of the gain at the edge of the sight range
        public float huntLoseSeconds;        // hunt continues this long without seeing Lukas
        public float searchHoldSeconds;      // search stays this long before decaying
        public float decayPerSecond;         // level lost per second once holds expire

        public static AlertTuning Default => new AlertTuning
        {
            sightGainPerSecond = 1.2f,
            farSightFactor = 0.35f,
            huntLoseSeconds = 5f,
            searchHoldSeconds = 6f,
            decayPerSecond = 0.12f,
        };
    }

    /// <summary>
    /// Alert meter of one enemy (S-02, M-09, M-10). Level 0..1 maps to Patrol → Suspicion (0.15) → Search (0.5)
    /// → Hunt (1.0). Seeing Lukas raises it (x2 when the flashlight lights the enemy, M-12); loud noises jump to
    /// Search, footsteps to Suspicion; The Mother's full attention pulls patrols into Search (D-24).
    /// Hunt is kept while Lukas is seen and drops to Search after <c>huntLoseSeconds</c> without sight.
    /// Pure logic: EditMode-tested.
    /// </summary>
    public sealed class AlertModel
    {
        public const float SuspicionThreshold = 0.15f;
        public const float SearchThreshold = 0.5f;
        public const float HuntThreshold = 1f;

        private readonly AlertTuning t;
        private float sinceSeen = float.MaxValue;
        private float sinceStimulus = float.MaxValue;

        public float Level { get; private set; }
        public AlertState State { get; private set; } = AlertState.Patrol;

        public event Action<AlertState, AlertState> StateChanged;

        public AlertModel(AlertTuning tuning) { t = tuning; }

        /// <param name="sees">Lukas is inside the sight cone with a clear line of sight.</param>
        /// <param name="proximity01">1 = touching, 0 = at the edge of the sight range.</param>
        /// <param name="lit">The flashlight beam is on this enemy.</param>
        public void Tick(float dt, bool sees, float proximity01, bool lit)
        {
            if (escalateIn >= 0f)
            {
                escalateIn -= dt;
                if (escalateIn < 0f && Level < SearchThreshold) { Level = SearchThreshold + 0.1f; sinceStimulus = 0f; }
            }
            if (sees)
            {
                float factor = t.farSightFactor + (1f - t.farSightFactor) * Clamp01(proximity01);
                Level = Math.Min(HuntThreshold, Level + t.sightGainPerSecond * factor * (lit ? 2f : 1f) * dt);
                sinceSeen = 0f;
                sinceStimulus = 0f;
            }
            else
            {
                sinceSeen += dt;
                sinceStimulus += dt;
                if (State == AlertState.Hunt)
                {
                    if (sinceSeen >= t.huntLoseSeconds) Level = Math.Min(Level, HuntThreshold - 0.25f); // → Search
                }
                else if (escalateIn < 0f && (sinceStimulus >= t.searchHoldSeconds || Level < SearchThreshold))
                {
                    Level = Math.Max(0f, Level - t.decayPerSecond * dt);
                }
            }
            UpdateState();
        }

        /// <summary>A heard noise. Loud sources jump to Search; quiet ones only to Suspicion. Never lowers the level.</summary>
        public void Hear(NoiseSource source)
        {
            bool loud = source == NoiseSource.Shot || source == NoiseSource.GroundStrike
                        || source == NoiseSource.Machinery || source == NoiseSource.Scream || source == NoiseSource.Running;
            float floor = loud ? SearchThreshold + 0.1f : SuspicionThreshold + 0.1f;
            if (Level < floor) Level = floor;
            sinceStimulus = 0f;
            UpdateState();
        }

        private float escalateIn = -1f;

        /// <summary>
        /// The Mother's attention is full (D-24, D-98): a patrolling enemy turns suspicious, then searches towards Lukas after
        /// <paramref name="suspicionSeconds"/>. Enemies already searching or hunting stay as they are.
        /// </summary>
        public void OnAttentionFull(float suspicionSeconds)
        {
            if (Level >= SearchThreshold) return;
            if (Level < SuspicionThreshold + 0.1f) Level = SuspicionThreshold + 0.1f;
            sinceStimulus = 0f;
            escalateIn = suspicionSeconds;
            UpdateState();
        }

        /// <summary>Directly enter Hunt (e.g. damaged by a shot, grabbed target, scripted).</summary>
        public void ForceHunt()
        {
            Level = HuntThreshold;
            sinceSeen = 0f;
            sinceStimulus = 0f;
            UpdateState();
        }

        public void Reset()
        {
            escalateIn = -1f;
            Level = 0f;
            UpdateState();
        }

        private void UpdateState()
        {
            AlertState next;
            if (State == AlertState.Hunt && Level >= HuntThreshold - 0.0001f) next = AlertState.Hunt;
            else if (Level >= HuntThreshold - 0.0001f) next = AlertState.Hunt;
            else if (Level >= SearchThreshold) next = AlertState.Search;
            else if (Level >= SuspicionThreshold) next = AlertState.Suspicion;
            else next = AlertState.Patrol;

            if (next == State) return;
            var prev = State;
            State = next;
            StateChanged?.Invoke(prev, next);
        }

        private static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
    }
}
