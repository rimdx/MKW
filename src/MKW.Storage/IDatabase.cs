// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public interface IDatabase : IDatabaseNG
    {
        // User Management
        DatabaseUser OpenUser(UserId id);

        IEnumerable<DatabaseUser> EnumerateUsers();

        ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj);
    }
}
