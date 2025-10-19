// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Implementation;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class Entry
        : IEntrySession
        , IDisposable
    {
        private readonly IDatabase database;
        private readonly IUserSession user;
        private readonly EntryId entryId;

        private readonly EntryEncoder encoder;
        private readonly EntryDecoder decoder;

        public EntryId Id => entryId;

        private Entry(IDatabase database,
                      ClientCryptography crypto,
                      IUserSession user,
                      IAsymmetricPrivateTransformer privateKey,
                      EntryId entryId)
        {
            this.database = database;
            this.user = user;
            this.entryId = entryId;

            encoder = new EntryEncoder(crypto, database, user);
            decoder = new EntryDecoder(crypto, user, privateKey);
        }

        public static Entry Create(IDatabase database,
                                   ClientCryptography crypto,
                                   IUserSession user,
                                   IAsymmetricPrivateTransformer privateKey,
                                   EntryId entryId)
        {
            DatabaseEntry entry = new DatabaseEntry
            {
                Id = entryId,
                Data = ReadOnlyMemory<byte>.Empty,
                Salt = ReadOnlyMemory<byte>.Empty,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            database.CreateEntry(entryId, entry);

            return new Entry(database,
                             crypto,
                             user,
                             privateKey,
                             entryId);
        }

        public static Entry Open(IDatabase database,
                                 ClientCryptography crypto,
                                 IUserSession user,
                                 IAsymmetricPrivateTransformer privateKey,
                                 EntryId entryId)
        {
            DatabaseEntry entry = database.OpenEntry(entryId);

            return new Entry(database,
                             crypto,
                             user,
                             privateKey,
                             entryId);
        }

        public void UpdatePayload(EntryPayload payload)
        {
            DatabaseEntry entry = database.OpenEntry(entryId);

            DatabaseEntry newEntry = encoder.EncodeEntry(entry, payload);

            database.UpdateEntry(Id, newEntry);
        }

        public EntryPayload? OpenPayload()
        {
            DatabaseEntry entry = database.OpenEntry(entryId);
            return decoder.DecodeEntry(entry);
        }

        public IEnumerable<UserId> EnumerateAccess()
        {
            foreach (UserId userId in user.EnumerateTrustedUsers())
            {
                yield return userId;
            }
        }

        public void Dispose()
        {
        }
    }
}
