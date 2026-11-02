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
    /// Shown on GameEvents.PauseChanged; built in code under this RectTransform.
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
            var box = UiKit.Rect("Box", root);
            box.anchorMin = box.anchorMax = box.pivot = new Vector2(0.5f, 0.5f);
            box.sizeDelta = new Vector2(620, 0);
            UiKit.Panel(box, new Color(0.01f, 0.05f, 0.05f, 0.95f), UiKit.TealDim);
            UiKit.Column(box, 12, 28);
            box.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var title = UiKit.Text(box, Loc.Get("UI.PAUSE.TITLE"), 40, UiKit.Teal);
            title.alignment = TMPro.TextAlignmentOptions.Center;
            var resume = UiKit.Button(box, Loc.Get("UI.PAUSE.RESUME"), 30, UiKit.Teal, () => Services.Get<PauseService>().TryResume(), 56);
            UiKit.Button(box, Loc.Get("UI.MENU.OPTIONS"), 30, UiKit.Teal, ShowOptions, 56);
            bool canRestart = pausedFrom == GameStateId.Playing;   // not in the Intro or the final decision
            var restart = UiKit.Button(box, Loc.Get("UI.PAUSE.RESTART"), 30, UiKit.Teal, ConfirmRestart, 56);
            restart.interactable = canRestart;
            UiKit.Button(box, Loc.Get("UI.PAUSE.QUIT"), 30, UiKit.Teal, ConfirmQuit, 56);
            UiKit.Select(resume);
            HbLog.Info("UI", "Pause menu");
        }

        private void ShowOptions()
        {
            Destroy(root.gameObject);
            root = UiKit.Stretch(UiKit.Rect("PauseOptions", transform), Vector2.zero, Vector2.one);
            root.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
            var box = UiKit.Rect("Box", root);
            box.anchorMin = box.anchorMax = box.pivot = new Vector2(0.5f, 0.5f);
            box.sizeDelta = new Vector2(1300, 0);
            UiKit.Panel(box, new Color(0.01f, 0.05f, 0.05f, 0.95f), UiKit.TealDim);
            UiKit.Column(box, 10, 24);
            box.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            OptionsView.Build(box, root, UiKit.Teal, () => { Destroy(root.gameObject); Build(); });
        }

        private void ConfirmRestart() =>
            ConfirmDialog.Show(root, Loc.Get(Services.Get<CycleController>().InSafeZone ? "UI.CONFIRM.RESTART_SAFE" : "UI.CONFIRM.RESTART"),
                () => Services.Get<CycleController>().Restart(), null, UiKit.Amber);   // free in a safe zone (D-147)

        private void ConfirmQuit()
        {
            var cycle = Services.Get<CycleController>();
            var cost = cycle.QuitCost;
            bool tutorial = cycle.Data != null && cycle.Data.firstAttempt;
            string key = cost == ExitCost.Hunt ? "UI.CONFIRM.QUIT_HUNT"
                : cost == ExitCost.Calm ? (tutorial ? "UI.CONFIRM.QUIT_CALM_TUTORIAL" : "UI.CONFIRM.QUIT_CALM")
                : "UI.CONFIRM.QUIT_FREE";
            HbLog.Info("UI", $"Quit confirmation shows cost={cost}");
            ConfirmDialog.Show(root, Loc.Get(key), cycle.QuitToMenu, null, cost == ExitCost.Hunt ? UiKit.Red : UiKit.Amber);
        }
    }
}
