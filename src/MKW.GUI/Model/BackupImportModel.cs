// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Backup;

namespace MKW.GUI.Model
{
    public sealed class BackupImportModel : BackupModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly IUserSession user;
        private readonly IBackupReader backup;

        public BackupImportModel(DatabaseUnlockedModel database,
                                 IUserSession user,
                                 IBackupReader backup)
        {
            this.database = database;
            this.user = user;
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
                if (entry.IsSelected)
                {
                    using IEntrySession newEntry = user.CreateEntry();
                    newEntry.UpdatePayload(entry.Payload);
                }
            }

            database.RefreshEntries();
        }

        public void Dispose()
        {
            backup.Dispose();
        }
    }
}
