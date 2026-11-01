using System.IO;
using Heartbeat.Cycle;
using Heartbeat.Inventory;
using Heartbeat.Narrative;
using Heartbeat.Save;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class SaveServiceTests
    {
        private string dir;
        private SaveService saves;

        [SetUp]
        public void SetUp()
        {
            dir = Path.Combine(Path.GetTempPath(), "hb_save_tests_" + System.Guid.NewGuid().ToString("N"));
            saves = new SaveService(dir);
        }

        [TearDown] public void TearDown() { if (Directory.Exists(dir)) Directory.Delete(dir, true); }

        [Test]
        public void Slot_RoundTrips_AllFields()
        {
            var d = new SlotData { mode = GameMode.Story, subjectCode = 9, deathCounter = 2, totalDeaths = 3, firstAttempt = false, hasAttemptSave = true };
            d.terminal.Add(new RecordEntry { id = "DOC.EMILIA.01", kind = RecordKind.EmiliaDocument });
            d.attemptSave.floor = 2;
            d.attemptSave.inventory.Add(new InventorySlot { Kind = ItemKind.Ammo, Count = 12 });
            d.attemptSave.revolverLoaded = 4;
            saves.Save(1, d);

            var l = saves.Load(1);
            Assert.AreEqual(GameMode.Story, l.mode);
            Assert.AreEqual("R-H-09", l.SubjectLabel);
            Assert.AreEqual(2, l.deathCounter);
            Assert.IsTrue(l.hasAttemptSave);
            Assert.AreEqual(2, l.attemptSave.floor);
            Assert.AreEqual(ItemKind.Ammo, l.attemptSave.inventory[0].Kind);
            Assert.AreEqual(12, l.attemptSave.inventory[0].Count);
            Assert.AreEqual(4, l.attemptSave.revolverLoaded);
            Assert.AreEqual("DOC.EMILIA.01", l.terminal[0].id);
        }

        [Test]
        public void SecondSave_KeepsBackup()
        {
            saves.Save(0, new SlotData { subjectCode = 7 });
            saves.Save(0, new SlotData { subjectCode = 8 });
            Assert.IsTrue(File.Exists(Path.Combine(dir, "slot_0.json.bak")));
            Assert.AreEqual(8, saves.Load(0).subjectCode);
        }

        [Test]
        public void CorruptFile_IsDamaged_AndBackupRestores()
        {
            saves.Save(0, new SlotData { subjectCode = 7 });
            saves.Save(0, new SlotData { subjectCode = 8 });
            File.WriteAllText(Path.Combine(dir, "slot_0.json"), "{ not json");
            Assert.AreEqual(SlotStatus.Damaged, saves.Status(0));
            Assert.IsNull(saves.Load(0));
            Assert.IsTrue(saves.RestoreBackup(0));
            Assert.AreEqual(7, saves.Load(0).subjectCode);
        }

        [Test]
        public void NewerVersion_IsTreatedAsDamaged()
        {
            File.WriteAllText(Path.Combine(dir, "slot_2.json"), "{\"saveVersion\":99,\"subjectCode\":7}");
            Assert.AreEqual(SlotStatus.Damaged, saves.Status(2));
        }

        [Test]
        public void EmptySlot_And_Delete()
        {
            Assert.AreEqual(SlotStatus.Empty, saves.Status(2));
            saves.Save(2, new SlotData());
            Assert.AreEqual(SlotStatus.Ok, saves.Status(2));
            saves.Delete(2);
            Assert.AreEqual(SlotStatus.Empty, saves.Status(2));
        }

        [Test]
        public void Global_RoundTrips()
        {
            var g = new GlobalData { lastUsedSlot = 2, brightnessCalibrated = true, language = "es" };
            saves.SaveGlobal(g);
            var l = saves.LoadGlobal();
            Assert.AreEqual(2, l.lastUsedSlot);
            Assert.AreEqual("es", l.language);
        }
    }

    public class DeathCounterTests
    {
        [Test]
        public void TutorialDeath_DoesNotCount_ButEndsFirstAttempt() // D-35
        {
            var s = new DeathCounter.State { SubjectCode = 7, FirstAttempt = true };
            Assert.IsFalse(DeathCounter.RegisterDeath(ref s));
            Assert.AreEqual(0, s.Consecutive);
            Assert.AreEqual(8, s.SubjectCode);
            Assert.IsFalse(s.FirstAttempt);
        }

        [Test]
        public void ScriptedIntroDeath_ChangesNothing() // D-63
        {
            var s = new DeathCounter.State { SubjectCode = 7, FirstAttempt = true };
            DeathCounter.RegisterDeath(ref s, scripted: true);
            Assert.AreEqual(7, s.SubjectCode);
            Assert.IsTrue(s.FirstAttempt);
        }

        [Test]
        public void FifthConsecutiveDeath_TriggersAssimilation() // M-18 acceptance
        {
            var s = new DeathCounter.State { SubjectCode = 8, Consecutive = 4 };
            Assert.IsTrue(DeathCounter.RegisterDeath(ref s));
        }

        [Test]
        public void Floor2Final_ResetsCounter()
        {
            var s = new DeathCounter.State { Consecutive = 4 };
            DeathCounter.ResetAtFloor2Final(ref s);
            Assert.IsFalse(DeathCounter.RegisterDeath(ref s));
            Assert.AreEqual(1, s.Consecutive);
        }

        [Test]
        public void Degradation_IsCapped()
        {
            var s = new DeathCounter.State { Degradation = DeathCounter.MaxDegradation };
            DeathCounter.RegisterDeath(ref s);
            Assert.AreEqual(DeathCounter.MaxDegradation, s.Degradation);
        }

        [Test]
        public void TutorialDeath_StillDegrades() // D-94
        {
            var s = new DeathCounter.State { FirstAttempt = true };
            DeathCounter.RegisterDeath(ref s);
            Assert.AreEqual(1, s.Degradation);
        }

        [Test]
        public void CalmExit_AddsCounterAndDegradation_KeepsSubjectAndAttempt() // D-31
        {
            var s = new DeathCounter.State { SubjectCode = 8, Consecutive = 1 };
            DeathCounter.RegisterCalmExit(ref s);
            Assert.AreEqual(2, s.Consecutive);
            Assert.AreEqual(1, s.Degradation);
            Assert.AreEqual(8, s.SubjectCode);
            Assert.AreEqual(0, s.Total);
        }

        [Test]
        public void CalmExit_InTutorial_OnlyDegrades() // D-94
        {
            var s = new DeathCounter.State { FirstAttempt = true };
            DeathCounter.RegisterCalmExit(ref s);
            Assert.AreEqual(0, s.Consecutive);
            Assert.AreEqual(1, s.Degradation);
            Assert.IsTrue(s.FirstAttempt);
        }
    }

    public class ExitRulesTests
    {
        [TestCase(true, false, ExitCost.Hunt)]
        [TestCase(true, true, ExitCost.Hunt)]      // chased into the final room: autosave still pending (D-96)
        [TestCase(false, false, ExitCost.Calm)]
        [TestCase(false, true, ExitCost.None)]
        public void ExitTable(bool hunting, bool inSavedFinalRoom, ExitCost expected) // D-31
        {
            Assert.AreEqual(expected, ExitRules.Evaluate(hunting, inSavedFinalRoom));
        }
    }
}
