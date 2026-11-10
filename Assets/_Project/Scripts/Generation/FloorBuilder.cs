using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.Generation.Layout;
using Heartbeat.Interaction;
using Heartbeat.Inventory;
using Heartbeat.Level;
using Heartbeat.Narrative;
using Unity.AI.Navigation;
using UnityEngine;

namespace Heartbeat.Generation
{
    /// <summary>
    /// Turns a FloorLayout into the playable floor (S-06): room prefabs placed on the 2 m grid and rotated, shells
    /// assembled from the kit, doors with their locks, items and enemies on their spawns, one NavMesh bake, Lukas at the
    /// entry. Target: ≤ 5 s including the bake. Lives in the Floor scene and registers itself as a scene service.
    /// </summary>
    [RequireComponent(typeof(NavMeshSurface), typeof(RoomActivation))]
    public sealed class FloorBuilder : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private RoomLibrary library;
        [SerializeField] private FloorConfig[] floorConfigs;
        [Tooltip("DOC_EMILIA_01..06 in order (2 per floor).")]
        [SerializeField] private DocumentDefinition[] emiliaDocuments;
        [SerializeField] private DocumentDefinition[] contextDocuments;
        [SerializeField] private DocumentDefinition[] confiscatedObjects;

        [Header("Prefabs")]
        [SerializeField] private GameObject healPrefab;
        [SerializeField] private GameObject ammoPrefab;
        [SerializeField] private GameObject spikePrefab;
        [SerializeField] private GameObject keyMandatoryPrefab;
        [SerializeField] private GameObject keyOptionalPrefab;
        [SerializeField] private GameObject documentPrefab;
        [SerializeField] private GameObject powerSwitchPrefab;
        [SerializeField] private GameObject altoPrefab;
        [SerializeField] private GameObject rastreroPrefab;
        [SerializeField] private GameObject bocaPrefab;
        [SerializeField] private GameObject mudoPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject playerCameraPrefab;

        private NavMeshSurface surface;
        private RoomActivation activation;
        private readonly List<RoomInstance> built = new List<RoomInstance>();
        private Transform floorRoot;
        private Transform doorsRoot;   // outside the NavMesh collection: doors must not cut the navmesh
        private readonly List<DoorLeaf> doors = new List<DoorLeaf>();
        private readonly List<DoorLeaf> spawnBarrier = new List<DoorLeaf>();
        private RoomInstance barredRoom;

        public FloorLayout Layout { get; private set; }
        public IReadOnlyList<RoomInstance> Rooms => built;
        public IReadOnlyList<DoorLeaf> Doors => doors;
        public GameObject Player { get; private set; }
        public float LastBuildMs { get; private set; }

        private void Awake()
        {
            surface = GetComponent<NavMeshSurface>();
            activation = GetComponent<RoomActivation>();
        }

        private void OnEnable() { Services.Register(this); GameEvents.RoomEntered += OnRoomEntered; }
        private void OnDisable() { Services.Unregister(this); GameEvents.RoomEntered -= OnRoomEntered; }

        /// <summary>The spawn room is closed to enemies until Lukas first enters another room.</summary>
        private void OnRoomEntered(MonoBehaviour room)
        {
            if (spawnBarrier.Count == 0 || room == barredRoom) return;
            foreach (var d in spawnBarrier) if (d != null) d.SetEnemyBarrier(false);
            spawnBarrier.Clear();
            HbLog.Info("Gen", "Spawn room open to enemies (Lukas left it)");
        }

        public FloorConfig ConfigFor(int floor, bool showcase)
        {
            string suffix = showcase ? "Showcase" : "Full";
            return floorConfigs.FirstOrDefault(c => c != null && c.floor == floor && c.name.EndsWith(suffix))
                   ?? floorConfigs.FirstOrDefault(c => c != null && c.floor == floor);
        }

        /// <summary>Generates and builds a floor. Returns false if no valid layout exists for the seed.</summary>
        /// <summary>The Mother's room is built as "floor 4" in the Floor scene (fixed key room after floor 3, D-33).</summary>
        public const int MotherFloor = 4;
        [SerializeField] private GameObject motherRoomPrefab;

        /// <summary>The PA central (D-59): one fixed, sealed room; no autosave on arrival (D-30).</summary>
        public bool BuildMother()
        {
            var sw = Stopwatch.StartNew();
            Clear();
            Layout = null;
            floorRoot = new GameObject("MotherRoom").transform;
            floorRoot.SetParent(transform, false);
            var room = Instantiate(motherRoomPrefab, floorRoot).GetComponent<RoomInstance>();
            room.Generation = RoomInstance.NextGeneration();
            room.name = "ROOM_Mother";
            room.GetComponent<RoomShell>().Build(new List<SocketData>());
            built.Add(room);
            surface.collectObjects = CollectObjects.Children;
            surface.layerMask = LayerMask.GetMask("Environment");
            surface.BuildNavMesh();
            activation.SetRooms(built);
            SpawnPlayer(0);
            activation.Apply(room);
            LastBuildMs = sw.ElapsedMilliseconds;
            HbLog.Info("Gen", $"floor={MotherFloor} The Mother's room built in {LastBuildMs} ms");
            GameEvents.RaiseFloorBuilt(MotherFloor, 0);
            return true;
        }

        public bool Build(int floor, int seed, bool firstAttempt, bool storyMode, bool showcase, IReadOnlyCollection<string> registered = null, bool startInFinalRoom = false)
        {
            if (floor >= MotherFloor) return BuildMother();
            var sw = Stopwatch.StartNew();
            Clear();

            var cfg = ConfigFor(floor, showcase);
            var p = cfg.ToParams(seed, firstAttempt, storyMode);
            var templates = library.rooms.Where(r => r != null && r.floor == floor).Select(RoomTemplate.From).ToList();
            if (templates.Count == 0)
            {
                // Development fallback until the floor's own rooms exist (E-14 F2/F3).
                HbLog.Warn("Gen", $"No rooms for floor {floor} yet: using floor 1 rooms");
                templates = library.rooms.Where(r => r != null && r.floor == 1).Select(RoomTemplate.From).ToList();
            }
            Layout = LayoutGenerator.Generate(p, templates);
            if (Layout == null)
            {
                HbLog.Error("Gen", $"floor={floor} seed={seed}: no valid layout");
                return false;
            }

            floorRoot = new GameObject($"Floor_{floor}_seed{Layout.Seed}").transform;
            floorRoot.SetParent(transform, false);
            doorsRoot = new GameObject($"Doors_{floor}").transform;
            // Start() of a freshly loaded scene runs before it becomes the active scene: anything instantiated at the
            // root would land in the previous (about to be unloaded) scene. Keep everything in this scene.
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(doorsRoot.gameObject, gameObject.scene);
            emiliaCursor = 0; placedDocs.Clear();

            int generation = RoomInstance.NextGeneration();
            foreach (var pr in Layout.Rooms)
            {
                var r = PlaceRoom(pr);
                r.Generation = generation;
                built.Add(r);
                // Safety posters only in the fixed rooms of the first attempt: capsule row + tutorial chain (D-89).
                bool tutorial = pr.TutorialIndex > 0 || (p.FirstAttempt && pr.Index == Layout.EntryRoom && floor == 1);
                r.IsTutorial = tutorial;   // D-160: Lukas cannot die here
                foreach (var poster in r.GetComponentsInChildren<UI.TutorialPoster>(true)) if (poster.TutorialOnly) poster.gameObject.SetActive(tutorial);
                foreach (var cue in r.GetComponentsInChildren<Audio.TutorialCue>(true)) cue.gameObject.SetActive(tutorial);
                // D-74: the guard's revolver in the tutorial depot; afterwards on the capsule tray of floor 1.
                foreach (var gun in r.GetComponentsInChildren<Interaction.RevolverPickup>(true))
                    gun.gameObject.SetActive(gun.Where == Interaction.RevolverPickup.Spot.TutorialGuard
                        ? pr.TutorialIndex == 2
                        : !p.FirstAttempt && pr.Index == Layout.EntryRoom && floor == 1);
            }
            foreach (var c in Layout.Connections)
            {
                built[c.A].AddNeighbour(built[c.B]);
                built[c.B].AddNeighbour(built[c.A]);
            }
            for (int i = 0; i < built.Count; i++) BuildShell(i);
            foreach (var c in Layout.Connections) PlaceDoor(c);
            BuildTutorialGates(cfg.tutorialGateTimeout);
            for (int i = 0; i < Layout.Items.Count; i++) PlaceItem(Layout.Items[i], registered);
            ConfigureExit(floor);

            surface.collectObjects = CollectObjects.Children;
            surface.layerMask = LayerMask.GetMask("Environment");
            surface.BuildNavMesh();

            activation.SetRooms(built);
            // Continue from the autosave starts in the final room (D-30); the spawn room never holds enemies (global rule 6).
            int spawnRoom = startInFinalRoom && Layout.FinalRoom >= 0 ? Layout.FinalRoom : Layout.EntryRoom;
            foreach (var e in Layout.Enemies) if (!startInFinalRoom || e.Room != spawnRoom) PlaceEnemy(e);
            SpawnPlayer(spawnRoom);
            BarSpawnRoom(spawnRoom);
            if (!startInFinalRoom && cfg.startingAmmo > 0)
            {
                // Starting supplies (D-new): a few rounds on the floor next to where Lukas wakes; outside the budget.
                var t = Player.transform;
                DropItem(ItemKind.Ammo, cfg.startingAmmo, t.position + t.forward * 1.2f);
            }
            activation.Apply(built[spawnRoom]);

            LastBuildMs = sw.ElapsedMilliseconds;
            HbLog.Info("Gen", $"floor={floor} seed={Layout.Seed} (requested {seed}, {Layout.Attempts} attempts): {Layout.Rooms.Count} rooms, " +
                              $"{Layout.Enemies.Count} enemies, {Layout.Items.Count} items, unpowered={Layout.FinalUnpowered}; built in {LastBuildMs} ms");
            GameEvents.RaiseFloorBuilt(floor, Layout.Seed);
            return true;
        }

        public void Clear()
        {
            foreach (var r in built) if (r != null) r.Retired = true;
            built.Clear();
            doors.Clear();
            spawnBarrier.Clear();
            barredRoom = null;
            if (floorRoot != null)
            {
                // Destroy is deferred to end of frame: detach first or the rebake below still collects the old floor.
                floorRoot.gameObject.SetActive(false);
                floorRoot.SetParent(null, false);
                Destroy(floorRoot.gameObject);
            }
            if (doorsRoot != null) Destroy(doorsRoot.gameObject);
            floorRoot = doorsRoot = null;
            surface.RemoveData();
        }

        // ------------------------------------------------------------------------------------------------------------

        private RoomInstance PlaceRoom(PlacedRoom pr)
        {
            var size = pr.Template.Size;
            Vector3 offset = (pr.Rotation & 3) switch
            {
                1 => new Vector3(0f, 0f, size.x * 2f),
                2 => new Vector3(size.x * 2f, 0f, size.y * 2f),
                3 => new Vector3(size.y * 2f, 0f, 0f),
                _ => Vector3.zero,
            };
            Vector3 pos = new Vector3(pr.Origin.x * 2f, 0f, pr.Origin.y * 2f) + offset;
            var go = Instantiate(pr.Template.Definition.prefab, pos, Quaternion.Euler(0f, 90f * pr.Rotation, 0f), floorRoot);
            go.name = $"{pr.Index:00}_{pr.Template.Id}";
            var room = go.GetComponent<RoomInstance>();
            room.NodeIndex = pr.Index;
            room.IsEmpty = pr.Empty;
            room.HasCamera = pr.HasCamera;
            // The tutorial depot is the dark room that teaches the flashlight (D-70, Niveles: "Depósito a oscuras").
            room.IsPowered = !(pr.Index == Layout.FinalRoom && Layout.FinalUnpowered) && pr.TutorialIndex != 2;
            return room;
        }

        private void BuildShell(int index)
        {
            var pr = Layout.Rooms[index];
            var connected = new List<SocketData>();
            foreach (var c in Layout.Connections)
            {
                if (c.A == index) connected.Add(pr.Template.Sockets[c.SocketA]);
                if (c.B == index) connected.Add(pr.Template.Sockets[c.SocketB]);
            }
            built[index].GetComponent<RoomShell>().Build(connected);
        }

        private DoorSocket SocketOf(int room, int socket) => built[room].GetComponentsInChildren<DoorSocket>(true)[socket];

        private void PlaceDoor(Connection c)
        {
            var s = SocketOf(c.A, c.SocketA);
            var kit = built[c.A].GetComponent<RoomShell>().kit;
            var door = Instantiate(kit.door, s.transform.position, s.transform.rotation, doorsRoot);
            door.name = $"Door_{c.A}_{c.B}";
            var leaf = door.GetComponent<DoorLeaf>();
            leaf.Configure(c.Lock);
            doors.Add(leaf);
        }

        /// <summary>D-164: each door leaving a tutorial room stays shut until that room's actions are done (see TutorialGuide).</summary>
        private void BuildTutorialGates(float timeout)
        {
            var stages = new Dictionary<RoomInstance, int>();
            foreach (var pr in Layout.Rooms)
                if (built[pr.Index].IsTutorial) stages[built[pr.Index]] = pr.TutorialIndex > 0 ? pr.TutorialIndex : 1;
            if (stages.Count == 0) return;

            int Stage(int room) => stages.TryGetValue(built[room], out int s) ? s : int.MaxValue;
            var gates = new Dictionary<int, List<DoorLeaf>>();
            for (int i = 0; i < Layout.Connections.Count; i++)
            {
                var c = Layout.Connections[i];
                int a = Stage(c.A), b = Stage(c.B);
                if (a == b || c.Lock != DoorLock.None) continue;
                int stage = Mathf.Min(a, b);
                if (!gates.TryGetValue(stage, out var list)) gates[stage] = list = new List<DoorLeaf>();
                doors[i].Configure(DoorLock.Tutorial);
                list.Add(doors[i]);
            }
            var guide = new GameObject("TutorialGuide").AddComponent<Tutorial.TutorialGuide>();
            guide.transform.SetParent(floorRoot, false);
            guide.Init(stages, gates, timeout);
        }

        private void PlaceItem(ItemPlacement item, IReadOnlyCollection<string> registered)
        {
            var spawns = built[item.Room].GetComponentsInChildren<SpawnPoint>(true).Where(s => s.kind == SpawnKind.Item).ToList();
            if (item.Spawn >= spawns.Count) return;
            var at = spawns[item.Spawn].transform;

            GameObject prefab = null;
            DocumentDefinition doc = null;
            bool stays = false;
            switch (item.Tag)
            {
                case ItemSpawnTag.Switch: prefab = powerSwitchPrefab; break;
                case ItemSpawnTag.Emilia:
                    doc = NextEmilia(registered);
                    if (doc == null)
                    {
                        // Already registered: 1 extra of the floor's scarcest resource instead (D-79).
                        Spawn(ammoPrefab, at, ItemKind.Ammo, 4);
                        return;
                    }
                    prefab = documentPrefab;
                    break;
                case ItemSpawnTag.Document: doc = Pick(contextDocuments, registered, item); prefab = doc != null ? documentPrefab : null; break;
                case ItemSpawnTag.Confiscated: doc = Pick(confiscatedObjects, registered, item); prefab = doc != null ? documentPrefab : null; stays = true; break;
                default:
                    Spawn(PrefabFor(item.Kind), at, item.Kind, item.Amount);
                    return;
            }
            if (prefab == null) return;
            var go = Instantiate(prefab, at.position, at.rotation, floorRoot);
            if (doc != null) go.GetComponent<DocumentPickup>().Setup(doc, stays);
        }

        private int emiliaCursor;

        private DocumentDefinition NextEmilia(IReadOnlyCollection<string> registered)
        {
            int floor = Layout.Floor;
            var mine = emiliaDocuments.Where(d => d != null && d.floor == floor).ToList();
            while (emiliaCursor < mine.Count)
            {
                var d = mine[emiliaCursor++];
                if (registered == null || !registered.Contains(d.id)) return d;
            }
            return null;
        }

        /// <summary>
        /// A record of this floor, unread ones first; deterministic from the floor seed and the spawn (same seed = same floor).
        /// </summary>
        private DocumentDefinition Pick(DocumentDefinition[] pool, IReadOnlyCollection<string> registered, ItemPlacement item)
        {
            // Never the same record twice on one floor (the registry only changes when Lukas reads them).
            var mine = (pool ?? new DocumentDefinition[0]).Where(d => d != null && d.floor == Layout.Floor && !placedDocs.Contains(d.id)).ToList();
            var unread = registered == null ? mine : mine.Where(d => !registered.Contains(d.id)).ToList();
            // shortcut: an unread fault document (M-17) takes the floor's first document spawn instead of a context one;
            // the GDD puts faults outside the per-floor budget — give them their own spawn tag if content gets tight.
            var pick = unread.FirstOrDefault(d => d.kind == RecordKind.Fault);
            if (pick == null)
            {
                // Already-read records may come back to re-read, but a read (and soon patched) fault never does.
                var from = unread.Count > 0 ? unread : mine.Where(d => d.kind != RecordKind.Fault).ToList();
                if (from.Count == 0) return null;
                var rng = new System.Random(Layout.Seed * 31 + item.Room * 7 + item.Spawn);
                pick = from[rng.Next(from.Count)];
            }
            placedDocs.Add(pick.id);
            return pick;
        }

        private readonly HashSet<string> placedDocs = new HashSet<string>();

        private GameObject PrefabFor(ItemKind k) => k switch
        {
            ItemKind.Heal => healPrefab,
            ItemKind.Ammo => ammoPrefab,
            ItemKind.Spike => spikePrefab,
            ItemKind.KeyMandatory => keyMandatoryPrefab,
            ItemKind.KeyOptional => keyOptionalPrefab,
            _ => null,
        };

        /// <summary>Discarded inventory item (M-07): a normal pickup on the floor of the current floor.</summary>
        public void DropItem(ItemKind kind, int amount, Vector3 position)
        {
            var prefab = PrefabFor(kind);
            if (prefab == null) return;
            var go = Instantiate(prefab, position, Quaternion.identity, floorRoot);
            if (go.TryGetComponent(out WorldItem wi)) wi.Setup(kind, amount);
        }

        private void Spawn(GameObject prefab, Transform at, ItemKind kind, int amount)
        {
            if (prefab == null) return;
            var go = Instantiate(prefab, at.position, at.rotation, floorRoot);
            var wi = go.GetComponent<WorldItem>();
            if (wi != null) wi.Setup(kind, amount);
        }

        private void PlaceEnemy(EnemyPlacement e)
        {
            var spawns = built[e.Room].GetComponentsInChildren<SpawnPoint>(true).Where(s => s.kind == SpawnKind.Enemy).ToList();
            if (e.Spawn >= spawns.Count) return;
            var prefab = e.Type switch
            {
                EnemyType.Rastrero => rastreroPrefab,
                EnemyType.Boca => bocaPrefab != null ? bocaPrefab : altoPrefab,
                EnemyType.Mudo => mudoPrefab != null ? mudoPrefab : altoPrefab,
                _ => altoPrefab,
            };
            var at = spawns[e.Spawn].transform;
            var go = Instantiate(prefab, at.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), floorRoot);
            go.name = $"{e.Type}_{e.Room}_{e.Spawn}";
            if (built[e.Room].IsTutorial) go.GetComponent<EnemyBrain>().HoldUntilShot();   // still and harmless until the first shot
            activation.Register(built[e.Room], go);
        }

        private void ConfigureExit(int floor)
        {
            if (Layout.FinalRoom < 0) return;
            var final = built[Layout.FinalRoom];
            var marker = final.transform.Find("StairsExit");
            if (marker == null) return;
            var exit = marker.GetComponent<FloorExit>();
            if (exit == null)
            {
                exit = marker.gameObject.AddComponent<FloorExit>();
                var col = marker.gameObject.AddComponent<SphereCollider>();
                col.isTrigger = true;
                col.radius = 1f;
                marker.gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            exit.Floor = floor;
            exit.Powered = final.IsPowered;
        }

        private void BarSpawnRoom(int room)
        {
            barredRoom = built[room];
            foreach (var c in Layout.Connections)
                if (c.A == room || c.B == room)
                {
                    var leaf = doors.FirstOrDefault(d => d != null && d.name == $"Door_{c.A}_{c.B}");
                    if (leaf == null) continue;
                    leaf.SetEnemyBarrier(true);
                    spawnBarrier.Add(leaf);
                }
        }

        private void SpawnPlayer(int room)
        {
            var entry = built[room];
            var sp = entry.GetComponentsInChildren<SpawnPoint>(true).FirstOrDefault(s => s.kind == SpawnKind.Player);
            Vector3 pos = sp != null ? sp.transform.position : entry.transform.position + entry.transform.rotation * new Vector3(2f, 0f, 2f);
            Quaternion rot = sp != null ? sp.transform.rotation : entry.transform.rotation;

            if (Player == null)
            {
                // Only reuse a player that belongs to this scene (the previous Floor scene may still be loaded).
                var existing = FindObjectsByType<Player.PlayerController>().FirstOrDefault(p => p.gameObject.scene == gameObject.scene);
                Player = existing != null ? existing.gameObject : Instantiate(playerPrefab);
                Player.name = "Player";
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(Player, gameObject.scene);
                if (!FindObjectsByType<CameraSystem.OtsCameraRig>().Any(r => r.gameObject.scene == gameObject.scene))
                {
                    var cam = Instantiate(playerCameraPrefab);
                    cam.name = "PlayerCamera";
                    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cam, gameObject.scene);
                }
            }
            Player.GetComponent<Player.PlayerController>().DebugTeleport(pos.x, pos.y, pos.z, rot.eulerAngles.y);
        }
    }
}
