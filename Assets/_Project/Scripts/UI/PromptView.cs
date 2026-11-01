using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.InputRouting;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Heartbeat.UI
{
    /// <summary>
    /// The contextual "E" prompt (D-84, D-86): "[E] Open" bottom-centre while an interactable is in range.
    /// The key follows the last used device (D-132). One of the four elements allowed on screen.
    /// </summary>
    public sealed class PromptView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup group;

        private InputRouter router;
        private IInteractable target;

        private void OnEnable()
        {
            GameEvents.InteractTargetChanged += OnTarget;
            GameEvents.GameStateChanged += OnState;
            LocalizationSettings.SelectedLocaleChanged += OnLocale;
            Show(null);
        }

        private void OnDisable()
        {
            GameEvents.InteractTargetChanged -= OnTarget;
            GameEvents.GameStateChanged -= OnState;
            LocalizationSettings.SelectedLocaleChanged -= OnLocale;
            if (router != null) router.DeviceChanged -= OnDevice;
        }

        private void Start()
        {
            if (Services.TryGet(out router)) router.DeviceChanged += OnDevice;
        }

        private void OnTarget(MonoBehaviour t) => Show(t as IInteractable);
        private void OnState(GameStateId _, GameStateId to) { if (to != GameStateId.Playing && to != GameStateId.Intro) Show(null); }
        private void OnDevice(bool _) => Show(target);
        private void OnLocale(UnityEngine.Localization.Locale _) => Show(target);

        private void Show(IInteractable t)
        {
            target = t;
            bool visible = t != null && (t as MonoBehaviour) != null;
            group.alpha = visible ? 1f : 0f;
            if (!visible) return;
            string key = router != null ? router.KeyFor(router.Actions.Gameplay.Interact) : "E";
            label.text = $"[{key}]  {Loc.Get(t.PromptKey)}";
        }
    }
}
