using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.Backup
{
    public sealed class BackupImportModel : BackupModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly IBackupReader backup;

        public BackupImportModel(DatabaseUnlockedModel database, IBackupReader backup)
        {
            foreach (EntryPayload payload in backup.EnumerateEntries())
            {
                Entries.Add(new BackupModelEntry(payload));
            }

            this.database = database;
            this.backup = backup;
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
