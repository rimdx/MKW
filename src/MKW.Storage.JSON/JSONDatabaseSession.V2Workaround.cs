// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using System.Text.Json;

namespace MKW.Storage.JSON
{
    public partial class JSONDatabaseSession
    {
        // User
        public BlobEntry OpenUser2(BlobId id)
        {
            UserId userId = UserId.FromBytes(id.GetBytes());
            return SerializeUser2(OpenUser(userId));
        }

        public void CreateUser2(BlobEntry user)
        {
            UserId userId = UserId.FromBytes(user.Id.GetBytes());
            CreateUser(userId, DeserializeUser2(user));
        }

        public void UpdateUser2(BlobEntry user)
        {
            UserId userId = UserId.FromBytes(user.Id.GetBytes());
            UpdateUser(userId, DeserializeUser2(user));
        }

        public IEnumerable<BlobEntry> EnumerateUsers2()
        {
            foreach (DatabaseUser user in EnumerateUsers())
            {
                yield return SerializeUser2(user);
            }
        }

        public BlobEntry SerializeUser2(DatabaseUser user)
        {
            JSONDatabaseUser obj = JSONDatabaseUser.Serialize(user);

            return new BlobEntry
            {
                Id = BlobId.From(user.Id),
                Data = JsonSerializer.SerializeToUtf8Bytes(obj),
            };
        }

        public DatabaseUser DeserializeUser2(BlobEntry blob)
        {
            UserId userId = UserId.FromBytes(blob.Id.GetBytes());
            JSONDatabaseUser? obj = JsonSerializer.Deserialize<JSONDatabaseUser>(blob.Data.Span);
            return JSONDatabaseUser.Deserialize(userId, obj);
        }

        // Entry
        public BlobEntry OpenEntry2(BlobId id)
        {
            EntryId entryId = EntryId.FromBytes(id.GetBytes().Span);
            return SerializeEntry2(OpenEntry(entryId));
        }

        public void CreateEntry2(BlobEntry entry)
        {
            EntryId entryId = EntryId.FromBytes(entry.Id.GetBytes().Span);
            CreateEntry(entryId, DeserializeEntry2(entry));
        }

        public void UpdateEntry2(BlobEntry entry)
        {
            EntryId entryId = EntryId.FromBytes(entry.Id.GetBytes().Span);
            UpdateEntry(entryId, DeserializeEntry2(entry));
        }

        public IEnumerable<BlobEntry> EnumerateEntries2()
        {
            foreach (DatabaseEntry entry in EnumerateEntries())
            {
                yield return SerializeEntry2(entry);
            }
        }

        public BlobEntry SerializeEntry2(DatabaseEntry entry)
        {
            JSONDatabaseSecretEntry obj = JSONDatabaseSecretEntry.Serialize(entry);

            return new BlobEntry
            {
                Id = BlobId.From(entry.Id),
                Data = JsonSerializer.SerializeToUtf8Bytes(obj),
            };
        }

        public DatabaseEntry DeserializeEntry2(BlobEntry blob)
        {
            EntryId entryId = EntryId.FromBytes(blob.Id.GetBytes().Span);
            JSONDatabaseSecretEntry? obj = JsonSerializer.Deserialize<JSONDatabaseSecretEntry>(blob.Data.Span);
            return JSONDatabaseSecretEntry.Deserialize(entryId, obj);
        }
    }
}
