using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Modal confirmation (UI y Menús, D-96): message + Cancel / Confirm, Cancel selected by default.
    /// Built in code on top of any canvas: <c>ConfirmDialog.Show(canvasTransform, text, onConfirm)</c>.
    /// </summary>
    public sealed class ConfirmDialog : MonoBehaviour
    {
        private Action onConfirm, onCancel;

        public static ConfirmDialog Show(Transform canvas, string message, Action onConfirm, Action onCancel = null, Color? accent = null)
        {
            var root = UiKit.Stretch(UiKit.Rect("ConfirmDialog", canvas), Vector2.zero, Vector2.one);
            root.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);   // blocks clicks behind
            var dlg = root.gameObject.AddComponent<ConfirmDialog>();
            dlg.onConfirm = onConfirm;
            dlg.onCancel = onCancel;

            Color c = accent ?? UiKit.Teal;
            var box = UiKit.Rect("Box", root);
            box.anchorMin = box.anchorMax = box.pivot = new Vector2(0.5f, 0.5f);
            box.sizeDelta = new Vector2(900, 320);
            UiKit.Panel(box, new Color(0.01f, 0.05f, 0.05f, 0.97f), c * 0.6f);
            UiKit.Column(box, 18, 32);
            var text = UiKit.Text(box, message, 30, c);
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

            var row = UiKit.Rect("Buttons", box);
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 24;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandHeight = false;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 52;
            var cancel = UiKit.Button(row, Loc.Get("UI.CONFIRM.CANCEL"), 28, c, dlg.Cancel, 52);
            UiKit.Button(row, Loc.Get("UI.CONFIRM.OK"), 28, c, dlg.Confirm, 52);
            UiKit.Select(cancel);
            return dlg;
        }

        public void Cancel()
        {
            Destroy(gameObject);
            onCancel?.Invoke();
        }

        private void Confirm()
        {
            Destroy(gameObject);
            onConfirm?.Invoke();
        }
    }
}
