using Heartbeat.Combat;
using Heartbeat.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Combat feedback on the reticle (D-163): tints over targets, and a four-tick hit marker for hits, weak points and
    /// kills. The marker grows in, holds and shrinks out over <see cref="SystemsBalance.hitMarkerSeconds"/> (C-72).
    /// </summary>
    public sealed class ReticleFeedback : MonoBehaviour
    {
        private const float Range = 60f;

        private Graphic[] parts;
        private Color[] baseColors;
        private readonly Image[] ticks = new Image[4];
        private RectTransform marker;
        private float markerStart = float.NegativeInfinity, markerSize = 1f;
        private Color markerColor;
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
            markerColor = killed ? UiKit.Red : weakPoint ? UiKit.Amber : UiKit.Text1;
            markerSize = killed ? 1.5f : 1f;
            markerStart = Time.unscaledTime;
        }

        /// <summary>Marker scale and alpha at <paramref name="t"/> (0..1 of its life): grows in, holds, shrinks and fades out.</summary>
        public static void MarkerPose(float t, out float scale, out float alpha)
        {
            const float growEnd = 0.2f, shrinkStart = 0.6f;
            if (t < growEnd) { scale = Mathf.Lerp(0.5f, 1f, t / growEnd); alpha = 1f; }
            else if (t < shrinkStart) { scale = 1f; alpha = 1f; }
            else { float k = Mathf.Clamp01((t - shrinkStart) / (1f - shrinkStart)); scale = Mathf.Lerp(1f, 0.6f, k); alpha = 1f - k; }
        }

        private void Update()
        {
            float t = (Time.unscaledTime - markerStart) / SystemsBalance.Current.hitMarkerSeconds;
            bool show = t >= 0f && t < 1f;
            if (show)
            {
                MarkerPose(t, out float scale, out float alpha);
                marker.localScale = Vector3.one * (markerSize * scale);
                var c = markerColor; c.a *= alpha;
                foreach (var tick in ticks) tick.color = c;
            }
            foreach (var tick in ticks) tick.enabled = show;
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
