using UnityEngine;

namespace Heartbeat.Interaction
{
    /// <summary>Anything Lukas can use with E / A (M-08): pickups, documents, doors, panels, switches, stairs.</summary>
    public interface IInteractable
    {
        /// <summary>Localization key of the prompt verb (e.g. UI.PROMPT.PICKUP). The E key itself comes from the binding.</summary>
        string PromptKey { get; }
        /// <summary>World point used for distance and "centre of the view" scoring.</summary>
        Vector3 InteractionPoint { get; }
        bool CanInteract(Interactor who);
        void Interact(Interactor who);
    }

    /// <summary>An interactable with a second action on holding E / A (a door: tap opens, hold seals).</summary>
    public interface IHoldInteractable : IInteractable
    {
        /// <summary>The hold action is on offer right now; while false, a press interacts at once.</summary>
        bool CanHold { get; }
        string HoldPromptKey { get; }
        void HoldComplete(Interactor who);
    }
}
