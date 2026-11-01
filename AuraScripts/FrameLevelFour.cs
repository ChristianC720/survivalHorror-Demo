using System;
using UnityEditor;
using UnityEngine;

public static class FrameLevelFour
{
    public static object Execute()
    {
        try
        {
            SceneView view = SceneView.lastActiveSceneView;
            if (view == null) return "sin SceneView";
            view.orthographic = true;
            view.pivot = new Vector3(43f, 0f, 4f);
            view.rotation = Quaternion.Euler(90f, 0f, 0f);
            view.size = 55f;
            view.Repaint();

            Camera overview = Camera.main;
            if (overview != null)
            {
                overview.transform.SetPositionAndRotation(new Vector3(43f, 60f, 4f), Quaternion.Euler(90f, 0f, 0f));
                overview.farClipPlane = 400f;
            }
            return $"sceneview pivot={view.pivot} size={view.size} ortho={view.orthographic}";
        }
        catch (Exception e)
        {
            return "EXCEPCION: " + e;
        }
    }
}
