#if UNITY_EDITOR
using System.Collections;
using Heartbeat.Boss;
using Heartbeat.Core;
using Heartbeat.Cycle;
using Heartbeat.Narrative;
using Heartbeat.Save;
using UnityEditor;
using UnityEngine;

namespace Heartbeat.DebugTools
{
    /// <summary>
    /// Editor only: plays the cinematic picked in the gallery panel through the real game services, on the hidden dev
    /// slot so the player's saves are never touched, then returns to the gallery.
    /// </summary>
    public sealed class CinematicPreview : MonoBehaviour
    {
        private const string Request = "Heartbeat.Sandbox.Cinematic", ReturnToGallery = "Heartbeat.Sandbox.ReturnToGallery";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Begin()
        {
            string id = SessionState.GetString(Request, "");
            if (id == "") return;
            SessionState.EraseString(Request);
            var go = new GameObject("CinematicPreview");
            DontDestroyOnLoad(go);
            var preview = go.AddComponent<CinematicPreview>();
            preview.StartCoroutine(preview.Run(id));
        }

        private IEnumerator Run(string id)
        {
            while (!Bootstrap.IsReady) yield return null;
            var states = Services.Get<GameStateMachine>();
            var loader = Services.Get<SceneLoader>();
            var cycle = Services.Get<CycleController>();
            while (states.Current == GameStateId.Boot || loader.IsBusy) yield return null;
            if (states.Current == GameStateId.BrightnessCalibration) states.TryTransition(GameStateId.Title);
            states.TryTransition(GameStateId.MainMenu);

            int floor = id == "Floor2Photo" ? 2 : id == "Awakening" ? FloorBuilderMother : 1;
            bool intro = id == "Intro";
            if (!intro)
                Services.Get<SaveService>().Save(CycleController.DevSlot, new SlotData
                {
                    introDone = true,
                    firstAttempt = false,
                    hasFloorStart = floor > 1,
                    floorStart = new AttemptSave { floor = floor },
                });
            cycle.StartFromMenu(CycleController.DevSlot, GameMode.Survival, newGame: intro);
            HbLog.Info("Preview", $"Cinematic {id}");

            if (intro)
            {
                while (states.Current != GameStateId.Playing) yield return null;
            }
            else
            {
                while (states.Current != GameStateId.Playing) yield return null;
                yield return Play(id, states, cycle);
            }
            SessionState.SetBool(ReturnToGallery, true);
            EditorApplication.isPlaying = false;
        }

        private const int FloorBuilderMother = Generation.FloorBuilder.MotherFloor;

        private static IEnumerator Play(string id, GameStateMachine states, CycleController cycle)
        {
            switch (id)
            {
                case "Reconstruction":
                    yield return new WaitForSeconds(1f);
                    cycle.DebugKillPlayer();
                    while (states.Current == GameStateId.Playing) yield return null;
                    while (states.Current != GameStateId.Playing) yield return null;
                    yield return new WaitForSeconds(1f);
                    break;
                case "Floor2Photo":
                    Floor2Photo photo = null;
                    for (float t = 0f; t < 5f && !Services.TryGet(out photo); t += Time.unscaledDeltaTime) yield return null;
                    if (photo == null) { HbLog.Warn("Preview", "No Floor2Photo on this floor"); break; }
                    yield return photo.Play();
                    break;
                case "Awakening":
                    MotherDirector mother = null;
                    while (!Services.TryGet(out mother)) yield return null;
                    yield return new WaitForSeconds(3f);   // the fight has started
                    mother.DebugDefeat();
                    while (states.Current == GameStateId.Playing || states.Current == GameStateId.Victory) yield return null;
                    break;
                default:
                    if (id == "Assimilation") states.TryTransition(GameStateId.Death);
                    else states.TryTransition(GameStateId.Victory);
                    if (id == "FullRelief" || id == "Ambiguous") states.TryTransition(GameStateId.FinalDecision);
                    states.TryTransition(id == "Assimilation" ? GameStateId.EndingAssimilation
                        : id == "BasicRelief" ? GameStateId.EndingBasicRelief
                        : id == "FullRelief" ? GameStateId.EndingFullRelief : GameStateId.EndingAmbiguous);
                    GameEvents.RaiseEndingTriggered(id);
                    var ending = FindAnyObjectByType<EndingDirector>();
                    while (!ending.IsRunning) yield return null;
                    while (ending.IsRunning) yield return null;
                    break;
            }
        }
    }
}
#endif
