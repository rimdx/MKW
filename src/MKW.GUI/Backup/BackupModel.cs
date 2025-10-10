using MKW.Core;
using System.Collections.ObjectModel;

namespace MKW.GUI.Backup
{
    public class BackupModel : ViewModelBase, IDisposable
    {
        private readonly IUserSession user;
        private readonly IBackup backup;

        public ObservableCollection<BackupModelEntry> Entries { get; }

        public BackupModel(IUserSession user, IBackup backup)
        {
            this.user = user;
            this.backup = backup;

            Entries = [];

            foreach (EntryPayload payload in backup.EnumerateEntries())
            {
                Entries.Add(new BackupModelEntry(payload));
            }
        }

        public void Import()
        {
            foreach (BackupModelEntry entry in Entries)
            {
                using IEntrySession entrySession = user.CreateEntry();
                entrySession.UpdatePayload(entry.Payload);
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
            backup.Dispose();
        }
    }
}
