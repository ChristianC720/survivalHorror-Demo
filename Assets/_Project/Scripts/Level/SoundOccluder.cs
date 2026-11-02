using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>Marks a kit wall piece as a sound occluder (−70% per wall, S-11). Lives on the F1 kit wall prefabs; F2/F3 variants inherit it.</summary>
    public sealed class SoundOccluder : MonoBehaviour, ISoundOccluder
    {
        public bool Occludes => true;
        public bool IsDoor => false;
    }
}
