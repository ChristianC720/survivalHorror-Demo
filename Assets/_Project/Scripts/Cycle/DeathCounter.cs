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

        /// <summary>Applies a death. Returns true if the Assimilation ending must play.</summary>
        public static bool RegisterDeath(ref State s, bool scripted = false)
        {
            if (scripted) return false;                   // intro death (D-63)
            s.Total++;
            s.SubjectCode++;
            if (s.FirstAttempt)                           // tutorial deaths do not count (D-35)
            {
                s.FirstAttempt = false;
                return false;
            }
            s.Consecutive++;
            if (s.Degradation < MaxDegradation) s.Degradation++;
            return s.Consecutive >= AssimilationThreshold;
        }

        /// <summary>Reaching floor 2's final key room (D-35).</summary>
        public static void ResetAtFloor2Final(ref State s) => s.Consecutive = 0;
    }
}
