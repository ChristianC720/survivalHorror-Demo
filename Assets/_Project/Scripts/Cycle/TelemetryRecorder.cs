using System;
using System.Globalization;
using System.IO;
using System.Text;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Cycle
{
    /// <summary>
    /// Local, anonymous playtest telemetry (GDD Seguridad y Riesgos Técnicos, Pruebas). Off unless the build is launched with
    /// -telemetry. Appends one JSON object per line to persistentDataPath/telemetry/session_*.jsonl and flushes each line, so
    /// a crash keeps everything written so far. The metrics of the Pruebas table are computed from these lines offline:
    /// surveillance uses and seconds per floor, attention fills, time per floor and in total, deaths by floor, room and
    /// cause, quitting before the end, finishers against recycled enemies, grabs (with or without a spike) and pick-ups (C-64), doors sealed
    /// with a hold (C-66).
    /// </summary>
    public sealed class TelemetryRecorder : IDisposable
    {
        public const string Arg = LaunchArgs.Telemetry;

        public static bool Requested(string[] args) => Array.IndexOf(args, Arg) >= 0;

        /// <summary>True when this run has -telemetry (or the editor simulates it, Heartbeat ▸ Launch Arguments).</summary>
        public static bool Requested() => LaunchArgs.Has(Arg);

        private readonly TextWriter writer;
        private readonly Func<float> clock;
        private int floor;
        private string room = "";

        public TelemetryRecorder(TextWriter writer, Func<float> clock)
        {
            this.writer = writer;
            this.clock = clock;
            GameEvents.FloorBuilt += OnFloorBuilt;
            GameEvents.RoomEntered += OnRoomEntered;
            GameEvents.PlaySubStateChanged += OnSubState;
            GameEvents.AttentionFull += OnAttentionFull;
            GameEvents.PlayerDied += OnDied;
            GameEvents.EnemyKilled += OnKilled;
            GameEvents.EnemyRecycled += OnRecycled;
            GameEvents.PlayerGesture += OnGesture;
            GameEvents.FloorExitRequested += OnFloorExit;
            GameEvents.EndingTriggered += OnEnding;
            GameEvents.GameStateChanged += OnState;
            GameEvents.GrabResolved += OnGrab;
            GameEvents.ItemPickedUp += OnPicked;
            GameEvents.DoorSealed += OnSealed;
            Write("session_start", null);
        }

        /// <summary>Opens a new session file under persistentDataPath/telemetry.</summary>
        public static TelemetryRecorder CreateFile()
        {
            string dir = Path.Combine(Application.persistentDataPath, "telemetry");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, $"session_{DateTime.Now:yyyyMMdd_HHmmss}.jsonl");
            HbLog.Info("Telemetry", $"Local telemetry on: {path}");
            // Seconds since the session started: realtimeSinceStartup restarts when the editor leaves Play, before OnDestroy.
            var clock = System.Diagnostics.Stopwatch.StartNew();
            return new TelemetryRecorder(new StreamWriter(path, false, new UTF8Encoding(false)) { AutoFlush = true }, () => (float)clock.Elapsed.TotalSeconds);
        }

        public void Dispose()
        {
            Write("session_end", null);
            GameEvents.FloorBuilt -= OnFloorBuilt;
            GameEvents.RoomEntered -= OnRoomEntered;
            GameEvents.PlaySubStateChanged -= OnSubState;
            GameEvents.AttentionFull -= OnAttentionFull;
            GameEvents.PlayerDied -= OnDied;
            GameEvents.EnemyKilled -= OnKilled;
            GameEvents.EnemyRecycled -= OnRecycled;
            GameEvents.PlayerGesture -= OnGesture;
            GameEvents.FloorExitRequested -= OnFloorExit;
            GameEvents.EndingTriggered -= OnEnding;
            GameEvents.GameStateChanged -= OnState;
            GameEvents.GrabResolved -= OnGrab;
            GameEvents.ItemPickedUp -= OnPicked;
            GameEvents.DoorSealed -= OnSealed;
            writer.Dispose();
        }

        private void OnFloorBuilt(int f, int seed) { floor = f; Write("floor_start", "\"seed\":" + seed); }

        /// <summary>A floor built before the recorder opened (Play pressed in the Floor scene): log its start now.</summary>
        public void FloorStarted(int f, int seed) => OnFloorBuilt(f, seed);
        private void OnRoomEntered(MonoBehaviour r) { room = r != null ? r.name : ""; Write("room", null); }
        private void OnSubState(PlaySubState from, PlaySubState to)
        {
            if (to == PlaySubState.Surveillance) Write("surveillance_on", null);
            else if (from == PlaySubState.Surveillance) Write("surveillance_off", null);
        }
        private void OnAttentionFull(Vector3 _) => Write("attention_full", null);
        private void OnDied(DeathCause cause) => Write("death", $"\"cause\":\"{cause}\"");
        private void OnKilled(MonoBehaviour e, Vector3 _) => Write("enemy_killed", $"\"enemy\":\"{Esc(e != null ? e.name : "")}\"");
        private void OnRecycled(MonoBehaviour e) => Write("enemy_recycled", $"\"enemy\":\"{Esc(e != null ? e.name : "")}\"");
        private void OnGesture(string g) { if (g == "Finisher" || g == "GroundStrike") Write("finisher", $"\"how\":\"{g}\""); }
        private void OnFloorExit(int f) => Write("floor_exit", null);
        private void OnEnding(string id) => Write("ending", $"\"id\":\"{Esc(id)}\"");
        private void OnGrab(bool spike) => Write("grab", "\"spike\":" + (spike ? "true" : "false"));
        private void OnPicked(string kind, int n) => Write("item_picked", $"\"kind\":\"{Esc(kind)}\",\"amount\":{n}");
        private void OnSealed(MonoBehaviour d) => Write("door_sealed", $"\"door\":\"{Esc(d != null ? d.name : "")}\"");
        private void OnState(GameStateId from, GameStateId to) => Write("state", $"\"from\":\"{from}\",\"to\":\"{to}\"");

        private void Write(string evt, string extra)
        {
            var sb = new StringBuilder(128);
            sb.Append("{\"t\":").Append(clock().ToString("F2", CultureInfo.InvariantCulture))
              .Append(",\"event\":\"").Append(evt).Append("\",\"floor\":").Append(floor)
              .Append(",\"room\":\"").Append(Esc(room)).Append('"');
            if (!string.IsNullOrEmpty(extra)) sb.Append(',').Append(extra);
            sb.Append('}');
            try { writer.WriteLine(sb.ToString()); }
            catch (Exception ex) { HbLog.Warn("Telemetry", "Write failed: " + ex.Message); }
        }

        private static string Esc(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
