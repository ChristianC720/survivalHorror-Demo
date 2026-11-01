using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class PlayCheckEight
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static GameObject player;
    private static Heartbeat.Player.PlayerHack hack;
    private static Heartbeat.Player.ThirdPersonCamera rig;
    private static Transform camTransform;

    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            player = GameObject.Find("Player");
            hack = player.GetComponent<Heartbeat.Player.PlayerHack>();
            camTransform = player.transform.Find("Camera");
            rig = camTransform.GetComponent<Heartbeat.Player.ThirdPersonCamera>();

            sb.AppendLine($"JUGADOR pos={player.transform.position} sala={RoomAt(player.transform.position)}");

            sb.AppendLine("--- salas y NavMesh ---");
            var rooms = new (string id, Vector3 center)[]
            {
                ("FilaCapsulas", new Vector3(0f, 0.05f, 0f)),
                ("Pasillo", new Vector3(24f, 0.05f, 0f)),
                ("Almacen", new Vector3(16f, 0.05f, 9f)),
                ("SalaAbierta", new Vector3(51f, 0.05f, 0f)),
                ("SalaVigilancia", new Vector3(69f, 0.05f, 12f)),
                ("SalaClaveSecundaria", new Vector3(69f, 0.05f, 0f)),
                ("SalaClaveFinal", new Vector3(85f, 0.05f, 0f)),
            };
            foreach (var room in rooms)
            {
                bool nav = NavMesh.SamplePosition(room.center, out NavMeshHit hit, 3f, NavMesh.AllAreas);
                sb.AppendLine($"  {room.id}: trigger={RoomAt(room.center)} navmesh={nav}{(nav ? $" y={hit.position.y:F2}" : "")}");
            }

            sb.AppendLine("--- caminos (NavMesh.CalculatePath) ---");
            CheckPath(sb, "FilaCapsulas", new Vector3(0f, 0.05f, 0f), "SalaClaveSecundaria", new Vector3(69f, 0.05f, 0f));
            CheckPath(sb, "Pasillo", new Vector3(24f, 0.05f, 0f), "Almacen", new Vector3(16f, 0.05f, 9f));
            CheckPath(sb, "SalaClaveSecundaria", new Vector3(69f, 0.05f, 0f), "SalaVigilancia", new Vector3(69f, 0.05f, 12f));
            CheckPath(sb, "SalaClaveSecundaria", new Vector3(69f, 0.05f, 0f), "SalaClaveFinal", new Vector3(85f, 0.05f, 0f));

            sb.AppendLine("--- puertas ---");
            Transform doors = GameObject.Find("VS_Piso1").transform.Find("Doors");
            for (int i = 0; i < doors.childCount; i++)
            {
                var door = doors.GetChild(i).GetComponent<Heartbeat.Level.SealableDoor>();
                sb.AppendLine($"  {door.name}: cerrada={door.IsClosed} sellada={door.IsSealed} llave={door.IsLocked} vano={door.OpeningCenter} brecha={door.BreachProgress01:F2}");
            }

            sb.AppendLine("--- paneles (deteccion del jugador) ---");
            LookAt("Panel_Almacen", new Vector3(14.4f, 1.45f, 1.79f), new Vector3(14.4f, 0.05f, 0.3f), sb);
            LookAt("Panel_SalaVigilancia", new Vector3(67.4f, 1.45f, 5.79f), new Vector3(67.4f, 0.05f, 4.3f), sb);
            LookAt("Panel_SalaClaveSecundaria", new Vector3(62.21f, 1.45f, -1.6f), new Vector3(63.5f, 0.05f, -1.6f), sb);
            LookAt("Panel_SalaClaveFinal", new Vector3(75.79f, 1.45f, -1.6f), new Vector3(74.5f, 0.05f, -1.6f), sb);

            sb.AppendLine("--- enemigos ---");
            GameObject enemies = GameObject.Find("Enemies");
            for (int i = 0; i < enemies.transform.childCount; i++)
            {
                Transform e = enemies.transform.GetChild(i);
                var agent = e.GetComponent<NavMeshAgent>();
                sb.AppendLine($"  {e.name} pos={e.position} sala={RoomAt(e.position)} enNavMesh={agent != null && agent.isOnNavMesh} agente={agent != null}");
            }
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }

    private static void CheckPath(StringBuilder sb, string from, Vector3 a, string to, Vector3 b)
    {
        var path = new NavMeshPath();
        bool computed = NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path);
        sb.AppendLine($"  {from} -> {to}: ok={computed} estado={path.status} esquinas={path.corners.Length}");
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

        sb.AppendLine($"  {panelName}: jugador={posicion} target={hack.HasTarget} score={hack.CenterScore:F3} prompt='{hack.Prompt}' arranque={!hack.IsHacking}");
    }

    private static string RoomAt(Vector3 position)
    {
        var room = Heartbeat.Level.RoomVolume.FindContaining(position);
        return room == null ? "FUERA" : room.RoomId + (room.IsSafeZone ? "(segura)" : "");
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
