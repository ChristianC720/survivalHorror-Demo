using System.Collections;
using Heartbeat.Core;
using Heartbeat.Cycle;
using Heartbeat.Level;
using Heartbeat.Narrative;
using Heartbeat.Player;
using Heartbeat.Save;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Heartbeat.Tests
{
    /// <summary>
    /// C-65: the new intro runs from a new game to floor 1 with the player in control, watched in full and skipped.
    /// Both runs end in the same state: intro done, voice note and ring in the terminal, Lukas on floor 1 able to move.
    /// The global options file is restored afterwards (the skip test marks both cinematics as seen).
    /// </summary>
    public sealed class IntroFlowTests
    {
        [UnityTest, Timeout(240000)]
        public IEnumerator IntroWatchedInFull_EndsOnFloor1WithThePlayerInControl()
        {
            yield return Run(skip: false);
        }

        [UnityTest, Timeout(240000)]
        public IEnumerator IntroSkipped_EndsInTheSameState()
        {
            yield return Run(skip: true);
        }

        private static IEnumerator Run(bool skip)
        {
            yield return SceneManager.LoadSceneAsync("Intro", LoadSceneMode.Single);
            while (!Bootstrap.IsReady) yield return null;
            var saves = Services.Get<SaveService>();
            string original = JsonUtility.ToJson(saves.LoadGlobal());
            try
            {
                if (skip)
                {
                    var g = saves.LoadGlobal();
                    foreach (var id in new[] { "Intro_House", "Reconstruction" }) if (!g.seenCinematics.Contains(id)) g.seenCinematics.Add(id);
                    saves.SaveGlobal(g);
                }
                var director = Object.FindAnyObjectByType<IntroDirector>();
                Assert.IsNotNull(director, "IntroDirector in the Intro scene");

                if (skip) { yield return Until(() => director.Stage == "House", 10f); SkippableCinematic.DebugSkip = true; }
                yield return Until(() => director.Stage == "Exterior", skip ? 10f : 60f);
                Assert.IsFalse(SkippableCinematic.DebugSkip, "the house was skipped");
                yield return Seconds(3f);
                Object.FindAnyObjectByType<HackPanel>().OnHackSucceeded();
                yield return Until(() => director.Stage == "Elevator", 5f);
                director.DebugGoto("Elevator");
                var panel = GameObject.Find("ElevatorPanel").GetComponent<IntroElevatorPanel>();
                yield return Seconds(1.5f);
                panel.Interact(null);   // descend
                yield return Seconds(5.5f);
                panel.Interact(null);   // repair, then the fall
                yield return Until(() => director.Stage == "NonexistentFloor", 10f);
                yield return Seconds(1.5f);
                director.DebugGoto("Attack");
                yield return Until(() => director.Stage == "Death", 5f);
                if (skip) { yield return Seconds(1f); SkippableCinematic.DebugSkip = true; }
                else
                {
                    yield return Until(() => director.Stage == "Ring", 40f);
                    yield return Seconds(1.5f);
                    director.DebugTakeRing();
                }

                yield return Until(() => SceneManager.GetActiveScene().name == "Floor" && Services.Get<GameStateMachine>().Current == GameStateId.Playing, 60f);
                var cycle = Services.Get<CycleController>();
                Assert.IsTrue(cycle.Data.introDone, "intro done");
                Assert.AreEqual(1, cycle.CurrentFloor, "floor 1");
                var registry = Services.Get<RegistryModel>();
                Assert.IsTrue(registry.Has(CycleController.VoiceNoteId), "voice note copied to the terminal");
                Assert.IsTrue(registry.Has(CycleController.RingId), "ring registered");
                Assert.IsNull(HackView_Wallpaper(), "phone wallpaper cleared with the intro");
                // Floor loads additively before Intro unloads: take floor 1's player, not the intro's locked one.
                PlayerController player = null;
                yield return Until(() => (player = System.Array.Find(Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None), p => p.gameObject.scene.name == "Floor")) != null, 10f);
                yield return Until(() => !player.MovementLocked, 20f);   // the reconstruction card on floor 1
                Debug.Log($"[Heartbeat][Test] C-65 intro {(skip ? "skipped" : "watched")}: {cycle.Data.SubjectLabel} on floor 1, in control");
            }
            finally
            {
                SkippableCinematic.DebugSkip = false;
                saves.SaveGlobal(JsonUtility.FromJson<GlobalData>(original));
            }
        }

        private static Texture HackView_Wallpaper() => UI.HackView.PhoneWallpaper;

        private static IEnumerator Seconds(float s)
        {
            for (float t = 0f; t < s; t += Time.deltaTime) yield return null;
        }

        private static IEnumerator Until(System.Func<bool> done, float timeout)
        {
            float t = 0f;
            while (!done())
            {
                t += Time.unscaledDeltaTime;
                if (t > timeout) Assert.Fail($"timed out after {timeout} s");
                yield return null;
            }
        }
    }
}
