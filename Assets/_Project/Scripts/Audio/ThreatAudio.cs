using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Audio
{
    /// <summary>
    /// Lukas's heartbeat (S-11, M-10): a looping cue whose tempo and volume follow the band of the nearest living enemy.
    /// Enemies behind a sealed door do not count. Added to the Bootstrap object by <see cref="AudioService"/>.
    /// Logs "[Heartbeat][Audio] Heartbeat band=N" when the band changes.
    /// </summary>
    public sealed class ThreatAudio : MonoBehaviour
    {
        private AudioService audioService;
        private AudioBalance balance;
        private AudioCue cue;
        private AudioSource voice;
        private int band = -1;
        private float nextScan;
        private readonly RaycastHit[] hits = new RaycastHit[16];

        public int Band => band;

        public void Init(AudioService service, AudioBalance b)
        {
            audioService = service;
            balance = b;
            service.TryGetCue("CUE_Heartbeat_Player", out cue);
        }

        private void Update()
        {
            if (balance == null || Time.unscaledTime < nextScan) return;
            nextScan = Time.unscaledTime + balance.scanInterval;

            float nearest = float.MaxValue;
            if (Services.TryGet(out PlayerController player) && Services.TryGet(out AlertDirector director))
            {
                Vector3 from = player.transform.position + Vector3.up * 1.2f;
                foreach (var e in director.Enemies)
                {
                    if (e == null || e.IsDead) continue;
                    float d = Vector3.Distance(player.transform.position, e.transform.position);
                    if (d < nearest && d <= balance.threatBands[0] && !BehindSealedDoor(from, e.transform.position + Vector3.up * 1.2f))
                        nearest = d;
                }
            }
            SetBand(HeartbeatModel.ThreatBand(nearest, balance.threatBands));
        }

        private bool BehindSealedDoor(Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            int n = Physics.RaycastNonAlloc(from, dir.normalized, hits, dir.magnitude, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < n; i++)
            {
                var door = hits[i].collider.GetComponentInParent<DoorLeaf>();
                if (door != null && door.State == DoorState.Sealed) return true;
            }
            return false;
        }

        private void SetBand(int b)
        {
            if (b == band) return;
            band = b;
            HbLog.Info("Audio", $"Heartbeat band={b}");
            if (b < 0)
            {
                audioService.Stop(voice);
                voice = null;
                return;
            }
            if (voice == null) voice = audioService.StartLoop(cue);
            if (voice == null) return;
            voice.pitch = balance.bandBpm[b] / balance.clipBpm;
            voice.volume = cue.volume * balance.bandVolume[b];
        }

        private void OnDisable() { SetBand(-1); }
    }
}
