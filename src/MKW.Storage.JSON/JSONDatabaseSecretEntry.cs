// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage.JSON
{
    internal sealed record class JSONDatabaseSecretEntry
    {
        public required IDictionary<string, ReadOnlyMemory<byte>> Keys { get; init; }

        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> Data { get; init; }

        public static JSONDatabaseSecretEntry Serialize(DatabaseEntry entry)
        {
            Dictionary<string, ReadOnlyMemory<byte>> keys = [];
            foreach (KeyValuePair<UserId, ReadOnlyMemory<byte>> pair in entry.Keys)
            {
                keys.Add(pair.Key.GetStringLegacy(), pair.Value);
            }

            return new JSONDatabaseSecretEntry
            {
                Keys = keys,
                Salt = entry.Salt,
                Data = entry.Data,
            };
        }

        public static DatabaseEntry Deserialize(EntryId id, JSONDatabaseSecretEntry entry)
        {
            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];
            foreach (KeyValuePair<string, ReadOnlyMemory<byte>> pair in entry.Keys)
            {
                keys.Add(UserId.FromStringLegacy(pair.Key), pair.Value);
            }

            return new DatabaseEntry
            {
                Id = id,
                Keys = keys,
                Salt = entry.Salt,
                Data = entry.Data,
            };
        }
    }
}
