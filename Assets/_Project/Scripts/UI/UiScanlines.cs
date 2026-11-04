using Heartbeat.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>Soft CRT lines drawn over the rest of its parent. Lower with "reduce CRT effects" (D-106), but still there.</summary>
    [RequireComponent(typeof(RawImage))]
    public sealed class UiScanlines : MonoBehaviour
    {
        private const float Pitch = 8f;
        private static Texture2D tile;

        public float Alpha { get; set; } = 0.16f;

        private RawImage image;

        private void Awake()
        {
            image = GetComponent<RawImage>();
            image.raycastTarget = false;
            if (tile == null)
            {
                tile = new Texture2D(1, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Repeat };
                tile.SetPixels(new[] { Color.clear, Color.black });
                tile.Apply();
            }
            image.texture = tile;
        }

        private void OnEnable() { GameEvents.OptionsChanged += Refresh; Refresh(); }
        private void OnDisable() => GameEvents.OptionsChanged -= Refresh;
        private void Start() { transform.SetAsLastSibling(); Fit(); }
        private void OnRectTransformDimensionsChange() => Fit();

        private void Refresh()
        {
            bool reduced = Services.TryGet(out Save.SaveService saves) && saves.LoadGlobal().reduceCrt;
            if (image != null) image.color = new Color(1f, 1f, 1f, reduced ? Alpha * 0.4f : Alpha);
        }

        private void Fit()
        {
            if (image != null) image.uvRect = new Rect(0f, 0f, 1f, ((RectTransform)transform).rect.height / Pitch);
        }
    }
}
