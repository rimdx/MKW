using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXmlV2;
using MKW.Core.Serialization.KeePassXmlV1;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class KeePassXmlSerializerTests
    {
        [Test]
        public void SimpleTest()
        {
            FileStream stream = new FileStream("key_pass_export_v2.xml", FileMode.Open, FileAccess.Read);
            KeePassXmlReader reader = new KeePassXmlReader(stream);

            BackupEntry[] entries = reader.EnumerateEntries().ToArray();

            ClassicAssert.AreEqual(3, entries.Length);
        }

        [Test]
        public void ReadV1()
        {
            FileStream stream = File.OpenRead("key_pass_export_v1.xml");
            KeePassXmlV1Reader reader = new KeePassXmlV1Reader(stream);

            BackupEntry[] entries = reader.GetEntriesEnumerator().ToArray();

            ClassicAssert.AreEqual(5, entries.Length);
        }
    }
}
