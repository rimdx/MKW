// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public partial interface IDatabaseNG
    {
        public interface ITransaction
            : ISnapshot
            , IDatabaseSerializer
            , IDisposable
        {
            void CreateUser(DatabaseUser user);
            void UpdateUser(DatabaseUser user);
            bool DeleteUser(UserId id);

            void CreateEntry(DatabaseEntry entry);
            void UpdateEntry(DatabaseEntry entry);
            bool DeleteEntry(EntryId id);

            void Commit();
        }
    }
}
