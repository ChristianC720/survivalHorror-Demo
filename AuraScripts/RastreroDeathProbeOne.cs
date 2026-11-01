using System.Collections;
using System.Reflection;
using System.Text;
using Heartbeat.AI;
using UnityEngine;
using UnityEngine.AI;

public static class RastreroDeathProbeOne
{
    private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    private static object Field(object target, string name)
    {
        if (target == null) return null;
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

        EnemyDirector director = EnemyDirector.Instance;
        sb.AppendLine($"--- Director antes ---");
        foreach (FieldInfo f in typeof(EnemyDirector).GetFields(Flags))
        {
            object v = f.GetValue(director);
            ICollection c = v as ICollection;
            sb.AppendLine($"  {f.Name} = {(c != null ? "Count=" + c.Count : (v != null ? v.ToString() : "null"))}");
        }

        EnemyAI[] enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        sb.AppendLine($"--- Enemigos en escena: {enemies.Length} ---");
        foreach (EnemyAI e in enemies)
        {
            EnemyHealth h = e.GetComponent<EnemyHealth>();
            NavMeshAgent a = e.GetComponent<NavMeshAgent>();
            Collider col = e.GetComponent<Collider>();
            sb.AppendLine($"  {e.name}: Type={e.Type} MaxHealth={h.MaxHealth} Health={h.Health} ShotsLeft={h.ShotsLeft} IsDead={h.IsDead} aiEnabled={e.enabled} agentEnabled={(a != null ? a.enabled.ToString() : "sin agente")} collider={col.GetType().Name} enabled={col.enabled} pos={e.transform.position}");
        }

        sb.AppendLine("--- Muerte de cada Rastrero ---");
        foreach (EnemyAI e in enemies)
        {
            if (e.Type != EnemyType.Rastrero) continue;

            EnemyHealth h = e.GetComponent<EnemyHealth>();
            NavMeshAgent a = e.GetComponent<NavMeshAgent>();
            Collider col = e.GetComponent<Collider>();
            Transform visual = h.transform.Find("Visual") != null ? h.transform.Find("Visual") : (h.GetComponentInChildren<Renderer>() != null ? h.GetComponentInChildren<Renderer>().transform : null);
            Vector3 aimPoint = h.transform.position + Vector3.up * 1f;

            sb.AppendLine($"### {e.name}");
            Check(sb, "perfil Rastrero 2 disparos", h.MaxHealth == 2 && h.ShotsLeft == 2, $"MaxHealth={h.MaxHealth} ShotsLeft={h.ShotsLeft}");
            Check(sb, "vivo antes del disparo", !h.IsDead && e.enabled && a.enabled && col.enabled, $"IsDead={h.IsDead} ai={e.enabled} agent={a.enabled} collider={col.enabled}");

            h.ApplyDamage(1, aimPoint);
            Check(sb, "tras 1 disparo sigue vivo", !h.IsDead && h.Health == 1 && h.ShotsLeft == 1, $"IsDead={h.IsDead} Health={h.Health} ShotsLeft={h.ShotsLeft} aiEnabled={e.enabled}");
            Check(sb, "tras 1 disparo conserva colisión y agente", col.enabled && a.enabled, $"collider={col.enabled} agent={a.enabled}");

            h.ApplyDamage(1, aimPoint);
            Check(sb, "tras 2 disparos muere", h.IsDead && h.Health == 0, $"IsDead={h.IsDead} Health={h.Health}");
            Check(sb, "IA apagada", !e.enabled, $"aiEnabled={e.enabled}");
            Check(sb, "agente apagado", !a.enabled, $"agentEnabled={a.enabled}");
            Check(sb, "colisión apagada", !col.enabled, $"colliderEnabled={col.enabled}");

            EnemyCorpse corpse = h.GetComponent<EnemyCorpse>();
            Check(sb, "cadáver creado", corpse != null, corpse != null ? $"EnemyCorpse presente; AllowRecycle={Field(corpse, "allowRecycle")}" : "sin EnemyCorpse");
            if (corpse != null)
            {
                Check(sb, "cadáver registrado", EnemyCorpse.All.Contains(corpse), $"Registry={EnemyCorpse.All.Count}");
                Check(sb, "cuerpo apoyado en el suelo", Mathf.Abs(corpse.Position.y) < 0.15f, $"Pos.y={corpse.Position.y:F3} (esperado ~0)");
                Check(sb, "punto de remate sobre el cuerpo", corpse.AimPoint.y > 0.2f && corpse.AimPoint.y < 1.2f, $"AimPoint.y={corpse.AimPoint.y:F3}");
                Check(sb, "cuerpo se puede rematar", corpse.CanBeFinished, $"CanBeFinished={corpse.CanBeFinished}");
                if (visual != null)
                {
                    sb.AppendLine($"  visual: localPos={visual.localPosition} localRot={visual.localRotation.eulerAngles} escala={visual.localScale} worldPos={visual.position}");
                }
            }

            InputProbeDamageGuard(sb, h);
        }

        sb.AppendLine("--- Director después ---");
        foreach (FieldInfo f in typeof(EnemyDirector).GetFields(Flags))
        {
            object v = f.GetValue(director);
            ICollection c = v as ICollection;
            sb.AppendLine($"  {f.Name} = {(c != null ? "Count=" + c.Count : (v != null ? v.ToString() : "null"))}");
        }

        return sb.ToString();
    }

    private static void InputProbeDamageGuard(StringBuilder sb, EnemyHealth h)
    {
        object dead = Field(h, "dead");
        Check(sb, "daño extra ignorado con el enemigo muerto", dead is bool && (bool)dead, $"dead={dead}");
    }
}
