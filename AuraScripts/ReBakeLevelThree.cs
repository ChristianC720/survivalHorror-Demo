using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class ReBakeLevelThree
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            GameObject root = GameObject.Find("VS_Piso1");
            Transform geometry = root.transform.Find("Geometry");

            float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
            int floors = 0, walls = 0, lintels = 0;
            for (int i = 0; i < geometry.childCount; i++)
            {
                Transform t = geometry.GetChild(i);
                if (t.name.StartsWith("Suelo_")) floors++;
                else if (t.name == "Muro") walls++;
                else if (t.name == "Dintel") lintels++;

                var col = t.GetComponent<Collider>();
                if (col == null || t.name == "Dintel") continue;
                Bounds b = col.bounds;
                minX = Mathf.Min(minX, b.min.x); maxX = Mathf.Max(maxX, b.max.x);
                minZ = Mathf.Min(minZ, b.min.z); maxZ = Mathf.Max(maxZ, b.max.z);
            }
            sb.AppendLine($"GEOMETRIA suelos={floors} muros={walls} dinteles={lintels} extension x[{minX:F1},{maxX:F1}] z[{minZ:F1},{maxZ:F1}]");
            foreach (string floor in new[] { "Suelo_FilaCapsulas", "Suelo_Pasillo", "Suelo_Almacen", "Suelo_SalaAbierta", "Suelo_SalaVigilancia", "Suelo_SalaClaveSecundaria", "Suelo_SalaClaveFinal" })
            {
                Transform t = geometry.Find(floor);
                sb.AppendLine(t != null ? $"  {floor} pos={t.position} escala={t.localScale}" : $"  FALTA {floor}");
            }

            Physics.SyncTransforms();

            Type tool = Type.GetType("Heartbeat.EditorTools.NavMeshBakeTool, Assembly-CSharp-Editor");
            MethodInfo bake = tool != null ? tool.GetMethod("BakeActiveScene", BindingFlags.Public | BindingFlags.Static) : null;
            if (bake == null) { sb.AppendLine("NO se encontro NavMeshBakeTool.BakeActiveScene"); return sb.ToString(); }
            bake.Invoke(null, null);

            var surface = root.GetComponent<Unity.AI.Navigation.NavMeshSurface>();
            sb.AppendLine($"SURFACE data={(surface.navMeshData != null ? surface.navMeshData.name : "null")} center={surface.center} size={surface.size}");

            NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
            if (tri.vertices != null && tri.vertices.Length > 0)
            {
                Vector3 lo = tri.vertices[0], hi = tri.vertices[0];
                foreach (Vector3 v in tri.vertices) { lo = Vector3.Min(lo, v); hi = Vector3.Max(hi, v); }
                sb.AppendLine($"NAVMESH vertices={tri.vertices.Length} triangulos={(tri.indices != null ? tri.indices.Length / 3 : 0)} extension x[{lo.x:F1},{hi.x:F1}] z[{lo.z:F1},{hi.z:F1}]");
            }
            else sb.AppendLine("NAVMESH sin vertices");
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }
}
