using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>
    /// Sound transmission between two points (S-11, D-new): each wall in between lets through <c>wallTransmission</c>
    /// of the sound (−70% → 0.3), each closed or sealed door <c>doorTransmission</c> (−50% → 0.5); they stack. Used for
    /// what Lukas hears and for the noise radius enemies perceive.
    /// </summary>
    public static class OcclusionModel
    {
        public static float Factor(int walls, int closedDoors, float wallTransmission, float doorTransmission)
            => Mathf.Pow(Mathf.Clamp01(wallTransmission), Mathf.Max(0, walls)) * Mathf.Pow(Mathf.Clamp01(doorTransmission), Mathf.Max(0, closedDoors));

        /// <summary>Hits closer than this along the ray belong to the same wall (two rooms' shells back to back).</summary>
        public const float SameWallGap = 0.8f;
    }

    /// <summary>Something that blocks sound: a kit wall (always) or a door (when not open).</summary>
    public interface ISoundOccluder
    {
        bool Occludes { get; }
        bool IsDoor { get; }
    }
}
