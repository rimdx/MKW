namespace MKW.Core.Storage.JSON.Types
{
    internal record class JSONDatabaseSecretEntry
    {
        public required IDictionary<Guid, ReadOnlyMemory<byte>> Keys { get; init; }

        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> Data { get; init; }

        public static JSONDatabaseSecretEntry Serialize(DatabaseEntry entry)
        {
            Dictionary<Guid, ReadOnlyMemory<byte>> keys = [];
            foreach (KeyValuePair<UserId, ReadOnlyMemory<byte>> pair in entry.Keys)
            {
                keys.Add(pair.Key.GetGuid(), pair.Value);
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
            foreach (KeyValuePair<Guid, ReadOnlyMemory<byte>> pair in entry.Keys)
            {
                keys.Add(UserId.FromGuid(pair.Key), pair.Value);
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
