using Heartbeat.Core;
using Heartbeat.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Heartbeat.Level
{
    /// <summary>
    /// Wall panel or room terminal (M-13, M-17): E starts a hack of the configured difficulty; success runs the
    /// configured action once (open a door, enable the elevator, restore power, activate a boss node).
    /// Room terminals store nothing persistent (M-16).
    /// </summary>
    public sealed class HackPanel : MonoBehaviour, IInteractable, IHackTarget
    {
        [SerializeField] private HackDifficulty difficulty = HackDifficulty.Door;
        [SerializeField] private DoorLeaf door;
        [SerializeField] private UnityEvent onHacked;

        public bool IsDone { get; private set; }
        public HackDifficulty Difficulty => difficulty;
        public string PromptKey => "UI.PROMPT.HACK";
        public Vector3 InteractionPoint => transform.position;

        public bool CanInteract(Interactor who) => !IsDone;

        public void Interact(Interactor who)
        {
            if (Services.TryGet<PlayerHack>(out var hack)) hack.Begin(this);
        }

        public void OnHackSucceeded()
        {
            IsDone = true;
            HbLog.Info("Hack", $"{name} hacked");
            if (door != null)
            {
                door.Configure(DoorLock.None);
                door.TryOpenByEnemy();   // opens a closed, now unlocked door
            }
            onHacked?.Invoke();
        }
    }
}
