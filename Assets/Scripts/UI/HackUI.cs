using System.Collections.Generic;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Ventana de hackeo (provisional): muestra la secuencia de flechas del panel, marca la
    /// flecha que toca, resalta el fallo y recuerda que E abandona. No pausa el juego.
    /// </summary>
    public class HackUI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerHack hack;

        [Header("Depuración")]
        [SerializeField] private bool logEvents;

        private const float SlotWidth = 46f;
        private const float SlotHeight = 46f;
        private const float SlotSpacing = 8f;
        private const int MaxGlyphs = 12;

        private static readonly Color SlotIdleColor = new Color(0.08f, 0.08f, 0.09f, 0.85f);
        private static readonly Color SlotDoneColor = new Color(0.18f, 0.45f, 0.22f, 0.9f);
        private static readonly Color SlotCurrentColor = new Color(0.55f, 0.45f, 0.1f, 0.95f);
        private static readonly Color SlotErrorColor = new Color(0.55f, 0.12f, 0.12f, 0.95f);

        private Font font;
        private string[] glyphs;

        private RectTransform root;
        private Text title;
        private RectTransform slotRow;
        private Text hint;

        private readonly List<RectTransform> slotRects = new List<RectTransform>();
        private readonly List<Image> slotBackgrounds = new List<Image>();
        private readonly List<Text> slotLabels = new List<Text>();

        private int laidOutCount = -1;

        private void Awake()
        {
            Build();
        }

        private void Update()
        {
            if (root == null) return;

            bool active = hack != null && hack.IsHacking;
            if (root.gameObject.activeSelf != active) root.gameObject.SetActive(active);
            if (!active) return;

            LayoutSlots(hack.Sequence.Count);
            Refresh();
        }

        private void Refresh()
        {
            IReadOnlyList<HackArrow> sequence = hack.Sequence;
            int index = hack.SequenceIndex;
            bool error = hack.IsWrongFlash;

            if (title != null) title.text = $"HACKEANDO · {hack.ActivePanelName.ToUpperInvariant()}";

            for (int i = 0; i < slotBackgrounds.Count; i++)
            {
                if (slotBackgrounds[i] == null) continue;

                bool done = i < index;
                bool current = i == index && !error;

                slotBackgrounds[i].color = error
                    ? SlotErrorColor
                    : (done ? SlotDoneColor : (current ? SlotCurrentColor : SlotIdleColor));

                if (slotLabels[i] != null) slotLabels[i].color = done ? Color.white : new Color(0.85f, 0.85f, 0.85f);
            }

            if (hint != null)
            {
                hint.text = error
                    ? "Fallo: la secuencia vuelve a empezar"
                    : $"{index}/{sequence.Count} · WASD o flechas · [E] Abandonar";
                hint.color = error ? new Color(1f, 0.45f, 0.4f) : new Color(0.85f, 0.85f, 0.85f);
            }
        }

        private void LayoutSlots(int count)
        {
            if (count <= 0) return;

            while (slotLabels.Count < count && slotLabels.Count < MaxGlyphs) AddSlot();
            if (count == laidOutCount) return;

            laidOutCount = count;

            for (int i = 0; i < slotLabels.Count; i++)
            {
                bool used = i < count;
                slotRects[i].gameObject.SetActive(used);
            }

            int visible = Mathf.Min(count, slotLabels.Count);
            float totalWidth = visible * SlotWidth + (visible - 1) * SlotSpacing;

            for (int i = 0; i < visible; i++)
            {
                float x = -totalWidth * 0.5f + SlotWidth * 0.5f + i * (SlotWidth + SlotSpacing);
                slotRects[i].anchoredPosition = new Vector2(x, 0f);
            }

            IReadOnlyList<HackArrow> sequence = hack.Sequence;
            for (int i = 0; i < slotLabels.Count && i < sequence.Count; i++)
            {
                string glyph = glyphs[(int)sequence[i]];
                if (slotLabels[i] != null) slotLabels[i].text = glyph;
            }
        }

        private void AddSlot()
        {
            int index = slotRects.Count;

            RectTransform slot = CreateRect($"Flecha_{index + 1}", slotRow, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(SlotWidth, SlotHeight));
            Image background = slot.gameObject.AddComponent<Image>();
            background.color = SlotIdleColor;
            background.raycastTarget = false;

            Text label = AddText(CreateStretchedRect("Texto", slot, new Vector4(2f, 2f, 2f, 2f)), 26, TextAnchor.MiddleCenter, Color.white);

            slotRects.Add(slot);
            slotBackgrounds.Add(background);
            slotLabels.Add(label);
        }

        private void Build()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            glyphs = ResolveGlyphs();

            root = CreateRect("Hackeo", transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(560f, 150f));

            Image background = root.gameObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.72f);
            background.raycastTarget = false;

            title = AddText(CreateStretchedRect("Titulo", root, new Vector4(12f, 12f, 10f, 104f)), 20, TextAnchor.MiddleCenter, new Color(1f, 0.9f, 0.55f));

            slotRow = CreateRect("Flechas", root, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(500f, SlotHeight));

            hint = AddText(CreateStretchedRect("Ayuda", root, new Vector4(12f, 12f, 96f, 10f)), 17, TextAnchor.MiddleCenter, new Color(0.85f, 0.85f, 0.85f));

            root.gameObject.SetActive(false);
        }

        private string[] ResolveGlyphs()
        {
            if (font != null && font.HasCharacter('↑') && font.HasCharacter('↓') && font.HasCharacter('←') && font.HasCharacter('→'))
            {
                return new[] { "↑", "↓", "←", "→" };
            }

            // Si la fuente no trae flechas, se muestran las teclas: se introduce igual con WASD.
            return new[] { "W", "S", "A", "D" };
        }

        private RectTransform CreateRect(string name, Transform parent, Vector2 anchor, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            return rect;
        }

        private RectTransform CreateStretchedRect(string name, RectTransform parent, Vector4 padding)
        {
            RectTransform rect = CreateRect(name, parent, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding.x, padding.w);
            rect.offsetMax = new Vector2(-padding.y, -padding.z);

            return rect;
        }

        private Text AddText(RectTransform rect, int fontSize, TextAnchor alignment, Color color)
        {
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            return text;
        }
    }
}
