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
    /// <summary>Phase 1 node (M-13): a mixing desk hacked with a main-terminal sequence (D-114).</summary>
    public sealed class BossNode : MonoBehaviour, IInteractable, IHackTarget
    {
        [SerializeField] private Renderer screen;
        public bool IsActive { get; private set; }
        public bool IsDone { get; private set; }

        public HackDifficulty Difficulty => HackDifficulty.Main;
        public string PromptKey => "UI.PROMPT.HACK";
        public Vector3 InteractionPoint => transform.position + Vector3.up * 1.1f;
        public bool CanInteract(Interactor who) => IsActive && !IsDone;

        public void SetActive(bool active) => IsActive = active;

        public void Interact(Interactor who)
        {
            if (Services.TryGet(out PlayerHack hack)) hack.Begin(this);
        }

        public void OnHackSucceeded()
        {
            if (IsDone) return;
            IsDone = true;
            if (screen != null) screen.enabled = false;
            Sfx.Play("CUE_Boss_NodeHacked", transform.position);
            Services.Get<MotherDirector>().NodeHacked(this);
        }
    }
}
