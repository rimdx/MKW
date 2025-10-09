using MKW.Core;
using MKW.Core.Serialization;
using MKW.Core.Serialization.KeePassXML;

namespace MKW.GUI.Model
{
    public sealed class KeePassXmlImporter
    {
        private readonly IUserSession user;

        public KeePassXmlImporter(IUserSession user)
        {
            this.user = user;
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

        public void Import(KeePassXmlReader data)
        {
            foreach (BackupEntry backupEntry in data.EnumerateEntries())
            {
                EntryPayload payload = new EntryPayload();

                foreach (KeyValuePair<string, string> property in backupEntry.Fields)
                {
                    payload.SetProperty(ConvertPropertyName(property.Key), property.Value);
                }

                using IEntrySession entrySession = user.CreateEntry();
                entrySession.UpdatePayload(payload);
            }
        }
    }
}
