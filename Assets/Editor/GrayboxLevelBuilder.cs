using System.Collections.Generic;
using Heartbeat.Config;
using Heartbeat.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Construye la escena graybox VS_Piso1 desde una tabla de coordenadas fija y hecha a mano.
    /// Es una herramienta de edición: se ejecuta una vez y no existe en runtime.
    /// </summary>
    public static class GrayboxLevelBuilder
    {
        private const string ScenePath = "Assets/Scenes/VS_Piso1.unity";
        private const string ConfigFolder = "Assets/ScriptableObjects";
        private const string ConfigPath = ConfigFolder + "/GameConfig.asset";

        // ---------------------------------------------------------------------
        // Tabla fija de la planta. X = este, Z = norte, suelo en y = 0. Sin azar.
        // ---------------------------------------------------------------------

        private sealed class RoomDef
        {
            public readonly string Id;
            public readonly Vector2 Center;
            public readonly Vector2 Size;
            public readonly bool Safe;
            public readonly string[] Adjacent;

            public RoomDef(string id, float cx, float cz, float sx, float sz, bool safe, params string[] adjacent)
            {
                Id = id;
                Center = new Vector2(cx, cz);
                Size = new Vector2(sx, sz);
                Safe = safe;
                Adjacent = adjacent;
            }

            public float MinX => Center.x - Size.x * 0.5f;
            public float MaxX => Center.x + Size.x * 0.5f;
            public float MinZ => Center.y - Size.y * 0.5f;
            public float MaxZ => Center.y + Size.y * 0.5f;
        }

        private sealed class OpeningDef
        {
            public readonly string A;
            public readonly string B;
            public readonly Vector2 Center;
            public readonly bool AlongZ;
            public readonly bool Locked;

            public OpeningDef(string a, string b, float x, float z, bool alongZ, bool locked = false)
            {
                A = a;
                B = b;
                Center = new Vector2(x, z);
                AlongZ = alongZ;
                Locked = locked;
            }
        }

        private sealed class Wall
        {
            public bool AlongZ;
            public float Coord;
            public readonly List<Vector2> Spans = new List<Vector2>();
        }

        private static readonly RoomDef[] Rooms =
        {
            new RoomDef("FilaCapsulas", 0f, 0f, 12f, 8f, true, "Pasillo"),
            new RoomDef("Pasillo", 19f, 0f, 26f, 3f, false, "FilaCapsulas", "Almacen", "SalaAbierta"),
            new RoomDef("Almacen", 14f, 7.5f, 12f, 12f, false, "Pasillo"),
            new RoomDef("SalaAbierta", 40f, 0f, 16f, 16f, false, "Pasillo", "SalaClaveSecundaria"),
            new RoomDef("SalaVigilancia", 53f, 10f, 10f, 10f, false, "SalaClaveSecundaria"),
            new RoomDef("SalaClaveSecundaria", 53f, 0f, 10f, 10f, false, "SalaAbierta", "SalaVigilancia", "SalaClaveFinal"),
            new RoomDef("SalaClaveFinal", 65f, 0f, 14f, 12f, false, "SalaClaveSecundaria"),
        };

        private static readonly OpeningDef[] Openings =
        {
            new OpeningDef("FilaCapsulas", "Pasillo", 6f, 0f, true),
            new OpeningDef("Pasillo", "Almacen", 17f, 1.5f, false),
            new OpeningDef("Pasillo", "SalaAbierta", 32f, 0f, true),
            new OpeningDef("SalaAbierta", "SalaClaveSecundaria", 48f, 0f, true),
            new OpeningDef("SalaClaveSecundaria", "SalaVigilancia", 53f, 5f, false),
            new OpeningDef("SalaClaveSecundaria", "SalaClaveFinal", 58f, 0f, true, true),
        };

        // ---------------------------------------------------------------------

        [MenuItem("Heartbeat/Graybox/Build VS_Piso1")]
        public static void Build()
        {
            GameConfig config = LoadOrCreateConfig();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            RemoveDirectionalLights();
            ConfigureEnvironment(config);
            ConfigureCamera();

            Transform root = CreateGroup("VS_Piso1", null);
            Transform geometry = CreateGroup("Geometry", root);
            Transform props = CreateGroup("Props", root);
            Transform lights = CreateGroup("Lights", root);
            Transform doors = CreateGroup("Doors", root);
            Transform triggers = CreateGroup("Triggers", root);

            int wallSegments = BuildShell(geometry, config);
            int propCount = BuildProps(props, config, doors);
            int lightCount = BuildLights(lights, config);
            Dictionary<string, RoomVolume> volumes = BuildRoomVolumes(triggers, config);
            WireAdjacency(volumes);
            CreateSpawnPoint(root);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            FrameSceneView();

            Debug.Log($"[Graybox] VS_Piso1 listo en {ScenePath}: {Rooms.Length} salas, {Openings.Length} vanos de {config.DoorWidth} m, "
                      + $"{wallSegments} muros, {propCount} props, {lightCount} focos ámbar, {volumes.Count} triggers de sala.");
        }

        // ---------------------------------------------------------------------
        // Cáscara: suelos, muros y vanos
        // ---------------------------------------------------------------------

        private static int BuildShell(Transform parent, GameConfig config)
        {
            var walls = new Dictionary<string, Wall>();

            foreach (RoomDef room in Rooms)
            {
                AddWallSpan(walls, true, room.MinX, room.MinZ, room.MaxZ);
                AddWallSpan(walls, true, room.MaxX, room.MinZ, room.MaxZ);
                AddWallSpan(walls, false, room.MinZ, room.MinX, room.MaxX);
                AddWallSpan(walls, false, room.MaxZ, room.MinX, room.MaxX);

                CreateBox("Suelo_" + room.Id, parent,
                    new Vector3(room.Center.x, -0.05f, room.Center.y),
                    new Vector3(room.Size.x, 0.1f, room.Size.y), true);
            }

            int segments = 0;
            float lintelHeight = config.WallHeight - config.DoorHeight;

            foreach (KeyValuePair<string, Wall> pair in walls)
            {
                Wall wall = pair.Value;
                List<Vector2> solids = MergeSpans(wall.Spans);

                var holes = new List<Vector2>();
                foreach (OpeningDef opening in Openings)
                {
                    float openingCoord = opening.AlongZ ? opening.Center.x : opening.Center.y;
                    if (opening.AlongZ != wall.AlongZ) continue;
                    if (Mathf.Abs(openingCoord - wall.Coord) > 0.01f) continue;

                    float mid = opening.AlongZ ? opening.Center.y : opening.Center.x;
                    holes.Add(new Vector2(mid - config.DoorWidth * 0.5f, mid + config.DoorWidth * 0.5f));
                }

                foreach (Vector2 span in SubtractSpans(solids, holes))
                {
                    CreateWallSegment(parent, wall, span, config);
                    segments++;
                }

                foreach (Vector2 hole in holes)
                {
                    float mid = (hole.x + hole.y) * 0.5f;
                    Vector3 center = wall.AlongZ
                        ? new Vector3(wall.Coord, config.DoorHeight + lintelHeight * 0.5f, mid)
                        : new Vector3(mid, config.DoorHeight + lintelHeight * 0.5f, wall.Coord);
                    Vector3 size = wall.AlongZ
                        ? new Vector3(config.WallThickness, lintelHeight, hole.y - hole.x)
                        : new Vector3(hole.y - hole.x, lintelHeight, config.WallThickness);

                    CreateBox("Dintel", parent, center, size, true);
                }
            }

            return segments;
        }

        private static void CreateWallSegment(Transform parent, Wall wall, Vector2 span, GameConfig config)
        {
            float length = span.y - span.x;
            if (length <= 0.01f) return;

            Vector3 center = wall.AlongZ
                ? new Vector3(wall.Coord, config.WallHeight * 0.5f, (span.x + span.y) * 0.5f)
                : new Vector3((span.x + span.y) * 0.5f, config.WallHeight * 0.5f, wall.Coord);
            Vector3 size = wall.AlongZ
                ? new Vector3(config.WallThickness, config.WallHeight, length)
                : new Vector3(length, config.WallHeight, config.WallThickness);

            CreateBox("Muro", parent, center, size, true);
        }

        private static void AddWallSpan(Dictionary<string, Wall> walls, bool alongZ, float coord, float a, float b)
        {
            string key = WallKey(alongZ, coord);
            if (!walls.TryGetValue(key, out Wall wall))
            {
                wall = new Wall { AlongZ = alongZ, Coord = coord };
                walls.Add(key, wall);
            }
            wall.Spans.Add(new Vector2(Mathf.Min(a, b), Mathf.Max(a, b)));
        }

        private static string WallKey(bool alongZ, float coord)
        {
            return (alongZ ? "Z:" : "X:") + (Mathf.Round(coord * 100f) / 100f).ToString("F2");
        }

        private static List<Vector2> MergeSpans(List<Vector2> spans)
        {
            var sorted = new List<Vector2>(spans);
            sorted.Sort((a, b) => a.x.CompareTo(b.x));

            var merged = new List<Vector2>();
            foreach (Vector2 span in sorted)
            {
                if (merged.Count == 0)
                {
                    merged.Add(span);
                    continue;
                }

                Vector2 last = merged[merged.Count - 1];
                if (span.x <= last.y + 0.001f) merged[merged.Count - 1] = new Vector2(last.x, Mathf.Max(last.y, span.y));
                else merged.Add(span);
            }
            return merged;
        }

        private static List<Vector2> SubtractSpans(List<Vector2> solids, List<Vector2> holes)
        {
            List<Vector2> result = solids;

            foreach (Vector2 hole in holes)
            {
                var next = new List<Vector2>();
                foreach (Vector2 span in result)
                {
                    if (hole.y <= span.x || hole.x >= span.y)
                    {
                        next.Add(span);
                        continue;
                    }
                    if (hole.x > span.x) next.Add(new Vector2(span.x, hole.x));
                    if (hole.y < span.y) next.Add(new Vector2(hole.y, span.y));
                }
                result = next;
            }
            return result;
        }

        // ---------------------------------------------------------------------
        // Props
        // ---------------------------------------------------------------------

        private static int BuildProps(Transform parent, GameConfig config, Transform doorParent)
        {
            int count = 0;

            // --- FilaCapsulas: 7 cápsulas verticales, bandeja de efectos personales ---
            RoomDef capsules = FindRoom("FilaCapsulas");
            float firstX = capsules.Center.x - config.CapsuleSpacing * 3f;
            for (int i = 0; i < 7; i++)
            {
                CreatePrimitive("Capsula_" + (i + 1), parent, PrimitiveType.Capsule,
                    new Vector3(firstX + i * config.CapsuleSpacing, 1f, capsules.MaxZ - 0.8f),
                    new Vector3(0.8f, 1f, 0.8f));
                count++;
            }

            CreateBox("Pedestal_Bandeja", parent,
                new Vector3(capsules.MinX + 1.5f, 0.5f, capsules.Center.y + 0.5f),
                new Vector3(0.6f, 1f, 0.5f), false);
            CreateBox("Bandeja_EfectosPersonales", parent,
                new Vector3(capsules.MinX + 1.5f, 1.05f, capsules.Center.y + 0.5f),
                new Vector3(0.5f, 0.1f, 0.35f), false);
            count += 2;

            // --- Pasillo: 2 coberturas ---
            CreateBox("Cobertura_1", parent, new Vector3(12f, 0.6f, 0.9f), new Vector3(2f, 1.2f, 1.2f), false);
            CreateBox("Cobertura_2", parent, new Vector3(24f, 0.6f, -0.9f), new Vector3(2f, 1.2f, 1.2f), false);
            count += 2;

            // --- Almacen: 3 estanterías que bloquean la visión ---
            RoomDef storage = FindRoom("Almacen");
            for (int i = 0; i < 3; i++)
            {
                CreateBox("Estanteria_" + (i + 1), parent,
                    new Vector3(storage.MinX + 2.5f + i * 3.5f, 1.1f, storage.Center.y),
                    new Vector3(0.6f, 2.2f, 9f), false);
                count++;
            }

            // --- Cámaras de vigilancia (SalaVigilancia, SalaClaveSecundaria, SalaClaveFinal) ---
            count += CreateSecurityCamera(parent, "SalaVigilancia", new Vector3(57.2f, 2.7f, 14.2f));
            count += CreateSecurityCamera(parent, "SalaClaveSecundaria", new Vector3(57.2f, 2.7f, 4.2f));
            count += CreateSecurityCamera(parent, "SalaClaveFinal", new Vector3(71.2f, 2.7f, 5.2f));

            // --- SalaClaveSecundaria: llave obligatoria sobre pedestal ---
            CreateBox("Pedestal_Llave", parent, new Vector3(51f, 0.45f, -3f), new Vector3(0.4f, 0.9f, 0.4f), false);
            CreateBox("Llave_Obligatoria", parent, new Vector3(51f, 0.95f, -3f), new Vector3(0.15f, 0.05f, 0.3f), false);
            count += 2;

            // --- SalaClaveFinal: rampa (escalera) al fondo + rellano ---
            GameObject ramp = CreateBox("Rampa_Escalera", parent, new Vector3(68.3f, 0.75f, 0f), new Vector3(4.6f, 0.3f, 4f), true);
            ramp.transform.rotation = Quaternion.Euler(0f, 0f, 20f);
            CreateBox("Rellano_Escalera", parent, new Vector3(71.2f, 1.55f, 0f), new Vector3(1.5f, 0.3f, 4f), true);
            count += 2;

            // --- Puerta bloqueada de SalaClaveFinal ---
            CreateLockedDoor(doorParent);
            count++;

            return count;
        }

        private static int CreateSecurityCamera(Transform parent, string roomId, Vector3 position)
        {
            RoomDef room = FindRoom(roomId);
            GameObject camera = CreateBox("CamaraVigilancia_" + roomId, parent, position, new Vector3(0.4f, 0.4f, 0.4f), false);

            Vector3 target = new Vector3(room.Center.x, 1.3f, room.Center.y);
            Vector3 direction = target - position;
            if (direction.sqrMagnitude > 0.001f)
            {
                camera.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
            return 1;
        }

        private static void CreateLockedDoor(Transform parent)
        {
            // Vano de 2 m en x = 58, z ∈ [-1, 1]: bisagra en el borde sur, hoja de 2 m.
            var hinge = new GameObject("Puerta_SalaClaveFinal");
            hinge.transform.SetParent(parent, false);
            hinge.transform.position = new Vector3(58f, 0f, -1f);

            GameObject leaf = CreateBox("Hoja", parent, new Vector3(58f, 1.05f, 0f), new Vector3(0.15f, 2.1f, 2f), false);
            leaf.transform.SetParent(hinge.transform, true);
            leaf.name = "Hoja";

            Door door = hinge.AddComponent<Door>();
            door.Unlock();
            door.Lock();
        }

        // ---------------------------------------------------------------------
        // Luces y volúmenes de sala
        // ---------------------------------------------------------------------

        private static int BuildLights(Transform parent, GameConfig config)
        {
            int count = 0;

            foreach (RoomDef room in Rooms)
            {
                foreach (Vector3 anchor in LightAnchors(room))
                {
                    var go = new GameObject("Luz_" + room.Id);
                    go.transform.SetParent(parent, false);
                    go.transform.position = new Vector3(anchor.x, config.EmergencyLightHeight, anchor.z);
                    go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                    Light light = go.AddComponent<Light>();
                    light.type = LightType.Spot;
                    light.color = config.EmergencyLightColor;
                    light.intensity = config.EmergencyLightIntensity;
                    light.range = config.EmergencyLightRange;
                    light.spotAngle = config.EmergencyLightAngle;
                    light.shadows = config.EmergencyLightShadows ? LightShadows.Soft : LightShadows.None;
                    count++;
                }
            }

            return count;
        }

        private static Vector3[] LightAnchors(RoomDef room)
        {
            switch (room.Id)
            {
                case "Pasillo":
                    return new[] { new Vector3(13f, 0f, 0f), new Vector3(25f, 0f, 0f) };
                case "SalaAbierta":
                    return new[] { new Vector3(36f, 0f, 0f), new Vector3(44f, 0f, 0f) };
                default:
                    return new[] { new Vector3(room.Center.x, 0f, room.Center.y) };
            }
        }

        private static Dictionary<string, RoomVolume> BuildRoomVolumes(Transform parent, GameConfig config)
        {
            var volumes = new Dictionary<string, RoomVolume>();

            foreach (RoomDef room in Rooms)
            {
                var go = new GameObject(room.Id);
                go.transform.SetParent(parent, false);
                go.transform.position = new Vector3(room.Center.x, 0f, room.Center.y);
                go.layer = 2;

                var box = go.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.center = new Vector3(0f, config.WallHeight * 0.5f, 0f);
                box.size = new Vector3(room.Size.x, config.WallHeight, room.Size.y);

                RoomVolume volume = go.AddComponent<RoomVolume>();
                volume.Configure(room.Id, room.Safe, room.Adjacent, new RoomVolume[0]);
                volumes.Add(room.Id, volume);
            }

            return volumes;
        }

        private static void WireAdjacency(Dictionary<string, RoomVolume> volumes)
        {
            foreach (RoomDef room in Rooms)
            {
                var neighbours = new List<RoomVolume>();
                foreach (string id in room.Adjacent)
                {
                    if (volumes.TryGetValue(id, out RoomVolume neighbour)) neighbours.Add(neighbour);
                    else Debug.LogWarning($"[Graybox] Adyacencia desconocida: {room.Id} -> {id}");
                }

                RoomVolume volume = volumes[room.Id];
                volume.Configure(room.Id, room.Safe, room.Adjacent, neighbours.ToArray());
                EditorUtility.SetDirty(volume);
            }
        }

        private static void CreateSpawnPoint(Transform root)
        {
            var spawn = new GameObject("PlayerSpawn");
            spawn.transform.SetParent(root, false);
            spawn.transform.position = new Vector3(-4.5f, 0.05f, -1.5f);
            spawn.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        // ---------------------------------------------------------------------
        // Escena, entorno y utilidades
        // ---------------------------------------------------------------------

        private static void ConfigureEnvironment(GameConfig config)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = config.AmbientColor;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.skybox = null;
            RenderSettings.fog = false;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        }

        private static void ConfigureCamera()
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.farClipPlane = 400f;
            camera.transform.SetPositionAndRotation(new Vector3(33f, 46f, -36f), Quaternion.Euler(52f, 0f, 0f));
            camera.name = "Main Camera";
        }

        private static void RemoveDirectionalLights()
        {
            foreach (Light light in Object.FindObjectsByType<Light>())
            {
                if (light.type == LightType.Directional) Object.DestroyImmediate(light.gameObject);
            }
        }

        private static void FrameSceneView()
        {
            SceneView view = SceneView.lastActiveSceneView;
            if (view == null) return;

            view.orthographic = false;
            view.pivot = new Vector3(33f, 0f, 6f);
            view.rotation = Quaternion.Euler(55f, 0f, 0f);
            view.size = 42f;
            view.Repaint();
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static GameObject CreateBox(string name, Transform parent, Vector3 worldPosition, Vector3 size, bool markStatic)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = worldPosition;
            go.transform.localScale = size;

            if (markStatic)
            {
                GameObjectUtility.SetStaticEditorFlags(go,
                    StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            }

            return go;
        }

        private static GameObject CreatePrimitive(string name, Transform parent, PrimitiveType type, Vector3 worldPosition, Vector3 scale)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = worldPosition;
            go.transform.localScale = scale;
            return go;
        }

        private static RoomDef FindRoom(string id)
        {
            foreach (RoomDef room in Rooms)
            {
                if (room.Id == id) return room;
            }
            return null;
        }

        private static GameConfig LoadOrCreateConfig()
        {
            GameConfig config = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
            if (config != null) return config;

            if (!AssetDatabase.IsValidFolder(ConfigFolder)) AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

            config = ScriptableObject.CreateInstance<GameConfig>();
            AssetDatabase.CreateAsset(config, ConfigPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Graybox] GameConfig creado en {ConfigPath}");
            return config;
        }
    }
}
