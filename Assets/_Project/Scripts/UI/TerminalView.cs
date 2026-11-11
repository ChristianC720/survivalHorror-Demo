using System;
using System.Collections.Generic;
using System.Linq;
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
    /// The arm terminal (M-07, M-08, M-16, D-86): Tab opens it in real time (never pauses), Inventory context. A strip
    /// with pulse, health, light and drum stays visible in every tab. Tabs: INVENTORY (3×2 slots; use, discard, ~1.5 s;
    /// also the key swap of M-08), STATUS (ECG by health state, flashlight, counters) and LOGS (categories and entries;
    /// reading happens in <see cref="ReaderView"/>, which does stop the world, D-159). Teal phosphor with a ~0.2 s CRT
    /// power-on. The widget tree is built in code under this RectTransform; the prefab only holds the root.
    /// shortcut: screen-space panel only; the world-space forearm canvas (10-ui.md) comes with the Lukas model.
    /// </summary>
    public sealed class TerminalView : MonoBehaviour
    {
        [SerializeField] private DocumentDefinition[] documents = new DocumentDefinition[0];

        private static Color Teal => UiKit.Teal;
        private static readonly Color Dim = new Color(0.165f, 0.498f, 0.459f);   // teal/dim of the design: opaque, still legible
        private static Color Amber => UiKit.Amber;
        private static Color Red => UiKit.Red;
        private const int SlotSize = 176, EcgBars = 64;
        private const float ToastSeconds = 3f;

        private InputRouter router;
        private RectTransform panel, footer, toast;
        private TMP_Text header, stripHealth, stripLight, stripDrum, healthLabel, flashLabel, emiliaCount, confiscatedCount;
        private TMP_Text detailName, detailInfo, actionHint, swapAlert, noLogs, toastText;
        private UiEcg stripEcg, bigEcg;
        private Image flashFill;
        private ReaderView reader;
        private readonly GameObject[] pages = new GameObject[3];
        private readonly Button[] tabButtons = new Button[3];
        private readonly Button[] slotButtons = new Button[InventoryModel.SlotCount];
        private readonly TMP_Text[] slotNames = new TMP_Text[InventoryModel.SlotCount];
        private readonly TMP_Text[] slotCounts = new TMP_Text[InventoryModel.SlotCount];
        private readonly TMP_Text[] slotEmpty = new TMP_Text[InventoryModel.SlotCount];
        private readonly Image[] slotIcons = new Image[InventoryModel.SlotCount];
        private readonly Dictionary<RecordKind, Button> categoryButtons = new Dictionary<RecordKind, Button>();
        private readonly Dictionary<string, DocumentDefinition> docsById = new Dictionary<string, DocumentDefinition>();
        private Transform logButtons;
        private Button firstEntry;
        private RecordKind category;
        private int logsShownCount = -1;
        private RecordKind logsShownCategory;
        private int tab, focusedSlot = -1;
        private float openTime, toastUntil, scale = 1f;
        private bool open, reduceMotion, toastPending;

        // Key swap of M-08: which key is waiting on the floor.
        private bool swapping;
        private ItemKind swapKind;
        private int swapAmount;
        private Action swapDone;

        private int lastLight = -1, lastDrum = -2;
        private HealthState lastState = (HealthState)(-1);

        private PlayerInventory inventory;
        private PlayerHealth health;
        private Flashlight flashlight;
        private Revolver revolver;

        public bool IsOpen => open;

        private void Awake()
        {
            foreach (var d in documents) if (d != null) docsById[d.id] = d;
            Build();
            ApplyScale();
            gameObject.AddComponent<HackView>();   // M-13: the hack chain lives on the arm terminal
            reader = gameObject.AddComponent<ReaderView>();
            panel.gameObject.SetActive(false);
        }

        private void Start()
        {
            router = Services.Get<InputRouter>();
            router.Actions.Gameplay.Terminal.performed += OnToggle;
            router.Actions.Inventory.Close.performed += OnToggle;
            router.Actions.Inventory.Use.performed += OnUse;
            router.Actions.Inventory.Discard.performed += OnDiscard;
            router.DeviceChanged += OnDevice;
        }

        private void OnDevice(bool _) { if (open) RefreshFooter(); }   // keys follow the device on the fly (D-132)

        private void OnEnable()
        {
            GameEvents.GameStateChanged += OnState;
            GameEvents.InventoryChanged += Refresh;
            GameEvents.ItemRegistered += OnRegistered;
            GameEvents.PlayerDied += OnDied;
            GameEvents.OptionsChanged += ApplyScale;
            GameEvents.KeySwapOffered += OnKeySwap;
        }

        private void OnDisable()
        {
            GameEvents.GameStateChanged -= OnState;
            GameEvents.InventoryChanged -= Refresh;
            GameEvents.ItemRegistered -= OnRegistered;
            GameEvents.PlayerDied -= OnDied;
            GameEvents.OptionsChanged -= ApplyScale;
            GameEvents.KeySwapOffered -= OnKeySwap;
        }

        private void OnDestroy()
        {
            if (router == null) return;
            router.Actions.Gameplay.Terminal.performed -= OnToggle;
            router.Actions.Inventory.Close.performed -= OnToggle;
            router.Actions.Inventory.Use.performed -= OnUse;
            router.Actions.Inventory.Discard.performed -= OnDiscard;
            router.DeviceChanged -= OnDevice;
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
            ApplyScale();
            panel.gameObject.SetActive(true);
            router.SetContext(InputContext.Inventory);
            Sfx.Play2D(Sfx.TerminalOn);
            lastLight = -1; lastDrum = -2; lastState = (HealthState)(-1);
            ShowTab(tab);
            HbLog.Info("Terminal", "Open");
            return true;
        }

        public void Close()
        {
            if (!open) return;
            open = false;
            if (swapping)
            {
                HbLog.Info("Interact", $"{swapKind} stays on the floor");
                EndSwap();
            }
            panel.gameObject.SetActive(false);
            if (router.Context == InputContext.Inventory) router.SetContext(InputContext.Exploration);
            EventSystem.current?.SetSelectedGameObject(null);
            HbLog.Info("Terminal", "Closed");
        }

        private void OnState(GameStateId _, GameStateId to) { if (to != GameStateId.Playing) Close(); }
        private void OnDied(DeathCause _) => Close();

        // ---------- Inventory actions ----------

        private bool SlotUnderFocus =>
            focusedSlot >= 0 && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == slotButtons[focusedSlot].gameObject;

        private void OnUse(InputAction.CallbackContext _)
        {
            if (!open || tab != 0 || inventory == null || !SlotUnderFocus) return;
            if (swapping) ReplaceFocused();
            else if (inventory.TryUseSlot(focusedSlot)) Refresh();
        }

        private void OnDiscard(InputAction.CallbackContext _)
        {
            if (!open || tab != 0 || swapping || inventory == null || !SlotUnderFocus) return;
            if (inventory.TryDiscardSlot(focusedSlot)) Refresh();
        }

        private void Focus(int slot)
        {
            focusedSlot = slot;
            RefreshDetail();
        }

        // ---------- Key swap (M-08) ----------

        private void OnKeySwap(ItemKind kind, int amount, Action replaced)
        {
            if (!TryOpen())
            {
                HbLog.Info("Interact", $"Inventory full: {kind} stays on the floor");
                GameEvents.RaiseInventoryFull(kind.ToString());
                return;
            }
            swapping = true;
            swapKind = kind;
            swapAmount = amount;
            swapDone = replaced;
            swapAlert.text = Loc.Get("UI.TERM.FULL_SWAP", Loc.Get("UI.TERM.SLOT_" + kind));
            swapAlert.gameObject.SetActive(true);
            foreach (var b in tabButtons) b.interactable = false;
            ShowTab(0);
            HbLog.Info("Interact", $"Inventory full: offering to replace a slot with {kind}");
        }

        private void ReplaceFocused()
        {
            var s = inventory.Model.Replace(focusedSlot, swapKind, swapAmount);
            // Like a discard, the replaced item lands at Lukas's feet: nothing is lost, not even another key.
            if (!s.IsEmpty && Services.TryGet(out Generation.FloorBuilder builder))
                builder.DropItem(s.Kind, s.Count, inventory.transform.position + inventory.transform.forward * 0.5f);
            HbLog.Info("Interact", $"{swapKind} replaced {s.Kind} x{s.Count}");
            Sfx.Play2D(Sfx.Pickup);
            var done = swapDone;
            EndSwap();
            done?.Invoke();
            Close();
        }

        private void EndSwap()
        {
            swapping = false;
            swapDone = null;
            swapAlert.gameObject.SetActive(false);
            foreach (var b in tabButtons) b.interactable = true;
        }

        // ---------- Refresh ----------

        private void OnRegistered(string id, int kind, bool isNew)
        {
            if (open) Refresh();
            if (!isNew) return;
            var registry = Services.Get<RegistryModel>();
            string count = (RecordKind)kind == RecordKind.EmiliaDocument ? "\n" + Loc.Get("UI.TERM.EMILIA_COUNT", registry.EmiliaCount)
                : (RecordKind)kind == RecordKind.Confiscated ? "\n" + Loc.Get("UI.TERM.CONFISCATED_COUNT", CountOf(registry, RecordKind.Confiscated)) : "";
            toastText.text = Loc.Get("UI.TERM.REGISTERED") + count;
            toastPending = true;   // shown when the reading it opened is closed
        }

        private static int CountOf(RegistryModel registry, RecordKind kind) => registry.Order.Count(id => registry.KindOf(id) == kind);

        private void Refresh()
        {
            if (!open) return;
            string subject = Services.TryGet(out Cycle.CycleController cycle) && cycle.Data != null ? cycle.Data.SubjectLabel : "R-H-??";
            header.text = Loc.Get("UI.TERM.SUBJECT", subject);

            if (inventory != null)
                for (int i = 0; i < InventoryModel.SlotCount; i++)
                {
                    var s = inventory.Model[i];
                    slotNames[i].text = s.IsEmpty ? "" : Loc.Get("UI.TERM.SLOT_" + s.Kind);
                    slotCounts[i].text = !s.IsEmpty && s.Count > 1 ? "x" + s.Count : "";
                    slotEmpty[i].text = s.IsEmpty ? Loc.Get("UI.TERM.EMPTY_SLOT") : "";
                    slotIcons[i].enabled = !s.IsEmpty;
                    if (!s.IsEmpty) slotIcons[i].sprite = UiIcons.For(s.Kind);
                    slotButtons[i].GetComponent<UiFocusStyle>().SetColor(s.IsEmpty ? Dim : Teal);
                }
            RefreshDetail();
            RefreshFooter();
            RefreshLogs();
        }

        private void RefreshDetail()
        {
            if (inventory == null || detailName == null) return;
            if (focusedSlot < 0 || inventory.Model[focusedSlot].IsEmpty) { detailName.text = ""; detailInfo.text = ""; return; }
            var s = inventory.Model[focusedSlot];
            detailName.text = Loc.Get("UI.ITEM." + s.Kind);
            detailInfo.text = s.Kind != ItemKind.Ammo ? ""
                : Loc.Get("UI.TERM.RESERVE", inventory.Model.Count(ItemKind.Ammo))
                  + (revolver != null && revolver.Owned ? "\n" + Loc.Get("UI.TERM.DRUM_SHORT", revolver.Model.Loaded) : "");
        }

        private void RefreshFooter()
        {
            foreach (Transform c in footer) Destroy(c.gameObject);
            var a = router.Actions.Inventory;   // keys follow the last device (D-132)
            if (swapping)
            {
                UiKit.KeyHint(footer, router.KeyFor(a.Use), Loc.Get("UI.TERM.REPLACE"), Amber);
                UiKit.KeyHint(footer, router.KeyFor(a.Close), Loc.Get("UI.TERM.LEAVE_IT"), Amber);
                return;
            }
            if (tab == 0)
            {
                UiKit.KeyHint(footer, router.KeyFor(a.Use), Loc.Get("UI.TERM.USE"), Teal);
                UiKit.KeyHint(footer, router.KeyFor(a.Discard), Loc.Get("UI.TERM.DISCARD"), Teal);
            }
            else if (tab == 2) UiKit.KeyHint(footer, router.KeyFor(router.Actions.UI.Submit), Loc.Get("UI.TERM.OPEN"), Teal);
            UiKit.KeyHint(footer, router.KeyFor(a.Close), Loc.Get("UI.TERM.CLOSE"), Teal);
        }

        private void RefreshLogs()
        {
            var registry = Services.Get<RegistryModel>();
            emiliaCount.text = Loc.Get("UI.TERM.EMILIA_COUNT", registry.EmiliaCount);
            confiscatedCount.text = Loc.Get("UI.TERM.CONFISCATED_COUNT", CountOf(registry, RecordKind.Confiscated));

            if (CountOf(registry, category) == 0)
                foreach (var k in categoryButtons.Keys)
                    if (CountOf(registry, k) > 0) { category = k; break; }

            foreach (var pair in categoryButtons)
            {
                int n = CountOf(registry, pair.Key);
                pair.Value.GetComponentInChildren<TMP_Text>().text = $"{CategoryName(pair.Key)}  {n}";
                pair.Value.interactable = n > 0;
                pair.Value.GetComponent<UiFocusStyle>().SetPinned(pair.Key == category && n > 0);
            }
            noLogs.gameObject.SetActive(registry.Count == 0);
            if (logsShownCount == registry.Count && logsShownCategory == category) return;
            logsShownCount = registry.Count;
            logsShownCategory = category;
            foreach (Transform c in logButtons) Destroy(c.gameObject);
            firstEntry = null;
            foreach (var id in registry.Order)
            {
                if (registry.KindOf(id) != category) continue;
                string captured = id;
                var b = UiKit.Button(logButtons, TitleOf(id), 24, Teal, () => Read(captured), 36, true);
                b.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
                if (firstEntry == null) firstEntry = b;
            }
        }

        private void SelectCategory(RecordKind k)
        {
            category = k;
            RefreshLogs();
        }

        private static string CategoryName(RecordKind k) => Loc.Get(k switch
        {
            RecordKind.EmiliaDocument => "UI.TERM.CAT_EMILIA",
            RecordKind.ContextDocument => "UI.TERM.CAT_DOCS",
            RecordKind.Confiscated => "UI.TERM.CAT_CONFISCATED",
            RecordKind.Bestiary => "UI.TERM.CAT_BESTIARY",
            RecordKind.Rule => "UI.TERM.CAT_RULES",
            RecordKind.Fault => "UI.TERM.CAT_FAULTS",
            RecordKind.Poster => "UI.TERM.CAT_POSTERS",
            RecordKind.VoiceNote => "UI.TERM.CAT_VOICE_NOTE",
            _ => "UI.TERM.CAT_RING",
        });

        private string TitleOf(string id) =>
            docsById.TryGetValue(id, out var d) ? ReaderView.TitleOf(d)
            : Services.Get<RegistryModel>().KindOf(id) == RecordKind.Poster ? TutorialPoster.TitleOf(id, router) : id;

        private void Read(string id)
        {
            if (Services.Get<RegistryModel>().KindOf(id) == RecordKind.Poster) { reader.OpenPoster(TutorialPoster.Render(id, router), null); return; }
            if (!docsById.TryGetValue(id, out var d)) { HbLog.Warn("Terminal", $"No document asset for {id}"); return; }
            reader.OpenDocument(d, () =>
            {
                if (id == Cycle.CycleController.RingId) Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.RING_EXAMINE");
                else if (id == Cycle.CycleController.VoiceNoteId) Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.VOICE_NOTE");
            });
        }

        private void Update()
        {
            UpdateToast();
            if (!open) return;
            float k = reduceMotion ? 1f : Mathf.Clamp01((Time.unscaledTime - openTime) / 0.2f);   // CRT power-on
            panel.localScale = new Vector3(scale, scale * Mathf.Max(0.02f, k * k), 1f);
            UpdateStatus();
            actionHint.text = inventory != null && inventory.IsUsingItem ? new string('|', 1 + (int)(inventory.UseProgress * 20)) : "";
        }

        private void UpdateToast()
        {
            if (toastPending && !reader.IsOpen)
            {
                toastPending = false;
                toast.gameObject.SetActive(true);
                toastUntil = Time.unscaledTime + ToastSeconds;
                HbLog.Info("Terminal", toastText.text.Replace('\n', ' '));
            }
            if (toast.gameObject.activeSelf && Time.unscaledTime >= toastUntil) toast.gameObject.SetActive(false);
        }

        private void UpdateStatus()
        {
            var state = health != null ? health.State : HealthState.Fine;
            Color c = state == HealthState.Critical ? Red : state == HealthState.Injured ? Amber : Teal;
            stripEcg.State = bigEcg.State = state;
            stripEcg.Color = bigEcg.Color = c;
            if (state != lastState)
            {
                lastState = state;
                string label = Loc.Get(state == HealthState.Critical ? "UI.TERM.HEALTH_CRITICAL"
                    : state == HealthState.Injured ? "UI.TERM.HEALTH_INJURED" : "UI.TERM.HEALTH_FINE");
                stripHealth.text = healthLabel.text = label;
                stripHealth.color = healthLabel.color = c;
            }

            int drum = revolver != null && revolver.Owned ? revolver.Model.Loaded : -1;
            if (drum != lastDrum)
            {
                lastDrum = drum;
                stripDrum.text = drum >= 0 ? Loc.Get("UI.TERM.DRUM_SHORT", drum) : Loc.Get("UI.TERM.DRUM_NONE");
            }

            if (flashlight == null) return;
            var m = flashlight.Model;
            flashFill.fillAmount = m.Charge;
            Color fc = m.IsLocked ? Red : m.Charge < 0.2f ? Amber : Teal;
            flashFill.color = stripLight.color = fc;
            int light = m.IsLocked ? -2 : Mathf.RoundToInt(m.Charge * 100f);
            if (light == lastLight) return;
            lastLight = light;
            stripLight.text = Loc.Get("UI.TERM.LIGHT", Mathf.Max(0, light));
            flashLabel.text = m.IsLocked ? Loc.Get("UI.TERM.FLASHLIGHT_LOCKED") : Loc.Get("UI.TERM.FLASHLIGHT", light);
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
            for (int p = 0; p < pages.Length; p++)
            {
                pages[p].SetActive(p == i);
                tabButtons[p].GetComponent<UiFocusStyle>().SetPinned(p == i);
            }
            Refresh();
            if (i == 0) UiKit.Select(slotButtons[Mathf.Max(0, focusedSlot)]);
            else UiKit.Select(i == 2 && firstEntry != null ? firstEntry : tabButtons[i]);
        }

        /// <summary>Terminal scale (Accesibilidad, independent from the subtitle size): the panel grows from its left edge.</summary>
        private void ApplyScale()
        {
            if (panel == null) return;
            if (Services.TryGet(out Save.SaveService saves))
            {
                var g = saves.LoadGlobal();
                scale = Mathf.Max(1f, g.terminalScale);
                reduceMotion = g.reduceMotion;
            }
            panel.localScale = new Vector3(scale, scale, 1f);
        }

        // ---------- Construction ----------

        private void Build()
        {
            panel = UiKit.Rect("Screen", transform);
            panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0f, 0.5f);
            panel.anchoredPosition = new Vector2(64f, 0f);
            panel.sizeDelta = new Vector2(880f, 760f);
            UiKit.Crt(panel, UiKit.BgTerminal, Dim);
            var v = UiKit.Column(panel, 12, 24);
            v.childForceExpandWidth = true;

            var head = UiKit.Rect("Header", panel);
            UiKit.Row(head, 12);
            header = UiKit.Text(head, "", 30, Teal);
            header.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            UiKit.Text(head, Loc.Get("UI.TERM.BRAND"), 24, UiKit.Cyan, true);

            var tabs = UiKit.Rect("Tabs", panel);
            UiKit.Row(tabs, 8).childForceExpandWidth = true;
            string[] keys = { "UI.TERM.TAB_INVENTORY", "UI.TERM.TAB_STATUS", "UI.TERM.TAB_LOGS" };
            for (int i = 0; i < 3; i++)
            {
                int ix = i;
                tabButtons[i] = UiKit.Button(tabs, Loc.Get(keys[i]), 28, Teal, () => ShowTab(ix), 48, true);
            }

            var strip = UiKit.Rect("Strip", panel);
            UiKit.Row(strip, 24);
            strip.gameObject.AddComponent<LayoutElement>().minHeight = 40;
            stripEcg = UiEcg.Build(strip, 28, 36);
            var ecgSize = stripEcg.GetComponent<LayoutElement>();
            ecgSize.preferredWidth = 150;
            ecgSize.flexibleWidth = 0;
            stripHealth = UiKit.Text(strip, "", 24, Teal);
            stripHealth.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            stripLight = UiKit.Text(strip, "", 24, Teal);
            stripDrum = UiKit.Text(strip, "", 24, Teal);

            BuildInventory();
            BuildStatus();
            BuildLogs();

            footer = UiKit.Rect("Keys", panel);
            UiKit.Row(footer, 24);
            footer.gameObject.AddComponent<LayoutElement>().minHeight = 40;
            UiKit.Scanlines(panel);

            toast = UiKit.Rect("Registered", transform);
            toast.anchorMin = toast.anchorMax = toast.pivot = new Vector2(0f, 1f);
            toast.anchoredPosition = new Vector2(64f, -36f);   // above the terminal's panel, which may be open
            toast.sizeDelta = new Vector2(420f, 100f);
            UiKit.Panel(toast, UiKit.BgTerminal, Dim);
            toastText = UiKit.Text(toast, "", 28, Teal);
            UiKit.Stretch(toastText.rectTransform, Vector2.zero, Vector2.one);
            toastText.margin = new Vector4(20, 10, 20, 10);
            toastText.alignment = TextAlignmentOptions.MidlineLeft;
            toast.gameObject.SetActive(false);
        }

        private void BuildInventory()
        {
            var page = NewPage("Inventory");
            pages[0] = page.gameObject;
            swapAlert = UiKit.Text(page, "", 28, Amber, true);
            swapAlert.textWrappingMode = TextWrappingModes.Normal;
            swapAlert.gameObject.SetActive(false);

            var row = UiKit.Rect("Row", page);
            UiKit.Row(row, 20, TextAnchor.UpperLeft);
            row.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

            var grid = UiKit.Rect("Slots", row);
            var g = grid.gameObject.AddComponent<GridLayoutGroup>();
            g.cellSize = new Vector2(SlotSize, SlotSize);
            g.spacing = new Vector2(8, 8);
            g.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            g.constraintCount = 3;
            var gridSize = grid.gameObject.AddComponent<LayoutElement>();
            gridSize.preferredWidth = 3 * SlotSize + 16;
            gridSize.preferredHeight = 2 * SlotSize + 8;

            for (int i = 0; i < InventoryModel.SlotCount; i++)
            {
                int ix = i;
                var b = UiKit.Button(grid, "", 22, Teal, null, SlotSize, true);
                slotButtons[i] = b;
                slotEmpty[i] = b.GetComponentInChildren<TMP_Text>();

                var icon = UiKit.Rect("Icon", b.transform);
                icon.anchorMin = icon.anchorMax = icon.pivot = new Vector2(0.5f, 0.62f);
                icon.sizeDelta = new Vector2(72f, 72f);
                slotIcons[i] = icon.gameObject.AddComponent<Image>();
                slotIcons[i].raycastTarget = false;
                slotIcons[i].enabled = false;

                slotNames[i] = UiKit.Text(b.transform, "", 24, Teal, true);
                UiKit.Stretch(slotNames[i].rectTransform, Vector2.zero, new Vector2(1f, 0.28f));
                slotNames[i].alignment = TextAlignmentOptions.Center;
                slotCounts[i] = UiKit.Text(b.transform, "", 24, Teal, true);
                UiKit.Stretch(slotCounts[i].rectTransform, new Vector2(0f, 0.8f), new Vector2(0.94f, 1f));
                slotCounts[i].alignment = TextAlignmentOptions.MidlineRight;

                var trigger = b.gameObject.AddComponent<EventTrigger>();
                var e = new EventTrigger.Entry { eventID = EventTriggerType.Select };
                e.callback.AddListener(_ => Focus(ix));
                trigger.triggers.Add(e);
            }

            var detail = UiKit.Rect("Detail", row);
            UiKit.Column(detail, 10);
            detail.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            detailName = Line(detail, 28);
            detailName.textWrappingMode = TextWrappingModes.Normal;
            detailInfo = UiKit.Text(detail, "", 24, Teal);
            actionHint = Line(detail, 24);
        }

        private void BuildStatus()
        {
            var page = NewPage("Status");
            pages[1] = page.gameObject;
            healthLabel = UiKit.Text(page, "", 48, Teal);
            // VT323 glyphs are taller than the size TMP lays them out with: reserve the line so the ECG never sits on it.
            healthLabel.gameObject.AddComponent<LayoutElement>().minHeight = 72;
            bigEcg = UiEcg.Build(page, EcgBars, 150);
            flashLabel = Line(page, 28);
            var barBg = UiKit.Rect("FlashBar", page);
            barBg.gameObject.AddComponent<LayoutElement>().preferredHeight = 18;
            barBg.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.6f);
            var fill = UiKit.Stretch(UiKit.Rect("Fill", barBg), Vector2.zero, Vector2.one);
            flashFill = fill.gameObject.AddComponent<Image>();
            flashFill.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero);
            flashFill.type = Image.Type.Filled;
            flashFill.fillMethod = Image.FillMethod.Horizontal;
            emiliaCount = Line(page, 28);
            confiscatedCount = Line(page, 28);
        }

        private void BuildLogs()
        {
            var page = NewPage("Logs");
            pages[2] = page.gameObject;
            noLogs = UiKit.Text(page, Loc.Get("UI.TERM.NO_LOGS"), 26, Dim, true);
            var row = UiKit.Rect("Row", page);
            UiKit.Row(row, 16, TextAnchor.UpperLeft);
            row.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

            var cats = UiKit.Rect("Categories", row);
            UiKit.Column(cats, 4);
            var catSize = cats.gameObject.AddComponent<LayoutElement>();
            catSize.preferredWidth = 290;
            catSize.flexibleWidth = 0;
            foreach (RecordKind k in Enum.GetValues(typeof(RecordKind)))
            {
                var kind = k;
                var b = UiKit.Button(cats, CategoryName(k), 22, Teal, () => SelectCategory(kind), 38, true);
                b.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
                categoryButtons[k] = b;
            }

            var entries = UiKit.Rect("Entries", row);
            UiKit.Column(entries, 2);
            entries.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            logButtons = entries;
        }

        /// <summary>A text line that keeps its height: VT323 lays out shorter than it draws, so plain lines overlap.</summary>
        private static TMP_Text Line(Transform parent, float size)
        {
            var t = UiKit.Text(parent, "", size, Teal);
            t.gameObject.AddComponent<LayoutElement>().minHeight = size * 1.3f;
            return t;
        }

        private RectTransform NewPage(string name)
        {
            var p = UiKit.Rect(name, panel);
            UiKit.Column(p, 8);
            p.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;
            return p;
        }
    }
}
