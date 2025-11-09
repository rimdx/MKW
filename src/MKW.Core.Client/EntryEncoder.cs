// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public class EntryEncoder : IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabaseNG.ISnapshot snapshot;
        private readonly UserTrustProvider trustProvider;

        public EntryEncoder(ClientCryptography crypto,
                            IDatabaseNG.ISnapshot snapshot,
                            UserTrustProvider trustProvider)
        {
            this.crypto = crypto;
            this.snapshot = snapshot;
            this.trustProvider = trustProvider;
        }

        public DatabaseEntry EncodeEntry(EntryId entryId, EntryPayload payload)
        {
            SymmetricKey sessionKey = crypto.CreateSymmetricKey();

            using ISymmetricTransformer payloadEncoder = crypto.OpenSymmetricTransformer(sessionKey);

            ReadOnlyMemory<byte> serializedPayload = EntryPayloadSerializer.Serialize(payload);
            ReadOnlyMemory<byte> data = payloadEncoder.Encrypt(serializedPayload.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];

            foreach (UserId userId in trustProvider.EnumerateTrustedUsers())
            {
                DatabaseUser user = snapshot.OpenUser(userId);

                AsymmetricPublicKey key = crypto.DecodePkcsPublicKey(user.ProtectedData.PublicKey.Span);

                using IAsymmetricPublicTransformer keyEncoder = crypto.OpenAsymmetricTransformer(key);

                ReadOnlyMemory<byte> encyptedKey = keyEncoder.Encrypt(sessionKey.Visit(new GetSymmetricKeyVisitor()).Span);

                keys.Add(user.Id, encyptedKey);
            }

            return new DatabaseEntry
            {
                Id = entryId,
                Keys = keys,
                Data = data,
                Salt = sessionKey.Visit(new GetSymmetricSaltVisitor()),
            };
        }

        public void Dispose()
        {
        }
    }
}
