using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Heartbeat.AI;
using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.Level;
using Heartbeat.Generation;
using Heartbeat.InputRouting;
using Heartbeat.Inventory;
using Heartbeat.Narrative;
using Heartbeat.Player;
using Heartbeat.Save;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Cycle
{
    /// <summary>
    /// The death → reconstruction → new floor loop and floor-to-floor progression (S-07, S-08, M-15, M-16, D-30, D-35, D-39).
    /// Lives on Bootstrap. Death: save first, lose the inventory, discard the autosave, raise the subject code, rebuild
    /// floor 1 with a new seed and hand the revolver back unloaded (D-74). Floor exit: ≥ 5 s transition, carry the
    /// inventory, next floor. Final key rooms of floors 1-2 autosave when no enemy hunts; floor 2's resets the counter.
    /// </summary>
    public sealed class CycleController : MonoBehaviour
    {

        private SaveService saves;
        private RegistryModel registry;
        private GameStateMachine states;
        private SceneLoader loader;
        private InputRouter router;
        private bool busy;
        private bool skipRequested;
        private AttemptSave carry;            // inventory brought to the next floor
        private bool autosavePending;
        private bool resumingFinalRoom;   // the next FloorBuilt resumes a final-room autosave (not a floor start)
        private RoomInstance savedFinalRoom;  // final key room whose autosave is current (no-cost quit)
        private readonly System.Random seeds = new System.Random();

        public int Slot { get; private set; }

        /// <summary>Story mode (D-103) of the current slot.</summary>
        public static bool StoryMode => Services.TryGet(out CycleController c) && c.Data != null && c.Data.mode == GameMode.Story;
        public SlotData Data { get; private set; }
        public int CurrentFloor { get; private set; } = 1;

        private void Start()
        {
            saves = Services.Get<SaveService>();
            registry = Services.Get<RegistryModel>();
            states = Services.Get<GameStateMachine>();
            loader = Services.Get<SceneLoader>();
            router = Services.Get<InputRouter>();
            router.Actions.Cinematic.Skip.performed += OnSkip;
            GameEvents.PlayerDied += OnPlayerDied;
            GameEvents.FloorExitRequested += OnFloorExit;
            GameEvents.RoomEntered += OnRoomEntered;
            GameEvents.FloorBuilt += OnFloorBuilt;
            GameEvents.HuntChanged += OnHuntChanged;

            if (Data != null) return;
            // Play pressed inside a gameplay/sandbox scene in the editor: a hidden dev slot, fresh every Play, so tests
            // never touch the player's real slots. Launches through Bootstrap/MainMenu use the real slots.
            string launch = BootstrapLoader.LaunchScene;
            bool devLaunch = Application.isEditor && !string.IsNullOrEmpty(launch) && launch != SceneNames.Bootstrap && launch != SceneNames.MainMenu;
            if (devLaunch) LoadOrCreate(DevSlot, GameMode.Survival, newGame: true);
            else LoadOrCreate(saves.LoadGlobal().lastUsedSlot, GameMode.Survival, newGame: false);
            // The floor may already be built (its FloorBuilt fired before this Start): record its start now.
            if (Services.TryGet(out FloorBuilder fb) && fb.Layout != null) RecordFloorStart(fb.Layout.Floor);
        }

        /// <summary>Hidden slot file used by editor play-from-scene (never listed in the menus, never "last used").</summary>
        public const int DevSlot = SaveService.SlotCount;

        private void OnDestroy()
        {
            if (router != null) router.Actions.Cinematic.Skip.performed -= OnSkip;
            GameEvents.PlayerDied -= OnPlayerDied;
            GameEvents.FloorExitRequested -= OnFloorExit;
            GameEvents.RoomEntered -= OnRoomEntered;
            GameEvents.FloorBuilt -= OnFloorBuilt;
            GameEvents.HuntChanged -= OnHuntChanged;
        }

        private void OnSkip(InputAction.CallbackContext _) => skipRequested = true;

        // ---------- Slots ----------

        /// <summary>Continue / Load (or New game when <paramref name="newGame"/>): puts the slot in memory and the registry.</summary>
        public void LoadOrCreate(int slot, GameMode mode, bool newGame)
        {
            Slot = slot;
            Data = newGame ? null : saves.Load(slot);
            if (Data == null)
            {
                if (newGame) saves.Delete(slot);
                // Showcase preset (×0.75 budgets, 6–8 + 2 + 1 rooms) for fairs/demos: launch the build with -showcase.
                bool showcase = System.Array.IndexOf(System.Environment.GetCommandLineArgs(), "-showcase") >= 0;
                Data = new SlotData { mode = mode, showcase = showcase };
                if (showcase) HbLog.Info("Cycle", "Showcase preset for this slot");
            }
            registry.Import(Data.terminal.Select(e => (e.id, e.kind)));
            Services.Get<FaultModel>().Import(Data.patchedFaults);
            if (slot != DevSlot)
            {
                var g = saves.LoadGlobal();
                g.lastUsedSlot = slot;
                saves.SaveGlobal(g);
            }
            HbLog.Info("Cycle", $"Slot {slot}: {Data.SubjectLabel}, deaths {Data.deathCounter}, registry {registry.Count}");
            GameEvents.RaiseDegradationChanged(Data.degradationLevel);
        }

        public void SaveNow()
        {
            Data.terminal = registry.Export().Select(e => new RecordEntry { id = e.id, kind = e.kind }).ToList();
            Data.patchedFaults = Services.Get<FaultModel>().Export();
            saves.Save(Slot, Data);
        }

        /// <summary>Request for the Floor scene: Continue starts from the autosave floor, otherwise floor 1.</summary>
        public FloorSession.Request NextRequest(int floor) => new FloorSession.Request
        {
            Floor = floor,
            Seed = seeds.Next(1, int.MaxValue / 2),
            FirstAttempt = Data.firstAttempt && floor == 1,
            StoryMode = Data.mode == GameMode.Story,
            Showcase = Data.showcase,
        };

        // ---------- Main menu → game ----------

        /// <summary>
        /// Continue / Load / New game (Reglas y Estados): loads the slot and enters the floor. With an autosave it
        /// resumes in that floor's final room with the saved inventory (D-30); otherwise the cycle starts on floor 1.
        /// After a quit during a hunt it plays the short reconstruction; Assimilation never plays here, only on a real death (D-149).
        /// A slot whose intro was never finished plays the Intro scene first (D-53, D-63).
        /// </summary>
        public void StartFromMenu(int slot, GameMode mode, bool newGame)
        {
            if (busy) return;
            StartCoroutine(StartRoutine(slot, mode, newGame));
        }

        private IEnumerator StartRoutine(int slot, GameMode mode, bool newGame)
        {
            busy = true;
            LoadOrCreate(slot, mode, newGame);
            bool afterHuntQuit = Data.confirmedExit && Data.exitCost == ExitCost.Hunt;
            Data.confirmedExit = false;
            if (Data.newCyclePending)
            {
                // New cycle after an ending (D-93): from the capsule, terminal kept, counter and degradation to 0, subject continues.
                Data.newCyclePending = false;
                Data.deathCounter = 0;
                Data.degradationLevel = 0;
                Data.hasAttemptSave = false;
                Data.hasFloorStart = false;
                Data.firstAttempt = false;
                GameEvents.RaiseDegradationChanged(0);
                HbLog.Info("Cycle", $"New cycle (D-93): {Data.SubjectLabel}, terminal kept ({registry.Count} records)");
            }
            SaveNow();

            if (!Data.introDone)
            {
                states.TryTransition(GameStateId.Intro);
                HbLog.Info("Cycle", $"Start slot {slot}: intro");
                loader.Load(SceneNames.Intro, null, fadeOut: false);
                while (loader.IsBusy) yield return null;
                busy = false;
                yield break;   // IntroDirector fades in and calls FinishIntro
            }

            states.TryTransition(GameStateId.Loading);
            // D-149: a counter at 5 reached by quitting is kept; Assimilation waits for Lukas's next real death.

            bool resume = Data.hasAttemptSave && !afterHuntQuit && !Data.finalDecisionSave;
            if (Data.finalDecisionSave) { CurrentFloor = FloorBuilder.MotherFloor; carry = null; }   // D-92
            else if (resume) { CurrentFloor = Data.attemptSave.floor; carry = Data.attemptSave; }
            else (CurrentFloor, carry) = FloorStartPoint();   // D-147: start of the saved floor, with what Lukas arrived with
            var request = NextRequest(CurrentFloor);
            request.StartInFinalRoom = resume;
            resumingFinalRoom = resume;
            FloorSession.Pending = request;
            loader.SetStatus(UI.Loc.Get("UI.HUD.SYNC_SUBJECT", Data.SubjectLabel));
            HbLog.Info("Cycle", $"Start slot {slot} ({(newGame ? "new" : "continue")}): floor {CurrentFloor}, resume={resume}, {Data.SubjectLabel}");
            if (afterHuntQuit) states.TryTransition(GameStateId.Reconstruction);
            loader.Load(SceneNames.Floor, null, fadeOut: false);
            while (loader.IsBusy) yield return null;
            loader.SetStatus("");
            states.TryTransition(GameStateId.Playing);
            yield return loader.Fade(1f, 0f);
            busy = false;
        }

        /// <summary>
        /// End of the Intro (D-53): the phone's voice note is copied, damaged, to the arm terminal as the subject's
        /// personal effects and the ring is registered (D-54); the cycle starts on floor 1, first attempt, from the capsule.
        /// </summary>
        public void FinishIntro()
        {
            if (busy) return;
            StartCoroutine(FinishIntroRoutine());
        }

        public const string VoiceNoteId = "DOC.VOICE_NOTE_EMILIA", RingId = "RING";

        private IEnumerator FinishIntroRoutine()
        {
            busy = true;
            Data.introDone = true;
            registry.Register(VoiceNoteId, RecordKind.Confiscated);
            registry.Register(RingId, RecordKind.Ring);
            Data.hasFloorStart = false;
            SaveNow();
            states.TryTransition(GameStateId.Loading);
            CurrentFloor = 1;
            carry = null;
            FloorSession.Pending = NextRequest(1);
            states.TryTransition(GameStateId.Reconstruction);
            GameEvents.RaiseReconstructed(Data.subjectCode);
            HbLog.Info("Cycle", $"Intro done: {Data.SubjectLabel}, floor 1 (first attempt {Data.firstAttempt})");
            loader.Load(SceneNames.Floor, null, fadeOut: false);
            while (loader.IsBusy) yield return null;
            states.TryTransition(GameStateId.Playing);
            yield return loader.Fade(1f, 0f);
            Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.RING");   // he picks up the ring and cannot remember (D-54)
            busy = false;
        }

        /// <summary>Pause ▸ Restart (confirmed): always counts as a death, also in a safe zone (D-32, D-148). Not in the Intro or final decision.</summary>
        public void Restart()
        {
            if (busy || states.Current != GameStateId.Paused) return;
            GameEvents.RaisePlayerDied(DeathCause.Restart);
        }

        /// <summary>
        /// Safe zone (D-148): the floor's spawn room or the saved final room, with no hunt. Quitting there costs nothing;
        /// Continue rebuilds the floor with a new seed from the matching save.
        /// </summary>
        public bool InSafeZone
        {
            get
            {
                if (Services.TryGet<AlertDirector>(out var d) && d.HuntingCount > 0) return false;
                var here = RoomInstance.Current;
                if (here == null) return false;
                if (savedFinalRoom != null && here == savedFinalRoom) return true;
                return Services.TryGet(out FloorBuilder fb) && fb.Layout != null && fb.Rooms.Count > fb.Layout.EntryRoom
                       && here == fb.Rooms[fb.Layout.EntryRoom];
            }
        }

        /// <summary>Start of the saved floor and its loadout (full health); floor 1 empty-handed when there is none.</summary>
        private (int floor, AttemptSave carry) FloorStartPoint()
        {
            if (!Data.hasFloorStart) return (1, null);
            var a = Data.floorStart;
            return (a.floor, new AttemptSave { floor = a.floor, inventory = a.inventory.ToList(), revolverLoaded = a.revolverLoaded, revolverOwned = a.revolverOwned, health = -1f });
        }

        // ---------- Death → reconstruction ----------

        private void OnPlayerDied(DeathCause cause)
        {
            if (busy) return;
            StartCoroutine(DeathRoutine(cause));
        }

        private IEnumerator DeathRoutine(DeathCause cause)
        {
            busy = true;
            if (states.Current == GameStateId.Paused) Services.Get<PauseService>().TryLeavePauseTo(GameStateId.Death);
            else states.TryTransition(GameStateId.Death);

            RegisterDeath(cause);
            bool assimilate = DeathCounter.Assimilates(Data.deathCounter, cause);   // only a real death plays it (D-149)
            SaveNow();                               // save before processing the death

            yield return loader.Fade(0f, 1f);

            if (assimilate)
            {
                states.TryTransition(GameStateId.EndingAssimilation);
                HbLog.Info("Cycle", "Assimilation ending (M-18)");
                GameEvents.RaiseEndingTriggered("Assimilation");
                busy = false;
                yield break;   // the ending sequence (E-08 endings) takes over
            }

            states.TryTransition(GameStateId.Reconstruction);
            // D-147: back to the start of this floor (The Mother's room has none: floor 3), new layout, arrival loadout.
            (CurrentFloor, carry) = FloorStartPoint();
            FloorSession.Pending = NextRequest(CurrentFloor);
            GameEvents.RaiseReconstructed(Data.subjectCode);
            if (Random.value < 1f / 3f) Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.CAPSULE_RETURN");   // 1 in 3 (Textos del Juego)
            HbLog.Info("Cycle", $"RECONSTRUCTION: subject {Data.SubjectLabel} (Space skips)");
            skipRequested = false;
            // Short reconstruction (D-39, D-43): Lukas in front of his capsule, wearing this reconstruction's foreign piece.
            loader.Load(SceneNames.Cinematics, null, fadeOut: false);
            while (loader.IsBusy) yield return null;
            yield return null;   // let the stage run Start (it switches its camera off) before the shot
            if (!skipRequested && Services.TryGet(out Narrative.CinematicStage stage))
            {
                stage.SetSubject(Data.subjectCode);
                loader.SetFade(0f);
                Sfx.Play2D("CUE_Capsule_Reconstruction");
                var shot = StartCoroutine(stage.Play("Capsule", SystemsBalance.Current.shortReconstructionSeconds));
                for (float t = 0f; t < SystemsBalance.Current.shortReconstructionSeconds && !skipRequested; t += Time.unscaledDeltaTime) yield return null;
                StopCoroutine(shot);
                stage.StopAll();
            }
            loader.SetStatus(UI.Loc.Get("UI.INTRO.SUBJECT_REGISTER", Data.subjectCode.ToString("00")));   // The Mother's label (Textos del Juego)
            loader.Load(SceneNames.Floor, null, fadeOut: false);
            while (loader.IsBusy) yield return null;
            loader.SetStatus("");
            states.TryTransition(GameStateId.Playing);
            yield return loader.Fade(1f, 0f);
            busy = false;
        }

        private DeathCounter.State CounterState => new DeathCounter.State
        {
            Consecutive = Data.deathCounter, Total = Data.totalDeaths, SubjectCode = Data.subjectCode,
            Degradation = Data.degradationLevel, FirstAttempt = Data.firstAttempt,
        };

        private void WriteCounter(DeathCounter.State s)
        {
            Data.deathCounter = s.Consecutive; Data.totalDeaths = s.Total; Data.subjectCode = s.SubjectCode;
            Data.degradationLevel = s.Degradation; Data.firstAttempt = s.FirstAttempt;
            GameEvents.RaiseDegradationChanged(Data.degradationLevel);
        }

        /// <summary>Counter, subject code and degradation; the autosave (and with it the inventory) is discarded (S-08).</summary>
        private bool RegisterDeath(DeathCause cause)
        {
            var s = CounterState;
            bool assimilate = DeathCounter.RegisterDeath(ref s, cause == DeathCause.Scripted);
            WriteCounter(s);
            Services.Get<FaultModel>().PatchDiscovered(registry.Has);   // M-17, D-09: no fault survives a death
            Data.hasAttemptSave = false;
            autosavePending = false;
            carry = null;
            HbLog.Info("Cycle", $"Death ({cause}) -> {Data.SubjectLabel}, consecutive {Data.deathCounter}/{DeathCounter.AssimilationThreshold}");
            return assimilate;
        }

        // ---------- Quit (D-31) ----------

        /// <summary>What quitting now would cost; the pause menu's confirmation dialog shows it before quitting.</summary>
        public ExitCost QuitCost => states.StateBeforePause == GameStateId.Intro || states.StateBeforePause == GameStateId.FinalDecision
            ? ExitCost.None   // the Intro never counts (D-63): quitting replays it on Continue; the final decision is saved (D-92)
            : ExitRules.Evaluate(Services.TryGet<AlertDirector>(out var d) && d.HuntingCount > 0, InSafeZone);

        /// <summary>Pause menu "Quit": applies the exit table, saves and returns to the main menu.</summary>
        public void QuitToMenu()
        {
            if (busy || states.Current != GameStateId.Paused) return;
            var cost = QuitCost;
            if (cost == ExitCost.Hunt) RegisterDeath(DeathCause.QuitDuringAlert);
            else if (cost == ExitCost.Calm)
            {
                var s = CounterState;
                DeathCounter.RegisterCalmExit(ref s);
                WriteCounter(s);
            }
            Data.exitCost = cost;
            Data.confirmedExit = true;
            SaveNow();
            HbLog.Info("Cycle", $"Quit cost={cost} -> {Data.SubjectLabel}, consecutive {Data.deathCounter}, degradation {Data.degradationLevel}, autosave={Data.hasAttemptSave}");
            Services.Get<PauseService>().TryLeavePauseTo(GameStateId.MainMenu);
            loader.Load(SceneNames.MainMenu);
        }

        // ---------- Floor progression ----------

        private void OnFloorExit(int floor)
        {
            if (busy) return;
            StartCoroutine(FloorTransitionRoutine(floor));
        }

        private IEnumerator FloorTransitionRoutine(int floor)
        {
            busy = true;
            states.TryTransition(GameStateId.FloorTransition);
            carry = Snapshot();
            CurrentFloor = floor + 1;
            HbLog.Info("Cycle", $"Floor transition {floor} -> {CurrentFloor} (≥ {SystemsBalance.Current.floorTransitionSeconds} s inside the elevator)");
            yield return loader.Fade(0f, 1f);
            float start = Time.unscaledTime;
            // The wait between floors happens inside the elevator (user 2026-11-09): the cabin shot of the Cinematics scene.
            loader.Load(SceneNames.Cinematics, null, fadeOut: false);
            while (loader.IsBusy) yield return null;
            if (Services.TryGet(out CinematicStage cabin))
            {
                Sfx.Play2D("CUE_Elevator_Hum");
                float ride = Mathf.Max(0f, SystemsBalance.Current.floorTransitionSeconds - (Time.unscaledTime - start));
                var shot = StartCoroutine(cabin.Play("Elevator", ride));
                yield return loader.Fade(1f, 0f);
                yield return shot;
                loader.SetFade(1f);
            }
            FloorSession.Pending = NextRequest(CurrentFloor);
            loader.Load(SceneNames.Floor, null, fadeOut: false);
            while (loader.IsBusy || Time.unscaledTime - start < SystemsBalance.Current.floorTransitionSeconds) yield return null;
            states.TryTransition(GameStateId.Playing);
            yield return loader.Fade(1f, 0f);
            busy = false;
        }

        private void OnFloorBuilt(int floor, int seed)
        {
            CurrentFloor = floor;
            bool resumedFinal = resumingFinalRoom;
            resumingFinalRoom = false;
            if (carry != null) Apply(carry);
            else ResetLoadout();
            carry = null;
            if (floor >= FloorBuilder.MotherFloor || Data == null) return;   // The Mother's room has no start save (death: floor 3)
            if (!resumedFinal) RecordFloorStart(floor);
        }

        /// <summary>A new floor start (D-147): what Lukas arrived with; the previous floor's final-room save no longer applies.</summary>
        private void RecordFloorStart(int floor)
        {
            Data.hasAttemptSave = false;
            savedFinalRoom = null;
            Data.floorStart = Snapshot();
            Data.floorStart.floor = floor;
            Data.floorStart.health = -1f;   // a death or restart always comes back at full health
            Data.hasFloorStart = true;
            SaveNow();
            HbLog.Info("Cycle", $"Floor {floor} start saved: {Data.floorStart.inventory.Count(i => i.Kind != ItemKind.None)} items, revolver {Data.floorStart.revolverLoaded}");
        }

        private void OnRoomEntered(MonoBehaviour m)
        {
            var room = m as RoomInstance;
            autosavePending = false;   // a pending autosave belongs to the final room Lukas was in (D-30)
            if (room != savedFinalRoom) savedFinalRoom = null;
            if (room == null || room.Definition == null || room.Definition.role != RoomRole.Final || CurrentFloor >= 3) return;
            bool hunting = Services.TryGet<AlertDirector>(out var d) && d.HuntingCount > 0;
            if (hunting) { autosavePending = true; HbLog.Info("Cycle", "Autosave pending: hunt in progress"); return; }
            Autosave();
        }

        private void OnHuntChanged(bool hunting)
        {
            if (!hunting && autosavePending) Autosave();
        }

        private void Autosave()
        {
            autosavePending = false;
            Data.hasAttemptSave = true;
            Data.attemptSave = Snapshot();
            savedFinalRoom = RoomInstance.Current;
            if (CurrentFloor == 2)
            {
                var s = new DeathCounter.State { Consecutive = Data.deathCounter };
                DeathCounter.ResetAtFloor2Final(ref s);
                Data.deathCounter = s.Consecutive;
                HbLog.Info("Cycle", "Floor 2 final room: death counter reset (D-35)");
                // The finding plays once per slot; later resets are silent (D-56, D-67).
                if (!Data.floor2PhotoSeen && Services.TryGet(out Floor2Photo photo))
                {
                    Data.floor2PhotoSeen = true;
                    StartCoroutine(photo.Play());
                }
            }
            SaveNow();
            GameEvents.RaiseAutosaved();
            HbLog.Info("Cycle", "SYNCHRONIZING… autosave done");
        }

        // ---------- Loadout ----------

        private AttemptSave Snapshot()
        {
            var a = new AttemptSave { floor = CurrentFloor };
            if (Services.TryGet<PlayerInventory>(out var inv)) a.inventory = inv.Model.Snapshot().ToList();
            if (Services.TryGet<PlayerController>(out var pc))
            {
                var rev = pc.GetComponent<Revolver>();
                if (rev != null) { a.revolverLoaded = rev.Model.Loaded; a.revolverOwned = rev.Owned; }
                var hp = pc.GetComponent<PlayerHealth>();
                if (hp != null) a.health = hp.Model.Value;
            }
            return a;
        }

        private void Apply(AttemptSave a)
        {
            if (Services.TryGet<PlayerInventory>(out var inv)) inv.Model.Restore(a.inventory.ToArray());
            if (Services.TryGet<PlayerController>(out var pc))
            {
                var rev = pc.GetComponent<Revolver>();
                if (rev != null) { rev.SetOwned(a.revolverOwned); rev.Model.SetLoaded(a.revolverLoaded); }
                if (a.health >= 0f) pc.GetComponent<PlayerHealth>()?.Model.SetValue(a.health);
            }
            HbLog.Info("Cycle", $"Loadout carried: {(inv != null ? inv.Describe() : "-")}, revolver {a.revolverLoaded}");
        }

        /// <summary>Reconstruction: empty inventory, full health, revolver unloaded on the tray (D-74).</summary>
        private void ResetLoadout()
        {
            if (Services.TryGet<PlayerInventory>(out var inv)) inv.Model.Clear();
            if (Services.TryGet<PlayerController>(out var pc) && pc.TryGetComponent(out Revolver rev))
            {
                rev.Unload();
                rev.SetOwned(false);   // it waits on the capsule tray (or, first attempt, on the Kessler guard)
            }
        }

        /// <summary>Debug / tests.</summary>
        public void DebugKillPlayer()
        {
            if (Services.TryGet<PlayerController>(out var pc)) pc.GetComponent<PlayerHealth>().Model.Kill();
        }

        public void DebugUseExit() => GameEvents.RaiseFloorExitRequested(CurrentFloor);

        public bool DebugRegister(string id) => registry.Register(id, RecordKind.ContextDocument);

        /// <summary>Playtests (CP-05): the 6 Emilia documents, so the victory offers the final decision.</summary>
        public void DebugRegisterAllEmilia()
        {
            for (int i = 1; i <= RegistryModel.EmiliaTotal; i++) registry.Register($"DOC.EMILIA.{i:00}", RecordKind.EmiliaDocument);
        }

        public int RegistryCount => registry.Count;
    }
}
