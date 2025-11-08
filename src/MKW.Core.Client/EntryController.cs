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
        private readonly UserId userId;
        private readonly UserTrustProvider trustProvider;
        private readonly IAsymmetricPrivateTransformer privateKey;

        public EntryController(IDatabase database,
                               ClientCryptography crypto,
                               UserId userId,
                               UserTrustProvider trustProvider,
                               IAsymmetricPrivateTransformer privateKey)
        {
            this.database = database;
            this.crypto = crypto;
            this.userId = userId;
            this.trustProvider = trustProvider;
            this.privateKey = privateKey;
        }

        public void Create(EntryId entryId,
                           EntryPayload payload)
        {
            using (IDatabaseNG.ITransaction transaction = database.BeginTransaction())
            {
                using EntryEncoder encoder = new EntryEncoder(crypto, transaction.Snapshot, trustProvider);

                DatabaseEntry entry = encoder.EncodeEntry(entryId, payload);

                transaction.CreateEntry(entry);
                transaction.Commit();
            }
        }

        public void Update(EntryId entryId, EntryPayload payload)
        {
            using (IDatabaseNG.ITransaction transaction = database.BeginTransaction())
            {
                using EntryEncoder encoder = new EntryEncoder(crypto, transaction.Snapshot, trustProvider);

                DatabaseEntry entry = encoder.EncodeEntry(entryId, payload);

                transaction.UpdateEntry(entry);
                transaction.Commit();
            }
        }

        public EntryPayload? Open(EntryId entryId)
        {
            IDatabaseNG.ISnapshot snapshot = database.CreateSnapshot();
            using EntryDecoder decoder = new EntryDecoder(crypto, userId, privateKey);

            DatabaseEntry entry = snapshot.OpenEntry(entryId);
            EntryPayload? payload = decoder.DecodeEntry(entry);

            return payload;
        }

        public void Dispose()
        {
        }
    }
}
