namespace Heartbeat.Cycle
{
    /// <summary>
    /// Death rules (M-16, M-18, D-35, D-63): each death raises the subject code (R-H-07 → 08…) and degradation;
    /// 5 consecutive deaths since the last reset trigger the Assimilation ending. Only reaching floor 2's final key
    /// room resets the counter. Deaths in the first (tutorial) attempt and the scripted intro death do not count.
    /// Pure logic over a SlotData-like state.
    /// </summary>
    public static class DeathCounter
    {
        public const int AssimilationThreshold = 5;
        public const int MaxDegradation = 10;

        public struct State
        {
            public int Consecutive;
            public int Total;
            public int SubjectCode;
            public int Degradation;
            public bool FirstAttempt;
        }

        /// <summary>Applies a death. Returns true if the counter has reached the threshold (see <see cref="Assimilates"/>).</summary>
        public static bool RegisterDeath(ref State s, bool scripted = false)
        {
            if (scripted) return false;                   // intro death (D-63)
            s.Total++;
            s.SubjectCode++;
            bool counts = !s.FirstAttempt;                // tutorial deaths do not count (D-35)...
            s.FirstAttempt = false;
            return AddCost(ref s, counts);
        }

        /// <summary>Quitting calmly outside a saved final key room (D-31, D-94): +1 counter, degradation and subject code, same attempt.</summary>
        public static bool RegisterCalmExit(ref State s)
        {
            s.SubjectCode++;
            return AddCost(ref s, !s.FirstAttempt);
        }

        private static bool AddCost(ref State s, bool counts)
        {
            if (s.Degradation < MaxDegradation) s.Degradation++;   // ...but always degrade (D-94)
            if (!counts) return false;
            s.Consecutive++;
            return s.Consecutive >= AssimilationThreshold;
        }

        /// <summary>
        /// D-149: Restart and costly quits still count, but the Assimilation ending only plays when Lukas actually dies
        /// (health or a grab in Critical) with the counter at the threshold — never on Restart, quitting or Continue.
        /// </summary>
        public static bool Assimilates(int consecutive, Core.DeathCause cause) =>
            (cause == Core.DeathCause.Health || cause == Core.DeathCause.GrabInCritical) && consecutive >= AssimilationThreshold;

        /// <summary>Reaching floor 2's final key room (D-35).</summary>
        public static void ResetAtFloor2Final(ref State s) => s.Consecutive = 0;
    }
}
