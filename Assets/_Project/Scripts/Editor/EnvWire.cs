using System.Collections.Generic;
using System.Linq;
using Heartbeat.Generation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Puts the Blender environment (Blender3DAssets/Scripts/env_models.py, D-156) on the prefabs: every primitive renderer gives way
    /// to a model while pivots, colliders, layers and scripts stay. Idempotent: run it again after a new export.
    /// </summary>
    public static class EnvWire
    {
        private const string Models = "Assets/_Project/Models/";
        private const string Env = Models + "Env/";
        private const string Prefabs = "Assets/_Project/Prefabs/";
        private const string ModelChild = "Model";
        private static readonly string[] FloorNames = { "F1", "F2", "F3" };
        private static string log;
        private static int swapped;

        [MenuItem("Heartbeat/Models/Wire Environment Models")]
        public static void Menu() => Debug.Log("[Heartbeat][Models] " + Run());

        private static Material EnvMaterial(string floor)
        {
            string path = $"{Env}M_Env_{floor}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>($"{Env}Textures/T_Env_{floor}.png"));
            mat.SetFloat("_Smoothness", .12f);
            mat.SetFloat("_Metallic", 0f);
            mat.SetTexture("_EmissionMap", AssetDatabase.LoadAssetAtPath<Texture2D>($"{Env}Textures/T_Env_{floor}_Emission.png"));
            mat.SetColor("_EmissionColor", Color.white);
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;   // with None, URP drops _EMISSION on validation
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static string FindModel(string name)
        {
            foreach (var dir in new[] { Models + "Props/", Env + "F1/", Env + "F2/", Env + "F3/", Env + "Shared/" })
                if (System.IO.File.Exists(dir + name + ".fbx")) return dir + name + ".fbx";
            return null;
        }

        private static string FloorOfModel(string fbx, string name)
        {
            foreach (var f in FloorNames) if (fbx.Contains("/" + f + "/") || name.Contains("_" + f + "_") || name.EndsWith("_" + f)) return f;
            return name == "CH_Capsule_Reconstruction" ? "F1" : "Shared";
        }

        private static Mesh MeshOf(string fbx) => AssetDatabase.LoadAllAssetsAtPath(fbx).OfType<Mesh>().FirstOrDefault();

        private static Material[] MaterialsFor(Mesh mesh, string floor)
        {
            var glass = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/Shared/MAT_Glass.mat");
            var env = EnvMaterial(floor);
            return mesh.subMeshCount > 1 ? new[] { env, glass } : new[] { env };
        }

        private static bool IsPrimitive(Mesh m)
        {
            if (m == null) return false;
            string p = AssetDatabase.GetAssetPath(m);
            return p.StartsWith("Library") || p.StartsWith("Resources");
        }

        /// <summary>Own primitive renderers go; colliders and transforms stay. Nested prefabs answer for themselves.</summary>
        private static void DropPrimitives(GameObject root)
        {
            foreach (var r in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!r.TryGetComponent<MeshFilter>(out var f) || !IsPrimitive(f.sharedMesh)) continue;
                if (PrefabUtility.GetNearestPrefabInstanceRoot(r.gameObject) != null && r.gameObject != root) continue;
                Object.DestroyImmediate(r);
                Object.DestroyImmediate(f);
            }
        }

        /// <summary>F2 and F3 prefabs copy or inherit F1's: its model would stay under theirs, coplanar, and z-fight in black triangles.</summary>
        private static void DropOtherModels(Transform parent, string keep)
        {
            foreach (var t in parent.Cast<Transform>().Where(c => c.name.StartsWith(ModelChild + "_") && c.name != ModelChild + "_" + keep).ToArray())
                Object.DestroyImmediate(t.gameObject);
        }

        private static GameObject AddModel(Transform parent, string modelName, Vector3 localPos, Quaternion localRot, bool shadows = true)
        {
            string fbx = FindModel(modelName);
            if (fbx == null) { log += "missing model " + modelName + "\n"; return null; }
            var mesh = MeshOf(fbx);
            var old = parent.Find(ModelChild + "_" + modelName);
            if (old != null) Object.DestroyImmediate(old.gameObject);
            var go = new GameObject(ModelChild + "_" + modelName);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = localRot;
            go.layer = parent.gameObject.layer;
            GameObjectUtility.SetStaticEditorFlags(go, GameObjectUtility.GetStaticEditorFlags(parent.gameObject));
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var r = go.AddComponent<MeshRenderer>();
            r.sharedMaterials = MaterialsFor(mesh, FloorOfModel(fbx, modelName));
            r.shadowCastingMode = shadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            swapped++;
            return go;
        }

        private static void WithPrefab(string path, System.Action<GameObject> edit)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null) { log += "missing prefab " + path + "\n"; return; }
            var root = PrefabUtility.LoadPrefabContents(path);
            edit(root);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

        /// <summary>Floors and ceilings cast nothing: a beam next to a ceiling lamp threw long straight black wedges down the walls.</summary>
        private static void SwapPrefab(string path, string modelName, bool shadows = true)
        {
            WithPrefab(path, root =>
            {
                DropPrimitives(root);
                DropOtherModels(root.transform, modelName);
                AddModel(root.transform, modelName, Vector3.zero, Quaternion.identity, shadows);
            });
        }

        /// <summary>Prefabs already nesting a CH_ model keep it; only the material changes to the floor's atlas.</summary>
        private static void Rematerial(string path, string modelName)
        {
            WithPrefab(path, root =>
            {
                string fbx = FindModel(modelName);
                var mesh = MeshOf(fbx);
                foreach (var f in root.GetComponentsInChildren<MeshFilter>(true))
                    if (f.sharedMesh == mesh) { f.GetComponent<MeshRenderer>().sharedMaterials = MaterialsFor(mesh, FloorOfModel(fbx, modelName)); swapped++; }
            });
        }

        private static void CreateProp(string floor, string name, bool collider)
        {
            string fbx = FindModel(name);
            if (fbx == null) { log += "missing model " + name + "\n"; return; }
            string dir = Prefabs + "Props/" + floor;
            string path = $"{dir}/{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                var root = new GameObject(name) { layer = LayerMask.NameToLayer("Environment"), isStatic = true };
                if (collider)
                {
                    var b = MeshOf(fbx).bounds;
                    var box = root.AddComponent<BoxCollider>();
                    box.center = b.center;
                    box.size = b.size;
                }
                PrefabUtility.SaveAsPrefabAsset(root, path);
                Object.DestroyImmediate(root);
            }
            SwapPrefab(path, name, name != "PROP_Shared_Puddle" && name != "PROP_Shared_Grate");
        }

        private static void CreateElevator(string floor)
        {
            CreateProp(floor, "CH_Elevator_" + floor, true);
            WithPrefab($"{Prefabs}Props/{floor}/CH_Elevator_{floor}.prefab", root =>
            {
                root.layer = LayerMask.NameToLayer("Interactable");
                if (!root.TryGetComponent<Heartbeat.Level.BrokenElevator>(out _)) root.AddComponent<Heartbeat.Level.BrokenElevator>();
            });
        }

        private static void WireDoors()
        {
            string basePath = Prefabs + "Level/Door_F1.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(basePath) == null) AssetDatabase.MoveAsset(Prefabs + "Level/Door_2m.prefab", basePath);
            foreach (var f in FloorNames)
            {
                string path = $"{Prefabs}Level/Door_{f}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null) AssetDatabase.CopyAsset(basePath, path);
                WithPrefab(path, root =>
                {
                    var leaf = root.transform.Find("Leaf");
                    // The leaf keeps its collider and carve at the same size, now with scale 1 so the model is not stretched.
                    Vector3 s = leaf.localScale;
                    if (s != Vector3.one)
                    {
                        var box = leaf.GetComponent<BoxCollider>();
                        box.size = Vector3.Scale(box.size, s);
                        box.center = Vector3.Scale(box.center, s);
                        var carve = leaf.GetComponent<UnityEngine.AI.NavMeshObstacle>();
                        if (carve != null)
                        {
                            carve.size = Vector3.Scale(carve.size, s);
                            carve.center = Vector3.Scale(carve.center, s);
                        }
                        leaf.localScale = Vector3.one;
                    }
                    var mesh = MeshOf(FindModel($"Door_{f}_Leaf"));
                    if (!leaf.TryGetComponent<MeshFilter>(out var mf)) mf = leaf.gameObject.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;
                    if (!leaf.TryGetComponent<MeshRenderer>(out var mr)) mr = leaf.gameObject.AddComponent<MeshRenderer>();
                    mr.sharedMaterials = MaterialsFor(mesh, f);
                    DropOtherModels(root.transform, $"Door_{f}_Frame");
                    AddModel(root.transform, $"Door_{f}_Frame", Vector3.zero, Quaternion.identity);
                    if (f == "F2") SplitSlidingDoor(root, leaf, mesh, f);
                });
                var kit = AssetDatabase.LoadAssetAtPath<KitSet>($"Assets/_Project/ScriptableObjects/Rooms/KitSet_{f}.asset");
                kit.door = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                EditorUtility.SetDirty(kit);
            }
        }

        /// <summary>F2's hospital door parts sideways: two leaves, each half the opening, sliding into the walls on either side.</summary>
        private static void SplitSlidingDoor(GameObject root, Transform leaf, Mesh mesh, string floor)
        {
            const float half = .475f;
            var p = leaf.localPosition;
            p.x = half;
            leaf.localPosition = p;
            // The carve stays on the first leaf and still covers the whole opening; each collider covers its own half.
            var box = leaf.GetComponent<BoxCollider>();
            box.center = new Vector3(0f, box.center.y, box.center.z);
            box.size = new Vector3(.95f, box.size.y, box.size.z);
            var carve = leaf.GetComponent<UnityEngine.AI.NavMeshObstacle>();
            if (carve != null) carve.center = new Vector3(-half, carve.center.y, carve.center.z);

            var second = root.transform.Find("LeafB");
            if (second == null)
            {
                second = new GameObject("LeafB").transform;
                second.SetParent(root.transform, false);
                second.gameObject.layer = leaf.gameObject.layer;
            }
            second.localPosition = new Vector3(-half, p.y, p.z);
            second.localRotation = Quaternion.Euler(0, 180, 0);
            if (!second.TryGetComponent<MeshFilter>(out var mf)) mf = second.gameObject.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            if (!second.TryGetComponent<MeshRenderer>(out var mr)) mr = second.gameObject.AddComponent<MeshRenderer>();
            mr.sharedMaterials = MaterialsFor(mesh, floor);
            if (!second.TryGetComponent<BoxCollider>(out var b2)) b2 = second.gameObject.AddComponent<BoxCollider>();
            b2.center = box.center;
            b2.size = box.size;

            var so = new SerializedObject(root.GetComponent<Heartbeat.Level.DoorLeaf>());
            so.FindProperty("sideSlide").floatValue = .95f;
            so.FindProperty("secondLeaf").objectReferenceValue = second;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireLamps()
        {
            foreach (var (f, light) in new[] { ("F1", "Light_F1_Sodium"), ("F2", "Light_F2_Fluorescent"), ("F3", "Light_F3_Crimson") })
                WithPrefab($"{Prefabs}Level/{light}.prefab", root =>
                {
                    // The light points down through a rotated root; the fixture stays upright and casts no shadow, so it never blocks its own lamp.
                    DropOtherModels(root.transform, "CH_Lamp_" + f);
                    AddModel(root.transform, "CH_Lamp_" + f, Vector3.zero, Quaternion.Inverse(root.transform.localRotation), false);
                });
        }

        private static void WireRoomsOneOffs()
        {
            void Sibling(Transform t, string model, Vector3 offset, Quaternion rot)
            {
                if (t == null) return;
                if (t.TryGetComponent<MeshRenderer>(out var r)) Object.DestroyImmediate(r);
                if (t.TryGetComponent<MeshFilter>(out var f)) Object.DestroyImmediate(f);
                var m = AddModel(t.parent, model, t.localPosition + offset, t.localRotation * rot);
                if (m != null) m.layer = t.gameObject.layer;
            }

            WithPrefab(Prefabs + "Rooms/F1/ROOM_F1_Vertical_A.prefab", root => Sibling(FindDeep(root, "Mezzanine"), "CH_Env_Mezzanine", Vector3.zero, Quaternion.identity));
            WithPrefab(Prefabs + "Rooms/F3/ROOM_F3_Vertical_B.prefab", root => Sibling(FindDeep(root, "Catwalk"), "CH_Env_Catwalk", Vector3.zero, Quaternion.identity));
            WithPrefab(Prefabs + "Rooms/F2/KEY_F2_Final.prefab", root =>
            {
                Sibling(FindDeep(root, "PhotoTable"), "CH_Env_PhotoTable", new Vector3(0, -.35f, 0), Quaternion.identity);
                var frame = FindDeep(root, "Frame");
                // The photograph faces +X in the room; models face +Z.
                Sibling(frame, "CH_Env_PhotoFrame", Vector3.zero, Quaternion.Euler(0, 90, 0));
                foreach (var n in new[] { "Picture", "FaceBlur" })
                {
                    var t = FindDeep(root, n);
                    if (t != null) Object.DestroyImmediate(t.gameObject);
                }
            });
            WithPrefab(Prefabs + "Rooms/F3/ROOM_Mother.prefab", root =>
            {
                Sibling(FindDeep(root, "Exit"), "CH_Env_MotherExit", Vector3.zero, Quaternion.identity);
                // Boss camera and arm already nest their real models: their cubes were only there to carry the scripts.
                foreach (var t in root.GetComponentsInChildren<Transform>(true).Where(x => x.name.StartsWith("HostileCamera") || x.name == "CollectorArm" || x.name == "Claw").ToArray())
                {
                    bool hasModel = t.GetComponentsInChildren<MeshRenderer>(true).Any(r => r.transform != t && r.TryGetComponent<MeshFilter>(out var mf) && !IsPrimitive(mf.sharedMesh));
                    if (t.TryGetComponent<MeshRenderer>(out var r0) && t.TryGetComponent<MeshFilter>(out var f0) && IsPrimitive(f0.sharedMesh))
                    {
                        if (hasModel) { Object.DestroyImmediate(r0); Object.DestroyImmediate(f0); }
                        else if (t.name.StartsWith("HostileCamera"))
                        {
                            Object.DestroyImmediate(r0);
                            Object.DestroyImmediate(f0);
                            var m = AddModel(t, "CH_Env_HostileCamera", Vector3.zero, Quaternion.identity);
                            if (m != null) m.transform.localScale = new Vector3(1f / t.localScale.x, 1f / t.localScale.y, 1f / t.localScale.z);
                        }
                        else log += "kept primitive: " + t.name + " in ROOM_Mother\n";
                    }
                }
            });
            foreach (var f in FloorNames)
                WithPrefab($"{Prefabs}Rooms/{f}/KEY_{f}_Final.prefab", root =>
                {
                    // F2 and F3 are variants of F1's layout: the F1 stairs and exit they inherit would z-fight with their own.
                    foreach (var t in root.GetComponentsInChildren<Transform>(true).Where(x => (x.name.StartsWith("Model_CH_Exit_") || x.name.StartsWith("Model_CH_Stairs_")) && !x.name.EndsWith(f)).ToArray())
                        Object.DestroyImmediate(t.gameObject);
                    var stairs = root.transform.Find("Stairs");
                    if (stairs != null)
                    {
                        // The ramp stays for walking; only its look gives way to steps.
                        foreach (var r in stairs.GetComponentsInChildren<MeshRenderer>(true)) r.enabled = false;
                        AddModel(stairs, "CH_Stairs_" + f, Vector3.zero, Quaternion.identity);
                    }
                    var exit = root.transform.Find("StairsExit");
                    if (exit != null) AddModel(exit, "CH_Exit_" + f, Vector3.zero, Quaternion.identity);
                });
        }

        /// <summary>A ceiling lamp reaches the walls up to near its own height: a narrower cone left them black above a hard line.</summary>
        public static void WireRoomLights()
        {
            foreach (var (f, intensity) in new[] { ("F1", 8f), ("F2", 7f), ("F3", 10f) })
                foreach (var g in AssetDatabase.FindAssets("t:Prefab", new[] { Prefabs + "Rooms/" + f }))
                    WithPrefab(AssetDatabase.GUIDToAssetPath(g), root =>
                    {
                        // Rooms re-dressed from another floor keep that floor's lamp prefab: only its fixture changes.
                        var lamp = MeshOf(FindModel("CH_Lamp_" + f));
                        foreach (var m in root.GetComponentsInChildren<MeshFilter>(true).Where(x => x.name.StartsWith("Model_CH_Lamp_") && x.sharedMesh != lamp))
                        {
                            m.sharedMesh = lamp;
                            m.GetComponent<MeshRenderer>().sharedMaterials = MaterialsFor(lamp, f);
                        }
                        foreach (var l in root.GetComponentsInChildren<Light>(true))
                        {
                            if (l.type != LightType.Spot || l.spotAngle < 100f) continue;
                            l.spotAngle = 165f;
                            l.innerSpotAngle = 100f;
                            l.intensity = intensity;
                        }
                    });
        }

        private static Transform FindDeep(GameObject root, string name) => root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == name);

        private static void WireScenes()
        {
            void Replace(Scene scene, string name, string model, Vector3 offset, float yaw = 0f)
            {
                var t = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).FirstOrDefault(x => x.name == name);
                if (t == null) { log += "scene object not found: " + name + "\n"; return; }
                if (t.TryGetComponent<MeshRenderer>(out var r)) Object.DestroyImmediate(r);
                if (t.TryGetComponent<MeshFilter>(out var f)) Object.DestroyImmediate(f);
                var m = AddModel(t, model, offset, Quaternion.Euler(0, yaw, 0));
                // The object carries a scale from its primitive days; undo it for the model, keep it for the collider.
                if (m != null) m.transform.localScale = new Vector3(1f / t.lossyScale.x, 1f / t.lossyScale.y, 1f / t.lossyScale.z);
                if (m != null) m.transform.localPosition = new Vector3(offset.x / t.lossyScale.x, offset.y / t.lossyScale.y, offset.z / t.lossyScale.z);
            }

            var intro = EditorSceneManager.OpenScene("Assets/_Project/Scenes/Intro.unity", OpenSceneMode.Additive);
            Replace(intro, "Table", "CH_Intro_Table", new Vector3(0, -.38f, 0));
            Replace(intro, "Chair", "CH_Intro_Chair", new Vector3(0, -.45f, 0));
            Replace(intro, "Photo", "CH_Intro_Photo", Vector3.zero);
            Replace(intro, "Hatch", "CH_Intro_Hatch", Vector3.zero);
            Replace(intro, "CorridorElevatorDoor", "CH_Intro_LiftDoor", Vector3.zero);
            // Models face +Z: the hatch panel looks south at the player, the lift panel west into the cabin.
            Replace(intro, "HatchPanel", "CH_Intro_Panel", Vector3.zero, 180f);
            Replace(intro, "ElevatorPanel", "CH_Intro_Panel", Vector3.zero, 270f);
            EditorSceneManager.SaveScene(intro);
            EditorSceneManager.CloseScene(intro, true);
        }

        public static string Run()
        {
            log = "";
            swapped = 0;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (var g in AssetDatabase.FindAssets("t:Model", new[] { Env.TrimEnd('/') })) AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(g), ImportAssetOptions.ForceUpdate);

            foreach (var f in FloorNames)
            {
                foreach (var piece in new[] { "Wall_2m", "Wall_2m_High", "WallWindow_2m", "Doorway_2m", "Floor_2x2", "Ceiling_2x2" })
                    SwapPrefab($"{Prefabs}Kit/{f}/KIT_{f}_{piece}.prefab", $"KIT_{f}_{piece}", piece != "Floor_2x2" && piece != "Ceiling_2x2");
                string column = $"{Prefabs}Kit/{f}/KIT_{f}_Column.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(column) == null) AssetDatabase.CopyAsset($"{Prefabs}Kit/F1/KIT_F1_Column.prefab", column);
                SwapPrefab(column, $"KIT_{f}_Column");
            }
            SwapPrefab(Prefabs + "Kit/Shared/KIT_Shared_Platform_2x2.prefab", "KIT_Shared_Platform_2x2");
            SwapPrefab(Prefabs + "Kit/Shared/KIT_Shared_Ramp_2x4.prefab", "KIT_Shared_Ramp_2x4");
            WireDoors();
            WireLamps();

            foreach (var n in new[] { "ThickPipe", "ToolShelf", "Workbench" }) SwapPrefab($"{Prefabs}Props/F1/PROP_F1_{n}.prefab", "PROP_F1_" + n);
            foreach (var n in new[] { "Console", "Desk", "FilingCabinet", "IVCart", "LabChair", "ResearchBed", "SampleFridge", "Screen", "SurgeryTable", "SurgicalLamp", "SurgicalTools" }) SwapPrefab($"{Prefabs}Props/F2/PROP_F2_{n}.prefab", "PROP_F2_" + n);
            foreach (var n in new[] { "ContainmentConsole", "CoolingColumn", "HeavyCabling", "PAHorn" }) SwapPrefab($"{Prefabs}Props/F3/PROP_F3_{n}.prefab", "PROP_F3_" + n);
            foreach (var n in new[] { "MetalCrate", "VitalisPoster" }) SwapPrefab($"{Prefabs}Props/Shared/PROP_Shared_{n}.prefab", "PROP_Shared_" + n);
            foreach (var (p, m) in new[] { ("Props/F1/PH_F1_Generator", "CH_F1_Generator"), ("Props/F1/PH_F1_Boiler", "CH_F1_Boiler"), ("Props/F1/PH_F1_Forklift", "CH_F1_Forklift"), ("Props/F1/PH_F1_GantryCrane", "CH_F1_GantryCrane"), ("Props/F1/PH_F1_WaterPump", "CH_F1_WaterPump"), ("Props/F1/PH_Capsule_Reconstruction", "CH_Capsule_Reconstruction"), ("Props/F2/PH_F2_HumanCapsule", "CH_F2_HumanCapsule"), ("Props/F2/PH_F2_MonsterCapsule", "CH_F2_MonsterCapsule"), ("Props/F2/PH_F2_ResonanceCapsule", "CH_F2_ResonanceCapsule"), ("Props/F3/PH_F3_BrokenCapsule", "CH_F3_BrokenCapsule"), ("Props/F3/PH_F3_CryoTank", "CH_F3_CryoTank"), ("Props/F3/PH_F3_Transformer", "CH_F3_Transformer"), ("Props/F3/PROP_F3_MixingTable", "CH_F3_MixingTable") })
                Rematerial($"{Prefabs}{p}.prefab", m);
            foreach (var (n, c) in new[] { ("PROP_Shared_Speaker", false), ("PROP_Shared_CableBundle", false), ("PROP_Shared_Pipe_2m", true), ("PROP_Shared_Pipe_Elbow", true), ("PROP_Shared_TrashPile", false), ("PROP_Shared_MetalCrate_Half", true), ("PROP_Shared_Grate", false), ("PROP_Shared_Puddle", false), ("PROP_Shared_HackPanel", true) }) CreateProp("Shared", n, c);
            foreach (var (n, c) in new[] { ("PROP_F1_WaterTank", true), ("PROP_F1_PressureValve", false), ("PROP_F1_Tools", false), ("PROP_F1_Gauge", false), ("PROP_F1_BigContainer", true), ("PROP_F1_PersonalTray", true) }) CreateProp("F1", n, c);
            CreateProp("F2", "PROP_F2_LabCoatRack", false);
            CreateProp("F1", "PROP_F1_CRT", false);
            CreateProp("F2", "PROP_F2_CRT", false);
            CreateProp("F3", "PROP_F3_CRTBank", true);
            CreateElevator("F2");
            CreateElevator("F3");
            foreach (var (p, m) in new[] { ("ITEM_Heal", "CH_Item_Heal"), ("ITEM_AmmoBlister", "CH_Item_Ammo"), ("ITEM_Spike", "CH_Item_Spike"), ("ITEM_KeyMandatory", "CH_Item_KeyMandatory"), ("ITEM_KeyOptional", "CH_Item_KeyOptional") })
                Rematerial($"{Prefabs}Items/{p}.prefab", m);

            WithPrefab(Prefabs + "Items/INT_PowerSwitch.prefab", root =>
            {
                DropPrimitives(root);
                AddModel(root.transform, "CH_INT_PowerSwitch", Vector3.zero, Quaternion.identity);
                var lever = root.transform.Find("Lever");
                if (lever != null) AddModel(lever.parent, "CH_INT_Lever", lever.localPosition, Quaternion.identity);
            });

            WireRoomsOneOffs();
            WireRoomLights();
            WireScenes();
            AssetDatabase.SaveAssets();
            return $"{swapped} models wired\n{log}";
        }
    }
}
