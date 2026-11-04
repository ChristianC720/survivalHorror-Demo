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
    /// The hack sequence on the arm terminal (M-13, Interfaz Visual): a chain of key boxes, entered ones filled, the next one
    /// framed and blinking, the rest dim, so the state is never colour alone. An error flashes red ACCESS DENIED and the
    /// chain restarts; success shows ACCESS GRANTED. Never pauses; added at runtime by <see cref="TerminalView"/>. In the
    /// Intro it is Lukas's own phone instead (D-63).
    /// </summary>
    public sealed class HackView : MonoBehaviour
    {
        private const float CellSize = 96f, CellGap = 8f, RowWidth = 832f, FlashSeconds = 0.6f, GrantedSeconds = 0.8f;
        private static readonly Color PhoneBg = new Color(0.93f, 0.91f, 0.88f);

        private RectTransform panel, row, keys;
        private Image panelImage;
        private Outline panelOutline;
        private TMP_Text title, status;
        private readonly List<Image> cellBoxes = new List<Image>();
        private readonly List<Outline> cellEdges = new List<Outline>();
        private readonly List<TMP_Text> cellText = new List<TMP_Text>();
        private PlayerHack hack;
        private InputRouter router;
        private Color fg = UiKit.Teal, ink = UiKit.Ink, off = new Color(0.165f, 0.498f, 0.459f);
        private float flashUntil, hideAt = -1f;
        private float scale = 1f;

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
            status.color = fg;
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
                for (int i = 0; i < cellBoxes.Count; i++) Paint(i, i < done, i == done, blink);
                if (Time.unscaledTime >= flashUntil) status.text = "";
            }
            else if (hideAt > 0f && Time.unscaledTime >= hideAt) Hide();
        }

        private void Paint(int i, bool entered, bool next, bool blink)
        {
            cellBoxes[i].color = entered ? fg : ink;   // solid: an Outline copies the shape, so the frame needs a fill inside
            cellEdges[i].effectColor = entered || next ? fg : off;
            cellEdges[i].effectDistance = next && blink ? new Vector2(5, -5) : new Vector2(2, -2);
            cellText[i].color = entered ? ink : next ? fg : off;
        }

        private void OnStarted(HackSequence seq)
        {
            foreach (var c in cellBoxes) Destroy(c.gameObject);
            cellBoxes.Clear();
            cellEdges.Clear();
            cellText.Clear();
            if (router == null) Services.TryGet(out router);

            // Intro: no arm terminal yet, the hack runs on Lukas's personal phone (D-63).
            bool intro = Services.TryGet(out GameStateMachine sm) && sm.Current == GameStateId.Intro;
            ApplyStyle(intro);
            float width = intro ? 432f : RowWidth;
            float size = Mathf.Min(CellSize, (width - CellGap * (seq.Steps.Count - 1)) / Mathf.Max(1, seq.Steps.Count));
            foreach (var d in seq.Steps) AddCell(d, size);
            title.text = Loc.Get(intro ? "UI.HACK.PHONE_TITLE" : "UI.HACK.TITLE");
            hideAt = -1f;
            status.text = "";
            status.color = fg;

            foreach (Transform c in keys) Destroy(c.gameObject);
            UiKit.KeyHint(keys, router != null ? router.KeyFor(router.Actions.Hacking.Abort) : "E", Loc.Get("UI.HACK.ABORT"), fg, ink);
            if (hack.AvailableFault != null)   // M-17: the player decides when to spend a fault
                UiKit.KeyHint(keys, router != null ? router.KeyFor(router.Actions.Hacking.UseFault) : "F", Loc.Get("UI.HACK.FAULT"), fg, ink);

            scale = Services.TryGet(out Save.SaveService saves) ? Mathf.Max(1f, saves.LoadGlobal().terminalScale) : 1f;
            panel.localScale = new Vector3(scale, scale, 1f);
            panel.gameObject.SetActive(true);
        }

        private void AddCell(HackDir d, float size)
        {
            var r = UiKit.Rect("Key", row);
            var le = r.gameObject.AddComponent<LayoutElement>();
            le.preferredWidth = le.preferredHeight = size;
            var box = r.gameObject.AddComponent<Image>();
            var edge = r.gameObject.AddComponent<Outline>();
            edge.useGraphicAlpha = false;
            var t = UiKit.Text(r, KeyLabel(d), size * 0.5f, off, true);
            t.alignment = TextAlignmentOptions.Center;
            UiKit.Stretch(t.rectTransform, Vector2.zero, Vector2.one);
            cellBoxes.Add(box);
            cellEdges.Add(edge);
            cellText.Add(t);
        }

        private void ApplyStyle(bool phone)
        {
            fg = phone ? UiKit.Ink : UiKit.Teal;
            ink = phone ? PhoneBg : UiKit.Ink;
            off = phone ? new Color(0.55f, 0.53f, 0.5f) : new Color(0.165f, 0.498f, 0.459f);
            panelImage.color = phone ? PhoneBg : UiKit.BgTerminal;
            panelOutline.effectColor = phone ? UiKit.Bezel : off;
            title.color = fg;
            panel.anchorMin = panel.anchorMax = panel.pivot = phone ? new Vector2(0.5f, 0.5f) : Vector2.zero;
            panel.anchoredPosition = phone ? Vector2.zero : new Vector2(64f, 72f);
            panel.sizeDelta = phone ? new Vector2(480f, 700f) : new Vector2(880f, 330f);
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
            for (int i = 0; i < cellBoxes.Count; i++) Paint(i, true, false, false);
            status.text = Loc.Get("UI.HUD.ACCESS_GRANTED");
            status.color = fg;
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

        /// <summary>The letter of the bound key when it is short (WASD); otherwise a direction, also for the gamepad.</summary>
        private string KeyLabel(HackDir d)
        {
            if (router != null)
            {
                var a = router.Actions.Hacking;
                string key = router.KeyFor(d == HackDir.Up ? a.Up : d == HackDir.Down ? a.Down : d == HackDir.Left ? a.Left : a.Right);
                string first = key.Split('|')[0].Trim();
                if (first.Length == 1) return first;
            }
            return Glyph(d);
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
            panelImage = UiKit.Crt(panel, UiKit.BgTerminal, off);
            panelOutline = panel.GetComponent<Outline>();
            UiKit.Column(panel, 14, 24);

            title = UiKit.Text(panel, Loc.Get("UI.HACK.TITLE"), 28, UiKit.Teal, true);
            title.alignment = TextAlignmentOptions.MidlineLeft;

            row = UiKit.Rect("Sequence", panel);
            var h = UiKit.Row(row, (int)CellGap, TextAnchor.MiddleLeft);
            h.childForceExpandWidth = false;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = CellSize;

            status = UiKit.Text(panel, "", 32, UiKit.Teal, true);
            status.alignment = TextAlignmentOptions.MidlineLeft;
            status.gameObject.AddComponent<LayoutElement>().minHeight = 40;

            keys = UiKit.Rect("Keys", panel);
            UiKit.Row(keys, 24);
            keys.gameObject.AddComponent<LayoutElement>().minHeight = 40;

            panel.gameObject.SetActive(false);
        }
    }
}
