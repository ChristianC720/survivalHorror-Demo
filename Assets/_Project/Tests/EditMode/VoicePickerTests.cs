using Heartbeat.Audio;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class VoicePickerTests
    {
        [Test]
        public void FreeVoice_IsUsedFirst()
        {
            Assert.AreEqual(1, VoicePicker.Pick(new[] { true, false }, new[] { 1, 1 }, new[] { 0f, 0f }, 4));
        }

        [Test]
        public void Signal_StealsLeastImportant_OldestOnTie()
        {
            var busy = new[] { true, true, true };
            var prio = new[] { 2, 3, 3 };
            var start = new[] { 0f, 5f, 1f };
            Assert.AreEqual(2, VoicePicker.Pick(busy, prio, start, (int)CuePriority.Signal));
        }

        [Test]
        public void Music_NeverStealsSignals_AndIsDropped() // M-10: signals always audible
        {
            Assert.AreEqual(-1, VoicePicker.Pick(new[] { true, true }, new[] { 1, 2 }, new[] { 0f, 0f }, (int)CuePriority.Music));
        }
    }
}
