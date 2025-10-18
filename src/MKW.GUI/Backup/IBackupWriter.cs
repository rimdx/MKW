// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.GUI.Backup
{
    public interface IBackupWriter : IDisposable
    {
        void WriteEntry(EntryPayload payload);
    }
}
