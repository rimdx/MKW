// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using System.Text.Json;

namespace MKW.Storage.JSON
{
    public partial class JSONDatabaseSession
    {
        // User
        public Blob OpenUser2(BlobId id)
        {
            UserId userId = UserId.FromBytes(id.GetBytes());
            return SerializeUser2(OpenUser(userId));
        }

        public void CreateUser2(Blob user)
        {
            UserId userId = UserId.FromBytes(user.Id.GetBytes());
            CreateUser(userId, DeserializeUser2(user));
        }

        public void UpdateUser2(Blob user)
        {
            UserId userId = UserId.FromBytes(user.Id.GetBytes());
            UpdateUser(userId, DeserializeUser2(user));
        }

        public IEnumerable<Blob> EnumerateUsers2()
        {
            foreach (DatabaseUser user in EnumerateUsers())
            {
                yield return SerializeUser2(user);
            }
        }

        public Blob SerializeUser2(DatabaseUser user)
        {
            JSONDatabaseUser obj = JSONDatabaseUser.Serialize(user);

            return new BlobUser
            {
                Id = BlobId.From(user.Id),
                Data = JsonSerializer.SerializeToUtf8Bytes(obj),
            };
        }

        public DatabaseUser DeserializeUser2(Blob blob)
        {
            UserId userId = UserId.FromBytes(blob.Id.GetBytes());
            JSONDatabaseUser? obj = JsonSerializer.Deserialize<JSONDatabaseUser>(blob.Data.Span);
            return JSONDatabaseUser.Deserialize(userId, obj);
        }

        // Entry
        public Blob OpenEntry2(BlobId id)
        {
            EntryId entryId = EntryId.FromBytes(id.GetBytes().Span);
            return SerializeEntry2(OpenEntry(entryId));
        }

        public void CreateEntry2(Blob entry)
        {
            EntryId entryId = EntryId.FromBytes(entry.Id.GetBytes().Span);
            CreateEntry(entryId, DeserializeEntry2(entry));
        }

        public void UpdateEntry2(Blob entry)
        {
            EntryId entryId = EntryId.FromBytes(entry.Id.GetBytes().Span);
            UpdateEntry(entryId, DeserializeEntry2(entry));
        }

        public IEnumerable<Blob> EnumerateEntries2()
        {
            foreach (DatabaseEntry entry in EnumerateEntries())
            {
                yield return SerializeEntry2(entry);
            }
        }

        public Blob SerializeEntry2(DatabaseEntry entry)
        {
            JSONDatabaseSecretEntry obj = JSONDatabaseSecretEntry.Serialize(entry);

            return new BlobSecretEntry
            {
                Id = BlobId.From(entry.Id),
                Data = JsonSerializer.SerializeToUtf8Bytes(obj),
            };
        }

        public DatabaseEntry DeserializeEntry2(Blob blob)
        {
            EntryId entryId = EntryId.FromBytes(blob.Id.GetBytes().Span);
            JSONDatabaseSecretEntry? obj = JsonSerializer.Deserialize<JSONDatabaseSecretEntry>(blob.Data.Span);
            return JSONDatabaseSecretEntry.Deserialize(entryId, obj);
        }
    }
}
