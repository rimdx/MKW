// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace MKW.GUI.Backup
{
    public interface IBackupFormat
    {
        string Name { get; }
        IReadOnlyList<string> FileExtensions { get; }

        IBackupReader OpenRead(Stream file);
        IBackupWriter OpenWrite(Stream file);
    }
}
