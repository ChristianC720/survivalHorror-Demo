using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Finishing pass for hand-built PH_ placeholders (Desarrollo/PLACEHOLDERS.md): the primitives under "Mesh" are
    /// visual only, so their auto-added colliders are removed (gameplay colliders live on the parent prefab).
    /// </summary>
    public static class PlaceholderTools
    {
        [MenuItem("Heartbeat/Placeholders/Strip Mesh Colliders In Scene")]
        public static void StripMenu() => Debug.Log(StripAllInScene());

        public static string StripAllInScene()
        {
            var sb = new StringBuilder();
            foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!t.name.StartsWith("PH_")) continue;
                var mesh = t.Find("Mesh");
                if (mesh == null) { sb.AppendLine($"{t.name}: no Mesh child"); continue; }
                int n = 0;
                foreach (var c in mesh.GetComponentsInChildren<Collider>(true)) { Object.DestroyImmediate(c); n++; }
                sb.AppendLine($"{t.name}: removed {n} colliders");
            }
            return sb.ToString();
        }
    }
}
