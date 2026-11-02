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

            switch (id)
            {
                case "Assimilation":
                    Sfx.Play2D("CUE_Music_Ending_Assimilation");
                    // 1. Distorted flashes of the capsule, edited like the reconstruction (D-53).
                    for (int i = 0; i < 4; i++)
                    {
                        Sfx.Play2D("CUE_Capsule_Reconstruction");
                        yield return Card(DocText.Render("<c>" + Loc.Get("END.ASSIM.CAPSULE", n) + "</c>", false), 0.35f);
                        yield return Card("", 0.25f);
                    }
                    // 2. The Mother's reclassification report (D-61).
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.ASSIMILATION", "R-H-" + n);
                    yield return Card(Loc.Get("END.ASSIM.FLOOR"), cardSeconds + 1.5f);
                    // 3. Lukas, now a Mudo, far away on the floor that does not exist; only here his text degrades (D-62).
                    //    Staged shot in the Cinematics scene: the only time that floor comes back (D-61).
                    var loader = Services.Get<SceneLoader>();
                    loader.Load(SceneNames.Cinematics, null, fadeOut: false);
                    while (loader.IsBusy) yield return null;
                    if (Services.TryGet(out Narrative.CinematicStage stage))
                    {
                        card.text = "";
                        canvas.enabled = false;
                        loader.SetFade(0f);
                        yield return stage.Play("Assimilation", cardSeconds + 2f);
                        canvas.enabled = true;
                    }
                    yield return Card(DocText.Render("<c>" + Loc.Get("SUB.LUKAS.NAME_OWN") + "</c>", false), cardSeconds);
                    yield return Card(Loc.Get("END.ASSIM.STATE"), cardSeconds);
                    break;
                default:   // BasicRelief, FullRelief, Ambiguous (after The Mother, E09)
                    Sfx.Play2D("CUE_Music_Ending_" + id);
                    yield return Card(Loc.Get("END.RELIEF.WAKE"), cardSeconds);
                    if (id == "BasicRelief") yield return Card(Loc.Get("END.RELIEF.BASIC"), cardSeconds);
                    if (id == "FullRelief") yield return Card(Loc.Get("END.RELIEF.FULL"), cardSeconds);
                    if (id == "Ambiguous") yield return Card(Loc.Get("END.AMBIGUOUS.SYNC"), cardSeconds);
                    // Open hook: a new arrival and The Mother starting the protocol (D-51).
                    Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.HOOK");
                    yield return Card("", cardSeconds);
                    break;
            }

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
            states.TryTransition(GameStateId.MainMenu);
            Services.Get<SceneLoader>().Load(SceneNames.MainMenu);
            HbLog.Info("Ending", $"{id} done -> main menu");
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
