using System.Collections;
using Heartbeat.AI;
using Heartbeat.Config;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Muerte y reaparición. Al morir tapa la pantalla en negro con el placeholder de la
    /// cinemática de reconstrucción (Espacio la salta) y recarga la escena tal cual está
    /// montada: enemigos, objetos, llave, puertas y sellados vuelven a su estado inicial,
    /// sin generación procedural, y el jugador reaparece en FilaCapsulas con la salud en
    /// Bien, la resistencia al 100 %, el inventario vacío y el revólver descargado.
    /// El contador de muertes y el código de sujeto sólo se ven en modo debug (F1).
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    public class DeathScreenUI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private PlayerHealth health;

        [Header("Depuración")]
        [SerializeField] private bool logEvents = true;

        private const string TitleText = "RECONSTRUCCIÓN";
        private const string BodyText = "La Mother te vuelve a tejer.\nLos efectos personales esperan en la bandeja.";
        private const string SkipText = "[Espacio] Saltar";

        private static readonly Color OverlayColor = new Color(0f, 0f, 0f, 1f);
        private static readonly Color TitleColor = new Color(0.92f, 0.95f, 1f, 1f);
        private static readonly Color BodyColor = new Color(0.62f, 0.68f, 0.75f, 1f);
        private static readonly Color DebugColor = new Color(1f, 0.95f, 0.6f, 1f);

        private CanvasGroup overlay;
        private GameObject overlayRoot;
        private GameObject cinematicRoot;
        private GameObject debugRoot;
        private Text debugLabel;

        private Coroutine routine;
        private bool sequenceRunning;

        /// <summary>Hay una muerte en curso (o el negro de la reaparición todavía no se ha retirado).</summary>
        public bool IsRunning => sequenceRunning;

        private void Awake()
        {
            Build();

            if (PlayerRespawn.IsRespawning)
            {
                // La escena acaba de recargarse por una muerte: encadena en negro sin parpadeo.
                SetCinematic(true);
                SetOverlay(true);
                SetAlpha(1f);
                return;
            }

            PlayerRespawn.ResetSession();
            SetCinematic(false);
            SetOverlay(false);
            SetAlpha(0f);
        }

        private void Start()
        {
            if (!PlayerRespawn.IsRespawning) return;

            StopRoutine();
            routine = StartCoroutine(RespawnRoutine());
        }

        private void OnEnable()
        {
            if (health != null) health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (health != null) health.Died -= OnDied;
        }

        private void OnDied()
        {
            StartDeath();
        }

        /// <summary>
        /// Arranca la pantalla de muerte. La usan tanto la muerte real como "Reiniciar"
        /// del menú de pausa, que cuenta como muerte.
        /// </summary>
        public void StartDeath()
        {
            if (sequenceRunning) return;

            sequenceRunning = true;
            StopRoutine();
            routine = StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            SetCinematic(false);
            SetOverlay(true);
            SetAlpha(0f);

            yield return Fade(0f, 1f, DurationOf(config != null ? config.DeathFadeInDuration : 0.35f, 0.35f));

            SetCinematic(true);

            float hold = DurationOf(config != null ? config.ReconstructionDuration : 2.5f, 2.5f);
            float elapsed = 0f;
            bool skipped = false;

            while (elapsed < hold)
            {
                if (SkipPressed())
                {
                    skipped = true;
                    break;
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (logEvents)
            {
                Debug.Log(skipped
                    ? "[Muerte] Reconstrucción saltada con Espacio; se reaparece en FilaCapsulas."
                    : "[Muerte] Reconstrucción terminada; se reaparece en FilaCapsulas.");
            }

            SetCinematic(false);

            // Se cuenta la muerte y se recarga: el nivel entero vuelve a su estado montado.
            PlayerRespawn.RegisterDeath();
            ReloadLevel();
        }

        private IEnumerator RespawnRoutine()
        {
            sequenceRunning = true;
            SetCinematic(false);
            SetOverlay(true);
            SetAlpha(1f);

            PlayerRespawn.ConsumeRespawn();

            yield return Fade(1f, 0f, DurationOf(config != null ? config.RespawnFadeOutDuration : 0.8f, 0.8f));

            SetOverlay(false);
            sequenceRunning = false;

            if (logEvents)
            {
                int start = config != null ? config.SubjectCodeStart : 7;
                Debug.Log($"[Muerte] Reaparecido en FilaCapsulas. Muertes: {PlayerRespawn.Deaths}, "
                    + $"sujeto {PlayerRespawn.SubjectCode(start)}.");
            }
        }

        /// <summary>
        /// Recarga la escena activa. Es el reinicio completo del nivel: no hay generación
        /// procedural, todo vuelve a como está montado en VS_Piso1.
        /// </summary>
        private void ReloadLevel()
        {
            Scene active = SceneManager.GetActiveScene();

            if (Application.CanStreamedLevelBeLoaded(active.name))
            {
                SceneManager.LoadScene(active.name);
                return;
            }

#if UNITY_EDITOR
            // Sin la escena en Build Settings (Play desde el editor) se carga por ruta.
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                active.path, new LoadSceneParameters(LoadSceneMode.Single));
#else
            Debug.LogError($"[Muerte] La escena '{active.name}' no está en Build Settings: no se puede reiniciar el nivel.", this);
#endif
        }

        private void Update()
        {
            RefreshDebugCounters();
        }

        // ---------------------------------------------------------------- contadores (F1)

        private void RefreshDebugCounters()
        {
            if (debugLabel == null || config == null) return;

            bool show = EnemyDirector.DebugEnabled;
            if (debugLabel.gameObject.activeSelf != show) debugLabel.gameObject.SetActive(show);
            if (!show) return;

            debugLabel.text = $"Muertes: {PlayerRespawn.Deaths}    Sujeto: {PlayerRespawn.SubjectCode(config.SubjectCodeStart)}";
        }

        // ------------------------------------------------------------------------ visual

        private static bool SkipPressed()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
        }

        private static float DurationOf(float value, float fallback)
        {
            return value > 0f ? value : fallback;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                SetAlpha(to);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration)));
                yield return null;
            }

            SetAlpha(to);
        }

        private void StopRoutine()
        {
            if (routine == null) return;
            StopCoroutine(routine);
            routine = null;
        }

        private void SetOverlay(bool visible)
        {
            if (overlayRoot == null) return;
            if (overlayRoot.activeSelf != visible) overlayRoot.SetActive(visible);
        }

        private void SetCinematic(bool visible)
        {
            if (cinematicRoot == null) return;
            if (cinematicRoot.activeSelf != visible) cinematicRoot.SetActive(visible);
        }

        private void SetAlpha(float alpha)
        {
            if (overlay == null) return;
            overlay.alpha = Mathf.Clamp01(alpha);
            overlay.blocksRaycasts = alpha > 0.001f;
        }

        // -------------------------------------------------------------------------- UI

        private void Build()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            RectTransform root = CreateRect("PantallaMuerte", transform, Vector2.zero, Vector2.zero);
            Stretch(root);
            overlayRoot = root.gameObject;

            Image black = root.gameObject.AddComponent<Image>();
            black.color = OverlayColor;
            black.raycastTarget = true;

            overlay = root.gameObject.AddComponent<CanvasGroup>();
            overlay.interactable = false;
            overlay.blocksRaycasts = false;

            RectTransform cinematic = CreateRect("Cinematica", root, new Vector2(0.5f, 0.5f), Vector2.zero);
            Stretch(cinematic);
            cinematicRoot = cinematic.gameObject;

            Text title = AddText(CreateRect("Titulo", cinematic, new Vector2(0.5f, 0.5f), new Vector2(0f, 26f), new Vector2(1200f, 90f)),
                54, TextAnchor.MiddleCenter, TitleColor, font);
            title.text = TitleText;

            Text body = AddText(CreateRect("Texto", cinematic, new Vector2(0.5f, 0.5f), new Vector2(0f, -50f), new Vector2(1200f, 80f)),
                22, TextAnchor.MiddleCenter, BodyColor, font);
            body.text = BodyText;

            Text skip = AddText(CreateRect("Saltar", cinematic, new Vector2(0.5f, 0f), new Vector2(0f, 90f), new Vector2(600f, 40f)),
                20, TextAnchor.MiddleCenter, BodyColor, font);
            skip.text = SkipText;

            // Los contadores se dibujan por encima del negro, arriba a la derecha para no
            // pisar el texto de depuración de la IA, y sólo en modo debug.
            Text debug = AddText(CreateRect("ContadoresDebug", transform, new Vector2(1f, 1f), new Vector2(-28f, -28f), new Vector2(720f, 34f)),
                20, TextAnchor.MiddleRight, DebugColor, font);
            debug.rectTransform.pivot = new Vector2(1f, 1f);
            debugRoot = debug.gameObject;
            debugLabel = debug;
            debugRoot.SetActive(false);
        }

        private static RectTransform CreateRect(string name, Transform parent, Vector2 anchor, Vector2 position)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = Vector2.zero;
            return rect;
        }

        private static RectTransform CreateRect(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            RectTransform rect = CreateRect(name, parent, anchor, position);
            rect.sizeDelta = size;
            return rect;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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
