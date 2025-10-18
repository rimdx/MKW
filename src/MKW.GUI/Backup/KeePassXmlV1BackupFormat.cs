// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassXmlV1BackupFormat : IBackupFormat
    {
        public string Name => "KeePass XML (1.x)";

        public IReadOnlyList<string> FileExtensions => ["*.xml"];

        public IBackupReader OpenRead(Stream file)
        {
            return new KeePassXmlV1BackupReader(file);
        }

        public IBackupWriter OpenWrite(Stream file)
        {
            return new KeePassXmlV1BackupWriter(file);
        }
    }
}
