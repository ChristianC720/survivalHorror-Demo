using Heartbeat.AI;
using Heartbeat.Core;
using Heartbeat.Interaction;
using Heartbeat.Inventory;
using UnityEngine;
using UnityEngine.AI;

namespace Heartbeat.Level
{
    public enum DoorLock { None, MandatoryKey, OptionalKey, NoPower }
    public enum DoorState { Closed, Open, Sealed }

    /// <summary>
    /// Door between two rooms (KIT_SPEC §4, M-14, M-15, D-36, D-97). E opens/closes it; a key door consumes its key
    /// (mandatory or optional); an unpowered door stays shut until power is restored. After Lukas crosses it, E on the
    /// far side starts the quick seal hack (2-3 inputs): a sealed door blocks enemies until a hunting enemy breaks it
    /// (~30 s next to it) or Lukas crosses back. A door that is not open is a wall for the NavMesh (carved): enemies in
    /// patrol or suspicion never cross it; in search or hunt they walk up to it and open it. An enemy barrier (the spawn
    /// room until Lukas leaves it) is never crossed nor opened by enemies.
    /// </summary>
    public sealed class DoorLeaf : MonoBehaviour, IInteractable, IHackTarget, ISoundOccluder
    {
        [SerializeField] private Transform leaf;
        [SerializeField] private NavMeshObstacle obstacle;
        [SerializeField] private DoorLock lockType = DoorLock.None;
        [SerializeField] private float sealBreakSeconds = 30f;
        [SerializeField] private float openHeight = 2.3f;
        [SerializeField] private float slideSpeed = 4f;
        [Tooltip("Set on a door that parts sideways into the walls (hospital door, D-157): each leaf travels this far along local X. 0 = the leaf rises.")]
        [SerializeField] private float sideSlide;
        [Tooltip("The other half of a door that parts sideways; it travels the opposite way.")]
        [SerializeField] private Transform secondLeaf;

        private float breakProgress;
        private int lastSide;               // +1 / -1: side of the door Lukas was last on (local Z)
        private Transform player;
        private bool sealPending;
        private float closedY;
        private float closedX;
        private float closedSecondX;

        public DoorState State { get; private set; } = DoorState.Closed;
        public DoorLock Lock => lockType;
        public bool IsLocked => lockType != DoorLock.None;
        /// <summary>Enemies can never open or cross it (spawn room, until Lukas leaves it).</summary>
        public bool EnemyBarrier { get; private set; }
        /// <summary>Search/hunt enemies may walk up to it: to open it, or to pound on it when sealed (M-14).</summary>
        public bool EnemyCanReach => !IsLocked && !EnemyBarrier && State != DoorState.Open;
        public HackDifficulty Difficulty => HackDifficulty.QuickSeal;
        public bool Occludes => State != DoorState.Open;   // −50% through a closed or sealed door (S-11)
        public bool IsDoor => true;

        public string PromptKey => IsLocked ? "UI.PROMPT.LOCKED" : State == DoorState.Open ? "UI.PROMPT.CLOSE" : "UI.PROMPT.OPEN";   // sealed: E opens it too
        public Vector3 InteractionPoint => transform.position + Vector3.up * 1.2f;

        public void Configure(DoorLock l) => lockType = l;

        public void SetEnemyBarrier(bool on)
        {
            EnemyBarrier = on;
            SetState(State);
        }

        public void RestorePower()
        {
            if (lockType != DoorLock.NoPower) return;
            lockType = DoorLock.None;
            HbLog.Info("Door", $"{name} power restored");
        }

        public bool CanInteract(Interactor who) => true;

        public void Interact(Interactor who)
        {
            switch (lockType)
            {
                case DoorLock.NoPower:
                    HbLog.Info("Door", $"{name}: no power");
                    Sfx.Play(Sfx.DoorLocked, transform.position);
                    return;
                case DoorLock.MandatoryKey:
                case DoorLock.OptionalKey:
                    var kind = lockType == DoorLock.MandatoryKey ? ItemKind.KeyMandatory : ItemKind.KeyOptional;
                    if (!who.Inventory.Model.TryConsume(kind))
                    {
                        HbLog.Info("Door", $"{name}: ACCESS DENIED ({kind} required)");
                        Sfx.Play(Sfx.DoorLocked, transform.position);
                        return;
                    }
                    lockType = DoorLock.None;
                    HbLog.Info("Door", $"{name}: unlocked with {kind}");
                    SetState(DoorState.Open);
                    return;
            }

            if (State == DoorState.Sealed)
            {
                // Lukas can always undo his own seal (it also breaks when he crosses, D-97).
                HbLog.Info("Door", $"{name}: seal released by Lukas");
                Sfx.Play(Sfx.DoorSealBreak, transform.position);
                SetState(DoorState.Open);
                return;
            }

            // Lukas crossed this door and turns back towards it while enemies hunt: offer the quick seal.
            bool hunting = Services.TryGet<AlertDirector>(out var d) && d.HuntingCount > 0;
            if (hunting && Services.TryGet<PlayerHack>(out var hack))
            {
                sealPending = true;
                hack.Begin(this);
                return;
            }
            SetState(State == DoorState.Open ? DoorState.Closed : DoorState.Open);
        }

        public void OnHackSucceeded()
        {
            if (!sealPending) return;
            sealPending = false;
            breakProgress = 0f;
            SetState(DoorState.Sealed);
            HbLog.Info("Door", $"{name}: SEALED");
            Sfx.Play(Sfx.DoorSeal, transform.position);
        }

        /// <summary>Enemies call this when their path crosses a closed, unlocked door.</summary>
        public bool TryOpenByEnemy()
        {
            if (State != DoorState.Closed || IsLocked || EnemyBarrier) return State == DoorState.Open && !EnemyBarrier;
            SetState(DoorState.Open);
            return true;
        }

        private void Update()
        {
            bool open = State == DoorState.Open;
            float step = slideSpeed * Time.deltaTime;
            var p = leaf.localPosition;
            if (sideSlide > 0f)
            {
                p.x = Mathf.MoveTowards(p.x, open ? closedX + sideSlide : closedX, step);
                var q = secondLeaf.localPosition;
                q.x = Mathf.MoveTowards(q.x, open ? closedSecondX - sideSlide : closedSecondX, step);
                secondLeaf.localPosition = q;
            }
            else p.y = Mathf.MoveTowards(p.y, open ? closedY + openHeight : closedY, step);
            leaf.localPosition = p;

            TrackPlayerSide();
            if (State == DoorState.Sealed) TickSealBreak();
            if (State == DoorState.Closed && !IsLocked && !EnemyBarrier) OpenForNearbyEnemies();
        }

        private void TrackPlayerSide()
        {
            if (player == null && Services.TryGet<Player.PlayerController>(out var pc)) player = pc.transform;
            if (player == null) return;
            Vector3 local = transform.InverseTransformPoint(player.position);
            if (Mathf.Abs(local.x) > 1.5f || Mathf.Abs(local.z) > 3f) return;
            int side = local.z >= 0f ? 1 : -1;
            if (lastSide != 0 && side != lastSide && State == DoorState.Sealed)
            {
                HbLog.Info("Door", $"{name}: seal broken (Lukas crossed back)");
                Sfx.Play(Sfx.DoorSealBreak, transform.position);
                SetState(DoorState.Open);
            }
            lastSide = side;
        }

        private void TickSealBreak()
        {
            if (!Services.TryGet<AlertDirector>(out var d)) return;
            bool hunterAtDoor = false;
            foreach (var e in d.Enemies)
                if (e.IsActive && e.Alert == AlertState.Hunt && Vector3.Distance(e.transform.position, transform.position) < 2.5f)
                { hunterAtDoor = true; break; }
            if (!hunterAtDoor) return;
            breakProgress += Time.deltaTime;
            if (breakProgress >= sealBreakSeconds)
            {
                HbLog.Info("Door", $"{name}: seal broken by an enemy");
                Sfx.Play(Sfx.DoorSealBreak, transform.position);
                SetState(DoorState.Open);
            }
        }

        private void OpenForNearbyEnemies()
        {
            if (!Services.TryGet<AlertDirector>(out var d)) return;
            foreach (var e in d.Enemies)
                if (e.IsActive && e.Alert >= AlertState.Search && Vector3.Distance(e.transform.position, transform.position) < 1.8f)
                { SetState(DoorState.Open); return; }
        }

        private void SetState(DoorState s)
        {
            if (s != State && (s == DoorState.Open || s == DoorState.Closed) && State != DoorState.Sealed)
                Sfx.Play(Sfx.DoorOpen, transform.position);   // one motor cue for open and close
            State = s;
            if (obstacle != null) obstacle.enabled = s != DoorState.Open || IsLocked || EnemyBarrier;   // carved = wall for agents
            if (s != DoorState.Sealed) breakProgress = 0f;
            HbLog.Dev("Door", $"{name} -> {s}");
        }

        private void Awake()
        {
            closedY = leaf.localPosition.y;
            closedX = leaf.localPosition.x;
            if (secondLeaf != null) closedSecondX = secondLeaf.localPosition.x;
        }

        private void Start() => SetState(State);

        /// <summary>Debug / tests.</summary>
        public void DebugSetState(string s) => SetState((DoorState)System.Enum.Parse(typeof(DoorState), s, true));
    }
}
