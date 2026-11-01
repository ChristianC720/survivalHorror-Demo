using System.Collections.Generic;
using Heartbeat.Audio;
using Heartbeat.Core;
using Heartbeat.Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Sound captions (D-102, Accesibilidad), off by default: signal sounds (alerts, heartbeat, Boca scream, recycle
    /// warning, hostile cameras, system warnings) appear as "[Searching sniffs, left]" from the CAP table, with an
    /// approximate direction only — never distance or screen position (D-02). Same styling options as subtitles.
    /// Development builds also list placeholder cues that have no clip yet (top right, small) so QA can "hear" them.
    /// </summary>
    public sealed class SoundCaptionView : MonoBehaviour
    {
        [SerializeField] private float baseFontSize = 30f;
        [SerializeField] private float seconds = 2.5f;
        [SerializeField] private int maxLines = 3;

        private RectTransform captionStack, devStack;
        private readonly List<(GameObject go, float until)> lines = new List<(GameObject, float)>();
        private bool enabledByPlayer, opaque;
        private float textScale = 1f;

        private void Awake()
        {
            captionStack = Stack("Captions", new Vector2(0.03f, 0.2f), new Vector2(0.45f, 0.45f), TextAnchor.LowerLeft);
            devStack = Stack("DevPlaceholders", new Vector2(0.72f, 0.7f), new Vector2(0.99f, 0.99f), TextAnchor.UpperRight);
        }

        private void OnEnable()
        {
            GameEvents.CuePlayed += OnCue;
            GameEvents.OptionsChanged += ReadOptions;
        }

        private void OnDisable()
        {
            GameEvents.CuePlayed -= OnCue;
            GameEvents.OptionsChanged -= ReadOptions;
        }

        private void Start() => ReadOptions();

        private void ReadOptions()
        {
            if (!Services.TryGet(out SaveService saves)) return;
            var g = saves.LoadGlobal();
            enabledByPlayer = g.soundCaptions;
            opaque = g.subtitleOpaque;
            textScale = Mathf.Max(1f, g.textScale);
        }

        private void OnCue(string id, Vector3? pos)
        {
            if (!Services.TryGet(out AudioService audio) || !audio.TryGetCue(id, out var cue)) return;
            if (enabledByPlayer && cue.priority == CuePriority.Signal)
            {
                string key = "CAP." + id.Substring(4).ToUpperInvariant();   // CUE_Alert_Hunt -> CAP.ALERT_HUNT
                string text = Loc.Get(key);
                if (text != key) Add(captionStack, $"[{text}{Direction(pos)}]", baseFontSize * textScale, opaque ? 1f : 0.65f);
            }
            else if (Debug.isDebugBuild && cue.PickClip() == null && !string.IsNullOrEmpty(cue.debugCaption))
                Add(devStack, "* " + cue.debugCaption, 16f, 0.5f);
        }

        /// <summary>Approximate direction relative to the camera: ahead / behind / left / right.</summary>
        private static string Direction(Vector3? pos)
        {
            var cam = Camera.main;
            if (!pos.HasValue || cam == null) return "";
            Vector3 d = cam.transform.InverseTransformPoint(pos.Value);
            float angle = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
            string key = Mathf.Abs(angle) < 45f ? "UI.DIR.AHEAD" : Mathf.Abs(angle) > 135f ? "UI.DIR.BEHIND" : angle > 0f ? "UI.DIR.RIGHT" : "UI.DIR.LEFT";
            return ", " + Loc.Get(key);
        }

        private void Add(RectTransform stack, string text, float size, float bgAlpha)
        {
            var row = UiKit.Rect("Line", stack);
            row.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, bgAlpha);
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.padding = new RectOffset(14, 14, 4, 4);
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandWidth = h.childForceExpandHeight = false;   // hug the text
            var t = UiKit.Text(row, text, size, new Color(0.95f, 0.95f, 0.92f));
            t.textWrappingMode = TextWrappingModes.NoWrap;
            lines.Add((row.gameObject, Time.unscaledTime + seconds));
            int count = 0;
            for (int i = lines.Count - 1; i >= 0; i--)
                if (lines[i].go != null && lines[i].go.transform.parent == stack && ++count > maxLines) { Destroy(lines[i].go); lines.RemoveAt(i); }
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            for (int i = lines.Count - 1; i >= 0; i--)
                if (lines[i].until < now) { if (lines[i].go != null) Destroy(lines[i].go); lines.RemoveAt(i); }
        }

        private RectTransform Stack(string name, Vector2 min, Vector2 max, TextAnchor align)
        {
            var r = UiKit.Stretch(UiKit.Rect(name, transform), min, max);
            var v = r.gameObject.AddComponent<VerticalLayoutGroup>();
            v.childAlignment = align;
            v.spacing = 4;
            v.childControlWidth = v.childControlHeight = true;
            v.childForceExpandWidth = v.childForceExpandHeight = false;
            return r;
        }
    }
}
