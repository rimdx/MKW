using MKW.Core;
using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI.Backup
{
    public class BackupModel : ViewModelBase, IDisposable
    {
        private readonly IBackupReader reader;
        private readonly DatabaseUnlockedModel database;

        public ObservableCollection<BackupModelEntry> Entries { get; }

        public BackupModel(DatabaseUnlockedModel database, IBackupReader backup)
        {
            this.database = database;
            reader = backup;

            Entries = [];

            foreach (EntryPayload payload in backup.EnumerateEntries())
            {
                Entries.Add(new BackupModelEntry(payload));
            }
        }

        public BackupModel(DatabaseUnlockedModel database)
        {
            this.database = database;

            Entries = [];

            foreach (EntryEditorModel entry in database.Entries)
            {
                Entries.Add(new BackupModelEntry(entry.GetPayload()));
            }
        }

        public void Import()
        {
            foreach (BackupModelEntry entry in Entries)
            {
                database.CreateEntry(EntryId.Create(), entry.Payload);
            }
        }

        public void Export(IBackupWriter writer)
        {
            foreach (BackupModelEntry entry in Entries)
            {
                writer.WriteEntry(entry.Payload);
            }
        }

        public void SetSelectedAll(bool isSelected)
        {
            foreach (BackupModelEntry entry in Entries)
            {
                entry.IsSelected = isSelected;
            }
        }

        public bool? GetSelectedAll()
        {
            bool? isSelected = null;

            foreach (BackupModelEntry entry in Entries)
            {
                if (isSelected is null)
                {
                    isSelected = entry.IsSelected;
                }
                else if (isSelected != entry.IsSelected)
                {
                    return null;
                }
            }

            return isSelected;
        }

        public void Dispose()
        {
            reader?.Dispose();
        }
    }
}
