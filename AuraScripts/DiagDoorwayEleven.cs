using System;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class DiagDoorwayEleven
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            Transform geometry = GameObject.Find("VS_Piso1").transform.Find("Geometry");

            sb.AppendLine("--- muros junto a x=62 ---");
            for (int i = 0; i < geometry.childCount; i++)
            {
                Transform t = geometry.GetChild(i);
                if (!t.name.StartsWith("Muro")) continue;
                if (Mathf.Abs(t.position.x - 62f) > 0.3f) continue;
                float zMin = t.position.z - t.localScale.z * 0.5f;
                float zMax = t.position.z + t.localScale.z * 0.5f;
                sb.AppendLine($"  {t.name} pos={t.position} scale={t.localScale} z[{zMin:F2},{zMax:F2}]");
            }

            sb.AppendLine("--- muros junto a z=6 ---");
            for (int i = 0; i < geometry.childCount; i++)
            {
                Transform t = geometry.GetChild(i);
                if (!t.name.StartsWith("Muro")) continue;
                if (Mathf.Abs(t.position.z - 6f) > 0.3f) continue;
                float xMin = t.position.x - t.localScale.x * 0.5f;
                float xMax = t.position.x + t.localScale.x * 0.5f;
                sb.AppendLine($"  {t.name} pos={t.position} scale={t.localScale} x[{xMin:F2},{xMax:F2}]");
            }

            sb.AppendLine("--- navmesh en el vano x=62 ---");
            for (float z = -0.9f; z <= 0.91f; z += 0.3f)
            {
                bool hit = NavMesh.SamplePosition(new Vector3(62f, 0.2f, z), out NavMeshHit h, 0.4f, NavMesh.AllAreas);
                sb.AppendLine($"  z={z:F1}: navmesh={hit}{(hit ? $" y={h.position.y:F2}" : "")}");
            }

            sb.AppendLine("--- navmesh en el vano z=6 ---");
            for (float x = 68.1f; x <= 69.91f; x += 0.3f)
            {
                bool hit = NavMesh.SamplePosition(new Vector3(x, 0.2f, 6f), out NavMeshHit h, 0.4f, NavMesh.AllAreas);
                sb.AppendLine($"  x={x:F1}: navmesh={hit}{(hit ? $" y={h.position.y:F2}" : "")}");
            }

            sb.AppendLine("--- cruces ---");
            Cross(sb, "x=62", new Vector3(61f, 0.05f, 0f), new Vector3(63f, 0.05f, 0f));
            Cross(sb, "z=6", new Vector3(69f, 0.05f, 5f), new Vector3(69f, 0.05f, 7f));

            sb.AppendLine("--- hojas ---");
            Transform doors = GameObject.Find("VS_Piso1").transform.Find("Doors");
            for (int i = 0; i < doors.childCount; i++)
            {
                Transform hinge = doors.GetChild(i);
                Transform leaf = hinge.Find("Hoja");
                var col = leaf.GetComponent<Collider>();
                sb.AppendLine($"  {hinge.name}: hingeRot={hinge.localEulerAngles} leafWorld={leaf.position} leafWorldRotY={leaf.eulerAngles.y:F1} bounds={col.bounds.min}..{col.bounds.max}");
            }
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }

    private static void Cross(StringBuilder sb, string label, Vector3 a, Vector3 b)
    {
        var path = new NavMeshPath();
        bool ok = NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path);
        sb.AppendLine($"  {label}: {a} -> {b} ok={ok} estado={path.status} esquinas={path.corners.Length}");
    }
}
