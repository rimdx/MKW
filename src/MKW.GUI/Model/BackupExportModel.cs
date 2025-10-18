// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Backup;

namespace MKW.GUI.Model
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
                if (entry.IsSelected)
                {
                    writer.WriteEntry(entry.Payload);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
