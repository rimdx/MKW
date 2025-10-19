// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Implementation
{
    public class EntryDecoder : IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IUserSession user;
        private readonly IAsymmetricPrivateTransformer transformer;

        public EntryDecoder(ClientCryptography crypto,
                            IUserSession user,
                            IAsymmetricPrivateTransformer transformer)
        {
            this.crypto = crypto;
            this.user = user;
            this.transformer = transformer;
        }

        public EntryPayload? DecodeEntry(DatabaseEntry entry)
        {
            if (entry.Keys.TryGetValue(user.Id, out ReadOnlyMemory<byte> encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            Memory<byte> decryptedKey = transformer.Decrypt(encodedKey.Span);

            SymmetricKey symkey = new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesGcm,
                KeyBytes = decryptedKey,
                IVBytes = entry.Salt,
            };

            using ISymmetricTransformer dataDecoder = crypto.OpenSymmetricTransformer(symkey);

            Memory<byte> decryptedData = dataDecoder.Decrypt(entry.Data.Span);

            return EntryPayloadSerializer.Deserialize(decryptedData.Span);
        }

        public void Dispose()
        {
        }
    }
}
