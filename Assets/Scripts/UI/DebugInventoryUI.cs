using Heartbeat.Config;
using Heartbeat.Items;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// UI de depuración del inventario (provisional, no final): 6 recuadros con texto,
    /// aviso "E" al apuntar a un objeto y mensajes de estado. Tab abre y cierra sin
    /// pausar el juego; con el panel abierto el cursor se libera y los clics usan
    /// (izquierdo) o descartan (derecho) la casilla bajo el ratón.
    /// </summary>
    public class DebugInventoryUI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerPickup pickup;
        [SerializeField] private PlayerFinisher finisher;
        [SerializeField] private PlayerHack hack;
        [SerializeField] private ThirdPersonCamera cameraRig;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";

        private static readonly Color SlotIdleColor = new Color(0.05f, 0.05f, 0.06f, 0.85f);
        private static readonly Color SlotHoverColor = new Color(0.22f, 0.4f, 0.25f, 0.95f);
        private static readonly Color SlotBusyColor = new Color(0.45f, 0.35f, 0.1f, 0.95f);

        private InputAction inventoryAction;
        private Font font;

        private RectTransform panel;
        private RectTransform[] slotRects;
        private Image[] slotBackgrounds;
        private Text[] slotLabels;
        private GameObject promptRoot;
        private Text promptLabel;
        private Text statusLabel;

        private int hovered = -1;
        private string message;
        private float messageUntil;
        private bool isOpen;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            Build();
        }

        private void OnEnable()
        {
            if (inventory != null) inventory.Status += OnStatus;

            if (inputActions == null) return;

            InputActionMap map = inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
            if (map == null) return;

            inventoryAction = map.FindAction("Inventory", throwIfNotFound: false);
            if (inventoryAction == null)
            {
                Debug.LogWarning("[Inventario UI] Falta la acción 'Inventory' en el mapa Player.", this);
                return;
            }

            inventoryAction.Enable();
            inventoryAction.performed += OnInventoryPerformed;
        }

        private void OnDisable()
        {
            if (inventory != null) inventory.Status -= OnStatus;

            if (inventoryAction != null)
            {
                inventoryAction.performed -= OnInventoryPerformed;
                inventoryAction.Disable();
            }
            inventoryAction = null;
        }

        private void OnInventoryPerformed(InputAction.CallbackContext context)
        {
            Toggle();
        }

        private void OnStatus(string text)
        {
            message = text;
            messageUntil = Time.time + 3f;
        }

        public void Toggle()
        {
            SetOpen(!isOpen);
        }

        public void SetOpen(bool open)
        {
            isOpen = open;
            if (panel != null) panel.gameObject.SetActive(isOpen);

            if (!isOpen)
            {
                hovered = -1;
                if (inventory != null) inventory.HoveredSlot = -1;
            }

            if (cameraRig != null)
            {
                cameraRig.CursorCaptureEnabled = !isOpen;
                cameraRig.SetCursorLocked(!isOpen);
            }

            if (logEvents) Debug.Log(isOpen ? "[Inventario] Abierto (el juego sigue corriendo)." : "[Inventario] Cerrado.");
        }

        private void Update()
        {
            if (isOpen) HandlePointer();
            RefreshSlots();
            RefreshPrompt();
            RefreshStatus();
        }

        private void HandlePointer()
        {
            if (inventory == null || Mouse.current == null)
            {
                hovered = -1;
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            hovered = -1;

            if (slotRects != null)
            {
                for (int i = 0; i < slotRects.Length; i++)
                {
                    if (!RectTransformUtility.RectangleContainsScreenPoint(slotRects[i], mousePosition, null)) continue;
                    hovered = i;
                    break;
                }
            }

            inventory.HoveredSlot = hovered;
            if (hovered < 0) return;

            if (Mouse.current.leftButton.wasPressedThisFrame) inventory.Use(hovered);
            else if (Mouse.current.rightButton.wasPressedThisFrame) inventory.Discard(hovered);
        }

        private void RefreshSlots()
        {
            if (slotLabels == null || inventory == null) return;

            for (int i = 0; i < slotLabels.Length; i++)
            {
                InventorySlot slot = inventory.GetSlot(i);

                string label = slot.IsEmpty
                    ? $"{i + 1}. —"
                    : (slot.Amount > 1
                        ? $"{i + 1}. {ItemTypeUtil.DisplayName(slot.Type)} x{slot.Amount}"
                        : $"{i + 1}. {ItemTypeUtil.DisplayName(slot.Type)}");

                slotLabels[i].text = label;

                if (slotBackgrounds[i] == null) continue;

                bool busySlot = inventory.IsBusy && hovered == i;
                slotBackgrounds[i].color = busySlot
                    ? SlotBusyColor
                    : (hovered == i ? SlotHoverColor : SlotIdleColor);
            }
        }

        private void RefreshPrompt()
        {
            if (promptRoot == null) return;

            string text = null;

            if (!isOpen)
            {
                if (finisher != null && finisher.IsBusy)
                {
                    text = $"Rematando... {finisher.ActionProgress01 * 100f:F0} %";
                }
                else if (finisher != null && finisher.HasTarget)
                {
                    text = finisher.HasSpike
                        ? "[E] Rematar con punzón · [Clic] Pisotón"
                        : "[Clic] Pisotón (sin punzones)";
                }
                else if (hack != null && !hack.IsHacking && hack.HasTarget)
                {
                    text = hack.Prompt;
                }
                else if (pickup != null && pickup.CurrentTarget != null)
                {
                    text = $"[E] Recoger {pickup.CurrentTarget.DisplayName} x{pickup.CurrentTarget.Amount}";
                }
            }

            bool show = !string.IsNullOrEmpty(text);
            promptRoot.SetActive(show);

            if (show && promptLabel != null) promptLabel.text = text;
        }

        private void RefreshStatus()
        {
            if (statusLabel == null || inventory == null) return;

            if (inventory.IsBusy)
            {
                statusLabel.text = $"Acción en curso... {inventory.ActionProgress01 * 100f:F0} %";
                return;
            }

            statusLabel.text = Time.time < messageUntil ? message : string.Empty;
        }

        private void Build()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            int slotCount = config != null ? Mathf.Max(1, config.InventorySlots) : 6;

            panel = CreateRect("InventarioDebug", transform, new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(1000f, 110f));
            panel.pivot = new Vector2(0.5f, 0f);

            slotRects = new RectTransform[slotCount];
            slotBackgrounds = new Image[slotCount];
            slotLabels = new Text[slotCount];

            const float slotWidth = 150f;
            const float slotHeight = 96f;
            const float spacing = 8f;
            float totalWidth = slotCount * slotWidth + (slotCount - 1) * spacing;

            for (int i = 0; i < slotCount; i++)
            {
                float x = -totalWidth * 0.5f + slotWidth * 0.5f + i * (slotWidth + spacing);

                RectTransform slot = CreateRect($"Casilla_{i + 1}", panel, new Vector2(0.5f, 0.5f), new Vector2(x, 0f), new Vector2(slotWidth, slotHeight));
                Image background = slot.gameObject.AddComponent<Image>();
                background.color = SlotIdleColor;
                background.raycastTarget = false;

                slotRects[i] = slot;
                slotBackgrounds[i] = background;
                RectTransform labelRect = CreateStretchedRect("Texto", slot, new Vector4(8f, 8f, 8f, 8f));
                slotLabels[i] = AddText(labelRect, 16, TextAnchor.UpperLeft, Color.white);
            }

            RectTransform prompt = CreateRect("AvisoRecoger", transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(440f, 44f));
            Image promptBackground = prompt.gameObject.AddComponent<Image>();
            promptBackground.color = new Color(0f, 0f, 0f, 0.6f);
            promptBackground.raycastTarget = false;
            promptLabel = AddText(CreateStretchedRect("Texto", prompt, new Vector4(6f, 6f, 4f, 4f)), 22, TextAnchor.MiddleCenter, Color.white);
            promptRoot = prompt.gameObject;
            promptRoot.SetActive(false);

            RectTransform statusRect = CreateRect("EstadoInventario", transform, new Vector2(0.5f, 0f), new Vector2(0f, 134f), new Vector2(1000f, 28f));
            statusRect.pivot = new Vector2(0.5f, 0f);
            statusLabel = AddText(statusRect, 17, TextAnchor.MiddleCenter, new Color(1f, 0.95f, 0.6f));

            panel.gameObject.SetActive(false);
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

        private Text CreateText2Unused()
        {
            return null;
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

        private Text CreateText(string name, RectTransform parent, int fontSize, TextAnchor alignment, Color color, Vector4 padding)
        {
            RectTransform rect = CreateStretchedRect(name, parent, padding);
            return AddText(rect, fontSize, alignment, color);
        }
    }
}
