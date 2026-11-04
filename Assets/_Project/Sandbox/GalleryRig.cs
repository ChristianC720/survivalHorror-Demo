using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Heartbeat.Sandbox
{
    /// <summary>
    /// Viewer and developer options for Sandbox_Gallery. Editor-only assembly (see Heartbeat.Sandbox.asmdef), so none of this
    /// reaches a player build. It reads devices directly and draws with IMGUI on purpose: it must not touch the game's
    /// InputRouter, input asset or UI. Panel text is plain Spanish: a tool for the team, not game text.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class GalleryRig : MonoBehaviour
    {
        private const float PanelWidth = 400f, LabelRange = 16f, CellLabelRange = 40f, Gravity = 18f, WalkSpeed = 3f;
        private static readonly string[] Tabs = { "Ir a", "Asset", "Luz", "Vista", "Audio" };
        private static readonly (string name, Color light, Color ambient, float intensity)[] Lights =
        {
            ("Neutra", new Color(1f, .98f, .95f), new Color(.5f, .5f, .52f), 1f),
            ("F1 ámbar de sodio", new Color(1f, .68f, .3f), new Color(.2f, .13f, .06f), .9f),
            ("F2 fluorescente fría", new Color(.76f, .9f, 1f), new Color(.14f, .18f, .22f), 1f),
            ("F3 carmesí", new Color(.85f, .13f, .16f), new Color(.14f, .03f, .04f), .9f),
            ("Oscuridad", Color.black, new Color(.015f, .015f, .02f), 0f),
        };

        private readonly GalleryCatalog catalog = new GalleryCatalog();
        private readonly Dictionary<Transform, GalleryItem> byRoot = new Dictionary<Transform, GalleryItem>();
        private readonly Dictionary<GalleryItem, PlayableGraph> graphs = new Dictionary<GalleryItem, PlayableGraph>();
        private readonly List<Light> assetLights = new List<Light>();
        private Camera cam;
        private Light sun, torch;
        private CharacterController walker;
        private AudioSource audioOut;
        private FieldInfo pixelHeight;
        private GalleryItem selected;
        private int tab, lightPreset, internalHeight;
        private bool showPanel = true, showLabels = true, walking, autoLight, assetLightsOn = true, spin, isolate;
        private float yaw, pitch = 12f, flySpeed = 6f, fallSpeed, smoothDt = .016f;
        private Vector2 scroll;
        private Rect panelRect = new Rect(10, 10, PanelWidth, 600);
        private GUIStyle tag;

        private void Start()
        {
            cam = GetComponent<Camera>();
            var content = new GameObject("Assets").transform;
            catalog.Build(content);
            foreach (var it in catalog.Items) byRoot[it.Root.transform] = it;
            assetLights.AddRange(content.GetComponentsInChildren<Light>(true));

            sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(48, 30, 0);
            torch = new GameObject("Flashlight").AddComponent<Light>();
            torch.type = LightType.Spot;
            torch.spotAngle = 50;
            torch.range = 30;
            torch.intensity = 40;
            torch.color = new Color(1f, .95f, .85f);
            torch.transform.SetParent(transform, false);
            torch.enabled = false;
            walker = gameObject.AddComponent<CharacterController>();
            walker.enabled = false;
            walker.height = 1.7f;
            walker.radius = .3f;
            audioOut = gameObject.AddComponent<AudioSource>();
            // Soft link by name: internal resolution is a static of the render feature, and this assembly references no game code.
            var feature = System.AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Heartbeat.Render.PixelationFeature")).FirstOrDefault(t => t != null);
            pixelHeight = feature?.GetField("HeightOverride", BindingFlags.Public | BindingFlags.Static);

            ApplyLight(0);
            RenderSettings.fog = false;
            foreach (var it in catalog.Items)
            {
                if (it.BodyVariant != null)
                {
                    var block = new MaterialPropertyBlock();
                    block.SetTexture("_BaseMap", it.BodyVariant);
                    foreach (var r in it.Root.GetComponentsInChildren<SkinnedMeshRenderer>(true)) if (r.enabled) r.SetPropertyBlock(block);
                }
                var rest = it.Clips.FirstOrDefault(c => c.name.Contains("Idle_Fine")) ?? it.Clips.FirstOrDefault(c => c.name.EndsWith("Idle") || c.name.EndsWith("Still"));
                if (rest != null) Play(it, rest);
            }
            GoTo("Personajes", "Shared");
            Debug.Log($"[Heartbeat][Sandbox] Gallery: {catalog.Items.Count} assets in {catalog.Cells.Count} cells, {catalog.Skipped.Count} skipped{(catalog.Skipped.Count > 0 ? ": " + string.Join(", ", catalog.Skipped) : "")}");
        }

        private void OnDestroy()
        {
            foreach (var g in graphs.Values) if (g.IsValid()) g.Destroy();
            pixelHeight?.SetValue(null, null);
            Time.timeScale = 1f;
        }

        private void Update()
        {
            smoothDt = Mathf.Lerp(smoothDt, Time.unscaledDeltaTime, .05f);
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null || mouse == null) return;
            if (kb.f1Key.wasPressedThisFrame) showPanel = !showPanel;
            if (kb.fKey.wasPressedThisFrame && selected != null) Focus(selected);
            if (kb.lKey.wasPressedThisFrame) torch.enabled = !torch.enabled;
            if (kb.gKey.wasPressedThisFrame) SetWalking(!walking);
            if (kb.nKey.wasPressedThisFrame) Step(1);
            if (kb.bKey.wasPressedThisFrame) Step(-1);

            bool overPanel = showPanel && panelRect.Contains(new Vector2(mouse.position.x.ReadValue(), Screen.height - mouse.position.y.ReadValue()));
            if (mouse.rightButton.isPressed)
            {
                var d = mouse.delta.ReadValue() * .12f;
                yaw += d.x;
                pitch = Mathf.Clamp(pitch - d.y, -89f, 89f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            }
            if (!overPanel) flySpeed = Mathf.Clamp(flySpeed * (1f + mouse.scroll.ReadValue().y * .001f), .5f, 120f);
            if (mouse.leftButton.wasPressedThisFrame && !overPanel) Pick(mouse.position.ReadValue());

            var move = new Vector3((kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0), 0, (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0));
            float dt = Time.unscaledDeltaTime;
            if (walking)
            {
                fallSpeed = walker.isGrounded ? -1f : fallSpeed - Gravity * dt;
                var flat = Quaternion.Euler(0, yaw, 0) * move;
                walker.Move((flat * WalkSpeed * (kb.leftShiftKey.isPressed ? 2f : 1f) + Vector3.up * fallSpeed) * dt);
            }
            else
            {
                float up = (kb.eKey.isPressed ? 1 : 0) - (kb.qKey.isPressed ? 1 : 0);
                transform.position += (transform.rotation * move + Vector3.up * up) * flySpeed * (kb.leftShiftKey.isPressed ? 3f : 1f) * dt;
            }

            if (spin && selected != null) selected.Root.transform.RotateAround(selected.Bounds.center, Vector3.up, 40f * dt);
            if (autoLight) FollowFloorLight();
        }

        private void FollowFloorLight()
        {
            var at = transform.position;
            foreach (var pair in catalog.Cells)
            {
                var b = pair.Value;
                if (at.x < b.min.x - 4 || at.x > b.max.x + 4) continue;
                int preset = System.Array.IndexOf(GalleryCatalog.Floors, pair.Key.floor);
                if (preset != lightPreset) ApplyLight(preset);
                return;
            }
        }

        private void ApplyLight(int preset)
        {
            lightPreset = preset;
            var p = Lights[preset];
            sun.color = p.light;
            sun.intensity = p.intensity;
            sun.enabled = p.intensity > 0;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = p.ambient;
        }

        private void SetWalking(bool on)
        {
            walking = on;
            walker.enabled = on;
            fallSpeed = 0;
        }

        private void Teleport(Vector3 at, Vector3 lookAt)
        {
            bool was = walker.enabled;
            walker.enabled = false;
            transform.position = at;
            var look = Quaternion.LookRotation(lookAt - at).eulerAngles;
            yaw = look.y;
            pitch = look.x > 180 ? look.x - 360 : look.x;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            walker.enabled = was;
        }

        private void GoTo(string type, string floor)
        {
            if (!catalog.Cells.TryGetValue((type, floor), out var b)) return;
            float back = Mathf.Clamp(Mathf.Max(b.size.x, b.size.z) * .6f, 4f, 30f);
            Teleport(new Vector3(b.center.x, Mathf.Max(1.7f, back * .45f), b.min.z - back), b.center + Vector3.up);
        }

        private void Focus(GalleryItem it)
        {
            var c = it.Bounds.center;
            float r = Mathf.Max(it.Bounds.extents.magnitude, .12f);
            Teleport(c + new Vector3(.35f, .3f, -1f).normalized * Mathf.Max(r * 2.4f, .35f), c);
        }

        private void Pick(Vector2 screen)
        {
            if (!Physics.Raycast(cam.ScreenPointToRay(screen), out var hit, 3000f, ~0, QueryTriggerInteraction.Collide)) { selected = null; return; }
            for (var t = hit.collider.transform; t != null; t = t.parent)
                if (byRoot.TryGetValue(t, out var it)) { Select(it); return; }
        }

        private void Select(GalleryItem it)
        {
            selected = it;
            tab = 1;
            if (isolate) Isolate();
        }

        private void Step(int dir)
        {
            if (catalog.Items.Count == 0) return;
            int i = selected == null ? 0 : (catalog.Items.IndexOf(selected) + dir + catalog.Items.Count) % catalog.Items.Count;
            Select(catalog.Items[i]);
            Focus(selected);
        }

        private void Isolate()
        {
            foreach (var it in catalog.Items) it.Root.SetActive(!isolate || selected == null || it == selected);
        }

        private void Play(GalleryItem it, AnimationClip clip)
        {
            if (it.Animator == null) return;
            if (graphs.TryGetValue(it, out var old) && old.IsValid()) old.Destroy();
            AnimationPlayableUtilities.PlayClip(it.Animator, clip, out var graph);
            graphs[it] = graph;
        }

        // -------------------------------------------------------------- panel

        private void OnGUI()
        {
            tag ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12, normal = { textColor = Color.white } };
            if (showLabels) DrawLabels();
            GUI.Label(new Rect(Screen.width - 430, 8, 422, 64), $"{1f / smoothDt:0} fps · {catalog.Items.Count} assets\nF1 panel · RMB mirar · WASD/QE · clic elegir · F acercar · L linterna · G andar · N/B siguiente");
            if (showPanel) panelRect = GUI.Window(0, panelRect, DrawPanel, "Opciones de desarrollador");
        }

        private void DrawLabels()
        {
            var cp = transform.position;
            foreach (var pair in catalog.Cells)
            {
                var at = new Vector3(pair.Value.min.x, 3.2f, pair.Value.min.z);
                if (Vector3.Distance(cp, at) < CellLabelRange) Label(at, $"<b>{pair.Key.type} · {pair.Key.floor}</b>", 14);
            }
            foreach (var it in catalog.Items)
            {
                if (it.Root == null || !it.Root.activeInHierarchy) continue;
                var top = it.Bounds.center + Vector3.up * (it.Bounds.extents.y + .15f);
                if (Vector3.Distance(cp, top) < LabelRange) Label(top, it == selected ? $"<b>▶ {it.Name}</b>" : it.Name, 12);
            }
        }

        private void Label(Vector3 world, string text, int size)
        {
            var s = cam.WorldToScreenPoint(world);
            if (s.z <= 0) return;
            tag.fontSize = size;
            tag.richText = true;
            GUI.Label(new Rect(s.x - 120, Screen.height - s.y - 12, 240, 24), text, tag);
        }

        private void DrawPanel(int id)
        {
            tab = GUILayout.Toolbar(tab, Tabs);
            scroll = GUILayout.BeginScrollView(scroll);
            switch (tab)
            {
                case 0: DrawGoTo(); break;
                case 1: DrawSelection(); break;
                case 2: DrawLightTab(); break;
                case 3: DrawView(); break;
                case 4: DrawAudio(); break;
            }
            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawGoTo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(130));
            foreach (var f in GalleryCatalog.Floors) GUILayout.Label(f, GUILayout.Width(42));
            GUILayout.EndHorizontal();
            foreach (var type in GalleryCatalog.Types)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(type, GUILayout.Width(130));
                foreach (var f in GalleryCatalog.Floors)
                {
                    int n = catalog.Items.Count(i => i.Type == type && i.Floor == f);
                    GUI.enabled = n > 0;
                    if (GUILayout.Button(n > 0 ? n.ToString() : "-", GUILayout.Width(42))) GoTo(type, f);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
            }
            if (catalog.Skipped.Count > 0) GUILayout.Label("Omitidos:\n" + string.Join("\n", catalog.Skipped));
        }

        private void DrawSelection()
        {
            if (selected == null) { GUILayout.Label("Haz clic en un asset, o pulsa N."); return; }
            GUILayout.Label($"<b>{selected.Name}</b>\n{selected.Type} · {selected.Floor}\n{selected.Path}\n{selected.Triangles} triángulos · {selected.Bounds.size.x:0.00} × {selected.Bounds.size.y:0.00} × {selected.Bounds.size.z:0.00} m\n{string.Join(", ", selected.Materials)}", new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true });
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Acercar (F)")) Focus(selected);
            spin = GUILayout.Toggle(spin, "Girar");
            bool iso = GUILayout.Toggle(isolate, "Aislar");
            if (iso != isolate) { isolate = iso; Isolate(); }
            GUILayout.EndHorizontal();
            if (selected.Clips.Length == 0) return;
            GUILayout.Label("Animaciones:");
            foreach (var c in selected.Clips)
                if (GUILayout.Button($"{c.name}  ({c.length:0.0} s)")) Play(selected, c);
        }

        private void DrawLightTab()
        {
            for (int i = 0; i < Lights.Length; i++)
                if (GUILayout.Toggle(lightPreset == i, Lights[i].name) && lightPreset != i) ApplyLight(i);
            autoLight = GUILayout.Toggle(autoLight, "Cambiar con el piso donde estoy");
            torch.enabled = GUILayout.Toggle(torch.enabled, "Linterna (L)");
            bool on = GUILayout.Toggle(assetLightsOn, "Luces propias de salas y props");
            if (on != assetLightsOn)
            {
                assetLightsOn = on;
                foreach (var l in assetLights) if (l != null) l.enabled = on;
            }
            GUILayout.Label($"Sol: {sun.intensity:0.00}");
            sun.intensity = GUILayout.HorizontalSlider(sun.intensity, 0f, 3f);
        }

        private void DrawView()
        {
            showLabels = GUILayout.Toggle(showLabels, "Nombres sobre los assets");
            bool walk = GUILayout.Toggle(walking, "Andar con gravedad (G)");
            if (walk != walking) SetWalking(walk);
            GUILayout.Label($"Velocidad de vuelo: {flySpeed:0.0} m/s (rueda)");
            flySpeed = GUILayout.HorizontalSlider(flySpeed, .5f, 120f);
            GUILayout.Label($"Campo de visión: {cam.fieldOfView:0}°");
            cam.fieldOfView = GUILayout.HorizontalSlider(cam.fieldOfView, 20f, 100f);
            GUILayout.Label($"Velocidad del tiempo: {Time.timeScale:0.00}");
            Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0f, 2f);
            if (pixelHeight != null)
            {
                GUILayout.Label("Resolución interna:");
                int[] heights = { 0, 540, 1080, 2160 };
                string[] names = { "Juego", "540", "1080", "2160" };
                int picked = GUILayout.Toolbar(System.Array.IndexOf(heights, internalHeight), names);
                if (picked >= 0 && heights[picked] != internalHeight)
                {
                    internalHeight = heights[picked];
                    pixelHeight.SetValue(null, internalHeight == 0 ? null : (int?)internalHeight);
                }
            }
        }

        private void DrawAudio()
        {
            if (catalog.AudioClips.Length == 0) { GUILayout.Label("No hay clips de audio en el proyecto todavía."); return; }
            if (GUILayout.Button("Parar")) audioOut.Stop();
            foreach (var c in catalog.AudioClips)
                if (GUILayout.Button($"{c.name}  ({c.length:0.0} s)")) { audioOut.clip = c; audioOut.Play(); }
        }
    }
}
