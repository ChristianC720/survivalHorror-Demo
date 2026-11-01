public class VigActivate
{
    public static string Execute()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();
        Heartbeat.Level.RoomVolume room = Heartbeat.Level.RoomVolume.FindById("SalaVigilancia");

        UnityEngine.CharacterController controller = player.GetComponent<UnityEngine.CharacterController>();
        UnityEngine.Vector3 destino = room.transform.position + new UnityEngine.Vector3(0f, 0.15f, 0f);

        controller.enabled = false;
        player.transform.position = destino;
        controller.enabled = true;

        sb.AppendLine("[Aura] Player movido a " + destino + " (SalaVigilancia).");

        Heartbeat.Level.RoomVolume ahora = Heartbeat.Level.RoomVolume.FindContaining(player.transform.position);
        sb.AppendLine("[Aura] Sala detectada: " + (ahora != null ? ahora.RoomId : "ninguna")
            + " camara=" + (ahora != null && ahora.HasSecurityCamera));

        bool ok = view.TryActivate();
        sb.AppendLine("[Aura] TryActivate -> " + ok + " | activa=" + view.IsActive + " bloqueada=" + view.IsBlocked
            + " señal=" + view.Signal + " ruido=" + view.NoiseAmount + " retardo=" + view.FeedDelay);

        return sb.ToString();
    }
}
