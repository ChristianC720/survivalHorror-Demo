public class VigReset
{
    private static void SetField(object target, string name, object value)
    {
        System.Reflection.FieldInfo f = target.GetType().GetField(name,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (f != null) f.SetValue(target, value);
    }

    public static string Execute()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Heartbeat.AI.EnemyAI[] enemies = UnityEngine.Object.FindObjectsByType<Heartbeat.AI.EnemyAI>(
            UnityEngine.FindObjectsSortMode.None);

        Heartbeat.AI.EnemyAI alto = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].name == "Enemy_Alto_SalaClaveFinal") alto = enemies[i];
        }

        Heartbeat.Level.RoomVolume secundaria = Heartbeat.Level.RoomVolume.FindById("SalaClaveSecundaria");
        Heartbeat.Level.RoomVolume vigilancia = Heartbeat.Level.RoomVolume.FindById("SalaVigilancia");

        if (alto != null && secundaria != null)
        {
            SetField(alto, "state", Heartbeat.AI.EnemyState.Patrulla);
            SetField(alto, "alert01", 0f);
            SetField(alto, "loseSightTimer", 0f);
            SetField(alto, "crossDoorTimer", 0f);
            SetField(alto, "hasDestination", false);

            UnityEngine.AI.NavMeshAgent agente = alto.GetComponent<UnityEngine.AI.NavMeshAgent>();
            agente.Warp(secundaria.transform.position + new UnityEngine.Vector3(0f, 0.1f, 0f));

            sb.AppendLine("[Aura] Alto devuelto a SalaClaveSecundaria y forzado a Patrulla.");
        }

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        UnityEngine.CharacterController controller = player.GetComponent<UnityEngine.CharacterController>();

        UnityEngine.Vector3 escondite = vigilancia.transform.position + new UnityEngine.Vector3(-4.2f, 0.15f, 4.2f);
        controller.enabled = false;
        player.transform.position = escondite;
        controller.enabled = true;

        sb.AppendLine("[Aura] Player en " + escondite + " (esquina de SalaVigilancia).");

        Heartbeat.AI.EnemyDirector director = Heartbeat.AI.EnemyDirector.Instance;
        if (director != null) SetField(director, "motherAttention01", 0f);

        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();
        sb.AppendLine("[Aura] Vista: activa=" + view.IsActive + " bloqueada=" + view.IsBlocked + " señal=" + view.Signal);

        return sb.ToString();
    }
}
