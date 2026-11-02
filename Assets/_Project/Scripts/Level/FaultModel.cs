using System;
using System.Collections.Generic;

namespace Heartbeat.Level
{
    /// <summary>
    /// Machine faults (M-17, D-09): knowledge shortcuts discovered by reading a fault document ([F] record). During a
    /// hack of the matching kind the player may exploit one to shorten the chain. The Mother patches a fault after
    /// <see cref="UsesBeforePatch"/> uses or on any death; patches persist in the slot, the record stays in the terminal.
    /// Pure logic.
    /// </summary>
    public sealed class FaultModel
    {
        /// <summary>The demo's faults (content, not balance): record id → hack kind it shortens.</summary>
        public static readonly IReadOnlyDictionary<string, HackDifficulty> Faults = new Dictionary<string, HackDifficulty>
        {
            { "FAULT.DOOR_BACKDOOR", HackDifficulty.Door },   // floor 1: maintenance backdoor in the door locks
            { "FAULT.MIXER_BYPASS", HackDifficulty.Main },    // floor 3: mixing-desk bypass (elevator, The Mother's nodes)
        };

        private readonly HashSet<string> patched = new HashSet<string>();
        private readonly Dictionary<string, int> uses = new Dictionary<string, int>();

        public int UsesBeforePatch { get; }
        public int DoorLength { get; }
        public float MainFraction { get; }

        public event Action<string> Used;
        public event Action<string> Patched;

        public FaultModel(int usesBeforePatch, int doorLength, float mainFraction)
        {
            UsesBeforePatch = Math.Max(1, usesBeforePatch);
            DoorLength = Math.Max(1, doorLength);
            MainFraction = mainFraction;
        }

        public bool IsPatched(string id) => patched.Contains(id);
        public int UsesLeft(string id) => IsPatched(id) ? 0 : UsesBeforePatch - (uses.TryGetValue(id, out int n) ? n : 0);

        /// <summary>A discovered, unpatched fault that shortens this kind of hack, or null.</summary>
        public string UsableFor(HackDifficulty kind, Func<string, bool> discovered)
        {
            foreach (var f in Faults)
                if (f.Value == kind && !patched.Contains(f.Key) && discovered(f.Key)) return f.Key;
            return null;
        }

        /// <summary>Chain length once the fault is exploited (never longer than the original).</summary>
        public int ShortLength(HackDifficulty kind, int original)
        {
            int n = kind == HackDifficulty.Main ? (int)Math.Ceiling(original * MainFraction) : DoorLength;
            return Math.Max(1, Math.Min(original, n));
        }

        /// <summary>Counts one use; the fault is patched when its uses run out.</summary>
        public void Use(string id)
        {
            if (IsPatched(id)) return;
            uses[id] = (uses.TryGetValue(id, out int n) ? n : 0) + 1;
            Used?.Invoke(id);
            if (uses[id] >= UsesBeforePatch) Patch(id);
        }

        /// <summary>On death The Mother patches every discovered fault, including one in use (D-09).</summary>
        public void PatchDiscovered(Func<string, bool> discovered)
        {
            foreach (var id in Faults.Keys)
                if (discovered(id)) Patch(id);
        }

        private void Patch(string id)
        {
            if (!patched.Add(id)) return;
            Patched?.Invoke(id);
        }

        /// <summary>Slot load (patches persist; uses are per attempt and start from zero).</summary>
        public void Import(IEnumerable<string> patchedIds)
        {
            patched.Clear();
            uses.Clear();
            foreach (var id in patchedIds) patched.Add(id);
        }

        public List<string> Export() => new List<string>(patched);
    }
}
