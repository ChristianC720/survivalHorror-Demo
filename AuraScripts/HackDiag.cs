using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class HackDiag
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public static object Execute()
    {
        var sb = new StringBuilder();

        GameObject player = GameObject.Find("Player");
        var hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
        Transform camTransform = player.transform.Find("Camera");
        var cam = camTransform.GetComponent<Camera>();
        var rig = camTransform.GetComponent<Heartbeat.Player.ThirdPersonCamera>();

        Transform panelsRoot = GameObject.Find("VS_Piso1").transform.Find("Panels");
        Transform panel = panelsRoot.Find("Panel_SalaVigilancia");

        var controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.SetPositionAndRotation(new Vector3(51.4f, 0.05f, 3.7f), Quaternion.identity);
        controller.enabled = true;

        Vector3 destino = panel.position;
        Vector3 dir = destino - player.transform.position;
        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        SetField(rig, "yaw", yaw);
        SetField(rig, "pitch", -4f);

        sb.AppendLine($"dir={dir} yaw={yaw:F1}");

        Invoke(rig, "LateUpdate");

        sb.AppendLine($"cam pos={camTransform.position} fwd={camTransform.forward} euler={camTransform.localEulerAngles}");
        sb.AppendLine($"viewport(panel)={cam.WorldToViewportPoint(destino)}");
        sb.AppendLine($"rig yaw={GetField<float>(rig, "yaw"):F1} pitch={GetField<float>(rig, "pitch"):F1}");
        sb.AppendLine($"panel.pos={panel.position} dist jugador={Vector3.Distance(player.transform.position, panel.position):F2}");

        // volver a aplicar y mirar otra vez
        Invoke(rig, "LateUpdate");
        sb.AppendLine($"2a pasada: cam pos={camTransform.position} fwd={camTransform.forward} viewport={cam.WorldToViewportPoint(destino)}");

        Invoke(hack, "Update");
        sb.AppendLine($"hack: HasTarget={hack.HasTarget} score={hack.CenterScore:F3} prompt='{hack.Prompt}'");
        sb.AppendLine($"config hackRange={GetField<Heartbeat.Config.GameConfig>(hack, "config").HackRange} aimRadius={GetField<Heartbeat.Config.GameConfig>(hack, "config").HackAimRadius}");

        var panelVigi = panel.GetComponent<Heartbeat.Level.HackPanel>();
        sb.AppendLine($"panel usable={panelVigi.IsUsable} registrados={Heartbeat.Level.HackPanel.All.Count}");

        return sb.ToString();
    }

    private static void Invoke(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, Any);
        if (method == null) throw new Exception($"Metodo '{methodName}' no encontrado en {target.GetType().Name}.");
        method.Invoke(target, null);
    }

    private static T GetField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, Any);
        if (field == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
        return (T)field.GetValue(target);
    }

    private static void SetField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, Any);
        if (field == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
        field.SetValue(target, value);
    }
}
