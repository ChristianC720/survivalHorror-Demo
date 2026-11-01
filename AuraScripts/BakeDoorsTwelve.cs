using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class BakeDoorsTwelve
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            var config = UnityEditor.AssetDatabase.LoadAssetAtPath<Heartbeat.Config.GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
            Transform doors = GameObject.Find("VS_Piso1").transform.Find("Doors");

            // Gira la BISAGRA (la raíz de la puerta), que es lo que anima SealableDoor en runtime.
            var abiertas = new[] { "Puerta_SalaClaveSecundaria_Oeste", "Puerta_SalaVigilancia" };
            var originales = new Dictionary<Transform, Quaternion>();

            foreach (string name in abiertas)
            {
                Transform hinge = doors.Find(name);
                if (hinge == null) { sb.AppendLine($"  falta {name}"); continue; }
                originales[hinge] = hinge.localRotation;
                hinge.localRotation = Quaternion.Euler(0f, config.DoorOpenAngle, 0f);
                sb.AppendLine($"  {name}: bisagra a {config.DoorOpenAngle}° (hoja en {hinge.Find("Hoja").position})");
            }

            Physics.SyncTransforms();

            Type tool = Type.GetType("Heartbeat.EditorTools.NavMeshBakeTool, Assembly-CSharp-Editor");
            MethodInfo bake = tool?.GetMethod("BakeActiveScene", BindingFlags.Public | BindingFlags.Static);
            if (bake == null) { sb.AppendLine("NO BakeActiveScene"); return sb.ToString(); }
            bake.Invoke(null, null);

            foreach (var pair in originales) pair.Key.localRotation = pair.Value;
            Physics.SyncTransforms();

            NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
            sb.AppendLine($"navmesh vertices={(tri.vertices != null ? tri.vertices.Length : 0)} triangulos={(tri.indices != null ? tri.indices.Length / 3 : 0)}");

            var surface = GameObject.Find("VS_Piso1").GetComponent<Unity.AI.Navigation.NavMeshSurface>();
            sb.AppendLine($"surface size={surface.size}");

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
