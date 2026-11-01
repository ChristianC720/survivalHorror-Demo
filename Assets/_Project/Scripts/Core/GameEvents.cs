using System;

namespace Heartbeat.Core
{
    /// <summary>
    /// Static event bus between systems (D-108). Catalog: Desarrollo/EVENTS.md — add rows there first.
    /// Events are added here as the systems that raise them are implemented.
    /// <see cref="ResetAll"/> runs on every play-mode entry because domain reload is disabled.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<GameStateId, GameStateId> GameStateChanged;
        public static event Action<PlaySubState, PlaySubState> PlaySubStateChanged;
        public static event Action<bool> PauseChanged;

        public static void RaiseGameStateChanged(GameStateId from, GameStateId to) => GameStateChanged?.Invoke(from, to);
        public static void RaisePlaySubStateChanged(PlaySubState from, PlaySubState to) => PlaySubStateChanged?.Invoke(from, to);
        public static void RaisePauseChanged(bool paused) => PauseChanged?.Invoke(paused);

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetAll()
        {
            GameStateChanged = null;
            PlaySubStateChanged = null;
            PauseChanged = null;
            Services.Clear();
        }
    }
}
