using System.Collections.Generic;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>All room definitions available to the generator. Refreshed by the editor menu.</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Room Library", fileName = "RoomLibrary")]
    public sealed class RoomLibrary : ScriptableObject
    {
        public List<RoomDefinition> rooms = new List<RoomDefinition>();

        public IEnumerable<RoomDefinition> For(int floor, RoomRole role)
        {
            foreach (var r in rooms)
                if (r != null && r.floor == floor && r.role == role) yield return r;
        }
    }
}
