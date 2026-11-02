using System.Collections.Generic;
using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Inventory;
using Heartbeat.Narrative;
using Heartbeat.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// The arm terminal (M-07, M-08, M-16, D-86): Tab opens it in real time (never pauses), Inventory context.
    /// Tabs: INVENTORY (6 slots; left click / A = use, right click / X = discard, ~1.5 s), STATUS (ECG by health
    /// state + HEALTH label, flashlight charge) and LOGS (registry by category, reader). Teal phosphor screen with a
    /// ~0.2 s CRT power-on. The widget tree is built in code under this RectTransform; the prefab only holds the root.
    /// shortcut: screen-space panel only; the world-space forearm canvas (10-ui.md) comes with the Lukas model.
    /// </summary>
    public sealed class TerminalView : MonoBehaviour
    {
        [SerializeField] private DocumentDefinition[] documents = new DocumentDefinition[0];
        [SerializeField] private float baseFontSize = 26f;

        private static Color Teal => UiKit.Teal;
        private static Color TealDim => UiKit.TealDim;
        private static Color Amber => UiKit.Amber;
        private static Color Red => UiKit.Red;
        private const int EcgBars = 64;

        private InputRouter router;
        private RectTransform panel;
        private TMP_Text header, healthLabel, flashLabel, logsList, readerTitle, readerBody, actionHint, emiliaCount;
        private Image flashFill;
        private readonly List<RectTransform> ecg = new List<RectTransform>();
        private RectTransform ecgStrip;
        private readonly GameObject[] pages = new GameObject[3];
        private readonly TMP_Text[] slotLabels = new TMP_Text[InventoryModel.SlotCount];
        private readonly Image[] slotFrames = new Image[InventoryModel.SlotCount];
        private readonly List<string> logIds = new List<string>();
        private readonly Dictionary<string, DocumentDefinition> docsById = new Dictionary<string, DocumentDefinition>();
        private Transform logButtons;
        private int tab, focusedSlot = -1;
        private float openTime;
        private float textScale = 1f;
        private bool open;

        private PlayerInventory inventory;
        private PlayerHealth health;
        private Flashlight flashlight;
        private Revolver revolver;

        public bool IsOpen => open;

        private void Awake()
        {
            foreach (var d in documents) if (d != null) docsById[d.id] = d;
            Build();
            ApplyTextScale();
            gameObject.AddComponent<HackView>();   // M-13: the hack chain lives on the arm terminal
            panel.gameObject.SetActive(false);
        }

        private void Start()
        {
            router = Services.Get<InputRouter>();
            router.Actions.Gameplay.Terminal.performed += OnToggle;
            router.Actions.Inventory.Close.performed += OnToggle;
            router.Actions.Inventory.Use.performed += OnUse;
            router.Actions.Inventory.Discard.performed += OnDiscard;
        }

        private void OnEnable()
        {
            GameEvents.GameStateChanged += OnState;
            GameEvents.InventoryChanged += Refresh;
            GameEvents.ItemRegistered += OnRegistered;
            GameEvents.PlayerDied += OnDied;
            GameEvents.OptionsChanged += ApplyTextScale;
        }

        private void OnDisable()
        {
            GameEvents.GameStateChanged -= OnState;
            GameEvents.InventoryChanged -= Refresh;
            GameEvents.ItemRegistered -= OnRegistered;
            GameEvents.PlayerDied -= OnDied;
            GameEvents.OptionsChanged -= ApplyTextScale;
        }

        private void OnDestroy()
        {
            if (router == null) return;
            router.Actions.Gameplay.Terminal.performed -= OnToggle;
            router.Actions.Inventory.Close.performed -= OnToggle;
            router.Actions.Inventory.Use.performed -= OnUse;
            router.Actions.Inventory.Discard.performed -= OnDiscard;
        }

        // ---------- Open / close ----------

        private void OnToggle(InputAction.CallbackContext _) { if (open) Close(); else TryOpen(); }

        public bool TryOpen()
        {
            if (open || router.Context != InputContext.Exploration) return false;   // not while aiming, hacking, surveilling
            if (!Services.TryGet(out PlayerController player)) return false;
            inventory = player.GetComponent<PlayerInventory>();
            health = player.GetComponent<PlayerHealth>();
            flashlight = player.GetComponentInChildren<Flashlight>();
            revolver = player.GetComponentInChildren<Revolver>();
            open = true;
            openTime = Time.unscaledTime;
            ApplyTextScale();
            panel.gameObject.SetActive(true);
            router.SetContext(InputContext.Inventory);
            Sfx.Play2D(Sfx.TerminalOn);
            ShowTab(tab);
            HbLog.Info("Terminal", "Open");
            return true;
        }

        public void Close()
        {
            if (!open) return;
            open = false;
            panel.gameObject.SetActive(false);
            if (router.Context == InputContext.Inventory) router.SetContext(InputContext.Exploration);
            EventSystem.current?.SetSelectedGameObject(null);
            HbLog.Info("Terminal", "Closed");
        }

        private void OnState(GameStateId _, GameStateId to) { if (to != GameStateId.Playing) Close(); }
        private void OnDied(DeathCause _) => Close();

        // ---------- Inventory actions ----------

        private void OnUse(InputAction.CallbackContext _)
        {
            if (!open || tab != 0 || focusedSlot < 0 || inventory == null) return;
            if (inventory.TryUseSlot(focusedSlot)) Refresh();
        }

        private void OnDiscard(InputAction.CallbackContext _)
        {
            if (!open || tab != 0 || focusedSlot < 0 || inventory == null) return;
            if (inventory.TryDiscardSlot(focusedSlot)) Refresh();
        }

        private void Focus(int slot)
        {
            focusedSlot = slot;
            for (int i = 0; i < slotFrames.Length; i++) slotFrames[i].color = i == slot ? TealDim : new Color(0, 0, 0, 0.4f);
        }

        // ---------- Refresh ----------

        private void OnRegistered(string id, int kind, bool isNew) { if (open) Refresh(); }

        private void Refresh()
        {
            if (!open) return;
            string subject = Services.TryGet(out Cycle.CycleController cycle) && cycle.Data != null ? cycle.Data.SubjectLabel : "R-H-??";
            string drum = revolver != null ? "   ·   " + Loc.Get("UI.TERM.DRUM", revolver.Model.Loaded) : "";
            header.text = Loc.Get("UI.TERM.SUBJECT", subject) + drum;

            if (inventory != null)
                for (int i = 0; i < InventoryModel.SlotCount; i++)
                {
                    var s = inventory.Model[i];
                    slotLabels[i].text = s.IsEmpty ? Loc.Get("UI.TERM.EMPTY_SLOT")
                        : s.Kind == ItemKind.Ammo ? $"{Loc.Get("UI.ITEM." + s.Kind)}  x{s.Count}" : Loc.Get("UI.ITEM." + s.Kind);
                    slotLabels[i].color = s.IsEmpty ? TealDim : Teal;
                }
            actionHint.text = inventory != null && inventory.IsUsingItem ? "…" : $"[LMB] {Loc.Get("UI.TERM.USE")}    [RMB] {Loc.Get("UI.TERM.DISCARD")}";
            RefreshLogs();
        }

        private void RefreshLogs()
        {
            var registry = Services.Get<RegistryModel>();
            emiliaCount.text = Loc.Get("UI.TERM.EMILIA_COUNT", registry.EmiliaCount);
            if (logIds.Count == registry.Count) return;
            logIds.Clear();
            foreach (Transform c in logButtons) Destroy(c.gameObject);
            // Grouped by category, in registration order inside each.
            foreach (RecordKind kind in System.Enum.GetValues(typeof(RecordKind)))
                foreach (var id in registry.Order)
                {
                    if (registry.KindOf(id) != kind) continue;
                    logIds.Add(id);
                    string captured = id;
                    var b = MakeButton(logButtons, $"{CategoryTag(kind)}  {TitleOf(id)}", () => Read(captured), 0.82f);
                    b.GetComponent<LayoutElement>().preferredHeight = 34;
                    b.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
                }
            logsList.text = registry.Count == 0 ? Loc.Get("UI.TERM.NO_LOGS") : "";
        }

        private static string CategoryTag(RecordKind k) => k switch
        {
            RecordKind.EmiliaDocument => "[E]",
            RecordKind.ContextDocument => "[D]",
            RecordKind.Confiscated => "[C]",
            RecordKind.Bestiary => "[B]",
            RecordKind.Rule => "[R]",
            RecordKind.Fault => "[F]",
            _ => "[*]",
        };

        private string TitleOf(string id)
        {
            if (!docsById.TryGetValue(id, out var d)) return id;
            string t = Loc.Get(d.titleKey);
            return t == d.titleKey && !string.IsNullOrEmpty(d.placeholderTitle) ? d.placeholderTitle : t;
        }

        private void Read(string id)
        {
            readerTitle.text = TitleOf(id);
            // {0} = Lukas's current subject number; {c} passages stay corrupt until the ending reveals them (D-127, D-55).
            var cycle = Services.TryGet(out Cycle.CycleController c) ? c.Data : null;
            string n = cycle != null ? cycle.subjectCode.ToString("00") : "07";
            bool reveal = cycle != null && cycle.cycleCompleted && Services.Get<RegistryModel>().HasAllEmilia;
            readerBody.text = docsById.TryGetValue(id, out var d) ? DocText.Render(Loc.Get(d.bodyKey, n), reveal) : "";
            if (Level.FaultModel.Faults.ContainsKey(id) && Services.TryGet(out Level.FaultModel faults))   // M-17 status
                readerBody.text += "\n\n" + (faults.IsPatched(id) ? Loc.Get("UI.TERM.FAULT_PATCHED") : Loc.Get("UI.TERM.FAULT_USES", faults.UsesLeft(id)));
            HbLog.Info("Terminal", $"Read {id}");
        }

        private void Update()
        {
            if (!open) return;
            float k = Mathf.Clamp01((Time.unscaledTime - openTime) / 0.2f);   // CRT power-on
            panel.localScale = new Vector3(textScale, textScale * Mathf.Max(0.02f, k * k), 1f);
            if (tab == 1) UpdateStatus();
            if (inventory != null && inventory.IsUsingItem) actionHint.text = new string('|', 1 + (int)(inventory.UseProgress * 20));
            else if (actionHint.text.StartsWith("|")) Refresh();
        }

        private void UpdateStatus()
        {
            var state = health != null ? health.State : HealthState.Fine;
            Color c = state == HealthState.Critical ? Red : state == HealthState.Injured ? Amber : Teal;
            bool blink = state != HealthState.Fine && Mathf.Repeat(Time.unscaledTime, 1f) < 0.5f;
            healthLabel.text = Loc.Get(state == HealthState.Critical ? "UI.TERM.HEALTH_CRITICAL"
                : state == HealthState.Injured ? "UI.TERM.HEALTH_INJURED" : "UI.TERM.HEALTH_FINE");
            healthLabel.color = blink ? c * 0.6f : c;
            float t = Time.unscaledTime;
            for (int i = 0; i < ecg.Count; i++)
            {
                float v = EcgSample(state, t - (ecg.Count - i) * 0.03f);
                // Bars fit their strip (clipped too): they used to overflow onto the HEALTH line every beat.
                float room = ecgStrip != null ? Mathf.Max(8f, ecgStrip.rect.height - 8f) : 110f;
                ecg[i].sizeDelta = new Vector2(ecg[i].sizeDelta.x, 4f + (room - 4f) * v);
                ecg[i].GetComponent<Image>().color = c;
            }

            if (flashlight != null)
            {
                var m = flashlight.Model;
                flashFill.fillAmount = m.Charge;
                flashFill.color = m.IsLocked ? Red : m.Charge < 0.2f ? Amber : Teal;
                flashLabel.text = m.IsLocked ? Loc.Get("UI.TERM.FLASHLIGHT_LOCKED") : Loc.Get("UI.TERM.FLASHLIGHT", Mathf.RoundToInt(m.Charge * 100f));
            }
        }

        /// <summary>ECG shape (Interfaz Visual): regular when Fine, irregular when Injured, erratic when Critical.</summary>
        public static float EcgSample(HealthState state, float t)
        {
            float period = state == HealthState.Critical ? 0.55f : state == HealthState.Injured ? 0.8f : 1f;
            float jitter = state == HealthState.Injured ? 0.15f * Mathf.Sin(Mathf.Floor(t / period) * 12.9898f) : 0f;
            float p = Mathf.Repeat(t / period + jitter, 1f);
            float beat = p < 0.08f ? Mathf.Sin(p / 0.08f * Mathf.PI) : p > 0.3f && p < 0.4f ? 0.2f * Mathf.Sin((p - 0.3f) / 0.1f * Mathf.PI) : 0f;
            if (state == HealthState.Critical) beat = Mathf.Max(beat * 0.7f, 0.35f * Mathf.PerlinNoise(t * 9f, 0.3f));
            return Mathf.Clamp01(beat);
        }

        // ---------- Tabs ----------

        private void ShowTab(int i)
        {
            tab = i;
            for (int p = 0; p < pages.Length; p++) pages[p].SetActive(p == i);
            if (i != 0) Focus(-1);
            Refresh();
        }

        /// <summary>
        /// Accessibility text size (subtitles and terminal, Accesibilidad): the whole panel scales from its top-left
        /// corner; its unscaled height shrinks by the same factor so it keeps covering 84% of the screen height.
        /// </summary>
        private void ApplyTextScale()
        {
            if (panel == null) return;
            float k = Services.TryGet(out Save.SaveService saves) ? Mathf.Max(1f, saves.LoadGlobal().textScale) : 1f;
            panel.pivot = new Vector2(0f, 1f);
            panel.anchorMin = new Vector2(0.03f, 0.92f - 0.84f / k);
            panel.anchorMax = new Vector2(0.48f, 0.92f);
            panel.offsetMin = panel.offsetMax = Vector2.zero;
            textScale = k;
            panel.localScale = new Vector3(k, k, 1f);
        }

        // ---------- Construction ----------

        private void Build()
        {
            panel = NewRect("Screen", (RectTransform)transform);
            panel.anchorMin = new Vector2(0.03f, 0.08f);
            panel.anchorMax = new Vector2(0.48f, 0.92f);
            panel.offsetMin = panel.offsetMax = Vector2.zero;
            panel.pivot = new Vector2(0.5f, 0.5f);
            var bg = panel.gameObject.AddComponent<Image>();
            bg.color = new Color(0.01f, 0.05f, 0.05f, 0.92f);
            var outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = TealDim;
            outline.effectDistance = new Vector2(2, -2);

            var v = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            v.padding = new RectOffset(24, 24, 18, 18);
            v.spacing = 12;
            v.childControlHeight = v.childControlWidth = true;
            v.childForceExpandHeight = false;

            header = MakeText(panel, "", 1f);
            var tabs = NewRect("Tabs", panel);
            var h = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 8;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandHeight = false;
            var tabsLe = tabs.gameObject.AddComponent<LayoutElement>();
            tabsLe.preferredHeight = 44;
            tabsLe.flexibleHeight = 0;
            string[] keys = { "UI.TERM.TAB_INVENTORY", "UI.TERM.TAB_STATUS", "UI.TERM.TAB_LOGS" };
            for (int i = 0; i < 3; i++) { int ix = i; MakeButton(tabs, Loc.Get(keys[i]), () => ShowTab(ix), 1f); }

            // Inventory page
            pages[0] = NewPage("Inventory").gameObject;
            for (int i = 0; i < InventoryModel.SlotCount; i++)
            {
                int ix = i;
                var b = MakeButton(pages[0].transform, "", () => Focus(ix), 1f);
                b.GetComponent<LayoutElement>().preferredHeight = 52;
                slotLabels[i] = b.GetComponentInChildren<TMP_Text>();
                slotLabels[i].alignment = TextAlignmentOptions.MidlineLeft;
                slotFrames[i] = b.GetComponent<Image>();
                var trigger = b.gameObject.AddComponent<EventTrigger>();
                AddTrigger(trigger, EventTriggerType.PointerEnter, () => Focus(ix));
                AddTrigger(trigger, EventTriggerType.Select, () => Focus(ix));
            }
            actionHint = MakeText(pages[0].transform, "", 0.8f);

            // Status page
            var status = NewPage("Status");
            pages[1] = status.gameObject;
            healthLabel = MakeText(status, "", 1.4f);
            // VT323 glyphs are taller than the size TMP lays them out with: reserve the line so the ECG never sits on it.
            healthLabel.gameObject.AddComponent<LayoutElement>().minHeight = baseFontSize * 1.4f * 1.6f;
            var strip = NewRect("ECG", status);
            var stripLayout = strip.gameObject.AddComponent<LayoutElement>();
            stripLayout.minHeight = stripLayout.preferredHeight = 130;
            strip.gameObject.AddComponent<RectMask2D>();
            ecgStrip = strip;
            var sh = strip.gameObject.AddComponent<HorizontalLayoutGroup>();
            sh.spacing = 2;
            sh.childAlignment = TextAnchor.MiddleLeft;
            sh.childControlHeight = false;
            sh.childControlWidth = true;
            sh.childForceExpandHeight = false;
            for (int i = 0; i < EcgBars; i++)
            {
                var bar = NewRect("b", strip);
                bar.sizeDelta = new Vector2(4, 4);
                bar.gameObject.AddComponent<Image>().color = Teal;
                ecg.Add(bar);
            }
            flashLabel = MakeText(status, "", 1f);
            var barBg = NewRect("FlashBar", status);
            barBg.gameObject.AddComponent<LayoutElement>().preferredHeight = 18;
            barBg.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.6f);
            var fill = NewRect("Fill", barBg);
            fill.anchorMin = Vector2.zero; fill.anchorMax = Vector2.one; fill.offsetMin = fill.offsetMax = Vector2.zero;
            flashFill = fill.gameObject.AddComponent<Image>();
            flashFill.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
            flashFill.type = Image.Type.Filled;
            flashFill.fillMethod = Image.FillMethod.Horizontal;

            // Logs page
            var logs = NewPage("Logs");
            pages[2] = logs.gameObject;
            emiliaCount = MakeText(logs, "", 1f);
            logsList = MakeText(logs, "", 0.9f);
            logButtons = NewRect("Entries", logs);
            var lv = logButtons.gameObject.AddComponent<VerticalLayoutGroup>();
            lv.spacing = 4;
            lv.childControlHeight = lv.childControlWidth = true;
            lv.childForceExpandHeight = false;
            readerTitle = MakeText(logs, "", 1.1f);
            readerBody = MakeText(logs, "", 0.9f);
            readerBody.textWrappingMode = TextWrappingModes.Normal;
        }

        private RectTransform NewPage(string name)
        {
            var p = NewRect(name, panel);
            var v = p.gameObject.AddComponent<VerticalLayoutGroup>();
            v.spacing = 8;
            v.childControlHeight = v.childControlWidth = true;
            v.childForceExpandHeight = false;
            p.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;
            return p;
        }

        private static RectTransform NewRect(string name, Transform parent) => UiKit.Rect(name, parent);

        private TMP_Text MakeText(Transform parent, string text, float scale) => UiKit.Text(parent, text, baseFontSize * scale, Teal);

        private Button MakeButton(Transform parent, string text, System.Action onClick, float scale) =>
            UiKit.Button(parent, text, baseFontSize * scale, Teal, onClick);

        private static void AddTrigger(EventTrigger t, EventTriggerType type, System.Action a)
        {
            var e = new EventTrigger.Entry { eventID = type };
            e.callback.AddListener(_ => a());
            t.triggers.Add(e);
        }
    }
}
