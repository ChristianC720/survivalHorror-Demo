using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// The repetitive shell of a room (floor and ceiling tiles, perimeter walls, socket doorways/walls), assembled from
    /// the hand-built kit pieces at runtime (KIT_SPEC §3). Everything else in the room — props, cover, interior walls,
    /// ramps, lights, spawns, sockets, camera mount — is placed by hand with Aura in the prefab.
    /// </summary>
    public sealed class RoomShell : MonoBehaviour
    {
        [Tooltip("Size in 2 m cells along X.")] public int cellsX = 4;
        [Tooltip("Size in 2 m cells along Z.")] public int cellsZ = 4;

        /// <summary>Size in cells (two ints so Aura's set_property can edit it).</summary>
        public Vector2Int size => new Vector2Int(cellsX, cellsZ);
        public KitSet kit;
        [Tooltip("6 m walls (vertical rooms).")] public bool tall;
        public bool ceiling = true;
        [Tooltip("Perimeter edges that get a window wall instead of a plain wall (surveillance rooms).")]
        public List<SocketData> windows = new List<SocketData>();

        public const string ShellName = "Shell";

        public Transform Built => transform.Find(ShellName);

        /// <summary>
        /// Builds the shell. <paramref name="connected"/> holds the sockets that lead to another room (doorway);
        /// all other sockets become walls. Rebuilding replaces the previous shell.
        /// </summary>
        public Transform Build(ICollection<SocketData> connected, bool editorPreview = false)
        {
            var old = Built;
            if (old != null) { if (Application.isPlaying) Destroy(old.gameObject); else DestroyImmediate(old.gameObject); }

            var shell = new GameObject(ShellName).transform;
            shell.SetParent(transform, false);
            if (editorPreview) shell.gameObject.hideFlags = HideFlags.DontSave;

            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.y; z++)
                {
                    Place(kit.floor2x2, shell, new Vector3(x * 2, 0, z * 2), 0f);
                    if (ceiling) Place(kit.ceiling2x2, shell, new Vector3(x * 2, tall ? 3 : 0, z * 2), 0f);
                }

            var sockets = new HashSet<(Vector2Int, Facing)>();
            foreach (var s in GetComponentsInChildren<DoorSocket>(true))
                sockets.Add((s.LocalCell(transform, size), s.LocalFacing(transform)));
            var open = new HashSet<(Vector2Int, Facing)>();
            if (connected != null) foreach (var c in connected) open.Add((c.cell, c.facing));
            var win = new HashSet<(Vector2Int, Facing)>();
            foreach (var w in windows) win.Add((w.cell, w.facing));

            foreach (var (cell, facing) in Edges())
            {
                var key = (cell, facing);
                GameObject piece;
                if (sockets.Contains(key)) piece = open.Contains(key) ? kit.doorway2m : (tall ? kit.wall2mHigh : kit.wall2m);
                else if (win.Contains(key)) piece = kit.wallWindow2m;
                else piece = tall ? kit.wall2mHigh : kit.wall2m;
                Place(piece, shell, EdgePoint(cell, facing), EdgeYaw(facing));
                if (piece == kit.doorway2m && tall) Place(kit.wall2m, shell, EdgePoint(cell, facing) + Vector3.up * 3f, EdgeYaw(facing));
            }
            return shell;
        }

        private IEnumerable<(Vector2Int, Facing)> Edges()
        {
            for (int x = 0; x < size.x; x++)
            {
                yield return (new Vector2Int(x, 0), Facing.South);
                yield return (new Vector2Int(x, size.y - 1), Facing.North);
            }
            for (int z = 0; z < size.y; z++)
            {
                yield return (new Vector2Int(0, z), Facing.West);
                yield return (new Vector2Int(size.x - 1, z), Facing.East);
            }
        }

        /// <summary>KIT_SPEC wall placement table (local room space).</summary>
        public Vector3 EdgePoint(Vector2Int cell, Facing f)
        {
            switch (f)
            {
                case Facing.South: return new Vector3(cell.x * 2 + 1, 0, 0);
                case Facing.North: return new Vector3(cell.x * 2 + 1, 0, size.y * 2);
                case Facing.West: return new Vector3(0, 0, cell.y * 2 + 1);
                default: return new Vector3(size.x * 2, 0, cell.y * 2 + 1);
            }
        }

        /// <summary>Wall yaw so its local +Z points into the room.</summary>
        public static float EdgeYaw(Facing f) => f == Facing.South ? 0f : f == Facing.North ? 180f : f == Facing.West ? 90f : 270f;

        private static void Place(GameObject prefab, Transform parent, Vector3 localPos, float yaw)
        {
            if (prefab == null) return;
            var go = Instantiate(prefab, parent);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        }

        [ContextMenu("Preview Shell (not saved)")]
        private void PreviewShell() => Build(null, editorPreview: true);

        [ContextMenu("Clear Shell")]
        private void ClearShell() { var b = Built; if (b != null) DestroyImmediate(b.gameObject); }
    }
}
