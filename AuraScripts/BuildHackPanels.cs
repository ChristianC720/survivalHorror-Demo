using System;
using System.Text;
using Heartbeat.Config;
using Heartbeat.Level;
using Heartbeat.Player;
using Heartbeat.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Añade hojas de puerta, paneles de hackeo y el cableado del hackeo a la escena VS_Piso1.
/// No regenera el graybox: sólo añade objetos y componentes sobre la escena existente.
/// </summary>
public static class BuildHackPanels
{
    private const string ConfigPath = "Assets/ScriptableObjects/GameConfig.asset";

    private sealed class DoorSpec
    {
        public string Name;
        public Vector3 HingePosition;
        public Vector3 LeafLocalPosition;
        public Vector3 LeafScale;
        public bool StartsClosed;
        public bool StartsLocked;
    }

    public static string Execute()
    {
        var report = new StringBuilder();

        GameObject root = GameObject.Find("VS_Piso1");
        if (root == null) return "ERROR: no existe VS_Piso1 en la escena activa.";

        Transform doorsRoot = root.transform.Find("Doors");
        if (doorsRoot == null) return "ERROR: no existe VS_Piso1/Doors.";

        Transform panelsRoot = root.transform.Find("Panels");
        if (panelsRoot == null)
        {
            var panelsGo = new GameObject("Panels");
            panelsGo.transform.SetParent(root.transform, false);
            panelsRoot = panelsGo.transform;
        }

        GameConfig config = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
        if (config == null) return "ERROR: no se encuentra GameConfig.asset.";

        // --- 1) Puerta existente de SalaClaveFinal: Door -> SealableDoor (cerrada con llave) ---
        Transform finalDoor = doorsRoot.Find("Puerta_SalaClaveFinal");
        if (finalDoor == null) return "ERROR: no existe Puerta_SalaClaveFinal.";

        Door legacy = finalDoor.GetComponent<Door>();
        if (legacy != null) UnityEngine.Object.DestroyImmediate(legacy);

        SealableDoor finalSeal = finalDoor.GetComponent<SealableDoor>();
        if (finalSeal == null) finalSeal = finalDoor.gameObject.AddComponent<SealableDoor>();
        Configure(finalSeal, config, finalDoor.Find("Hoja"), true, true);
        report.AppendLine($"Puerta_SalaClaveFinal -> SealableDoor (cerrada, con llave)");

        // --- 2) Hojas nuevas ---
        SealableDoor west = CreateDoor(doorsRoot, new DoorSpec
        {
            Name = "Puerta_SalaClaveSecundaria_Oeste",
            HingePosition = new Vector3(48f, 0f, -1f),
            LeafLocalPosition = new Vector3(0f, 1.05f, 1f),
            LeafScale = new Vector3(0.15f, 2.1f, 2f),
            StartsClosed = false,
            StartsLocked = false,
        }, config);
        report.AppendLine("Puerta_SalaClaveSecundaria_Oeste (abierta)");

        SealableDoor vigilancia = CreateDoor(doorsRoot, new DoorSpec
        {
            Name = "Puerta_SalaVigilancia",
            HingePosition = new Vector3(52f, 0f, 5f),
            LeafLocalPosition = new Vector3(1f, 1.05f, 0f),
            LeafScale = new Vector3(2f, 2.1f, 0.15f),
            StartsClosed = false,
            StartsLocked = false,
        }, config);
        report.AppendLine("Puerta_SalaVigilancia (abierta)");

        SealableDoor almacen = CreateDoor(doorsRoot, new DoorSpec
        {
            Name = "Puerta_Almacen",
            HingePosition = new Vector3(16f, 0f, 1.5f),
            LeafLocalPosition = new Vector3(1f, 1.05f, 0f),
            LeafScale = new Vector3(2f, 2.1f, 0.15f),
            StartsClosed = true,
            StartsLocked = false,
        }, config);
        report.AppendLine("Puerta_Almacen (cerrada, se abre hackeando)");

        // --- 3) Paneles ---
        CreatePanel(panelsRoot, "Panel_SalaClaveSecundaria", new Vector3(48.21f, 1.45f, -1.6f),
            new Vector3(0.12f, 0.42f, 0.28f), HackPanelKind.Sellado, west, config);
        report.AppendLine("Panel_SalaClaveSecundaria (sellado) @ Puerta_SalaClaveSecundaria_Oeste");

        CreatePanel(panelsRoot, "Panel_SalaClaveFinal", new Vector3(57.79f, 1.45f, -1.6f),
            new Vector3(0.12f, 0.42f, 0.28f), HackPanelKind.Sellado, finalSeal, config);
        report.AppendLine("Panel_SalaClaveFinal (sellado) @ Puerta_SalaClaveFinal");

        CreatePanel(panelsRoot, "Panel_SalaVigilancia", new Vector3(51.4f, 1.45f, 4.79f),
            new Vector3(0.28f, 0.42f, 0.12f), HackPanelKind.Sellado, vigilancia, config);
        report.AppendLine("Panel_SalaVigilancia (sellado) @ Puerta_SalaVigilancia");

        CreatePanel(panelsRoot, "Panel_Almacen", new Vector3(15.4f, 1.45f, 1.29f),
            new Vector3(0.28f, 0.42f, 0.12f), HackPanelKind.PuertaEstandar, almacen, config);
        report.AppendLine("Panel_Almacen (puerta estándar) @ Puerta_Almacen, lado Pasillo");

        // --- 4) PlayerHack ---
        GameObject player = GameObject.Find("Player");
        if (player == null) return "ERROR: no existe Player.";

        PlayerHack hack = player.GetComponent<PlayerHack>();
        if (hack == null) hack = player.AddComponent<PlayerHack>();

        Transform cameraTransform = player.transform.Find("Camera");
        Camera viewCamera = cameraTransform != null ? cameraTransform.GetComponent<Camera>() : null;

        SetObject(hack, "config", config);
        SetObject(hack, "viewCamera", viewCamera);
        SetObject(hack, "playerRoot", player.transform);
        SetObject(hack, "motor", player.GetComponent<PlayerMotor>());
        SetObject(hack, "health", player.GetComponent<PlayerHealth>());
        report.AppendLine($"PlayerHack cableado (cámara={(viewCamera != null ? "ok" : "NULL")})");

        SetObject(player.GetComponent<PlayerPickup>(), "hack", hack);

        // --- 5) HUD ---
        GameObject hud = GameObject.Find("HUD");
        if (hud == null) return "ERROR: no existe HUD.";

        HackUI hackUI = hud.GetComponent<HackUI>();
        if (hackUI == null) hackUI = hud.AddComponent<HackUI>();
        SetObject(hackUI, "hack", hack);

        SetObject(hud.GetComponent<DebugInventoryUI>(), "hack", hack);
        report.AppendLine("HackUI en HUD y DebugInventoryUI.hack cableados");

        // --- 6) Guardar ---
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        report.AppendLine($"escena guardada={SceneManager.GetActiveScene().isDirty == false}");

        return report.ToString();
    }

    private static SealableDoor CreateDoor(Transform parent, DoorSpec spec, GameConfig config)
    {
        Transform existing = parent.Find(spec.Name);
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

        var hinge = new GameObject(spec.Name);
        hinge.transform.SetParent(parent, false);
        hinge.transform.position = spec.HingePosition;
        hinge.transform.localRotation = Quaternion.identity;

        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leaf.name = "Hoja";
        leaf.transform.SetParent(hinge.transform, false);
        leaf.transform.localPosition = spec.LeafLocalPosition;
        leaf.transform.localRotation = Quaternion.identity;
        leaf.transform.localScale = spec.LeafScale;

        SealableDoor door = hinge.AddComponent<SealableDoor>();
        Configure(door, config, leaf.transform, spec.StartsClosed, spec.StartsLocked);
        return door;
    }

    private static void Configure(SealableDoor door, GameConfig config, Transform leaf, bool startsClosed, bool startsLocked)
    {
        SetObject(door, "config", config);
        SetObject(door, "leaf", leaf);
        SetBool(door, "startsClosed", startsClosed);
        SetBool(door, "startsLocked", startsLocked);
    }

    private static HackPanel CreatePanel(Transform parent, string name, Vector3 position, Vector3 size,
        HackPanelKind kind, SealableDoor door, GameConfig config)
    {
        Transform existing = parent.Find(name);
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = name;
        panel.transform.SetParent(parent, false);
        panel.transform.position = position;
        panel.transform.localRotation = Quaternion.identity;
        panel.transform.localScale = size;

        HackPanel component = panel.AddComponent<HackPanel>();
        SetObject(component, "config", config);
        SetObject(component, "door", door);
        SetEnum(component, "kind", (int)kind);

        return component;
    }

    private static void SetObject(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
    {
        if (target == null) throw new Exception($"SetObject: target nulo para '{fieldName}'.");

        var so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty(fieldName);
        if (property == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");

        property.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    private static void SetBool(UnityEngine.Object target, string fieldName, bool value)
    {
        var so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty(fieldName);
        if (property == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");

        property.boolValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    private static void SetEnum(UnityEngine.Object target, string fieldName, int value)
    {
        var so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty(fieldName);
        if (property == null) throw new Exception($"Campo '{fieldName}' no encontrado en {target.GetType().Name}.");

        property.enumValueIndex = value;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }
}
