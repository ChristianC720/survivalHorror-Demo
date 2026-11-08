using System.Collections.Generic;
using Heartbeat.Core;
using Heartbeat.Generation;
using Heartbeat.InputRouting;
using Heartbeat.Level;
using Heartbeat.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heartbeat.Tutorial
{
    /// <summary>
    /// Guided first attempt (D-164): shows the next action of the room with its key and opens the exit when the room's
    /// actions are done, or after a time-out so nobody gets stuck. Built by <see cref="FloorBuilder"/>; dies with the floor.
    /// </summary>
    public sealed class TutorialGuide : MonoBehaviour
    {
        private static readonly Dictionary<TutorialAction, (string key, string action)> Hints = new Dictionary<TutorialAction, (string, string)>
        {
            { TutorialAction.Terminal, ("UI.TUT.TERMINAL", "Gameplay/Terminal") },
            { TutorialAction.Flashlight, ("UI.TUT.FLASHLIGHT", "Gameplay/Flashlight") },
            { TutorialAction.PickUp, ("UI.TUT.PICKUP", "Gameplay/Interact") },
            { TutorialAction.Sprint, ("UI.TUT.SPRINT", "Gameplay/Sprint") },
            { TutorialAction.Crouch, ("UI.TUT.CROUCH", "Gameplay/Crouch") },
            { TutorialAction.Aim, ("UI.TUT.AIM", "Gameplay/Aim") },
            { TutorialAction.Attack, ("UI.TUT.ATTACK", "Gameplay/Attack") },
        };

        private readonly TutorialModel model = new TutorialModel();
        private Dictionary<RoomInstance, int> stages;
        private Dictionary<int, List<DoorLeaf>> gates;
        private PlayerController player;
        private InputRouter router;
        private float timeout, stageClock;

        public void Init(Dictionary<RoomInstance, int> roomStages, Dictionary<int, List<DoorLeaf>> doors, float gateTimeout)
        {
            stages = roomStages;
            gates = doors;
            timeout = gateTimeout;
        }

        private void Start()
        {
            model.StageCleared += Open;
            GameEvents.RoomEntered += OnRoom;
            GameEvents.InventoryChanged += OnInventory;
            GameEvents.AimChanged += OnAim;
            Services.TryGet(out player);
            if (Services.TryGet(out router))
            {
                var a = router.Actions.Gameplay;
                a.Terminal.performed += OnTerminal;
                a.Flashlight.performed += OnFlashlight;
                a.Attack.performed += OnAttack;
            }
            ShowHint();
        }

        private void OnDestroy()
        {
            GameEvents.RoomEntered -= OnRoom;
            GameEvents.InventoryChanged -= OnInventory;
            GameEvents.AimChanged -= OnAim;
            GameEvents.RaiseHint(null);
            if (router == null) return;
            try
            {
                var a = router.Actions.Gameplay;
                a.Terminal.performed -= OnTerminal;
                a.Flashlight.performed -= OnFlashlight;
                a.Attack.performed -= OnAttack;
            }
            catch (System.NullReferenceException) { }   // input wrapper torn down first (leaving Play mode)
        }

        private void OnTerminal(InputAction.CallbackContext _) => Do(TutorialAction.Terminal);
        private void OnFlashlight(InputAction.CallbackContext _) => Do(TutorialAction.Flashlight);
        private void OnAttack(InputAction.CallbackContext _) => Do(TutorialAction.Attack);
        private void OnInventory() => Do(TutorialAction.PickUp);
        private void OnAim(bool on) { if (on) Do(TutorialAction.Aim); }

        private void Do(TutorialAction action)
        {
            if (!model.Do(action)) return;
            HbLog.Info("Tutorial", $"Stage {model.Stage}: {action} done");
            Sfx.Play2D(Sfx.Pickup);
            ShowHint();
        }

        private void OnRoom(MonoBehaviour room)
        {
            int stage = room is RoomInstance r && stages.TryGetValue(r, out int s) ? s : 0;
            if (stage == model.Stage) return;
            model.Stage = stage;
            stageClock = 0f;
            HbLog.Info("Tutorial", $"Stage {stage}");
            ShowHint();
        }

        private void Update()
        {
            if (player == null) return;
            if (player.IsRunning) Do(TutorialAction.Sprint);
            if (player.IsCrouched) Do(TutorialAction.Crouch);
            if (model.Stage < 1 || model.IsCleared(model.Stage)) return;
            stageClock += Time.deltaTime;
            if (stageClock < timeout) return;
            HbLog.Info("Tutorial", $"Stage {model.Stage} timed out: exit opened");
            model.Clear(model.Stage);
            ShowHint();
        }

        private void Open(int stage)
        {
            if (gates.TryGetValue(stage, out var doors))
                foreach (var door in doors) if (door != null) door.Configure(DoorLock.None);
            HbLog.Info("Tutorial", $"Stage {stage} cleared: exit unlocked");
            GameEvents.RaiseNotice("UI.NOTICE.EXIT_OPEN");
            Sfx.Play2D(Sfx.HackSuccess);
        }

        private void ShowHint()
        {
            var next = model.Current;
            if (next == null) { GameEvents.RaiseHint(null); return; }
            var hint = Hints[next.Value];
            GameEvents.RaiseHint(hint.key, hint.action);
        }
    }
}
