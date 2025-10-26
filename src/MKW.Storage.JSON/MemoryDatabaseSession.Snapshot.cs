// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession
    {
        private sealed class Snapshot
            : SnapshotBase
            , IDatabaseSerializer
            , IDatabaseNG.ISnapshot
        {
            protected override JSONDatabase Database { get; }

            public Snapshot(JSONDatabase database)
            {
                Database = database;
            }
        }
    }
}
