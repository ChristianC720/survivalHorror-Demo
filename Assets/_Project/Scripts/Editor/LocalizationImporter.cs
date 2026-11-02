using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Heartbeat.EditorTools
{
    /// <summary>
    /// Source of truth for all in-game text: Assets/_Project/Localization/Source/&lt;TABLE&gt;.tsv
    /// (columns: key, en, es[, note]). This importer creates the Localization settings, the en/es locales and one
    /// string table collection per file, and upserts every row. Edit the TSV, then run Heartbeat/Import Localization.
    /// A note column containing DE-NO-TRANSLATE (German lines) or KEEP-EN (terminal labels, D-62) copies the English text to Spanish.
    /// </summary>
    public static class LocalizationImporter
    {
        private const string Root = "Assets/_Project/Localization";
        private const string Source = Root + "/Source";
        private const string Tables = Root + "/Tables";
        private static readonly string[] Codes = { "en", "es" };

        [MenuItem("Heartbeat/Import Localization")]
        public static string Import()
        {
            EnsureSettings();
            var locales = Codes.Select(EnsureLocale).ToList();
            int rows = 0;
            var tables = new List<string>();
            foreach (var file in Directory.GetFiles(Source, "*.tsv"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                tables.Add(name);
                var col = LocalizationEditorSettings.GetStringTableCollection(name)
                          ?? LocalizationEditorSettings.CreateStringTableCollection(name, Tables, locales);
                foreach (var line in File.ReadAllLines(file).Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var c = line.Split('\t');
                    if (c.Length < 3) { Debug.LogWarning($"[Heartbeat][Loc] {name}: bad row '{line}'"); continue; }
                    string key = c[0].Trim();
                    bool keepGerman = c.Length > 3 && (c[3].Contains("DE-NO-TRANSLATE") || c[3].Contains("KEEP-EN"));
                    if (col.SharedData.GetEntry(key) == null) col.SharedData.AddKey(key);
                    for (int i = 0; i < Codes.Length; i++)
                    {
                        var table = (StringTable)col.GetTable(Codes[i]);
                        if (table == null) continue;
                        string text = keepGerman ? c[1] : c[1 + i];
                        table.AddEntry(key, text.Replace("\\n", "\n"));   // TSV escape -> real line break (was a no-op)
                        EditorUtility.SetDirty(table);
                    }
                    rows++;
                }
                EditorUtility.SetDirty(col.SharedData);
            }
            AssetDatabase.SaveAssets();
            string msg = $"[Heartbeat][Loc] Imported {rows} rows into {string.Join(", ", tables)}";
            Debug.Log(msg);
            return msg;
        }

        private static void EnsureSettings()
        {
            var active = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (active != null)
            {
                // Missing keys show the key itself, so gaps are visible and greppable.
                active.GetStringDatabase().NoTranslationFoundMessage = "{key}";
                EditorUtility.SetDirty(active);
                return;
            }
            Directory.CreateDirectory(Root);
            var s = ScriptableObject.CreateInstance<LocalizationSettings>();
            AssetDatabase.CreateAsset(s, Root + "/LocalizationSettings.asset");
            LocalizationEditorSettings.ActiveLocalizationSettings = s;
            EnsureSettings();
        }

        private static Locale EnsureLocale(string code)
        {
            var existing = LocalizationEditorSettings.GetLocale(new LocaleIdentifier(code));
            if (existing != null) return existing;
            Directory.CreateDirectory(Root + "/Locales");
            var l = Locale.CreateLocale(code);
            AssetDatabase.CreateAsset(l, $"{Root}/Locales/{code}.asset");
            LocalizationEditorSettings.AddLocale(l);
            return l;
        }
    }
}
