// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using System.Buffers;

namespace MKW.Storage.MKPG
{
    public sealed class MKPGDatabase : IDatabase, IDisposable
    {
        private readonly IBlobStorage entries;
        private readonly IBlobStorage users;

        public MKPGDatabase()
        {
            entries = new BlobStorageSingleFile(new MemoryStream());
            users = new BlobStorageSingleFile(new MemoryStream());
        }

        // Entry
        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            EntrySerializer.Serialize(writer, entry);
            BlobEntry blob = new BlobEntry(BlobId.From(id),
                                           MKPGConstants.ArmourTypeHeaders.Entry,
                                           writer.WrittenMemory);

            entries.Create(blob);
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            throw new NotImplementedException();
        }

        public DatabaseEntry OpenEntry(EntryId id)
        {
            BlobEntry blob = entries.Open(BlobId.From(id));
            DatabaseEntry entry = EntrySerializer.Deserialize(blob.CreateReader(), id);
            return entry;
        }

        public bool DeleteEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseEntry> EnumerateEntries()
        {
            foreach (BlobEntry blob in entries.Enumerate())
            {
                EntryId id = EntryId.FromBytes(blob.Id.GetBytes().Span);
                DatabaseEntry entry = EntrySerializer.Deserialize(blob.CreateReader(), id);
                yield return entry;
            }
        }

        public bool HasEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        // User
        public void CreateUser(UserId id, DatabaseUser user)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            UserSerializer.Serialize(writer, user);
            BlobEntry blob = new BlobEntry(BlobId.From(id),
                                           MKPGConstants.ArmourTypeHeaders.User,
                                           writer.WrittenMemory);

            users.Create(blob);
        }

        public void UpdateUser(UserId id, DatabaseUser user)
        {
            throw new NotImplementedException();
        }

        public DatabaseUser OpenUser(UserId id)
        {
            BlobEntry blob = users.Open(BlobId.From(id));
            return UserSerializer.Deserialize(blob.CreateReader(), id);
        }

        public bool DeleteUser(UserId id)
        {
            return users.Delete(BlobId.From(id));
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            foreach (BlobEntry blob in users.Enumerate())
            {
                UserId id = UserId.FromBytes(blob.Id.GetBytes());
                yield return UserSerializer.Deserialize(blob.CreateReader(), id);
            }
        }

        public bool HasUser(UserId id)
        {
            return users.Exists(BlobId.From(id));
        }

        // Misc

        public void ReloadDatabaseFile()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
