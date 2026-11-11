namespace Heartbeat.Save
{
    public enum OptionsSection { Audio, Video, Controls, Accessibility, Game }

    /// <summary>"Restore defaults" per Options section (UI y Menús: the confirmation names the section). Pure, testable.</summary>
    public static class OptionsDefaults
    {
        public static void Reset(GlobalData g, OptionsSection section)
        {
            var d = new GlobalData();
            switch (section)
            {
                case OptionsSection.Audio:
                    g.volMaster = d.volMaster; g.volMusic = d.volMusic; g.volSfx = d.volSfx; g.volAmbience = d.volAmbience; g.volVoices = d.volVoices;
                    break;
                case OptionsSection.Video:
                    g.brightness = d.brightness; g.resolutionWidth = d.resolutionWidth; g.resolutionHeight = d.resolutionHeight;
                    g.screenMode = d.screenMode; g.vsync = d.vsync; g.fpsLimit = d.fpsLimit; g.shadowQuality = d.shadowQuality;
                    break;
                case OptionsSection.Controls:
                    g.lookSensitivity = d.lookSensitivity; g.aimSensitivity = d.aimSensitivity; g.invertY = d.invertY;
                    g.crouchToggle = d.crouchToggle; g.aimAssistMouse = d.aimAssistMouse; g.aimAssistGamepad = d.aimAssistGamepad; g.bindingOverrides = d.bindingOverrides;
                    break;
                case OptionsSection.Accessibility:
                    g.textScale = d.textScale; g.dyslexiaFont = d.dyslexiaFont; g.subtitleOpaque = d.subtitleOpaque; g.soundCaptions = d.soundCaptions;
                    g.terminalScale = d.terminalScale; g.reduceMotion = d.reduceMotion; g.reduceFlashes = d.reduceFlashes; g.reduceCrt = d.reduceCrt;
                    break;
                case OptionsSection.Game:
                    g.language = d.language;
                    break;
            }
        }
    }
}
