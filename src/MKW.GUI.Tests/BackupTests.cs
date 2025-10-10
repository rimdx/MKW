using MKW.Common;
using MKW.Core;
using MKW.GUI.Backup;
using MKW.GUI.Model;
using NUnit.Framework.Legacy;
using System.Text;

namespace MKW.GUI.Tests
{
    [TestFixture]
    public class BackupTests
    {
        [Test]
        public void KeePassXmlV1Test()
        {
            IBackupFormat format = CommonBackupFormats.KeePassXmlV1;

            EntryPayload entry = new EntryPayload();
            entry.SetProperty(CommonEntryPropertiesModel.Title.Key, "Example Title");
            entry.SetProperty(CommonEntryPropertiesModel.Username.Key, "Example Username");
            entry.SetProperty(CommonEntryPropertiesModel.Password.Key, "Example Password");
            entry.SetProperty(CommonEntryPropertiesModel.Url.Key, "https://example.com/");
            entry.SetProperty(CommonEntryPropertiesModel.Notes.Key, "Example Notes");
            entry.SetProperty(CommonEntryPropertiesModel.CustomPropertyNamespace.Branch("CustomField1"), "Custom Value 1");

            using MemoryStream stream = new MemoryStream();

            using (IBackupWriter writer = format.OpenWrite(new StreamDisown(stream)))
            {
                writer.WriteEntry(entry);
                writer.WriteEntry(entry);
            }

            Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));

            stream.Seek(0, SeekOrigin.Begin);

            using IBackupReader reader = format.OpenRead(new StreamDisown(stream));

            List<EntryPayload> entries = [.. reader.EnumerateEntries()];

            CollectionAssert.AreEqual(
                new[]
                {
                    entry,
                    entry
                },
                entries);
        }
    }
}
