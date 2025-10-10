using MKW.Core;
using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXmlV1;
using System.IO;

namespace MKW.GUI.Model
{
    public sealed class KeePassXmlV1Backup : IBackup, IDisposable
    {
        private readonly Stream stream;

        public KeePassXmlV1Backup(Stream stream)
        {
            this.stream = stream;
        }

        private static EntryPayloadKey ConvertPropertyName(string propname) => propname switch
        {
            KeePassXmlV1CommonFields.Title => CommonEntryPropertiesModel.Title.Key,
            KeePassXmlV1CommonFields.Username => CommonEntryPropertiesModel.Username.Key,
            KeePassXmlV1CommonFields.Password => CommonEntryPropertiesModel.Password.Key,
            KeePassXmlV1CommonFields.Url => CommonEntryPropertiesModel.Url.Key,
            KeePassXmlV1CommonFields.Notes => CommonEntryPropertiesModel.Notes.Key,
            _ => CommonEntryPropertiesModel.CustomPropertyNamespace.Branch(propname),
        };

        public IEnumerable<EntryPayload> EnumerateEntries()
        {
            using KeePassXmlV1Reader reader = new KeePassXmlV1Reader(stream);

            foreach (BackupEntry backupEntry in reader.GetEntriesEnumerator())
            {
                EntryPayload payload = new EntryPayload();

                foreach (KeyValuePair<string, string> property in backupEntry.Fields)
                {
                    payload.SetProperty(ConvertPropertyName(property.Key), property.Value);
                }

                yield return payload;
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
