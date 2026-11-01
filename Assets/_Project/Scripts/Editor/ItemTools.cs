using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Finishing pass for hand-built ITEM_ / INT_ prefabs: visual primitives lose their colliders, the root gets one
    /// trigger sphere for the Interactor, and everything goes to the Interactable layer. Creates no geometry.
    /// </summary>
    public static class ItemTools
    {
        [MenuItem("Heartbeat/Items/Finish Items In Scene")]
        public static void FinishMenu() => Debug.Log(FinishAllInScene());

        public static string FinishAllInScene(float radius = 0.4f)
        {
            var sb = new StringBuilder();
            int layer = LayerMask.NameToLayer("Interactable");
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (!root.name.StartsWith("ITEM_") && !root.name.StartsWith("INT_")) continue;
                int removed = 0;
                foreach (var c in root.GetComponentsInChildren<Collider>(true))
                {
                    if (c.gameObject == root) continue;
                    Object.DestroyImmediate(c);
                    removed++;
                }
                var trigger = root.GetComponent<SphereCollider>();   // no '??': Unity's fake null breaks it
                if (trigger == null) trigger = root.AddComponent<SphereCollider>();
                trigger.isTrigger = true;
                trigger.radius = radius;
                trigger.center = new Vector3(0f, 0.1f, 0f);
                foreach (var t in root.GetComponentsInChildren<Transform>(true)) t.gameObject.layer = layer;
                sb.AppendLine($"{root.name}: removed {removed} colliders, trigger r={radius}");
            }
            return sb.ToString();
        }
    }
}
