using Heartbeat.Core;
using TMPro;
using UnityEngine;

namespace Heartbeat.UI
{
    /// <summary>"SYNCHRONIZING…" corner notice while the autosave is written (D-30). Pulses, then fades out.</summary>
    public sealed class SyncIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private float seconds = 2.5f;

        private float until;

        private void Awake() => UiFonts.Apply(label, data: true, outline: true);
        private void OnEnable() { GameEvents.Autosaved += OnSaved; GameEvents.SyncFailed += OnFailed; group.alpha = 0f; }
        private void OnDisable() { GameEvents.Autosaved -= OnSaved; GameEvents.SyncFailed -= OnFailed; }

        private void OnFailed() => Show(Loc.Get("UI.HUD.SYNC_FAILED"), UiKit.Red);
        private void OnSaved() => Show(Loc.Get("UI.HUD.SYNC"), UiKit.Teal);

        private void Show(string text, Color color)
        {
            label.text = text;
            label.color = color;
            until = Time.unscaledTime + seconds;
        }

        private void Update()
        {
            float left = until - Time.unscaledTime;
            group.alpha = left <= 0f ? 0f : Mathf.Min(1f, left / 0.5f) * (0.6f + 0.4f * Mathf.Abs(Mathf.Sin(Time.unscaledTime * 4f)));
        }
    }
}
