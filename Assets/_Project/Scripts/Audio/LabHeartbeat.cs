using Heartbeat.Core;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Audio
{
    /// <summary>
    /// Floor ambience layer (S-11): the floor drone (CUE_Amb_F1..F3) plus the lab heartbeat (D-46, D-95), which never
    /// reacts to enemies — only to degradation (M-18) and to Lukas being Critical. Starts on FloorBuilt, stops when the
    /// game leaves the floor states. Added to the Bootstrap object by <see cref="AudioService"/>.
    /// </summary>
    public sealed class LabHeartbeat : MonoBehaviour
    {
        private AudioService audioService;
        private AudioBalance balance;
        private AudioSource drone, lab;
        private AudioCue labCue;
        private int degradation;
        private bool critical;

        public void Init(AudioService service, AudioBalance b)
        {
            audioService = service;
            balance = b;
            service.TryGetCue("CUE_Heartbeat_Lab", out labCue);
        }

        private void OnEnable()
        {
            GameEvents.FloorBuilt += OnFloorBuilt;
            GameEvents.GameStateChanged += OnState;
            GameEvents.DegradationChanged += OnDegradation;
            GameEvents.HealthStateChanged += OnHealth;
        }

        private void OnDisable()
        {
            GameEvents.FloorBuilt -= OnFloorBuilt;
            GameEvents.GameStateChanged -= OnState;
            GameEvents.DegradationChanged -= OnDegradation;
            GameEvents.HealthStateChanged -= OnHealth;
            StopAll();
        }

        private void OnFloorBuilt(int floor, int seed)
        {
            StopAll();
            if (Services.TryGet(out Cycle.CycleController cycle) && cycle.Data != null) degradation = cycle.Data.degradationLevel;
            critical = false;
            if (audioService.TryGetCue($"CUE_Amb_F{floor}", out var amb)) drone = audioService.StartLoop(amb);
            lab = audioService.StartLoop(labCue);
            Apply();
        }

        private void OnState(GameStateId from, GameStateId to)
        {
            bool onFloor = to == GameStateId.Playing || to == GameStateId.Paused || to == GameStateId.Reconstruction
                || to == GameStateId.FloorTransition;
            if (!onFloor) StopAll();
        }

        private void OnDegradation(int level) { degradation = level; Apply(); }
        private void OnHealth(int state) { critical = state == (int)HealthState.Critical; Apply(); }

        private void Apply()
        {
            if (lab == null) return;
            lab.volume = labCue.volume * HeartbeatModel.LabVolume(balance, degradation, critical);
            lab.pitch = HeartbeatModel.LabBpm(balance, degradation, critical) / balance.clipBpm;
            HbLog.Dev("Audio", $"Lab heartbeat degradation={degradation} critical={critical}");
        }

        private void StopAll()
        {
            if (audioService == null) return;
            audioService.Stop(drone);
            audioService.Stop(lab);
            drone = lab = null;
        }
    }
}
