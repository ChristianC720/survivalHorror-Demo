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
        private readonly System.Random rng = new System.Random();

        public HackSequence Current { get; private set; }
        public bool IsHacking => Current != null;

        public event Action<HackSequence> Started;
        public event Action<HackStep> Stepped;
        public event Action<bool> Ended;   // true = success

        private void OnEnable() => Services.Register(this);
        private void OnDisable() => Services.Unregister(this);

        private void Start()
        {
            router = Services.Get<InputRouter>();
            Services.TryGet(out states);
            var h = router.Actions.Hacking;
            h.Up.performed += OnUp;
            h.Down.performed += OnDown;
            h.Left.performed += OnLeft;
            h.Right.performed += OnRight;
            h.Abort.performed += OnAbort;
            if (health != null) health.Hit += OnHit;
        }

        private void OnDestroy()
        {
            if (health != null) health.Hit -= OnHit;
            if (router == null) return;
            var h = router.Actions.Hacking;
            h.Up.performed -= OnUp;
            h.Down.performed -= OnDown;
            h.Left.performed -= OnLeft;
            h.Right.performed -= OnRight;
            h.Abort.performed -= OnAbort;
        }

        private void OnUp(InputAction.CallbackContext _) => Feed(HackDir.Up);
        private void OnDown(InputAction.CallbackContext _) => Feed(HackDir.Down);
        private void OnLeft(InputAction.CallbackContext _) => Feed(HackDir.Left);
        private void OnRight(InputAction.CallbackContext _) => Feed(HackDir.Right);

        public bool Begin(IHackTarget hackTarget, HackSequence fixedSequence = null)
        {
            if (IsHacking) return false;
            target = hackTarget;
            Current = fixedSequence ?? HackSequence.Generate(hackTarget.Difficulty, rng);
            previousContext = router.Context;
            router.SetContext(InputContext.Hacking);
            states?.TrySetSubState(PlaySubState.Hacking);
            player.MovementLocked = true;
            startFrame = Time.frameCount;   // the E press that started the hack must not abort it
            HbLog.Info("Hack", $"Started ({Current.Steps.Count} inputs): {string.Join(" ", Current.Steps)}");
            Started?.Invoke(Current);
            return true;
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

        private void OnAbort(InputAction.CallbackContext _)
        {
            if (IsHacking && Time.frameCount > startFrame + 1) End(false, "abandoned");
        }

        private void OnHit()
        {
            if (IsHacking) End(false, "cancelled by a hit");
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
