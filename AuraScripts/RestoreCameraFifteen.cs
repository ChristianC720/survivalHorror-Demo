using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class RestoreCameraFifteen
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            Transform player = GameObject.Find("Player").transform;
            Transform camera = player.Find("Camera");
            if (camera == null) return "no hay Player/Camera";

            camera.localPosition = new Vector3(0f, 1.6f, -3.2f);
            camera.localRotation = Quaternion.identity;
            sb.AppendLine($"Player/Camera local={camera.localPosition} world={camera.position}");

            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                view.orthographic = true;
                view.pivot = new Vector3(43f, 0f, 4f);
                view.rotation = Quaternion.Euler(90f, 0f, 0f);
                view.size = 55f;
                view.Repaint();
                sb.AppendLine($"SceneView reencuadrado a la planta ({view.pivot}, size {view.size})");
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
