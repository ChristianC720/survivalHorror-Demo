using System.Collections.Generic;
using System.Linq;
using Heartbeat.Audio;
using Heartbeat.Core;
using Heartbeat.Cycle;
using Heartbeat.InputRouting;
using Heartbeat.Narrative;
using Heartbeat.Save;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Front end in the MainMenu scene (UI y Menús): brightness calibration (first launch, D-106) → title (logo +
    /// lab heartbeat, any key, D-90) → main menu terminal (Continue / New game / Load / Options / Credits / Quit,
    /// D-39, D-105) → slot selection (3 slots, D-105) → mode selection (Survival / Story, D-103).
    /// Facility screens are green phosphor (D-82). Built in code on this object's canvas.
    /// The title and the calibration are black (D-90); from the main menu on, the screens are a green facility terminal in
    /// the capsule-row room next to Lukas's capsule (<see cref="backdrop"/>, a hand-placed room instance in the scene).
    /// </summary>
    public sealed class MenuView : MonoBehaviour
    {
        private const float Size = 32f;
        private static Color Green => UiKit.Green;
        private static readonly Color TitlePulse = new Color(0.286f, 0.129f, 0.325f);   // #492153, the game's purple

        // Calibration symbols: linear grey levels on black. Left visible, middle barely, right not.
        [SerializeField] private float visibleLevel = 0.03f;
        [SerializeField] private float hiddenLevel = 0.008f;
        [Tooltip("Capsule-row room seen behind the menu terminal (D-90); hidden on the black title and calibration.")]
        [SerializeField] private GameObject backdrop;

        private Transform canvas;
        private RectTransform screen;
        private GameStateMachine states;
        private SaveService saves;
        private InputRouter router;
        private AudioService audioService;
        private AudioSource loop;
        private Image[] symbols;
        private float brightness = 1f;
        private System.Action onEscape;
        private OptionsView options;
        private readonly Dictionary<GameObject, int> slotRows = new Dictionary<GameObject, int>();
        private bool slotsNewGame;

        private void Start()
        {
            states = Services.Get<GameStateMachine>();
            saves = Services.Get<SaveService>();
            router = Services.Get<InputRouter>();
            router.Actions.UI.Cancel.performed += OnCancel;
            router.Actions.UI.Delete.performed += OnDelete;
            Services.TryGet(out audioService);
            canvas = UiKit.Canvas(gameObject, 20).transform;
            if (backdrop != null)
            {
                // A still room: no tutorial posters (they need a player's bindings) and no revolver on the tray.
                foreach (var p in backdrop.GetComponentsInChildren<TutorialPoster>(true)) if (p.TutorialOnly) p.gameObject.SetActive(false);
                foreach (var r in backdrop.GetComponentsInChildren<Interaction.RevolverPickup>(true)) r.gameObject.SetActive(false);
            }
            brightness = saves.LoadGlobal().brightness;
            switch (states.Current)
            {
                case GameStateId.BrightnessCalibration: ShowCalibration(); break;
                case GameStateId.Title: ShowTitle(); break;
                default: ShowMain(); break;
            }
        }

        private void OnDestroy()
        {
            StopLoop();
            if (router == null) return;
            router.Actions.UI.Cancel.performed -= OnCancel;
            router.Actions.UI.Delete.performed -= OnDelete;
        }

        private void OnCancel(InputAction.CallbackContext _)
        {
            if (ConfirmDialog.AnyOpen || (options != null && options.IsRebinding)) return;
            onEscape?.Invoke();
        }

        private void OnDelete(InputAction.CallbackContext _)
        {
            if (ConfirmDialog.AnyOpen || EventSystem.current == null) return;
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null || !slotRows.TryGetValue(selected, out int slot) || saves.Status(slot) == SlotStatus.Empty) return;
            ConfirmDialog.Show(canvas, Loc.Get("UI.CONFIRM.DELETE"), () =>
            {
                saves.Delete(slot);
                HbLog.Info("Menu", $"Slot {slot} deleted");
                ShowSlots(slotsNewGame);
            }, null, Green);
        }

        // ---------- Screens ----------

        private RectTransform NewScreen(float width = 760f, bool inRoom = true)
        {
            if (screen != null) Destroy(screen.gameObject);
            if (backdrop != null) backdrop.SetActive(inRoom);
            onEscape = null;
            options = null;
            slotRows.Clear();
            screen = UiKit.Rect("Screen", canvas);
            bool terminal = inRoom && backdrop != null;
            screen.anchorMin = screen.anchorMax = screen.pivot = terminal ? new Vector2(0.65f, 0.5f) : new Vector2(0.5f, 0.5f);
            screen.sizeDelta = new Vector2(width, 0);
            if (terminal) UiKit.Crt(screen, UiKit.BgFacility, Green);   // the facility terminal
            UiKit.Column(screen, 14, 28).childForceExpandWidth = true;
            screen.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            if (terminal)
            {
                var head = UiKit.Text(screen, StationLine(), 26, UiKit.Cyan, true);
                head.alignment = TextAlignmentOptions.MidlineLeft;
                UiKit.Scanlines(screen);
            }
            return screen;
        }

        private string StationLine()
        {
            int last = saves.LoadGlobal().lastUsedSlot;
            return saves.Status(last) == SlotStatus.Ok ? Loc.Get("UI.MENU.STATION_FLOOR", FloorOf(saves.Load(last))) : Loc.Get("UI.MENU.STATION");
        }

        private static int FloorOf(SlotData d) => d.hasAttemptSave ? d.attemptSave.floor : d.hasFloorStart ? d.floorStart.floor : 1;   // D-147: Continue starts on the saved floor

        private TMP_Text Line(string text, float size = Size, TextAlignmentOptions align = TextAlignmentOptions.Center, bool data = false)
        {
            var t = UiKit.Text(screen, text, size, Green, data);
            t.alignment = align;
            t.textWrappingMode = TextWrappingModes.Normal;
            return t;
        }

        private Button Btn(string text, System.Action onClick) => UiKit.Button(screen, text, Size, Green, onClick, 56);

        private RectTransform Keys()
        {
            var r = UiKit.Rect("Keys", screen);
            UiKit.Row(r, 24);
            r.gameObject.AddComponent<LayoutElement>().minHeight = 40;
            return r;
        }

        private string KeyName(InputAction action, string keyboard) => router.UsingGamepad ? router.KeyFor(action) : keyboard;

        private void ShowCalibration()
        {
            NewScreen(1200, inRoom: false);
            Line(Loc.Get("UI.BRIGHT.TITLE"), 32);
            var row = UiKit.Rect("Symbols", screen);
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 120;
            h.childAlignment = TextAnchor.MiddleCenter;
            h.childControlWidth = h.childControlHeight = false;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 220;
            symbols = new Image[3];
            for (int i = 0; i < symbols.Length; i++)
            {
                var s = UiKit.Rect("Symbol", row);
                s.sizeDelta = new Vector2(180, 180);
                symbols[i] = s.gameObject.AddComponent<Image>();
                symbols[i].sprite = UiIcons.Get("Pulse");
            }
            var slider = MakeSlider(screen, Render.Brightness.Min, Render.Brightness.Max, brightness, v => { brightness = v; ApplyBrightnessPreview(); });
            ApplyBrightnessPreview();
            Btn(Loc.Get("UI.BRIGHT.ACCEPT"), AcceptCalibration);
            UiKit.Select(slider);
            HbLog.Info("Menu", "Brightness calibration");
        }

        private void ApplyBrightnessPreview()
        {
            Render.Brightness.Apply(brightness);
            float[] levels = { visibleLevel, Mathf.Sqrt(visibleLevel * hiddenLevel), hiddenLevel };
            for (int i = 0; i < symbols.Length; i++)
            {
                float v = Render.Brightness.Perceived(levels[i], brightness);
                symbols[i].color = new Color(v, v, v);
            }
        }

        private void AcceptCalibration()
        {
            var g = saves.LoadGlobal();
            g.brightness = brightness;
            g.brightnessCalibrated = true;
            saves.SaveGlobal(g);
            HbLog.Info("Menu", $"Brightness calibrated {brightness:0.00}");
            states.TryTransition(GameStateId.Title);
            ShowTitle();
        }

        private void ShowTitle()
        {
            NewScreen(1200, inRoom: false);
            var logo = UiKit.Rect("Logo", screen).gameObject.AddComponent<Image>();
            logo.sprite = Resources.Load<Sprite>("HB_Logo");
            logo.preserveAspect = true;
            logo.raycastTarget = false;
            var logoSize = logo.gameObject.AddComponent<LayoutElement>();
            logoSize.preferredWidth = 1000;
            logoSize.preferredHeight = 252;
            var pulse = UiEcg.Build(screen, 96, 110);
            pulse.Color = TitlePulse;
            Line(Loc.Get("UI.TITLE.PRESS"), 28).color = UiKit.Text1;
            StartLoop("CUE_Music_Title");                                   // the title only has the heartbeat (D-90)
            InputSystem.onAnyButtonPress.CallOnce(_ =>
            {
                if (this == null || states.Current != GameStateId.Title) return;
                states.TryTransition(GameStateId.MainMenu);
                ShowMain();
            });
            HbLog.Info("Menu", "Title");
        }

        private void ShowMain()
        {
            StartLoop("CUE_Music_Menu");                                    // Emilia's theme (D-87)
            NewScreen();
            var global = saves.LoadGlobal();
            int last = global.lastUsedSlot;
            bool canContinue = saves.Status(last) == SlotStatus.Ok;
            var cont = Btn(Loc.Get("UI.MENU.CONTINUE"), () => StartGame(last, saves.Load(last).mode, false));
            cont.interactable = canContinue;                                // grey without a game (D-96)
            var newBtn = Btn(Loc.Get("UI.MENU.NEW"), () => ShowSlots(newGame: true));
            Btn(Loc.Get("UI.MENU.LOAD"), () => ShowSlots(newGame: false));
            Btn(Loc.Get("UI.MENU.OPTIONS"), ShowOptions);
            Btn(Loc.Get("UI.MENU.CREDITS"), ShowCredits);
            Btn(Loc.Get("UI.MENU.QUIT"), () => ConfirmDialog.Show(canvas, Loc.Get("UI.CONFIRM.QUIT_GAME"), QuitGame, null, Green));
            UiKit.Select(canContinue ? cont : newBtn);
            HbLog.Info("Menu", $"Main menu (continue={(canContinue ? "slot " + last : "disabled")})");
        }

        private void ShowSlots(bool newGame)
        {
            slotsNewGame = newGame;
            NewScreen(1180);
            onEscape = ShowMain;
            Line(Loc.Get("UI.SLOT.TITLE"), 36);
            Button first = null;
            for (int i = 0; i < SaveService.SlotCount; i++)
            {
                int slot = i;
                var status = saves.Status(slot);
                var b = SlotRow(slot, status, () => PickSlot(slot, status, newGame));
                b.interactable = newGame || status == SlotStatus.Ok;       // empty slots cannot be loaded
                slotRows[b.gameObject] = slot;
                if (first == null && b.interactable) first = b;
            }
            var back = Btn(Loc.Get("UI.MENU.BACK"), ShowMain);
            var keys = Keys();
            UiKit.KeyHint(keys, KeyName(router.Actions.UI.Submit, "ENTER"), Loc.Get("UI.SLOT.PICK"), Green);
            UiKit.KeyHint(keys, KeyName(router.Actions.UI.Delete, "DEL"), Loc.Get("UI.SLOT.DELETE"), Green);
            UiKit.KeyHint(keys, KeyName(router.Actions.UI.Cancel, "ESC"), Loc.Get("UI.MENU.BACK"), Green);
            UiKit.Select(first != null ? first : back);
        }

        /// <summary>One slot as a row of columns, so saves can be compared at a glance.</summary>
        private Button SlotRow(int slot, SlotStatus status, System.Action onClick)
        {
            var b = UiKit.Button(screen, "", 28, status == SlotStatus.Damaged ? UiKit.Amber : Green, onClick, 64, true);
            var cols = UiKit.Stretch(UiKit.Rect("Columns", b.transform), Vector2.zero, Vector2.one);
            cols.offsetMin = new Vector2(44, 0);
            cols.offsetMax = new Vector2(-12, 0);
            UiKit.Row(cols, 16);
            Column(cols, (slot + 1).ToString(), 40);
            if (status == SlotStatus.Ok)
            {
                var d = saves.Load(slot);
                string date = string.IsNullOrEmpty(d.updatedAt) ? "" : d.updatedAt.Substring(0, Mathf.Min(16, d.updatedAt.Length)).Replace('T', ' ');
                Column(cols, d.SubjectLabel, 130);
                Column(cols, Loc.Get("UI.MODE_LABEL." + d.mode), 200);
                Column(cols, Loc.Get("UI.SLOT.FLOOR", FloorOf(d)), 120);
                Column(cols, Loc.Get("UI.SLOT.EMILIA", d.terminal.Count(e => e.kind == RecordKind.EmiliaDocument)), 170);
                Column(cols, date, 280);
            }
            else Column(cols, Loc.Get(status == SlotStatus.Empty ? "UI.SLOT.EMPTY" : "UI.SLOT.DAMAGED"), 500);
            b.GetComponent<UiFocusStyle>().Apply();
            return b;
        }

        private static void Column(Transform parent, string text, float width)
        {
            var t = UiKit.Text(parent, text, 28, Green, true);
            t.textWrappingMode = TextWrappingModes.NoWrap;
            t.alignment = TextAlignmentOptions.MidlineLeft;
            t.gameObject.AddComponent<LayoutElement>().preferredWidth = width;
        }

        private void PickSlot(int slot, SlotStatus status, bool newGame)
        {
            if (!newGame) { StartGame(slot, saves.Load(slot).mode, false); return; }
            if (status == SlotStatus.Empty) ShowMode(slot);
            else ConfirmDialog.Show(canvas, Loc.Get("UI.CONFIRM.OVERWRITE"), () => ShowMode(slot), null, Green);
        }

        private void ShowMode(int slot)
        {
            NewScreen(1000);
            onEscape = () => ShowSlots(true);
            Line(Loc.Get("UI.MODE.TITLE"), 36);
            var survival = Btn(Loc.Get("UI.MODE.SURVIVAL"), () => StartGame(slot, GameMode.Survival, true));
            Line(Loc.Get("UI.MODE.SURVIVAL_DESC"), 26, TextAlignmentOptions.Center, true);
            Btn(Loc.Get("UI.MODE.STORY"), () => StartGame(slot, GameMode.Story, true));
            Line(Loc.Get("UI.MODE.STORY_DESC"), 26, TextAlignmentOptions.Center, true);
            Btn(Loc.Get("UI.MENU.BACK"), () => ShowSlots(true));
            UiKit.Select(survival);
        }

        private void ShowCredits()
        {
            NewScreen(1000);
            onEscape = ShowMain;
            Line(Loc.Get("UI.MENU.CREDITS"), 40);
            Line(Loc.Get("END.CREDITS.TITLE"), 30);
            Line(Loc.Get("END.CREDITS.ROLL"), 26);   // the 5 members (Roles y Responsabilidades), same roll as the ending credits
            UiKit.Select(Btn(Loc.Get("UI.MENU.BACK"), ShowMain));
        }

        private void ShowOptions()
        {
            NewScreen(1300);
            options = OptionsView.Build(screen, canvas, Green, ShowMain);
            onEscape = ShowMain;
            HbLog.Info("Menu", "Options");
        }

        private void StartGame(int slot, GameMode mode, bool newGame)
        {
            StopLoop();
            onEscape = null;
            slotRows.Clear();
            if (screen != null) Destroy(screen.gameObject);
            Services.Get<CycleController>().StartFromMenu(slot, mode, newGame);
        }

        private static void QuitGame()
        {
            HbLog.Info("Menu", "Quit game");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // ---------- Helpers ----------

        private string loopCue;

        private void StartLoop(string cue)
        {
            if (cue == loopCue && loop != null) return;   // coming back from Options or Credits: the theme carries on
            loopCue = cue;
            if (audioService != null && audioService.TryGetCue(cue, out var c)) loop = audioService.StartMusic(c, loop);
        }

        private void StopLoop()
        {
            if (audioService != null) audioService.FadeOutMusic(loop);
            loop = null;
            loopCue = null;
        }

        public static Slider MakeSlider(Transform parent, float min, float max, float value, System.Action<float> onChange)
        {
            var r = UiKit.Rect("Slider", parent);
            r.gameObject.AddComponent<LayoutElement>().preferredHeight = 36;
            var bg = UiKit.Stretch(UiKit.Rect("Background", r), new Vector2(0, 0.35f), new Vector2(1, 0.65f));
            bg.gameObject.AddComponent<Image>().color = new Color(0.1f, 0.25f, 0.1f);
            var area = UiKit.Stretch(UiKit.Rect("Fill Area", r), new Vector2(0, 0.35f), new Vector2(1, 0.65f));
            var fill = UiKit.Stretch(UiKit.Rect("Fill", area), Vector2.zero, Vector2.one);
            fill.gameObject.AddComponent<Image>().color = UiKit.Green * 0.8f;
            var handleArea = UiKit.Stretch(UiKit.Rect("Handle Area", r), Vector2.zero, Vector2.one);
            var handle = UiKit.Rect("Handle", handleArea);
            handle.sizeDelta = new Vector2(16, 0);
            handle.anchorMin = new Vector2(0, 0); handle.anchorMax = new Vector2(0, 1);
            var himg = handle.gameObject.AddComponent<Image>();
            himg.color = UiKit.Green;
            var s = r.gameObject.AddComponent<Slider>();
            s.fillRect = fill;
            s.handleRect = handle;
            s.targetGraphic = himg;
            s.minValue = min;
            s.maxValue = max;
            s.value = value;
            s.onValueChanged.AddListener(v => onChange(v));
            // Focus is shape, not only colour: the handle doubles in width.
            var t = r.gameObject.AddComponent<EventTrigger>();
            AddFocus(t, EventTriggerType.Select, handle, 32f);
            AddFocus(t, EventTriggerType.Deselect, handle, 16f);
            return s;
        }

        private static void AddFocus(EventTrigger t, EventTriggerType type, RectTransform handle, float width)
        {
            var e = new EventTrigger.Entry { eventID = type };
            e.callback.AddListener(_ => handle.sizeDelta = new Vector2(width, handle.sizeDelta.y));
            t.triggers.Add(e);
        }
    }
}
