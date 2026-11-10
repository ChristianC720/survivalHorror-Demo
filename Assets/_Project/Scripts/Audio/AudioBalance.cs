using UnityEngine;

namespace Heartbeat.Audio
{
    /// <summary>S-11 balance values. Initial, subject to balance (GDD: "nivel base y curva de crecimiento: sujetos a balance").</summary>
    [CreateAssetMenu(menuName = "Heartbeat/Balance/Audio", fileName = "AudioBalance")]
    public sealed class AudioBalance : ScriptableObject
    {
        [Header("Player heartbeat (S-11: approximate, never a radar)")]
        [Tooltip("Band edges in metres, far to near. Threats beyond the first edge: no heartbeat.")]
        public float[] threatBands = { 15f, 8f, 4f };
        [Tooltip("Beats per minute for each band (same length as threatBands).")]
        public float[] bandBpm = { 80f, 110f, 140f };
        [Tooltip("Volume for each band.")]
        public float[] bandVolume = { 0.45f, 0.7f, 1f };
        [Tooltip("Tempo the heartbeat clips are authored at; pitch = bpm / clipBpm.")]
        public float clipBpm = 60f;
        [Tooltip("Seconds between threat scans.")]
        public float scanInterval = 0.25f;

        [Header("Lab heartbeat (D-46, D-95, M-18)")]
        public float labBaseVolume = 0.3f;
        public float labVolumePerDegradation = 0.08f;
        [Tooltip("Cap: the lab heartbeat never covers the M-10 signals (D-29).")]
        public float labMaxVolume = 0.7f;
        public float labCriticalBonus = 0.15f;
        public float labBaseBpm = 48f;
        public float labBpmPerDegradation = 4f;
        public float labCriticalBpmBonus = 10f;

        [Header("Music")]
        [Tooltip("Fade-in and fade-out of every music cue, in seconds.")]
        public float musicFadeSeconds = 2f;
        [Tooltip("Silence between the end of an ending's music, carried into the main menu, and the menu theme.")]
        public float menuMusicDelayAfterEnding = 2f;

        [Header("Occlusion (S-11): share of the sound that gets through")]
        [Tooltip("Per wall between source and listener (−70% → 0.3). Also scales the noise radius enemies perceive.")]
        [Range(0f, 1f)] public float wallTransmission = 0.3f;
        [Tooltip("Per closed or sealed door (−50% → 0.5). Open doors do not occlude.")]
        [Range(0f, 1f)] public float doorTransmission = 0.5f;
        [Tooltip("Seconds between occlusion updates of playing positional sounds.")]
        public float occlusionInterval = 0.2f;

        [Header("Footsteps")]
        [Tooltip("Metres travelled per step sound.")] public float strideLength = 0.8f;
        [Tooltip("Below this speed (m/s) steps are silent (crouch).")] public float minStepSpeed = 1.0f;
    }
}
