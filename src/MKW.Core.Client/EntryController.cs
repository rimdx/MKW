// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class EntryController
    {
        private readonly IDatabase database;
        private readonly ClientCryptography crypto;
        private readonly IUserSession user;
        private readonly IAsymmetricPrivateTransformer privateKey;

        public EntryController(IDatabase database,
                               ClientCryptography crypto,
                               IUserSession user,
                               IAsymmetricPrivateTransformer privateKey)
        {
            this.database = database;
            this.crypto = crypto;
            this.user = user;
            this.privateKey = privateKey;
        }

        public void Create(EntryId entryId,
                           EntryPayload payload)
        {
            using EntryEncoder encoder = new EntryEncoder(crypto, database, user);

            DatabaseEntry entry = encoder.EncodeEntry(entryId, payload);

            database.CreateEntry(entryId, entry);
        }

        public void Update(EntryId entryId, EntryPayload payload)
        {
            using EntryEncoder encoder = new EntryEncoder(crypto, database, user);

            DatabaseEntry entry = encoder.EncodeEntry(entryId, payload);

            database.UpdateEntry(entryId, entry);
        }

        public EntryPayload? Open(EntryId entryId)
        {
            using EntryDecoder decoder = new EntryDecoder(crypto, user, privateKey);

            DatabaseEntry entry = database.OpenEntry(entryId);
            EntryPayload? payload = decoder.DecodeEntry(entry);

            return payload;
        }

        public void Dispose()
        {
        }
    }
}
