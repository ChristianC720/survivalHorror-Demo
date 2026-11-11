using System;
using System.Collections.Generic;

namespace Heartbeat.Narrative
{
    /// <summary>Kinds of records in the arm terminal's Logs tab (M-08, M-16).</summary>
    /// <remarks>Saves store the numeric value: append new kinds at the end, never renumber.</remarks>
    public enum RecordKind { EmiliaDocument, ContextDocument, Confiscated, Bestiary, Rule, Fault, Ring, Poster, VoiceNote }

    /// <summary>
    /// Persistent knowledge (S-07): everything registered in the arm terminal. Survives deaths and Restart;
    /// only "New game" clears it (D-32). The 6 Emilia documents unlock the final decision (D-51).
    /// Pure logic, serialisable as a list of ids.
    /// </summary>
    public sealed class RegistryModel
    {
        public const int EmiliaTotal = 6;

        private readonly Dictionary<string, RecordKind> records = new Dictionary<string, RecordKind>();
        private readonly List<string> order = new List<string>();

        public event Action<string, RecordKind> Registered;

        public int Count => records.Count;
        public IReadOnlyList<string> Order => order;

        public bool Has(string id) => records.ContainsKey(id);

        public int CountOf(RecordKind kind)
        {
            int n = 0;
            foreach (var k in records.Values) if (k == kind) n++;
            return n;
        }

        public int EmiliaCount => CountOf(RecordKind.EmiliaDocument);
        public bool HasAllEmilia => EmiliaCount >= EmiliaTotal;

        /// <summary>Returns false if the id was already registered.</summary>
        public bool Register(string id, RecordKind kind)
        {
            if (string.IsNullOrEmpty(id) || records.ContainsKey(id)) return false;
            records[id] = kind;
            order.Add(id);
            Registered?.Invoke(id, kind);
            return true;
        }

        public RecordKind KindOf(string id) => records[id];

        public void Clear()
        {
            records.Clear();
            order.Clear();
        }

        public List<(string id, RecordKind kind)> Export()
        {
            var list = new List<(string, RecordKind)>();
            foreach (var id in order) list.Add((id, records[id]));
            return list;
        }

        public void Import(IEnumerable<(string id, RecordKind kind)> entries)
        {
            Clear();
            foreach (var (id, kind) in entries) { records[id] = kind; order.Add(id); }
        }
    }
}
