using System;
using UnityEditor;
using UnityEngine;

public static class FrameLevelFive
{
    public static object Execute(Newtonsoft.Json.Linq.JObject args)
    {
        try
        {
            float x = args != null && args["x"] != null ? (float)args["x"] : 43f;
            float z = args != null && args["z"] != null ? (float)args["z"] : 4f;
            float size = args != null && args["size"] != null ? (float)args["size"] : 55f;
            float tilt = args != null && args["tilt"] != null ? (float)args["tilt"] : 90f;

            SceneView view = SceneView.lastActiveSceneView;
            if (view == null) return "sin SceneView";
            view.orthographic = true;
            view.pivot = new Vector3(x, 0f, z);
            view.rotation = Quaternion.Euler(tilt, 0f, 0f);
            view.size = size;
            view.Repaint();
            return $"pivot=({x},{z}) size={size} tilt={tilt}";
        }
        catch (Exception e)
        {
            return "EXCEPCION: " + e;
        }
    }
}
