using Heartbeat.Interaction;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>
    /// An intro prop E uses only when <see cref="IntroDirector"/> arms it: the elevator panel (Descend, then Repair) and
    /// the ring on the capsule's tray (Pick up, read by the director during the reconstruction cinematic).
    /// </summary>
    public sealed class IntroElevatorPanel : MonoBehaviour, IInteractable
    {
        private string promptKey;
        private bool armed, pressed;

        public string PromptKey => promptKey;
        public Vector3 InteractionPoint => transform.position;

        public void Arm(string key)
        {
            promptKey = key;
            armed = true;
            pressed = false;
        }

        /// <summary>True once after the player pressed E on the armed panel.</summary>
        public bool Consume()
        {
            if (!pressed) return false;
            pressed = false;
            return true;
        }

        public bool CanInteract(Interactor who) => armed;

        public void Interact(Interactor who)
        {
            armed = false;
            pressed = true;
        }
    }
}
