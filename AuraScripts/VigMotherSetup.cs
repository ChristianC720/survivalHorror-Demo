public class VigMotherSetup
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

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();

        Heartbeat.Level.RoomVolume vigilancia = Heartbeat.Level.RoomVolume.FindById("SalaVigilancia");
        Heartbeat.Level.RoomVolume secundaria = Heartbeat.Level.RoomVolume.FindById("SalaClaveSecundaria");

        UnityEngine.CharacterController controller = player.GetComponent<UnityEngine.CharacterController>();
        controller.enabled = false;
        player.transform.position = vigilancia.transform.position + new UnityEngine.Vector3(0f, 0.15f, 0f);
        controller.enabled = true;

        Heartbeat.AI.EnemyAI[] enemies = UnityEngine.Object.FindObjectsByType<Heartbeat.AI.EnemyAI>(
            UnityEngine.FindObjectsSortMode.None);

        Heartbeat.AI.EnemyAI alto = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].name == "Enemy_Alto_SalaClaveFinal") alto = enemies[i];
        }

        if (alto != null && secundaria != null)
        {
            // Andamio de prueba: se deja ciego y sordo para que no entre en Caza por su cuenta.
            Heartbeat.Config.EnemyProfile perfil = alto.Profile;
            perfil.visionRange = 0f;
            perfil.hearingRange = 0f;
            SetField(alto, "profile", perfil);

            SetField(alto, "state", Heartbeat.AI.EnemyState.Patrulla);
            SetField(alto, "alert01", 0f);
            SetField(alto, "loseSightTimer", 0f);
            SetField(alto, "crossDoorTimer", 0f);

            UnityEngine.AI.NavMeshAgent agente = alto.GetComponent<UnityEngine.AI.NavMeshAgent>();
            agente.Warp(secundaria.transform.position + new UnityEngine.Vector3(0f, 0.1f, 0f));

            sb.AppendLine("[Aura] Alto en SalaClaveSecundaria, Patrulla, visión/oído a 0 (andamio de prueba).");
        }

        Heartbeat.AI.EnemyDirector director = Heartbeat.AI.EnemyDirector.Instance;
        if (director != null) SetField(director, "motherAttention01", 0f);

        bool ok = view.TryActivate();
        sb.AppendLine("[Aura] Activar -> " + ok + " | activa=" + view.IsActive + " madre="
            + (director != null ? director.MotherAttention01.ToString("F3") : "-"));

        if (alto != null)
        {
            sb.AppendLine("[Aura] Alto: estado=" + alto.State + " sala=" + (alto.CurrentRoom != null ? alto.CurrentRoom.RoomId : "ninguna"));
        }

        return sb.ToString();
    }
}
