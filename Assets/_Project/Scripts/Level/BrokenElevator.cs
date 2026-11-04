using Heartbeat.Core;
using Heartbeat.Interaction;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>
    /// The elevator that brought Lukas to this floor (D-158): dead behind him, so the only way is forward. It can be tried but
    /// never repaired or hacked.
    /// </summary>
    public sealed class BrokenElevator : MonoBehaviour, IInteractable
    {
        public string PromptKey => "UI.PROMPT.OUT_OF_ORDER";
        public Vector3 InteractionPoint => transform.position + transform.forward * .3f + Vector3.up * 1.2f;

        public bool CanInteract(Interactor who) => true;

        public void Interact(Interactor who)
        {
            HbLog.Info("Door", $"{name}: out of order");
            Sfx.Play(Sfx.DoorLocked, transform.position);
            Subtitles.Say(Subtitles.Lukas, "SUB.LUKAS.ELEVATOR_DEAD");
        }
    }
}
