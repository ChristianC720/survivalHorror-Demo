public class AuraFrame
{
    public static string Execute()
    {
        if (!UnityEngine.Application.isPlaying) return "NO_PLAY";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        if (player == null) return "NO_PLAYER";

        UnityEngine.GameObject camGo = UnityEngine.GameObject.Find("CamaraVigilanciaFeed");
        if (camGo == null) return "NO_FEEDCAM";
        UnityEngine.Camera cam = camGo.GetComponent<UnityEngine.Camera>();

        UnityEngine.MonoBehaviour room = null;
        UnityEngine.MonoBehaviour[] all = UnityEngine.Object.FindObjectsByType<UnityEngine.MonoBehaviour>(UnityEngine.FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null && all[i].GetType().Name == "RoomVolume" && all[i].gameObject.name == "SalaVigilancia") room = all[i];
        }
        if (room == null) return "NO_ROOM";

        UnityEngine.Renderer rend = room.GetComponent<UnityEngine.Renderer>();
        UnityEngine.Collider col = room.GetComponent<UnityEngine.Collider>();
        UnityEngine.Bounds b;
        if (rend != null) b = rend.bounds;
        else if (col != null) b = col.bounds;
        else b = new UnityEngine.Bounds(room.transform.position, room.transform.lossyScale);

        sb.Append("roomCenter=").Append(b.center.ToString()).Append(" size=").Append(b.size.ToString());
        sb.Append(" dist=").Append(UnityEngine.Vector3.Distance(cam.transform.position, b.center).ToString("F2"));

        Vector3[] cs = new Vector3[8];
        int k = 0;
        for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    cs[k++] = b.center + new Vector3(b.extents.x * x, b.extents.y * y, b.extents.z * z);
                }

        float minx = 999f, maxx = -999f, miny = 999f, maxy = -999f;
        float inside = 0f;
        for (int i = 0; i < 8; i++)
        {
            Vector3 v = cam.WorldToViewportPoint(cs[i]);
            bool behind = v.z <= 0f;
            if (behind) { sb.Append(" corner").Append(i).Append("=behind"); continue; }
            if (v.x < minx) minx = v.x;
            if (v.x > maxx) maxx = v.x;
            if (v.y < miny) miny = v.y;
            if (v.y > maxy) maxy = v.y;
            inside += 1f;
        }
        sb.Append(" viewportX=").Append(minx.ToString("F2")).Append("..").Append(maxx.ToString("F2"));
        sb.Append(" viewportY=").Append(miny.ToString("F2")).Append("..").Append(maxy.ToString("F2"));
        sb.Append(" cornersInFront=").Append(inside.ToString("F0"));

        Vector3 pv = cam.WorldToViewportPoint(player.transform.position);
        sb.Append(" playerVP=").Append(pv.x.ToString("F2")).Append(",").Append(pv.y.ToString("F2")).Append(",").Append(pv.z.ToString("F2"));

        UnityEngine.Light[] lights = UnityEngine.Object.FindObjectsByType<UnityEngine.Light>(UnityEngine.FindObjectsSortMode.None);
        int lit = 0;
        int inRoom = 0;
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            if (!lights[i].isActiveAndEnabled) continue;
            if (!b.Contains(lights[i].transform.position)) continue;
            inRoom++;
            sb.Append(" | light ").Append(lights[i].gameObject.name)
              .Append(" type=").Append(lights[i].type.ToString())
              .Append(" int=").Append(lights[i].intensity.ToString("F2"))
              .Append(" range=").Append(lights[i].range.ToString("F1"));
            if (lit < 5) lit++;
        }
        sb.Append(" activeLightsInRoom=").Append(inRoom.ToString());

        System.Reflection.FieldInfo fi = cam.GetType().GetField("m_AllowHDR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        sb.Append(" camHDR=").Append(cam.allowHDR).Append(" culling=0x").Append(cam.cullingMask.ToString("X"));

        return sb.ToString();
    }
}
