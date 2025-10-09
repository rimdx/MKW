using MKW.Core;

namespace MKW.GUI.Model
{
    public class BackupModel : ViewModelBase
    {
        private readonly IUserSession user;
        private readonly KeePassXmlBackup backup;

        public BackupModel(IUserSession user, KeePassXmlBackup backup)
        {
            this.user = user;
            this.backup = backup;
        }

        public void Import()
        {
            foreach (EntryPayload payload in backup.EnumerateEntries())
            {
                using IEntrySession entrySession = user.CreateEntry();
                entrySession.UpdatePayload(payload);
            }
        }
    }
}
