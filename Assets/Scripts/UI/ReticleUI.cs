using Heartbeat.Config;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Retícula de punto: un punto central fijo y cuatro marcas que se separan
    /// según la dispersión de <see cref="PlayerAim"/>. Sólo se muestra al apuntar.
    /// Se coloca en el Canvas del HUD y busca al jugador por sí sola.
    /// </summary>
    public class ReticleUI : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private RectTransform dot;
        [SerializeField] private RectTransform tickUp;
        [SerializeField] private RectTransform tickDown;
        [SerializeField] private RectTransform tickLeft;
        [SerializeField] private RectTransform tickRight;

        private PlayerAim aim;

        private void Awake()
        {
            if (group == null) group = GetComponent<CanvasGroup>();
            SetVisible(false);
            ApplyDot();
        }

        private void Update()
        {
            if (config == null) return;

            if (aim == null)
            {
                aim = Object.FindAnyObjectByType<PlayerAim>();
                if (aim == null) return;
            }

            bool visible = aim.IsAiming;
            SetVisible(visible);
            if (!visible) return;

            float radius = Mathf.Lerp(config.ReticleClosedRadius, config.ReticleOpenRadius, aim.Spread01);
            ApplyRadius(radius);
        }

        private void ApplyDot()
        {
            if (config == null || dot == null) return;

            Vector2 size = new Vector2(config.ReticleDotSize, config.ReticleDotSize);
            dot.sizeDelta = size;
            dot.anchoredPosition = Vector2.zero;
        }

        private void ApplyRadius(float radius)
        {
            if (tickUp != null) tickUp.anchoredPosition = new Vector2(0f, radius);
            if (tickDown != null) tickDown.anchoredPosition = new Vector2(0f, -radius);
            if (tickLeft != null) tickLeft.anchoredPosition = new Vector2(-radius, 0f);
            if (tickRight != null) tickRight.anchoredPosition = new Vector2(radius, 0f);
        }

        private void SetVisible(bool visible)
        {
            if (group == null) return;

            group.alpha = visible ? 1f : 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        }
    }
}
