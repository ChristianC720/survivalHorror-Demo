using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
/// <summary>
/// Re-dresses the hand-built F1 room layouts as F2/F3 prefab variants (floor kit set, light tint, floor props replacing
/// the F1-specific ones at the same footprint) and adds their RoomDefinitions to the library. Only creates variants that
/// do not exist yet — existing ones (and anything placed in them with Aura, e.g. the extra F3 enemy spawns) are kept.
/// Run Heartbeat/Rooms/Refresh Definitions afterwards. Menu: Heartbeat/Rooms/Re-dress Missing F2-F3 Variants.
/// </summary>
public static class FloorRedress
{
    [MenuItem("Heartbeat/Rooms/Re-dress Missing F2-F3 Variants")]
    public static void Menu() => Debug.Log(Execute());

    const string Rooms = "Assets/_Project/Prefabs/Rooms";
    const string Props = "Assets/_Project/Prefabs/Props";
    const string Defs = "Assets/_Project/ScriptableObjects/Rooms";

    static readonly Dictionary<string, string[]> Swap = new Dictionary<string, string[]>
    {   // F1 prop name prefix -> { F2 prop, F3 prop }
        ["Boiler"] = new[] { "PH_F2_HumanCapsule", "PH_F3_CryoTank" },
        ["PumpUnit"] = new[] { "PROP_F2_SampleFridge", "PH_F3_Transformer" },
        ["ElectricalPanel"] = new[] { "PROP_F2_Console", "PROP_F3_ContainmentConsole" },
        ["GuardDesk"] = new[] { "PROP_F2_Desk", "PROP_F3_ContainmentConsole" },
        ["Desk"] = new[] { "PROP_F2_Desk", "PROP_F3_ContainmentConsole" },
        ["Pipe"] = new[] { "PROP_F2_Screen", "PROP_F3_PAHorn" },
        ["Shelf"] = new[] { "PROP_F2_FilingCabinet", "PH_F3_BrokenCapsule" },
        ["Workbench"] = new[] { "PROP_F2_SurgeryTable", "PROP_F3_ContainmentConsole" },
    };
    static readonly Color[] Tint = { new Color(0.78f, 0.9f, 1f), new Color(0.85f, 0.12f, 0.18f) };
    static readonly float[] Intensity = { 14f, 22f };

    public static string Execute()
    {
        var log = new System.Text.StringBuilder();
        var library = AssetDatabase.LoadAssetAtPath<Heartbeat.Generation.RoomLibrary>($"{Defs}/RoomLibrary.asset");
        for (int fi = 0; fi < 2; fi++)
        {
            int floor = fi + 2;
            string F = "F" + floor;
            var kit = MakeKitSet(F);
            foreach (var path in AssetDatabase.FindAssets("t:Prefab", new[] { $"{Rooms}/F1" }).Select(AssetDatabase.GUIDToAssetPath))
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (name.Contains("CapsuleRow")) continue;   // the capsule row only exists on floor 1
                string newName = name.Replace("_F1_", $"_{F}_");
                string newPath = $"{Rooms}/{F}/{newName}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(newPath) != null) continue;   // never overwrite

                // Variant of the F1 layout.
                var inst = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                inst.name = newName;
                PrefabUtility.SaveAsPrefabAsset(inst, newPath);
                Object.DestroyImmediate(inst);

                var root = PrefabUtility.LoadPrefabContents(newPath);
                var shell = root.GetComponent<Heartbeat.Generation.RoomShell>();
                shell.kit = kit;
                foreach (var l in root.GetComponentsInChildren<Light>(true)) { l.color = Tint[fi]; l.intensity = Intensity[fi]; }
                int swapped = 0;
                foreach (var t in root.GetComponentsInChildren<Transform>(true).Where(t => t.parent == root.transform || (t.parent != null && t.parent.name == "Props")).ToList())
                {
                    var key = Swap.Keys.FirstOrDefault(k => t.name.StartsWith(k));
                    if (key == null) continue;
                    var rends = t.GetComponentsInChildren<Renderer>();
                    if (rends.Length == 0) continue;
                    var b = rends[0].bounds; foreach (var r in rends) b.Encapsulate(r.bounds);
                    Vector3 foot = root.transform.InverseTransformPoint(new Vector3(b.center.x, b.min.y, b.center.z));
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Props}/{F}/{Swap[key][fi]}.prefab");
                    var p = (GameObject)PrefabUtility.InstantiatePrefab(prefab, t.parent);
                    p.name = t.name.Replace(key, Swap[key][fi].Split('_').Last());
                    p.transform.localPosition = foot;
                    p.transform.localRotation = t.localRotation;
                    Object.DestroyImmediate(t.gameObject);   // removed from this variant only
                    swapped++;
                }
                PrefabUtility.SaveAsPrefabAsset(root, newPath);
                PrefabUtility.UnloadPrefabContents(root);

                // RoomDefinition: copy of the F1 one (same sockets/spawns), pointing at the variant.
                var srcDef = AssetDatabase.LoadAssetAtPath<Heartbeat.Generation.RoomDefinition>($"{Defs}/RD_{name}.asset");
                string defPath = $"{Defs}/RD_{newName}.asset";
                var def = AssetDatabase.LoadAssetAtPath<Heartbeat.Generation.RoomDefinition>(defPath);
                if (def == null) { def = Object.Instantiate(srcDef); AssetDatabase.CreateAsset(def, defPath); }
                else EditorUtility.CopySerialized(srcDef, def);
                def.name = "RD_" + newName;
                def.id = newName;
                def.floor = floor;
                def.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);
                if (floor == 3) def.canBeUnpowered = false;   // only floors 1-2 (D-72)
                EditorUtility.SetDirty(def);
                if (!library.rooms.Contains(def)) library.rooms.Add(def);
                log.AppendLine($"{newName}: {swapped} props swapped");
            }
        }
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        return log.ToString();
    }

    static Heartbeat.Generation.KitSet MakeKitSet(string F)
    {
        string path = $"{Defs}/KitSet_{F}.asset";
        var k = AssetDatabase.LoadAssetAtPath<Heartbeat.Generation.KitSet>(path);
        if (k == null) { k = ScriptableObject.CreateInstance<Heartbeat.Generation.KitSet>(); AssetDatabase.CreateAsset(k, path); }
        GameObject G(string n) => AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Kit/{F}/KIT_{F}_{n}.prefab");
        k.floor2x2 = G("Floor_2x2");
        k.ceiling2x2 = G("Ceiling_2x2");
        k.wall2m = G("Wall_2m");
        k.wall2mHigh = G("Wall_2m_High");
        k.doorway2m = G("Doorway_2m");
        k.wallWindow2m = G("WallWindow_2m");
        k.door = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/Door_2m.prefab");
        EditorUtility.SetDirty(k);
        return k;
    }
}
}
