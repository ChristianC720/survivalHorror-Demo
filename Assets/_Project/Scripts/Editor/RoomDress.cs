using System.Collections.Generic;
using System.Linq;
using Heartbeat.Generation;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Authored set dressing: the catalogue props the rooms did not use yet (cables, puddles, speakers, gauges, CRTs...), placed by
    /// hand-chosen coordinates in a "Dress_F#" child of each room. Idempotent: every run rebuilds that child. Rooms that are variants
    /// of another floor's layout switch off what they inherit, so a floor never shows another floor's props.
    /// Reports any solid prop in front of a socket or on a spawn. Menu: Heartbeat/Rooms/Dress Rooms.
    /// </summary>
    public static class RoomDress
    {
        private const string Rooms = "Assets/_Project/Prefabs/Rooms";
        private const string Props = "Assets/_Project/Prefabs/Props";
        private const float OnTop = -1f;   // y: rest on whatever is below
        private const float Speaker = 2.2f, Gauge = 1.4f, Valve = 1.1f;
        private const float North = 180f, East = 270f, South = 0f, West = 90f;

        private struct P
        {
            public string prop; public float x, z, yaw, y;
            public P(string prop, float x, float z, float yaw = 0f, float y = 0f) { this.prop = prop; this.x = x; this.z = z; this.yaw = yaw; this.y = y; }
        }

        private static string Crt(string f) => f == "F1" ? "PROP_F1_CRT" : "PROP_F2_CRT";

        /// <summary>Same layout on every floor; "#" is the floor.</summary>
        private static IEnumerable<P> ForLayout(string layout, string f)
        {
            switch (layout)
            {
                case "ROOM_#_Corridor_A":
                    return new[] { new P("PROP_Shared_Puddle", 2.9f, 7.6f), new P("PROP_Shared_TrashPile", 3.4f, 1.6f), new P("PROP_Shared_CableBundle", 3.55f, 9.6f, 90f), new P("PROP_Shared_Speaker", 3.5f, 6f, East, Speaker), new P("PROP_Shared_Grate", 1f, 7.6f) };
                case "ROOM_#_Hack_A":
                    return new[] { new P("PROP_Shared_HackPanel", .3f, 3f, West), new P("PROP_Shared_TrashPile", 7f, 7.1f), new P("PROP_Shared_Puddle", 3.4f, 3f), new P("PROP_Shared_CableBundle", 1.3f, 3.8f), f == "F3" ? new P("PROP_F3_HeavyCabling", 7.5f, 3.6f, East) : new P(Crt(f), 6f, 6f, 200f, OnTop), new P(f == "F1" ? "PROP_F1_Gauge" : "PROP_Shared_Speaker", 5f, .45f, South, f == "F1" ? Gauge : Speaker) };
                case "ROOM_#_Open_A":
                    return new[] { new P(f == "F1" ? "PROP_F1_WaterTank" : f == "F2" ? "PROP_F2_LabCoatRack" : "PROP_F3_CoolingColumn", 10.8f, 10.8f), new P("PROP_Shared_Grate", 6f, 3.8f), new P("PROP_Shared_Puddle", 6.6f, 9.5f), new P("PROP_Shared_TrashPile", 11f, 1f), new P("PROP_Shared_Speaker", 3f, 11.5f, North, Speaker), new P("PROP_Shared_Pipe_2m", 8.6f, .6f), new P("PROP_Shared_Pipe_Elbow", 1.2f, .7f) };
                case "ROOM_#_Storage_A":
                    return new[] { new P("PROP_Shared_TrashPile", 9f, 9f), new P("PROP_Shared_Puddle", 7f, 7f), new P("PROP_Shared_Speaker", .5f, 8f, West, Speaker), f == "F1" ? new P("PROP_F1_CRT", 5.6f, 9.2f, 170f, OnTop) : f == "F2" ? new P("PROP_F2_SurgicalTools", 5.6f, 9.2f, 0f, OnTop) : new P("PROP_Shared_CableBundle", 8f, 9.2f), f == "F1" ? new P("PROP_F1_Tools", 4.2f, 9.2f, 0f, OnTop) : new P("PROP_Shared_Grate", 4.5f, 2f), f == "F1" ? new P("PROP_F1_Gauge", 9.55f, 1f, East, Gauge) : new P("PROP_Shared_CableBundle", 9.4f, 5f, 90f) };
                case "ROOM_#_Surveillance_A":
                    {
                        var list = new List<P> { new P("PROP_Shared_Speaker", 1.2f, .5f, South, Speaker), new P("PROP_Shared_TrashPile", .9f, 7.2f), new P("PROP_Shared_Puddle", 4f, 6f) };
                        if (f == "F3") list.Add(new P("PROP_F3_CRTBank", 7f, 7.42f, North));
                        else { list.Add(new P(Crt(f), 4.6f, 2f, 15f, OnTop)); list.Add(new P(Crt(f), 5.4f, 2f, -15f, OnTop)); }
                        if (f == "F2") list.Add(new P("PROP_F2_LabChair", 5f, 3f, North));
                        return list;
                    }
                case "ROOM_#_Vertical_A":
                    return new[] { f == "F1" ? new P("PROP_F1_Gauge", .45f, 2.5f, West, Gauge) : new P("PROP_Shared_CableBundle", .8f, 2.5f, 90f), f == "F1" ? new P("PROP_F1_PressureValve", .4f, 5.5f, West, Valve) : new P("PROP_Shared_Grate", 3f, 5.5f), new P("PROP_Shared_Puddle", 3.4f, 3f), new P("PROP_Shared_TrashPile", 9.3f, 2.5f), new P("PROP_Shared_Speaker", 2f, 7.5f, North, Speaker) };
                case "KEY_#_Secondary_A":
                    return f == "F3" ? new[] { new P("PROP_Shared_Puddle", 2f, 3.4f), new P("PROP_Shared_Speaker", 5.5f, 2f, East, Speaker) } : new[] { new P(Crt(f), 3.5f, 5.2f, 165f, OnTop), new P("PROP_Shared_Puddle", 2f, 3.4f), new P("PROP_Shared_Speaker", 5.5f, 2f, East, Speaker) };
                case "KEY_#_Secondary_B":
                    return new[] { new P("PROP_Shared_Puddle", 4f, 2.6f), new P("PROP_Shared_TrashPile", 7.1f, 5.1f), new P(f == "F1" ? "PROP_F1_Gauge" : "PROP_Shared_Speaker", 3.8f, 5.5f, North, f == "F1" ? Gauge : Speaker) };
                case "KEY_#_Final":
                    return new[] { new P("PROP_Shared_Speaker", 2f, 7.5f, North, Speaker), new P("PROP_Shared_TrashPile", 7.2f, 1f), new P("PROP_Shared_Grate", 3f, 5f) };
                case "KEY_#_CapsuleRow":
                    return new[] { new P("PROP_Shared_Speaker", .5f, 4.5f, West, Speaker), new P("PROP_Shared_CableBundle", 4f, 4f) };
                // B layouts: F2's by hand, F1's re-dressed from them (FloorRedress).
                case "ROOM_#_Corridor_B":
                    return f == "F3" ? new[] { new P("PROP_Shared_Puddle", 2f, 6f), new P("PROP_Shared_Grate", 1.2f, 9f) } : new[] { new P("PROP_Shared_Puddle", 1.2f, 9f), new P("PROP_Shared_Speaker", .5f, 12f, West, Speaker), new P("PROP_Shared_TrashPile", .9f, 2f) };
                case "ROOM_#_Hack_B":
                    return f == "F3" ? new[] { new P("PROP_Shared_HackPanel", 7.7f, 3f, East), new P("PROP_Shared_Puddle", 3f, 6f) } : new[] { new P("PROP_Shared_HackPanel", .3f, 5f, West), new P("PROP_Shared_Puddle", 6f, 5.4f) };
                case "ROOM_#_Open_B":
                    if (f == "F3") return new[] { new P("PROP_F3_CRTBank", 6.2f, .58f, South), new P("PROP_Shared_Grate", 5f, 2f) };
                    if (f == "F1") return new[] { new P("PROP_F1_BigContainer", 8.9f, 7f), new P("PROP_Shared_MetalCrate_Half", 6.8f, 9.4f), new P("PROP_Shared_Puddle", 5.6f, 2f) };
                    return new[] { new P("PROP_F2_CRT", 8.5f, 9.2f, 170f, OnTop), new P("PROP_Shared_TrashPile", .8f, 9.2f), new P("PROP_Shared_Puddle", 5.6f, 2f) };
                case "ROOM_#_Storage_B":
                    return f == "F3" ? new[] { new P("PROP_Shared_TrashPile", 4.8f, 1f), new P("PROP_Shared_Puddle", 6f, 7f) } : new[] { new P(Crt(f), 5.4f, 5.5f, 170f, OnTop), new P("PROP_Shared_TrashPile", 6.4f, 1f) };
                case "ROOM_#_Surveillance_B":
                    if (f == "F3") return new[] { new P("PROP_F3_CRTBank", .58f, 6f, West), new P("PROP_Shared_Puddle", 5f, 2f) };
                    return new[] { new P(Crt(f), 3.6f, 5.6f, 170f, OnTop), new P(Crt(f), 4.4f, 5.6f, 190f, OnTop), new P("PROP_Shared_TrashPile", 1f, 4.5f) };
                case "ROOM_#_Vertical_B":
                    return new[] { new P("PROP_Shared_Puddle", 4f, 5f), new P("PROP_Shared_Grate", 1.2f, 6f) };
            }
            return null;
        }

        [MenuItem("Heartbeat/Rooms/Dress Rooms")]
        public static void Menu() => Debug.Log("[Heartbeat][Rooms] " + Run());

        public static string Run()
        {
            var log = new System.Text.StringBuilder();
            int placed = 0;
            // The containers keep their identity between runs, so the variants' "switched off" overrides stay valid. F1's B rooms inherit
            // from F2 and F2's A rooms from F1, so a pass over F1 and F2 each must come after the other has its container.
            foreach (var f in new[] { "F2", "F1", "F2", "F3" })
                foreach (var g in AssetDatabase.FindAssets("t:Prefab", new[] { $"{Rooms}/{f}" }))
                {
                    string path = AssetDatabase.GUIDToAssetPath(g);
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    var items = ForLayout(name.Replace(f, "#"), f);
                    if (items == null) continue;
                    var root = PrefabUtility.LoadPrefabContents(path);
                    string own = "Dress_" + f;
                    foreach (Transform c in root.transform)
                        if (c.name.StartsWith("Dress") && c.name != own) c.gameObject.SetActive(false);
                    var dress = root.transform.Find(own);
                    if (dress == null)
                    {
                        dress = new GameObject(own) { isStatic = true }.transform;
                        dress.SetParent(root.transform, false);
                    }
                    foreach (var c in dress.Cast<Transform>().ToArray()) Object.DestroyImmediate(c.gameObject);
                    foreach (var p in items)
                    {
                        var prefab = FindProp(p.prop);
                        if (prefab == null) { log.AppendLine($"{name}: missing {p.prop}"); continue; }
                        var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, dress);
                        inst.transform.localPosition = new Vector3(p.x, p.y == OnTop ? TopBelow(root, dress, p.x, p.z) : p.y, p.z);
                        inst.transform.localRotation = Quaternion.Euler(0, p.yaw, 0);
                        placed++;
                        Check(root, inst, name, log);
                    }
                    if (name == "ROOM_F1_Open_B") MakeWayForContainer(root);
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    PrefabUtility.UnloadPrefabContents(root);
                }
            AssetDatabase.SaveAssets();
            return $"{placed} props placed\n{log}";
        }

        /// <summary>The 6 m container takes the east side; its desk and console go, and the spawns there move.</summary>
        private static void MakeWayForContainer(GameObject root)
        {
            foreach (var n in new[] { "NurseConsole", "NurseDesk", "PatientScreen_1" })
            {
                var t = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == n || x.name.StartsWith(n + "_F1"));
                if (t != null) Object.DestroyImmediate(t.gameObject);
            }
            void Move(string n, float x, float z)
            {
                var t = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(s => s.name == n);
                if (t != null) t.localPosition = new Vector3(x, t.localPosition.y, z);
            }
            Move("Item_2", 7f, 1.6f);
            Move("Item_Doc", 6.8f, 9.4f);
            Move("Enemy_2", 5.5f, 6f);
        }

        private static GameObject FindProp(string name)
        {
            foreach (var dir in new[] { "Shared", "F1", "F2", "F3" })
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>($"{Props}/{dir}/{name}.prefab");
                if (go != null) return go;
            }
            return null;
        }

        private static float TopBelow(GameObject root, Transform dress, float x, float z)
        {
            float top = 0f;
            foreach (var r in root.GetComponentsInChildren<Renderer>(false))
            {
                if (r.transform.IsChildOf(dress)) continue;
                var b = r.bounds;
                if (x >= b.min.x && x <= b.max.x && z >= b.min.z && z <= b.max.z && b.max.y < 2f) top = Mathf.Max(top, b.max.y);
            }
            return top;
        }

        private static void Check(GameObject root, GameObject inst, string room, System.Text.StringBuilder log)
        {
            foreach (var col in inst.GetComponentsInChildren<Collider>(true))
            {
                var b = col.bounds;
                b.Expand(new Vector3(.2f, 0, .2f));
                foreach (var s in root.GetComponentsInChildren<SpawnPoint>(true))
                    if (s.gameObject.activeInHierarchy && b.Contains(new Vector3(s.transform.position.x, b.center.y, s.transform.position.z)))
                        log.AppendLine($"{room}: {inst.name} on spawn {s.name}");
                foreach (var s in root.GetComponentsInChildren<DoorSocket>(true))
                {
                    var p = s.transform.position + s.transform.forward * -1f;
                    if (new Vector2(b.center.x - p.x, b.center.z - p.z).magnitude < 1f + b.extents.magnitude * .5f && b.Contains(new Vector3(p.x, b.center.y, p.z)))
                        log.AppendLine($"{room}: {inst.name} blocks socket {s.name}");
                }
            }
        }
    }
}
