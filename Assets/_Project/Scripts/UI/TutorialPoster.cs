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
    public sealed class TutorialPoster : MonoBehaviour
    {
        [Tooltip("POST table key; {0}, {1}… are the keys of the actions below.")]
        [SerializeField] private string textKey;
        [Tooltip("Comma-separated actions as Map/Action, e.g. Gameplay/Attack,Gameplay/Aim (a plain string so Aura can set it).")]
        [SerializeField] private string actions = "";
        [SerializeField] private TMP_Text text;

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
            text.text = Loc.Get(textKey, keys);
        }
    }
}
