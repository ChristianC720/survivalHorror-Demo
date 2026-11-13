using System;

namespace Heartbeat.Core
{
    /// <summary>
    /// Command-line switches of a build: -showcase (event preset for new slots) and -telemetry (local playtest file).
    /// In the editor, Heartbeat ▸ Launch Arguments simulates them for the current editor session.
    /// </summary>
    public static class LaunchArgs
    {
        public const string Showcase = "-showcase";
        public const string Telemetry = "-telemetry";

        public static bool Has(string arg)
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), arg) >= 0) return true;
#if UNITY_EDITOR
            return UnityEditor.SessionState.GetBool(EditorKey(arg), false);
#else
            return false;
#endif
        }

        /// <summary>SessionState key of a simulated switch (cleared when the editor closes).</summary>
        public static string EditorKey(string arg) => "Heartbeat.LaunchArg" + arg;
    }
}
