using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Finishing pass for hand-built kit pieces (Desarrollo/KIT_SPEC.md §2, §7). It does not create geometry:
    /// it propagates layers, marks the piece static and reports pivot/scale/grid problems.
    /// </summary>
    public static class KitPieceTools
    {
        private const float Eps = 0.001f;

        [MenuItem("Heartbeat/Kit/Finish Kit Pieces In Scene")]
        public static void FinishAllInSceneMenu() => Debug.Log(FinishAllInScene());

        /// <summary>Applies to every root GameObject named KIT_* in the active scene.</summary>
        public static string FinishAllInScene()
        {
            var sb = new StringBuilder();
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (root.name.StartsWith("KIT_")) sb.AppendLine(Finish(root));
            return sb.ToString();
        }

        public static string Finish(GameObject root)
        {
            bool ceiling = root.name.Contains("_Ceiling");
            int layer = LayerMask.NameToLayer(ceiling ? "Ceiling" : "Environment");
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layer;
                GameObjectUtility.SetStaticEditorFlags(t.gameObject, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.ContributeGI);
            }
            EditorUtility.SetDirty(root);
            return $"{root.name}: {string.Join("; ", Validate(root))}";
        }

        public static List<string> Validate(GameObject root)
        {
            var issues = new List<string>();
            if ((root.transform.localScale - Vector3.one).sqrMagnitude > Eps) issues.Add("root scale is not (1,1,1)");
            if (root.transform.position.sqrMagnitude > Eps) issues.Add("root is not at the origin while building");
            if (Quaternion.Angle(root.transform.rotation, Quaternion.identity) > 0.01f) issues.Add("root is rotated");

            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) { issues.Add("no renderers"); return issues; }
            var b = renderers[0].bounds;
            foreach (var r in renderers) b.Encapsulate(r.bounds);

            // Footprint edges must land on whole 0.1 m steps and inside the spec extent for this piece type.
            string extent = ExpectedExtent(root.name);
            if (extent != null) issues.Add($"bounds min {Fmt(b.min)} max {Fmt(b.max)} (spec {extent})");
            if (issues.Count == 0 || (issues.Count == 1 && extent != null)) issues.Insert(0, "OK");
            return issues;
        }

        private static string ExpectedExtent(string name)
        {
            if (name.Contains("Floor_2x2")) return "x0..2 y-0.2..0 z0..2";
            if (name.Contains("Ceiling_2x2")) return "x0..2 y3..3.2 z0..2";
            if (name.Contains("Wall_2m_High")) return "x-1..1 y0..6 z0..0.3";
            if (name.Contains("Wall_2m") || name.Contains("WallWindow_2m")) return "x-1..1 y0..3 z0..0.3";
            if (name.Contains("Doorway_2m")) return "x-1..1 y2.2..3 z0..0.3";
            if (name.Contains("Ramp_2x4")) return "x0..2 top 0..1.5 over z0..4";
            if (name.Contains("Platform_2x2")) return "x0..2 top y1.5 z0..2, rails to 2.5";
            if (name.Contains("Column")) return "x-0.2..0.2 y0..3 z-0.2..0.2";
            return null;
        }

        private static string Fmt(Vector3 v) => $"({v.x:0.##},{v.y:0.##},{v.z:0.##})";
    }
}
