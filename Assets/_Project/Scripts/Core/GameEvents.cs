using System;
using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>Who made a noise (M-09). Enemies react differently to some sources later (e.g. ground strike).</summary>
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
    /// Events are added here as the systems that raise them are implemented.
    /// <see cref="ResetAll"/> runs on every play-mode entry because domain reload is disabled.
    /// Health state is passed as int to keep Core independent of Player; cast to Heartbeat.Player.HealthState.
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
        public static event Action<int> HealthStateChanged;
        public static event Action<DeathCause> PlayerDied;

        // Combat
        public static event Action<Vector3> ShotFired;

        // Inventory
        public static event Action InventoryChanged;

        // Camera
        public static event Action<bool> AimChanged;

        public static void RaiseGameStateChanged(GameStateId from, GameStateId to) => GameStateChanged?.Invoke(from, to);
        public static void RaisePlaySubStateChanged(PlaySubState from, PlaySubState to) => PlaySubStateChanged?.Invoke(from, to);
        public static void RaisePauseChanged(bool paused) => PauseChanged?.Invoke(paused);
        public static void RaiseNoiseEmitted(NoiseInfo info) => NoiseEmitted?.Invoke(info);
        public static void RaiseStaminaExhausted() => StaminaExhausted?.Invoke();
        public static void RaiseHealthStateChanged(int state) => HealthStateChanged?.Invoke(state);
        public static void RaisePlayerDied(DeathCause cause) => PlayerDied?.Invoke(cause);
        public static void RaiseShotFired(Vector3 origin) => ShotFired?.Invoke(origin);
        public static void RaiseInventoryChanged() => InventoryChanged?.Invoke();
        public static void RaiseAimChanged(bool aiming) => AimChanged?.Invoke(aiming);

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
            InventoryChanged = null;
            ShotFired = null;
            Services.Clear();
        }
    }
}
