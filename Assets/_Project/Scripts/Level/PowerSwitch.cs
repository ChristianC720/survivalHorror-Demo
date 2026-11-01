using Heartbeat.Core;
using Heartbeat.Interaction;
using UnityEngine;

namespace Heartbeat.Level
{
    /// <summary>
    /// Restores power to the final key room (D-72). Loud: the machinery start-up alerts nearby enemies (M-09).
    /// </summary>
    public sealed class PowerSwitch : MonoBehaviour, IInteractable
    {
        [SerializeField] private float noiseRadius = 14f;

        public bool IsOn { get; private set; }
        public string PromptKey => "UI.PROMPT.POWER";
        public Vector3 InteractionPoint => transform.position + Vector3.up * 1.2f;

        public bool CanInteract(Interactor who) => !IsOn;

        public void Interact(Interactor who)
        {
            IsOn = true;
            HbLog.Info("Power", "Power restored");
            GameEvents.RaiseNoiseEmitted(new NoiseInfo(transform.position, noiseRadius, NoiseSource.Machinery));
            GameEvents.RaisePowerRestored();
        }
    }
}
