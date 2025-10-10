using MKW.GUI.Model;

namespace MKW.GUI.Backup
{
    public sealed class BackupExportModel : BackupModelBase, IDisposable
    {
        public BackupExportModel(DatabaseUnlockedModel database)
        {
            foreach (EntryEditorModel entry in database.Entries)
            {
                Entries.Add(new BackupModelEntry(entry.GetPayload()));
            }
        }

        public void Export(IBackupWriter writer)
        {
            foreach (BackupModelEntry entry in Entries)
            {
                writer.WriteEntry(entry.Payload);
            }
        }

        public void Dispose()
        {
        }
    }
}
