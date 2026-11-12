using System;
using System.Collections.Generic;

namespace Heartbeat.Level
{
    public enum HackDir { Up, Down, Left, Right }
    public enum HackStep { Correct, Error, Complete }

    /// <summary>Hack lengths (M-13): quick 2-3 (no target uses it since doors seal by holding Interact), standard door 4-5, elevator / main terminal 7-8.</summary>
    public enum HackDifficulty { QuickSeal, Door, Main }

    /// <summary>
    /// Directional input chain (M-13). A wrong input restarts the chain from the first entry. Pure logic, seeded.
    /// </summary>
    public sealed class HackSequence
    {
        private readonly List<HackDir> steps;

        public IReadOnlyList<HackDir> Steps => steps;
        public int Progress { get; private set; }
        public int Errors { get; private set; }
        public bool IsComplete => Progress >= steps.Count;

        public HackSequence(IEnumerable<HackDir> fixedSteps) { steps = new List<HackDir>(fixedSteps); }

        public static HackSequence Generate(HackDifficulty difficulty, Random rng)
        {
            int length = difficulty switch
            {
                HackDifficulty.QuickSeal => rng.Next(2, 4),
                HackDifficulty.Door => rng.Next(4, 6),
                _ => rng.Next(7, 9),
            };
            var list = new List<HackDir>(length);
            for (int i = 0; i < length; i++) list.Add((HackDir)rng.Next(0, 4));
            return new HackSequence(list);
        }

        /// <summary>Machine fault (M-17): drops the tail of the chain; at least one input is always left to enter.</summary>
        public void Truncate(int length)
        {
            length = Math.Max(length, Progress + 1);
            if (length < steps.Count) steps.RemoveRange(length, steps.Count - length);
        }

        public HackStep Input(HackDir dir)
        {
            if (IsComplete) return HackStep.Complete;
            if (steps[Progress] != dir)
            {
                Progress = 0;
                Errors++;
                return HackStep.Error;
            }
            Progress++;
            return IsComplete ? HackStep.Complete : HackStep.Correct;
        }
    }
}
