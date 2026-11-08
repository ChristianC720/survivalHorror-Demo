using System;
using System.Collections.Generic;
using System.Linq;

namespace Heartbeat.Tutorial
{
    public enum TutorialAction { Terminal, Flashlight, PickUp, Sprint, Crouch, Aim, Attack }

    /// <summary>What each room of the guided tutorial asks before its exit opens (D-164). Actions count only in their own room.</summary>
    public sealed class TutorialModel
    {
        public const int LastStage = 4;

        private static readonly TutorialAction[][] Steps =
        {
            new[] { TutorialAction.Terminal },
            new[] { TutorialAction.Flashlight, TutorialAction.PickUp },
            new[] { TutorialAction.Sprint },
            new[] { TutorialAction.Crouch, TutorialAction.Aim, TutorialAction.Attack },
        };

        private readonly HashSet<TutorialAction> done = new HashSet<TutorialAction>();
        private readonly HashSet<int> cleared = new HashSet<int>();

        /// <summary>1 capsule row, 2 depot, 3 tunnel, 4 boiler room; 0 outside the tutorial.</summary>
        public int Stage { get; set; } = 1;
        public event Action<int> StageCleared;

        public TutorialAction? Current => Stage < 1 || Stage > LastStage || IsCleared(Stage)
            ? (TutorialAction?)null
            : Steps[Stage - 1].First(a => !done.Contains(a));

        public bool IsCleared(int stage) => cleared.Contains(stage);

        public bool Do(TutorialAction action)
        {
            if (Stage < 1 || Stage > LastStage || !Steps[Stage - 1].Contains(action) || !done.Add(action)) return false;
            if (Steps[Stage - 1].All(done.Contains)) Clear(Stage);
            return true;
        }

        /// <summary>Also the time-out safety: a room can never hold the player forever.</summary>
        public void Clear(int stage)
        {
            if (cleared.Add(stage)) StageCleared?.Invoke(stage);
        }
    }
}
