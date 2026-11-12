using System;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Level
{
    /// <summary>
    /// Runs a hack (M-13): Lukas stays still in OTS, WASD/arrows enter the chain, E abandons, a hit cancels.
    /// The game never pauses. The arm terminal (E-10) draws the chain from <see cref="Current"/> and the events.
    /// </summary>
    public sealed class PlayerHack : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerHealth health;

        private InputRouter router;
        private GameStateMachine states;
        private IHackTarget target;
        private InputContext previousContext;
        private int startFrame;
        private bool faultUsed;
        private FaultModel faults;
        private Narrative.RegistryModel registry;
        private readonly System.Random rng = new System.Random();
        private readonly bool[] heldAtStart = new bool[4];   // indexed by HackDir: held when the hack began, ignored until released

        public HackSequence Current { get; private set; }
        public bool IsHacking => Current != null;

        /// <summary>Discovered, unpatched fault the current hack may exploit (M-17), or null.</summary>
        public string AvailableFault => IsHacking && !faultUsed && faults != null && registry != null
            ? faults.UsableFor(target.Difficulty, registry.Has) : null;

        public event Action<HackSequence> Started;
        public event Action<HackStep> Stepped;
        public event Action<bool> Ended;   // true = success

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Start()
        {
            router = Services.Get<InputRouter>();
            Services.TryGet(out states);
            Services.TryGet(out faults);
            Services.TryGet(out registry);
            var h = router.Actions.Hacking;
            h.Up.performed += OnUp;
            h.Down.performed += OnDown;
            h.Left.performed += OnLeft;
            h.Right.performed += OnRight;
            h.Abort.performed += OnAbort;
            h.UseFault.performed += OnUseFault;
            if (health != null) health.Hit += OnHit;
            GameEvents.GrabStarted += OnGrabStarted;
        }

        private void OnDestroy()
        {
            if (health != null) health.Hit -= OnHit;
            GameEvents.GrabStarted -= OnGrabStarted;
            if (router == null) return;
            var h = router.Actions.Hacking;
            h.Up.performed -= OnUp;
            h.Down.performed -= OnDown;
            h.Left.performed -= OnLeft;
            h.Right.performed -= OnRight;
            h.Abort.performed -= OnAbort;
            h.UseFault.performed -= OnUseFault;
        }

        private void OnUp(InputAction.CallbackContext _) => OnDirection(HackDir.Up);
        private void OnDown(InputAction.CallbackContext _) => OnDirection(HackDir.Down);
        private void OnLeft(InputAction.CallbackContext _) => OnDirection(HackDir.Left);
        private void OnRight(InputAction.CallbackContext _) => OnDirection(HackDir.Right);

        public bool Begin(IHackTarget hackTarget, HackSequence fixedSequence = null)
        {
            if (IsHacking) return false;
            target = hackTarget;
            Current = fixedSequence ?? HackSequence.Generate(hackTarget.Difficulty, rng);
            faultUsed = false;
            previousContext = router.Context;
            router.SetContext(InputContext.Hacking);
            states?.TrySetSubState(PlaySubState.Hacking);
            player.MovementLocked = true;
            startFrame = Time.frameCount;   // the E press that started the hack must not abort it
            for (int i = 0; i < 4; i++) heldAtStart[i] = Held((HackDir)i);   // WASD or stick still held from walking must not count
            HbLog.Info("Hack", $"Started ({Current.Steps.Count} inputs): {string.Join(" ", Current.Steps)}");
            Started?.Invoke(Current);
            return true;
        }

        private void Update()
        {
            if (!IsHacking) return;
            for (int i = 0; i < 4; i++)
                if (heldAtStart[i] && !Held((HackDir)i)) heldAtStart[i] = false;
        }

        private InputAction ActionFor(HackDir d)
        {
            var h = router.Actions.Hacking;
            return d == HackDir.Up ? h.Up : d == HackDir.Down ? h.Down : d == HackDir.Left ? h.Left : h.Right;
        }

        // Reads the controls bound to the hack action (arrows, WASD, D-pad, left stick), not a device directly.
        private bool Held(HackDir d)
        {
            foreach (var c in ActionFor(d).controls)
                if (c.IsPressed()) return true;
            return false;
        }

        private void OnDirection(HackDir dir)
        {
            if (IsHacking && heldAtStart[(int)dir] && Held(dir)) return;
            heldAtStart[(int)dir] = false;
            Feed(dir);
        }

        private void Feed(HackDir dir)
        {
            if (!IsHacking) return;
            var step = Current.Input(dir);
            Stepped?.Invoke(step);
            Sfx.Play(step == HackStep.Error ? Sfx.HackError : step == HackStep.Complete ? Sfx.HackSuccess : Sfx.HackInput, transform.position);
            if (step == HackStep.Error) HbLog.Info("Hack", $"Error beep, sequence restarted (errors={Current.Errors})");
            if (step == HackStep.Complete) End(true, "success");
        }

        private void OnUseFault(InputAction.CallbackContext _) => UseFault();

        /// <summary>M-17: exploit the available fault once per hack; the chain is cut and redrawn. Returns false if none.</summary>
        public bool UseFault()
        {
            string id = AvailableFault;
            if (id == null) return false;
            faultUsed = true;
            int before = Current.Steps.Count;
            Current.Truncate(faults.ShortLength(target.Difficulty, before));
            Started?.Invoke(Current);   // the arm terminal redraws the shorter chain
            faults.Use(id);
            HbLog.Info("Fault", $"{id} exploited: {before} -> {Current.Steps.Count} inputs (uses left {faults.UsesLeft(id)})");
            return true;
        }

        private void OnAbort(InputAction.CallbackContext _)
        {
            if (IsHacking && Time.frameCount > startFrame + 1) End(false, "abandoned");
        }

        private void OnHit()
        {
            if (IsHacking) End(false, "cancelled by a hit");
        }

        /// <summary>A grab cancels the hack before it resolves, with or without a spike (Punzón de descarga).</summary>
        private void OnGrabStarted(MonoBehaviour _)
        {
            if (IsHacking) End(false, "cancelled by a grab");
        }

        private void End(bool success, string why)
        {
            var t = target;
            Current = null;
            target = null;
            player.MovementLocked = false;
            bool hunting = Services.TryGet<AI.AlertDirector>(out var d) && d.HuntingCount > 0;
            states?.TrySetSubState(hunting ? PlaySubState.Alert : PlaySubState.Exploration);
            router.SetContext(previousContext == InputContext.Hacking ? InputContext.Exploration : previousContext);
            HbLog.Info("Hack", $"Ended: {why}");
            Ended?.Invoke(success);
            if (success) t?.OnHackSucceeded();
        }

        /// <summary>Debug / tests: feed a wrong input (anything but the expected one).</summary>
        public void DebugFeedWrong()
        {
            if (!IsHacking) return;
            var expected = Current.Steps[Current.Progress];
            Feed((HackDir)(((int)expected + 1) % 4));
        }

        /// <summary>Debug / tests: feed every remaining correct input.</summary>
        public void DebugSolve()
        {
            while (IsHacking) Feed(Current.Steps[Current.Progress]);
        }

        /// <summary>Debug / tests: feed one input by name (Up/Down/Left/Right).</summary>
        public void DebugInput(string dir) => Feed((HackDir)Enum.Parse(typeof(HackDir), dir, true));
    }
}
