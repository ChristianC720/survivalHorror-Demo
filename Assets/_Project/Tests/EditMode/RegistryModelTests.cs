using Heartbeat.Narrative;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class RegistryModelTests
    {
        [Test]
        public void EmiliaDocuments_CountTowardsSix()
        {
            var r = new RegistryModel();
            for (int i = 1; i <= 5; i++) r.Register($"DOC.EMILIA.0{i}", RecordKind.EmiliaDocument);
            r.Register("DOC.VITALIS.01", RecordKind.ContextDocument);
            Assert.AreEqual(5, r.EmiliaCount);
            Assert.IsFalse(r.HasAllEmilia);
            r.Register("DOC.EMILIA.06", RecordKind.EmiliaDocument);
            Assert.IsTrue(r.HasAllEmilia);
        }

        [Test]
        public void SameId_IsNotCountedTwice()
        {
            var r = new RegistryModel();
            Assert.IsTrue(r.Register("DOC.EMILIA.01", RecordKind.EmiliaDocument));
            Assert.IsFalse(r.Register("DOC.EMILIA.01", RecordKind.EmiliaDocument));
            Assert.AreEqual(1, r.EmiliaCount);
        }

        [Test]
        public void ExportImport_RoundTrips_InOrder()
        {
            var r = new RegistryModel();
            r.Register("A", RecordKind.Bestiary);
            r.Register("B", RecordKind.Confiscated);
            var copy = new RegistryModel();
            copy.Import(r.Export());
            CollectionAssert.AreEqual(new[] { "A", "B" }, copy.Order);
            Assert.AreEqual(RecordKind.Confiscated, copy.KindOf("B"));
        }

        [Test]
        public void RecordKinds_KeepTheirSavedNumbers()   // saves store the number: new kinds are appended (C-36, C-39)
        {
            Assert.AreEqual(6, (int)RecordKind.Ring);
            Assert.AreEqual(7, (int)RecordKind.Poster);
            Assert.AreEqual(8, (int)RecordKind.VoiceNote);
            var save = UnityEngine.JsonUtility.FromJson<Heartbeat.Save.RecordEntry>("{\"id\":\"RING\",\"kind\":6}");
            Assert.AreEqual(RecordKind.Ring, save.kind);
        }
    }
}
