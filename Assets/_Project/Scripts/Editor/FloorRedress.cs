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

    /// <summary>F1 only had one layout per archetype. Its B rooms re-dress the hand-built F2 B layouts as maintenance rooms.</summary>
    static readonly (string prefix, string prop)[] ToF1 =
    {
        ("ResearchBed", "PROP_F1_ToolShelf"), ("Bed", "PROP_F1_Workbench"), ("IVCart", "PROP_Shared_MetalCrate_Half"), ("PatientScreen", "PROP_F1_ThickPipe"),
        ("SurgicalLamp", null), ("SurgicalTools", "PROP_F1_Tools"), ("SurgeryTable", "PROP_F1_Workbench"), ("SampleFridge", "PH_F1_WaterPump"),
        ("FilingCabinet", "PROP_F1_ToolShelf"), ("CabinetAisle", "PROP_Shared_MetalCrate"), ("CabinetN", "PROP_Shared_MetalCrate_Half"), ("Fridge", "PROP_F1_ToolShelf"),
        ("NurseConsole", "PH_F1_Generator"), ("Console", "PH_F1_Generator"), ("NurseDesk", "PROP_F1_Workbench"), ("Desk", "PROP_F1_Workbench"),
        ("NurseChair", "PROP_Shared_MetalCrate_Half"), ("Chair", "PROP_Shared_MetalCrate_Half"), ("Monitor", "PROP_Shared_MetalCrate_Half"),
    };

    [MenuItem("Heartbeat/Rooms/Re-dress F2 B Layouts as F1")]
    public static void MenuF1() => Debug.Log(ExecuteF1());

    public static string ExecuteF1()
    {
        var log = new System.Text.StringBuilder();
        var kit = AssetDatabase.LoadAssetAtPath<Heartbeat.Generation.KitSet>($"{Defs}/KitSet_F1.asset");
        var sodium = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/Light_F1_Sodium.prefab").GetComponentInChildren<Light>(true).color;
        foreach (var layout in new[] { "Corridor_B", "Hack_B", "Open_B", "Storage_B", "Surveillance_B" })
        {
            string newPath = $"{Rooms}/F1/ROOM_F1_{layout}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(newPath) != null) continue;   // never overwrite
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>($"{Rooms}/F2/ROOM_F2_{layout}.prefab"));
            inst.name = $"ROOM_F1_{layout}";
            PrefabUtility.SaveAsPrefabAsset(inst, newPath);
            Object.DestroyImmediate(inst);

            var root = PrefabUtility.LoadPrefabContents(newPath);
            root.GetComponent<Heartbeat.Generation.RoomShell>().kit = kit;
            foreach (var l in root.GetComponentsInChildren<Light>(true)) l.color = sodium;
            var props = root.transform.Find("Props");
            int swapped = 0;
            foreach (var t in props.Cast<Transform>().ToList())
            {
                var (prefix, prop) = ToF1.FirstOrDefault(m => t.name.StartsWith(m.prefix));
                if (prefix == null) { log.AppendLine($"{root.name}: no match for {t.name}"); continue; }
                if (prop != null)
                {
                    var src = AssetDatabase.LoadAssetAtPath<GameObject>($"{Props}/F1/{prop}.prefab") ?? AssetDatabase.LoadAssetAtPath<GameObject>($"{Props}/Shared/{prop}.prefab");
                    var p = (GameObject)PrefabUtility.InstantiatePrefab(src, props);
                    p.name = t.name;
                    p.transform.localPosition = new Vector3(t.localPosition.x, 0f, t.localPosition.z);
                    p.transform.localRotation = t.localRotation;
                }
                Object.DestroyImmediate(t.gameObject);
                swapped++;
            }
            PrefabUtility.SaveAsPrefabAsset(root, newPath);
            PrefabUtility.UnloadPrefabContents(root);
            log.AppendLine($"ROOM_F1_{layout}: {swapped} props swapped");
        }
        AssetDatabase.SaveAssets();
        return log.ToString();
    }

    /// <summary>F2 and F3 start in the room the elevator opens on (D-158): the capsule room's layout without its capsules and tutorial.</summary>
    public static string ExecuteArrival()
    {
        var log = new System.Text.StringBuilder();
        for (int fi = 0; fi < 2; fi++)
        {
            string F = "F" + (fi + 2);
            string newPath = $"{Rooms}/{F}/KEY_{F}_Arrival.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(newPath) != null) continue;   // never overwrite
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>($"{Rooms}/F1/KEY_F1_CapsuleRow.prefab"));
            inst.name = $"KEY_{F}_Arrival";
            PrefabUtility.SaveAsPrefabAsset(inst, newPath);
            Object.DestroyImmediate(inst);

            var root = PrefabUtility.LoadPrefabContents(newPath);
            root.GetComponent<Heartbeat.Generation.RoomShell>().kit = AssetDatabase.LoadAssetAtPath<Heartbeat.Generation.KitSet>($"{Defs}/KitSet_{F}.asset");
            foreach (var l in root.GetComponentsInChildren<Light>(true)) l.color = Tint[fi];
            foreach (var t in root.transform.Cast<Transform>().Where(t => t.name.StartsWith("Capsule_") || t.name.StartsWith("Poster_") || t.name.Contains("Tray")).ToList())
                Object.DestroyImmediate(t.gameObject);
            var spawn = root.transform.Find("PlayerSpawn");
            spawn.localPosition = new Vector3(4f, spawn.localPosition.y, 2f);
            spawn.localRotation = Quaternion.identity;   // back to the elevator, facing the room
            var lift = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>($"{Props}/{F}/CH_Elevator_{F}.prefab"), root.transform);
            lift.name = "Elevator";
            lift.transform.localPosition = new Vector3(4f, 0f, .3f);   // on the south wall's face
            PrefabUtility.SaveAsPrefabAsset(root, newPath);
            PrefabUtility.UnloadPrefabContents(root);
            log.AppendLine($"KEY_{F}_Arrival created");
        }
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
        k.door = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Level/Door_{F}.prefab");
        EditorUtility.SetDirty(k);
        return k;
    }
}
}
