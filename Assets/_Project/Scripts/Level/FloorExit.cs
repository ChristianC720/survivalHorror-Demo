using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.Interaction;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>
    /// Elevator of a final key room (M-15, D-72, D-96; no ramp since 2026-11-09: every floor leaves by elevator). Usable once the room has power and no enemy is hunting
    /// (LOCKDOWN while a hunt lasts). Raises FloorExitRequested; the cycle controller runs the ≥ 5 s transition.
    /// </summary>
    public sealed class FloorExit : MonoBehaviour, IInteractable
    {
        public bool Powered { get; set; } = true;
        public int Floor { get; set; } = 1;

        public string PromptKey => !Powered ? "UI.PROMPT.NOPOWER" : Locked ? "UI.PROMPT.LOCKDOWN" : "UI.PROMPT.DESCEND";
        public Vector3 InteractionPoint => transform.position + transform.forward * .3f + Vector3.up * 1.2f;   // the elevator door, facing the room

        private static bool Locked => Services.TryGet<AlertDirector>(out var d) && d.HuntingCount > 0;

        private void OnEnable() => GameEvents.PowerRestored += OnPower;
        private void OnDisable() => GameEvents.PowerRestored -= OnPower;
        private void OnPower() => Powered = true;

        public bool CanInteract(Interactor who) => true;

        public void Interact(Interactor who)
        {
            if (!Powered) { HbLog.Info("Exit", "No power"); return; }
            if (Locked) { HbLog.Info("Exit", "LOCKDOWN: Containment breach. Transit suspended."); Sfx.Play(Sfx.TransitLockdown, transform.position); Subtitles.Say(Subtitles.Mother, "SUB.MOTHER.LOCKDOWN"); return; }
            HbLog.Info("Exit", $"Floor {Floor} exit used");
            GameEvents.RaiseFloorExitRequested(Floor);
        }
    }
}
