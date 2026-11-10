using System.Collections.Generic;
using Heartbeat.Core;
using Heartbeat.Generation;
using Heartbeat.InputRouting;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.CameraSystem
{
    /// <summary>
    /// "Seeing through the machine's eyes" (M-02, M-03, S-01, D-23, D-24, D-25). Q opens the current room's camera:
    /// isometric orthographic view from its CameraMount, low resolution with a ~0.3 s delay (frame ring buffer) and CRT
    /// noise; ceilings hidden. Lukas can only stand or walk very slowly; running, aiming, shoving, damage or leaving the
    /// room close it. Search degrades the signal; Hunt (or damage) cuts it instantly, and a hunt locks it until the hunt
    /// ends or Lukas crosses into another room. Feeds The Mother's hidden attention meter.
    /// </summary>
    public sealed class SurveillanceSystem : MonoBehaviour
    {
        [Tooltip("Feed resolution: high enough that the room reads; the CRT look comes from scanlines and noise (C-19).")]
        [SerializeField] private int textureHeight = 720;
        [Tooltip("Upper bound of the orthographic size; the view fits the room it shows (M-02: the room fills the feed).")]
        [SerializeField] private float orthoSize = 7f;
        [SerializeField] private float minOrthoSize = 3f;
        [Tooltip("Flat ambient light while the feed is up: the camera image covers the whole screen, so only the feed sees it (M-02 legibility).")]
        [SerializeField] private Color viewAmbient = new Color(0.95f, 1f, 0.95f);   // 0.32 left the far half of the room black (C-19)

        private UnityEngine.Rendering.AmbientMode savedAmbientMode;
        private Color savedAmbient;

        private const int RingSize = 24;
        private const float FeedFrameSeconds = 1f / 30f;

        private InputRouter router;
        private GameStateMachine states;
        private PlayerController player;
        private Camera cam;
        private RenderTexture[] ring;
        private float[] stamps;
        private int head;
        private RawImage screen;
        private Image noise;
        private GameObject live, noSignal;
        private float noiseScale = 1f, noSignalUntil;
        private const float NoSignalSeconds = 0.6f;
        private RoomInstance viewingRoom;
        // M-02: "only what the room's camera covers" — everything outside the viewed room is hidden from this camera.
        private readonly List<Renderer> outside = new List<Renderer>();
        private readonly List<Renderer> movers = new List<Renderer>();
        private Bounds viewBounds;
        private AlertState highest;

        public AttentionMeter Attention { get; private set; }
        public bool IsOpen => viewingRoom != null;
        public bool IsLocked => highest == AlertState.Hunt;
        public float Noise { get; private set; }

        private void Awake()
        {
            var b = SystemsBalance.Current;
            Attention = new AttentionMeter(b.attentionFillSeconds, b.attentionDrainSeconds);
        }

        private void OnEnable()
        {
            Services.Register(this);
            GameEvents.HighestAlertChanged += OnHighestAlert;
            GameEvents.BossPhaseChanged += OnBossPhase;
            GameEvents.RoomEntered += OnRoomEntered;
            RenderPipelineManager.beginCameraRendering += OnBeginCamera;
            RenderPipelineManager.endCameraRendering += OnEndCamera;
            GameEvents.ShotFired += OnShot;
            GameEvents.AimChanged += OnAim;
            Attention.Full += OnAttentionFull;
        }

        private void OnDisable()
        {
            Services.Unregister(this);
            GameEvents.HighestAlertChanged -= OnHighestAlert;
            GameEvents.BossPhaseChanged -= OnBossPhase;
            GameEvents.RoomEntered -= OnRoomEntered;
            RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
            RenderPipelineManager.endCameraRendering -= OnEndCamera;
            GameEvents.ShotFired -= OnShot;
            GameEvents.AimChanged -= OnAim;
            Attention.Full -= OnAttentionFull;
        }

        private void Start()
        {
            router = Services.Get<InputRouter>();
            Services.TryGet(out states);
            router.Actions.Gameplay.Surveillance.performed += OnToggle;
            router.Actions.Gameplay.Attack.performed += OnAttack;
            BuildCameraAndOverlay();
        }

        private void OnDestroy()
        {
            if (router != null)
            {
                router.Actions.Gameplay.Surveillance.performed -= OnToggle;
                router.Actions.Gameplay.Attack.performed -= OnAttack;
            }
            if (ring != null) foreach (var rt in ring) if (rt != null) rt.Release();
        }

        // ---------- Open / close ----------

        private void OnToggle(InputAction.CallbackContext _)
        {
            if (IsOpen) Close("Q");
            else TryOpen();
        }

        public bool TryOpen()
        {
            if (player == null && !Services.TryGet(out player)) return false;
            if (IsLocked) { HbLog.Info("Surveillance", "Locked: hunt in progress"); ShowNoSignal(); return false; }
            if (BossCamerasHostile) { HbLog.Info("Surveillance", "Locked: The Mother's cameras are hostile (phase 2)"); ShowNoSignal(); return false; }
            var room = RoomInstance.Current;
            var mount = MountFor(room);
            if (mount == null) { HbLog.Info("Surveillance", "No camera in this room"); return false; }

            System.Array.Clear(stamps, 0, RingSize);   // never show a frame from the last time the view was open
            viewingRoom = room;
            cam.transform.SetPositionAndRotation(mount.position, mount.rotation);
            FrameRoom(room);
            CollectOutside(room);   // after the pose: the hidden near walls depend on where the camera looks from
            cam.enabled = true;
            screen.enabled = true;
            noise.enabled = true;
            live.SetActive(true);
            noiseScale = Services.TryGet(out Save.SaveService saves) && saves.LoadGlobal().reduceCrt ? 0.5f : 1f;   // the signal difference stays
            savedAmbientMode = RenderSettings.ambientMode;
            savedAmbient = RenderSettings.ambientLight;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = viewAmbient;
            player.IsSurveilling = true;
            if (player.TryGetComponent(out PlayerHealth health)) health.Hit += OnPlayerHit;
            router.SetContext(InputContext.Surveillance);
            states?.TrySetSubState(PlaySubState.Surveillance);
            HbLog.Info("Surveillance", $"Open room={room.name}");
            return true;
        }

        public void Close(string reason)
        {
            if (!IsOpen) return;
            viewingRoom = null;
            cam.enabled = false;
            screen.enabled = false;
            noise.enabled = false;
            live.SetActive(false);
            RenderSettings.ambientMode = savedAmbientMode;
            RenderSettings.ambientLight = savedAmbient;
            if (player != null)
            {
                player.IsSurveilling = false;
                if (player.TryGetComponent(out PlayerHealth health)) health.Hit -= OnPlayerHit;
            }
            if (router.Context == InputContext.Surveillance) router.SetContext(InputContext.Exploration);
            if (states != null && states.SubState == PlaySubState.Surveillance) states.TrySetSubState(PlaySubState.Exploration);
            HbLog.Info("Surveillance", $"Cut reason={reason}");
        }

        /// <summary>Camera pose: the room's mount, or a default high corner in key rooms (all key rooms have cameras, D-25).</summary>
        private static Transform MountFor(RoomInstance room)
        {
            if (room == null) return null;
            if (!room.HasCamera) return null;   // D-25: set per floor by the generator
            if (room.CameraMount != null) return room.CameraMount;
            if (room.Definition == null) return null;
            var t = room.transform.Find("AutoCameraMount");
            if (t != null) return t;
            t = new GameObject("AutoCameraMount").transform;
            t.SetParent(room.transform, false);
            var size = room.Definition.size;
            t.localPosition = new Vector3(size.x * 2f - 0.6f, 2.7f, size.y * 2f - 0.6f);
            t.LookAt(room.transform.TransformPoint(new Vector3(size.x, 0f, size.y)));
            return t;
        }

        // ---------- Rules ----------

        private void Update()
        {
            Attention.Tick(Time.deltaTime, IsOpen);
            if (noSignal.activeSelf && Time.unscaledTime >= noSignalUntil) noSignal.SetActive(false);
            if (!IsOpen) return;
            if (player != null && player.IsRunning) { Close("Run"); return; }

            float baseNoise = 0.15f + Attention.Value * 0.35f;
            Noise = highest == AlertState.Search ? baseNoise + 0.35f : baseNoise;
            var c = noise.color;
            c.a = Mathf.Clamp01(Noise * 0.5f * noiseScale * (0.7f + 0.3f * Random.value));
            noise.color = c;
        }

        private void LateUpdate()
        {
            if (!IsOpen) return;
            // Ring buffer at ~30 fps (24 frames hold 0.8 s at any frame rate): show the newest frame at least the delay old.
            // The delay grows while an enemy in scope searches (D-23).
            var b = SystemsBalance.Current;
            float delay = highest == AlertState.Search ? b.searchFeedDelaySeconds : b.feedDelaySeconds;
            if (Time.time - stamps[head] >= FeedFrameSeconds)
            {
                head = (head + 1) % RingSize;
                cam.targetTexture = ring[head];
                stamps[head] = Time.time;
                cam.Render();
            }
            int show = head;
            for (int i = 1; i < RingSize; i++)
            {
                int idx = (head - i + RingSize) % RingSize;
                if (stamps[idx] <= 0f) break;
                show = idx;
                if (Time.time - stamps[idx] >= delay) break;
            }
            screen.texture = ring[show];
        }

        private const float StandOff = 40f;   // orthographic: distance only keeps the whole room in front of the near plane

        /// <summary>
        /// The mount gives the isometric angle; the camera then backs out of the room along its view and is centred on the
        /// room's floor so the room fills the feed (C-19). The near walls it looks through are hidden while it renders.
        /// </summary>
        private void FrameRoom(RoomInstance room)
        {
            if (!room.TryGetComponent(out Collider vol)) { cam.orthographicSize = orthoSize; return; }
            var b = vol.bounds;
            var t = cam.transform;
            t.position = new Vector3(b.center.x, b.min.y, b.center.z) - t.forward * StandOff;
            float aspect = (float)ring[0].width / ring[0].height;
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < 8; i++)
            {
                // Floor and wall tops of the room volume; the hidden near walls do not need to fit.
                var corner = new Vector3((i & 1) == 0 ? b.min.x : b.max.x, (i & 2) == 0 ? b.min.y : b.max.y, (i & 4) == 0 ? b.min.z : b.max.z);
                var p = t.InverseTransformPoint(corner);
                minX = Mathf.Min(minX, p.x); maxX = Mathf.Max(maxX, p.x); minY = Mathf.Min(minY, p.y); maxY = Mathf.Max(maxY, p.y);
            }
            t.position += t.right * ((minX + maxX) * 0.5f) + t.up * ((minY + maxY) * 0.5f);
            float half = Mathf.Max((maxY - minY) * 0.5f, (maxX - minX) * 0.5f / aspect);
            cam.orthographicSize = Mathf.Max(minOrthoSize, half);
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = StandOff * 3f;
        }

        private void CollectOutside(RoomInstance room)
        {
            outside.Clear();
            movers.Clear();
            var inner = room.TryGetComponent(out Collider vol) ? vol.bounds : new Bounds(room.transform.position, Vector3.one * 20f);
            viewBounds = inner;
            viewBounds.Expand(new Vector3(1f, 100f, 1f));   // the room's own walls sit on its edge
            // C-19: the two walls on the camera's side would block the view: hidden like everything outside the room.
            var fwd = cam.transform.forward;
            bool NearWall(Bounds rb) => rb.size.y > 1f &&
                ((fwd.x > 0.1f && rb.center.x < inner.min.x + 0.6f) || (fwd.x < -0.1f && rb.center.x > inner.max.x - 0.6f) ||
                 (fwd.z > 0.1f && rb.center.z < inner.min.z + 0.6f) || (fwd.z < -0.1f && rb.center.z > inner.max.z - 0.6f));
            foreach (var r in FindObjectsByType<Renderer>(FindObjectsInactive.Exclude))
            {
                if (r.GetComponentInParent<AI.EnemyBrain>() != null) { movers.Add(r); continue; }   // they walk in and out
                if (r.GetComponentInParent<PlayerController>() != null) continue;
                if (!viewBounds.Contains(r.bounds.center) || NearWall(r.bounds)) outside.Add(r);
            }
        }

        private void OnBeginCamera(ScriptableRenderContext _, Camera c)
        {
            if (c != cam || !IsOpen) return;
            foreach (var r in outside) if (r != null) r.forceRenderingOff = true;
            foreach (var r in movers) if (r != null) r.forceRenderingOff = !viewBounds.Contains(r.bounds.center);
        }

        private void OnEndCamera(ScriptableRenderContext _, Camera c)
        {
            if (c != cam) return;
            foreach (var r in outside) if (r != null) r.forceRenderingOff = false;
            foreach (var r in movers) if (r != null) r.forceRenderingOff = false;
        }

        // M-03: a hunt in Lukas's room or a connected room (the AlertDirector's scope) cuts and locks the view;
        // it unlocks when that enemy dies, loses the hunt or is left behind across a door.
        private void OnHighestAlert(AlertState s)
        {
            highest = s;
            if (s != AlertState.Hunt) return;
            if (IsOpen) ShowNoSignal();
            Close("Hunt");
        }

        private static bool BossCamerasHostile =>
            Services.TryGet(out Boss.MotherDirector m) && m.Fight != null && m.Fight.Phase == Boss.MotherPhase.Cameras;

        private void OnBossPhase(int phase) { if (phase == (int)Boss.MotherPhase.Cameras) Close("BossCameras"); }   // D-28

        private void ShowNoSignal()
        {
            noSignal.SetActive(true);
            noSignalUntil = Time.unscaledTime + NoSignalSeconds;
        }

        private void OnRoomEntered(MonoBehaviour room)
        {
            if (IsOpen && room != viewingRoom) Close("LeftRoom");
        }

        private void OnAim(bool aiming) { if (aiming) Close("Aim"); }
        private void OnAttack(InputAction.CallbackContext _) { if (IsOpen) Close("Shove"); }
        private void OnShot(Vector3 _) => Attention.Add(SystemsBalance.Current.shotAttention);

        /// <summary>Damage cuts the view (M-03); subscribed to PlayerHealth.Hit while open.</summary>
        private void OnPlayerHit() => Close("Damage");

        private void OnAttentionFull()
        {
            if (player == null && !Services.TryGet(out player)) return;
            HbLog.Info("Surveillance", "The Mother's attention is full");
            GameEvents.RaiseAttentionFull(player.transform.position);
        }

        // ---------- Setup ----------

        private void BuildCameraAndOverlay()
        {
            var go = new GameObject("SurveillanceCamera");
            go.transform.SetParent(transform, false);
            cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = orthoSize;
            cam.cullingMask = ~LayerMask.GetMask("Ceiling", "RoomVolume");
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.enabled = false;
            go.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

            int w = Mathf.RoundToInt(textureHeight * 16f / 9f);
            ring = new RenderTexture[RingSize];
            stamps = new float[RingSize];
            for (int i = 0; i < RingSize; i++)
            {
                ring[i] = new RenderTexture(w, textureHeight, 16) { filterMode = FilterMode.Bilinear, name = $"Surveillance_{i}" };
                ring[i].Create();
            }

            var canvasGo = new GameObject("SurveillanceOverlay", typeof(Canvas));
            canvasGo.transform.SetParent(transform, false);
            UI.UiKit.Canvas(canvasGo, 50);
            screen = MakeFull<RawImage>(canvasGo.transform, "Feed");
            screen.color = Color.white;   // C-19: no green monitor tint; the CRT look is scanlines and noise
            screen.raycastTarget = false;
            noise = MakeFull<Image>(canvasGo.transform, "CRTNoise");
            noise.color = new Color(0.85f, 0.85f, 0.85f, 0.2f);
            noise.raycastTarget = false;
            screen.enabled = noise.enabled = false;

            var liveRoot = UI.UiKit.Stretch(UI.UiKit.Rect("Live", canvasGo.transform), Vector2.zero, Vector2.one);
            UI.UiKit.Scanlines(liveRoot);
            var label = UI.UiKit.Text(liveRoot, UI.Loc.Get("UI.SURV.LIVE"), 32, UI.UiKit.Green, true);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = label.rectTransform.pivot = new Vector2(0f, 1f);
            label.rectTransform.anchoredPosition = new Vector2(48f, -36f);
            label.rectTransform.sizeDelta = new Vector2(400f, 48f);
            live = liveRoot.gameObject;
            live.SetActive(false);

            var cut = MakeFull<Image>(canvasGo.transform, "NoSignal");
            cut.color = new Color(0f, 0f, 0f, 0.55f);   // brief and see-through: a hunt has just started
            cut.raycastTarget = false;
            var text = UI.UiKit.Text(cut.transform, UI.Loc.Get("UI.SURV.NO_SIGNAL"), 64, UI.UiKit.Red, true);
            UI.UiKit.Stretch(text.rectTransform, Vector2.zero, Vector2.one);
            text.alignment = TMPro.TextAlignmentOptions.Center;
            noSignal = cut.gameObject;
            noSignal.SetActive(false);
        }

        private static T MakeFull<T>(Transform parent, string name) where T : Graphic
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(T));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return go.GetComponent<T>();
        }
    }
}
