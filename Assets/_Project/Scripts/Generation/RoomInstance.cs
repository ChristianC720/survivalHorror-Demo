using System.Collections.Generic;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Root component of a room prefab. At runtime it knows its definition, its placement on the floor grid and its
    /// neighbours; its trigger (RoomVolume, layer RoomVolume) reports when Lukas enters (S-06 activation, S-01 cameras).
    /// </summary>
    public sealed class RoomInstance : MonoBehaviour
    {
        [SerializeField] private RoomDefinition definition;
        [Tooltip("Optional surveillance camera pose (KIT_SPEC §3).")]
        [SerializeField] private Transform cameraMount;

        private readonly List<RoomInstance> neighbours = new List<RoomInstance>();

        public RoomDefinition Definition { get => definition; set => definition = value; }
        public Transform CameraMount => cameraMount;
        public int NodeIndex { get; set; } = -1;
        public bool IsEmpty { get; set; }
        public bool IsPowered { get; set; } = true;
        /// <summary>Capsule row and the fixed tutorial chain of the first attempt (D-70): damage there never kills (D-160).</summary>
        public bool IsTutorial { get; set; }
        /// <summary>Set when the floor is torn down (rebuild, death): its trigger can still fire before Destroy runs.</summary>
        public bool Retired { get; set; }
        public IReadOnlyList<RoomInstance> Neighbours => neighbours;

        public static RoomInstance Current { get; private set; }
        /// <summary>Each floor build is a new generation; rooms of an older one (a Floor scene still unloading after a
        /// death) never become Current — their trigger fired after the new floor's and left Current on a dying room.</summary>
        public static int LiveGeneration { get; private set; }
        public int Generation { get; set; }
        public static int NextGeneration() => ++LiveGeneration;

        public void AddNeighbour(RoomInstance r) { if (r != null && !neighbours.Contains(r)) neighbours.Add(r); }

        public IEnumerable<DoorSocket> Sockets() => GetComponentsInChildren<DoorSocket>(true);
        public IEnumerable<SpawnPoint> Spawns() => GetComponentsInChildren<SpawnPoint>(true);

        private void OnTriggerEnter(Collider other)
        {
            if (Retired || Generation != LiveGeneration || !other.CompareTag("Player")) return;
            Current = this;
            HbLog.Info("Room", $"Entered {name}");
            GameEvents.RaiseRoomEntered(this);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { Current = null; LiveGeneration = 0; }
    }
}
