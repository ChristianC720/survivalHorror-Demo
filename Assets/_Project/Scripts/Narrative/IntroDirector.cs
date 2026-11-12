using System.Collections;
using Heartbeat.Core;
using Heartbeat.Level;
using Heartbeat.Player;
using Heartbeat.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// The Intro scene (Premisa y Trama 4.1, Niveles ▸ Introducción; rebuilt in C-65). House cinematic: Lukas, the family
    /// photo and Emilia's scheduled voice note → outside Sektor V-7, hatch hacked on the phone (the photo is its wallpaper)
    /// → elevator, playable until it fails, then the fall → the floor that does not exist, Mudo decoy far away → the death
    /// on screen: hit from behind by something out of frame, the fall, the body held still while the heartbeat stops →
    /// full reconstruction in 1–2 s images with cuts to black: the civil body left behind, the capsule running, the R-H-07
    /// body forming, its face framed like the civil one, The Mother registering the subject, Lukas awake in the capsule →
    /// the ring → <see cref="Cycle.CycleController.FinishIntro"/>. Every shot is its own CinemachineCamera, cut by the
    /// player's brain. The house and the death + reconstruction are skippable once seen (D-91); a skip ends in the same
    /// state as watching. The scripted death never touches the counter or the degradation.
    /// </summary>
    public sealed class IntroDirector : MonoBehaviour
    {
        [System.Serializable]
        public struct ShotCam
        {
            public string id;
            public CinemachineCamera cam;
        }

        [Header("Actors")]
        [SerializeField] private PlayerController player;
        [Tooltip("One CinemachineCamera per shot (C-65). Shots framed at runtime (Attack, Body, FaceCivil, FaceRH07) are moved by code.")]
        [SerializeField] private ShotCam[] shots = new ShotCam[0];

        [Header("House (cinematic)")]
        [Tooltip("The family photo, also the phone's wallpaper during the hatch hack.")]
        [SerializeField] private Texture familyPhoto;

        [Header("Outside Sektor V-7")]
        [SerializeField] private Transform exteriorStart;
        [SerializeField] private HackPanel hatchPanel;
        [SerializeField] private Transform hatchDoor;

        [Header("Elevator")]
        [SerializeField] private Transform elevatorInside;
        [SerializeField] private IntroElevatorPanel elevatorPanel;
        [SerializeField] private Light[] elevatorLights = new Light[0];

        [Header("The floor that does not exist")]
        [SerializeField] private Transform corridorStart;
        [SerializeField] private Transform corridorElevatorDoor;
        [SerializeField] private Transform attackPoint;
        [Tooltip("The corridor lights behind Lukas go dark when he is hit: the attacker is never shown (D-53).")]
        [SerializeField] private Light[] corridorLightsBehind = new Light[0];
        [SerializeField] private Light[] corridorLightsAhead = new Light[0];

        [Header("Reconstruction (D-39)")]
        [SerializeField] private Light capsuleLight;
        [Tooltip("Lukas R-H-07 standing in his capsule.")]
        [SerializeField] private Animator newBody;
        [Tooltip("The ring resting on the personal effects tray by the capsule (GDD Premisa y Trama: full reconstruction). E picks it up.")]
        [SerializeField] private IntroElevatorPanel ringOnTray;
        [SerializeField] private float ringSeconds = 2.5f;

        [Header("Timing (initial values, subject to balance)")]
        [SerializeField] private float houseSeconds = 4f;
        [SerializeField] private float photoSeconds = 4f;
        [SerializeField] private float lookSeconds = 3f;
        [SerializeField] private float voiceLineSeconds = 3.75f;   // one bar of Emilia's theme
        [SerializeField] private float rideSeconds = 4f;
        [SerializeField] private float fallSeconds = 2.5f;           // = SFX_Elevator_Fall (C-57)
        [SerializeField] private float attackRadius = 2.5f;
        [SerializeField] private float attackBackupSeconds = 25f;
        [SerializeField] private float attackShotSeconds = 3f;
        [SerializeField] private float bodySeconds = 6f;
        [SerializeField] private float faceSeconds = 3f;
        [SerializeField] private float flashSeconds = 1.5f;          // fleeting images, 1–2 s (D-39)
        [SerializeField] private float blackSeconds = 0.6f;
        [SerializeField] private float wakeSeconds = 3f;
        [SerializeField] private float faceDistance = 0.7f;
        [Tooltip("Both face shots are turned this much around the spine (the capsule's front bar would cover a straight one).")]
        [SerializeField] private float faceAngle = 28f;

        private const int VoiceLines = 7;   // SUB.EMILIA.NOTE_1..7 (Textos del Juego, nota de voz)
        private static readonly float[] HeartGaps = { 0.8f, 0.9f, 1.1f, 1.4f, 1.9f };   // the beat slows, then stops
        private SceneLoader loader;
        private AudioSource music;
        private CinemachineBrain brain;
        private CinemachineBlendDefinition playerBlend;
        private CinemachineCamera live;
        private bool debugRing;

        public string Stage { get; private set; } = "Boot";

        private IEnumerator Start()
        {
            while (!Bootstrap.IsReady) yield return null;
            loader = Services.Get<SceneLoader>();
            loader.SetFade(1f);
            player.MovementLocked = true;
            HackView.PhoneWallpaper = familyPhoto;
            foreach (var s in shots) if (s.cam != null) s.cam.enabled = false;
            BuildShells();
            yield return null;   // let the player rig settle before the first cut
            brain = FindAnyObjectByType<CinemachineBrain>();
            playerBlend = brain.DefaultBlend;
            var router = Services.Get<InputRouting.InputRouter>();

            router.SetContext(InputRouting.InputContext.Cinematic);
            yield return SkippableCinematic.Run(this, "Intro_House", House(), OnHouseSkipped);
            yield return Exterior();
            yield return Elevator();
            yield return NonexistentFloor();
            router.SetContext(InputRouting.InputContext.Cinematic);   // hold Skip once seen (D-91)
            yield return SkippableCinematic.Run(this, "Reconstruction", DeathAndReconstruction(), OnReconstructionSkipped);
            SetStage("Done");
            Services.Get<Cycle.CycleController>().FinishIntro();
        }

        private void OnDestroy()
        {
            HackView.PhoneWallpaper = null;
            Render.Degradation.Apply(0);
            // Quitting from the pause menu mid-intro: Emilia's loop must not follow the player to the menu.
            if (Services.TryGet(out Audio.AudioService audio)) audio.FadeOutMusic(music);
        }

        // ---------- house ----------

        private IEnumerator House()
        {
            SetStage("House");
            Cut("House");
            yield return loader.Fade(1f, 0f);
            yield return Wait(houseSeconds);   // Lukas, alone, in a plain, run-down house
            Cut("Photo");
            yield return Wait(photoSeconds);   // the family photo: Lukas and Emilia
            Cut("LukasLooks");
            Sfx.Play2D("CUE_Intro_PhoneMessage");
            yield return Wait(lookSeconds);
            StartVoiceNote();
            Cut("VoiceNote");
            var push = StartCoroutine(Drift("VoiceNote", 0.05f));
            for (int i = 1; i <= VoiceLines; i++)
            {
                if (i == 5) { StopCoroutine(push); Cut("Photo"); }   // "Don't come looking for me": back to the photo
                Subtitles.Say(Subtitles.Emilia, $"SUB.EMILIA.NOTE_{i}");
                yield return Wait(voiceLineSeconds);
            }
            yield return loader.Fade(0f, 1f);
            EndShots();
        }

        private void StartVoiceNote()
        {
            if (music == null && Services.TryGet(out Audio.AudioService audio) && audio.TryGetCue("CUE_Music_VoiceNote", out var note)) music = audio.StartMusic(note);   // Emilia (D-87)
        }

        /// <summary>Skipped house: the same end state as watching it, black, Emilia's theme playing, no shot live.</summary>
        private void OnHouseSkipped()
        {
            StartVoiceNote();
            loader.SetFade(1f);
            EndShots();
        }

        // ---------- playable: hatch and elevator ----------

        private IEnumerator Exterior()
        {
            SetStage("Exterior");
            Place(exteriorStart);
            loader.SetStatus(Loc.Get("UI.INTRO.LOCATION"));
            yield return Wait(2f);
            loader.SetStatus("");
            Services.Get<InputRouting.InputRouter>().SetContext(InputRouting.InputContext.IntroPlayable);
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
            Sfx.Stop("CUE_Elevator_Hum");
            Sfx.Play2D("CUE_Elevator_Fail");
            foreach (var l in elevatorLights) l.color = new Color(0.8f, 0.1f, 0.05f);
            yield return Flicker(0.8f);
            elevatorPanel.Arm("UI.PROMPT.REPAIR");
            while (!elevatorPanel.Consume()) yield return null;

            // Cinematic: the elevator distorts and drops (D-46: the lab is the creature).
            player.MovementLocked = true;
            var fall = Cut("Fall");
            Sfx.Stop("CUE_Elevator_Fail");
            Sfx.Play2D("CUE_Elevator_Fall");
            var home = fall.transform.position;
            for (float t = 0f; t < fallSeconds; t += Time.deltaTime)
            {
                float k = t / fallSeconds;
                Render.Degradation.Apply(Mathf.RoundToInt(k * 5f));
                fall.transform.position = home + Random.insideUnitSphere * 0.08f * k;
                foreach (var l in elevatorLights) l.enabled = Random.value > 0.35f;
                yield return null;
            }
            fall.transform.position = home;
            loader.SetFade(1f);
            Render.Degradation.Apply(0);
            Sfx.Play2D("CUE_Elevator_Impact");
            EndShots();
            yield return Wait(2f);
        }

        // ---------- playable: the floor that does not exist ----------

        private IEnumerator NonexistentFloor()
        {
            SetStage("NonexistentFloor");
            Place(corridorStart);
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
            player.MovementLocked = true;
        }

        // ---------- cinematic: death and full reconstruction ----------

        private IEnumerator DeathAndReconstruction()
        {
            yield return Death();
            yield return Reconstruction();
        }

        /// <summary>
        /// Hit from behind on screen (C-65): the lights behind him die, something out of frame strikes, he falls and lies
        /// still; a held shot of the body while his heartbeat slows and stops; his face. Only then the cut to black.
        /// </summary>
        private IEnumerator Death()
        {
            SetStage("Death");
            Sfx.Stop("CUE_Elevator_Hum");
            Transform t = player.transform;
            Vector3 fwd = Flat(t.forward);
            Aim("Attack", t.position + fwd * 2.2f + Vector3.up * 0.7f, t.position + Vector3.up * 1.3f);
            foreach (var l in corridorLightsBehind) l.enabled = false;
            yield return Wait(0.4f);
            if (Services.TryGet(out Audio.AudioService sound)) sound.FadeOutMusic(music);   // Emilia lasts while Lukas is human
            music = null;
            Sfx.Play2D("CUE_Intro_Attack");
            player.GetComponent<PlayerAnimator>().PlayScriptedDeath();
            HbLog.Info("Intro", "Scripted death (does not count, D-63)");
            yield return Wait(attackShotSeconds);

            foreach (var l in corridorLightsBehind) l.enabled = true;   // whatever struck him is gone; the body lies in the light
            var anim = player.GetComponentInChildren<Animator>();
            Transform hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            Aim("Body", hips.position + Vector3.up * 2.6f + fwd * 0.3f, hips.position);
            for (int i = 0; i < HeartGaps.Length; i++)
            {
                Sfx.Play2D("CUE_Ending_BeatLukas");
                yield return Wait(HeartGaps[i]);
            }
            HbLog.Info("Intro", "Heart stopped");
            yield return Wait(bodySeconds - Sum(HeartGaps));

            FrameFace("FaceCivil", anim);
            yield return Wait(faceSeconds);
            loader.SetFade(1f);
            yield return Wait(1f);
        }

        private IEnumerator Reconstruction()
        {
            SetStage("Reconstruction");
            // 1. The civil body left behind in the empty corridor.
            Cut("LeftBehind");
            loader.SetFade(0f);
            StartCoroutine(Dim(corridorLightsAhead, flashSeconds));
            yield return Wait(flashSeconds);
            yield return Black();

            // 2–4. The capsule running, the new body forming, image after image.
            Sfx.Play2D("CUE_Capsule_Reconstruction");
            capsuleLight.intensity = 1f;
            yield return Flash("Capsules", 0);
            capsuleLight.intensity = 2.5f;
            Frame("Forming", newBody, HumanBodyBones.Chest, 1.6f, 15f);
            yield return Flash("Forming", 4);
            capsuleLight.intensity = 4f;
            Frame("FormingClose", newBody, HumanBodyBones.LeftLowerArm, 0.8f, 30f);   // the arm terminal
            yield return Flash("FormingClose", 2);

            // 5. His new face, framed like the civil one; The Mother registers the subject.
            capsuleLight.intensity = 5.5f;
            FrameFace("FaceRH07", newBody);
            loader.SetFade(0f);
            int n = Services.Get<Cycle.CycleController>().Data.subjectCode;
            Sfx.Stop("CUE_Capsule_Reconstruction");
            Sfx.Play2D("CUE_Vocal_Mother");
            Subtitles.Say(Subtitles.Mother, "UI.INTRO.SUBJECT_REGISTER", n.ToString("00"));
            loader.SetStatus(Loc.Get("UI.INTRO.SUBJECT_REGISTER", n.ToString("00")));
            yield return Wait(faceSeconds);
            loader.SetStatus("");

            // 6. Lukas awake in the capsule.
            Cut("Wake");
            StartCoroutine(Drift("Wake", 0.15f));
            yield return Wait(wakeSeconds);

            yield return RingPickup();
            Sfx.Play2D("CUE_Vocal_Mother");
            Subtitles.Say(Subtitles.Mother, "UI.INTRO.PHONE_COPY", n.ToString("00"));
            loader.SetStatus(Loc.Get("UI.INTRO.PHONE_COPY", n.ToString("00")));
            yield return Wait(3f);
            loader.SetStatus("");
            EndShots();
        }

        /// <summary>Skipped death + reconstruction: black, silent, no shot live; FinishIntro registers the voice note and the ring.</summary>
        private void OnReconstructionSkipped()
        {
            Sfx.Stop("CUE_Capsule_Reconstruction");
            if (Services.TryGet(out Audio.AudioService sound)) sound.FadeOutMusic(music);
            music = null;
            Render.Degradation.Apply(0);
            ringOnTray.gameObject.SetActive(false);
            loader.SetFade(1f);
            loader.SetStatus("");
            EndShots();
        }

        /// <summary>
        /// Lukas, out of the capsule, finds a ring on the personal effects tray and cannot remember why it matters (C-16,
        /// C-26, D-54). A close shot of the ring; the cinematic holds, with only E enabled, until the player picks it up.
        /// The ring is registered once, by the cycle controller when the intro ends.
        /// </summary>
        private IEnumerator RingPickup()
        {
            SetStage("Ring");
            Cut("Ring");
            loader.SetFade(0f);
            yield return Wait(ringSeconds * 0.5f);
            var router = Services.Get<InputRouting.InputRouter>();
            router.SetContext(InputRouting.InputContext.CinematicPickup);
            ringOnTray.Arm("UI.PROMPT.PICKUP");
            GameEvents.RaiseInteractTargetChanged(ringOnTray);   // the same "[E] Pick up" prompt as any item
            HbLog.Info("Intro", "Ring waits on the tray");
            while (!router.Actions.Gameplay.Interact.WasPerformedThisFrame() && !debugRing) yield return null;
            GameEvents.RaiseInteractTargetChanged(null);
            router.SetContext(InputRouting.InputContext.Cinematic);
            HbLog.Info("Intro", "Ring picked up");
            Sfx.Play2D(Sfx.Pickup);
            ringOnTray.gameObject.SetActive(false);
            Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.RING");   // "This is important. I don't remember why."
            yield return Wait(ringSeconds * 0.5f);
            loader.SetFade(1f);
        }

        // ---------- shots ----------

        private CinemachineCamera Find(string id)
        {
            foreach (var s in shots) if (s.id == id) return s.cam;
            HbLog.Warn("Intro", $"No shot '{id}'");
            return null;
        }

        /// <summary>Hard cut to a shot: only one shot camera is live, and it outranks the player's cameras.</summary>
        private CinemachineCamera Cut(string id)
        {
            var cam = Find(id);
            if (cam == null) return null;
            brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.Cut, 0f);
            if (live != null && live != cam) live.enabled = false;
            cam.enabled = true;
            live = cam;
            HbLog.Info("Intro", $"Shot {id}");
            return cam;
        }

        /// <summary>Back to the player's cameras with their own blend.</summary>
        private void EndShots()
        {
            if (live != null) live.enabled = false;
            live = null;
            if (brain != null) brain.DefaultBlend = playerBlend;
        }

        private void Aim(string id, Vector3 from, Vector3 at)
        {
            var cam = Find(id);
            if (cam == null) return;
            cam.transform.SetPositionAndRotation(from, Quaternion.LookRotation(at - from));
            Cut(id);
        }

        /// <summary>The same close framing for both faces (civil and R-H-07), so the two bodies compare.</summary>
        private void FrameFace(string id, Animator body)
        {
            Transform head = body.GetBoneTransform(HumanBodyBones.Head);
            Vector3 up = (head.position - Chest(body).position).normalized;
            Vector3 eye = head.position + up * 0.06f;
            Aim(id, eye + Facing(body, up, faceAngle) * faceDistance, eye);
        }

        /// <summary>Frames a bone from in front of the body, turned <paramref name="angle"/> degrees around its spine.</summary>
        private void Frame(string id, Animator body, HumanBodyBones bone, float distance, float angle)
        {
            Vector3 at = body.GetBoneTransform(bone).position;
            Vector3 up = (body.GetBoneTransform(HumanBodyBones.Head).position - Chest(body).position).normalized;
            Aim(id, at + Facing(body, up, angle) * distance + Vector3.up * 0.1f, at);
        }

        private static Transform Chest(Animator body) => body.GetBoneTransform(HumanBodyBones.Chest) ?? body.GetBoneTransform(HumanBodyBones.Spine);

        // The way the body faces (shoulders × spine), turned around the spine so the capsule's front bar stays out of the face.
        private static Vector3 Facing(Animator body, Vector3 up, float angle)
        {
            Vector3 across = body.GetBoneTransform(HumanBodyBones.RightUpperArm).position - body.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            return Quaternion.AngleAxis(angle, up) * Vector3.Cross(across, up).normalized;
        }

        private IEnumerator Flash(string id, int degradation)
        {
            Cut(id);
            Render.Degradation.Apply(degradation);
            loader.SetFade(0f);
            yield return Wait(flashSeconds);
            yield return Black();
            Render.Degradation.Apply(0);
        }

        private IEnumerator Black()
        {
            loader.SetFade(1f);
            yield return Wait(blackSeconds);
        }

        private IEnumerator Drift(string id, float metres)
        {
            var cam = Find(id);
            if (cam == null) yield break;
            for (float t = 0f; t < 30f && live == cam; t += Time.deltaTime)
            {
                cam.transform.position += cam.transform.forward * (metres * Time.deltaTime / 10f);
                yield return null;
            }
        }

        private static IEnumerator Dim(Light[] lights, float seconds)
        {
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                foreach (var l in lights) l.enabled = Save.AccessOptions.ReduceFlashes || Random.value > t / seconds;   // D-106
                yield return null;
            }
            foreach (var l in lights) l.enabled = false;
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

        private void Place(Transform at) => player.DebugTeleport(at.position.x, at.position.y, at.position.z, at.eulerAngles.y);

        private static float FlatDistance(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));

        private static Vector3 Flat(Vector3 v) { v.y = 0f; return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.forward; }

        private static float Sum(float[] values) { float s = 0f; foreach (var v in values) s += v; return s; }

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

        /// <summary>Playtests: picks the ring up as if E had been pressed.</summary>
        public void DebugTakeRing() => debugRing = true;

        /// <summary>Playtests: jump the player to a stage marker (Exterior, Elevator, Corridor, Attack).</summary>
        public void DebugGoto(string marker)
        {
            var t = marker == "Elevator" ? elevatorInside : marker == "Corridor" ? corridorStart : marker == "Attack" ? attackPoint : exteriorStart;
            Place(t);
        }
    }
}
