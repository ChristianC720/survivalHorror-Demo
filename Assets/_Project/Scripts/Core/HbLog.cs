using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Heartbeat.Core
{
    /// <summary>
    /// Project logger. Every runtime line is prefixed "[Heartbeat][System]" so playtests can assert on it.
    /// <see cref="Dev"/> calls are stripped from release builds.
    /// </summary>
    public static class HbLog
    {
        public static void Info(string system, string message) => Debug.Log($"[Heartbeat][{system}] {message}");
        public static void Warn(string system, string message) => Debug.LogWarning($"[Heartbeat][{system}] {message}");
        public static void Error(string system, string message) => Debug.LogError($"[Heartbeat][{system}] {message}");

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Dev(string system, string message) => Debug.Log($"[Heartbeat][{system}][dev] {message}");
    }
}
