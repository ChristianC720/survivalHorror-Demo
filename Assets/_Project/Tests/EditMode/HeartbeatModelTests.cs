using Heartbeat.Audio;
using NUnit.Framework;
using UnityEngine;

namespace Heartbeat.Tests
{
    public class HeartbeatModelTests
    {
        private static readonly float[] Bands = { 15f, 8f, 4f };

        [TestCase(30f, -1)]
        [TestCase(15f, 0)]
        [TestCase(10f, 0)]
        [TestCase(6f, 1)]
        [TestCase(2f, 2)]
        public void ThreatBand_IsQuantized(float d, int expected)
        {
            Assert.AreEqual(expected, HeartbeatModel.ThreatBand(d, Bands));
        }

        [Test]
        public void LabHeartbeat_GrowsWithDegradation_Capped_CriticalOnTop() // D-95, D-29
        {
            var b = ScriptableObject.CreateInstance<AudioBalance>();
            float v0 = HeartbeatModel.LabVolume(b, 0, false);
            float v3 = HeartbeatModel.LabVolume(b, 3, false);
            Assert.Greater(v3, v0);
            Assert.LessOrEqual(HeartbeatModel.LabVolume(b, 50, true), b.labMaxVolume);
            Assert.Greater(HeartbeatModel.LabBpm(b, 1, true), HeartbeatModel.LabBpm(b, 1, false));
            Object.DestroyImmediate(b);
        }
    }
}
