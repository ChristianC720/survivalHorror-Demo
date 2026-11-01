namespace Heartbeat.Audio
{
    /// <summary>Pure voice-allocation rule for the AudioService pool (testable without audio).</summary>
    public static class VoicePicker
    {
        /// <summary>
        /// Returns a free voice; otherwise steals the least important busy voice that is not more important than the
        /// new cue (ties: the oldest). -1 = drop the new sound. Priority values: lower = more important.
        /// </summary>
        public static int Pick(bool[] busy, int[] priority, float[] startTime, int newPriority)
        {
            int victim = -1;
            for (int i = 0; i < busy.Length; i++)
            {
                if (!busy[i]) return i;
                if (priority[i] < newPriority) continue;
                if (victim < 0 || priority[i] > priority[victim]
                    || (priority[i] == priority[victim] && startTime[i] < startTime[victim]))
                    victim = i;
            }
            return victim;
        }
    }
}
