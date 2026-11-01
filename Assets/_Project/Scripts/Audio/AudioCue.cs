using UnityEngine;
using UnityEngine.Audio;

namespace Heartbeat.Audio
{
    /// <summary>Lower value = more important. Signals are never stolen by anything else (M-10).</summary>
    public enum CuePriority { Signal = 1, Combat = 2, Ambience = 3, Music = 4 }

    /// <summary>
    /// One sound (S-11). Clips may be empty: the cue still "plays" (logs, raises CuePlayed, shows its debug caption
    /// in development builds), so the final audio drops in without code changes. Registry: Desarrollo/AUDIO_CUES.md.
    /// </summary>
    [CreateAssetMenu(menuName = "Heartbeat/Audio Cue", fileName = "CUE_")]
    public sealed class AudioCue : ScriptableObject
    {
        public AudioClip[] clips = new AudioClip[0];
        public AudioMixerGroup bus;
        public CuePriority priority = CuePriority.Combat;
        public bool spatial = true;
        [Range(0f, 1f)] public float volume = 1f;
        public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
        public float maxDistance = 20f;
        public bool loop;
        [Tooltip("Shown as a corner caption in development builds while the cue has no clip.")]
        public string debugCaption;

        public AudioClip PickClip() => clips == null || clips.Length == 0 ? null : clips[Random.Range(0, clips.Length)];
    }
}
