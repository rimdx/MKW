using MKW.Core;
using MKW.GUI.Backup;

namespace MKW.GUI.Model
{
    public sealed class BackupImportModel : BackupModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly IBackupReader backup;

        public BackupImportModel(DatabaseUnlockedModel database, IBackupReader backup)
        {
            this.database = database;
            this.backup = backup;

            foreach (EntryPayload payload in backup.EnumerateEntries())
            {
                BackupModelEntry entry = new BackupModelEntry(payload);

                if (FindSimilar(payload) != null)
                {
                    entry.IsSelected = false;
                }

                Entries.Add(entry);
            }
        }

        private EntryEditorModel? FindSimilar(EntryPayload payload)
        {
            string title1 = payload.GetPropertyOrEmpty(CommonEntryPropertiesModel.Title.Key);
            string username1 = payload.GetPropertyOrEmpty(CommonEntryPropertiesModel.Username.Key);

            if (title1 == string.Empty && username1 == string.Empty)
            {
                return null;
            }

            foreach (EntryEditorModel entry in database.Entries)
            {
                EntryPayload entryPl = entry.GetPayload();

                string title2 = entryPl.GetPropertyOrEmpty(CommonEntryPropertiesModel.Title.Key);
                string username2 = entryPl.GetPropertyOrEmpty(CommonEntryPropertiesModel.Username.Key);

                if (title1 == title2 && username1 == username2)
                {
                    return entry;
                }
            }

            return null;
        }

        public void Import()
        {
            foreach (BackupModelEntry entry in Entries)
            {
                database.CreateEntry(EntryId.Create(), entry.Payload);
            }
        }

        public void Dispose()
        {
            backup.Dispose();
        }
    }
}
