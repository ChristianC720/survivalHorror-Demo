using Heartbeat.Combat;
using Heartbeat.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>Combat feedback on the reticle (D-163): tints over targets, and a four-tick hit marker for hits, weak points and kills.</summary>
    public sealed class ReticleFeedback : MonoBehaviour
    {
        private const float MarkerSeconds = 0.18f, Range = 60f;

        private Graphic[] parts;
        private Color[] baseColors;
        private readonly Image[] ticks = new Image[4];
        private RectTransform marker;
        private float markerUntil;
        private int mask;

        private void Awake()
        {
            parts = GetComponentsInChildren<Graphic>(true);
            baseColors = new Color[parts.Length];
            for (int i = 0; i < parts.Length; i++) baseColors[i] = parts[i].color;
            mask = LayerMask.GetMask("Environment", "Hitbox");   // same targets as the Revolver

            var go = new GameObject("HitMarker", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            marker = (RectTransform)go.transform;
            for (int i = 0; i < ticks.Length; i++)
            {
                var tick = new GameObject("Tick", typeof(RectTransform), typeof(Image));
                tick.transform.SetParent(marker, false);
                var rt = (RectTransform)tick.transform;
                rt.sizeDelta = new Vector2(14f, 3f);
                float angle = 45f + 90f * i;
                rt.localRotation = Quaternion.Euler(0f, 0f, angle);
                rt.anchoredPosition = Quaternion.Euler(0f, 0f, angle) * Vector3.right * 18f;
                ticks[i] = tick.GetComponent<Image>();
                ticks[i].raycastTarget = false;
                ticks[i].enabled = false;
            }
        }

        private void OnEnable() => GameEvents.ShotLanded += OnShot;
        private void OnDisable() => GameEvents.ShotLanded -= OnShot;

        private void OnShot(bool weakPoint, bool killed)
        {
            Color color = killed ? UiKit.Red : weakPoint ? UiKit.Amber : UiKit.Text1;
            marker.localScale = Vector3.one * (killed ? 1.5f : 1f);
            foreach (var t in ticks) { t.color = color; t.enabled = true; }
            markerUntil = Time.unscaledTime + MarkerSeconds;
        }

        private void Update()
        {
            if (Time.unscaledTime >= markerUntil) foreach (var t in ticks) t.enabled = false;
            var cam = Camera.main;
            if (cam == null) return;

            Color? tint = null;
            if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out var hit, Range, mask, QueryTriggerInteraction.Ignore))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (hit.collider.GetComponent<WeakPoint>() != null) tint = UiKit.Amber;
                else if (damageable != null && damageable.IsAlive) tint = UiKit.Red;
            }
            for (int i = 0; i < parts.Length; i++) parts[i].color = tint ?? baseColors[i];
        }
    }
}
