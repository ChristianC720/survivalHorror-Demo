using System.Collections.Generic;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>The electrocardiogram: its shape tells the state, not only its colour (Interfaz Visual).</summary>
    public sealed class UiEcg : MonoBehaviour
    {
        public HealthState State { get; set; } = HealthState.Fine;
        public Color Color { get; set; } = UiKit.Teal;
        public float Period { get; set; } = 0.03f;

        private readonly List<RectTransform> bars = new List<RectTransform>();
        private readonly List<Image> images = new List<Image>();
        private RectTransform strip;

        public static UiEcg Build(Transform parent, int count, float height, float barSpacing = 2f)
        {
            var r = UiKit.Rect("ECG", parent);
            var le = r.gameObject.AddComponent<LayoutElement>();
            le.minHeight = le.preferredHeight = height;
            r.gameObject.AddComponent<RectMask2D>();
            var h = UiKit.Row(r, (int)barSpacing);
            h.childControlHeight = false;
            h.childForceExpandWidth = true;
            var e = r.gameObject.AddComponent<UiEcg>();
            e.strip = r;
            for (int i = 0; i < count; i++)
            {
                var bar = UiKit.Rect("b", r);
                bar.sizeDelta = new Vector2(3, 4);
                e.images.Add(bar.gameObject.AddComponent<Image>());
                e.bars.Add(bar);
            }
            return e;
        }

        private void Update()
        {
            float t = Time.unscaledTime;
            float room = Mathf.Max(6f, strip.rect.height - 4f);
            for (int i = 0; i < bars.Count; i++)
            {
                float v = TerminalView.EcgSample(State, t - (bars.Count - i) * Period);
                bars[i].sizeDelta = new Vector2(bars[i].sizeDelta.x, 3f + (room - 3f) * v);
                images[i].color = Color;
            }
        }
    }
}
