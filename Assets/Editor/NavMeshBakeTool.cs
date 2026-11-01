using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Hornea el NavMesh del graybox con un NavMeshSurface sobre la raíz del nivel.
    /// Los enemigos usan NavMeshAgent, así que sin este paso no se mueven.
    /// </summary>
    public static class NavMeshBakeTool
    {
        private const string RootName = "VS_Piso1";
        private const string DataFolder = "Assets/Scenes/NavMesh";
        private const string DataPath = "Assets/Scenes/NavMesh/VS_Piso1_NavMesh.asset";

        [MenuItem("Heartbeat/AI/Hornear NavMesh (escena activa)")]
        public static void BakeActiveScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError("[NavMesh] No hay una escena cargada.");
                return;
            }

            if (scene.isDirty) EditorSceneManager.SaveScene(scene);

            GameObject root = GameObject.Find(RootName);
            if (root == null)
            {
                Debug.LogError($"[NavMesh] No se encontró '{RootName}' en la escena activa.");
                return;
            }

            NavMeshSurface surface = root.GetComponent<NavMeshSurface>();
            if (surface == null)
            {
                surface = root.AddComponent<NavMeshSurface>();
                Debug.Log($"[NavMesh] NavMeshSurface añadido a {RootName}.");
            }

            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.layerMask = ~0;
            surface.agentTypeID = 0;
            surface.defaultArea = 0;
            surface.ignoreNavMeshAgent = true;
            surface.ignoreNavMeshObstacle = true;

            if (TryComputeBounds(root, out Bounds bounds))
            {
                surface.center = root.transform.InverseTransformPoint(bounds.center);
                surface.size = bounds.size + Vector3.one * 2f;
            }

            surface.BuildNavMesh();

            if (surface.navMeshData == null)
            {
                Debug.LogError("[NavMesh] El horneado no generó datos.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(DataFolder))
            {
                AssetDatabase.CreateFolder("Assets/Scenes", "NavMesh");
            }

            if (AssetDatabase.LoadAssetAtPath<NavMeshData>(DataPath) != null)
            {
                AssetDatabase.DeleteAsset(DataPath);
            }

            surface.navMeshData.name = "VS_Piso1_NavMesh";
            AssetDatabase.CreateAsset(surface.navMeshData, DataPath);
            AssetDatabase.SaveAssets();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            int triangles = triangulation.indices != null ? triangulation.indices.Length / 3 : 0;

            Debug.Log($"[NavMesh] Horneado en {DataPath}: {triangles} triángulos, " +
                      $"{triangulation.vertices.Length} vértices, área {bounds.size}.");
        }

        private static bool TryComputeBounds(GameObject root, out Bounds bounds)
        {
            bounds = new Bounds(root.transform.position, Vector3.zero);
            bool has = false;

            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider == null || collider.isTrigger) continue;

                if (!has)
                {
                    bounds = collider.bounds;
                    has = true;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }

            return has;
        }
    }
}
