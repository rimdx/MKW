// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.Exceptions;

namespace MKW.Storage
{
    public sealed partial class MDatabase
    {
        private abstract class SnapshotBase
            : SerializerBase
            , IDatabaseSerializer
            , IDatabase3.ISnapshot
            , IDisposable
        {
            protected abstract IDatabaseBlobStore.ISnapshot BlobStoreSnapshot { get; }

            public SnapshotBase(IDatabaseSerializer serializer)
                : base(serializer)
            {
            }

            // Entry
            public IEnumerable<DatabaseEntry> EnumerateEntries()
            {
                foreach (BlobSecretEntry entry in BlobStoreSnapshot.EnumerateBlobEntry<BlobSecretEntry>())
                {
                    yield return serializer.DeserializeEntry(entry);
                }
            }

            public DatabaseEntry OpenEntry(EntryId entryId)
            {
                foreach (BlobSecretEntry entry in BlobStoreSnapshot.EnumerateBlobEntry<BlobSecretEntry>())
                {
                    if (entry.Id.Equals(entryId))
                    {
                        return serializer.DeserializeEntry(entry);
                    }
                }

                throw new EntryDoesNotExistException();
            }

            public bool HasEntry(EntryId entryId)
            {
                foreach (BlobSecretEntry entry in BlobStoreSnapshot.EnumerateBlobEntry<BlobSecretEntry>())
                {
                    if (entry.Id.Equals(entryId))
                    {
                        return true;
                    }
                }

                return false;
            }

            // User
            public IEnumerable<DatabaseUser> EnumerateUsers()
            {
                foreach (BlobUser user in BlobStoreSnapshot.EnumerateBlobEntry<BlobUser>())
                {
                    yield return serializer.DeserializeUser(user);
                }
            }

            public DatabaseUser OpenUser(UserId userId)
            {
                foreach (BlobUser user in BlobStoreSnapshot.EnumerateBlobEntry<BlobUser>())
                {
                    if (user.Id.Equals(userId))
                    {
                        return serializer.DeserializeUser(user);
                    }
                }

                throw new UserDoesNotExistException();
            }

            public bool HasUser(UserId userId)
            {
                foreach (BlobUser user in BlobStoreSnapshot.EnumerateBlobEntry<BlobUser>())
                {
                    if (user.Id.Equals(userId))
                    {
                        return true;
                    }
                }

                return false;
            }

            // Misc
            public abstract void Dispose();
        }
    }
}
