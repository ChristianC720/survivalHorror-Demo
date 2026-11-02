using Heartbeat.Interaction;
using UnityEngine;

namespace Heartbeat.Narrative
{
    /// <summary>The intro elevator's control panel: E only when <see cref="IntroDirector"/> arms it (Descend, then Repair).</summary>
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
