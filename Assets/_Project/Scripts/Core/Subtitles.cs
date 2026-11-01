namespace Heartbeat.Core
{
    /// <summary>
    /// Request a subtitle line from gameplay (D-52). Speakers: Lukas, Mother, Emilia. The key is a SUB table key;
    /// the UI resolves the text, shows the speaker name and plays CUE_Vocal_&lt;Speaker&gt;.
    /// </summary>
    public static class Subtitles
    {
        public const string Lukas = "Lukas";
        public const string Mother = "Mother";
        public const string Emilia = "Emilia";

        public static void Say(string speaker, string key, params object[] args) => GameEvents.RaiseSubtitleRequested(speaker, key, args);
    }
}
