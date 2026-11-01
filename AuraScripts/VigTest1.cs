public class VigTest1
{
    public static string Execute()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Heartbeat.Config.GameConfig cfg = UnityEditor.AssetDatabase.LoadAssetAtPath<Heartbeat.Config.GameConfig>(
            "Assets/ScriptableObjects/GameConfig.asset");

        if (cfg == null) return "[Aura] No se encontró GameConfig.asset.";

        sb.AppendLine("[Aura] Config vigilancia: FOV=" + cfg.SurveillanceFov + " pitch=" + cfg.SurveillancePitch
            + " yaw=" + cfg.SurveillanceYaw + " framing=" + cfg.SurveillanceFraming + " padding=" + cfg.SurveillancePadding);
        sb.AppendLine("[Aura] Feed: escala=" + cfg.SurveillanceFeedScale + " fps=" + cfg.SurveillanceFeedFps
            + " delay=" + cfg.SurveillanceDelay + " delayBusqueda=" + cfg.SurveillanceDelaySearch
            + " ruido=" + cfg.SurveillanceNoise + " ruidoBusqueda=" + cfg.SurveillanceNoiseSearch);
        sb.AppendLine("[Aura] Jugador: velMult=" + cfg.SurveillanceSpeedMultiplier
            + " madreLlenado=" + cfg.MotherAttentionFillDuration + " madreVaciado=" + cfg.MotherAttentionDrainDuration);

        string[] ids = new string[] { "SalaVigilancia", "SalaClaveSecundaria", "SalaClaveFinal", "Almacen", "Pasillo" };

        for (int i = 0; i < ids.Length; i++)
        {
            Heartbeat.Level.RoomVolume room = Heartbeat.Level.RoomVolume.FindById(ids[i]);
            if (room == null)
            {
                sb.AppendLine("[Aura] " + ids[i] + ": no existe.");
                continue;
            }

            System.Text.StringBuilder adj = new System.Text.StringBuilder();
            for (int a = 0; a < room.AdjacentRooms.Count; a++)
            {
                if (room.AdjacentRooms[a] == null) continue;
                if (adj.Length > 0) adj.Append(", ");
                adj.Append(room.AdjacentRooms[a].RoomId);
            }

            sb.AppendLine("[Aura] " + ids[i] + ": camara=" + room.HasSecurityCamera
                + " prop=" + (room.SecurityCameraProp != null ? room.SecurityCameraProp.name : "ninguna")
                + " adyacentes=[" + adj + "]");
        }

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        if (player == null) return sb.ToString() + "[Aura] No hay Player.";

        Heartbeat.Level.RoomVolume playerRoom = Heartbeat.Level.RoomVolume.FindContaining(player.transform.position);
        sb.AppendLine("[Aura] Player en " + (playerRoom != null ? playerRoom.RoomId : "ninguna sala"));

        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();
        if (view == null) return sb.ToString() + "[Aura] El Player no tiene SurveillanceView.";

        sb.AppendLine("[Aura] Vista: activa=" + view.IsActive + " bloqueada=" + view.IsBlocked
            + " señal=" + view.Signal + " ruido=" + view.NoiseAmount + " retardo=" + view.FeedDelay);

        Heartbeat.AI.EnemyAI[] enemies = UnityEngine.Object.FindObjectsByType<Heartbeat.AI.EnemyAI>(
            UnityEngine.FindObjectsSortMode.None);

        for (int i = 0; i < enemies.Length; i++)
        {
            Heartbeat.AI.EnemyAI e = enemies[i];
            sb.AppendLine("  enemigo " + e.name + " estado=" + e.State
                + " sala=" + (e.CurrentRoom != null ? e.CurrentRoom.RoomId : "ninguna")
                + " aturdido=" + e.IsStunned);
        }

        return sb.ToString();
    }
}
