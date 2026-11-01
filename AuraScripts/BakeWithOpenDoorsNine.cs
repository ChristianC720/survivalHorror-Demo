using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class BakeWithOpenDoorsNine
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            var config = UnityEditor.AssetDatabase.LoadAssetAtPath<Heartbeat.Config.GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
            Transform doors = GameObject.Find("VS_Piso1").transform.Find("Doors");

            // Las puertas que nacen abiertas deben estar abiertas al hornear; si no, el vano queda
            // tallado como muro y el NavMesh parte el nivel en fragmentos.
            var abiertas = new[] { "Puerta_SalaClaveSecundaria_Oeste", "Puerta_SalaVigilancia" };
            var originales = new System.Collections.Generic.Dictionary<Transform, Quaternion>();

            foreach (string name in abiertas)
            {
                Transform leaf = doors.Find(name)?.Find("Hoja");
                if (leaf == null) { sb.AppendLine($"  falta hoja de {name}"); continue; }
                originales[leaf] = leaf.localRotation;
                leaf.localRotation = Quaternion.Euler(0f, config.DoorOpenAngle, 0f);
                sb.AppendLine($"  {name}: hoja girada a {config.DoorOpenAngle}° para hornear");
            }

            foreach (string name in new[] { "Puerta_Almacen", "Puerta_SalaClaveFinal" })
            {
                Transform leaf = doors.Find(name)?.Find("Hoja");
                sb.AppendLine($"  {name}: sigue cerrada al hornear (cerrada={leaf != null && leaf.localRotation == Quaternion.identity})");
            }

            Physics.SyncTransforms();

            Type tool = Type.GetType("Heartbeat.EditorTools.NavMeshBakeTool, Assembly-CSharp-Editor");
            MethodInfo bake = tool?.GetMethod("BakeActiveScene", BindingFlags.Public | BindingFlags.Static);
            if (bake == null) { sb.AppendLine("NO se encontro BakeActiveScene"); return sb.ToString(); }
            bake.Invoke(null, null);

            foreach (var pair in originales)
            {
                pair.Key.localRotation = pair.Value;
            }
            Physics.SyncTransforms();

            var surface = GameObject.Find("VS_Piso1").GetComponent<Unity.AI.Navigation.NavMeshSurface>();
            sb.AppendLine($"surface center={surface.center} size={surface.size} data={(surface.navMeshData != null ? surface.navMeshData.name : "null")}");

            NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
            sb.AppendLine($"navmesh vertices={(tri.vertices != null ? tri.vertices.Length : 0)} triangulos={(tri.indices != null ? tri.indices.Length / 3 : 0)}");

            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            sb.AppendLine($"escena guardada={UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene)}");
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }
}
