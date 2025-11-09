// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Cryptography.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class SymmetricCipher
    {
        private readonly IBufferedCipher cipher;
        private readonly ICipherParameters parameters;

        public SymmetricCipher(IBufferedCipher cipher, ICipherParameters parameters)
        {
            this.cipher = cipher;
            this.parameters = parameters;
        }

        public ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                cipher.Init(false, parameters);

                using MemoryStream output = new MemoryStream();

                using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                    null, cipher))
                {
                    cipherStream.Write(data);
                }

                return output.ToArray();
            }
            catch (CryptoException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }

        public ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                cipher.Init(true, parameters);

                using MemoryStream output = new MemoryStream();

                using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                    null, cipher))
                {
                    cipherStream.Write(data);
                }

                return output.ToArray();
            }
            catch (CryptoException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }
    }
}
