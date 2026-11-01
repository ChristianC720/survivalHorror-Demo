using System;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WireCamerasSeven
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            Transform root = GameObject.Find("VS_Piso1").transform;
            Transform triggers = root.Find("Triggers");
            Transform props = root.Find("Props");

            foreach (string roomId in new[] { "SalaVigilancia", "SalaClaveSecundaria", "SalaClaveFinal" })
            {
                Transform trigger = triggers.Find(roomId);
                Transform prop = props.Find("CamaraVigilancia_" + roomId);
                if (trigger == null || prop == null) { sb.AppendLine($"  falta {roomId} trigger={trigger != null} prop={prop != null}"); continue; }

                var volume = trigger.GetComponent<Heartbeat.Level.RoomVolume>();
                var so = new SerializedObject(volume);
                SerializedProperty camProp = so.FindProperty("securityCameraProp");
                SerializedProperty hasCam = so.FindProperty("hasSecurityCamera");
                if (camProp == null) { sb.AppendLine($"  AVISO: {roomId} sin campo securityCameraProp"); continue; }

                camProp.objectReferenceValue = prop;
                if (hasCam != null) hasCam.boolValue = true;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(volume);
                sb.AppendLine($"  {roomId} -> {prop.name} (y={prop.eulerAngles.y:F0}°)");
            }

            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            bool saved = EditorSceneManager.SaveScene(scene);
            sb.AppendLine($"escena guardada={saved}");
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }
}
