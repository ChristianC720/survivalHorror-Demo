using System.Reflection;
using System.Text;
using Heartbeat.AI;
using UnityEngine;
using UnityEngine.AI;

public static class RastreroRecycleProbeThree
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    private static object Field(object target, string name)
    {
        FieldInfo f = target.GetType().GetField(name, Flags);
        return f != null ? f.GetValue(target) : null;
    }

    private static void Check(StringBuilder sb, string label, bool ok, string detail)
    {
        sb.AppendLine($"[{(ok ? "OK" : "FALLO")}] {label}: {detail}");
    }

    public static string Execute()
    {
        var sb = new StringBuilder();

        EnemyAI[] enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        int before = EnemyCorpse.All.Count;
        sb.AppendLine($"Cadáveres registrados antes: {before}");

        foreach (EnemyAI e in enemies)
        {
            if (e.Type != EnemyType.Rastrero) continue;

            EnemyHealth h = e.GetComponent<EnemyHealth>();
            if (!h.IsDead) continue;

            EnemyCorpse corpse = h.GetComponent<EnemyCorpse>();
            if (corpse == null) { sb.AppendLine($"### {e.name}: sin cadáver, se omite"); continue; }

            WaterRoom original = RoomOf(corpse.Position);
            Vector3 beforePos = corpse.Position;

            // Adelanta el aviso hasta el final y ejecuta el reciclaje.
            MethodInfo tryRecycle = typeof(EnemyCorpse).GetMethod("TryRecycle", Flags);
            object result = tryRecycle.Invoke(corpse, null);

            NavMeshAgent a = e.GetComponent<NavMeshAgent>();
            Collider col = e.GetComponent<Collider>();

            sb.AppendLine($"### {e.name}");
            Check(sb, "el reciclaje se ejecuta", result is bool && (bool)result, $"TryRecycle={result}");
            Check(sb, "revive el enemigo", !h.IsDead, $"IsDead={h.IsDead} Health={h.Health}");
            Check(sb, "marcado como reciclado", h.HasBeenRecycled, $"HasBeenRecycled={h.HasBeenRecycled}");
            Check(sb, "vuelve con 1 disparo (Rastrero reciclado)", h.ShotsLeft == 1, $"ShotsLeft={h.ShotsLeft} MaxHealth={h.MaxHealth}");
            Check(sb, "IA reactivada", e.enabled, $"aiEnabled={e.enabled} estado={Field(e, "state")}");
            Check(sb, "agente reactivado", a.enabled, $"agentEnabled={a.enabled}");
            Check(sb, "colision reactivada", col.enabled, $"colliderEnabled={col.enabled}");
            Check(sb, "cambia de sala", RoomOf(e.transform.position) != original, $"antes={original} ahora={RoomOf(e.transform.position)} posAntes={beforePos} posAhora={e.transform.position}");
            Check(sb, "vuelve al registro del director", RegisteredContains(e), $"Registered={RegisteredCount()}");
        }

        sb.AppendLine("--- Estado final ---");
        foreach (EnemyAI e in Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            EnemyHealth h = e.GetComponent<EnemyHealth>();
            sb.AppendLine($"  {e.name}: Type={e.Type} IsDead={h.IsDead} Health={h.Health} ShotsLeft={h.ShotsLeft} reciclado={h.HasBeenRecycled} ai={e.enabled} corpse={(h.GetComponent<EnemyCorpse>() != null ? "si" : "no")}");
        }
        sb.AppendLine($"Cadáveres registrados ahora: {EnemyCorpse.All.Count}");

        return sb.ToString();
    }

    private enum WaterRoom { Ninguna, FilaCapsulas, Pasillo, Almacen, SalaAbierta, SalaClaveSecundaria, SalaVigilancia, SalaClaveFinal }

    private static WaterRoom RoomOf(Vector3 position)
    {
        Heartbeat.Level.RoomVolume room = Heartbeat.Level.RoomVolume.FindContaining(position + Vector3.up * 0.5f);
        if (room == null) return WaterRoom.Ninguna;
        switch (room.RoomId)
        {
            case "FilaCapsulas": return WaterRoom.FilaCapsulas;
            case "Pasillo": return WaterRoom.Pasillo;
            case "Almacen": return WaterRoom.Almacen;
            case "SalaAbierta": return WaterRoom.SalaAbierta;
            case "SalaClaveSecundaria": return WaterRoom.SalaClaveSecundaria;
            case "SalaVigilancia": return WaterRoom.SalaVigilancia;
            case "SalaClaveFinal": return WaterRoom.SalaClaveFinal;
            default: return WaterRoom.Ninguna;
        }
    }

    private static int RegisteredCount()
    {
        object registered = Field(EnemyDirector.Instance, "Registered");
        var list = registered as System.Collections.ICollection;
        return list != null ? list.Count : -1;
    }

    private static bool RegisteredContains(EnemyAI enemy)
    {
        object registered = Field(EnemyDirector.Instance, "Registered");
        var list = registered as System.Collections.IEnumerable;
        if (list == null) return false;
        foreach (object o in list) if (ReferenceEquals(o, enemy)) return true;
        return false;
    }
}
