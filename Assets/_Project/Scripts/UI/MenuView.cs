using System.Linq;
using Heartbeat.Audio;
using Heartbeat.Core;
using Heartbeat.Cycle;
using Heartbeat.Narrative;
using Heartbeat.Save;
using TMPro;
using UnityEngine;
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

        // Calibration symbols: linear grey levels on black. The left one must be barely visible, the right one not.
        [SerializeField] private float visibleLevel = 0.03f;
        [SerializeField] private float hiddenLevel = 0.008f;
        [Tooltip("Capsule-row room seen behind the menu terminal (D-90); hidden on the black title and calibration.")]
        [SerializeField] private GameObject backdrop;

        private Transform canvas;
        private RectTransform screen;
        private GameStateMachine states;
        private SaveService saves;
        private AudioService audioService;
        private AudioSource loop;
        private Image[] symbols;
        private float brightness = 1f;

        private void Start()
        {
            states = Services.Get<GameStateMachine>();
            saves = Services.Get<SaveService>();
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

        private void OnDestroy() => StopLoop();

        // ---------- Screens ----------

        private RectTransform NewScreen(float width = 900f, bool inRoom = true)
        {
            if (screen != null) Destroy(screen.gameObject);
            if (backdrop != null) backdrop.SetActive(inRoom);
            screen = UiKit.Rect("Screen", canvas);
            bool terminal = inRoom && backdrop != null;
            screen.anchorMin = screen.anchorMax = screen.pivot = terminal ? new Vector2(0.7f, 0.5f) : new Vector2(0.5f, 0.5f);
            screen.sizeDelta = new Vector2(terminal ? Mathf.Min(width, 760f) : width, 0);
            if (terminal) UiKit.Panel(screen, new Color(0.01f, 0.04f, 0.02f, 0.88f), UiKit.Green);   // the facility terminal
            UiKit.Column(screen, 14, 28);
            screen.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return screen;
        }

        private TMP_Text Line(string text, float size = Size, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var t = UiKit.Text(screen, text, size, Green);
            t.alignment = align;
            t.textWrappingMode = TextWrappingModes.Normal;
            return t;
        }

        private Button Btn(string text, System.Action onClick) => UiKit.Button(screen, text, Size, Green, onClick, 56);

        private void ShowCalibration()
        {
            NewScreen(1200, inRoom: false);
            Line(Loc.Get("UI.BRIGHT.TITLE"), 30);
            var row = UiKit.Rect("Symbols", screen);
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 160;
            h.childAlignment = TextAnchor.MiddleCenter;
            h.childControlWidth = h.childControlHeight = false;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 220;
            symbols = new Image[2];
            for (int i = 0; i < 2; i++)
            {
                var s = UiKit.Rect("Symbol", row);
                s.sizeDelta = new Vector2(180, 180);
                symbols[i] = s.gameObject.AddComponent<Image>();
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
            float a = Render.Brightness.Perceived(visibleLevel, brightness);
            float b = Render.Brightness.Perceived(hiddenLevel, brightness);
            symbols[0].color = new Color(a, a, a);
            symbols[1].color = new Color(b, b, b);
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
            Line("IDENTITY DISSOLUTION", 72).fontStyle = FontStyles.Bold;   // logo placeholder (PLACEHOLDERS.md)
            Line(Loc.Get("UI.TITLE.PRESS"), 28).color = UiKit.TealDim;
            StartLoop("CUE_Heartbeat_Lab");                                 // the title only has the heartbeat (D-90)
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
            NewScreen(1300);
            Line(Loc.Get("UI.SLOT.TITLE"), 36);
            Button first = null;
            for (int i = 0; i < SaveService.SlotCount; i++)
            {
                int slot = i;
                var status = saves.Status(slot);
                string label = $"{slot + 1}.  " + (status == SlotStatus.Empty ? Loc.Get("UI.SLOT.EMPTY")
                    : status == SlotStatus.Damaged ? Loc.Get("UI.SLOT.DAMAGED") : Summary(saves.Load(slot)));
                var b = Btn(label, () => PickSlot(slot, status, newGame));
                b.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;
                b.GetComponentInChildren<TMP_Text>().fontSize = 26;
                b.interactable = newGame || status == SlotStatus.Ok;       // empty slots cannot be loaded
                if (first == null && b.interactable) first = b;
                if (status != SlotStatus.Empty)
                {
                    var del = UiKit.Button(screen, Loc.Get("UI.SLOT.DELETE") + $" {slot + 1}", 22, UiKit.Amber, () =>
                        ConfirmDialog.Show(canvas, Loc.Get("UI.CONFIRM.DELETE"), () => { saves.Delete(slot); HbLog.Info("Menu", $"Slot {slot} deleted"); ShowSlots(newGame); }, null, Green), 36);
                    del.GetComponent<LayoutElement>().preferredHeight = 36;
                }
            }
            var back = Btn(Loc.Get("UI.MENU.BACK"), ShowMain);
            UiKit.Select(first != null ? first : back);
        }

        private static string Summary(SlotData d)
        {
            int emilia = d.terminal.Count(e => e.kind == RecordKind.EmiliaDocument);
            string mode = Loc.Get("UI.MODE_LABEL." + d.mode);
            int floor = d.hasAttemptSave ? d.attemptSave.floor : 1;
            string date = string.IsNullOrEmpty(d.updatedAt) ? "" : d.updatedAt.Substring(0, Mathf.Min(16, d.updatedAt.Length)).Replace('T', ' ');
            return Loc.Get("UI.SLOT.SUMMARY", d.SubjectLabel, mode, floor, emilia, date);
        }

        private void PickSlot(int slot, SlotStatus status, bool newGame)
        {
            if (!newGame) { StartGame(slot, saves.Load(slot).mode, false); return; }
            if (status == SlotStatus.Empty) ShowMode(slot);
            else ConfirmDialog.Show(canvas, Loc.Get("UI.CONFIRM.OVERWRITE"), () => ShowMode(slot), null, Green);
        }

        private void ShowMode(int slot)
        {
            NewScreen(1100);
            Line(Loc.Get("UI.MODE.TITLE"), 36);
            var survival = Btn(Loc.Get("UI.MODE.SURVIVAL"), () => StartGame(slot, GameMode.Survival, true));
            Line(Loc.Get("UI.MODE.SURVIVAL_DESC"), 24).color = UiKit.TealDim;
            Btn(Loc.Get("UI.MODE.STORY"), () => StartGame(slot, GameMode.Story, true));
            Line(Loc.Get("UI.MODE.STORY_DESC"), 24).color = UiKit.TealDim;
            Btn(Loc.Get("UI.MENU.BACK"), () => ShowSlots(true));
            UiKit.Select(survival);
        }

        private void ShowCredits()
        {
            NewScreen(1100);
            Line(Loc.Get("UI.MENU.CREDITS"), 40);
            Line(Loc.Get("END.CREDITS.TITLE"), 30);
            Line(Loc.Get("END.CREDITS.ROLL"), 26);   // the 5 members (Roles y Responsabilidades), same roll as the ending credits
            UiKit.Select(Btn(Loc.Get("UI.MENU.BACK"), ShowMain));
        }

        private void ShowOptions()
        {
            NewScreen(1300);
            OptionsView.Build(screen, canvas, Green, ShowMain);
            HbLog.Info("Menu", "Options");
        }

        private void StartGame(int slot, GameMode mode, bool newGame)
        {
            StopLoop();
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

        private void StartLoop(string cue)
        {
            StopLoop();
            if (audioService != null && audioService.TryGetCue(cue, out var c)) loop = audioService.StartLoop(c);
        }

        private void StopLoop()
        {
            if (audioService != null) audioService.Stop(loop);
            loop = null;
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
            handle.sizeDelta = new Vector2(20, 0);
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
            return s;
        }
    }
}
