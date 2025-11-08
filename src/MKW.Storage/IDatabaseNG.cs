// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public partial interface IDatabaseNG : IDisposable
    {
        ITransaction BeginTransaction();

        ISnapshot CreateSnapshot();

        DatabaseConfiguration GetConfiguration();

        Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken);
        void ReloadDatabaseFile();
    }
}
