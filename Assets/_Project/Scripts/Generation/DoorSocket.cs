using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Door socket on a room boundary (KIT_SPEC §4): at the edge midpoint, y = 0, local +Z pointing OUT of the room.
    /// The FloorBuilder fills it with a doorway + door (connected) or a wall (unused).
    /// </summary>
    public sealed class DoorSocket : MonoBehaviour
    {
        [Tooltip("This socket may receive a key door, a power-locked door or a hack panel.")]
        public bool allowLocked = true;

        /// <summary>Facing in the room's local axes, from this transform's forward.</summary>
        public Facing LocalFacing(Transform roomRoot)
        {
            Vector3 f = roomRoot.InverseTransformDirection(transform.forward);
            if (Mathf.Abs(f.x) > Mathf.Abs(f.z)) return f.x > 0 ? Facing.East : Facing.West;
            return f.z > 0 ? Facing.North : Facing.South;
        }

        /// <summary>Cell inside the room that this socket opens from, for a room of <paramref name="size"/> cells.</summary>
        public Vector2Int LocalCell(Transform roomRoot, Vector2Int size)
        {
            Vector3 p = roomRoot.InverseTransformPoint(transform.position);
            var facing = LocalFacing(roomRoot);
            int x = facing == Facing.East ? size.x - 1 : facing == Facing.West ? 0 : Mathf.FloorToInt(p.x / 2f);
            int y = facing == Facing.North ? size.y - 1 : facing == Facing.South ? 0 : Mathf.FloorToInt(p.z / 2f);
            return new Vector2Int(x, y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = allowLocked ? new Color(1f, 0.6f, 0.1f) : Color.cyan;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 1.1f, new Vector3(2f, 2.2f, 0.1f));
            Gizmos.DrawLine(transform.position + Vector3.up * 1.1f, transform.position + Vector3.up * 1.1f + transform.forward);
        }
    }
}
