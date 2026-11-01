using Heartbeat.Save;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class OptionsDefaultsTests
    {
        [Test]
        public void ResetAudio_OnlyTouchesAudio()
        {
            var g = new GlobalData { volMaster = 0.1f, volMusic = 0.2f, brightness = 1.4f, invertY = true, language = "es", brightnessCalibrated = true };
            OptionsDefaults.Reset(g, OptionsSection.Audio);
            Assert.AreEqual(1f, g.volMaster);
            Assert.AreEqual(0.8f, g.volMusic);
            Assert.AreEqual(1.4f, g.brightness);
            Assert.IsTrue(g.invertY);
            Assert.AreEqual("es", g.language);
            Assert.IsTrue(g.brightnessCalibrated);
        }

        [Test]
        public void ResetControls_ClearsRebinds()
        {
            var g = new GlobalData { bindingOverrides = "{json}", crouchToggle = false };
            OptionsDefaults.Reset(g, OptionsSection.Controls);
            Assert.AreEqual("", g.bindingOverrides);
            Assert.IsTrue(g.crouchToggle);
        }
    }
}
