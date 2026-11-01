using UnityEngine;

namespace Heartbeat.Generation
{
    public enum SpawnKind { Enemy, Item, Player }

    /// <summary>What an item spawn may hold (S-06 budget, D-73, D-79).</summary>
    public enum ItemSpawnTag { Budget, Document, Emilia, Confiscated, Key, Switch }

    /// <summary>Marker placed by hand in a room prefab; the FloorBuilder fills it from the floor budget.</summary>
    public sealed class SpawnPoint : MonoBehaviour
    {
        public SpawnKind kind = SpawnKind.Item;
        public ItemSpawnTag itemTag = ItemSpawnTag.Budget;

        private void OnDrawGizmos()
        {
            Gizmos.color = kind == SpawnKind.Enemy ? Color.red : kind == SpawnKind.Player ? Color.green : Color.yellow;
            if (kind == SpawnKind.Enemy) Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(0.6f, 2f, 0.6f));
            else Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, 0.25f);
        }
    }
}
