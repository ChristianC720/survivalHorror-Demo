using Heartbeat.Config;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Menú de pausa (Esc): congela el tiempo, libera el cursor y bloquea el mapa de
    /// acciones "Player". Ofrece "Reiniciar", que cuenta como muerte y encadena con la
    /// pantalla de reconstrucción, y "Salir". Sin EventSystem en el proyecto: las
    /// opciones se pulsan por rectángulo del ratón, igual que la UI de inventario.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private DeathScreenUI deathScreen;
        [SerializeField] private DebugInventoryUI inventoryUI;

        [Tooltip("Cámara OTS: se le devuelve el cursor al salir de la pausa.")]
        [SerializeField] private ThirdPersonCamera cameraRig;

        [SerializeField] private InputActionAsset inputActions;

        [Tooltip("Si hay una sesión de hackeo abierta, se aborta al pausar.")]
        [SerializeField] private PlayerHack hack;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string ActionMapName = "Player";
        private const string TitleText = "PAUSA";
        private const string HintText = "[Esc] Continuar";

        private static readonly Color DimColor = new Color(0f, 0f, 0f, 0.82f);
        private static readonly Color OptionIdleColor = new Color(0.12f, 0.13f, 0.15f, 0.95f);
        private static readonly Color OptionHoverColor = new Color(0.28f, 0.42f, 0.3f, 1f);

        private GameObject panel;
        private RectTransform[] optionRects;
        private Image[] optionBackgrounds;
        private int hovered = -1;

        private bool isPaused;

        public bool IsPaused => isPaused;

        private void Awake()
        {
            Build();
            panel.SetActive(false);
        }

        private void OnDisable()
        {
            // Nunca dejar el tiempo congelado si se desactiva el menú (recarga de escena incluida).
            if (!isPaused) return;

            isPaused = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        private void Update()
        {
            HandlePauseToggle();
            if (!isPaused) return;

            HandlePointer();
        }

        private void HandlePauseToggle()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

            // Durante la muerte/reaparición el menú no se abre: la pantalla ya manda.
            if (deathScreen != null && deathScreen.IsRunning) return;

            SetPaused(!isPaused);
        }

        private void SetPaused(bool paused)
        {
            if (isPaused == paused) return;

            isPaused = paused;
            panel.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
            AudioListener.pause = paused;

            InputActionMap map = FindPlayerMap();
            if (map != null)
            {
                if (paused) map.Disable();
                else map.Enable();
            }

            if (paused)
            {
                hovered = -1;
                if (inventoryUI != null && inventoryUI.IsOpen) inventoryUI.SetOpen(false);
                if (hack != null && hack.IsHacking) hack.Abort("pausa");
            }
            else
            {
                hovered = -1;
            }

            if (cameraRig != null)
            {
                cameraRig.CursorCaptureEnabled = !paused;
                cameraRig.SetCursorLocked(!paused);
            }

            if (logEvents) Debug.Log(paused ? "[Pausa] Menú abierto (tiempo detenido)." : "[Pausa] Menú cerrado.");
        }

        private void HandlePointer()
        {
            if (Mouse.current == null || optionRects == null)
            {
                hovered = -1;
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            hovered = -1;

            for (int i = 0; i < optionRects.Length; i++)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(optionRects[i], mousePosition, null)) continue;
                hovered = i;
                break;
            }

            RefreshOptions();

            if (hovered < 0 || !Mouse.current.leftButton.wasPressedThisFrame) return;

            if (hovered == 0) Restart();
            else Quit();
        }

        private void RefreshOptions()
        {
            if (optionBackgrounds == null) return;

            for (int i = 0; i < optionBackgrounds.Length; i++)
            {
                if (optionBackgrounds[i] == null) continue;
                optionBackgrounds[i].color = hovered == i ? OptionHoverColor : OptionIdleColor;
            }
        }

        /// <summary>"Reiniciar": cuenta como muerte y encadena con la pantalla de reconstrucción.</summary>
        private void Restart()
        {
            SetPaused(false);
            if (logEvents) Debug.Log("[Pausa] Reiniciar: cuenta como muerte; se reinicia el nivel.");

            if (deathScreen != null)
            {
                deathScreen.StartDeath();
                return;
            }

            Debug.LogError("[Pausa] Sin referencia a DeathScreenUI: no se puede reiniciar el nivel.", this);
        }

        /// <summary>"Salir": cierra el juego (o detiene el Play Mode en el editor).</summary>
        private void Quit()
        {
            if (logEvents) Debug.Log("[Pausa] Salir.");

            SetPaused(false);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private InputActionMap FindPlayerMap()
        {
            if (inputActions == null) return null;
            return inputActions.FindActionMap(ActionMapName, throwIfNotFound: false);
        }

        // -------------------------------------------------------------------------- UI

        private void Build()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            panel = new GameObject("MenuPausa", typeof(RectTransform));
            RectTransform root = panel.GetComponent<RectTransform>();
            root.SetParent(transform, false);
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.pivot = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = Vector2.zero;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            Image dim = panel.AddComponent<Image>();
            dim.color = DimColor;
            dim.raycastTarget = true;

            Text title = AddText(CreateRect("Titulo", root, 400f, 60f), 48, TextAnchor.MiddleCenter, Color.white, font);
            title.text = TitleText;
            title.rectTransform.anchoredPosition = new Vector2(0f, 120f);

            optionRects = new RectTransform[2];
            optionBackgrounds = new Image[2];
            string[] labels = { "Reiniciar", "Salir" };

            for (int i = 0; i < labels.Length; i++)
            {
                RectTransform option = CreateRect($"Opcion_{labels[i]}", root, 420f, 64f);
                option.anchoredPosition = new Vector2(0f, 10f - i * 84f);

                Image background = option.gameObject.AddComponent<Image>();
                background.color = OptionIdleColor;
                background.raycastTarget = false;

                Text label = AddText(CreateStretchedRect("Texto", option, new Vector4(8f, 8f, 4f, 4f)),
                    28, TextAnchor.MiddleCenter, Color.white, font);
                label.text = labels[i];

                optionRects[i] = option;
                optionBackgrounds[i] = background;
            }

            Text hint = AddText(CreateRect("Ayuda", root, 400f, 40f), 20, TextAnchor.MiddleCenter,
                new Color(0.7f, 0.72f, 0.75f), font);
            hint.text = HintText;
            hint.rectTransform.anchoredPosition = new Vector2(0f, -150f);
        }

        private static RectTransform CreateRect(string name, RectTransform parent, float width, float height)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static RectTransform CreateStretchedRect(string name, RectTransform parent, Vector4 padding)
        {
            RectTransform rect = CreateRect(name, parent, 0f, 0f);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding.x, padding.w);
            rect.offsetMax = new Vector2(-padding.y, -padding.z);
            return rect;
        }

        private static Text AddText(RectTransform rect, int fontSize, TextAnchor alignment, Color color, Font font)
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
