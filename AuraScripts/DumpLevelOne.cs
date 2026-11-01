using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class DumpLevelOne
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            var config = UnityEditor.AssetDatabase.LoadAssetAtPath<Heartbeat.Config.GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
            sb.AppendLine($"CONFIG wallHeight={config.WallHeight} wallThickness={config.WallThickness} doorWidth={config.DoorWidth} doorHeight={config.DoorHeight} capsuleSpacing={config.CapsuleSpacing}");

            GameObject root = GameObject.Find("VS_Piso1");
            foreach (string group in new[] { "Geometry", "Props", "Lights", "Doors", "Panels", "Triggers" })
            {
                Transform g = root.transform.Find(group);
                if (g == null) { sb.AppendLine($"{group}: (no existe)"); continue; }
                sb.AppendLine($"--- {group} ({g.childCount}) ---");
                for (int i = 0; i < g.childCount; i++)
                {
                    Transform t = g.GetChild(i);
                    var col = t.GetComponent<Collider>();
                    string colInfo = col != null ? $" col.size={col.bounds.size}" : "";
                    var light = t.GetComponent<Light>();
                    string lightInfo = light != null ? $" light={light.type}/{light.range}" : "";
                    sb.AppendLine($"  {t.name} pos={t.position} scale={t.localScale} rot={t.localEulerAngles}{colInfo}{lightInfo}");
                }
            }

            Transform doors = root.transform.Find("Doors");
            for (int i = 0; i < doors.childCount; i++)
            {
                Transform hinge = doors.GetChild(i);
                Transform leaf = hinge.Find("Hoja");
                sb.AppendLine($"  HOJA {hinge.name}: hinge={hinge.position} rot={hinge.localEulerAngles} leafLocal={leaf.localPosition} leafScale={leaf.localScale}");
            }

            sb.AppendLine($"--- Raíces de la escena ---");
            foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                sb.AppendLine($"  {go.name} pos={go.transform.position} activo={go.activeSelf} hijos={go.transform.childCount}");
            }

            foreach (string path in new[] { "Enemies", "Player", "HUD" })
            {
                GameObject g = GameObject.Find(path);
                if (g == null) { sb.AppendLine($"{path}: (no existe)"); continue; }
                sb.AppendLine($"--- {path} ---");
                for (int i = 0; i < g.transform.childCount; i++)
                {
                    Transform t = g.transform.GetChild(i);
                    sb.AppendLine($"  {t.name} pos={t.position} activo={t.gameObject.activeSelf}");
                }
            }

            GameObject enemies = GameObject.Find("Enemies");
            for (int i = 0; i < enemies.transform.childCount; i++)
            {
                Transform t = enemies.transform.GetChild(i);
                if (t.childCount == 0) continue;
                for (int j = 0; j < t.childCount; j++)
                {
                    Transform c = t.GetChild(j);
                    sb.AppendLine($"  enemyHijo {t.name}/{c.name} pos={c.position}");
                }
            }

            sb.AppendLine($"NavMeshSurface en raiz: {root.GetComponent<Unity.AI.Navigation.NavMeshSurface>() != null}");
            var surfaces = UnityEngine.Object.FindObjectsByType<Unity.AI.Navigation.NavMeshSurface>(FindObjectsSortMode.None);
            foreach (var s in surfaces) sb.AppendLine($"  surface en '{s.gameObject.name}' data={(s.navMeshData != null ? s.navMeshData.name : "null")} center={s.center} size={s.size}");
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }
}
