using System;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public static class PathCheckThirteen
{
    public static object Execute()
    {
        var sb = new StringBuilder();
        try
        {
            Check(sb, "FilaCapsulas->Pasillo", new Vector3(0f, 0.05f, 0f), new Vector3(24f, 0.05f, 0f));
            Check(sb, "Pasillo->SalaAbierta", new Vector3(24f, 0.05f, 0f), new Vector3(51f, 0.05f, 0f));
            Check(sb, "SalaAbierta->SalaClaveSecundaria", new Vector3(51f, 0.05f, 0f), new Vector3(69f, 0.05f, 0f));
            Check(sb, "SalaClaveSecundaria->SalaVigilancia", new Vector3(69f, 0.05f, 0f), new Vector3(69f, 0.05f, 12f));
            Check(sb, "SalaClaveSecundaria->SalaClaveFinal (cerrada)", new Vector3(69f, 0.05f, 0f), new Vector3(85f, 0.05f, 0f));
            Check(sb, "Pasillo->Almacen (cerrada)", new Vector3(24f, 0.05f, 0f), new Vector3(16f, 0.05f, 9f));
            Check(sb, "FilaCapsulas->SalaClaveSecundaria", new Vector3(0f, 0.05f, 0f), new Vector3(69f, 0.05f, 0f));

            sb.AppendLine("--- cruces de vano ---");
            Cross(sb, "x=8", new Vector3(7f, 0.05f, 0f), new Vector3(9f, 0.05f, 0f));
            Cross(sb, "x=40", new Vector3(39f, 0.05f, 0f), new Vector3(41f, 0.05f, 0f));
            Cross(sb, "x=62", new Vector3(61f, 0.05f, 0f), new Vector3(63f, 0.05f, 0f));
            Cross(sb, "z=6", new Vector3(69f, 0.05f, 5f), new Vector3(69f, 0.05f, 7f));
            Cross(sb, "x=76 (cerrada)", new Vector3(75f, 0.05f, 0f), new Vector3(77f, 0.05f, 0f));
            Cross(sb, "z=2 (cerrada)", new Vector3(16f, 0.05f, 1f), new Vector3(16f, 0.05f, 3f));
        }
        catch (Exception e)
        {
            sb.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }
        return sb.ToString();
    }

    private static void Check(StringBuilder sb, string label, Vector3 a, Vector3 b)
    {
        var path = new NavMeshPath();
        bool ok = NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path);
        float restante = path.corners.Length > 0 ? Vector3.Distance(path.corners[path.corners.Length - 1], b) : -1f;
        sb.AppendLine($"  {label}: ok={ok} estado={path.status} esquinas={path.corners.Length} resta={restante:F2} m");
    }

    private static void Cross(StringBuilder sb, string label, Vector3 a, Vector3 b)
    {
        var path = new NavMeshPath();
        bool ok = NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path);
        sb.AppendLine($"  {label}: ok={ok} estado={path.status} esquinas={path.corners.Length}");
    }
}
