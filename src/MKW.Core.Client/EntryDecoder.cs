// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public class EntryDecoder : IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly UserId userId;
        private readonly IAsymmetricPrivateTransformer transformer;

        public EntryDecoder(ClientCryptography crypto,
                            UserId userId,
                            IAsymmetricPrivateTransformer transformer)
        {
            this.crypto = crypto;
            this.userId = userId;
            this.userId = userId;
            this.transformer = transformer;
        }

        public EntryPayload? DecodeEntry(DatabaseEntry entry)
        {
            if (entry.Keys.TryGetValue(userId, out ReadOnlyMemory<byte> encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            ReadOnlyMemory<byte> decryptedKey = transformer.Decrypt(encodedKey.Span);

            SymmetricKey symkey = crypto.OpenSymmetricKey(decryptedKey, entry.Salt);

            using ISymmetricTransformer dataDecoder = crypto.OpenSymmetricTransformer(symkey);

            ReadOnlyMemory<byte> decryptedData = dataDecoder.Decrypt(entry.Data.Span);

            return EntryPayloadSerializer.Deserialize(decryptedData.Span);
        }

        public void Dispose()
        {
        }
    }
}
