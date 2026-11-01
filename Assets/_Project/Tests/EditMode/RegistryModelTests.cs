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
    }
}
