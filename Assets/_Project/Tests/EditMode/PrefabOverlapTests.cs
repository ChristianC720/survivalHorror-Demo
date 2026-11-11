using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Tests
{
    /// <summary>
    /// Two renderers with the same bounds in one prefab z-fight into black triangles. It happened when a floor's prefab inherited
    /// another floor's model. Also checks that no floor shows another floor's model.
    /// </summary>
    public class PrefabOverlapTests
    {
        private static IEnumerable<string> Prefabs(string folder) => AssetDatabase.FindAssets("t:Prefab", new[] { folder }).Select(AssetDatabase.GUIDToAssetPath);

        private static IEnumerable<string> LevelPrefabs() => Prefabs("Assets/_Project/Prefabs/Kit").Concat(Prefabs("Assets/_Project/Prefabs/Level")).Concat(Prefabs("Assets/_Project/Prefabs/Rooms")).Concat(Prefabs("Assets/_Project/Prefabs/Props"));

        [Test]
        public void NoPrefabHasTwoRenderersWithTheSameBounds()
        {
            var bad = new List<string>();
            foreach (var path in LevelPrefabs())
            {
                var root = PrefabUtility.LoadPrefabContents(path);
                var seen = new Dictionary<(Vector3Int, Vector3Int), string>();
                foreach (var r in root.GetComponentsInChildren<MeshRenderer>(false))
                {
                    if (!r.enabled || !r.TryGetComponent<MeshFilter>(out var f) || f.sharedMesh == null) continue;
                    var b = r.bounds;
                    var key = (Vector3Int.RoundToInt(b.center * 200f), Vector3Int.RoundToInt(b.size * 200f));
                    if (seen.TryGetValue(key, out var other)) bad.Add($"{path}: {r.name} and {other}");
                    else seen[key] = r.name;
                }
                PrefabUtility.UnloadPrefabContents(root);
            }
            Assert.IsEmpty(bad, string.Join("\n", bad));
        }

        /// <summary>URP clears _EMISSION when it validates a material whose GI flags are not emissive: seams and screens went dark.</summary>
        [Test]
        public void MaterialsWithAnEmissionMapStayEmissive()
        {
            var bad = new List<string>();
            foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Project/Models" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var m = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (!m.HasProperty("_EmissionMap") || m.GetTexture("_EmissionMap") == null) continue;
                if ((m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) == 0) bad.Add(path);
            }
            Assert.IsEmpty(bad, string.Join("\n", bad));
        }

        [Test]
        public void NoFloorShowsAnotherFloorsModel()
        {
            var bad = new List<string>();
            foreach (var f in new[] { "F1", "F2", "F3" })
                foreach (var folder in new[] { "Kit", "Rooms", "Props" })
                {
                    string dir = $"Assets/_Project/Prefabs/{folder}/{f}";
                    if (!AssetDatabase.IsValidFolder(dir)) continue;
                    // The Mother's room is on F3 but deliberately keeps the clinic's capsules and consoles.
                    foreach (var path in Prefabs(dir).Where(p => !p.EndsWith("ROOM_Mother.prefab")))
                    {
                        var root = PrefabUtility.LoadPrefabContents(path);
                        foreach (var m in root.GetComponentsInChildren<MeshFilter>(false))
                        {
                            if (m.sharedMesh == null) continue;
                            string n = m.sharedMesh.name;
                            if (new[] { "F1", "F2", "F3" }.Any(o => o != f && n.Contains("_" + o))) bad.Add($"{path}: {m.name} uses {n}");
                        }
                        PrefabUtility.UnloadPrefabContents(root);
                    }
                }
            Assert.IsEmpty(bad, string.Join("\n", bad));
        }
    }
}
