using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXML;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class KeePassXmlSerializerTests
    {
        [Test]
        public void SimpleTest()
        {
            FileStream stream = new FileStream("key_pass_export.xml", FileMode.Open, FileAccess.Read);
            KeePassXmlReader reader = new KeePassXmlReader(stream);

            BackupEntry[] entries = reader.EnumerateEntries().ToArray();

            ClassicAssert.AreEqual(3, entries.Length);
        }
    }
}
