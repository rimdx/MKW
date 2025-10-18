// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassXmlV2BackupFormat : IBackupFormat
    {
        public string Name => "KeePass XML (2.x)";

        public IReadOnlyList<string> FileExtensions => ["*.xml"];

        public IBackupReader OpenRead(Stream file)
        {
            return new KeePassXmlV2Backup(file);
        }

        public IBackupWriter OpenWrite(Stream file)
        {
            throw new NotSupportedException();
        }
    }
}
