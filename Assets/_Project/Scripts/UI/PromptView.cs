using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.InputRouting;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Heartbeat.UI
{
    /// <summary>
    /// The contextual "E" prompt (D-84, D-86): "[E] Open" bottom-centre while an interactable is in range, or the two
    /// finisher actions while Lukas stands over a body (Controles, D-27).
    /// The key follows the last used device (D-132). One of the four elements allowed on screen.
    /// </summary>
    public sealed class PromptView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup group;

        private InputRouter router;
        private IInteractable target;
        private Interactor interactor;
        private RectTransform holdBar;
        private string shownKey;
        private bool shownHold;
        private const float HoldBarWidth = 160f;

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
            if (router != null) { router.DeviceChanged -= OnDevice; router.ContextChanged -= OnContext; }
        }

        private void Awake()
        {
            UiFonts.Apply(label, data: false, outline: true);
            // Progress of a held Interact (door seal), right under the prompt; it grows from the left.
            holdBar = UiKit.Rect("HoldBar", label.transform);
            holdBar.anchorMin = holdBar.anchorMax = new Vector2(0.5f, 0f);
            holdBar.pivot = new Vector2(0f, 1f);
            holdBar.sizeDelta = new Vector2(HoldBarWidth, 6f);
            holdBar.anchoredPosition = new Vector2(-HoldBarWidth / 2f, -4f);
            holdBar.gameObject.AddComponent<UnityEngine.UI.Image>().raycastTarget = false;
            holdBar.localScale = new Vector3(0f, 1f, 1f);
        }

        private void Update()
        {
            if (target == null || (target as MonoBehaviour) == null) { holdBar.localScale = new Vector3(0f, 1f, 1f); return; }
            // A door changes its prompt without changing target (open, closed, sealed).
            if (!ReferenceEquals(target.PromptKey, shownKey) || HoldOffered(target) != shownHold) Show(target);
            float p = interactor != null || Services.TryGet(out interactor) ? interactor.HoldProgress : 0f;
            holdBar.localScale = new Vector3(group.alpha > 0f ? p : 0f, 1f, 1f);
        }

        private static bool HoldOffered(IInteractable t) => t is IHoldInteractable h && h.CanHold;

        private void Start()
        {
            if (Services.TryGet(out router)) { router.DeviceChanged += OnDevice; router.ContextChanged += OnContext; }
        }

        private void OnTarget(MonoBehaviour t) => Show(t as IInteractable);
        private void OnState(GameStateId _, GameStateId to) { if (!Playable(to)) Show(null); }

        private static bool Playable(GameStateId s) => s == GameStateId.Playing || s == GameStateId.Intro || s == GameStateId.FinalDecision;
        private void OnDevice(bool _) => Show(target);
        private void OnContext(InputRouting.InputContext _) => Show(target);   // terminal, hack, surveillance: E does not interact there
        private void OnLocale(UnityEngine.Localization.Locale _) => Show(target);

        private void Show(IInteractable t)
        {
            target = t;
            string text = null;
            if (!Services.TryGet(out GameStateMachine states) || Playable(states.Current))
            {
                // Over a body (D-27) the click and E finish it instead of shoving and interacting, so the prompt says so.
                if (router != null && router.Context == InputContext.OverBody) text = FinisherText();
                else if (t != null && (t as MonoBehaviour) != null && (router == null || router.Actions.Gameplay.Interact.enabled))
                {
                    string key = Key(router?.Actions.Gameplay.Interact, "E");
                    text = $"[{key}]  {Loc.Get(t.PromptKey)}";
                    if (HoldOffered(t)) text += $"    [{key}]  {Loc.Get(((IHoldInteractable)t).HoldPromptKey)}";
                }
            }
            shownKey = (t as MonoBehaviour) != null ? t.PromptKey : null;
            shownHold = shownKey != null && HoldOffered(t);
            group.alpha = text != null ? 1f : 0f;
            if (text != null) label.text = $"<mark=#000000A0 padding=\"12,12,4,4\">{text}</mark>";
        }

        private string FinisherText()
        {
            string text = $"[{Key(router.Actions.Gameplay.Attack, "LMB")}]  {Loc.Get("UI.PROMPT.STRIKE")}";
            if (Services.TryGet(out Interactor who) && who.Inventory.Model.Has(Inventory.ItemKind.Spike))
                text += $"    [{Key(router.Actions.Gameplay.Interact, "E")}]  {Loc.Get("UI.PROMPT.SPIKE")}";
            return text;
        }

        private string Key(UnityEngine.InputSystem.InputAction action, string fallback) => router != null && action != null ? router.KeyFor(action) : fallback;
    }
}
