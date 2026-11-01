using System;
using System.Collections.Generic;
using Heartbeat.Core;
using UnityEngine.InputSystem;

namespace Heartbeat.InputRouting
{
    /// <summary>Input contexts (GDD 02 - Diseño/04 - Controles, "Contextos de entrada").</summary>
    public enum InputContext
    {
        None,
        Exploration,
        Aiming,
        Surveillance,
        OverBody,
        Inventory,
        Hacking,
        Menu,
        Cinematic,
        IntroPlayable,   // D-63: only move, look, interact (+ pause)
    }

    /// <summary>
    /// The only owner of the input actions. Gameplay code reads actions through <see cref="Actions"/> and never
    /// touches devices directly. Switching context enables exactly the maps/actions that context allows, so no key
    /// has two meanings inside one context. System/Pause stays enabled in every context except None.
    /// </summary>
    public sealed class InputRouter : IDisposable
    {
        private static readonly Dictionary<InputContext, string[]> GameplayWhitelist = new Dictionary<InputContext, string[]>
        {
            { InputContext.Exploration, null },   // null = whole Gameplay map
            { InputContext.Aiming, null },
            { InputContext.OverBody, null },
            // Surveillance: stand still or walk very slowly; running/aiming/shoving exit the view, so they stay readable.
            { InputContext.Surveillance, new[] { "Move", "Sprint", "Aim", "Attack", "Surveillance", "Interact" } },
            // Inventory: real time; the character can still move, but no aim/shoot/shove (Controles).
            { InputContext.Inventory, new[] { "Move", "Look", "Sprint", "Crouch" } },
            { InputContext.IntroPlayable, new[] { "Move", "Look", "Interact" } },
        };

        public HeartbeatInput Actions { get; }
        public InputContext Context { get; private set; } = InputContext.None;

        public event Action<InputContext> ContextChanged;

        /// <summary>Last device that performed an action; prompts and posters follow it on the fly (D-132).</summary>
        public bool UsingGamepad { get; private set; }
        public event Action<bool> DeviceChanged;

        public InputRouter()
        {
            Actions = new HeartbeatInput();
            SetContext(InputContext.None);
            InputSystem.onActionChange += OnActionChange;
        }

        /// <summary>Display name of the action's binding for the current device, e.g. "E" or "A".</summary>
        public string KeyFor(InputAction action) =>
            action.GetBindingDisplayString(InputBinding.MaskByGroup(UsingGamepad ? "Gamepad" : "KeyboardMouse"));

        private void OnActionChange(object o, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed || !(o is InputAction a) || a.activeControl == null) return;
            bool pad = a.activeControl.device is Gamepad;
            if (pad == UsingGamepad) return;
            UsingGamepad = pad;
            DeviceChanged?.Invoke(pad);
        }

        /// <summary>
        /// Enables exactly the actions of <paramref name="context"/>. Actions that stay enabled across the switch are
        /// not touched, so a button held during the change (e.g. Aim when entering the Aiming context) keeps its state.
        /// </summary>
        public void SetContext(InputContext context)
        {
            var wanted = new HashSet<InputAction>();
            if (context != InputContext.None) AddMap(wanted, Actions.System.Get());

            if (GameplayWhitelist.TryGetValue(context, out var allowed))
            {
                if (allowed == null) AddMap(wanted, Actions.Gameplay.Get());
                else
                    foreach (var name in allowed)
                        wanted.Add(Actions.asset.FindAction("Gameplay/" + name, true));
            }

            switch (context)
            {
                case InputContext.Inventory:
                    AddMap(wanted, Actions.Inventory.Get());
                    AddMap(wanted, Actions.UI.Get());
                    break;
                case InputContext.Hacking:
                    AddMap(wanted, Actions.Hacking.Get());
                    break;
                case InputContext.Menu:
                    AddMap(wanted, Actions.UI.Get());
                    break;
                case InputContext.Cinematic:
                    AddMap(wanted, Actions.Cinematic.Get());
                    break;
            }

            // Inventory with a gamepad: the left stick drives the slot cursor, so it must not move Lukas.
            // Changing a binding mask re-resolves the action, so only touch it when it actually changes.
            InputBinding? moveMask = context == InputContext.Inventory ? InputBinding.MaskByGroup("KeyboardMouse") : (InputBinding?)null;
            if (!Nullable.Equals(Actions.Gameplay.Move.bindingMask, moveMask)) Actions.Gameplay.Move.bindingMask = moveMask;

            foreach (var map in Actions.asset.actionMaps)
                foreach (var action in map.actions)
                {
                    bool want = wanted.Contains(action);
                    if (want && !action.enabled) action.Enable();
                    else if (!want && action.enabled) action.Disable();
                }

            if (Context == context) return;
            Context = context;
            HbLog.Dev("Input", $"Context -> {context}");
            ContextChanged?.Invoke(context);
        }

        private static void AddMap(HashSet<InputAction> set, InputActionMap map)
        {
            foreach (var a in map.actions) set.Add(a);
        }

        /// <summary>Keyboard/mouse rebinding overrides (D-120: gamepad map is fixed in the demo).</summary>
        public string SaveBindingOverrides() => Actions.asset.SaveBindingOverridesAsJson();

        public void LoadBindingOverrides(string json)
        {
            if (!string.IsNullOrEmpty(json)) Actions.asset.LoadBindingOverridesFromJson(json);
        }

        /// <summary>True if <paramref name="path"/> is already used by another enabled-together action of the same map.</summary>
        public bool IsBindingTakenInMap(InputAction action, string path)
        {
            foreach (var other in action.actionMap.actions)
            {
                if (other == action) continue;
                foreach (var b in other.bindings)
                    if (b.effectivePath == path) return true;
            }
            return false;
        }

        public void Dispose()
        {
            InputSystem.onActionChange -= OnActionChange;
            Actions.Disable();
            // The generated wrapper's Dispose calls Object.Destroy, which is invalid in edit mode (tests).
            if (UnityEngine.Application.isPlaying) Actions.Dispose();
            else UnityEngine.Object.DestroyImmediate(Actions.asset);
        }
    }
}
