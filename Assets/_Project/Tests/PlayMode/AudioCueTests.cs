using System.Collections;
using Heartbeat.Audio;
using Heartbeat.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>C-57: long one-shots can be faded out, and the clips fit the events they play under.</summary>
    public sealed class AudioCueTests
    {
        [UnityTest]
        public IEnumerator SfxStop_FadesOutTheCue_AndClipsFitTheirEvents()
        {
            yield return SceneManager.LoadSceneAsync("Floor", LoadSceneMode.Single);
            yield return null;
            var audio = Services.Get<AudioService>();

            Assert.IsTrue(audio.TryGetCue("CUE_Elevator_Hum", out var hum));
            Assert.IsFalse(hum.loop, "the hum is a one-shot (P-42)");
            Assert.LessOrEqual(hum.clips[0].length, SystemsBalance.Current.floorTransitionSeconds + 0.01f, "the hum ends with the ride");
            foreach (string step in new[] { "CUE_Step_Metal", "CUE_Step_Water", "CUE_Step_Debris" })
            {
                Assert.IsTrue(audio.TryGetCue(step, out var s));
                Assert.LessOrEqual(s.clips[0].length, 0.401f, step);
            }
            foreach (string amb in new[] { "CUE_Amb_F1", "CUE_Amb_F2", "CUE_Amb_F3", "CUE_Amb_Mother" })
            {
                Assert.IsTrue(audio.TryGetCue(amb, out var a));
                Assert.AreEqual(AudioClipLoadType.CompressedInMemory, a.clips[0].loadType, amb);
            }

            var voice = audio.Play(hum);
            yield return null;
            Assert.IsTrue(voice.isPlaying);
            Sfx.Stop("CUE_Elevator_Hum");
            for (float t = 0f; t < 0.5f; t += Time.unscaledDeltaTime) yield return null;
            Assert.IsFalse(voice.isPlaying, "Sfx.Stop fades the cue out and frees its voice");

            // C-67: the reconstruction event plays nothing; the shot that shows the capsule plays the cue once and stops it.
            Assert.IsTrue(audio.TryGetCue("CUE_Capsule_Reconstruction", out var capsule));
            int before = Voices(capsule.clips[0]);
            GameEvents.RaiseReconstructed(1);
            yield return null;
            Assert.AreEqual(before, Voices(capsule.clips[0]), "Reconstructed starts no capsule voice");
        }

        private static int Voices(AudioClip clip)
        {
            int n = 0;
            foreach (var s in Object.FindObjectsByType<AudioSource>())
                if (s.isPlaying && s.clip == clip) n++;
            return n;
        }
    }
}
