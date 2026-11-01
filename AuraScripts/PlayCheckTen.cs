using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class PlayCheckTen
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private static GameObject player;
    private static Heartbeat.Player.PlayerHack hack;
    private static Heartbeat.Player.ThirdPersonCamera rig;

    public static object Execute(Newtonsoft.Json.Linq.JObject args)
    {
        var sb = new StringBuilder();
        try
        {
            player = GameObject.Find("Player");
            hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
            rig = player.transform.Find("Camera").GetComponent<Heartbeat.Player.ThirdPersonCamera>();

            string mode = args != null && args["mode"] != null ? (string)args["mode"] : "paths";

            if (mode == "pose")
            {
                var position = new Vector3((float)args["x"], 0.05f, (float)args["z"]);
                var controller = player.GetComponent<CharacterController>();
                controller.enabled = false;
                player.transform.SetPositionAndRotation(position, Quaternion.identity);
                controller.enabled = true;

                SetField(rig, "yaw", (float)args["yaw"]);
                SetField(rig, "pitch", -8f);
                SetField(rig, "smoothedPivot", position);
                for (int i = 0; i < 120; i++) Invoke(rig, "LateUpdate");
                return $"pose aplicada: jugador={player.transform.position} sala={(Heartbeat.Level.RoomVolume.FindContaining(position)?.RoomId ?? "FUERA")} yaw={args["yaw"]}";
            }

            sb.AppendLine("--- caminos ---");
            CheckPath(sb, "FilaCapsulas->SalaClaveSecundaria", new Vector3(0f, 0.05f, 0f), new Vector3(69f, 0.05f, 0f));
            CheckPath(sb, "Pasillo->Almacen (cerrada)", new Vector3(24f, 0.05f, 0f), new Vector3(16f, 0.05f, 9f));
            CheckPath(sb, "SalaClaveSecundaria->SalaVigilancia", new Vector3(69f, 0.05f, 0f), new Vector3(69f, 0.05f, 12f));
            CheckPath(sb, "SalaClaveSecundaria->SalaClaveFinal (llave)", new Vector3(69f, 0.05f, 0f), new Vector3(85f, 0.05f, 0f));
            CheckPath(sb, "Pasillo->SalaAbierta", new Vector3(24f, 0.05f, 0f), new Vector3(51f, 0.05f, 0f));

            sb.AppendLine("--- paneles (a 1.7 m) ---");
            LookAt("Panel_Almacen", new Vector3(14.4f, 1.45f, 1.79f), new Vector3(14.4f, 0.05f, 0.45f), sb);
            LookAt("Panel_SalaVigilancia", new Vector3(67.4f, 1.45f, 5.79f), new Vector3(67.4f, 0.05f, 4.45f), sb);
            LookAt("Panel_SalaClaveSecundaria", new Vector3(62.21f, 1.45f, -1.6f), new Vector3(63.4f, 0.05f, -1.6f), sb);
            LookAt("Panel_SalaClaveFinal", new Vector3(75.79f, 1.45f, -1.6f), new Vector3(74.3f, 0.05f, -1.6f), sb);
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }

    private static void CheckPath(StringBuilder sb, string label, Vector3 a, Vector3 b)
    {
        var path = new NavMeshPath();
        bool computed = NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path);
        float restante = path.corners.Length > 0 ? Vector3.Distance(path.corners[path.corners.Length - 1], b) : -1f;
        sb.AppendLine($"  {label}: ok={computed} estado={path.status} esquinas={path.corners.Length} resta={restante:F2} m");
    }

    private static void LookAt(string panelName, Vector3 destino, Vector3 posicion, StringBuilder sb)
    {
        var controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.SetPositionAndRotation(posicion, Quaternion.identity);
        controller.enabled = true;

        Vector3 dir = destino - posicion;
        SetField(rig, "yaw", Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
        SetField(rig, "pitch", -4f);
        SetField(rig, "smoothedPivot", posicion);
        for (int i = 0; i < 80; i++) Invoke(rig, "LateUpdate");
        Invoke(hack, "Update");

        sb.AppendLine($"  {panelName}: distancia={Vector3.Distance(posicion, destino):F2} target={hack.HasTarget} score={hack.CenterScore:F3} prompt='{hack.Prompt}'");
    }

    private static void Invoke(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, Any);
        if (method == null) throw new Exception($"Metodo '{methodName}' no encontrado.");
        method.Invoke(target, null);
    }

    private static void SetField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, Any);
        if (field == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");
        field.SetValue(target, value);
    }
}
