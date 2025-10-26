// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public partial interface IDatabaseNG
    {
        public interface ISnapshot
            : IDatabaseSerializer
            , IDisposable
        {
            DatabaseUser OpenUser(UserId userId);
            bool HasUser(UserId id);
            IEnumerable<DatabaseUser> EnumerateUsers();

            DatabaseEntry OpenEntry(EntryId id);
            bool HasEntry(EntryId id);
            IEnumerable<DatabaseEntry> EnumerateEntries();
        }
    }
}
