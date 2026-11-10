using System.Collections;
using Heartbeat.Core;
using Heartbeat.Level;
using Heartbeat.Player;
using Heartbeat.UI;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// The Intro scene (D-53, D-63; Niveles ▸ Introducción): house shot with Emilia's scheduled voice note → outside
    /// Sektor V-7, hatch hacked on the phone → elevator, playable until it fails, then the fall (cinematic) → the floor
    /// that does not exist, Mudo decoy far away, attack from behind (trigger + backup timer) → full reconstruction
    /// cinematic (D-39) → <see cref="Cycle.CycleController.FinishIntro"/>. Minimal HUD (move, look, interact, pause without
    /// Restart). The scripted death never touches the counter or the degradation. Every set is hand-built in the scene.
    /// </summary>
    public sealed class IntroDirector : MonoBehaviour
    {
        [Header("Actors")]
        [SerializeField] private PlayerController player;
        [Tooltip("Cinematic camera, disabled while the player has control.")]
        [SerializeField] private Camera shotCamera;

        [Header("House (cinematic)")]
        [SerializeField] private Transform houseShot;

        [Header("Outside Sektor V-7")]
        [SerializeField] private Transform exteriorStart;
        [SerializeField] private HackPanel hatchPanel;
        [SerializeField] private Transform hatchDoor;

        [Header("Elevator")]
        [SerializeField] private Transform elevatorInside;
        [SerializeField] private IntroElevatorPanel elevatorPanel;
        [SerializeField] private Transform elevatorShot;
        [SerializeField] private Light[] elevatorLights = new Light[0];

        [Header("The floor that does not exist")]
        [SerializeField] private Transform corridorStart;
        [SerializeField] private Transform corridorElevatorDoor;
        [SerializeField] private Transform attackPoint;

        [Header("Reconstruction (D-39)")]
        [SerializeField] private Transform capsuleShot;
        [SerializeField] private Light capsuleLight;

        [Header("Timing (initial values, subject to balance)")]
        [SerializeField] private float houseSeconds = 4f;
        [SerializeField] private float voiceLineSeconds = 3.75f;   // one bar of Emilia's theme
        [SerializeField] private float rideSeconds = 4f;
        [SerializeField] private float fallSeconds = 2.5f;
        [SerializeField] private float attackRadius = 2.5f;
        [SerializeField] private float attackBackupSeconds = 25f;

        private const int VoiceLines = 7;   // SUB.EMILIA.NOTE_1..7 (Textos del Juego, nota de voz)
        private SceneLoader loader;
        private AudioSource music;

        public string Stage { get; private set; } = "Boot";

        private IEnumerator Start()
        {
            while (!Bootstrap.IsReady) yield return null;
            loader = Services.Get<SceneLoader>();
            loader.SetFade(1f);
            player.MovementLocked = true;
            BuildShells();
            yield return null;   // let the player rig settle before the first cut

            yield return House();
            yield return Exterior();
            yield return Elevator();
            yield return NonexistentFloor();
            Services.Get<InputRouting.InputRouter>().SetContext(InputRouting.InputContext.Cinematic);   // hold Skip once seen (D-91)
            yield return SkippableCinematic.Run(this, "Reconstruction", Reconstruction(), () => { loader.SetFade(1f); loader.SetStatus(""); });
            SetStage("Done");
            Services.Get<Cycle.CycleController>().FinishIntro();
        }

        // Quitting from the pause menu mid-intro: Emilia's loop must not follow the player to the menu.
        private void OnDestroy()
        {
            if (Services.TryGet(out Audio.AudioService audio)) audio.FadeOutMusic(music);
        }

        private IEnumerator House()
        {
            SetStage("House");
            Cut(houseShot);
            yield return loader.Fade(1f, 0f);
            yield return Wait(houseSeconds);   // Lukas, alone, looks at the family photo
            Sfx.Play2D("CUE_Intro_PhoneMessage");
            if (Services.TryGet(out Audio.AudioService audio) && audio.TryGetCue("CUE_Music_VoiceNote", out var note)) music = audio.StartMusic(note);   // Emilia (D-87)
            yield return Wait(voiceLineSeconds);
            for (int i = 1; i <= VoiceLines; i++)
            {
                Subtitles.Say(Subtitles.Emilia, $"SUB.EMILIA.NOTE_{i}");
                yield return Wait(voiceLineSeconds);
            }
            yield return loader.Fade(0f, 1f);
        }

        private IEnumerator Exterior()
        {
            SetStage("Exterior");
            Place(exteriorStart);
            loader.SetStatus(Loc.Get("UI.INTRO.LOCATION"));
            yield return Wait(2f);
            loader.SetStatus("");
            shotCamera.enabled = false;
            player.MovementLocked = false;
            yield return loader.Fade(1f, 0f);

            while (!hatchPanel.IsDone) yield return null;   // first hack, on the phone (D-63)
            Sfx.Play(Sfx.DoorOpen, hatchDoor.position);
            yield return Slide(hatchDoor, Vector3.down * 2.4f, 1f);
        }

        private IEnumerator Elevator()
        {
            SetStage("Elevator");
            while (FlatDistance(player.transform.position, elevatorInside.position) > 0.8f) yield return null;
            Sfx.Play(Sfx.DoorOpen, hatchDoor.position);
            yield return Slide(hatchDoor, Vector3.up * 2.4f, 1f);   // the hatch closes behind him

            elevatorPanel.Arm("UI.PROMPT.DESCEND");
            while (!elevatorPanel.Consume()) yield return null;
            Sfx.Play2D("CUE_Elevator_Hum");
            yield return Wait(rideSeconds);

            // It fails: a jolt, the lights die to red, the panel asks for a repair.
            Sfx.Play2D("CUE_Elevator_Fail");
            foreach (var l in elevatorLights) l.color = new Color(0.8f, 0.1f, 0.05f);
            yield return Flicker(0.8f);
            elevatorPanel.Arm("UI.PROMPT.REPAIR");
            while (!elevatorPanel.Consume()) yield return null;

            // Cinematic: the elevator distorts and drops (D-46: the lab is the creature).
            player.MovementLocked = true;
            Cut(elevatorShot);
            Sfx.Play2D("CUE_Elevator_Fall");
            var home = shotCamera.transform.localPosition;
            for (float t = 0f; t < fallSeconds; t += Time.deltaTime)
            {
                float k = t / fallSeconds;
                Render.Degradation.Apply(Mathf.RoundToInt(k * 5f));
                shotCamera.transform.localPosition = home + Random.insideUnitSphere * 0.08f * k;
                foreach (var l in elevatorLights) l.enabled = Random.value > 0.35f;
                yield return null;
            }
            shotCamera.transform.localPosition = home;
            loader.SetFade(1f);
            Render.Degradation.Apply(0);
            Sfx.Play2D("CUE_Elevator_Impact");
            yield return Wait(2f);
        }

        private IEnumerator NonexistentFloor()
        {
            SetStage("NonexistentFloor");
            Place(corridorStart);
            shotCamera.enabled = false;
            yield return loader.Fade(1f, 0f);
            Sfx.Play(Sfx.DoorOpen, corridorElevatorDoor.position);
            yield return Slide(corridorElevatorDoor, Vector3.up * 2.4f, 1f);
            player.MovementLocked = false;

            float arrived = Time.time;
            bool doorClosed = false;
            while (true)
            {
                var p = player.transform.position;
                if (!doorClosed && FlatDistance(p, corridorStart.position) > 2.5f)
                {
                    doorClosed = true;   // the elevator closes and rises on its own
                    StartCoroutine(Slide(corridorElevatorDoor, Vector3.down * 2.4f, 1f));
                    Sfx.Play(Sfx.DoorOpen, corridorElevatorDoor.position);
                    Sfx.Play2D("CUE_Elevator_Hum");
                }
                if (FlatDistance(p, attackPoint.position) < attackRadius) break;
                if (Time.time - arrived > attackBackupSeconds) { HbLog.Info("Intro", "Attack by backup timer"); break; }
                yield return null;
            }

            // Attack from behind: cut to black, the attacker is never shown (D-53).
            player.MovementLocked = true;
            loader.SetFade(1f);
            if (Services.TryGet(out Audio.AudioService sound)) sound.FadeOutMusic(music);   // Emilia lasts while Lukas is human
            Sfx.Play2D("CUE_Intro_Attack");
            HbLog.Info("Intro", "Scripted death (does not count, D-63)");
            yield return Wait(2.5f);
        }

        private IEnumerator Reconstruction()
        {
            SetStage("Reconstruction");
            Cut(capsuleShot);
            // Fleeting 1-2 s images with cuts to black, each one a little more complete (D-39, D-53).
            for (int i = 0; i < 5; i++)
            {
                capsuleLight.intensity = 1f + i * 1.5f;
                Sfx.Play2D("CUE_Capsule_Reconstruction");
                loader.SetFade(0f);
                yield return Wait(Random.Range(1f, 2f));
                loader.SetFade(1f);
                yield return Wait(0.6f);
            }
            int n = Services.Get<Cycle.CycleController>().Data.subjectCode;
            loader.SetStatus(Loc.Get("UI.INTRO.SUBJECT_REGISTER", n.ToString("00")));
            yield return Wait(3f);
            loader.SetStatus(Loc.Get("UI.INTRO.PHONE_COPY", n.ToString("00")));
            yield return Wait(3f);
            loader.SetStatus("");
        }

        // ---------- helpers ----------

        /// <summary>Kit shells of the hand-placed sets, every socket open (the hatches are separate door blocks).</summary>
        private static void BuildShells()
        {
            foreach (var shell in FindObjectsByType<Generation.RoomShell>(FindObjectsInactive.Exclude))
            {
                var open = new System.Collections.Generic.List<Generation.SocketData>();
                foreach (var s in shell.GetComponentsInChildren<Generation.DoorSocket>(true))
                    open.Add(new Generation.SocketData { cell = s.LocalCell(shell.transform, shell.size), facing = s.LocalFacing(shell.transform) });
                shell.Build(open);
            }
        }

        private void SetStage(string s)
        {
            Stage = s;
            HbLog.Info("Intro", $"Stage {s}");
        }

        private void Cut(Transform pose)
        {
            shotCamera.transform.SetPositionAndRotation(pose.position, pose.rotation);
            shotCamera.enabled = true;
        }

        private void Place(Transform at) => player.DebugTeleport(at.position.x, at.position.y, at.position.z, at.eulerAngles.y);

        private static float FlatDistance(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));

        // Waits in game time, so the pause freezes the intro too.
        private static IEnumerator Wait(float seconds)
        {
            for (float t = 0f; t < seconds; t += Time.deltaTime) yield return null;
        }

        private static IEnumerator Slide(Transform t, Vector3 by, float seconds)
        {
            Vector3 from = t.position, to = t.position + by;
            for (float k = 0f; k < 1f; k += Time.deltaTime / seconds)
            {
                t.position = Vector3.Lerp(from, to, k);
                yield return null;
            }
            t.position = to;
        }

        private IEnumerator Flicker(float seconds)
        {
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                foreach (var l in elevatorLights) l.enabled = Save.AccessOptions.ReduceFlashes || Random.value > 0.5f;   // D-106
                yield return null;
            }
            foreach (var l in elevatorLights) l.enabled = true;
        }

        // ---------- debug / playtests ----------

        /// <summary>Playtests: jump the player to a stage marker (Exterior, Elevator, Corridor, Attack).</summary>
        public void DebugGoto(string marker)
        {
            var t = marker == "Elevator" ? elevatorInside : marker == "Corridor" ? corridorStart : marker == "Attack" ? attackPoint : exteriorStart;
            Place(t);
        }
    }
}
