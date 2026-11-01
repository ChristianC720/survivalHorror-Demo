using System;

namespace Heartbeat.Audio
{
    /// <summary>Pure heartbeat rules (S-11), testable without audio.</summary>
    public static class HeartbeatModel
    {
        /// <summary>
        /// Quantized threat band: -1 = no audible threat, 0 = far … n-1 = closest. Bands, not a continuous value,
        /// so the heartbeat stays approximate and never works as a radar (S-11).
        /// </summary>
        public static int ThreatBand(float nearestDistance, float[] bandEdges)
        {
            int band = -1;
            for (int i = 0; i < bandEdges.Length; i++)
                if (nearestDistance <= bandEdges[i]) band = i;
            return band;
        }

        /// <summary>Lab heartbeat volume: grows with degradation up to a cap; Critical adds a removable bonus (D-95).</summary>
        public static float LabVolume(AudioBalance b, int degradation, bool critical)
        {
            float v = Math.Min(b.labMaxVolume, b.labBaseVolume + b.labVolumePerDegradation * degradation);
            return Math.Min(b.labMaxVolume, v + (critical ? b.labCriticalBonus : 0f));
        }

        public static float LabBpm(AudioBalance b, int degradation, bool critical) =>
            b.labBaseBpm + b.labBpmPerDegradation * degradation + (critical ? b.labCriticalBpmBonus : 0f);
    }
}
