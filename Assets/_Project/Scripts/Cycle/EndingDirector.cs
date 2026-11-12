using System.Collections;
using Heartbeat.Core;
using Heartbeat.Narrative;
using Heartbeat.Save;
using Heartbeat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.Cycle
{
    /// <summary>
    /// Endings (Premisa y Trama, D-42, D-51, D-61): plays the ending as a sequence of black cards with text, subtitles
    /// and cues, then the credits (skippable with Space only once seen, D-39), marks the cycle completed (D-93) and
    /// returns to the main menu. Lives on the Bootstrap object; started by GameEvents.EndingTriggered.
    /// shortcut: endings are mostly code-driven card sequences over black; only the Assimilation has a staged 3D shot (the nonexistent floor
    /// with Lukas as a Mudo, Cinematics scene); add more <see cref="Narrative.CinematicStage"/> shots for the others.
    /// </summary>
    public sealed class EndingDirector : MonoBehaviour
    {
        [SerializeField] private float cardSeconds = 3.5f;
        [SerializeField] private float creditsSeconds = 12f;

        private Canvas canvas;
        private TMP_Text card, small;
        private bool skip, running;
        private AudioSource music;

        // Cut points in seconds of the rendered clips (songs/latido and songs/emilia-tema in reaper-daemon).
        private const float Bar = 3.75f;   // Emilia's theme: 4/4 at 64 BPM
        private const float FlashSeconds = 0.2f, AssimReportAt = 16f, AssimRootFallsAt = 28f, AssimNameAt = 42f, AssimStateAt = 45f, AssimEnd = 47f;
        private static readonly float[] ArrhythmiaBeats = { 0f, 0.5f, 1f, 1.3f, 2.2f, 2.7f, 2.95f, 3.2f, 4.3f, 4.8f, 5.2f, 6.6f, 6.9f, 7.5f, 9.3f, 9.8f, 11.6f };

        public bool IsRunning => running;

        private void OnEnable() => GameEvents.EndingTriggered += OnEnding;
        private void OnDisable() => GameEvents.EndingTriggered -= OnEnding;

        private void OnEnding(string id)
        {
            if (running) return;
            StartCoroutine(Run(id));
        }

        private IEnumerator Run(string id)
        {
            running = true;
            Build();
            var cycle = Services.Get<CycleController>();
            var data = cycle.Data;
            string n = data.subjectCode.ToString("00");
            var router = Services.Get<InputRouting.InputRouter>();
            router.Actions.Cinematic.Skip.performed += OnSkip;
            HbLog.Info("Ending", $"Start {id} ({data.SubjectLabel})");

            yield return Narrative.SkippableCinematic.Run(this, id, Sequence(id, n), OnSequenceSkipped);   // D-91

            // Cycle completed (D-93): the next Continue starts a new cycle from the capsule.
            data.cycleCompleted = true;
            data.newCyclePending = true;
            cycle.SaveNow();

            var states = Services.Get<GameStateMachine>();
            states.TryTransition(GameStateId.Credits);
            yield return Credits();

            router.Actions.Cinematic.Skip.performed -= OnSkip;
            Destroy(canvas.gameObject);
            running = false;
            // The ending's music carries on into the main menu until it ends or an action cuts it; the menu theme waits.
            if (Services.TryGet(out Audio.AudioService carry)) carry.CarryMusic(music);
            music = null;
            states.TryTransition(GameStateId.MainMenu);
            Services.Get<SceneLoader>().Load(SceneNames.MainMenu);
            HbLog.Info("Ending", $"{id} done -> main menu");
        }

        private IEnumerator Sequence(string id, string n)
        {
            switch (id)
            {
                case "Assimilation":
                    PlayMusic("CUE_Music_Ending_Assimilation");
                    // 1. Distorted flashes of the capsule, each one on a beat of the arrhythmia (D-53).
                    foreach (float beat in ArrhythmiaBeats)
                    {
                        yield return CardUntil("", beat);
                        Sfx.Stop("CUE_Capsule_Reconstruction");   // one flash, one hit: 17 stacked 9 s clips would fill the voice pool
                        Sfx.Play2D("CUE_Capsule_Reconstruction");
                        yield return CardUntil(DocText.Render("<c>" + Loc.Get("END.ASSIM.CAPSULE", n) + "</c>", false), beat + FlashSeconds);
                    }
                    Sfx.Stop("CUE_Capsule_Reconstruction");   // silence for the stopped heart and The Mother's report
                    yield return CardUntil("", AssimReportAt);   // the heart stops: 2 s of nothing
                    // 2. The Mother's reclassification report (D-61).
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.ASSIMILATION", "R-H-" + n);
                    card.text = Loc.Get("END.ASSIM.FLOOR");
                    // 3. Lukas, now a Mudo, far away on the floor that does not exist; only here his text degrades (D-62).
                    //    Staged shot in the Cinematics scene: the only time that floor comes back (D-61).
                    var loader = Services.Get<SceneLoader>();
                    loader.Load(SceneNames.Cinematics, null, fadeOut: false);
                    while (loader.IsBusy) yield return null;
                    loader.SetFade(0f);
                    if (Services.TryGet(out Narrative.CinematicStage stage))
                    {
                        yield return CardUntil(card.text, AssimRootFallsAt);   // the root sinks to The Mother's key under the cut
                        yield return StageUntil(stage, "Assimilation", AssimNameAt);
                    }
                    yield return CardUntil(DocText.Render("<c>" + Loc.Get("SUB.LUKAS.NAME_OWN") + "</c>", false), AssimStateAt);
                    yield return CardUntil(Loc.Get("END.ASSIM.STATE"), AssimEnd);
                    break;
                default:   // BasicRelief, FullRelief, Ambiguous (after The Mother, E09)
                    PlayMusic("CUE_Music_Ending_" + id);
                    card.text = Loc.Get("END.RELIEF.WAKE");
                    var sets = Services.Get<SceneLoader>();
                    sets.Load(SceneNames.Cinematics, null, fadeOut: false);   // behind the card
                    while (sets.IsBusy) yield return null;
                    Services.TryGet(out Narrative.CinematicStage set);
                    Services.Get<SceneLoader>().SetFade(0f);
                    yield return CardUntil(card.text, Bar * (id == "FullRelief" ? 1 : 2));
                    if (id == "BasicRelief")
                    {
                        yield return StageUntil(set, "Relief_Wide", Bar * 4);
                        yield return StageUntil(set, "Relief_Close", Bar * 6);
                        yield return CardUntil(Loc.Get("END.RELIEF.BASIC"), Bar * 8);
                    }
                    if (id == "FullRelief")
                    {
                        yield return StageUntil(set, "Relief_Wide", Bar * 3);
                        yield return StageUntil(set, "Relief_Close", Bar * 5);
                        yield return CardUntil(Loc.Get("END.RELIEF.FULL"), Bar * 6);
                        // He hears the second act: on screen only total glitch, never readable (D-61, D-16).
                        Sfx.Play2D("CUE_Ending_BeatLab");
                        yield return CardUntil(DocText.Render("<c>" + Loc.Get("END.RELIEF.BASIC") + "</c>", false), Bar * 8);
                    }
                    if (id == "Ambiguous")
                    {
                        yield return StageUntil(set, "Ambiguous_Wide", Bar * 5);
                        yield return StageUntil(set, "Ambiguous_Close", Bar * 8);
                        // In black: Lukas's heartbeat and the lab's, out of phase, closing in until they beat as one (D-61).
                        card.text = "";
                        StopMusic();   // only the two heartbeats, then silence (D-61)
                        for (float gap = 0.6f; gap > 0.02f; gap *= 0.6f)
                        {
                            Sfx.Play2D("CUE_Ending_BeatLukas");
                            for (float t = 0f; t < gap; t += Time.unscaledDeltaTime) yield return null;
                            Sfx.Play2D("CUE_Ending_BeatLab");
                            for (float t = 0f; t < 1.1f - gap; t += Time.unscaledDeltaTime) yield return null;
                        }
                        yield return Card("", 2f);   // silence before the hook
                    }
                    // Open hook: a new arrival and The Mother starting the protocol (D-51).
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.HOOK");
                    yield return Card("", cardSeconds);
                    break;
            }
        }

        /// <summary>Skipped with a hold (D-91): leave the staged shot and the music, go straight to the credits.</summary>
        private void OnSequenceSkipped()
        {
            if (Services.TryGet(out Narrative.CinematicStage stage)) stage.StopAll();
            canvas.enabled = true;
            card.text = "";
            StopMusic();
        }

        private void PlayMusic(string cue)
        {
            if (Services.TryGet(out Audio.AudioService audio) && audio.TryGetCue(cue, out var c)) music = audio.StartMusic(c, music);
        }

        private void StopMusic()
        {
            if (Services.TryGet(out Audio.AudioService audio)) audio.FadeOutMusic(music);
            music = null;
        }

        private IEnumerator Credits()
        {
            var saves = Services.Get<SaveService>();
            var g = saves.LoadGlobal();
            bool canSkip = g.creditsSeen;
            small.text = canSkip ? Loc.Get("END.CREDITS.SKIP") : "";
            card.fontSize = 30;
            HbLog.Info("Ending", $"Credits (skippable={canSkip})");
            yield return Card(Loc.Get("END.CREDITS.TITLE") + "\n\n" + Loc.Get("END.CREDITS.ROLL"), creditsSeconds, canSkip);
            g.creditsSeen = true;
            saves.SaveGlobal(g);
        }

        /// <summary>Holds a card until the music reaches <paramref name="musicTime"/>, so the cut lands on the beat.</summary>
        private IEnumerator CardUntil(string text, float musicTime)
        {
            card.text = text;
            while (MusicTime < musicTime && music != null && music.isPlaying) yield return null;
        }

        private IEnumerator StageUntil(Narrative.CinematicStage stage, string shot, float musicTime)
        {
            float seconds = musicTime - MusicTime;
            if (stage == null || seconds <= 0f) yield break;
            card.text = "";
            canvas.enabled = false;
            yield return stage.Play(shot, seconds);
            canvas.enabled = true;
        }

        private float MusicTime => music != null ? music.time : float.MaxValue;

        private IEnumerator Card(string text, float seconds, bool skippable = false)
        {
            card.text = text;
            skip = false;
            for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime)
            {
                if (skippable && skip) break;
                yield return null;
            }
        }

        private void OnSkip(InputAction.CallbackContext _) => skip = true;

        private void Build()
        {
            var go = new GameObject("EndingCanvas");
            go.transform.SetParent(transform, false);
            canvas = UiKit.Canvas(go, 5);     // under the HUD (10) so subtitles stay readable, under the scene fade (1000)
            UiKit.Stretch(UiKit.Rect("Black", go.transform), Vector2.zero, Vector2.one).gameObject.AddComponent<Image>().color = Color.black;
            card = UiKit.Text(go.transform, "", 40, UiKit.Teal);
            card.alignment = TextAlignmentOptions.Center;
            card.textWrappingMode = TextWrappingModes.Normal;
            UiKit.Stretch((RectTransform)card.transform, new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.8f));
            small = UiKit.Text(go.transform, "", 24, UiKit.TealDim);
            small.alignment = TextAlignmentOptions.BottomRight;
            UiKit.Stretch((RectTransform)small.transform, new Vector2(0.6f, 0.03f), new Vector2(0.97f, 0.1f));
            Services.Get<SceneLoader>().SetFade(0f);
        }
    }
}
