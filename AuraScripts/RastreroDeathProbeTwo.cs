using System.Collections;
using System.Reflection;
using System.Text;
using Heartbeat.AI;
using UnityEngine;
using UnityEngine.AI;

public static class RastreroDeathProbeTwo
{
    private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    private static object Field(object target, string name)
    {
        if (target == null) return null;
        FieldInfo f = target.GetType().GetField(name, Flags);
        return f != null ? f.GetValue(target) : null;
    }

    private static bool InRegistry(EnemyCorpse corpse)
    {
        for (int i = 0; i < EnemyCorpse.All.Count; i++) if (EnemyCorpse.All[i] == corpse) return true;
        return false;
    }

    private static void Check(StringBuilder sb, string label, bool ok, string detail)
    {
        sb.AppendLine($"[{(ok ? "OK" : "FALLO")}] {label}: {detail}");
    }

    public static string Execute()
    {
        var sb = new StringBuilder();

        EnemyDirector director = EnemyDirector.Instance;
        sb.AppendLine("--- Director antes ---");
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
            EnemyHealth hh = e.GetComponent<EnemyHealth>();
            NavMeshAgent aa = e.GetComponent<NavMeshAgent>();
            Collider cc = e.GetComponent<Collider>();
            sb.AppendLine($"  {e.name}: Type={e.Type} MaxHealth={hh.MaxHealth} Health={hh.Health} ShotsLeft={hh.ShotsLeft} IsDead={hh.IsDead} ai={e.enabled} agent={aa.enabled} collider={cc.GetType().Name}/{cc.enabled} pos={e.transform.position}");
        }

        sb.AppendLine("--- Muerte de cada Rastrero ---");
        foreach (EnemyAI e in enemies)
        {
            if (e.Type != EnemyType.Rastrero) continue;

            EnemyHealth h = e.GetComponent<EnemyHealth>();
            NavMeshAgent a = e.GetComponent<NavMeshAgent>();
            Collider col = e.GetComponent<Collider>();
            Renderer[] renderers = h.GetComponentsInChildren<Renderer>();
            Transform visual = renderers.Length > 0 ? renderers[0].transform : null;
            Vector3 aimPoint = h.transform.position + Vector3.up * 1f;

            sb.AppendLine($"### {e.name}");
            Check(sb, "perfil Rastrero 2 disparos", h.MaxHealth == 2 && h.ShotsLeft == 2, $"MaxHealth={h.MaxHealth} ShotsLeft={h.ShotsLeft}");
            Check(sb, "vivo antes del disparo", !h.IsDead && e.enabled && a.enabled && col.enabled, $"IsDead={h.IsDead} ai={e.enabled} agent={a.enabled} collider={col.enabled}");

            h.ApplyDamage(1, aimPoint);
            Check(sb, "tras 1 disparo sigue vivo", !h.IsDead && h.Health == 1 && h.ShotsLeft == 1, $"IsDead={h.IsDead} Health={h.Health} ShotsLeft={h.ShotsLeft} aiEnabled={e.enabled}");
            Check(sb, "tras 1 disparo conserva colision y agente", col.enabled && a.enabled, $"collider={col.enabled} agent={a.enabled}");

            h.ApplyDamage(1, aimPoint);
            Check(sb, "tras 2 disparos muere", h.IsDead && h.Health == 0, $"IsDead={h.IsDead} Health={h.Health}");
            Check(sb, "IA apagada", !e.enabled, $"aiEnabled={e.enabled}");
            Check(sb, "agente apagado", !a.enabled, $"agentEnabled={a.enabled}");
            Check(sb, "colision apagada", !col.enabled, $"colliderEnabled={col.enabled}");

            EnemyCorpse corpse = h.GetComponent<EnemyCorpse>();
            Check(sb, "cadaver creado", corpse != null, corpse != null ? $"EnemyCorpse presente; AllowRecycle={Field(corpse, "allowRecycle")}" : "sin EnemyCorpse");
            if (corpse != null)
            {
                Check(sb, "cadaver registrado", InRegistry(corpse), $"Registry={EnemyCorpse.All.Count}");
                Check(sb, "cuerpo apoyado en el suelo", Mathf.Abs(corpse.Position.y) < 0.15f, $"Pos.y={corpse.Position.y:F3} (esperado ~0)");
                Check(sb, "punto de remate sobre el cuerpo", corpse.AimPoint.y > 0.2f && corpse.AimPoint.y < 1.2f, $"AimPoint.y={corpse.AimPoint.y:F3}");
                Check(sb, "cuerpo se puede rematar", corpse.CanBeFinished, $"CanBeFinished={corpse.CanBeFinished}");
                if (visual != null)
                    sb.AppendLine($"  visual: localPos={visual.localPosition} localRot={visual.localRotation.eulerAngles} escala={visual.localScale} worldPos={visual.position}");
            }

            h.ApplyDamage(1, aimPoint);
            Check(sb, "damage extra ignorado estando muerto", h.IsDead && h.Health == 0, $"IsDead={h.IsDead} Health={h.Health}");
        }

        sb.AppendLine("--- Director despues ---");
        foreach (FieldInfo f in typeof(EnemyDirector).GetFields(Flags))
        {
            object v = f.GetValue(director);
            ICollection c = v as ICollection;
            sb.AppendLine($"  {f.Name} = {(c != null ? "Count=" + c.Count : (v != null ? v.ToString() : "null"))}");
        }

        return sb.ToString();
    }
}
