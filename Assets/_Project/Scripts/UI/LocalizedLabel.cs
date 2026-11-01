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

        private TMP_Text text;

        public string Key { get => key; set { key = value; Refresh(); } }

        private void Awake() => text = GetComponent<TMP_Text>();
        private void OnEnable() { LocalizationSettings.SelectedLocaleChanged += OnLocale; Refresh(); }
        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocale;
        private void OnLocale(UnityEngine.Localization.Locale _) => Refresh();

        public void Refresh()
        {
            if (text == null) text = GetComponent<TMP_Text>();
            text.text = Loc.Get(key);
        }
    }
}
