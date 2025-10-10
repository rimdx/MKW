using MKW.Core;
using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXmlV2;
using MKW.GUI.Model;
using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassXmlV2Backup : IBackupReader, IDisposable
    {
        private readonly KeePassXmlV2Reader reader;
        private readonly Stream file;

        public KeePassXmlV2Backup(Stream file)
        {
            this.file = file;
            reader = new KeePassXmlV2Reader(file);
        }

        private static EntryPayloadKey ConvertPropertyName(string propname) => propname switch
        {
            KeePassCommonFields.Title => CommonEntryPropertiesModel.Title.Key,
            KeePassCommonFields.Username => CommonEntryPropertiesModel.Username.Key,
            KeePassCommonFields.Password => CommonEntryPropertiesModel.Password.Key,
            KeePassCommonFields.Url => CommonEntryPropertiesModel.Url.Key,
            KeePassCommonFields.Notes => CommonEntryPropertiesModel.Notes.Key,
            _ => CommonEntryPropertiesModel.CustomPropertyNamespace.Branch(propname),
        };

        public IEnumerable<EntryPayload> EnumerateEntries()
        {
            reader.Reset();

            foreach (BackupEntry backupEntry in reader.EnumerateEntries())
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
            reader.Dispose();
            file.Dispose();
        }
    }
}
