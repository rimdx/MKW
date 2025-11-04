// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class AesOpenPgpTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly SymmetricKeyAesOpenPgpCfb key;
        private readonly SymmetricCipher cipher;

        public AesOpenPgpTransformer(SymmetricKeyAesOpenPgpCfb key)
        {
            this.key = key;

            AesEngine blockCipher = new AesEngine();
            OpenPgpCfbBlockCipher blockCipherMode = new OpenPgpCfbBlockCipher(blockCipher);
            BufferedBlockCipher cipher = new BufferedBlockCipher(blockCipherMode);

            KeyParameter parameters = new KeyParameter(key.KeyBytes.ToArray());

            this.cipher = new SymmetricCipher(cipher, parameters);
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            return cipher.Decrypt(data);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            return cipher.Encrypt(data);
        }

        public void Dispose()
        {
            // no-op
        }
    }
}
