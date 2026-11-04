using System;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Reading a document or a poster (D-159): a view of its own, separate from the terminal's list, that stops the world
    /// until closed. Added at runtime by <see cref="TerminalView"/>, which also opens it for the entries of LOGS.
    /// </summary>
    public sealed class ReaderView : MonoBehaviour
    {
        private static readonly Color PosterPaper = new Color(0.91f, 0.89f, 0.82f);

        private InputRouter router;
        private PauseService pause;
        private RectTransform root, panel, footer;
        private Image panelImage, band;
        private Outline panelOutline;
        private TMP_Text title, body, pageLabel;
        private GameObject scanlines;
        private Canvas sorting;
        private Color footerColor;
        private Action onClosed;
        private InputContext previous;
        private GameObject previousSelected;
        private int page = 1, openedFrame;
        private bool navHeld;

        // ---------- Text of a document ----------

        public static string TitleOf(DocumentDefinition d)
        {
            string t = Loc.Get(d.titleKey);
            return t == d.titleKey && !string.IsNullOrEmpty(d.placeholderTitle) ? d.placeholderTitle : t;
        }

        private static string BodyOf(DocumentDefinition d)
        {
            // {0} = Lukas's current subject number; {c} passages stay corrupt until the ending reveals them (D-127, D-55).
            var cycle = Services.TryGet(out Cycle.CycleController c) ? c.Data : null;
            string n = cycle != null ? cycle.subjectCode.ToString("00") : "07";
            bool reveal = cycle != null && cycle.cycleCompleted && Services.Get<RegistryModel>().HasAllEmilia;
            string text = DocText.Render(Loc.Get(d.bodyKey, n), reveal);
            if (Level.FaultModel.Faults.ContainsKey(d.id) && Services.TryGet(out Level.FaultModel faults))   // M-17 status
                text += "\n\n" + (faults.IsPatched(d.id) ? Loc.Get("UI.TERM.FAULT_PATCHED") : Loc.Get("UI.TERM.FAULT_USES", faults.UsesLeft(d.id)));
            return text;
        }

        // Vitalis wrote these; Emilia's files and Lukas's own things are not theirs (D-157).
        private static bool IsVitalis(RecordKind k) => k == RecordKind.ContextDocument || k == RecordKind.Rule || k == RecordKind.Fault;

        // ---------- Life cycle ----------

        private void Awake() => Build();

        private void Start()
        {
            router = Services.Get<InputRouter>();
            pause = Services.Get<PauseService>();
            router.Actions.Gameplay.Interact.performed += OnCloseInput;
            router.Actions.UI.Cancel.performed += OnCloseInput;
            router.Actions.UI.Navigate.performed += OnNavigate;
            router.Actions.UI.Navigate.canceled += OnNavigate;
        }

        private void OnEnable() { GameEvents.ReadDocument += OpenDocument; GameEvents.ReadPoster += OpenPoster; }
        private void OnDisable() { GameEvents.ReadDocument -= OpenDocument; GameEvents.ReadPoster -= OpenPoster; }

        private void OnDestroy()
        {
            if (root != null && root.gameObject.activeSelf) pause?.EndReading();   // a stopped clock must not outlive the view
            if (root != null) Destroy(root.gameObject);
            if (router == null) return;
            router.Actions.Gameplay.Interact.performed -= OnCloseInput;
            router.Actions.UI.Cancel.performed -= OnCloseInput;
            router.Actions.UI.Navigate.performed -= OnNavigate;
            router.Actions.UI.Navigate.canceled -= OnNavigate;
        }

        public bool IsOpen => root != null && root.gameObject.activeSelf;

        // ---------- Open / close ----------

        public void OpenDocument(DocumentDefinition d, Action closed)
        {
            if (IsOpen || d == null) { closed?.Invoke(); return; }
            Style(1320f, 860f, UiKit.BgTerminal, UiKit.Teal, UiKit.Teal, IsVitalis(d.kind) ? UiKit.Cyan : UiKit.Teal, true, false);
            float k = Services.TryGet(out Save.SaveService saves) ? Mathf.Max(1f, saves.LoadGlobal().terminalScale) : 1f;
            title.text = TitleOf(d);
            body.fontSize = 32f * k;
            Open(BodyOf(d), closed);
            HbLog.Info("Terminal", $"Read {d.id}");
        }

        public void OpenPoster(string text, Action closed)
        {
            if (IsOpen) { closed?.Invoke(); return; }
            Style(760f, 900f, PosterPaper, UiKit.Ink, UiKit.Ink, UiKit.Ink, false, true);
            title.text = Loc.Get("UI.TERM.BRAND");
            body.fontSize = 34f;
            Open(text, closed);
            HbLog.Info("Terminal", "Read poster");
        }

        private void Style(float w, float h, Color bg, Color edge, Color text, Color head, bool data, bool poster)
        {
            panel.sizeDelta = new Vector2(w, h);
            panelImage.color = bg;
            panelOutline.effectColor = edge;
            band.gameObject.SetActive(poster);
            UiFonts.Apply(title, !poster);
            UiFonts.Apply(body, data);
            title.color = poster ? Color.white : head;
            body.color = text;
            pageLabel.color = text;
            scanlines.SetActive(!poster);
            foreach (Transform c in footer) Destroy(c.gameObject);
            footerColor = text;
        }

        private void Open(string text, Action closed)
        {
            onClosed = closed;
            previous = router.Context;
            previousSelected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            EventSystem.current?.SetSelectedGameObject(null);   // Submit must not press the list entry again behind the page
            openedFrame = Time.frameCount;
            root.gameObject.SetActive(true);
            sorting.overrideSorting = true;   // only takes while active
            sorting.sortingOrder = 60;        // above the terminal and the prompt
            body.text = text;
            page = 1;
            navHeld = false;
            pause.BeginReading();
            router.SetContext(InputContext.Reading);
            UiKit.KeyHint(footer, router.KeyFor(router.Actions.Gameplay.Interact), Loc.Get("UI.TERM.CLOSE"), footerColor, panelImage.color);
            Canvas.ForceUpdateCanvases();
            ShowPage();
        }

        private void ShowPage()
        {
            body.pageToDisplay = page;
            body.ForceMeshUpdate();
            int pages = Mathf.Max(1, body.textInfo.pageCount);
            pageLabel.text = pages > 1 ? Loc.Get("UI.READ.PAGE", page, pages) : "";
        }

        private void OnCloseInput(InputAction.CallbackContext _)
        {
            if (!IsOpen || Time.frameCount == openedFrame) return;   // the E that opened it
            root.gameObject.SetActive(false);
            pause.EndReading();
            router.SetContext(previous);
            if (previousSelected != null) EventSystem.current?.SetSelectedGameObject(previousSelected);
            var closed = onClosed;
            onClosed = null;
            closed?.Invoke();
        }

        private void OnNavigate(InputAction.CallbackContext c)
        {
            if (!IsOpen) return;
            var v = c.ReadValue<Vector2>();
            float step = v.x + -v.y;
            if (Mathf.Abs(step) < 0.5f) { navHeld = false; return; }
            if (navHeld) return;
            navHeld = true;
            int pages = Mathf.Max(1, body.textInfo.pageCount);
            page = Mathf.Clamp(page + (step > 0f ? 1 : -1), 1, pages);
            ShowPage();
        }

        // ---------- Construction ----------

        private void Build()
        {
            root = UiKit.Stretch(UiKit.Rect("Reader", GetComponentInParent<Canvas>(true).rootCanvas.transform), Vector2.zero, Vector2.one);
            sorting = root.gameObject.AddComponent<Canvas>();
            root.gameObject.AddComponent<GraphicRaycaster>();
            root.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

            panel = UiKit.Rect("Page", root);
            panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
            panelImage = UiKit.Panel(panel, UiKit.BgTerminal, UiKit.Teal);
            panelOutline = panel.GetComponent<Outline>();
            panelOutline.useGraphicAlpha = false;

            band = UiKit.Rect("Band", panel).gameObject.AddComponent<Image>();
            band.color = UiKit.Cyan;
            var bandRect = band.rectTransform;
            bandRect.anchorMin = new Vector2(0f, 1f);
            bandRect.anchorMax = Vector2.one;
            bandRect.pivot = new Vector2(0.5f, 1f);
            bandRect.offsetMin = new Vector2(0f, -76f);
            bandRect.offsetMax = Vector2.zero;

            title = UiKit.Text(panel, "", 40, UiKit.Teal);
            Anchor(title.rectTransform, 1f, 1f, 36, -68, -36, -8);
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.textWrappingMode = TextWrappingModes.NoWrap;
            title.overflowMode = TextOverflowModes.Ellipsis;

            body = UiKit.Text(panel, "", 32, UiKit.Teal);
            Anchor(body.rectTransform, 0f, 1f, 36, 76, -36, -92);
            body.textWrappingMode = TextWrappingModes.Normal;
            body.overflowMode = TextOverflowModes.Page;
            body.alignment = TextAlignmentOptions.TopLeft;

            footer = UiKit.Rect("Keys", panel);
            Anchor(footer, 0f, 0f, 36, 18, -36, 66);
            UiKit.Row(footer, 16);
            pageLabel = UiKit.Text(panel, "", 26, UiKit.Teal, true);
            Anchor(pageLabel.rectTransform, 0f, 0f, 36, 18, -36, 66);
            pageLabel.alignment = TextAlignmentOptions.MidlineRight;
            scanlines = UiKit.Scanlines(panel);

            root.gameObject.SetActive(false);
        }

        private static void Anchor(RectTransform r, float minY, float maxY, float left, float bottom, float right, float top)
        {
            r.anchorMin = new Vector2(0f, minY);
            r.anchorMax = new Vector2(1f, maxY);
            r.offsetMin = new Vector2(left, bottom);
            r.offsetMax = new Vector2(right, top);
        }
    }
}
