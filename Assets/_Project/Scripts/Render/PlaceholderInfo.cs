using UnityEngine;

namespace Heartbeat.Render
{
    /// <summary>
    /// Marks a PH_ prefab as a stand-in for a Blender model (Desarrollo/PLACEHOLDERS.md).
    /// Swap the children of "Mesh" for the imported model; keep this root, its pivot, colliders and scripts.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlaceholderInfo : MonoBehaviour
    {
        [Tooltip("Model that will replace this placeholder.")]
        public string intendedModel;
        [Tooltip("Final triangle budget.")]
        public int triangleBudget;
        public string owner = "Bryan Iván Navarrete Canché";
        [TextArea] public string notes;
    }
}
