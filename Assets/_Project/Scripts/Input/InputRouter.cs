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

        public InputRouter()
        {
            Actions = new HeartbeatInput();
            SetContext(InputContext.None);
        }

        public void SetContext(InputContext context)
        {
            Actions.Disable();

            // Inventory with a gamepad: the left stick drives the slot cursor, so it must not move Lukas.
            Actions.Gameplay.Move.bindingMask = context == InputContext.Inventory
                ? InputBinding.MaskByGroup("KeyboardMouse")
                : (InputBinding?)null;

            if (context != InputContext.None) Actions.System.Enable();

            if (GameplayWhitelist.TryGetValue(context, out var allowed))
            {
                if (allowed == null) Actions.Gameplay.Enable();
                else
                    foreach (var name in allowed)
                        Actions.asset.FindAction("Gameplay/" + name, true).Enable();
            }

            switch (context)
            {
                case InputContext.Inventory:
                    Actions.Inventory.Enable();
                    Actions.UI.Enable();
                    break;
                case InputContext.Hacking:
                    Actions.Hacking.Enable();
                    break;
                case InputContext.Menu:
                    Actions.UI.Enable();
                    break;
                case InputContext.Cinematic:
                    Actions.Cinematic.Enable();
                    break;
            }

            if (Context == context) return;
            Context = context;
            HbLog.Dev("Input", $"Context -> {context}");
            ContextChanged?.Invoke(context);
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
            Actions.Disable();
            // The generated wrapper's Dispose calls Object.Destroy, which is invalid in edit mode (tests).
            if (UnityEngine.Application.isPlaying) Actions.Dispose();
            else UnityEngine.Object.DestroyImmediate(Actions.asset);
        }
    }
}
