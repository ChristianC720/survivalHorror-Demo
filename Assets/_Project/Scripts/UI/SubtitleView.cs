using System.Collections.Generic;
using Heartbeat.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Subtitles (D-52, Accesibilidad): bottom-centre box, ≥ 32 px at 1080p, speaker name in its colour plus the name
    /// itself (never colour alone). Lines queue; each stays for a reading time based on its length. Plays the
    /// speaker's vocal cue. Background opacity and text scale come from the options.
    /// </summary>
    public sealed class SubtitleView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image background;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private float baseFontSize = 36f;
        [SerializeField] private float minSeconds = 2.5f;
        [SerializeField] private float secondsPerChar = 0.06f;

        private readonly Queue<(string speaker, string text)> queue = new Queue<(string, string)>();
        private float until;

        private static string ColorOf(string speaker) =>
            speaker == Subtitles.Mother ? "#FF5A4E" : speaker == Subtitles.Emilia ? "#F2D27A" : "#9FE8DD";

        private void Awake() => UiFonts.Apply(label, data: false);

        private void OnEnable() { GameEvents.SubtitleRequested += OnRequest; GameEvents.OptionsChanged += Start; group.alpha = 0f; }
        private void OnDisable() { GameEvents.SubtitleRequested -= OnRequest; GameEvents.OptionsChanged -= Start; }

        /// <summary>Options hook: text scale (≥ 1) and opaque background (Accesibilidad).</summary>
        public void ApplyOptions(float textScale, bool opaque)
        {
            label.fontSize = baseFontSize * Mathf.Max(1f, textScale);
            var c = background.color;
            c.a = opaque ? 1f : 0.65f;
            background.color = c;
        }

        private void Start()
        {
            if (Services.TryGet(out Save.SaveService saves))
            {
                var g = saves.LoadGlobal();
                ApplyOptions(g.textScale, g.subtitleOpaque);
            }
        }

        private void OnRequest(string speaker, string key, object[] args)
        {
            queue.Enqueue((speaker, Loc.Get(key, args)));
            Sfx.Play2D("CUE_Vocal_" + speaker);
            HbLog.Info("Subtitle", $"{speaker}: {key}");
            if (Time.unscaledTime >= until) Next();
        }

        private void Next()
        {
            if (queue.Count == 0) { group.alpha = 0f; return; }
            var (speaker, text) = queue.Dequeue();
            string name = Loc.Get("SUB.SPEAKER." + speaker.ToUpperInvariant());
            label.text = $"<color={ColorOf(speaker)}>{name}:</color> {text}";
            group.alpha = 1f;
            until = Time.unscaledTime + Mathf.Max(minSeconds, text.Length * secondsPerChar);
        }

        private void Update()
        {
            if (group.alpha > 0f && Time.unscaledTime >= until) Next();
        }
    }
}
