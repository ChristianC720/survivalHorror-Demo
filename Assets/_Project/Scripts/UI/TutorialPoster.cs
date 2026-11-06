using System.Linq;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using TMPro;
using UnityEngine;

namespace Heartbeat.UI
{
    /// <summary>
    /// Vitalis safety poster of the tutorial rooms (D-89): English text with the current binding of each action,
    /// following the last device used and rebinding on the fly (D-132). Placed by hand in the F1 tutorial room prefabs
    /// (pivot on the wall face, local +Z into the wall); <see cref="Generation.FloorBuilder"/> keeps it only in the fixed
    /// rooms of the first attempt and switches it off everywhere else (procedural reuse, F2/F3 re-dressed variants).
    /// </summary>
    public sealed class TutorialPoster : MonoBehaviour, Interaction.IInteractable
    {
        public string PromptKey => "UI.PROMPT.READ";
        public Vector3 InteractionPoint => transform.position;
        public bool CanInteract(Interaction.Interactor who) => text != null && isActiveAndEnabled;

        /// <summary>Read up close like any document: the world stops (D-159).</summary>
        public void Interact(Interaction.Interactor who) => GameEvents.RaiseReadPoster(text.text, null);

        private void Awake()
        {
            if (text == null) return;
            var trigger = new GameObject("ReadTrigger", typeof(BoxCollider));
            trigger.layer = LayerMask.NameToLayer("Interactable");
            trigger.transform.SetParent(transform, false);
            var box = trigger.GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(0.8f, 0.8f, 0.2f);
        }

        [Tooltip("POST table key; {0}, {1}… are the keys of the actions below.")]
        [SerializeField] private string textKey;
        [Tooltip("Comma-separated actions as Map/Action, e.g. Gameplay/Attack,Gameplay/Aim (a plain string so Aura can set it).")]
        [SerializeField] private string actions = "";
        [SerializeField] private TMP_Text text;
        [Tooltip("Tutorial safety posters exist only in the first attempt; ambience posters (German, D-58) always stay.")]
        [SerializeField] private bool tutorialOnly = true;

        public bool TutorialOnly => tutorialOnly;

        private InputRouter router;

        private void OnEnable()
        {
            if (Services.TryGet(out router)) router.DeviceChanged += OnDeviceChanged;
            GameEvents.OptionsChanged += Refresh;   // rebinding and language
            Refresh();
        }

        private void OnDisable()
        {
            if (router != null) router.DeviceChanged -= OnDeviceChanged;
            GameEvents.OptionsChanged -= Refresh;
        }

        private void OnDeviceChanged(bool _) => Refresh();

        private void Refresh()
        {
            if (text == null) return;
            var keys = actions.Split(',').Select(a =>
            {
                var action = router?.Actions.asset.FindAction(a.Trim());
                return (object)(action != null ? router.KeyFor(action) : "?");
            }).ToArray();
            text.text = Headline(Loc.Get(textKey, keys));
        }

        /// <summary>The first line is the headline, bold in deep Vitalis cyan (D-157), so a safety poster reads at a glance.
        /// (TMP's mark tag draws over the glyphs, so a solid band would hide them.)</summary>
        private static string Headline(string s)
        {
            int br = s.IndexOf('\n');
            string head = br < 0 ? s : s.Substring(0, br), rest = br < 0 ? "" : s.Substring(br);
            return $"<color=#0B5E6B><size=120%><b>{head}</b></size></color>{rest}";
        }
    }
}
