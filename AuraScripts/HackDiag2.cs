using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class HackDiag2
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public static object Execute()
    {
        var sb = new StringBuilder();

        GameObject player = GameObject.Find("Player");
        var hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
        var health = player.GetComponent<Heartbeat.Player.PlayerHealth>();
        var motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();

        sb.AppendLine($"muerte={health.IsDead} golpes={health.CurrentHits}/{health.MaxHits} hacking={hack.IsHacking} motorBloqueado={motor.IsMovementLocked}");
        sb.AppendLine($"config={(GetField<object>(hack, "config") != null)} cam={(GetField<object>(hack, "viewCamera") != null)} root={(GetField<object>(hack, "playerRoot") != null)}");

        Transform panelsRoot = GameObject.Find("VS_Piso1").transform.Find("Panels");
        Transform panel = panelsRoot.Find("Panel_SalaVigilancia");
        var camTransform = player.transform.Find("Camera");
        var cam = camTransform.GetComponent<Camera>();
        var rig = camTransform.GetComponent<Heartbeat.Player.ThirdPersonCamera>();

        var controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.SetPositionAndRotation(new Vector3(51.4f, 0.05f, 3.7f), Quaternion.identity);
        controller.enabled = true;

        Vector3 destino = panel.position;
        Vector3 dir = destino - player.transform.position;
        SetField(rig, "yaw", Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
        SetField(rig, "pitch", -4f);

        for (int i = 0; i < 8; i++) Invoke(rig, "LateUpdate");

        sb.AppendLine($"cam pos={camTransform.position} viewport={cam.WorldToViewportPoint(destino)} dist={Vector3.Distance(player.transform.position, destino):F2}");

        Invoke(hack, "Update");
        sb.AppendLine($"-> HasTarget={hack.HasTarget} score={hack.CenterScore:F3} prompt='{hack.Prompt}'");

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
