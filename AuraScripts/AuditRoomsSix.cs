using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class AuditRoomsSix
{
    private const BindingFlags Any = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            Transform triggers = GameObject.Find("VS_Piso1").transform.Find("Triggers");
            for (int i = 0; i < triggers.childCount; i++)
            {
                Transform t = triggers.GetChild(i);
                var room = t.GetComponent<Heartbeat.Level.RoomVolume>();
                if (room == null) { sb.AppendLine($"{t.name}: sin RoomVolume"); continue; }

                FieldInfo camField = room.GetType().GetField("securityCameraProp", Any);
                var camProp = camField != null ? camField.GetValue(room) as Transform : null;
                string camInfo = camField == null ? "campo?" : (camProp == null ? "NULL" : $"{camProp.name} pos={camProp.position}");

                var box = t.GetComponent<BoxCollider>();
                sb.AppendLine($"{room.RoomId}: seguro={room.IsSafeZone} camara={room.HasSecurityCamera} prop={camInfo} " +
                              $"adyacentes={room.AdjacentRooms.Count} box={(box != null ? box.size.ToString() : "sin box")}");
            }

            sb.AppendLine("--- props de cámara en escena ---");
            Transform props = GameObject.Find("VS_Piso1").transform.Find("Props");
            for (int i = 0; i < props.childCount; i++)
            {
                Transform t = props.GetChild(i);
                if (t.name.StartsWith("CamaraVigilancia_")) sb.AppendLine($"  {t.name} pos={t.position} rot={t.localEulerAngles}");
            }

            sb.AppendLine("--- SurveillanceView ---");
            var sv = GameObject.Find("Player").GetComponent<Heartbeat.Player.SurveillanceView>();
            sb.AppendLine($"  existe={sv != null}");

            sb.AppendLine("--- puertas / paneles ---");
            Transform doors = GameObject.Find("VS_Piso1").transform.Find("Doors");
            for (int i = 0; i < doors.childCount; i++)
            {
                Transform hinge = doors.GetChild(i);
                var door = hinge.GetComponent<Heartbeat.Level.SealableDoor>();
                sb.AppendLine($"  {hinge.name} hinge={hinge.position} leafLocal={hinge.Find("Hoja").localPosition} door={(door != null)}");
            }
            Transform panels = GameObject.Find("VS_Piso1").transform.Find("Panels");
            for (int i = 0; i < panels.childCount; i++)
            {
                Transform t = panels.GetChild(i);
                sb.AppendLine($"  {t.name} pos={t.position}");
            }
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }
}
