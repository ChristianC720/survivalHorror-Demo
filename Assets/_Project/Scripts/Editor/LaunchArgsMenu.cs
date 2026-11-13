using Heartbeat.Core;
using UnityEditor;

namespace Heartbeat.EditorTools
{
    /// <summary>Simulates the build's -showcase and -telemetry switches in Play mode (until the editor closes).</summary>
    public static class LaunchArgsMenu
    {
        private const string ShowcaseItem = "Heartbeat/Launch Arguments/-showcase";
        private const string TelemetryItem = "Heartbeat/Launch Arguments/-telemetry";

        [MenuItem(ShowcaseItem)] private static void ToggleShowcase() => Toggle(LaunchArgs.Showcase);
        [MenuItem(ShowcaseItem, true)] private static bool ShowcaseCheck() => Check(ShowcaseItem, LaunchArgs.Showcase);
        [MenuItem(TelemetryItem)] private static void ToggleTelemetry() => Toggle(LaunchArgs.Telemetry);
        [MenuItem(TelemetryItem, true)] private static bool TelemetryCheck() => Check(TelemetryItem, LaunchArgs.Telemetry);

        private static void Toggle(string arg)
        {
            string key = LaunchArgs.EditorKey(arg);
            SessionState.SetBool(key, !SessionState.GetBool(key, false));
        }

        private static bool Check(string item, string arg)
        {
            Menu.SetChecked(item, SessionState.GetBool(LaunchArgs.EditorKey(arg), false));
            return !EditorApplication.isPlaying;   // read once at Play start: toggle outside Play
        }
    }
}
