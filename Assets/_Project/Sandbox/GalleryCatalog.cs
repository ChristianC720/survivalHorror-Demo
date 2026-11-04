using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.Sandbox
{
    /// <summary>One asset on show.</summary>
    public sealed class GalleryItem
    {
        public string Name, Path, Type, Floor;
        public GameObject Root;
        public Bounds Bounds;
        public int Triangles;
        public string[] Materials;
        public Animator Animator;
        public AnimationClip[] Clips = new AnimationClip[0];
        public Texture2D BodyVariant;
    }

    /// <summary>
    /// Finds the project's assets when Play starts and lays out inert copies by type and floor. Nothing is stored in the
    /// scene, so it holds no asset references and always shows the current files. Copies lose every game script before they
    /// activate, so no game code runs here.
    /// </summary>
    public sealed class GalleryCatalog
    {
        public static readonly string[] Types = { "Sala de muestra", "Personajes", "Enemigos", "Objetos", "Confiscados", "Props", "Máquinas", "Nivel", "Kit", "Salas", "Materiales", "Modelos sueltos" };
        public static readonly string[] Floors = { "Shared", "F1", "F2", "F3" };

        private const string Prefabs = "Assets/_Project/Prefabs/";
        private const string Models = "Assets/_Project/Models/";
        private const float CellGap = 8f, ItemGap = 1.2f, CellWidth = 45f, PedestalBelow = 0.6f, PedestalHeight = 1f;

        // GDD Personajes: the floor where each type first appears.
        private static readonly Dictionary<string, string> EnemyFloor = new Dictionary<string, string> { { "Alto", "F1" }, { "Rastrero", "F1" }, { "Boca", "F2" }, { "Mudo", "F3" } };
        private static readonly string[] Inert = { "UnityEngine.AI.NavMeshAgent", "UnityEngine.AI.NavMeshObstacle", "UnityEngine.CharacterController", "UnityEngine.Rigidbody", "UnityEngine.AudioListener", "UnityEngine.Camera", "UnityEngine.Playables.PlayableDirector" };

        public readonly List<GalleryItem> Items = new List<GalleryItem>();
        public readonly Dictionary<(string type, string floor), Bounds> Cells = new Dictionary<(string, string), Bounds>();
        public readonly List<string> Skipped = new List<string>();
        public AudioClip[] AudioClips = new AudioClip[0];

        private Transform holder;
        private readonly HashSet<Mesh> shown = new HashSet<Mesh>();

        public void Build(Transform parent)
        {
            var hidden = new GameObject("Unpacking");
            hidden.SetActive(false);
            holder = hidden.transform;

            AddSampleRooms();
            AddLukas();
            AddEnemies();
            AddFolder(Prefabs + "Items", "Objetos", false);
            AddFolder(Prefabs + "Items/Confiscated", "Confiscados", false);
            AddFolder(Prefabs + "Props", "Props", true);
            AddFolder(Prefabs + "PH", "Máquinas", true);
            AddFolder(Prefabs + "Level", "Nivel", true);
            AddFolder(Prefabs + "Kit", "Kit", true);
            AddFolder(Prefabs + "Rooms", "Salas", true);
            AddMaterials();
            AddLooseModels();
            AudioClips = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/_Project" }).Select(g => AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(g))).Where(c => c != null).ToArray();

            Layout(parent);
            Object.Destroy(hidden);
        }

        /// <summary>A small room per floor, built from the kit by the KIT_SPEC rules with one shadowed light inside: any seam that leaks shows on the ground outside.</summary>
        private void AddSampleRooms()
        {
            const int cells = 3;
            foreach (var f in new[] { "F1", "F2", "F3" })
            {
                string kit = $"{Prefabs}Kit/{f}/KIT_{f}_";
                GameObject P(string n) => AssetDatabase.LoadAssetAtPath<GameObject>(n);
                var room = new GameObject("Sala " + f);
                room.transform.SetParent(holder, false);
                void Put(GameObject asset, Vector3 at, float yaw)
                {
                    if (asset == null) return;
                    var go = Object.Instantiate(asset, room.transform);
                    go.transform.SetLocalPositionAndRotation(at, Quaternion.Euler(0, yaw, 0));
                }
                for (int x = 0; x < cells; x++)
                    for (int z = 0; z < cells; z++)
                    {
                        Put(P(kit + "Floor_2x2.prefab"), new Vector3(x * 2, 0, z * 2), 0);
                        Put(P(kit + "Ceiling_2x2.prefab"), new Vector3(x * 2, 0, z * 2), 0);
                    }
                for (int k = 0; k < cells; k++)
                {
                    // South: wall, door, wall. North: window in the middle. East and west closed.
                    Put(P(kit + (k == 1 ? "Doorway_2m.prefab" : "Wall_2m.prefab")), new Vector3(2 * k + 1, 0, 0), 0);
                    Put(P(kit + (k == 1 ? "WallWindow_2m.prefab" : "Wall_2m.prefab")), new Vector3(2 * k + 1, 0, 2 * cells), 180);
                    Put(P(kit + "Wall_2m.prefab"), new Vector3(0, 0, 2 * k + 1), 90);
                    Put(P(kit + "Wall_2m.prefab"), new Vector3(2 * cells, 0, 2 * k + 1), 270);
                }
                Put(P($"{Prefabs}Level/Door_{f}.prefab"), new Vector3(3, 0, 0), 0);
                Put(P(kit + "Column.prefab"), new Vector3(2, 0, 4), 0);
                string lamp = f == "F1" ? "Light_F1_Sodium" : f == "F2" ? "Light_F2_Fluorescent" : "Light_F3_Crimson";
                var lampAsset = P($"{Prefabs}Level/{lamp}.prefab");
                if (lampAsset != null)
                {
                    var go = Object.Instantiate(lampAsset, room.transform);
                    go.transform.localPosition = new Vector3(3, 2.8f, 3);
                    foreach (var l in go.GetComponentsInChildren<Light>(true)) { l.shadows = LightShadows.Soft; l.range = 12f; }
                }
                Add(room, "Sala " + f, "kit de " + f, "Sala de muestra", f);
                Object.Destroy(room);
            }
        }

        private static string FloorOf(string path)
        {
            var m = Regex.Match(path, @"(^|[/_])F([123])([/_.]|$)");
            return m.Success ? "F" + m.Groups[2].Value : "Shared";
        }

        private void AddFolder(string folder, string type, bool recursive)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { folder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!recursive && System.IO.Path.GetDirectoryName(path).Replace('\\', '/') != folder) continue;
                Add(AssetDatabase.LoadAssetAtPath<GameObject>(path), System.IO.Path.GetFileNameWithoutExtension(path), path, type, FloorOf(path));
            }
        }

        private GalleryItem Add(GameObject asset, string name, string path, string type, string floor, System.Func<GameObject, bool> prepare = null)
        {
            if (asset == null) { Skipped.Add(path); return null; }
            var go = Object.Instantiate(asset, holder);
            go.name = name;
            go.transform.localPosition = Vector3.zero;
            Strip(go);
            if (prepare != null && !prepare(go)) { Object.Destroy(go); Skipped.Add(path); return null; }
            // Models are authored facing away from the first view; rooms and kit keep their grid orientation.
            if (type != "Kit" && type != "Salas" && type != "Nivel" && type != "Sala de muestra") go.transform.localRotation *= Quaternion.Euler(0, 180, 0);
            var renderers = go.GetComponentsInChildren<Renderer>(true).Where(r => r.enabled).ToArray();
            if (renderers.Length == 0)
            {
                // Lights, sockets and spawn points have no mesh: a small marker makes them findable and pickable.
                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.DestroyImmediate(marker.GetComponent<Collider>());
                marker.name = "Marker";
                marker.transform.SetParent(go.transform, false);
                marker.transform.localScale = Vector3.one * .25f;
                marker.GetComponent<Renderer>().sharedMaterial = Flat(new Color(.9f, .75f, .2f));
                renderers = new[] { marker.GetComponent<Renderer>() };
            }
            var item = new GalleryItem { Name = name, Path = path, Type = type, Floor = floor, Root = go };
            item.Bounds = Measure(go, renderers, out item.Triangles);
            item.Materials = renderers.SelectMany(r => r.sharedMaterials).Where(m => m != null).Select(m => m.name).Distinct().ToArray();
            Items.Add(item);
            return item;
        }

        /// <summary>Removes game scripts from the inactive copy, so their Awake never runs.</summary>
        private static void Strip(GameObject go)
        {
            // Several passes: RequireComponent refuses to remove a component while another still needs it.
            for (int pass = 0; pass < 4; pass++)
            {
                foreach (var c in go.GetComponentsInChildren<Component>(true))
                {
                    if (c == null || c is Transform) continue;
                    string type = c.GetType().FullName;
                    if (type.StartsWith("Heartbeat.") || type.StartsWith("Unity.AI.Navigation.") || System.Array.IndexOf(Inert, type) >= 0)
                        Object.DestroyImmediate(c);
                }
            }
        }

        private Bounds Measure(GameObject go, Renderer[] renderers, out int triangles)
        {
            bool any = false;
            var b = new Bounds();
            triangles = 0;
            foreach (var r in renderers)
            {
                Mesh mesh = r is SkinnedMeshRenderer s ? s.sharedMesh : r.GetComponent<MeshFilter>() != null ? r.GetComponent<MeshFilter>().sharedMesh : null;
                if (mesh == null) continue;
                shown.Add(mesh);
                for (int sub = 0; sub < mesh.subMeshCount; sub++) triangles += (int)mesh.GetIndexCount(sub) / 3;   // imported meshes are not readable
                var m = mesh.bounds;
                var min = m.min;
                var max = m.max;
                for (int i = 0; i < 8; i++)
                {
                    var p = r.transform.TransformPoint(new Vector3((i & 1) == 0 ? min.x : max.x, (i & 2) == 0 ? min.y : max.y, (i & 4) == 0 ? min.z : max.z));
                    if (!any) { b = new Bounds(p, Vector3.zero); any = true; }
                    else b.Encapsulate(p);
                }
            }
            return b;
        }

        private void AddLukas()
        {
            string fbx = Models + "Characters/CH_Lukas_RH07.fbx";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(fbx);
            var clips = ClipsOf(fbx);
            var suit = AssetDatabase.LoadAssetAtPath<Material>(Models + "Characters/M_Lukas_RH07.mat");
            var civil = AssetDatabase.LoadAssetAtPath<Material>(Models + "Characters/M_Lukas_Civil.mat");

            void One(string name, bool isCivil, Texture2D variant)
            {
                var item = Add(asset, name, fbx, "Personajes", "Shared", go =>
                {
                    foreach (var r in go.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    {
                        bool rCivil = r.name.EndsWith("_Civil");
                        r.enabled = rCivil == isCivil;
                        r.sharedMaterial = rCivil ? civil : suit;
                    }
                    if (!isCivil) Equip(go);
                    return true;
                });
                if (item == null) return;
                SetAnim(item, clips);
                item.BodyVariant = variant;
            }

            One("Lukas", false, null);
            One("Lukas civil", true, null);
            for (int i = 1; i <= 4; i++)
                One("Lukas variante " + i, false, AssetDatabase.LoadAssetAtPath<Texture2D>($"{Models}Characters/Textures/CH_Lukas_RH07_Var{i}.png"));
        }

        /// <summary>The revolver and terminal ride on the skeleton at the same places the game uses, read from the model's markers.</summary>
        private static void Equip(GameObject lukas)
        {
            var all = lukas.GetComponentsInChildren<Transform>(true);
            Transform Bone(string n) => all.FirstOrDefault(t => t.name == n);
            Transform grip = Bone("WeaponGrip"), toward = Bone("WeaponGrip_Barrel"), up = Bone("WeaponGrip_Up");
            var gunAsset = AssetDatabase.LoadAssetAtPath<GameObject>(Models + "Props/CH_Revolver.fbx");
            if (grip != null && gunAsset != null)
            {
                var gun = Object.Instantiate(gunAsset, Bone("RightHand"), true);
                foreach (var r in gun.GetComponentsInChildren<Renderer>(true)) r.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(Models + "Props/M_Revolver.mat");
                gun.transform.SetPositionAndRotation(grip.position, Quaternion.LookRotation(toward.position - grip.position, up.position - grip.position));
            }
            var termAsset = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + "Items/PROP_ArmTerminal.prefab");
            var forearm = Bone("LeftLowerArm");
            if (termAsset != null && forearm != null)
            {
                var t = Object.Instantiate(termAsset, forearm, true);
                t.transform.SetPositionAndRotation(lukas.transform.TransformPoint(new Vector3(-0.355f, 0.99f, -0.045f)), Quaternion.LookRotation(-lukas.transform.right, lukas.transform.up));
            }
        }

        private void AddEnemies()
        {
            foreach (var pair in EnemyFloor)
            {
                string path = $"{Prefabs}Characters/Enemy_{pair.Key}.prefab";
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var clips = ClipsOf($"{Models}Enemies/CH_{pair.Key}.fbx");
                var item = Add(asset, pair.Key, path, "Enemigos", pair.Value);
                if (item != null) SetAnim(item, clips);
            }
        }

        private static AnimationClip[] ClipsOf(string fbx) =>
            AssetDatabase.LoadAllAssetsAtPath(fbx).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview") && !c.name.Contains("000_Bind")).OrderBy(c => c.name).ToArray();

        private static void SetAnim(GalleryItem item, AnimationClip[] clips)
        {
            item.Animator = item.Root.GetComponentInChildren<Animator>(true);
            if (item.Animator == null) return;
            item.Animator.runtimeAnimatorController = null;   // the gallery plays clips directly: the game's state machines stay out
            item.Animator.applyRootMotion = false;
            item.Clips = clips;
        }

        private void AddMaterials()
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(holder, false);
            Object.DestroyImmediate(sphere.GetComponent<Collider>());
            foreach (var guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Project/Materials" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                Add(sphere, System.IO.Path.GetFileNameWithoutExtension(path), path, "Materiales", FloorOf(path), go =>
                {
                    go.GetComponent<Renderer>().sharedMaterial = mat;
                    return true;
                });
            }
            Object.Destroy(sphere);
        }

        /// <summary>Models no prefab uses yet, such as the team's Aturdidor and the scene-only statics.</summary>
        private void AddLooseModels()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { Models }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/ModelsNew/") || !path.EndsWith(".fbx")) continue;
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset == null) continue;
                var meshes = asset.GetComponentsInChildren<MeshFilter>(true).Select(f => f.sharedMesh).Concat(asset.GetComponentsInChildren<SkinnedMeshRenderer>(true).Select(s => s.sharedMesh)).Where(m => m != null).ToArray();
                if (meshes.Length == 0 || meshes.Any(shown.Contains)) continue;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                string dir = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                var mat = AssetDatabase.LoadAssetAtPath<Material>($"{dir}/M_{name}.mat")
                    ?? AssetDatabase.LoadAssetAtPath<Material>(name.StartsWith("CH_Item_") ? Models + "Props/M_Herramientas.mat" : Models + "Props/M_Palette.mat");
                Add(asset, name, path, "Modelos sueltos", FloorOf(path), go =>
                {
                    foreach (var r in go.GetComponentsInChildren<Renderer>(true)) r.sharedMaterial = mat;
                    return true;
                });
            }
        }

        private void Layout(Transform parent)
        {
            // Shelf pack each cell around its own origin, then give rows and columns the room their largest cell needs.
            var sizes = new Dictionary<(string, string), Vector2>();
            var placed = new Dictionary<GalleryItem, Vector3>();
            foreach (var group in Items.GroupBy(i => (i.Type, i.Floor)))
            {
                float x = 0, z = 0, row = 0, width = 0;
                foreach (var it in group.OrderByDescending(i => i.Bounds.size.z))
                {
                    var s = it.Bounds.size;
                    if (x > 0 && x + s.x > CellWidth) { z += row + ItemGap; x = 0; row = 0; }
                    bool tiny = Mathf.Max(s.x, s.y, s.z) < PedestalBelow;
                    placed[it] = new Vector3(x - it.Bounds.min.x, (tiny ? PedestalHeight : 0f) - it.Bounds.min.y, z - it.Bounds.min.z);
                    x += s.x + ItemGap;
                    row = Mathf.Max(row, s.z);
                    width = Mathf.Max(width, x);
                }
                sizes[group.Key] = new Vector2(width, z + row);
            }
            var colX = new Dictionary<string, float>();
            float cursor = 0;
            foreach (var f in Floors)
            {
                colX[f] = cursor;
                cursor += sizes.Where(p => p.Key.Item2 == f).Select(p => p.Value.x).DefaultIfEmpty(10f).Max() + CellGap;
            }
            float totalX = cursor;
            float rowZ = 0;
            foreach (var type in Types)
            {
                float depth = 0;
                foreach (var f in Floors)
                {
                    if (!sizes.TryGetValue((type, f), out var size)) continue;
                    depth = Mathf.Max(depth, size.y);
                    var origin = new Vector3(colX[f], 0, rowZ);
                    Cells[(type, f)] = new Bounds(origin + new Vector3(size.x / 2, 0, size.y / 2), new Vector3(size.x, 0, size.y));
                    var cell = new GameObject($"{type} {f}").transform;
                    cell.SetParent(parent, false);
                    Plate(cell, Cells[(type, f)], FloorTint(f));
                    foreach (var it in Items.Where(i => i.Type == type && i.Floor == f))
                    {
                        var at = origin + placed[it];
                        it.Root.transform.SetParent(cell, false);
                        it.Root.transform.position = at;
                        it.Bounds.center += at;
                        if (at.y - 0.01f > 0 && Mathf.Max(it.Bounds.size.x, it.Bounds.size.y, it.Bounds.size.z) < PedestalBelow) Pedestal(cell, it.Bounds);
                        var box = it.Root.AddComponent<BoxCollider>();   // trigger: lets the mouse pick it, never blocks walking
                        box.isTrigger = true;
                        box.center = it.Root.transform.InverseTransformPoint(it.Bounds.center);
                        box.size = it.Bounds.size + Vector3.one * .05f;
                    }
                }
                rowZ += depth + CellGap;
            }
            // Ground under the whole gallery, for walk mode.
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.SetParent(parent, false);
            ground.transform.localScale = new Vector3(totalX + 40, 1, rowZ + 40);
            ground.transform.position = new Vector3(totalX / 2, -.56f, rowZ / 2);
            ground.GetComponent<Renderer>().sharedMaterial = Flat(new Color(.08f, .08f, .09f));
        }

        private static Color FloorTint(string floor) => floor == "F1" ? new Color(.2f, .15f, .08f) : floor == "F2" ? new Color(.1f, .15f, .19f) : floor == "F3" ? new Color(.2f, .07f, .08f) : new Color(.14f, .14f, .15f);

        private static void Plate(Transform cell, Bounds b, Color color)
        {
            var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Plate";
            plate.transform.SetParent(cell, false);
            plate.transform.localScale = new Vector3(b.size.x + 2, .02f, b.size.z + 2);
            plate.transform.position = new Vector3(b.center.x, -.03f, b.center.z);
            Object.Destroy(plate.GetComponent<Collider>());
            plate.GetComponent<Renderer>().sharedMaterial = Flat(color);
        }

        private static void Pedestal(Transform cell, Bounds b)
        {
            var p = GameObject.CreatePrimitive(PrimitiveType.Cube);
            p.name = "Pedestal";
            p.transform.SetParent(cell, false);
            p.transform.localScale = new Vector3(Mathf.Max(b.size.x, .2f) + .06f, PedestalHeight, Mathf.Max(b.size.z, .2f) + .06f);
            p.transform.position = new Vector3(b.center.x, PedestalHeight / 2, b.center.z);
            p.GetComponent<Renderer>().sharedMaterial = Flat(new Color(.22f, .22f, .23f));
        }

        private static Material Flat(Color c)
        {
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = c };
            m.SetFloat("_Smoothness", 0f);
            return m;
        }
    }
}
