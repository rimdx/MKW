// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession
    {
        private sealed class Snapshot
            : SnapshotBase
            , IDatabaseSerializer
            , IDatabase3.ISnapshot
            , IDisposable
        {
            protected override JSONDatabase Database { get; }

            public Snapshot(JSONDatabase database)
            {
                Database = database;
            }

            public override void Dispose()
            {
            }
        }
    }
}
