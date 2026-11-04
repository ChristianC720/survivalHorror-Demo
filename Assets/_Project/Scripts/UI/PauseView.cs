using Heartbeat.Core;
using Heartbeat.Cycle;
using Heartbeat.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Pause menu over the darkened image (UI y Menús): Resume, Options, Restart (confirmed: counts as a death; not in
    /// the Intro or the final decision, D-63, D-92) and Quit (confirmed with the D-31 cost of the current situation).
    /// A facility screen: green phosphor (D-82). Shown on GameEvents.PauseChanged; built in code under this RectTransform.
    /// </summary>
    public sealed class PauseView : MonoBehaviour
    {
        private RectTransform root;
        private GameStateId pausedFrom;

        private void OnEnable() { GameEvents.PauseChanged += OnPause; GameEvents.GameStateChanged += OnState; }
        private void OnDisable() { GameEvents.PauseChanged -= OnPause; GameEvents.GameStateChanged -= OnState; }

        private void OnState(GameStateId from, GameStateId to) { if (to == GameStateId.Paused) pausedFrom = from; }

        private void OnPause(bool paused)
        {
            if (root != null) Destroy(root.gameObject);
            root = null;
            if (paused) Build();
        }

        private void Build()
        {
            root = UiKit.Stretch(UiKit.Rect("Pause", transform), Vector2.zero, Vector2.one);
            root.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
            var box = Screen(root, 620f);

            var title = UiKit.Text(box, Loc.Get("UI.PAUSE.TITLE"), 40, UiKit.Green);
            title.alignment = TMPro.TextAlignmentOptions.Center;
            var resume = UiKit.Button(box, Loc.Get("UI.PAUSE.RESUME"), 30, UiKit.Green, () => Services.Get<PauseService>().TryResume(), 56);
            UiKit.Button(box, Loc.Get("UI.MENU.OPTIONS"), 30, UiKit.Green, ShowOptions, 56);
            // No Restart in the Intro or the final decision (D-63, D-92): the button is not shown at all.
            if (pausedFrom == GameStateId.Playing)
                UiKit.Button(box, Loc.Get("UI.PAUSE.RESTART"), 30, UiKit.Green, ConfirmRestart, 56);
            UiKit.Button(box, Loc.Get("UI.PAUSE.QUIT"), 30, UiKit.Green, ConfirmQuit, 56);
            UiKit.Scanlines(box);
            UiKit.Select(resume);
            HbLog.Info("UI", "Pause menu");
        }

        private static RectTransform Screen(RectTransform parent, float width)
        {
            var box = UiKit.Rect("Box", parent);
            box.anchorMin = box.anchorMax = box.pivot = new Vector2(0.5f, 0.5f);
            box.sizeDelta = new Vector2(width, 0);
            UiKit.Crt(box, UiKit.BgFacility, UiKit.Green);
            UiKit.Column(box, 12, 28).childForceExpandWidth = true;
            box.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return box;
        }

        private void ShowOptions()
        {
            Destroy(root.gameObject);
            root = UiKit.Stretch(UiKit.Rect("PauseOptions", transform), Vector2.zero, Vector2.one);
            root.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
            var box = Screen(root, 1300f);
            OptionsView.Build(box, root, UiKit.Green, () => { Destroy(root.gameObject); Build(); });
            UiKit.Scanlines(box);
        }

        private void ConfirmRestart()
        {
            var cycle = Services.Get<CycleController>();
            bool free = cycle.InSafeZone;   // free in a safe zone (D-147)
            ConfirmDialog.Show(root, Loc.Get(free ? "UI.CONFIRM.RESTART_SAFE" : "UI.CONFIRM.RESTART"), cycle.Restart, null, UiKit.Green,
                free ? null : Loc.Get("UI.CONFIRM.COST_DEATH"));
        }

        private void ConfirmQuit()
        {
            var cycle = Services.Get<CycleController>();
            var cost = cycle.QuitCost;
            bool tutorial = cycle.Data != null && cycle.Data.firstAttempt;
            string key = cost == ExitCost.Hunt ? "UI.CONFIRM.QUIT_HUNT"
                : cost == ExitCost.Calm ? (tutorial ? "UI.CONFIRM.QUIT_CALM_TUTORIAL" : "UI.CONFIRM.QUIT_CALM")
                : "UI.CONFIRM.QUIT_FREE";
            string costLine = cost == ExitCost.None ? null : cost == ExitCost.Calm && tutorial ? Loc.Get("UI.CONFIRM.COST_DEGRADE") : Loc.Get("UI.CONFIRM.COST_DEATH");
            HbLog.Info("UI", $"Quit confirmation shows cost={cost}");
            ConfirmDialog.Show(root, Loc.Get(key), cycle.QuitToMenu, null, UiKit.Green, costLine);
        }
    }
}
