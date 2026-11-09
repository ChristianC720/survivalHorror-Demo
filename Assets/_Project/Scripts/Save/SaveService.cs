using System;
using System.IO;
using Heartbeat.Core;
using UnityEngine;

namespace Heartbeat.Save
{
    public enum SlotStatus { Empty, Ok, Damaged }

    /// <summary>
    /// JSON saves with JsonUtility (D-109): global.json + slot_0..2.json under persistentDataPath/saves.
    /// Atomic writes: .tmp then File.Replace keeping .bak; a failure never leaves a slot corrupt. Versioned.
    /// </summary>
    public sealed class SaveService
    {
        public const int CurrentVersion = 1;
        public const int SlotCount = 3;

        private readonly string dir;

        public SaveService(string directory = null)
        {
            dir = directory ?? Path.Combine(Application.persistentDataPath, "saves");
            Directory.CreateDirectory(dir);
        }

        public string Directory_ => dir;
        private string SlotPath(int slot) => Path.Combine(dir, $"slot_{slot}.json");
        private string GlobalPath => Path.Combine(dir, "global.json");

        // ---------- Slots ----------

        public SlotStatus Status(int slot)
        {
            if (!File.Exists(SlotPath(slot))) return SlotStatus.Empty;
            return TryRead(SlotPath(slot), out SlotData d) && d.saveVersion <= CurrentVersion ? SlotStatus.Ok : SlotStatus.Damaged;
        }

        public SlotData Load(int slot)
        {
            if (!TryRead(SlotPath(slot), out SlotData d)) return null;
            if (d.saveVersion > CurrentVersion) return null;   // newer than this build: treat as damaged
            if (d.saveVersion < CurrentVersion) { Migrate(d); Save(slot, d); }
            return d;
        }

        public void Save(int slot, SlotData data)
        {
            data.saveVersion = CurrentVersion;
            if (string.IsNullOrEmpty(data.createdAt)) data.createdAt = DateTime.UtcNow.ToString("o");
            data.updatedAt = DateTime.UtcNow.ToString("o");
            if (!WriteAtomic(SlotPath(slot), JsonUtility.ToJson(data, true))) return;
            HbLog.Info("Save", $"Slot {slot} saved ({data.SubjectLabel}, deaths {data.deathCounter}, autosave={data.hasAttemptSave})");
        }

        public void Delete(int slot)
        {
            foreach (var p in new[] { SlotPath(slot), SlotPath(slot) + ".bak", SlotPath(slot) + ".tmp" })
                if (File.Exists(p)) File.Delete(p);
        }

        /// <summary>Restores the .bak of a damaged slot. Returns false if there is none or it is unreadable.</summary>
        public bool RestoreBackup(int slot)
        {
            string bak = SlotPath(slot) + ".bak";
            if (!TryRead(bak, out SlotData _)) return false;
            File.Copy(bak, SlotPath(slot), true);
            return true;
        }

        // ---------- Global ----------

        public GlobalData LoadGlobal() => TryRead(GlobalPath, out GlobalData g) ? g : new GlobalData();

        public void SaveGlobal(GlobalData g)
        {
            g.saveVersion = CurrentVersion;
            WriteAtomic(GlobalPath, JsonUtility.ToJson(g, true));
        }

        // ---------- IO ----------

        /// <summary>Writes .tmp then swaps it in, so a failure never corrupts the previous save. On failure the old file
        /// stays, the terminal shows SYNC FAILED and the next change retries (Guardado y Configuración).</summary>
        private static bool WriteAtomic(string path, string json)
        {
            try
            {
                string tmp = path + ".tmp";
                File.WriteAllText(tmp, json);
                if (File.Exists(path)) File.Replace(tmp, path, path + ".bak");
                else File.Move(tmp, path);
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                HbLog.Error("Save", $"SYNC FAILED writing {Path.GetFileName(path)}: {e.Message}");
                GameEvents.RaiseSyncFailed();
                return false;
            }
        }

        private static bool TryRead<T>(string path, out T data) where T : class
        {
            data = null;
            if (!File.Exists(path)) return false;
            try
            {
                data = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return data != null;
            }
            catch (Exception e)
            {
                HbLog.Warn("Save", $"Unreadable save {Path.GetFileName(path)}: {e.Message}");
                return false;
            }
        }

        /// <summary>vN → vN+1 migrations, in order. None yet (version 1).</summary>
        private static void Migrate(SlotData d) { }
    }
}
