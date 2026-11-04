using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>Small facility screen in the world: the diegetic reason why a door does not open.</summary>
    public static class StatusPanel
    {
        private const float Scale = 0.001f;

        /// <param name="front">Local position of the face looking along the parent's +Z.</param>
        /// <param name="back">Also a face on the other side, mirrored in Z.</param>
        /// <returns>The holder, to be destroyed when the reason is gone.</returns>
        public static GameObject Create(Transform parent, string line1, string line2, Color color, Vector3 front, bool back)
        {
            var holder = new GameObject("StatusPanel");
            holder.transform.SetParent(parent, false);
            Face(holder.transform, front, Quaternion.Euler(0f, 180f, 0f), line1, line2, color);
            if (back) Face(holder.transform, new Vector3(front.x, front.y, -front.z), Quaternion.identity, line1, line2, color);
            return holder;
        }

        private static void Face(Transform parent, Vector3 position, Quaternion rotation, string line1, string line2, Color color)
        {
            var r = UiKit.Rect("Face", parent);
            var canvas = r.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            r.sizeDelta = new Vector2(400f, 170f);
            r.localScale = Vector3.one * Scale;
            r.localPosition = position;
            r.localRotation = rotation;
            r.gameObject.AddComponent<Image>().color = UiKit.BgFacility;
            var o = r.gameObject.AddComponent<Outline>();
            o.effectColor = color;
            o.effectDistance = new Vector2(4, -4);
            o.useGraphicAlpha = false;
            var column = UiKit.Column(r, 4, 12);
            column.childAlignment = TextAnchor.MiddleCenter;
            column.childForceExpandWidth = true;
            Line(r, line1, 56, color);
            Line(r, line2, 40, color);
        }

        private static void Line(Transform parent, string text, float size, Color color)
        {
            var t = UiKit.Text(parent, text, size, color, true);
            t.alignment = TextAlignmentOptions.Center;
            t.textWrappingMode = TextWrappingModes.NoWrap;
            t.gameObject.AddComponent<LayoutElement>().minHeight = size * 1.05f;   // VT323 lays out shorter than it draws
        }
    }
}
