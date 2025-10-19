// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Implementation
{
    public class EntryEncoder : IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;
        private readonly IUserSession user;

        public EntryEncoder(ClientCryptography crypto,
                            IDatabase database,
                            IUserSession user)
        {
            this.crypto = crypto;
            this.database = database;
            this.user = user;
        }

        public DatabaseEntry EncodeEntry(DatabaseEntry entry, EntryPayload payload)
        {
            SymmetricKey sessionKey = crypto.CreateSymmetricKey();

            using ISymmetricTransformer payloadEncoder = crypto.OpenSymmetricTransformer(sessionKey);

            ReadOnlyMemory<byte> serializedPayload = EntryPayloadSerializer.Serialize(payload);
            ReadOnlyMemory<byte> data = payloadEncoder.Encrypt(serializedPayload.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];

            foreach (UserId userId in user.EnumerateTrustedUsers())
            {
                DatabaseUser user = database.OpenUser(userId);

                AsymmetricPublicKey key = crypto.DecodePkcsPublicKey(user.PublicKey.Payload.Span);

                using IAsymmetricPublicTransformer keyEncoder = crypto.OpenAsymmetricTransformer(key);

                Memory<byte> encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey().Span);

                keys.Add(user.Id, encyptedKey);
            }

            return entry with
            {
                Keys = keys,
                Data = data,
                Salt = payloadEncoder.ExportIV(),
            };
        }

        public void Dispose()
        {
        }
    }
}
