using System.Text;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// Terminal rendering of record text (D-127, D-58): passages marked <c>…</c> (Lukas, his life with Emilia) show as
    /// purple glitch until the ending reveals them (D-55). Spaces and line breaks are kept so the shape still reads.
    /// Deterministic per text, so the glitch does not flicker between openings. Pure, testable.
    /// </summary>
    public static class DocText
    {
        public const string GlitchColor = "#A050FF";
        private const string Glyphs = "#%&@$?!/|*=+~^ÆØ§¤¥£";

        public const string SubtitleColor = "#8FB3A8";

        /// <summary>Localization key of a German line's translation: "DE." + the text in capitals, ÄÖÜß spelled out, runs of other characters as "_".</summary>
        public static string GermanKey(string german)
        {
            var sb = new StringBuilder("DE.", german.Length + 8);
            bool gap = false;
            foreach (char ch in german.ToUpperInvariant())
            {
                string part = ch switch { 'Ä' => "AE", 'Ö' => "OE", 'Ü' => "UE", 'ß' => "SS", _ => (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') ? ch.ToString() : null };
                if (part == null) { gap = sb.Length > 3; continue; }
                if (gap) sb.Append('_');
                gap = false;
                sb.Append(part);
            }
            return sb.ToString();
        }

        /// <summary>
        /// German lines (<i>…</i>) stay German; in the arm terminal's records each one gets a small subtitle below it with the
        /// translation (C-18). <paramref name="translate"/> returns null when a line has no translation (e.g. "(breathing)").
        /// </summary>
        public static string WithGermanSubtitles(string raw, System.Func<string, string> translate)
        {
            if (string.IsNullOrEmpty(raw)) return raw;
            var sb = new StringBuilder(raw.Length + 64);
            int i = 0;
            while (i < raw.Length)
            {
                int open = raw.IndexOf("<i>", i, System.StringComparison.Ordinal);
                int close = open < 0 ? -1 : raw.IndexOf("</i>", open + 3, System.StringComparison.Ordinal);
                if (open < 0 || close < 0) { sb.Append(raw, i, raw.Length - i); break; }
                sb.Append(raw, i, close + 4 - i);
                string line = translate(raw.Substring(open + 3, close - open - 3));
                if (!string.IsNullOrEmpty(line)) sb.Append("\n<size=75%><color=").Append(SubtitleColor).Append(">").Append(line).Append("</color></size>");
                i = close + 4;
            }
            return sb.ToString();
        }

        public static string Render(string raw, bool reveal)
        {
            if (string.IsNullOrEmpty(raw)) return raw;
            var sb = new StringBuilder(raw.Length + 32);
            int i = 0;
            while (i < raw.Length)
            {
                int open = raw.IndexOf("<c>", i, System.StringComparison.Ordinal);
                if (open < 0) { sb.Append(raw, i, raw.Length - i); break; }
                int close = raw.IndexOf("</c>", open + 3, System.StringComparison.Ordinal);
                if (close < 0) close = raw.Length;
                sb.Append(raw, i, open - i);
                string inner = raw.Substring(open + 3, close - open - 3);
                if (reveal) sb.Append(inner);
                else
                {
                    sb.Append("<color=").Append(GlitchColor).Append('>');
                    for (int k = 0; k < inner.Length; k++)
                    {
                        char ch = inner[k];
                        sb.Append(char.IsWhiteSpace(ch) ? ch : Glyphs[(ch * 31 + k * 17) % Glyphs.Length]);
                    }
                    sb.Append("</color>");
                }
                i = close + 4;
            }
            return sb.ToString();
        }
    }
}
