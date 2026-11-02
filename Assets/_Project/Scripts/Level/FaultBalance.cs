using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>Machine faults (M-17, D-09). Initial values, subject to balance.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Faults", fileName = "FaultBalance")]
    public sealed class FaultBalance : ScriptableObject
    {
        [Tooltip("Uses before The Mother patches a fault (GDD: 1-2).")] public int usesBeforePatch = 2;
        [Tooltip("Door hack length (normally 4-5) when the door backdoor is exploited.")] public int doorLength = 2;
        [Tooltip("Fraction of a main sequence (7-8) left when the mixer bypass is exploited.")] [Range(0.1f, 1f)] public float mainFraction = 0.5f;
    }
}
