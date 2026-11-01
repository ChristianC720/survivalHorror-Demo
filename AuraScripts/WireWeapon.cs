using Heartbeat.Config;
using Heartbeat.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public static class WireWeapon
{
    public static void Execute()
    {
        const string prefabPath = "Assets/Prefabs/Player/Player.prefab";

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            PlayerWeapon weapon = root.GetComponent<PlayerWeapon>();
            if (weapon == null) weapon = root.AddComponent<PlayerWeapon>();

            Transform cameraTransform = root.transform.Find("Camera");

            SerializedObject so = new SerializedObject(weapon);
            so.FindProperty("config").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/ScriptableObjects/GameConfig.asset");
            so.FindProperty("inputActions").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            so.FindProperty("viewCamera").objectReferenceValue =
                cameraTransform != null ? cameraTransform.GetComponent<Camera>() : null;
            so.FindProperty("cameraRig").objectReferenceValue =
                cameraTransform != null ? cameraTransform.GetComponent<ThirdPersonCamera>() : null;
            so.FindProperty("aim").objectReferenceValue = root.GetComponent<PlayerAim>();
            so.FindProperty("health").objectReferenceValue = root.GetComponent<PlayerHealth>();
            so.FindProperty("inventory").objectReferenceValue = root.GetComponent<PlayerInventory>();
            so.FindProperty("stamina").objectReferenceValue = root.GetComponent<PlayerStamina>();
            so.FindProperty("noiseEmitter").objectReferenceValue = root.GetComponent<PlayerNoiseEmitter>();
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[Revólver] PlayerWeapon cableado en el prefab del jugador.");
    }
}
