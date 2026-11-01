public class WireSurveillance
{
    public static string Execute()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        if (player == null) return "[Aura] No se encontró 'Player'.";

        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();
        if (view == null)
        {
            view = player.AddComponent<Heartbeat.Player.SurveillanceView>();
            sb.AppendLine("[Aura] SurveillanceView añadido al jugador.");
        }
        else
        {
            sb.AppendLine("[Aura] SurveillanceView ya existía en el jugador.");
        }

        Heartbeat.Config.GameConfig config = UnityEditor.AssetDatabase.LoadAssetAtPath<Heartbeat.Config.GameConfig>(
            "Assets/ScriptableObjects/GameConfig.asset");
        UnityEngine.InputSystem.InputActionAsset actions = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(
            "Assets/InputSystem_Actions.inputactions");

        UnityEngine.Transform cameraTransform = player.transform.Find("Camera");
        UnityEngine.Camera viewCamera = cameraTransform != null ? cameraTransform.GetComponent<UnityEngine.Camera>() : null;

        Heartbeat.Player.PlayerMotor motor = player.GetComponent<Heartbeat.Player.PlayerMotor>();

        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(view);
        so.FindProperty("config").objectReferenceValue = config;
        so.FindProperty("inputActions").objectReferenceValue = actions;
        so.FindProperty("motor").objectReferenceValue = motor;
        so.FindProperty("aim").objectReferenceValue = player.GetComponent<Heartbeat.Player.PlayerAim>();
        so.FindProperty("weapon").objectReferenceValue = player.GetComponent<Heartbeat.Player.PlayerWeapon>();
        so.FindProperty("health").objectReferenceValue = player.GetComponent<Heartbeat.Player.PlayerHealth>();
        so.FindProperty("viewCamera").objectReferenceValue = viewCamera;
        so.FindProperty("logEvents").boolValue = true;
        so.ApplyModifiedPropertiesWithoutUndo();

        if (motor != null)
        {
            UnityEditor.SerializedObject motorSo = new UnityEditor.SerializedObject(motor);
            motorSo.FindProperty("surveillance").objectReferenceValue = view;
            motorSo.ApplyModifiedPropertiesWithoutUndo();
        }

        sb.AppendLine("[Aura] Referencias: config=" + (config != null) + " actions=" + (actions != null)
            + " camara=" + (viewCamera != null) + " motor=" + (motor != null));

        string[] rooms = new string[] { "SalaVigilancia", "SalaClaveSecundaria", "SalaClaveFinal" };

        for (int i = 0; i < rooms.Length; i++)
        {
            string id = rooms[i];
            UnityEngine.GameObject roomGo = UnityEngine.GameObject.Find("VS_Piso1/Triggers/" + id);
            if (roomGo == null)
            {
                sb.AppendLine("[Aura] FALTA la sala " + id);
                continue;
            }

            Heartbeat.Level.RoomVolume room = roomGo.GetComponent<Heartbeat.Level.RoomVolume>();
            if (room == null)
            {
                sb.AppendLine("[Aura] " + id + " no tiene RoomVolume.");
                continue;
            }

            UnityEngine.GameObject propGo = UnityEngine.GameObject.Find("VS_Piso1/Props/CamaraVigilancia_" + id);
            UnityEngine.Transform prop = propGo != null ? propGo.transform : null;

            UnityEditor.SerializedObject roomSo = new UnityEditor.SerializedObject(room);
            roomSo.FindProperty("hasSecurityCamera").boolValue = true;
            roomSo.FindProperty("securityCameraProp").objectReferenceValue = prop;
            roomSo.ApplyModifiedPropertiesWithoutUndo();

            sb.AppendLine("[Aura] Sala " + id + ": camara=" + true + " prop=" + (prop != null));
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        sb.AppendLine("[Aura] Escena guardada.");

        return sb.ToString();
    }
}
