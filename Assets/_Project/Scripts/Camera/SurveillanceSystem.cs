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
        [SerializeField] private int textureHeight = 270;
        [SerializeField, Range(0f, 1f)] private float delaySeconds = 0.3f;
        [SerializeField] private float orthoSize = 7f;
        [SerializeField] private float shotAttention = 0.15f;
        [Tooltip("Flat ambient light while the feed is up: the camera image covers the whole screen, so only the feed sees it (M-02 legibility).")]
        [SerializeField] private Color viewAmbient = new Color(0.32f, 0.36f, 0.34f);

        private UnityEngine.Rendering.AmbientMode savedAmbientMode;
        private Color savedAmbient;

        private const int RingSize = 24;

        private InputRouter router;
        private GameStateMachine states;
        private PlayerController player;
        private Camera cam;
        private RenderTexture[] ring;
        private float[] stamps;
        private int head;
        private RawImage screen;
        private Image noise;
        private RoomInstance viewingRoom;
        // M-02: "only what the room's camera covers" — everything outside the viewed room is hidden from this camera.
        private readonly List<Renderer> outside = new List<Renderer>();
        private readonly List<Renderer> movers = new List<Renderer>();
        private Bounds viewBounds;
        private bool lockedByHunt;
        private AlertState highest;

        public AttentionMeter Attention { get; } = new AttentionMeter();
        public bool IsOpen => viewingRoom != null;
        public bool IsLocked => lockedByHunt;
        public float Noise { get; private set; }

        private void OnEnable()
        {
            Services.Register(this);
            GameEvents.HighestAlertChanged += OnHighestAlert;
            GameEvents.HuntChanged += OnHunt;
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
            GameEvents.HuntChanged -= OnHunt;
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
            if (lockedByHunt || highest == AlertState.Hunt) { HbLog.Info("Surveillance", "Locked: hunt in progress"); return false; }
            var room = RoomInstance.Current;
            var mount = MountFor(room);
            if (mount == null) { HbLog.Info("Surveillance", "No camera in this room"); return false; }

            viewingRoom = room;
            CollectOutside(room);
            cam.transform.SetPositionAndRotation(mount.position, mount.rotation);
            cam.enabled = true;
            screen.enabled = true;
            noise.enabled = true;
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
            if (room.CameraMount != null) return room.CameraMount;
            if (room.Definition == null || !room.Definition.hasCamera) return null;
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
            if (!IsOpen) return;
            if (player != null && player.IsRunning) { Close("Run"); return; }

            float baseNoise = 0.15f + Attention.Value * 0.35f;
            Noise = highest == AlertState.Search ? baseNoise + 0.35f : baseNoise;
            var c = noise.color;
            c.a = Mathf.Clamp01(Noise * 0.5f * (0.7f + 0.3f * Random.value));
            noise.color = c;
        }

        private void LateUpdate()
        {
            if (!IsOpen) return;
            // Ring buffer: render into the next slot, show the newest frame that is at least delaySeconds old.
            head = (head + 1) % RingSize;
            cam.targetTexture = ring[head];
            stamps[head] = Time.time;
            cam.Render();
            int show = head;
            for (int i = 1; i < RingSize; i++)
            {
                int idx = (head - i + RingSize) % RingSize;
                if (stamps[idx] <= 0f) break;
                show = idx;
                if (Time.time - stamps[idx] >= delaySeconds) break;
            }
            screen.texture = ring[show];
        }

        private void CollectOutside(RoomInstance room)
        {
            outside.Clear();
            movers.Clear();
            viewBounds = room.TryGetComponent(out Collider vol) ? vol.bounds : new Bounds(room.transform.position, Vector3.one * 20f);
            viewBounds.Expand(new Vector3(1f, 100f, 1f));   // the room's own walls sit on its edge
            foreach (var r in FindObjectsByType<Renderer>(FindObjectsInactive.Exclude))
            {
                if (r.GetComponentInParent<AI.EnemyBrain>() != null) { movers.Add(r); continue; }   // they walk in and out
                if (r.transform.IsChildOf(room.transform) || r.GetComponentInParent<PlayerController>() != null) continue;
                if (!viewBounds.Contains(r.bounds.center)) outside.Add(r);
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

        private void OnHighestAlert(AlertState s)
        {
            highest = s;
            if (s == AlertState.Hunt) OnHunt(true);
        }

        // Any hunt on the floor locks the view, not only nearby ones (M-03).
        private void OnHunt(bool hunting)
        {
            lockedByHunt = hunting;
            if (hunting) Close("Hunt");
        }

        private void OnRoomEntered(MonoBehaviour room)
        {
            if (IsOpen && room != viewingRoom) Close("LeftRoom");
            lockedByHunt = false;   // crossing a door releases the lock (M-03)
        }

        private void OnAim(bool aiming) { if (aiming) Close("Aim"); }
        private void OnAttack(InputAction.CallbackContext _) { if (IsOpen) Close("Shove"); }
        private void OnShot(Vector3 _) => Attention.Add(shotAttention);

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
                ring[i] = new RenderTexture(w, textureHeight, 16) { filterMode = FilterMode.Point, name = $"Surveillance_{i}" };
                ring[i].Create();
            }

            var canvasGo = new GameObject("SurveillanceOverlay", typeof(Canvas));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            screen = MakeFull<RawImage>(canvasGo.transform, "Feed");
            screen.color = new Color(0.75f, 1f, 0.8f);   // phosphor tint
            noise = MakeFull<Image>(canvasGo.transform, "CRTNoise");
            noise.color = new Color(0.6f, 0.9f, 0.7f, 0.2f);
            noise.raycastTarget = false;
            screen.enabled = noise.enabled = false;
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
