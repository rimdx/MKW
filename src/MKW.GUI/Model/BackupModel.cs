using MKW.Core;
using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public class BackupModel : ViewModelBase
    {
        private readonly IUserSession user;
        private readonly KeePassXmlBackup backup;

        public ObservableCollection<BackupModelEntry> Entries { get; }

        public BackupModel(IUserSession user, KeePassXmlBackup backup)
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
    }
}
