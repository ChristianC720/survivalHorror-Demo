using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class TidyRemakeFourteen
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            Camera overview = Camera.main;
            sb.AppendLine(overview != null
                ? $"Camera.main = '{overview.name}' pos={overview.transform.position} rot={overview.transform.eulerAngles} padre={(overview.transform.parent != null ? overview.transform.parent.name : "raiz")}"
                : "Camera.main = null");

            if (overview != null && overview.transform.parent == null)
            {
                overview.transform.SetPositionAndRotation(new Vector3(43f, 46f, -40f), Quaternion.Euler(50f, 0f, 0f));
                overview.farClipPlane = 400f;
                sb.AppendLine($"camara de planta recolocada a {overview.transform.position}");
            }

            if (AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GetAssetPath(AssetDatabase.LoadMainAssetAtPath("Assets/Screenshots/remake_planta.png"))) != null)
            {
                bool deleted = AssetDatabase.DeleteAsset("Assets/Screenshots/remake_planta.png");
                sb.AppendLine($"captura temporal borrada={deleted}");
            }
            else sb.AppendLine("captura temporal: no estaba");

            Transform geometry = GameObject.Find("VS_Piso1").transform.Find("Geometry");
            int muros = 0, dinteles = 0, suelos = 0;
            for (int i = 0; i < geometry.childCount; i++)
            {
                string n = geometry.GetChild(i).name;
                if (n == "Muro") muros++;
                else if (n == "Dintel") dinteles++;
                else if (n.StartsWith("Suelo_")) suelos++;
            }
            sb.AppendLine($"GEOMETRIA suelos={suelos} muros={muros} dinteles={dinteles}");

            Transform props = GameObject.Find("VS_Piso1").transform.Find("Props");
            sb.AppendLine($"PROPS ({props.childCount}):");
            for (int i = 0; i < props.childCount; i++)
            {
                Transform t = props.GetChild(i);
                sb.AppendLine($"  {t.name} pos={t.position} scale={t.localScale}");
            }

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
