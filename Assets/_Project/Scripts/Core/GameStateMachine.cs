using System;
using System.Collections.Generic;

namespace Heartbeat.Core
{
    /// <summary>
    /// Pure state machine for the game flow. Transitions follow the "Transiciones" table of
    /// GDD 02 - Diseño/07 - Reglas y Estados del Juego. Invalid transitions are rejected (returns false).
    /// No Unity dependencies so it is EditMode-testable; the GameFlow MonoBehaviour drives scenes from it.
    /// </summary>
    public sealed class GameStateMachine
    {
        private static readonly Dictionary<GameStateId, GameStateId[]> Allowed = new Dictionary<GameStateId, GameStateId[]>
        {
            { GameStateId.Boot, new[] { GameStateId.BrightnessCalibration, GameStateId.Title, GameStateId.MainMenu, GameStateId.Playing, GameStateId.Intro } },
            { GameStateId.BrightnessCalibration, new[] { GameStateId.Title } },
            { GameStateId.Title, new[] { GameStateId.MainMenu } },
            { GameStateId.MainMenu, new[] { GameStateId.Intro, GameStateId.Loading, GameStateId.Credits, GameStateId.Quit } },
            { GameStateId.Intro, new[] { GameStateId.Loading, GameStateId.Paused } },
            { GameStateId.Loading, new[] { GameStateId.Playing, GameStateId.Reconstruction, GameStateId.EndingAssimilation } },
            { GameStateId.Playing, new[] { GameStateId.Paused, GameStateId.FloorTransition, GameStateId.Death, GameStateId.Victory } },
            { GameStateId.Paused, new[] { GameStateId.Playing, GameStateId.Intro, GameStateId.Death, GameStateId.MainMenu } },
            { GameStateId.FloorTransition, new[] { GameStateId.Playing } },
            { GameStateId.Death, new[] { GameStateId.Reconstruction, GameStateId.EndingAssimilation, GameStateId.MainMenu } },
            { GameStateId.Reconstruction, new[] { GameStateId.Playing } },
            { GameStateId.Victory, new[] { GameStateId.EndingBasicRelief, GameStateId.FinalDecision } },
            { GameStateId.FinalDecision, new[] { GameStateId.EndingFullRelief, GameStateId.EndingAmbiguous } },
            { GameStateId.EndingBasicRelief, new[] { GameStateId.Credits } },
            { GameStateId.EndingFullRelief, new[] { GameStateId.Credits } },
            { GameStateId.EndingAmbiguous, new[] { GameStateId.Credits } },
            { GameStateId.EndingAssimilation, new[] { GameStateId.Credits } },
            { GameStateId.Credits, new[] { GameStateId.MainMenu } },
            { GameStateId.Quit, Array.Empty<GameStateId>() },
        };

        private static readonly Dictionary<PlaySubState, PlaySubState[]> AllowedSub = new Dictionary<PlaySubState, PlaySubState[]>
        {
            { PlaySubState.Exploration, new[] { PlaySubState.Surveillance, PlaySubState.Alert, PlaySubState.Hacking } },
            { PlaySubState.Surveillance, new[] { PlaySubState.Exploration, PlaySubState.Alert } },
            { PlaySubState.Alert, new[] { PlaySubState.Exploration, PlaySubState.Hacking } },
            { PlaySubState.Hacking, new[] { PlaySubState.Exploration, PlaySubState.Alert } },
        };

        public GameStateId Current { get; private set; } = GameStateId.Boot;
        public PlaySubState SubState { get; private set; } = PlaySubState.Exploration;

        /// <summary>State that Paused returns to (Playing or Intro).</summary>
        public GameStateId StateBeforePause { get; private set; } = GameStateId.Playing;

        public event Action<GameStateId, GameStateId> StateChanged;
        public event Action<PlaySubState, PlaySubState> SubStateChanged;

        public static bool IsAllowed(GameStateId from, GameStateId to) =>
            Allowed.TryGetValue(from, out var targets) && Array.IndexOf(targets, to) >= 0;

        public static bool IsAllowed(PlaySubState from, PlaySubState to) =>
            AllowedSub.TryGetValue(from, out var targets) && Array.IndexOf(targets, to) >= 0;

        /// <summary>The game world runs (time flows, AI ticks) only in these states.</summary>
        public bool IsWorldRunning => Current == GameStateId.Playing || Current == GameStateId.Intro || Current == GameStateId.FinalDecision;

        public bool TryTransition(GameStateId to)
        {
            if (!IsAllowed(Current, to)) return false;
            if (to == GameStateId.Paused && Current != GameStateId.Paused) StateBeforePause = Current;
            if (Current == GameStateId.Paused && (to == GameStateId.Playing || to == GameStateId.Intro) && to != StateBeforePause) return false;

            var from = Current;
            Current = to;

            // Entering Playing from anything except Pause starts in Exploration (Loading, transition, reconstruction).
            if (to == GameStateId.Playing && from != GameStateId.Paused && SubState != PlaySubState.Exploration)
            {
                var oldSub = SubState;
                SubState = PlaySubState.Exploration;
                SubStateChanged?.Invoke(oldSub, SubState);
            }

            StateChanged?.Invoke(from, to);
            return true;
        }

        /// <summary>Returns to the state that was active before pausing.</summary>
        public bool TryResume() => Current == GameStateId.Paused && TryTransition(StateBeforePause);

        public bool TrySetSubState(PlaySubState to)
        {
            if (Current != GameStateId.Playing) return false;
            if (to == SubState) return true;
            if (!IsAllowed(SubState, to)) return false;
            var from = SubState;
            SubState = to;
            SubStateChanged?.Invoke(from, to);
            return true;
        }
    }
}
