using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>The hand-built kit pieces of one floor theme (KIT_SPEC §2). Used by <see cref="RoomShell"/>.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Kit Set", fileName = "KitSet_")]
    public sealed class KitSet : ScriptableObject
    {
        public GameObject floor2x2;
        public GameObject ceiling2x2;
        public GameObject wall2m;
        public GameObject wall2mHigh;
        public GameObject doorway2m;
        public GameObject wallWindow2m;
        public GameObject door;
    }
}
