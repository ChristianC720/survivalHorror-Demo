using Heartbeat.Core;
using Heartbeat.Save;
using TMPro;
using UnityEngine;

namespace Heartbeat.UI
{
    /// <summary>
    /// Font assignment (D-126, Accesibilidad): data texts use VT323, everything else IBM Plex Sans; the dyslexia
    /// option swaps both for OpenDyslexic in every open scene (GameEvents.OptionsChanged).
    /// </summary>
    public static class UiFonts
    {
        private static UiFontSet set;
        private static bool dyslexia;
        private static bool listening;

        private static UiFontSet Set => set != null ? set : (set = Resources.Load<UiFontSet>("UiFontSet"));

        public static void Apply(TMP_Text t, bool data, bool outline = false)
        {
            if (t == null) return;
            var role = t.GetComponent<FontRole>();
            if (role == null) role = t.gameObject.AddComponent<FontRole>();
            role.data = data;
            role.outline = outline;
            Assign(t, role);
        }

        private static void Assign(TMP_Text t, FontRole role)
        {
            var s = Set;
            if (s == null) return;
            var font = dyslexia ? s.dyslexia : role.data ? s.data : s.menu;
            if (font == null) return;
            var mat = role.outline ? OutlineMaterial(font) : font.material;
            if (t.font == font && t.fontSharedMaterial == mat) return;
            t.font = font;
            // Never t.outlineWidth: it makes a material instance that TMP keeps reusing after the next font swap, so
            // the new font was drawn with the old font's atlas (garbled text that survived switching back).
            t.fontSharedMaterial = mat;
        }

        private static readonly System.Collections.Generic.Dictionary<TMP_FontAsset, Material> outlineMats =
            new System.Collections.Generic.Dictionary<TMP_FontAsset, Material>();

        /// <summary>One outlined copy of each font's material, owned here (the font asset's own material is never touched).</summary>
        private static Material OutlineMaterial(TMP_FontAsset font)
        {
            if (outlineMats.TryGetValue(font, out var m) && m != null) return m;
            m = new Material(font.material) { name = font.material.name + " (Outline)" };
            m.EnableKeyword(ShaderUtilities.Keyword_Outline);
            m.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.2f);
            m.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
            outlineMats[font] = m;
            return m;
        }

        /// <summary>Reads the option and re-fonts every text in the open scenes (tags untagged ones as menu text).</summary>
        public static void Refresh()
        {
            if (!listening) { GameEvents.OptionsChanged += Refresh; listening = true; }
            if (Services.TryGet(out SaveService saves)) dyslexia = saves.LoadGlobal().dyslexiaFont;
            foreach (var t in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include))
            {
                var role = t.GetComponent<FontRole>();
                if (role == null)
                {
                    role = t.gameObject.AddComponent<FontRole>();
                    role.outline = t.fontSharedMaterial != null && t.fontSharedMaterial.name.Contains("Outline")
                                   || t.fontSharedMaterial != null && t.fontSharedMaterial.HasProperty(ShaderUtilities.ID_OutlineWidth)
                                      && t.fontSharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth) > 0f;
                }
                Assign(t, role);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { listening = false; dyslexia = false; outlineMats.Clear(); }
    }
}
