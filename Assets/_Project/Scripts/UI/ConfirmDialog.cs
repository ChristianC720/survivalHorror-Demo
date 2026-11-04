using System;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Modal confirmation (UI y Menús, D-96): message, the cost apart in amber, then Cancel (selected) and Confirm.
    /// Built in code on top of any canvas: <c>ConfirmDialog.Show(canvasTransform, text, onConfirm)</c>.
    /// </summary>
    public sealed class ConfirmDialog : MonoBehaviour
    {
        /// <summary>Screens with their own Esc look here so Esc only cancels the dialog.</summary>
        public static bool AnyOpen => open > 0;
        private static int open;

        private Action onConfirm, onCancel;

        public static ConfirmDialog Show(Transform canvas, string message, Action onConfirm, Action onCancel = null, Color? accent = null, string cost = null)
        {
            var root = UiKit.Stretch(UiKit.Rect("ConfirmDialog", canvas), Vector2.zero, Vector2.one);
            root.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);   // blocks clicks behind
            var dlg = root.gameObject.AddComponent<ConfirmDialog>();
            dlg.onConfirm = onConfirm;
            dlg.onCancel = onCancel;
            open++;

            Color c = accent ?? UiKit.Teal;
            var box = UiKit.Rect("Box", root);
            box.anchorMin = box.anchorMax = box.pivot = new Vector2(0.5f, 0.5f);
            box.sizeDelta = new Vector2(900, 0);
            UiKit.Crt(box, UiKit.BgFacility, c);
            UiKit.Column(box, 16, 32).childForceExpandWidth = true;
            box.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var text = UiKit.Text(box, message, 32, UiKit.Text1, false);
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.textWrappingMode = TextWrappingModes.Normal;
            if (!string.IsNullOrEmpty(cost))
            {
                var line = UiKit.Text(box, cost, 30, UiKit.Amber, true);
                line.textWrappingMode = TextWrappingModes.Normal;
            }

            var cancel = UiKit.Button(box, Loc.Get("UI.CONFIRM.CANCEL"), 30, c, dlg.Cancel, 56);
            UiKit.Button(box, Loc.Get("UI.CONFIRM.OK"), 30, c, dlg.Confirm, 56);
            UiKit.Select(cancel);
            return dlg;
        }

        private InputRouter router;

        private void Start()
        {
            if (Services.TryGet(out router)) router.Actions.UI.Cancel.performed += OnEsc;
        }

        private void OnEsc(UnityEngine.InputSystem.InputAction.CallbackContext _) => Cancel();

        private void OnDestroy()
        {
            open--;
            if (router != null) router.Actions.UI.Cancel.performed -= OnEsc;
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
