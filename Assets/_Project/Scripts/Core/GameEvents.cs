using System;
using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>Who made a noise (M-09). Enemies react differently to some sources (loud vs quiet).</summary>
    public enum NoiseSource { Footsteps, Running, Shot, GroundStrike, Machinery, Scream, Other }

    public readonly struct NoiseInfo
    {
        public readonly Vector3 Position;
        public readonly float Radius;
        public readonly NoiseSource Source;

        public NoiseInfo(Vector3 position, float radius, NoiseSource source)
        {
            Position = position;
            Radius = radius;
            Source = source;
        }
    }

    public enum DeathCause { Health, GrabInCritical, Restart, QuitDuringAlert, Scripted }

    /// <summary>
    /// Static event bus between systems (D-108). Catalog: Desarrollo/EVENTS.md — add rows there first.
    /// Events are added as the systems that raise them are implemented. <see cref="ResetAll"/> runs on every
    /// play-mode entry because domain reload is disabled. Payloads use Core types or MonoBehaviour so Core does not
    /// depend on gameplay code (cast the MonoBehaviour to the concrete type in the listener).
    /// </summary>
    public static class GameEvents
    {
        // Core
        public static event Action<GameStateId, GameStateId> GameStateChanged;
        public static event Action<PlaySubState, PlaySubState> PlaySubStateChanged;
        public static event Action<bool> PauseChanged;

        // Player
        public static event Action<NoiseInfo> NoiseEmitted;
        public static event Action StaminaExhausted;
        public static event Action<int> HealthStateChanged;   // Heartbeat.Player.HealthState
        public static event Action<DeathCause> PlayerDied;

        // Camera / surveillance
        public static event Action<bool> AimChanged;
        public static event Action<Vector3> AttentionFull;

        // Combat
        public static event Action<Vector3> ShotFired;
        public static event Action<AttackInfo> PlayerAttacked;
        public static event Action<bool> GrabResolved;        // true = countered by the spike
        public static event Action<MonoBehaviour, Vector3> EnemyKilled;
        public static event Action<Vector3> RecycleWarning;
        public static event Action<MonoBehaviour> EnemyRecycled;

        // AI
        public static event Action<MonoBehaviour, AlertState, float> EnemyAlertChanged;
        public static event Action<AlertState> HighestAlertChanged;
        public static event Action<bool> HuntChanged;          // true = at least one enemy hunting

        // Level
        public static event Action<MonoBehaviour> RoomEntered;            // Heartbeat.Generation.RoomInstance
        public static event Action<int, int> FloorBuilt;                  // floor, seed
        public static event Action PowerRestored;
        public static event Action<int> FloorExitRequested;               // floor being left

        // Cycle
        public static event Action<int> DegradationChanged;
        public static event Action<int> Reconstructed;                    // new subject code
        public static event Action<string> EndingTriggered;               // Assimilation, BasicRelief, FullRelief, Ambiguous
        public static event Action Autosaved;

        // Inventory / interaction
        public static event Action InventoryChanged;
        public static event Action<string> InventoryFull;                 // item kind that did not fit
        public static event Action<MonoBehaviour> InteractTargetChanged;  // null = nothing to interact with
        public static event Action<string, int, bool> ItemRegistered;     // record id, RecordKind, isNew

        // Audio (S-11)
        public static event Action<string, Vector3?> CueRequested;        // cue id, world position (null = 2D)
        public static event Action<string, Vector3?> CuePlayed;           // cue id actually played (tests, debug captions)

        // Narrative text (D-52: no voice acting, subtitles for every vocal sound)
        public static event Action<string, string, object[]> SubtitleRequested;   // speaker id, SUB key, format args

        // Options (GlobalData saved; listeners re-read it from SaveService)
        public static event Action OptionsChanged;

        public static void RaiseGameStateChanged(GameStateId from, GameStateId to) => GameStateChanged?.Invoke(from, to);
        public static void RaisePlaySubStateChanged(PlaySubState from, PlaySubState to) => PlaySubStateChanged?.Invoke(from, to);
        public static void RaisePauseChanged(bool paused) => PauseChanged?.Invoke(paused);
        public static void RaiseNoiseEmitted(NoiseInfo info) => NoiseEmitted?.Invoke(info);
        public static void RaiseStaminaExhausted() => StaminaExhausted?.Invoke();
        public static void RaiseHealthStateChanged(int state) => HealthStateChanged?.Invoke(state);
        public static void RaisePlayerDied(DeathCause cause) => PlayerDied?.Invoke(cause);
        public static void RaiseAimChanged(bool aiming) => AimChanged?.Invoke(aiming);
        public static void RaiseAttentionFull(Vector3 playerPos) => AttentionFull?.Invoke(playerPos);
        public static void RaiseShotFired(Vector3 origin) => ShotFired?.Invoke(origin);
        public static void RaisePlayerAttacked(AttackInfo info) => PlayerAttacked?.Invoke(info);
        public static void RaiseGrabResolved(bool countered) => GrabResolved?.Invoke(countered);
        public static void RaiseEnemyKilled(MonoBehaviour enemy, Vector3 pos) => EnemyKilled?.Invoke(enemy, pos);
        public static void RaiseRecycleWarning(Vector3 pos) => RecycleWarning?.Invoke(pos);
        public static void RaiseEnemyRecycled(MonoBehaviour enemy) => EnemyRecycled?.Invoke(enemy);
        public static void RaiseEnemyAlertChanged(MonoBehaviour enemy, AlertState state, float distance) => EnemyAlertChanged?.Invoke(enemy, state, distance);
        public static void RaiseHighestAlertChanged(AlertState state) => HighestAlertChanged?.Invoke(state);
        public static void RaiseHuntChanged(bool hunting) => HuntChanged?.Invoke(hunting);
        public static void RaiseRoomEntered(MonoBehaviour room) => RoomEntered?.Invoke(room);
        public static void RaiseFloorBuilt(int floor, int seed) => FloorBuilt?.Invoke(floor, seed);
        public static void RaisePowerRestored() => PowerRestored?.Invoke();
        public static void RaiseFloorExitRequested(int floor) => FloorExitRequested?.Invoke(floor);
        public static void RaiseDegradationChanged(int level) => DegradationChanged?.Invoke(level);
        public static void RaiseReconstructed(int subject) => Reconstructed?.Invoke(subject);
        public static void RaiseEndingTriggered(string ending) => EndingTriggered?.Invoke(ending);
        public static void RaiseAutosaved() => Autosaved?.Invoke();
        public static void RaiseInventoryChanged() => InventoryChanged?.Invoke();
        public static void RaiseInventoryFull(string kind) => InventoryFull?.Invoke(kind);
        public static void RaiseInteractTargetChanged(MonoBehaviour target) => InteractTargetChanged?.Invoke(target);
        public static void RaiseItemRegistered(string id, int kind, bool isNew) => ItemRegistered?.Invoke(id, kind, isNew);
        public static void RaiseCueRequested(string cue, Vector3? pos) => CueRequested?.Invoke(cue, pos);
        public static void RaiseCuePlayed(string cue, Vector3? pos) => CuePlayed?.Invoke(cue, pos);
        public static void RaiseSubtitleRequested(string speaker, string key, object[] args) => SubtitleRequested?.Invoke(speaker, key, args);
        public static void RaiseOptionsChanged() => OptionsChanged?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetAll()
        {
            GameStateChanged = null;
            PlaySubStateChanged = null;
            PauseChanged = null;
            NoiseEmitted = null;
            StaminaExhausted = null;
            HealthStateChanged = null;
            PlayerDied = null;
            AimChanged = null;
            AttentionFull = null;
            ShotFired = null;
            PlayerAttacked = null;
            GrabResolved = null;
            EnemyKilled = null;
            RecycleWarning = null;
            EnemyRecycled = null;
            EnemyAlertChanged = null;
            HighestAlertChanged = null;
            HuntChanged = null;
            InventoryChanged = null;
            DegradationChanged = null;
            Reconstructed = null;
            EndingTriggered = null;
            Autosaved = null;
            RoomEntered = null;
            FloorBuilt = null;
            PowerRestored = null;
            FloorExitRequested = null;
            InventoryFull = null;
            InteractTargetChanged = null;
            ItemRegistered = null;
            CueRequested = null;
            CuePlayed = null;
            SubtitleRequested = null;
            OptionsChanged = null;
            Services.Clear();
        }
    }
}
