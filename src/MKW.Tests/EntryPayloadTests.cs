using MKW.Core;
using MKW.Core.Exceptions;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class EntryPayloadTests
    {
        [Test]
        public void EntryPayloadKeyParseTests()
        {
            EntryPayloadKey key = new EntryPayloadKey("a:b:c");
            ClassicAssert.AreEqual("a:b:c", key.ToString());

            EntryPayloadKey branch = key.Branch("x");

            Assert.Throws<InvalidEntryPayloadKey>(() => new EntryPayloadKey("a:b:c:"));
            Assert.Throws<InvalidEntryPayloadKey>(() => new EntryPayloadKey(""));
            Assert.Throws<InvalidEntryPayloadKey>(() => new EntryPayloadKey("ъъ"));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch(""));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch("ъъ"));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch("y:z"));
            Assert.Throws<InvalidEntryPayloadKey>(() => branch.Branch(":"));
        }
    }
}
