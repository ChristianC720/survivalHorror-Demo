using System.Collections.Generic;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// The hack sequence on the arm terminal (M-13, Interfaz Visual): the chain of directional inputs, entered ones
    /// lit, the next one blinking, the rest dim. An error flashes red ACCESS DENIED and the chain restarts; success
    /// shows ACCESS GRANTED. Never pauses; added at runtime by <see cref="TerminalView"/> and scales with its text size.
    /// </summary>
    public sealed class HackView : MonoBehaviour
    {
        private const float CellSize = 64f, FlashSeconds = 0.6f, GrantedSeconds = 0.8f;

        private RectTransform panel, row;
        private TMP_Text title, status, hint;
        private readonly List<TMP_Text> cells = new List<TMP_Text>();
        private PlayerHack hack;
        private InputRouter router;
        private float flashUntil, hideAt = -1f;

        public bool IsShown => panel != null && panel.gameObject.activeSelf;

        private void Awake()
        {
            Build();
            GameEvents.FaultUsed += OnFaultUsed;
        }

        private void OnDestroy()
        {
            GameEvents.FaultUsed -= OnFaultUsed;
            Unsubscribe();
        }

        private void OnFaultUsed(string id)
        {
            status.text = Loc.Get("UI.HACK.FAULT_USED");
            status.color = UiKit.Teal;
            flashUntil = Time.unscaledTime + GrantedSeconds * 2f;
        }

        private void Update()
        {
            if (hack == null && Services.TryGet(out hack))
            {
                hack.Started += OnStarted;
                hack.Stepped += OnStepped;
                hack.Ended += OnEnded;
            }
            if (!IsShown) return;

            if (hack != null && hack.IsHacking)
            {
                int done = hack.Current.Progress;
                bool blink = Mathf.Repeat(Time.unscaledTime, 0.5f) < 0.25f;
                for (int i = 0; i < cells.Count; i++)
                    cells[i].color = i < done ? UiKit.Teal : i == done ? (blink ? Color.white : UiKit.Teal) : UiKit.TealDim;
                if (Time.unscaledTime >= flashUntil) status.text = "";
            }
            else if (hideAt > 0f && Time.unscaledTime >= hideAt) Hide();
        }

        private void OnStarted(HackSequence seq)
        {
            foreach (var c in cells) Destroy(c.gameObject);
            cells.Clear();
            foreach (var d in seq.Steps)
            {
                var t = UiKit.Text(row, Glyph(d), CellSize, UiKit.TealDim);
                t.alignment = TextAlignmentOptions.Center;
                t.gameObject.AddComponent<LayoutElement>().preferredWidth = CellSize;
                cells.Add(t);
            }
            // Intro: no arm terminal yet, the hack runs on Lukas's personal phone (D-63).
            bool intro = Services.TryGet(out GameStateMachine sm) && sm.Current == GameStateId.Intro;
            title.text = Loc.Get(intro ? "UI.HACK.PHONE_TITLE" : "UI.HACK.TITLE");
            hideAt = -1f;
            status.text = "";
            status.color = UiKit.Teal;
            if (router == null) Services.TryGet(out router);
            string key = router != null ? router.KeyFor(router.Actions.Hacking.Abort) : "E";
            hint.text = $"[{key}]  {Loc.Get("UI.HACK.ABORT")}";
            if (hack.AvailableFault != null)   // M-17: the player decides when to spend a fault
                hint.text += $"    [{router?.KeyFor(router.Actions.Hacking.UseFault) ?? "F"}]  {Loc.Get("UI.HACK.FAULT")}";
            float k = Services.TryGet(out Save.SaveService saves) ? Mathf.Max(1f, saves.LoadGlobal().textScale) : 1f;
            panel.localScale = new Vector3(k, k, 1f);
            panel.gameObject.SetActive(true);
        }

        private void OnStepped(HackStep step)
        {
            if (step != HackStep.Error) return;
            status.text = Loc.Get("UI.HUD.ACCESS_DENIED");
            status.color = UiKit.Red;
            flashUntil = Time.unscaledTime + FlashSeconds;
        }

        private void OnEnded(bool success)
        {
            if (!success) { Hide(); return; }
            foreach (var c in cells) c.color = UiKit.Teal;
            status.text = Loc.Get("UI.HUD.ACCESS_GRANTED");
            status.color = UiKit.Teal;
            hideAt = Time.unscaledTime + GrantedSeconds;
        }

        private void Hide()
        {
            hideAt = -1f;
            if (panel != null) panel.gameObject.SetActive(false);
        }

        private void Unsubscribe()
        {
            if (hack == null) return;
            hack.Started -= OnStarted;
            hack.Stepped -= OnStepped;
            hack.Ended -= OnEnded;
        }

        // Latin-1 only (static VT323 atlas): no arrow glyphs.
        private static string Glyph(HackDir d) => d switch
        {
            HackDir.Up => "^",
            HackDir.Down => "v",
            HackDir.Left => "<",
            _ => ">",
        };

        private void Build()
        {
            panel = UiKit.Rect("Hack", transform);
            panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.42f);   // above the [E] prompt line
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(8 * (CellSize + 8) + 64, 210);
            UiKit.Panel(panel, new Color(0.01f, 0.05f, 0.05f, 0.92f), UiKit.TealDim);
            UiKit.Column(panel, 6, 16);

            title = UiKit.Text(panel, Loc.Get("UI.HACK.TITLE"), 26, UiKit.Teal);
            title.alignment = TextAlignmentOptions.Center;

            row = UiKit.Rect("Sequence", panel);
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 8;
            h.childAlignment = TextAnchor.MiddleCenter;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandWidth = false;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = CellSize * 1.2f;

            status = UiKit.Text(panel, "", 28, UiKit.Teal);
            status.alignment = TextAlignmentOptions.Center;
            status.gameObject.AddComponent<LayoutElement>().minHeight = 34;
            hint = UiKit.Text(panel, "", 22, UiKit.Teal);
            hint.alignment = TextAlignmentOptions.Center;

            panel.gameObject.SetActive(false);
        }
    }
}
