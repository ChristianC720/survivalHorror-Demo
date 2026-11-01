using System.Collections;
using System.Reflection;
using System.Text;
using Heartbeat.AI;
using UnityEngine;
using UnityEngine.AI;

public static class RastreroSecondDeathProbeFour
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    private static object Field(object target, string name)
    {
        FieldInfo f = target.GetType().GetField(name, Flags);
        return f != null ? f.GetValue(target) : null;
    }

    private static void SetField(object target, string name, object value)
    {
        FieldInfo f = target.GetType().GetField(name, Flags);
        if (f != null) f.SetValue(target, value);
    }

    private static void Check(StringBuilder sb, string label, bool ok, string detail)
    {
        sb.AppendLine($"[{(ok ? "OK" : "FALLO")}] {label}: {detail}");
    }

    public static string Execute()
    {
        var sb = new StringBuilder();

        EnemyAI target = null;
        foreach (EnemyAI e in Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            EnemyHealth h = e.GetComponent<EnemyHealth>();
            if (e.Type == EnemyType.Rastrero && h != null && h.HasBeenRecycled && !h.IsDead) { target = e; break; }
        }

        if (target == null) { sb.AppendLine("No hay ningún Rastrero reciclado vivo."); return sb.ToString(); }

        EnemyHealth health = target.GetComponent<EnemyHealth>();
        Vector3 name = target.transform.position;

        Check(sb, "cadáver anterior retirado", health.GetComponent<EnemyCorpse>() == null, $"EnemyCorpse={health.GetComponent<EnemyCorpse>()}");

        sb.AppendLine($"### Segunda muerte de {target.name} (reciclado, {health.ShotsLeft} disparo)");

        health.ApplyDamage(1, name + Vector3.up);
        Check(sb, "muere de 1 disparo estando reciclado", health.IsDead, $"IsDead={health.IsDead} Health={health.Health}");

        EnemyCorpse corpse = health.GetComponent<EnemyCorpse>();
        Check(sb, "cadáver nuevo creado", corpse != null, corpse != null ? "presente" : "sin EnemyCorpse");
        if (corpse != null)
        {
            object allow = Field(corpse, "allowRecycle");
            Check(sb, "cuerpo gastado: no se reciclará", allow is bool && !(bool)allow, $"allowRecycle={allow}");
            Check(sb, "sigue en el suelo", Mathf.Abs(corpse.Position.y) < 0.15f, $"Pos.y={corpse.Position.y:F3}");

            MethodInfo tryRecycle = typeof(EnemyCorpse).GetMethod("TryRecycle", Flags);
            object recycled = tryRecycle.Invoke(corpse, null);
            Check(sb, "no se recicla por segunda vez", recycled is bool && !(bool)recycled, $"TryRecycle={recycled} IsDead={health.IsDead}");

            // Cumple su tiempo: debe retirarse en silencio.
            SetField(corpse, "age", 999f);
            MethodInfo update = typeof(EnemyCorpse).GetMethod("Update", Flags);
            update.Invoke(corpse, null);
            sb.AppendLine($"  cuerpo gastado tras cumplir su tiempo: {(corpse == null ? "retirado" : "todavía presente (Destroy diferido)")}");
        }

        sb.AppendLine("--- Estado ---");
        object registered = Field(EnemyDirector.Instance, "Registered");
        var list = registered as ICollection;
        sb.AppendLine($"  Registered={(list != null ? list.Count : -1)}  cadáveres={EnemyCorpse.All.Count}");
        foreach (EnemyAI e in Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            EnemyHealth h = e.GetComponent<EnemyHealth>();
            sb.AppendLine($"  {e.name}: Type={e.Type} IsDead={h.IsDead} ShotsLeft={h.ShotsLeft} reciclado={h.HasBeenRecycled} ai={e.enabled} agent={e.GetComponent<NavMeshAgent>().enabled}");
        }

        return sb.ToString();
    }
}
