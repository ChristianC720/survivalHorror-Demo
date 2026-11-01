using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Remake del graybox VS_Piso1: rehace suelos, muros y vanos con salas más grandes
/// (módulos de 2 m) y recoloca props, puertas, paneles, luces, triggers, enemigos e ítems.
/// NO borra jugador, HUD, enemigos, EnemyDirector, paneles, puertas ni ítems: solo los mueve.
/// </summary>
public static class RemakeGrayboxLevel
{
    private const string ConfigPath = "Assets/ScriptableObjects/GameConfig.asset";
    private const string RootName = "VS_Piso1";

    // El almacén se pide oscuro: se atenúa su foco respecto a la cifra común de GameConfig.
    private const float StorageLightFactor = 0.4f;

    private sealed class Room
    {
        public readonly string Id;
        public readonly Vector2 Center;
        public readonly Vector2 Size;

        public Room(string id, float cx, float cz, float sx, float sz)
        {
            Id = id;
            Center = new Vector2(cx, cz);
            Size = new Vector2(sx, sz);
        }

        public float MinX => Center.x - Size.x * 0.5f;
        public float MaxX => Center.x + Size.x * 0.5f;
        public float MinZ => Center.y - Size.y * 0.5f;
        public float MaxZ => Center.y + Size.y * 0.5f;
    }

    private sealed class Opening
    {
        public readonly Vector2 Center;
        public readonly bool AlongZ;

        public Opening(float x, float z, bool alongZ)
        {
            Center = new Vector2(x, z);
            AlongZ = alongZ;
        }
    }

    private sealed class WallSpec
    {
        public bool AlongZ;
        public float Coord;
        public readonly List<Vector2> Spans = new List<Vector2>();
    }

    // ---------------------------------------------------------------------
    // Planta nueva. Módulos de 2 m. Suelo en y = 0. Sin azar.
    // ---------------------------------------------------------------------
    private static readonly Room[] Rooms =
    {
        new Room("FilaCapsulas", 0f, 0f, 16f, 10f),
        new Room("Pasillo", 24f, 0f, 32f, 4f),
        new Room("Almacen", 16f, 9f, 16f, 14f),
        new Room("SalaAbierta", 51f, 0f, 22f, 20f),
        new Room("SalaClaveSecundaria", 69f, 0f, 14f, 12f),
        new Room("SalaVigilancia", 69f, 12f, 14f, 12f),
        new Room("SalaClaveFinal", 85f, 0f, 18f, 14f),
    };

    private static readonly Opening[] Openings =
    {
        new Opening(8f, 0f, true),    // FilaCapsulas <-> Pasillo
        new Opening(16f, 2f, false),  // Pasillo <-> Almacen
        new Opening(40f, 0f, true),   // Pasillo <-> SalaAbierta
        new Opening(62f, 0f, true),   // SalaAbierta <-> SalaClaveSecundaria
        new Opening(69f, 6f, false),  // SalaClaveSecundaria <-> SalaVigilancia
        new Opening(76f, 0f, true),   // SalaClaveSecundaria <-> SalaClaveFinal
    };

    private static readonly string[] RebuildableProps =
    {
        "Capsula_1", "Capsula_2", "Capsula_3", "Capsula_4", "Capsula_5", "Capsula_6", "Capsula_7",
        "Pedestal_Bandeja", "Bandeja_EfectosPersonales",
        "Cobertura_1", "Cobertura_2",
        "Estanteria_1", "Estanteria_2", "Estanteria_3",
        "CamaraVigilancia_SalaVigilancia", "CamaraVigilancia_SalaClaveSecundaria", "CamaraVigilancia_SalaClaveFinal",
        "Pedestal_Llave",
        "Rampa_Escalera", "Rellano_Escalera",
    };

    public static object Execute()
    {
        var log = new StringBuilder();

        try
        {
            var config = AssetDatabase.LoadAssetAtPath<Heartbeat.Config.GameConfig>(ConfigPath);
            if (config == null) throw new Exception($"No se encontró {ConfigPath}.");

            GameObject rootObject = GameObject.Find(RootName);
            if (rootObject == null) throw new Exception($"No se encontró '{RootName}' en la escena.");

            Transform root = rootObject.transform;
            Transform geometry = root.Find("Geometry");
            Transform props = root.Find("Props");
            Transform lights = root.Find("Lights");
            Transform doors = root.Find("Doors");
            Transform panels = root.Find("Panels");
            Transform triggers = root.Find("Triggers");

            log.AppendLine($"[Remake] config: muro {config.WallHeight} m, grosor {config.WallThickness} m, vano {config.DoorWidth} x {config.DoorHeight} m");

            // ---------------- 1. Cáscara ----------------
            int removed = ClearAll(geometry);
            int floors = 0, segments = 0, lintels = 0;
            BuildShell(geometry, config, ref floors, ref segments, ref lintels);
            log.AppendLine($"[Remake] Geometry: {removed} objetos borrados, {floors} suelos, {segments} muros, {lintels} dinteles nuevos");

            // ---------------- 2. Props decorativos ----------------
            int removedProps = RemoveProps(props, RebuildableProps, log);
            int newProps = BuildProps(props, config);
            log.AppendLine($"[Remake] Props decorativos: {removedProps} borrados, {newProps} creados");

            // ---------------- 3. Puertas y paneles ----------------
            MoveDoor(doors, "Puerta_SalaClaveFinal", new Vector3(76f, 0f, -1f), log);
            MoveDoor(doors, "Puerta_SalaClaveSecundaria_Oeste", new Vector3(62f, 0f, -1f), log);
            MoveDoor(doors, "Puerta_SalaVigilancia", new Vector3(68f, 0f, 6f), log);
            MoveDoor(doors, "Puerta_Almacen", new Vector3(15f, 0f, 2f), log);

            MovePanel(panels, "Panel_SalaClaveSecundaria", new Vector3(62.21f, 1.45f, -1.6f), log);
            MovePanel(panels, "Panel_SalaClaveFinal", new Vector3(75.79f, 1.45f, -1.6f), log);
            MovePanel(panels, "Panel_SalaVigilancia", new Vector3(67.4f, 1.45f, 5.79f), log);
            MovePanel(panels, "Panel_Almacen", new Vector3(14.4f, 1.45f, 1.79f), log);

            // ---------------- 4. Luces ----------------
            MoveLights(lights, config, log);

            // ---------------- 5. Triggers de sala ----------------
            ResizeTriggers(triggers, config, log);

            // ---------------- 6. Gameplay móvil ----------------
            MoveEnemies(log);
            MoveItems(props, log);
            MovePlayer(root, log);

            // ---------------- 7. NavMesh ----------------
            string navMeshResult = BakeNavMesh();

            // ---------------- 8. Guardar ----------------
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            bool saved = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            log.AppendLine($"[Remake] NavMesh: {navMeshResult}");
            log.AppendLine($"[Remake] escena guardada={saved}");
        }
        catch (Exception e)
        {
            log.AppendLine("EXCEPCION: " + (e.InnerException != null ? e.InnerException.ToString() : e.ToString()));
        }

        return log.ToString();
    }

    // ---------------------------------------------------------------------
    // Cáscara
    // ---------------------------------------------------------------------

    private static void BuildShell(Transform parent, Heartbeat.Config.GameConfig config, ref int floors, ref int segments, ref int lintels)
    {
        var walls = new Dictionary<string, WallSpec>();

        foreach (Room room in Rooms)
        {
            AddWallSpan(walls, true, room.MinX, room.MinZ, room.MaxZ);
            AddWallSpan(walls, true, room.MaxX, room.MinZ, room.MaxZ);
            AddWallSpan(walls, false, room.MinZ, room.MinX, room.MaxX);
            AddWallSpan(walls, false, room.MaxZ, room.MinX, room.MaxX);

            CreateBox("Suelo_" + room.Id, parent,
                new Vector3(room.Center.x, -0.05f, room.Center.y),
                new Vector3(room.Size.x, 0.1f, room.Size.y), true);
            floors++;
        }

        float lintelHeight = config.WallHeight - config.DoorHeight;

        foreach (KeyValuePair<string, WallSpec> pair in walls)
        {
            WallSpec wall = pair.Value;
            List<Vector2> solids = MergeSpans(wall.Spans);

            var holes = new List<Vector2>();
            foreach (Opening opening in Openings)
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
                lintels++;
            }
        }
    }

    private static void CreateWallSegment(Transform parent, WallSpec wall, Vector2 span, Heartbeat.Config.GameConfig config)
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

    private static void AddWallSpan(Dictionary<string, WallSpec> walls, bool alongZ, float coord, float a, float b)
    {
        string key = (alongZ ? "Z:" : "X:") + (Mathf.Round(coord * 100f) / 100f).ToString("F2");
        if (!walls.TryGetValue(key, out WallSpec wall))
        {
            wall = new WallSpec { AlongZ = alongZ, Coord = coord };
            walls.Add(key, wall);
        }
        wall.Spans.Add(new Vector2(Mathf.Min(a, b), Mathf.Max(a, b)));
    }

    private static List<Vector2> MergeSpans(List<Vector2> spans)
    {
        var sorted = new List<Vector2>(spans);
        sorted.Sort((a, b) => a.x.CompareTo(b.x));

        var merged = new List<Vector2>();
        foreach (Vector2 span in sorted)
        {
            if (merged.Count == 0) { merged.Add(span); continue; }

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
                if (hole.y <= span.x || hole.x >= span.y) { next.Add(span); continue; }
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

    private static int BuildProps(Transform parent, Heartbeat.Config.GameConfig config)
    {
        int count = 0;

        // --- FilaCapsulas: 7 cápsulas verticales contra el muro norte + bandeja ---
        Room capsules = FindRoom("FilaCapsulas");
        float firstX = capsules.Center.x - config.CapsuleSpacing * 3f;
        for (int i = 0; i < 7; i++)
        {
            CreatePrimitive("Capsula_" + (i + 1), parent, PrimitiveType.Capsule,
                new Vector3(firstX + i * config.CapsuleSpacing, 1f, capsules.MaxZ - 0.9f),
                new Vector3(0.8f, 1f, 0.8f));
            count++;
        }

        float trayX = capsules.MinX + 1.5f;
        CreateBox("Pedestal_Bandeja", parent, new Vector3(trayX, 0.5f, capsules.Center.y + 0.5f),
            new Vector3(0.6f, 1f, 0.5f), false);
        CreateBox("Bandeja_EfectosPersonales", parent, new Vector3(trayX, 1.05f, capsules.Center.y + 0.5f),
            new Vector3(0.5f, 0.1f, 0.35f), false);
        count += 2;

        // --- Pasillo: 2 coberturas, separadas de los tres vanos (x = 8, 16 y 40) ---
        CreateBox("Cobertura_1", parent, new Vector3(22f, 0.6f, 1f), new Vector3(2f, 1.2f, 1.2f), false);
        CreateBox("Cobertura_2", parent, new Vector3(32f, 0.6f, -1f), new Vector3(2f, 1.2f, 1.2f), false);
        count += 2;

        // --- Almacen: 3 estanterías altas que cortan la visión, sin tapar el vano (x 15-17) ---
        var shelfXs = new[] { 10.5f, 14f, 20.5f };
        for (int i = 0; i < shelfXs.Length; i++)
        {
            CreateBox("Estanteria_" + (i + 1), parent, new Vector3(shelfXs[i], 1.1f, 9f),
                new Vector3(0.6f, 2.2f, 10f), false);
            count++;
        }

        // --- Cámaras de vigilancia: esquina alta de cada sala vigilada ---
        CreateSecurityCamera(parent, "SalaVigilancia", new Vector3(75.2f, 2.7f, 17.2f));
        CreateSecurityCamera(parent, "SalaClaveSecundaria", new Vector3(75.2f, 2.7f, 5.2f));
        CreateSecurityCamera(parent, "SalaClaveFinal", new Vector3(93.2f, 2.7f, 6.2f));
        count += 3;

        // --- SalaClaveSecundaria: pedestal de la llave obligatoria ---
        CreateBox("Pedestal_Llave", parent, new Vector3(66.2f, 0.45f, -3.6f), new Vector3(0.4f, 0.9f, 0.4f), false);
        count++;

        // --- SalaClaveFinal: rampa (escalera) al fondo + rellano ---
        GameObject ramp = CreateBox("Rampa_Escalera", parent, new Vector3(90.3f, 0.75f, 0f),
            new Vector3(4.6f, 0.3f, 4f), true);
        ramp.transform.rotation = Quaternion.Euler(0f, 0f, 20f);
        CreateBox("Rellano_Escalera", parent, new Vector3(93.2f, 1.55f, 0f), new Vector3(1.5f, 0.3f, 4f), true);
        count += 2;

        return count;
    }

    private static void CreateSecurityCamera(Transform parent, string roomId, Vector3 position)
    {
        GameObject camera = CreateBox("CamaraVigilancia_" + roomId, parent, position, new Vector3(0.4f, 0.4f, 0.4f), false);

        Room room = FindRoom(roomId);
        Vector3 target = new Vector3(room.Center.x, 1.3f, room.Center.y);
        Vector3 direction = target - position;
        if (direction.sqrMagnitude > 0.001f)
        {
            camera.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
    }

    private static int RemoveProps(Transform props, string[] names, StringBuilder log)
    {
        int removed = 0;
        foreach (string name in names)
        {
            Transform target = props.Find(name);
            if (target == null) continue;

            int components = target.GetComponents<Component>().Length;
            if (components > 4) log.AppendLine($"  AVISO: '{name}' tenía {components} componentes; se rehace sin ellos.");

            UnityEngine.Object.DestroyImmediate(target.gameObject);
            removed++;
        }
        return removed;
    }

    // ---------------------------------------------------------------------
    // Puertas, paneles, luces, triggers
    // ---------------------------------------------------------------------

    private static void MoveDoor(Transform doors, string name, Vector3 hingePosition, StringBuilder log)
    {
        Transform hinge = doors.Find(name);
        if (hinge == null) { log.AppendLine($"  falta puerta '{name}'"); return; }

        hinge.position = hingePosition;
        Transform leaf = hinge.Find("Hoja");
        if (leaf == null) log.AppendLine($"  AVISO: '{name}' no tiene hijo Hoja");
    }

    private static void MovePanel(Transform panels, string name, Vector3 position, StringBuilder log)
    {
        Transform panel = panels.Find(name);
        if (panel == null) { log.AppendLine($"  falta panel '{name}'"); return; }
        panel.position = position;
    }

    private static void MoveLights(Transform lights, Heartbeat.Config.GameConfig config, StringBuilder log)
    {
        var anchors = new Dictionary<string, Vector3[]>
        {
            { "FilaCapsulas", new[] { new Vector3(0f, 0f, 0f) } },
            { "Pasillo", new[] { new Vector3(16f, 0f, 0f), new Vector3(32f, 0f, 0f) } },
            { "Almacen", new[] { new Vector3(16f, 0f, 9f) } },
            { "SalaAbierta", new[] { new Vector3(46f, 0f, 0f), new Vector3(56f, 0f, 0f) } },
            { "SalaVigilancia", new[] { new Vector3(69f, 0f, 12f) } },
            { "SalaClaveSecundaria", new[] { new Vector3(69f, 0f, 0f) } },
            { "SalaClaveFinal", new[] { new Vector3(85f, 0f, 0f) } },
        };

        var used = new Dictionary<string, int>();
        for (int i = 0; i < lights.childCount; i++)
        {
            Transform lightTransform = lights.GetChild(i);
            string key = lightTransform.name.StartsWith("Luz_") ? lightTransform.name.Substring(4) : lightTransform.name;
            if (!anchors.TryGetValue(key, out Vector3[] list)) { log.AppendLine($"  luz sin ancla: {lightTransform.name}"); continue; }

            used.TryGetValue(key, out int index);
            if (index >= list.Length) { log.AppendLine($"  luz sobrante: {lightTransform.name}"); continue; }
            used[key] = index + 1;

            Vector3 anchor = list[index];
            lightTransform.position = new Vector3(anchor.x, config.EmergencyLightHeight, anchor.z);

            Light light = lightTransform.GetComponent<Light>();
            if (light != null && key == "Almacen")
            {
                light.intensity = config.EmergencyLightIntensity * StorageLightFactor;
            }
        }
    }

    private static void ResizeTriggers(Transform triggers, Heartbeat.Config.GameConfig config, StringBuilder log)
    {
        for (int i = 0; i < triggers.childCount; i++)
        {
            Transform trigger = triggers.GetChild(i);
            Room room = FindRoom(trigger.name);
            if (room == null) { log.AppendLine($"  trigger sin sala: {trigger.name}"); continue; }

            trigger.position = new Vector3(room.Center.x, 0f, room.Center.y);

            var box = trigger.GetComponent<BoxCollider>();
            if (box == null) { log.AppendLine($"  AVISO: trigger '{trigger.name}' sin BoxCollider"); continue; }

            box.isTrigger = true;
            box.center = new Vector3(0f, config.WallHeight * 0.5f, 0f);
            box.size = new Vector3(room.Size.x, config.WallHeight, room.Size.y);
        }
    }

    // ---------------------------------------------------------------------
    // Gameplay: enemigos, objetos, jugador
    // ---------------------------------------------------------------------

    private static void MoveEnemies(StringBuilder log)
    {
        SetPosition("Enemies/Enemy_Rastrero_Almacen", new Vector3(19f, 0.05f, 6f), log);
        SetPosition("Enemies/Enemy_Rastrero_SalaAbierta", new Vector3(52f, 0.05f, 2f), log);
        SetPosition("Enemies/Enemy_Alto_Pasillo", new Vector3(22f, 0.05f, -1f), log);
        SetPosition("Enemies/Enemy_Alto_SalaClaveFinal", new Vector3(83f, 0.05f, 0.5f), log);
    }

    private static void MoveItems(Transform props, StringBuilder log)
    {
        SetChildPosition(props, "Objeto_Cura_1", new Vector3(-2.7f, 0.25f, -1.9f), log);
        SetChildPosition(props, "Objeto_Cura_2", new Vector3(45.5f, 0.25f, 3.8f), log);
        SetChildPosition(props, "Objeto_Municion_1", new Vector3(13f, 0.25f, -1.3f), log);
        SetChildPosition(props, "Objeto_Municion_2", new Vector3(25.5f, 0.25f, 1.3f), log);
        SetChildPosition(props, "Objeto_Municion_3", new Vector3(13.3f, 0.25f, 5f), log);
        SetChildPosition(props, "Objeto_Municion_4", new Vector3(56.5f, 0.25f, -5f), log);
        SetChildPosition(props, "Objeto_Punzon_1", new Vector3(18.5f, 0.25f, 5.5f), log);
        SetChildPosition(props, "Objeto_Punzon_2", new Vector3(69f, 0.25f, 12f), log);
        SetChildPosition(props, "Llave_Obligatoria", new Vector3(66.2f, 0.95f, -3.6f), log);
        SetChildPosition(props, "PruebaIluminacion", new Vector3(10f, 1f, 1.2f), log);
    }

    private static void MovePlayer(Transform root, StringBuilder log)
    {
        Transform spawn = root.Find("PlayerSpawn");
        if (spawn != null) spawn.position = new Vector3(-4.5f, 0.05f, -1.5f);

        SetPosition("Player", new Vector3(-4.5f, 0.05f, -1.5f), log);
    }

    private static void SetPosition(string path, Vector3 position, StringBuilder log)
    {
        GameObject target = GameObject.Find(path);
        if (target == null) { log.AppendLine($"  falta '{path}'"); return; }
        target.transform.position = position;
    }

    private static void SetChildPosition(Transform parent, string name, Vector3 position, StringBuilder log)
    {
        Transform target = parent.Find(name);
        if (target == null) { log.AppendLine($"  falta prop '{name}'"); return; }
        target.position = position;
    }

    // ---------------------------------------------------------------------
    // NavMesh
    // ---------------------------------------------------------------------

    private static string BakeNavMesh()
    {
        Type tool = Type.GetType("Heartbeat.EditorTools.NavMeshBakeTool, Assembly-CSharp-Editor");
        if (tool == null) return "no se encontró NavMeshBakeTool";

        MethodInfo bake = tool.GetMethod("BakeActiveScene", BindingFlags.Public | BindingFlags.Static);
        if (bake == null) return "NavMeshBakeTool sin BakeActiveScene";

        bake.Invoke(null, null);

        var surface = GameObject.Find(RootName)?.GetComponent<Unity.AI.Navigation.NavMeshSurface>();
        if (surface == null) return "sin NavMeshSurface";

        return surface.navMeshData != null
            ? $"horneado '{surface.navMeshData.name}' center={surface.center} size={surface.size}"
            : "sin datos de NavMesh";
    }

    // ---------------------------------------------------------------------
    // Utilidades
    // ---------------------------------------------------------------------

    private static int ClearAll(Transform group)
    {
        int count = group.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(group.GetChild(i).gameObject);
        }
        return count;
    }

    private static Room FindRoom(string id)
    {
        foreach (Room room in Rooms)
        {
            if (room.Id == id) return room;
        }
        return null;
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

    private static void CreatePrimitive(string name, Transform parent, PrimitiveType type, Vector3 worldPosition, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = worldPosition;
        go.transform.localScale = scale;
    }
}
