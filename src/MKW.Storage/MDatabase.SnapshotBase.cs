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
            protected abstract IDatabaseBlobStore.ISnapshot ProxySnapshot { get; }

            public SnapshotBase(IDatabaseSerializer serializer)
                : base(serializer)
            {
            }

            // Entry
            public IEnumerable<DatabaseEntry> EnumerateEntries()
            {
                foreach (Blob blob in ProxySnapshot.Enumerate())
                {
                    if (blob is BlobSecretEntry entry)
                    {
                        yield return serializer.DeserializeEntry(entry);
                    }
                }
            }

            public DatabaseEntry OpenEntry(EntryId entryId)
            {
                foreach (Blob blob in ProxySnapshot.Enumerate())
                {
                    if (blob is BlobSecretEntry entry && blob.Id.Equals(entryId))
                    {
                        return serializer.DeserializeEntry(entry);
                    }
                }

                throw new EntryDoesNotExistException();
            }

            public bool HasEntry(EntryId entryId)
            {
                foreach (Blob blob in ProxySnapshot.Enumerate())
                {
                    if (blob is BlobSecretEntry && blob.Id.Equals(entryId))
                    {
                        return true;
                    }
                }

                return false;
            }

            // User
            public IEnumerable<DatabaseUser> EnumerateUsers()
            {
                foreach (Blob blob in ProxySnapshot.Enumerate())
                {
                    if (blob is BlobUser user)
                    {
                        yield return serializer.DeserializeUser(user);
                    }
                }
            }

            public DatabaseUser OpenUser(UserId userId)
            {
                foreach (Blob blob in ProxySnapshot.Enumerate())
                {
                    if (blob is BlobUser user && blob.Id.Equals(userId))
                    {
                        return serializer.DeserializeUser(user);
                    }
                }

                throw new UserDoesNotExistException();
            }

            public bool HasUser(UserId userId)
            {
                foreach (Blob blob in ProxySnapshot.Enumerate())
                {
                    if (blob is BlobUser && blob.Id.Equals(userId))
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
