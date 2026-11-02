using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Heartbeat.EditorTools
{
/// <summary>
/// Builds the primitive props of OBJECT_CATALOG.md (F2/F3) that do not exist yet: pivot at the floor centre, +Z front,
/// shared palette materials, then the official PropTools finish (root collider, layer, static). Never overwrites an
/// existing prefab, so hand edits made with Aura are kept. Menu: Heartbeat/Props/Build Missing Catalog Props.
/// </summary>
public static class PropBuilder
{
    [MenuItem("Heartbeat/Props/Build Missing Catalog Props")]
    public static void Menu() => Debug.Log(Execute());

    const string Shared = "Assets/_Project/Materials/Shared";
    static readonly string[] NoCollider = { "PROP_F3_HeavyCabling" };
    struct P { public string n; public PrimitiveType t; public Vector3 pos, size; public string mat; }
    static P C(string n, float x, float y, float z, float sx, float sy, float sz, string m) => new P { n = n, t = PrimitiveType.Cube, pos = new Vector3(x, y, z), size = new Vector3(sx, sy, sz), mat = m };
    static P Cy(string n, float x, float y, float z, float d, float h, string m) => new P { n = n, t = PrimitiveType.Cylinder, pos = new Vector3(x, y, z), size = new Vector3(d, h * 0.5f, d), mat = m };

    public static string Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/Sandbox_Kit.unity", OpenSceneMode.Single);
        var specs = new Dictionary<string, (string floor, P[] parts, string phNotes)>
        {
            // ---- F2 illegal clinic
            ["PROP_F2_Desk"] = ("F2", new[] { C("Top", 0, 0.76f, 0, 1.6f, 0.06f, 0.8f, "MAT_Base_Light"), C("Body", 0.5f, 0.38f, 0, 0.55f, 0.74f, 0.75f, "MAT_Base_Mid"), C("LegL", -0.75f, 0.37f, 0, 0.05f, 0.74f, 0.7f, "MAT_Base_Mid") }, null),
            ["PROP_F2_Console"] = ("F2", new[] { C("Body", 0, 0.5f, 0, 1.0f, 1.0f, 0.6f, "MAT_Base_Mid"), C("Screen", 0, 1.05f, -0.05f, 0.8f, 0.3f, 0.06f, "MAT_Phosphor") }, null),
            ["PROP_F2_ResearchBed"] = ("F2", new[] { C("Mattress", 0, 0.6f, 0, 0.9f, 0.12f, 1.95f, "MAT_WhiteDirty"), C("Frame", 0, 0.3f, 0, 0.95f, 0.5f, 2.0f, "MAT_Base_Metal"), C("Strap", 0, 0.68f, 0.2f, 1.0f, 0.03f, 0.08f, "MAT_Base_Dark") }, null),
            ["PROP_F2_Screen"] = ("F2", new[] { C("Panel", 0, 0.95f, 0, 1.8f, 1.6f, 0.04f, "MAT_WhiteDirty"), C("FootL", -0.8f, 0.05f, 0, 0.06f, 0.1f, 0.5f, "MAT_Base_Metal"), C("FootR", 0.8f, 0.05f, 0, 0.06f, 0.1f, 0.5f, "MAT_Base_Metal") }, null),
            ["PROP_F2_IVCart"] = ("F2", new[] { Cy("Pole", 0, 0.9f, 0, 0.04f, 1.8f, "MAT_Base_Metal"), Cy("Base", 0, 0.05f, 0, 0.5f, 0.1f, "MAT_Base_Dark"), C("Bag", 0.1f, 1.6f, 0, 0.15f, 0.25f, 0.05f, "MAT_Glass") }, null),
            ["PROP_F2_FilingCabinet"] = ("F2", new[] { C("Body", 0, 0.7f, 0, 0.5f, 1.4f, 0.6f, "MAT_Base_Mid"), C("Drawer1", 0, 1.1f, 0.31f, 0.4f, 0.04f, 0.02f, "MAT_Base_Metal"), C("Drawer2", 0, 0.7f, 0.31f, 0.4f, 0.04f, 0.02f, "MAT_Base_Metal"), C("Drawer3", 0, 0.3f, 0.31f, 0.4f, 0.04f, 0.02f, "MAT_Base_Metal") }, null),
            ["PROP_F2_SampleFridge"] = ("F2", new[] { C("Body", 0, 1.0f, 0, 0.8f, 2.0f, 0.7f, "MAT_Base_Light"), C("Glass", 0, 1.1f, 0.36f, 0.65f, 1.5f, 0.02f, "MAT_Glass"), C("Seal", 0, 1.9f, 0.36f, 0.7f, 0.05f, 0.03f, "MAT_Crimson") }, null),
            ["PROP_F2_SurgeryTable"] = ("F2", new[] { C("Top", 0, 0.85f, 0, 0.8f, 0.08f, 2.0f, "MAT_Base_Metal"), Cy("Pillar", 0, 0.42f, 0, 0.25f, 0.8f, "MAT_Base_Mid"), C("Base", 0, 0.03f, 0, 0.6f, 0.06f, 0.8f, "MAT_Base_Dark") }, null),
            ["PROP_F2_LabChair"] = ("F2", new[] { C("Seat", 0, 0.45f, 0, 0.5f, 0.06f, 0.5f, "MAT_Base_Mid"), C("Back", 0, 0.75f, -0.22f, 0.5f, 0.55f, 0.05f, "MAT_Base_Mid"), Cy("Leg", 0, 0.22f, 0, 0.06f, 0.44f, "MAT_Base_Metal"), Cy("Foot", 0, 0.02f, 0, 0.55f, 0.04f, "MAT_Base_Dark") }, null),
            ["PROP_F2_SurgicalLamp"] = ("F2", new[] { Cy("Arm", 0, 2.75f, 0, 0.06f, 0.5f, "MAT_Base_Metal"), Cy("Head", 0, 2.42f, 0, 0.8f, 0.14f, "MAT_Base_Light"), Cy("Lens", 0, 2.34f, 0, 0.6f, 0.02f, "MAT_Glass") }, null),
            ["PROP_F2_SurgicalTools"] = ("F2", new[] { Cy("Stand", 0, 0.45f, 0, 0.05f, 0.9f, "MAT_Base_Metal"), C("Tray", 0, 0.92f, 0, 0.6f, 0.03f, 0.4f, "MAT_Base_Metal"), C("Scalpel", -0.1f, 0.95f, 0.05f, 0.2f, 0.01f, 0.02f, "MAT_Base_Light"), C("Clamp", 0.12f, 0.95f, -0.05f, 0.15f, 0.02f, 0.05f, "MAT_Base_Light") }, null),
            ["PH_F2_HumanCapsule"] = ("F2", new[] { Cy("Shell", 0, 1.1f, 0, 1.0f, 2.2f, "MAT_Glass"), Cy("Cap", 0, 2.15f, 0, 1.0f, 0.1f, "MAT_Base_Metal"), Cy("Plinth", 0, 0.08f, 0, 1.0f, 0.16f, "MAT_Base_Dark"), C("Body", 0, 1.0f, 0, 0.35f, 1.6f, 0.25f, "MAT_Skin") },
                "Glass human capsule (Vitalis clinical), body floating inside; 1.0 × 2.2 × 1.0"),
            // ---- F3 core
            ["PH_F3_CryoTank"] = ("F3", new[] { Cy("Tank", 0, 1.5f, 0, 1.6f, 3.0f, "MAT_Base_Metal"), Cy("Window", 0, 1.6f, 0.05f, 1.62f, 0.8f, "MAT_Glass"), Cy("Ring", 0, 2.9f, 0, 1.7f, 0.15f, "MAT_Crimson") },
                "Cryogenic vial store tank, frosted window band; Ø 1.6 × 3.0"),
            ["PH_F3_Transformer"] = ("F3", new[] { C("Core", 0, 1.0f, 0, 1.5f, 2.0f, 1.5f, "MAT_Base_Dark"), C("Fins", 0, 1.0f, 0, 1.6f, 1.6f, 0.1f, "MAT_Base_Metal"), Cy("Insulator", 0, 2.1f, 0, 0.3f, 0.3f, "MAT_CrimsonGlow") },
                "Industrial transformer (also boss phase 3 target with a WeakPoint); 1.5 × 2.2 × 1.5"),
            ["PROP_F3_CoolingColumn"] = ("F3", new[] { Cy("Column", 0, 3.0f, 0, 1.0f, 6.0f, "MAT_Base_Metal"), Cy("Band1", 0, 1.5f, 0, 1.05f, 0.1f, "MAT_Crimson"), Cy("Band2", 0, 4.5f, 0, 1.05f, 0.1f, "MAT_Crimson") }, null),
            ["PROP_F3_ContainmentConsole"] = ("F3", new[] { C("Body", 0, 0.5f, 0, 1.2f, 1.0f, 0.8f, "MAT_Base_Dark"), C("Screen", 0, 1.05f, -0.1f, 1.0f, 0.3f, 0.06f, "MAT_Phosphor"), C("Lamp", 0.45f, 1.05f, 0.2f, 0.1f, 0.1f, 0.1f, "MAT_CrimsonGlow") }, null),
            ["PH_F3_BrokenCapsule"] = ("F3", new[] { Cy("ShellHalf", 0, 1.1f, -0.2f, 1.2f, 2.2f, "MAT_Glass"), Cy("Plinth", 0, 0.08f, 0, 1.2f, 0.16f, "MAT_Base_Dark"), C("Shard", 0.4f, 0.1f, 0.6f, 0.3f, 0.05f, 0.4f, "MAT_Glass") },
                "Shattered containment capsule, glass on the floor; 1.2 × 2.2 × 1.2"),
            ["PROP_F3_HeavyCabling"] = ("F3", new[] { C("CableA", 0, 0.11f, -0.15f, 2.0f, 0.22f, 0.22f, "MAT_Base_Dark"), C("CableB", 0, 0.09f, 0.13f, 2.0f, 0.18f, 0.18f, "MAT_Base_Dark"), C("CableC", 0, 0.28f, 0, 2.0f, 0.14f, 0.14f, "MAT_Crimson") }, null),
            ["PROP_F3_PAHorn"] = ("F3", new[] { Cy("Horn", 0, 0.3f, 0, 0.6f, 0.6f, "MAT_Base_Metal"), C("Bracket", 0, 0.3f, -0.35f, 0.2f, 0.2f, 0.1f, "MAT_Base_Dark") }, null),
        };

        string log = "";
        foreach (var name in new List<string>(specs.Keys))
            if (AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Props/{specs[name].floor}/{name}.prefab") != null) specs.Remove(name);
        if (specs.Count == 0) return "all catalog props already exist";
        foreach (var kv in specs)
        {
            var root = new GameObject(kv.Key);
            var mesh = new GameObject("Mesh").transform;
            mesh.SetParent(root.transform, false);
            foreach (var p in kv.Value.parts)
            {
                var g = GameObject.CreatePrimitive(p.t);
                g.name = p.n;
                g.transform.SetParent(mesh, false);
                g.transform.localPosition = p.pos;
                g.transform.localScale = p.size;
                g.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{Shared}/{p.mat}.mat");
            }
            if (kv.Value.phNotes != null)
            {
                var info = root.AddComponent(System.Type.GetType("Heartbeat.Render.PlaceholderInfo, Heartbeat.Runtime"));
                var so = new SerializedObject(info);
                so.FindProperty("intendedModel").stringValue = kv.Value.phNotes;
                so.FindProperty("triangleBudget").intValue = 3000;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        log += PropTools.FinishAllInScene();
        // Trip-free floor cabling (OBJECT_CATALOG): no collider at all.
        foreach (var name in NoCollider)
            if (specs.ContainsKey(name) && GameObject.Find(name) is GameObject g)
                foreach (var col in g.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(col);
        foreach (var kv in specs)
        {
            var go = GameObject.Find(kv.Key);
            string dir = $"Assets/_Project/Prefabs/Props/{kv.Value.floor}";
            PrefabUtility.SaveAsPrefabAsset(go, $"{dir}/{kv.Key}.prefab");
            Object.DestroyImmediate(go);
        }
        EditorSceneManager.SaveScene(scene);
        return $"{specs.Count} props\n" + log;
    }
}
}
