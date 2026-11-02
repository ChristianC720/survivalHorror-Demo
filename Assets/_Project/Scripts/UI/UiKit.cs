using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>Small helpers to build the terminal-style uGUI screens in code (terminal, menus, pause, dialogs).</summary>
    public static class UiKit
    {
        public static readonly Color Teal = new Color(0.31f, 0.88f, 0.8f);
        public static readonly Color TealDim = new Color(0.31f, 0.88f, 0.8f, 0.35f);
        public static readonly Color Green = new Color(0.45f, 0.95f, 0.45f);    // facility screens (D-82)
        public static readonly Color Amber = new Color(0.95f, 0.7f, 0.24f);
        public static readonly Color Red = new Color(1f, 0.32f, 0.26f);
        public static readonly Color ButtonIdle = new Color(0f, 0f, 0f, 0.4f);

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static RectTransform Stretch(RectTransform r, Vector2 min, Vector2 max)
        {
            r.anchorMin = min; r.anchorMax = max; r.offsetMin = r.offsetMax = Vector2.zero;
            return r;
        }

        public static VerticalLayoutGroup Column(Component c, int spacing = 8, int padding = 0)
        {
            var v = c.gameObject.AddComponent<VerticalLayoutGroup>();
            v.spacing = spacing;
            v.padding = new RectOffset(padding, padding, padding, padding);
            v.childControlHeight = v.childControlWidth = true;
            v.childForceExpandHeight = false;
            return v;
        }

        public static Image Panel(RectTransform r, Color bg, Color border)
        {
            var img = r.gameObject.AddComponent<Image>();
            img.color = bg;
            var o = r.gameObject.AddComponent<Outline>();
            o.effectColor = border;
            o.effectDistance = new Vector2(2, -2);
            return img;
        }

        public static TMP_Text Text(Transform parent, string text, float size, Color color)
        {
            var t = Rect("Text", parent).gameObject.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.raycastTarget = false;
            // Terminal and facility screens are data (VT323); everything else is menu text (IBM Plex Sans), D-126.
            bool data = SameHue(color, Teal) || SameHue(color, Green);
            UiFonts.Apply(t, data);
            return t;
        }

        public static Button Button(Transform parent, string text, float size, Color color, Action onClick, float height = 44)
        {
            var r = Rect("Button", parent);
            var img = r.gameObject.AddComponent<Image>();
            img.color = new Color(color.r * 0.12f, color.g * 0.12f, color.b * 0.12f, 0.9f);   // tinted so focus is visible on black
            var b = r.gameObject.AddComponent<Button>();
            var colors = b.colors;
            colors.highlightedColor = colors.selectedColor = new Color(3f, 3f, 3f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            b.colors = colors;
            if (onClick != null) b.onClick.AddListener(() => onClick());
            var le = r.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.flexibleHeight = 0;
            var label = Text(r, text, size, color);
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;   // a label never breaks mid-word; it shrinks instead
            label.enableAutoSizing = true;
            label.fontSizeMax = size;
            label.fontSizeMin = size * 0.6f;
            Stretch((RectTransform)label.transform, Vector2.zero, Vector2.one);
            ((RectTransform)label.transform).offsetMin = new Vector2(12, 0);
            ((RectTransform)label.transform).offsetMax = new Vector2(-12, 0);
            return b;
        }

        private static bool SameHue(Color a, Color b) =>
            Mathf.Abs(a.r - b.r) < 0.02f && Mathf.Abs(a.g - b.g) < 0.02f && Mathf.Abs(a.b - b.b) < 0.02f;

        public static void Select(Selectable s)
        {
            if (s != null && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(s.gameObject);
        }

        /// <summary>Canvas root for a code-built screen (1920×1080 reference).</summary>
        public static Canvas Canvas(GameObject go, int order)
        {
            var c = go.GetComponent<Canvas>();
            if (c == null) c = go.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = order;
            var s = go.GetComponent<CanvasScaler>();
            if (s == null) s = go.AddComponent<CanvasScaler>();
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = new Vector2(1920, 1080);
            s.matchWidthOrHeight = 0.5f;
            if (go.GetComponent<GraphicRaycaster>() == null) go.AddComponent<GraphicRaycaster>();
            return c;
        }
    }
}
