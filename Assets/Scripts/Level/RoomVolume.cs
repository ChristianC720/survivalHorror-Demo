using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>
    /// Volumen lógico de una sala: nombre, zona segura y salas adyacentes que usa la IA.
    /// El trigger ocupa el interior de la sala; no bloquea el paso.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class RoomVolume : MonoBehaviour
    {
        [Header("Identidad")]
        [SerializeField] private string roomId = "Sala";

        [Tooltip("Los enemigos nunca entran en una zona segura (FilaCapsulas).")]
        [SerializeField] private bool isSafeZone;

        [Header("Adyacencias (para la IA)")]
        [SerializeField] private string[] adjacentRoomIds = new string[0];
        [SerializeField] private RoomVolume[] adjacentRooms = new RoomVolume[0];

        [Header("Vigilancia")]
        [Tooltip("La sala tiene cámara de seguridad: permite la vista de vigilancia con Q.")]
        [SerializeField] private bool hasSecurityCamera;

        [Tooltip("Cámara física de la sala; su giro orienta la vista isométrica.")]
        [SerializeField] private Transform securityCameraProp;

        [Header("Depuración")]
        [SerializeField] private bool logOccupancy = true;

        private static readonly List<RoomVolume> Registry = new List<RoomVolume>();

        public static IReadOnlyList<RoomVolume> All => Registry;

        public string RoomId => roomId;
        public bool IsSafeZone => isSafeZone;
        public bool HasSecurityCamera => hasSecurityCamera;
        public Transform SecurityCameraProp => securityCameraProp;
        public IReadOnlyList<RoomVolume> AdjacentRooms => adjacentRooms;
        public IReadOnlyList<string> AdjacentRoomIds => adjacentRoomIds;

        private void OnEnable()
        {
            if (!Registry.Contains(this)) Registry.Add(this);
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        /// <summary>Rellena la sala desde el constructor de graybox o desde el Inspector.</summary>
        public void Configure(string id, bool safeZone, string[] adjacentIds, RoomVolume[] rooms)
        {
            roomId = id;
            isSafeZone = safeZone;
            adjacentRoomIds = adjacentIds ?? new string[0];
            adjacentRooms = rooms ?? new RoomVolume[0];
        }

        public bool Contains(Vector3 worldPosition)
        {
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null) return false;

            Vector3 local = transform.InverseTransformPoint(worldPosition) - box.center;
            Vector3 half = box.size * 0.5f;
            return Mathf.Abs(local.x) <= half.x
                && Mathf.Abs(local.y) <= half.y
                && Mathf.Abs(local.z) <= half.z;
        }

        public bool IsAdjacentTo(RoomVolume other)
        {
            return other != null && System.Array.IndexOf(adjacentRooms, other) >= 0;
        }

        public static RoomVolume FindById(string id)
        {
            for (int i = 0; i < Registry.Count; i++)
            {
                if (Registry[i].roomId == id) return Registry[i];
            }
            return null;
        }

        public static RoomVolume FindContaining(Vector3 worldPosition)
        {
            for (int i = 0; i < Registry.Count; i++)
            {
                if (Registry[i].Contains(worldPosition)) return Registry[i];
            }
            return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (logOccupancy) Debug.Log($"[Room] {roomId} <- {other.name} entró");
        }

        private void OnTriggerExit(Collider other)
        {
            if (logOccupancy) Debug.Log($"[Room] {roomId} -> {other.name} salió");
        }

        private void OnDrawGizmos()
        {
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null) return;

            Gizmos.color = isSafeZone ? new Color(0.2f, 1f, 0.4f, 0.6f) : new Color(0.3f, 0.7f, 1f, 0.5f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}
