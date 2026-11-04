using System.Collections.Generic;
using System.Linq;
using Heartbeat.InputRouting;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace Heartbeat.Tests
{
    public class InputRouterTests
    {
        private InputRouter router;

        [SetUp] public void SetUp() => router = new InputRouter();
        [TearDown] public void TearDown() => router.Dispose();

        private bool Enabled(string path) => router.Actions.asset.FindAction(path, true).enabled;

        [Test]
        public void Exploration_EnablesGameplay_AndPause_NotInventory()
        {
            router.SetContext(InputContext.Exploration);
            Assert.IsTrue(Enabled("Gameplay/Attack"));
            Assert.IsTrue(Enabled("System/Pause"));
            Assert.IsFalse(Enabled("Inventory/Use"));
            Assert.IsFalse(Enabled("Hacking/Up"));
        }

        [Test]
        public void Reading_OnlyClosesAndTurnsPages()
        {
            router.SetContext(InputContext.Reading);
            Assert.IsTrue(Enabled("Gameplay/Interact"));
            Assert.IsTrue(Enabled("UI/Cancel"));
            Assert.IsTrue(Enabled("UI/Navigate"));
            Assert.IsFalse(Enabled("Gameplay/Move"));
            Assert.IsFalse(Enabled("Gameplay/Look"));
            Assert.IsFalse(Enabled("Gameplay/Terminal"));
        }

        [Test]
        public void Inventory_DisablesAimAndAttack_KeepsMovement()
        {
            router.SetContext(InputContext.Inventory);
            Assert.IsFalse(Enabled("Gameplay/Aim"));
            Assert.IsFalse(Enabled("Gameplay/Attack"));
            Assert.IsTrue(Enabled("Gameplay/Move"));
            Assert.IsTrue(Enabled("Inventory/Use"));
        }

        [Test]
        public void Hacking_FreezesCharacter()
        {
            router.SetContext(InputContext.Hacking);
            Assert.IsFalse(Enabled("Gameplay/Move"));
            Assert.IsTrue(Enabled("Hacking/Left"));
            Assert.IsTrue(Enabled("Hacking/Abort"));
        }

        [Test]
        public void IntroPlayable_OnlyMoveLookInteract()
        {
            router.SetContext(InputContext.IntroPlayable);
            Assert.IsTrue(Enabled("Gameplay/Interact"));
            Assert.IsFalse(Enabled("Gameplay/Flashlight"));
            Assert.IsFalse(Enabled("Gameplay/Terminal"));
            Assert.IsFalse(Enabled("Gameplay/Aim"));
        }

        /// <summary>Regression: entering Aiming while Aim is held must not reset Gameplay actions (it toggled aim off).</summary>
        [Test]
        public void SwitchingBetweenContexts_DoesNotDisableSharedActions()
        {
            router.SetContext(InputContext.Exploration);
            int disabled = 0;
            System.Action<object, InputActionChange> onChange = (obj, change) =>
            {
                if (change == InputActionChange.ActionDisabled && obj is InputAction a && a.actionMap.name == "Gameplay") disabled++;
            };
            InputSystem.onActionChange += onChange;
            try
            {
                router.SetContext(InputContext.Aiming);
                router.SetContext(InputContext.Exploration);
            }
            finally { InputSystem.onActionChange -= onChange; }
            Assert.AreEqual(0, disabled);
        }

        [Test]
        public void None_DisablesEverything()
        {
            router.SetContext(InputContext.Exploration);
            router.SetContext(InputContext.None);
            Assert.IsFalse(router.Actions.asset.actionMaps.SelectMany(m => m.actions).Any(a => a.enabled));
        }

        /// <summary>GDD Controles: "ninguna tiene dos funciones dentro del mismo contexto".</summary>
        [Test]
        public void NoKeyHasTwoFunctions_InAnyContext([Values] InputContext context)
        {
            router.SetContext(context);
            var seen = new Dictionary<string, string>();
            foreach (var action in router.Actions.asset.actionMaps.SelectMany(m => m.actions).Where(a => a.enabled))
            {
                // UI navigation intentionally overlaps with the Inventory map (the UI module drives the same widgets).
                if (action.actionMap.name == "UI") continue;
                foreach (var b in action.bindings)
                {
                    if (b.isComposite || string.IsNullOrEmpty(b.effectivePath)) continue;
                    if (action.bindingMask.HasValue && !action.bindingMask.Value.Matches(b)) continue;
                    string key = b.effectivePath;
                    string full = action.actionMap.name + "/" + action.name;
                    if (seen.TryGetValue(key, out var other) && other != full)
                        Assert.Fail($"{context}: '{key}' bound to both {other} and {full}");
                    seen[key] = full;
                }
            }
        }
    }
}
