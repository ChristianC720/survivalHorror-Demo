public class VigMoveEnemy
{
    public static string Execute()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Heartbeat.Level.RoomVolume destino = Heartbeat.Level.RoomVolume.FindById("SalaClaveSecundaria");
        if (destino == null) return "[Aura] No existe SalaClaveSecundaria.";

        Heartbeat.AI.EnemyAI[] enemies = UnityEngine.Object.FindObjectsByType<Heartbeat.AI.EnemyAI>(
            UnityEngine.FindObjectsSortMode.None);

        Heartbeat.AI.EnemyAI elegido = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].name == "Enemy_Alto_SalaClaveFinal") elegido = enemies[i];
        }

        if (elegido == null) return "[Aura] No se encontró Enemy_Alto_SalaClaveFinal.";

        UnityEngine.Vector3 punto = destino.transform.position + new UnityEngine.Vector3(0f, 0.1f, 0f);
        UnityEngine.AI.NavMeshAgent agente = elegido.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agente.Warp(punto);

        // El medidor se pone a cero para observar un llenado limpio de 5 s.
        Heartbeat.AI.EnemyDirector director = Heartbeat.AI.EnemyDirector.Instance;
        if (director != null)
        {
            System.Reflection.FieldInfo f = typeof(Heartbeat.AI.EnemyDirector).GetField(
                "motherAttention01", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (f != null) f.SetValue(director, 0f);
        }

        sb.AppendLine("[Aura] " + elegido.name + " movido a " + punto + " (SalaClaveSecundaria), medidor a 0.");

        return sb.ToString();
    }
}
