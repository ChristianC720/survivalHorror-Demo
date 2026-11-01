using Heartbeat.AI;
using Heartbeat.Audio;
using Heartbeat.Combat;
using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;

namespace Heartbeat.Boss
{
    /// <summary>Final decision (D-51): the central terminal or the exit, used with E; no menu.</summary>
    public sealed class DecisionPoint : MonoBehaviour, IInteractable
    {
        public enum Kind { CentralTerminal, Exit }
        [SerializeField] private Kind kind;
        private bool active;

        public string PromptKey => kind == Kind.CentralTerminal ? "UI.PROMPT.EXAMINE" : "UI.PROMPT.LEAVE";
        public Vector3 InteractionPoint => transform.position + Vector3.up * 1.1f;
        public bool CanInteract(Interactor who) => active;
        public void SetActive(bool on) => active = on;
        public void Interact(Interactor who) => Services.Get<MotherDirector>().Decide(kind);
    }
}
