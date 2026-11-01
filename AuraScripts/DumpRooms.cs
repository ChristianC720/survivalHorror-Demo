using System.IO;
using System.Text;
using Heartbeat.Level;
using UnityEngine;

public static class DumpRooms
{
    public static void Execute()
    {
        StringBuilder sb = new StringBuilder();
        RoomVolume[] rooms = Object.FindObjectsByType<RoomVolume>(FindObjectsSortMode.None);

        foreach (RoomVolume room in rooms)
        {
            BoxCollider box = room.GetComponent<BoxCollider>();
            if (box == null) continue;

            Vector3 center = room.transform.TransformPoint(box.center);
            Vector3 size = Vector3.Scale(box.size, room.transform.lossyScale);

            sb.Append(room.RoomId)
              .Append(" safe=").Append(room.IsSafeZone)
              .Append(" x[").Append((center.x - size.x / 2f).ToString("F1")).Append("..").Append((center.x + size.x / 2f).ToString("F1")).Append("]")
              .Append(" y[").Append((center.y - size.y / 2f).ToString("F1")).Append("..").Append((center.y + size.y / 2f).ToString("F1")).Append("]")
              .Append(" z[").Append((center.z - size.z / 2f).ToString("F1")).Append("..").Append((center.z + size.z / 2f).ToString("F1")).Append("]")
              .AppendLine();
        }

        sb.AppendLine("NavMeshSurface=" + (Object.FindAnyObjectByType<Unity.AI.Navigation.NavMeshSurface>() != null));

        File.WriteAllText("AuraScripts/rooms.txt", sb.ToString());
        Debug.Log("[Rooms] escrito AuraScripts/rooms.txt");
    }
}
