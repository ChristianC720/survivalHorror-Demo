using System.Collections.Generic;
using Heartbeat.Inventory;
using UnityEngine;

namespace Heartbeat.UI
{
    /// <summary>Line icons of the terminal, monochrome and tinted by the screen that shows them (Interfaz Visual). Drawn once, at first use.</summary>
    public static class UiIcons
    {
        private const int Size = 72, Stroke = 4;
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        private static readonly Dictionary<string, int[]> Lines = new Dictionary<string, int[]>
        {
            // x0,y0,x1,y1 per line, y down, as in the Figma icons.
            ["Heal"] = new[] { 10,26,62,26, 62,26,62,60, 62,60,10,60, 10,60,10,26, 26,26,26,16, 26,16,46,16, 46,16,46,26, 36,34,36,52, 27,43,45,43 },
            ["Ammo"] = new[] { 8,58,64,58, 14,58,14,26, 14,26,19,16, 19,16,24,26, 24,26,24,58, 31,58,31,26, 31,26,36,16, 36,16,41,26, 41,26,41,58, 48,58,48,26, 48,26,53,16, 53,16,58,26, 58,26,58,58 },
            ["Spike"] = new[] { 10,62,22,50, 22,50,16,44, 16,44,42,18, 42,18,54,30, 54,30,28,56, 28,56,22,50, 48,24,62,10, 26,34,38,46 },
            ["KeyMandatory"] = new[] { 8,16,64,16, 64,16,64,56, 64,56,8,56, 8,56,8,16, 8,26,64,26, 18,48,28,48, 28,48,28,41, 28,41,38,41, 38,41,38,34, 38,34,48,34 },
            ["KeyOptional"] = new[] { 14,22,58,22, 58,22,58,50, 58,50,14,50, 14,50,14,22, 14,31,58,31, 22,41,36,41 },
            ["Pulse"] = new[] { 8,36,22,36, 22,36,28,18, 28,18,38,56, 38,56,44,36, 44,36,64,36 },
        };

        public static Sprite For(ItemKind kind) => Get(kind.ToString());

        public static Sprite Get(string name)
        {
            if (Cache.TryGetValue(name, out var s) && s != null) return s;
            if (!Lines.TryGetValue(name, out var l)) return null;
            var pixels = new Color32[Size * Size];
            for (int i = 0; i + 3 < l.Length; i += 4) Line(pixels, l[i], l[i + 1], l[i + 2], l[i + 3]);
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            tex.SetPixels32(pixels);
            tex.Apply();
            s = Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), 100f);
            Cache[name] = s;
            return s;
        }

        private static void Line(Color32[] p, int x0, int y0, int x1, int y1)
        {
            int steps = Mathf.Max(Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0), 1);
            for (int i = 0; i <= steps; i++)
            {
                int x = x0 + (x1 - x0) * i / steps, y = y0 + (y1 - y0) * i / steps;
                for (int dx = -Stroke / 2; dx < Stroke / 2; dx++)
                    for (int dy = -Stroke / 2; dy < Stroke / 2; dy++)
                    {
                        int px = x + dx, py = Size - 1 - (y + dy);   // the drawings are y down, textures y up
                        if (px >= 0 && py >= 0 && px < Size && py < Size) p[py * Size + px] = new Color32(255, 255, 255, 255);
                    }
            }
        }
    }
}
