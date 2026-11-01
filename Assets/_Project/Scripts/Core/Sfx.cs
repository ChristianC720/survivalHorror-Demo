using UnityEngine;

namespace Heartbeat.Core
{
    /// <summary>
    /// Fire-and-forget sound request for gameplay code (S-11). Gameplay never references clips or the audio service:
    /// it names a cue and the AudioService (listening on <see cref="GameEvents.CueRequested"/>) plays it.
    /// Cue ids are the asset names in ScriptableObjects/Audio (registry: Desarrollo/AUDIO_CUES.md).
    /// </summary>
    public static class Sfx
    {
        public const string RevolverReloadRound = "CUE_Revolver_ReloadRound";
        public const string RevolverDryFire = "CUE_Revolver_DryFire";
        public const string Shove = "CUE_Shove";
        public const string SpikeDischarge = "CUE_Spike_Discharge";
        public const string HackInput = "CUE_Hack_Input";
        public const string HackError = "CUE_Hack_Error";
        public const string HackSuccess = "CUE_Hack_Success";
        public const string PowerStartup = "CUE_Power_Startup";
        public const string DoorOpen = "CUE_Door_Open";
        public const string DoorLocked = "CUE_Door_Locked";
        public const string DoorSeal = "CUE_Door_Seal";
        public const string DoorSealBreak = "CUE_Door_SealBreak";
        public const string Pickup = "CUE_Pickup";
        public const string HealUse = "CUE_Heal_Use";
        public const string FlashlightToggle = "CUE_Flashlight_Toggle";
        public const string FlashlightFlicker = "CUE_Flashlight_Flicker";
        public const string TerminalOn = "CUE_Terminal_On";
        public const string TransitLockdown = "CUE_Transit_Lockdown";

        public static void Play(string cue, Vector3 position) => GameEvents.RaiseCueRequested(cue, position);
        public static void Play2D(string cue) => GameEvents.RaiseCueRequested(cue, null);
    }
}
