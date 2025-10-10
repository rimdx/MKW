using MKW.Core;
using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXmlV2;

namespace MKW.GUI.Model
{
    public sealed class KeePassXmlV2Backup : IBackup
    {
        private readonly KeePassXmlV2Reader reader;

        public KeePassXmlV2Backup(KeePassXmlV2Reader reader)
        {
            this.reader = reader;
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
    }
}
