using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// The look of every button: dark row when idle; lit row, dark text and a ">" marker when focused, so focus never
    /// depends on colour alone (regla visual 1). The mouse moves the focus too. Pinned = the current tab.
    /// </summary>
    public sealed class UiFocusStyle : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerMoveHandler
    {
        private Image bg;
        private Color color;
        private TMP_Text marker;
        private Selectable selectable;
        private bool focused, pinned, lastInteractable;

        public void Init(Image image, Color c, TMP_Text mark, Selectable s)
        {
            bg = image;
            color = c;
            marker = mark;
            selectable = s;
            lastInteractable = s.interactable;
            Apply();
        }

        public void SetColor(Color c) { color = c; Apply(); }
        public void SetPinned(bool on) { pinned = on; Apply(); }

        public void OnSelect(BaseEventData _) { focused = true; Apply(); }
        public void OnDeselect(BaseEventData _) { focused = false; Apply(); }

        // On move, not on enter: a resting cursor must not steal the focus from the keyboard when a screen appears under it.
        public void OnPointerMove(PointerEventData e)
        {
            if (!focused && e.delta != Vector2.zero && selectable.interactable && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void Update()
        {
            if (selectable.interactable == lastInteractable) return;
            lastInteractable = selectable.interactable;
            Apply();
        }

        /// <summary>Text added after Init (slot columns) takes the state right away.</summary>
        public void Apply()
        {
            bool enabled = selectable.interactable;
            bool lit = (focused || pinned) && enabled;
            bg.color = lit ? color : new Color(color.r * 0.2f, color.g * 0.2f, color.b * 0.2f, enabled ? 0.9f : 0.4f);
            foreach (var i in GetComponentsInChildren<Image>(true))   // icons
                if (i != bg) i.color = lit ? UiKit.Ink : enabled ? color : new Color(color.r, color.g, color.b, 0.45f);
            foreach (var t in GetComponentsInChildren<TMP_Text>(true))
            {
                if (t == marker) { t.enabled = focused && enabled; t.color = UiKit.Ink; continue; }
                t.color = lit ? UiKit.Ink : enabled ? color : new Color(color.r, color.g, color.b, 0.45f);
                t.fontStyle = lit ? FontStyles.Bold : FontStyles.Normal;   // dark text on a lit row reads thin without it
            }
        }
    }
}
