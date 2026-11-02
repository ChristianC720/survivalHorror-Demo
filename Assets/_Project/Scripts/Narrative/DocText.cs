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
