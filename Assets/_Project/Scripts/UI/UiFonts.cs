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
            if (font == null || t.font == font) return;
            t.font = font;
            t.fontSharedMaterial = font.material;
            if (role.outline) { t.outlineWidth = 0.2f; t.outlineColor = new Color32(0, 0, 0, 255); }
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
                    role.outline = t.fontSharedMaterial != null && t.fontSharedMaterial.name.Contains("Outline");
                }
                Assign(t, role);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { listening = false; dyslexia = false; }
    }
}
