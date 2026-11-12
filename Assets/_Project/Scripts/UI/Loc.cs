using Heartbeat.Core;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Heartbeat.UI
{
    /// <summary>
    /// Text lookup for the whole game. Keys are "&lt;TABLE&gt;.&lt;area&gt;.&lt;id&gt;" (15-texts.md): the table is the first
    /// segment, e.g. Loc.Get("UI.TERM.SUBJECT", "R-H-09"). Missing keys return the key itself, so gaps are visible.
    /// </summary>
    public static class Loc
    {
        public static string Get(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key)) return "";
            int dot = key.IndexOf('.');
            if (dot <= 0) return key;
            return LocalizationSettings.StringDatabase.GetLocalizedString(key.Substring(0, dot), key, args);
        }

        /// <summary>The English text whatever the selected language: signs fixed in the world, like the Vitalis safety posters (C-51).</summary>
        public static string GetEnglish(string key, params object[] args)
        {
            int dot = string.IsNullOrEmpty(key) ? -1 : key.IndexOf('.');
            if (dot <= 0) return key ?? "";
            var english = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en"));
            return LocalizationSettings.StringDatabase.GetLocalizedString(key.Substring(0, dot), key, english, FallbackBehavior.UseProjectSettings, args);
        }

        public static string Language => LocalizationSettings.SelectedLocale != null ? LocalizationSettings.SelectedLocale.Identifier.Code : "en";

        /// <summary>Switch at runtime ("en" / "es"). Every LocalizedLabel refreshes through SelectedLocaleChanged.</summary>
        public static void SetLanguage(string code)
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));
            if (locale == null) { HbLog.Warn("Loc", $"No locale '{code}'"); return; }
            LocalizationSettings.SelectedLocale = locale;
            HbLog.Info("Loc", $"Language={code}");
        }
    }
}
