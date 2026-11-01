using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartbeat.Generation;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Reads every hand-built room prefab under Prefabs/Rooms, validates it against Desarrollo/KIT_SPEC.md §3-4 and
    /// writes its RoomDefinition (ScriptableObjects/Rooms/RD_name) plus the RoomLibrary. Never creates geometry.
    /// Naming drives metadata: ROOM_F1_Storage_A, KEY_F1_Secondary_A, KEY_F1_Final, KEY_F1_CapsuleRow, TUT_F1_2_Storage.
    /// </summary>
    public static class RoomDefinitionRefresher
    {
        const string PrefabDir = "Assets/_Project/Prefabs/Rooms";
        const string DefDir = "Assets/_Project/ScriptableObjects/Rooms";

        [MenuItem("Heartbeat/Rooms/Refresh Definitions")]
        public static void RefreshMenu() => Debug.Log(RefreshAll());

        public static string RefreshAll()
        {
            var sb = new StringBuilder();
            var defs = new List<RoomDefinition>();
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabDir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var issues = new List<string>();
                var def = Refresh(prefab, issues);
                if (def != null) defs.Add(def);
                sb.AppendLine($"{prefab.name}: {(issues.Count == 0 ? "OK" : string.Join("; ", issues))}" +
                              (def != null ? $" | {def.size.x}x{def.size.y} cells, {def.sockets.Count} sockets, {def.enemySpawns} enemy / {def.itemSpawns} item spawns" : ""));
            }

            string libPath = DefDir + "/RoomLibrary.asset";
            var lib = AssetDatabase.LoadAssetAtPath<RoomLibrary>(libPath);
            if (lib == null) { lib = ScriptableObject.CreateInstance<RoomLibrary>(); AssetDatabase.CreateAsset(lib, libPath); }
            lib.rooms = defs.OrderBy(d => d.name).ToList();
            EditorUtility.SetDirty(lib);
            AssetDatabase.SaveAssets();
            sb.AppendLine($"RoomLibrary: {defs.Count} rooms");
            return sb.ToString();
        }

        public static RoomDefinition Refresh(GameObject prefab, List<string> issues)
        {
            var root = prefab.transform;
            var inst = prefab.GetComponent<RoomInstance>();
            if (inst == null) { issues.Add("missing RoomInstance on root"); return null; }
            if ((root.localScale - Vector3.one).sqrMagnitude > 0.0001f) issues.Add("root scale is not 1");

            // Size comes from the RoomShell (the shell itself is assembled at runtime from the kit).
            var shell = prefab.GetComponent<RoomShell>();
            if (shell == null) { issues.Add("missing RoomShell on root"); return null; }
            if (shell.kit == null) issues.Add("RoomShell has no KitSet");
            var size = shell.size;
            if (size.x < 1 || size.y < 1 || size.x > 8 || size.y > 8) issues.Add($"size {size} outside 1..8 cells");
            if (shell.Built != null) issues.Add("prefab contains a built Shell (clear it before saving)");

            // Sockets
            var sockets = new List<SocketData>();
            var facings = new HashSet<Facing>();
            foreach (var s in prefab.GetComponentsInChildren<DoorSocket>(true))
            {
                var facing = s.LocalFacing(root);
                var cell = s.LocalCell(root, size);
                Vector3 p = root.InverseTransformPoint(s.transform.position);
                Vector3 expected = SocketPoint(cell, facing, size);
                if ((p - expected).sqrMagnitude > 0.01f) issues.Add($"socket {s.name} at ({p.x:0.##},{p.z:0.##}) should be at ({expected.x},{expected.z})");
                sockets.Add(new SocketData { cell = cell, facing = facing, allowLocked = s.allowLocked });
                facings.Add(facing);
            }
            if (sockets.Count < 2 && !prefab.name.Contains("Mother") && !prefab.name.Contains("Final")) issues.Add("needs >= 2 sockets");
            if (facings.Count < 2 && sockets.Count >= 2) issues.Add("sockets must be on different edges");

            // Spawns
            var spawns = prefab.GetComponentsInChildren<SpawnPoint>(true);
            int enemies = spawns.Count(s => s.kind == SpawnKind.Enemy);
            int items = spawns.Count(s => s.kind == SpawnKind.Item);
            if (enemies > 2) issues.Add("more than 2 enemy spawns (D-37)");
            foreach (var e in spawns.Where(s => s.kind == SpawnKind.Enemy))
                foreach (var s in prefab.GetComponentsInChildren<DoorSocket>(true))
                    if (Vector3.Distance(e.transform.position, s.transform.position) < 6f) { issues.Add($"enemy spawn {e.name} < 6 m from {s.name}"); break; }

            if (prefab.GetComponent<BoxCollider>() is BoxCollider bc && !bc.isTrigger) issues.Add("RoomVolume collider must be a trigger");
            if (prefab.GetComponent<BoxCollider>() == null) issues.Add("missing RoomVolume BoxCollider trigger on root");

            // Metadata from the name
            string n = prefab.name;
            var def = LoadOrCreate("RD_" + n);
            def.id = n;
            def.prefab = prefab;
            def.size = size;
            def.sockets = sockets;
            def.enemySpawns = enemies;
            def.itemSpawns = items;
            def.itemSpawnTags = spawns.Where(s => s.kind == SpawnKind.Item).Select(s => s.itemTag).ToList();
            def.hasCamera = inst.CameraMount != null || n.StartsWith("KEY_");
            def.floor = n.Contains("_F3_") ? 3 : n.Contains("_F2_") ? 2 : 1;
            def.role = n.StartsWith("TUT_") ? RoomRole.Tutorial
                : n.Contains("CapsuleRow") ? RoomRole.CapsuleRow
                : n.Contains("Mother") ? RoomRole.Mother
                : n.Contains("_Final") ? RoomRole.Final
                : n.Contains("_Secondary") ? RoomRole.Secondary
                : RoomRole.Procedural;
            def.archetype = RoomArchetype.None;
            foreach (RoomArchetype a in System.Enum.GetValues(typeof(RoomArchetype)))
                if (a != RoomArchetype.None && n.Contains("_" + a)) def.archetype = a;
            if (def.role == RoomRole.Tutorial)
            {
                var parts = n.Split('_');
                if (parts.Length > 2 && int.TryParse(parts[2], out int idx)) def.tutorialIndex = idx;
            }
            def.canBeUnpowered = def.role == RoomRole.Final && def.floor < 3;
            inst.Definition = def;
            EditorUtility.SetDirty(def);
            EditorUtility.SetDirty(prefab);
            return def;
        }

        /// <summary>Socket point for a cell edge (KIT_SPEC wall placement table).</summary>
        public static Vector3 SocketPoint(Vector2Int cell, Facing f, Vector2Int size)
        {
            switch (f)
            {
                case Facing.South: return new Vector3(cell.x * 2 + 1, 0, 0);
                case Facing.North: return new Vector3(cell.x * 2 + 1, 0, size.y * 2);
                case Facing.West: return new Vector3(0, 0, cell.y * 2 + 1);
                default: return new Vector3(size.x * 2, 0, cell.y * 2 + 1);
            }
        }

        static RoomDefinition LoadOrCreate(string name)
        {
            string path = $"{DefDir}/{name}.asset";
            var d = AssetDatabase.LoadAssetAtPath<RoomDefinition>(path);
            if (d != null) return d;
            d = ScriptableObject.CreateInstance<RoomDefinition>();
            AssetDatabase.CreateAsset(d, path);
            return d;
        }
    }
}
