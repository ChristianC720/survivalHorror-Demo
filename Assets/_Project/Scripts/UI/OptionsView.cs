using System;
using Heartbeat.Core;
using Heartbeat.InputRouting;
using Heartbeat.Save;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Heartbeat.UI
{
    /// <summary>
    /// Options (Guardado y Configuración, Accesibilidad, D-104, D-106, D-120): Audio, Video, Controls (sensitivity,
    /// invert Y, crouch toggle/hold, aim assist, keyboard rebinding), Accessibility (text size, dyslexia font,
    /// opaque subtitles, sound captions) and Game (language). Every change is saved to GlobalData at once and
    /// broadcast with GameEvents.OptionsChanged. Used from the main menu and the pause menu.
    /// shortcut: aim assist and the dyslexia font are stored but have no effect yet (no assist code, font not imported).
    /// </summary>
    public sealed class OptionsView : MonoBehaviour
    {
        private static readonly string[] RebindActions = { "Interact", "Flashlight", "Reload", "Crouch", "Sprint", "Surveillance", "Terminal" };
        private static readonly string[] SectionKeys = { "UI.OPT.AUDIO", "UI.OPT.VIDEO", "UI.OPT.CONTROLS", "UI.OPT.ACCESSIBILITY", "UI.OPT.GAME" };

        private SaveService saves;
        private GlobalData g;
        private Color color;
        private Action onBack;
        private Transform dialogParent;
        private RectTransform parent;
        private RectTransform body;
        private OptionsSection section;
        private InputActionRebindingExtensions.RebindingOperation rebind;

        /// <summary>Builds the options screen inside <paramref name="parent"/> (a vertical layout).</summary>
        public static OptionsView Build(RectTransform parent, Transform dialogParent, Color color, Action onBack)
        {
            var v = parent.gameObject.AddComponent<OptionsView>();
            v.saves = Services.Get<SaveService>();
            v.g = v.saves.LoadGlobal();
            v.color = color;
            v.onBack = onBack;
            v.dialogParent = dialogParent;
            v.parent = parent;

            var tabs = UiKit.Rect("Sections", parent);
            var h = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 6;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandHeight = false;
            tabs.gameObject.AddComponent<LayoutElement>().preferredHeight = 44;
            Button first = null;
            for (int i = 0; i < SectionKeys.Length; i++)
            {
                var s = (OptionsSection)i;
                var b = UiKit.Button(tabs, Loc.Get(SectionKeys[i]), 22, color, () => v.Show(s));
                if (first == null) first = b;
            }
            v.body = UiKit.Rect("Body", parent);
            UiKit.Column(v.body, 6);
            var row = UiKit.Rect("Footer", parent);
            var fh = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            fh.spacing = 12;
            fh.childControlWidth = fh.childControlHeight = true;
            fh.childForceExpandHeight = false;
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 48;
            UiKit.Button(row, Loc.Get("UI.OPT.RESET"), 24, UiKit.Amber, v.ConfirmReset);
            UiKit.Button(row, Loc.Get("UI.MENU.BACK"), 24, color, v.Back);
            v.Show(OptionsSection.Audio);
            UiKit.Select(first);
            return v;
        }

        private void OnDestroy() => rebind?.Dispose();

        private void Back()
        {
            rebind?.Cancel();
            onBack?.Invoke();
        }

        // ---------- Sections ----------

        private void Show(OptionsSection s)
        {
            rebind?.Cancel();
            section = s;
            foreach (Transform c in body) Destroy(c.gameObject);
            switch (s)
            {
                case OptionsSection.Audio:
                    SliderRow("UI.OPT.VOL_MASTER", 0f, 1f, g.volMaster, x => g.volMaster = x);
                    SliderRow("UI.OPT.VOL_MUSIC", 0f, 1f, g.volMusic, x => g.volMusic = x);
                    SliderRow("UI.OPT.VOL_SFX", 0f, 1f, g.volSfx, x => g.volSfx = x);
                    SliderRow("UI.OPT.VOL_AMBIENCE", 0f, 1f, g.volAmbience, x => g.volAmbience = x);
                    SliderRow("UI.OPT.VOL_VOICES", 0f, 1f, g.volVoices, x => g.volVoices = x);
                    break;
                case OptionsSection.Video:
                    SliderRow("UI.OPT.BRIGHTNESS", Render.Brightness.Min, Render.Brightness.Max, g.brightness, x => { g.brightness = x; Render.Brightness.Apply(x); });
                    break;
                case OptionsSection.Controls:
                    SliderRow("UI.OPT.LOOK_SENS", 0.25f, 3f, g.lookSensitivity, x => g.lookSensitivity = x);
                    SliderRow("UI.OPT.AIM_SENS", 0.25f, 3f, g.aimSensitivity, x => g.aimSensitivity = x);
                    ToggleRow("UI.OPT.INVERT_Y", g.invertY, x => g.invertY = x);
                    ToggleRow("UI.OPT.CROUCH_TOGGLE", g.crouchToggle, x => g.crouchToggle = x);
                    ToggleRow("UI.OPT.AIM_ASSIST", g.aimAssist, x => g.aimAssist = x);
                    foreach (var a in RebindActions) RebindRow(a);
                    break;
                case OptionsSection.Accessibility:
                    SliderRow("UI.OPT.TEXT_SCALE", 1f, 1.6f, g.textScale, x => g.textScale = x);
                    ToggleRow("UI.OPT.DYSLEXIA_FONT", g.dyslexiaFont, x => g.dyslexiaFont = x);
                    ToggleRow("UI.OPT.SUB_OPAQUE", g.subtitleOpaque, x => g.subtitleOpaque = x);
                    ToggleRow("UI.OPT.SOUND_CAPTIONS", g.soundCaptions, x => g.soundCaptions = x);
                    break;
                case OptionsSection.Game:
                    var lang = Row("UI.OPT.LANGUAGE");
                    UiKit.Button(lang, Loc.Get("UI.OPT.LANG_EN"), 22, color, () => SetLanguage("en"), 36);
                    UiKit.Button(lang, Loc.Get("UI.OPT.LANG_ES"), 22, color, () => SetLanguage("es"), 36);
                    break;
            }
        }

        private void SetLanguage(string code)
        {
            g.language = code;
            Loc.SetLanguage(code);
            Commit();
            // Rebuild the whole screen so section tabs and footer switch language too.
            foreach (Transform c in parent) Destroy(c.gameObject);
            var again = Build(parent, dialogParent, color, onBack);
            again.Show(OptionsSection.Game);
            Destroy(this);
        }

        private void Commit()
        {
            saves.SaveGlobal(g);
            GameEvents.RaiseOptionsChanged();
        }

        private void ConfirmReset() =>
            ConfirmDialog.Show(dialogParent, Loc.Get("UI.CONFIRM.RESET_SECTION", Loc.Get(SectionKeys[(int)section])), () =>
            {
                OptionsDefaults.Reset(g, section);
                if (section == OptionsSection.Controls) Services.Get<InputRouter>().Actions.asset.RemoveAllBindingOverrides();
                if (section == OptionsSection.Video) Render.Brightness.Apply(g.brightness);
                if (section == OptionsSection.Game) Loc.SetLanguage(g.language);
                Commit();
                HbLog.Info("Options", $"Reset {section}");
                Show(section);
            }, null, UiKit.Amber);

        // ---------- Rows ----------

        private RectTransform Row(string labelKey)
        {
            var r = UiKit.Rect("Row", body);
            var h = r.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 12;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandHeight = false;
            h.childAlignment = TextAnchor.MiddleLeft;
            r.gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
            var label = UiKit.Text(r, Loc.Get(labelKey), 24, color);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 420;
            return r;
        }

        private void SliderRow(string key, float min, float max, float value, Action<float> set)
        {
            var r = Row(key);
            var s = MenuView.MakeSlider(r, min, max, value, x => { set(x); Commit(); });
            s.GetComponent<LayoutElement>().flexibleWidth = 1;
        }

        private void ToggleRow(string key, bool value, Action<bool> set)
        {
            var r = Row(key);
            Button b = null;
            b = UiKit.Button(r, Loc.Get(value ? "UI.OPT.ON" : "UI.OPT.OFF"), 22, color, () =>
            {
                bool now = b.GetComponentInChildren<TMP_Text>().text != Loc.Get("UI.OPT.ON");
                b.GetComponentInChildren<TMP_Text>().text = Loc.Get(now ? "UI.OPT.ON" : "UI.OPT.OFF");
                set(now);
                Commit();
            }, 36);
        }

        /// <summary>Keyboard rebinding (D-120: the gamepad map is fixed). A key already used in Gameplay is refused.</summary>
        private void RebindRow(string actionName)
        {
            var router = Services.Get<InputRouter>();
            var action = router.Actions.asset.FindAction("Gameplay/" + actionName, true);
            var r = UiKit.Rect("Row", body);
            var h = r.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 12;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandHeight = false;
            r.gameObject.AddComponent<LayoutElement>().preferredHeight = 36;
            var label = UiKit.Text(r, actionName, 22, color);   // action names are English UI terms in the demo
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 420;
            Button b = null;
            b = UiKit.Button(r, Current(action), 22, color, () => StartRebind(action, b), 34);
        }

        private static string Current(InputAction a) =>
            a.GetBindingDisplayString(InputBinding.MaskByGroup("KeyboardMouse"));

        private void StartRebind(InputAction action, Button button)
        {
            rebind?.Cancel();
            int index = action.GetBindingIndex(InputBinding.MaskByGroup("KeyboardMouse"));
            if (index < 0) return;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.text = Loc.Get("UI.OPT.REBIND_WAIT");
            bool wasEnabled = action.enabled;
            action.Disable();
            var router = Services.Get<InputRouter>();
            string before = action.bindings[index].effectivePath;
            rebind = action.PerformInteractiveRebinding(index)
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithControlsExcluding("<Gamepad>")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(op => { text.text = Current(action); Finish(op, action, wasEnabled); })
                .OnComplete(op =>
                {
                    string path = action.bindings[index].effectivePath;
                    if (path != before && router.IsBindingTakenInMap(action, path))
                    {
                        action.ApplyBindingOverride(index, before);
                        text.text = Loc.Get("UI.OPT.REBIND_TAKEN");
                    }
                    else
                    {
                        text.text = Current(action);
                        g.bindingOverrides = router.SaveBindingOverrides();
                        Commit();
                        HbLog.Info("Options", $"Rebound {action.name} -> {path}");
                    }
                    Finish(op, action, wasEnabled);
                })
                .Start();
        }

        private void Finish(InputActionRebindingExtensions.RebindingOperation op, InputAction action, bool wasEnabled)
        {
            op.Dispose();
            if (rebind == op) rebind = null;
            if (wasEnabled) action.Enable();
        }
    }
}
