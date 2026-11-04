using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Small helpers to build the terminal-style uGUI screens in code (terminal, menus, pause, dialogs). Colours and
    /// sizes follow the approved design (Figma "Identity Dissolution — UI/UX", GDD Interfaz Visual).
    /// </summary>
    public static class UiKit
    {
        public static readonly Color Teal = new Color(0.31f, 0.88f, 0.8f);
        public static readonly Color TealDim = new Color(0.31f, 0.88f, 0.8f, 0.35f);
        public static readonly Color Green = new Color(0.45f, 0.95f, 0.45f);    // facility screens (D-82)
        public static readonly Color Amber = new Color(0.95f, 0.7f, 0.24f);
        public static readonly Color Red = new Color(1f, 0.32f, 0.26f);
        public static readonly Color Cyan = new Color(0.098f, 0.769f, 0.863f);  // Vitalis Konzern's mark, never player information (D-157)
        public static readonly Color Text1 = new Color(0.93f, 0.91f, 0.88f);
        public static readonly Color Ink = new Color(0.012f, 0.063f, 0.027f);   // text on a lit (focused) row
        public static readonly Color BgTerminal = new Color(0.016f, 0.063f, 0.059f);
        public static readonly Color BgFacility = new Color(0.012f, 0.063f, 0.027f);
        public static readonly Color Bezel = new Color(0.29f, 0.267f, 0.259f);
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

        public static HorizontalLayoutGroup Row(Component c, int spacing = 8, TextAnchor align = TextAnchor.MiddleLeft)
        {
            var h = c.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = spacing;
            h.childAlignment = align;
            h.childControlHeight = h.childControlWidth = true;
            h.childForceExpandHeight = h.childForceExpandWidth = false;
            return h;
        }

        public static Image Panel(RectTransform r, Color bg, Color border)
        {
            var img = r.gameObject.AddComponent<Image>();
            img.color = bg;
            var o = r.gameObject.AddComponent<Outline>();
            o.effectColor = border;
            o.effectDistance = new Vector2(3, -3);
            return img;
        }

        /// <summary>A CRT screen: phosphor edge inside a dark housing (Interfaz Visual).</summary>
        public static Image Crt(RectTransform r, Color bg, Color edge)
        {
            var img = Panel(r, bg, edge);
            var housing = r.gameObject.AddComponent<Outline>();
            housing.effectColor = Bezel;
            housing.effectDistance = new Vector2(10, -10);
            housing.useGraphicAlpha = false;
            return img;
        }

        /// <param name="data">VT323 (data and readings) or IBM Plex Sans (titles and menus), D-126. Teal text is data unless told otherwise.</param>
        public static TMP_Text Text(Transform parent, string text, float size, Color color, bool? data = null)
        {
            var t = Rect("Text", parent).gameObject.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.raycastTarget = false;
            UiFonts.Apply(t, data ?? SameHue(color, Teal));
            return t;
        }

        public static Button Button(Transform parent, string text, float size, Color color, Action onClick, float height = 44, bool? data = null)
        {
            var r = Rect("Button", parent);
            var img = r.gameObject.AddComponent<Image>();
            var b = r.gameObject.AddComponent<Button>();
            b.transition = Selectable.Transition.None;   // UiFocusStyle inverts the row: focus is never colour alone
            if (onClick != null) b.onClick.AddListener(() => onClick());
            var le = r.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.flexibleHeight = 0;
            var label = Text(r, text, size, color, data);
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;   // a label never breaks mid-word; it shrinks instead
            label.enableAutoSizing = true;
            label.fontSizeMax = size;
            label.fontSizeMin = size * 0.6f;
            Stretch((RectTransform)label.transform, Vector2.zero, Vector2.one);
            // VT323's line box is taller than its glyphs: with a tight rect, auto-size would shrink the text on low rows.
            ((RectTransform)label.transform).offsetMin = new Vector2(32, -16);
            ((RectTransform)label.transform).offsetMax = new Vector2(-12, 16);
            var marker = Text(r, ">", size, color, true);
            marker.alignment = TextAlignmentOptions.MidlineLeft;
            Stretch((RectTransform)marker.transform, Vector2.zero, Vector2.one);
            ((RectTransform)marker.transform).offsetMin = new Vector2(12, 0);
            r.gameObject.AddComponent<UiFocusStyle>().Init(img, color, marker, b);
            return b;
        }

        /// <summary>A key in its box plus what it does. Follows the screen's colour.</summary>
        public static RectTransform KeyHint(Transform parent, string key, string action, Color color, Color? fill = null)
        {
            var r = Rect("KeyHint", parent);
            Row(r, 8);
            var box = Rect("Key", r);
            var pad = Row(box, 0, TextAnchor.MiddleCenter);
            pad.padding = new RectOffset(8, 8, 0, 0);
            var img = box.gameObject.AddComponent<Image>();
            img.color = fill ?? Color.black;   // an Outline copies the shape: the frame only shows around a solid fill
            var o = box.gameObject.AddComponent<Outline>();
            o.effectColor = color;
            o.effectDistance = new Vector2(2, -2);
            o.useGraphicAlpha = false;
            Text(box, key, 24, color, true);
            Text(r, action, 24, color, true);
            return r;
        }

        /// <summary>Soft CRT lines over everything else in <paramref name="parent"/>.</summary>
        public static GameObject Scanlines(RectTransform parent, float alpha = 0.16f)
        {
            var r = Stretch(Rect("Scanlines", parent), Vector2.zero, Vector2.one);
            r.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            r.gameObject.AddComponent<UiScanlines>().Alpha = alpha;
            return r.gameObject;
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
