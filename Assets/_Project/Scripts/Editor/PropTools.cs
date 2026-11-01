using System.Text;
using Heartbeat.Render;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Finishing pass for hand-built PROP_ / PH_ props (Desarrollo/OBJECT_CATALOG.md): Environment layer, static flags,
    /// one root BoxCollider fitted to the visual bounds (so swapping the mesh never breaks collision or cover),
    /// PlaceholderInfo on PH_. Creates no geometry.
    /// </summary>
    public static class PropTools
    {
        [MenuItem("Heartbeat/Props/Finish Props In Scene")]
        public static void FinishMenu() => Debug.Log(FinishAllInScene());

        public static string FinishAllInScene()
        {
            var sb = new StringBuilder();
            int env = LayerMask.NameToLayer("Environment");
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (!root.name.StartsWith("PROP_") && !(root.name.StartsWith("PH_F") || root.name.StartsWith("PH_Shared"))) continue;
                foreach (var c in root.GetComponentsInChildren<Collider>(true))
                    if (c.gameObject != root) Object.DestroyImmediate(c);

                var renderers = root.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) { sb.AppendLine($"{root.name}: no renderers"); continue; }
                var b = renderers[0].bounds;
                foreach (var r in renderers) b.Encapsulate(r.bounds);

                var box = root.GetComponent<BoxCollider>();
                if (box == null) box = root.AddComponent<BoxCollider>();
                box.center = root.transform.InverseTransformPoint(b.center);
                box.size = b.size;

                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = env;
                    GameObjectUtility.SetStaticEditorFlags(t.gameObject, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
                }
                if (root.name.StartsWith("PH_") && root.GetComponent<PlaceholderInfo>() == null)
                {
                    var info = root.AddComponent<PlaceholderInfo>();
                    info.intendedModel = root.name.Replace("PH_", "") + " (see Desarrollo/OBJECT_CATALOG.md)";
                    info.triangleBudget = 2000;
                }
                bool cover = b.size.y >= 1f;
                sb.AppendLine($"{root.name}: collider {b.size.x:0.##}x{b.size.y:0.##}x{b.size.z:0.##}{(cover ? " (cover)" : "")}");
            }
            return sb.ToString();
        }
    }
}
