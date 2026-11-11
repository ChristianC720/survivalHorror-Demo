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
    /// Options (Guardado y Configuración, Accesibilidad, D-104, D-106, D-120): Audio, Video (resolution, mode, V-Sync, FPS, brightness, shadows), Controls (sensitivity,
    /// invert Y, crouch toggle/hold, aim assist, keyboard rebinding), Accessibility (subtitle size, terminal scale, reduced motion/flashes/CRT, dyslexia font,
    /// opaque subtitles, sound captions) and Game (language). Every change is saved to GlobalData at once and
    /// broadcast with GameEvents.OptionsChanged. Used from the main menu and the pause menu.
    /// </summary>
    public sealed class OptionsView : MonoBehaviour
    {
        private static readonly string[] RebindActions = { "Interact", "Flashlight", "Reload", "Crouch", "Sprint", "Surveillance", "Terminal", "Hacking/Abort", "Hacking/UseFault" };   // conflicts are checked inside each map (context)
        private static readonly string[] SectionKeys = { "UI.OPT.AUDIO", "UI.OPT.VIDEO", "UI.OPT.CONTROLS", "UI.OPT.ACCESSIBILITY", "UI.OPT.GAME" };

        private SaveService saves;
        private GlobalData g;
        private Color color;
        private Action onBack;
        private Transform dialogParent;
        private RectTransform parent;
        private RectTransform body;
        private OptionsSection section;
        private readonly Button[] sectionButtons = new Button[SectionKeys.Length];
        private TMP_Text preview;
        private int rebindEndedFrame = -1;
        private InputActionRebindingExtensions.RebindingOperation rebind;
        private readonly System.Collections.Generic.Dictionary<string, Button> rebindButtons = new System.Collections.Generic.Dictionary<string, Button>();

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
            UiKit.Row(tabs, 6).childForceExpandWidth = true;
            Button first = null;
            for (int i = 0; i < SectionKeys.Length; i++)
            {
                var s = (OptionsSection)i;
                var b = UiKit.Button(tabs, Loc.Get(SectionKeys[i]), 26, color, () => v.Show(s), 48);
                v.sectionButtons[i] = b;
                if (first == null) first = b;
            }
            v.body = UiKit.Rect("Body", parent);
            UiKit.Column(v.body, 6);
            var row = UiKit.Rect("Footer", parent);
            UiKit.Row(row, 12).childForceExpandWidth = true;
            UiKit.Button(row, Loc.Get("UI.OPT.RESET"), 28, UiKit.Amber, v.ConfirmReset, 52);
            UiKit.Button(row, Loc.Get("UI.MENU.BACK"), 28, color, v.Back, 52);
            v.Show(OptionsSection.Audio);
            UiKit.Select(first);
            return v;
        }

        private void OnDestroy() => rebind?.Dispose();

        /// <summary>Esc belongs to the rebind in progress, not to the screen behind it.</summary>
        public bool IsRebinding => rebind != null || rebindEndedFrame == Time.frameCount;

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
            for (int i = 0; i < sectionButtons.Length; i++) sectionButtons[i].GetComponent<UiFocusStyle>().SetPinned(i == (int)s);
            foreach (Transform c in body) Destroy(c.gameObject);
            rebindButtons.Clear();
            preview = null;
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
                    BuildVideo();
                    break;
                case OptionsSection.Controls:
                    SliderRow("UI.OPT.LOOK_SENS", 0.25f, 3f, g.lookSensitivity, x => g.lookSensitivity = x);
                    SliderRow("UI.OPT.AIM_SENS", 0.25f, 3f, g.aimSensitivity, x => g.aimSensitivity = x);
                    ToggleRow("UI.OPT.INVERT_Y", g.invertY, x => g.invertY = x);
                    ToggleRow("UI.OPT.CROUCH_TOGGLE", g.crouchToggle, x => g.crouchToggle = x);
                    // One row for the device in use: each device keeps its own setting and default.
                    bool pad = Services.Get<InputRouter>().UsingGamepad;
                    ToggleRow("UI.OPT.AIM_ASSIST", g.AimAssist(pad), x => g.SetAimAssist(pad, x));
                    foreach (var a in RebindActions) RebindRow(a);
                    break;
                case OptionsSection.Accessibility:
                    SliderRow("UI.OPT.TEXT_SCALE", 1f, 1.6f, g.textScale, x => { g.textScale = x; RefreshPreview(); });
                    ToggleRow("UI.OPT.SUB_OPAQUE", g.subtitleOpaque, x => { g.subtitleOpaque = x; RefreshPreview(); });
                    SliderRow("UI.OPT.TERMINAL_SCALE", 1f, 1.25f, g.terminalScale, x => g.terminalScale = x);
                    ToggleRow("UI.OPT.SOUND_CAPTIONS", g.soundCaptions, x => g.soundCaptions = x);
                    ToggleRow("UI.OPT.REDUCE_MOTION", g.reduceMotion, x => g.reduceMotion = x);
                    ToggleRow("UI.OPT.REDUCE_FLASHES", g.reduceFlashes, x => g.reduceFlashes = x);
                    ToggleRow("UI.OPT.REDUCE_CRT", g.reduceCrt, x => { g.reduceCrt = x; Render.Degradation.SetReduced(x); });
                    ToggleRow("UI.OPT.DYSLEXIA_FONT", g.dyslexiaFont, x => g.dyslexiaFont = x);
                    preview = UiKit.Text(body, Loc.Get("UI.OPT.PREVIEW"), 32, UiKit.Text1, false);
                    preview.alignment = TextAlignmentOptions.Center;
                    RefreshPreview();
                    break;
                case OptionsSection.Game:
                    var lang = Row("UI.OPT.LANGUAGE");
                    UiKit.Button(lang, Loc.Get("UI.OPT.LANG_EN"), 24, color, () => SetLanguage("en"), 36).GetComponent<LayoutElement>().flexibleWidth = 1;
                    UiKit.Button(lang, Loc.Get("UI.OPT.LANG_ES"), 24, color, () => SetLanguage("es"), 36).GetComponent<LayoutElement>().flexibleWidth = 1;
                    break;
            }
        }

        private void RefreshPreview()
        {
            if (preview == null) return;
            preview.fontSize = 32f * g.textScale;
            preview.text = g.subtitleOpaque ? $"<mark=#000000FF padding=\"12,12,4,4\">{Loc.Get("UI.OPT.PREVIEW")}</mark>" : Loc.Get("UI.OPT.PREVIEW");
        }

        private void BuildVideo()
        {
            var sizes = new System.Collections.Generic.List<Resolution>();
            foreach (var r in Screen.resolutions)
                if (!sizes.Exists(x => x.width == r.width && x.height == r.height)) sizes.Add(r);
            if (sizes.Count > 0)
            {
                var names = sizes.ConvertAll(r => $"{r.width}\u00D7{r.height}").ToArray();
                int current = sizes.FindIndex(r => r.width == g.resolutionWidth && r.height == g.resolutionHeight);
                CycleRow("UI.OPT.RESOLUTION", names, current >= 0 ? current : sizes.Count - 1, i =>
                {
                    g.resolutionWidth = sizes[i].width;
                    g.resolutionHeight = sizes[i].height;
                    Render.VideoSettings.Apply(g);
                });
            }
            CycleRow("UI.OPT.SCREEN_MODE", new[] { Loc.Get("UI.OPT.MODE_FULL"), Loc.Get("UI.OPT.MODE_WINDOW"), Loc.Get("UI.OPT.MODE_BORDERLESS") }, g.screenMode,
                i => { g.screenMode = i; Render.VideoSettings.Apply(g); });
            ToggleRow("UI.OPT.VSYNC", g.vsync, x => { g.vsync = x; Render.VideoSettings.Apply(g); });
            int[] limits = { 0, 30, 60, 120, 144 };
            CycleRow("UI.OPT.FPS", new[] { Loc.Get("UI.OPT.FPS_NONE"), "30", "60", "120", "144" }, Mathf.Max(0, System.Array.IndexOf(limits, g.fpsLimit)),
                i => { g.fpsLimit = limits[i]; Render.VideoSettings.Apply(g); });
            SliderRow("UI.OPT.BRIGHTNESS", Render.Brightness.Min, Render.Brightness.Max, g.brightness, x => { g.brightness = x; Render.Brightness.Apply(x); });
            CycleRow("UI.OPT.SHADOWS", new[] { Loc.Get("UI.OPT.LOW"), Loc.Get("UI.OPT.MEDIUM"), Loc.Get("UI.OPT.HIGH") }, g.shadowQuality, i => g.shadowQuality = i);
        }

        /// <summary>A value that advances with each press and wraps around.</summary>
        private void CycleRow(string key, string[] values, int index, Action<int> set)
        {
            var r = Row(key);
            int current = Mathf.Clamp(index, 0, values.Length - 1);
            Button b = null;
            b = UiKit.Button(r, $"< {values[current]} >", 24, color, () =>
            {
                current = (current + 1) % values.Length;
                b.GetComponentInChildren<TMP_Text>().text = $"< {values[current]} >";
                set(current);
                Commit();
            }, 36, true);
            b.GetComponent<LayoutElement>().flexibleWidth = 1;
        }

        private void SetLanguage(string code)
        {
            g.language = code;
            Loc.SetLanguage(code);
            Commit();
            // Rebuild the whole screen so section tabs and footer switch language too.
            foreach (Transform c in parent) if (c.name != "Scanlines" && c.name != "Text") Destroy(c.gameObject);
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
                if (section == OptionsSection.Video) { Render.Brightness.Apply(g.brightness); Render.VideoSettings.Apply(g); }
                if (section == OptionsSection.Accessibility) Render.Degradation.SetReduced(g.reduceCrt);
                if (section == OptionsSection.Game) Loc.SetLanguage(g.language);
                Commit();
                HbLog.Info("Options", $"Reset {section}");
                Show(section);
            }, null, color);

        // ---------- Rows ----------

        private RectTransform Row(string labelKey)
        {
            var r = UiKit.Rect("Row", body);
            UiKit.Row(r, 12).padding = new RectOffset(12, 12, 0, 0);
            r.gameObject.AddComponent<LayoutElement>().preferredHeight = 44;
            r.gameObject.AddComponent<Image>().color = new Color(color.r * 0.12f, color.g * 0.12f, color.b * 0.12f, 0.8f);
            var label = UiKit.Text(r, Loc.Get(labelKey), 26, color);
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
            }, 36, true);
            b.GetComponent<LayoutElement>().flexibleWidth = 1;
        }

        /// <summary>Keyboard rebinding (D-120: the gamepad map is fixed). A key already used in Gameplay is refused.</summary>
        private void RebindRow(string actionName)
        {
            var router = Services.Get<InputRouter>();
            var action = router.Actions.asset.FindAction(actionName.Contains("/") ? actionName : "Gameplay/" + actionName, true);
            var r = UiKit.Rect("Row", body);
            UiKit.Row(r, 12).padding = new RectOffset(12, 12, 0, 0);
            r.gameObject.AddComponent<LayoutElement>().preferredHeight = 36;
            r.gameObject.AddComponent<Image>().color = new Color(color.r * 0.12f, color.g * 0.12f, color.b * 0.12f, 0.8f);
            var label = UiKit.Text(r, actionName.Replace("/", " · "), 24, color);   // action names are English UI terms in the demo
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 420;
            Button b = null;
            b = UiKit.Button(r, Current(action), 24, color, () => StartRebind(action, b), 34, true);
            b.GetComponent<LayoutElement>().flexibleWidth = 1;
            rebindButtons[actionName] = b;
        }

        /// <summary>Playtests: same as clicking the rebind button of a Gameplay action (Controls section must be shown).</summary>
        public void DebugRebind(string actionName)
        {
            if (rebindButtons.TryGetValue(actionName, out var b) && b != null) b.onClick.Invoke();
        }

        public string DebugBinding(string actionName) =>
            Current(Services.Get<InputRouter>().Actions.asset.FindAction("Gameplay/" + actionName, true));

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
            rebindEndedFrame = Time.frameCount;
            if (wasEnabled) action.Enable();
        }
    }
}
