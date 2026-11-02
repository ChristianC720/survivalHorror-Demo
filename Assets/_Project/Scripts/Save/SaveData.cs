using System;
using System.Collections.Generic;
using Heartbeat.Inventory;
using Heartbeat.Narrative;

namespace Heartbeat.Save
{
    public enum GameMode { Survival, Story }
    public enum ExitCost { None, Calm, Hunt }

    /// <summary>global.json (GDD 07 - Tecnología/04 - Datos y Persistencia).</summary>
    [Serializable]
    public sealed class GlobalData
    {
        public int saveVersion = SaveService.CurrentVersion;
        public int lastUsedSlot;
        public bool brightnessCalibrated;
        public float brightness = 1f;
        public string language = "en";
        public string bindingOverrides = "";
        public float volMaster = 1f, volMusic = 0.8f, volSfx = 1f, volAmbience = 1f, volVoices = 1f;
        public bool subtitles = true;          // shortcut: unused, subtitles are mandatory (D-52); kept for save compatibility
        public bool subtitleOpaque;
        public bool soundCaptions;             // D-102, off by default
        public float textScale = 1f;
        public bool dyslexiaFont;
        public bool crouchToggle = true;
        public bool invertY;
        public float lookSensitivity = 1f, aimSensitivity = 1f;
        public bool aimAssist;
        public List<string> seenCinematics = new List<string>();
        public bool creditsSeen;
    }

    [Serializable]
    public struct RecordEntry
    {
        public string id;
        public RecordKind kind;
    }

    /// <summary>Autosave of the current attempt (D-30): floor and the inventory Lukas brings.</summary>
    [Serializable]
    public sealed class AttemptSave
    {
        public int floor = 1;
        public List<InventorySlot> inventory = new List<InventorySlot>();
        public int revolverLoaded;
        public bool revolverOwned = true;   // D-74; true keeps saves written before the field armed
        public float health = -1f;   // -1 = full
    }

    /// <summary>slot_N.json. Floor layouts are never saved: continuing generates a new seed (D-101).</summary>
    [Serializable]
    public sealed class SlotData
    {
        public int saveVersion = SaveService.CurrentVersion;
        public string createdAt;
        public string updatedAt;
        public GameMode mode = GameMode.Survival;
        public bool showcase;
        public int subjectCode = 7;            // R-H-07
        public int deathCounter;               // consecutive, for the Assimilation ending (M-18)
        public int totalDeaths;
        public int degradationLevel;
        public bool firstAttempt = true;       // tutorial attempt (D-38, D-70)
        public bool introDone;
        public bool floor2PhotoSeen;         // the floor-2 finding plays once (D-56)
        public bool cycleCompleted;            // D-93
        public bool newCyclePending;           // an ending was reached: the next Continue starts a new cycle (D-93)
        public List<RecordEntry> terminal = new List<RecordEntry>();
        public List<string> patchedFaults = new List<string>();
        public bool hasAttemptSave;            // JsonUtility cannot serialise null objects
        public AttemptSave attemptSave = new AttemptSave();
        // Start of the current floor (D-147): where a death or a safe-zone restart returns, with what Lukas arrived with.
        public bool hasFloorStart;
        public AttemptSave floorStart = new AttemptSave();
        public bool confirmedExit;
        public ExitCost exitCost;

        public string SubjectLabel => $"R-H-{subjectCode:00}";
    }
}
