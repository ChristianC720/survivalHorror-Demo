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
}
