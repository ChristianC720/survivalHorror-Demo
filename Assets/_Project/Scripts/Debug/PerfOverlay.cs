using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.DebugTools
{
    /// <summary>
    /// Performance readout for playtests. Off unless the player is launched with <c>-perf</c>, so the
    /// diegetic no-HUD rule (D-86) holds in normal play. Logs a summary every 10 s and every spike to Player.log.
    /// </summary>
    public sealed class PerfOverlay : MonoBehaviour
    {
        private const float SpikeMs = 50f;      // tooling threshold, not gameplay balance
        private const float SummaryEvery = 10f;

        private float windowTime, windowMax, summaryTime, summaryMax, summaryMinFps = float.MaxValue;
        private int windowFrames, summaryFrames, spikes, gcStart;
        private string text = "";
        private GUIStyle style;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (!System.Environment.CommandLine.Contains("-perf")) return;
            var go = new GameObject("[PerfOverlay]");
            DontDestroyOnLoad(go);
            go.AddComponent<PerfOverlay>();
            HbLog.Info("Perf", $"overlay on · {SystemInfo.graphicsDeviceName} · {SystemInfo.processorType} · {Screen.width}x{Screen.height}");
        }

        private void Start() => gcStart = System.GC.CollectionCount(0);

        private void Update()
        {
            float ms = Time.unscaledDeltaTime * 1000f; // unscaled: pause/reading sets timeScale 0
            if (ms > SpikeMs && Time.frameCount > 10)
            {
                spikes++;
                HbLog.Info("Perf", $"spike {ms:F1} ms at frame {Time.frameCount}");
            }

            windowFrames++; windowTime += Time.unscaledDeltaTime; windowMax = Mathf.Max(windowMax, ms);
            summaryFrames++; summaryTime += Time.unscaledDeltaTime; summaryMax = Mathf.Max(summaryMax, ms);

            if (windowTime >= 0.5f)
            {
                float fps = windowFrames / windowTime;
                summaryMinFps = Mathf.Min(summaryMinFps, fps);
                text = $"{fps:F0} FPS  {windowTime * 1000f / windowFrames:F1} ms  max {windowMax:F1} ms\n" +
                       $"spikes>{SpikeMs:F0}ms {spikes}  GC0 {System.GC.CollectionCount(0) - gcStart}";
                windowFrames = 0; windowTime = 0f; windowMax = 0f;
            }

            if (summaryTime >= SummaryEvery)
            {
                HbLog.Info("Perf", $"avg {summaryFrames / summaryTime:F1} fps · min {summaryMinFps:F0} fps · worst {summaryMax:F1} ms · spikes {spikes}");
                summaryFrames = 0; summaryTime = 0f; summaryMax = 0f; summaryMinFps = float.MaxValue;
            }
        }

        private void OnGUI()
        {
            style ??= new GUIStyle(GUI.skin.label) { fontSize = 18, normal = { textColor = Color.yellow } };
            GUI.Label(new Rect(10, 10, 600, 60), text, style);
        }
    }
}
