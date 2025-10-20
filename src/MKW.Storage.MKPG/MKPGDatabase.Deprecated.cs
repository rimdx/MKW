// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage.MKPG
{
    public partial class MKPGDatabase
    {
        public void CreateUser(UserId id, DatabaseUser user)
        {
            CreateUser2(SerializeUser(user));
        }

        public void UpdateUser(UserId id, DatabaseUser user)
        {
            UpdateUser2(SerializeUser(user));
        }

        public DatabaseUser OpenUser(UserId id)
        {
            return DeserializeUser(OpenUser2(BlobId.From(id)));
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            foreach (BlobEntry user in EnumerateUsers2())
            {
                yield return DeserializeUser(user);
            }
        }

        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            CreateEntry2(SerializeEntry(entry));
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            UpdateEntry2(SerializeEntry(entry));
        }

        public DatabaseEntry OpenEntry(EntryId id)
        {
            return DeserializeEntry(OpenEntry2(BlobId.From(id)));
        }

        public IEnumerable<DatabaseEntry> EnumerateEntries()
        {
            foreach (BlobEntry blob in EnumerateEntries2())
            {
                yield return DeserializeEntry(blob);
            }
        }
    }
}
