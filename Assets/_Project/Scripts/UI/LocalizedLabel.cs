using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Heartbeat.UI
{
    /// <summary>Static TMP label bound to a localization key; refreshes when the language changes.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedLabel : MonoBehaviour
    {
        [SerializeField] private string key;
        [Tooltip("Fills {0} with the current subject code, e.g. 09 for R-H-09 (the capsule tray label, C-44).")]
        [SerializeField] private bool subjectCode;

        private TMP_Text text;

        public string Key { get => key; set { key = value; Refresh(); } }

        private void Awake() => text = GetComponent<TMP_Text>();
        private void OnEnable() { LocalizationSettings.SelectedLocaleChanged += OnLocale; Refresh(); }
        private void Start() { if (subjectCode) Refresh(); }   // the cycle's data may arrive after the room is enabled
        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocale;
        private void OnLocale(UnityEngine.Localization.Locale _) => Refresh();

        public void Refresh()
        {
            if (text == null) text = GetComponent<TMP_Text>();
            text.text = subjectCode && Core.Services.TryGet(out Cycle.CycleController cycle) && cycle.Data != null
                ? Loc.Get(key, cycle.Data.subjectCode.ToString("00")) : Loc.Get(key);
        }
    }
}
