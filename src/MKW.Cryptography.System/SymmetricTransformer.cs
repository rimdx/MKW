// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography.Exceptions;
using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    public class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly Aes aes;

        protected SymmetricTransformer(Aes aes)
        {
            this.aes = aes;
        }

        public static SymmetricKey CreateKey()
        {
            Aes aes = Aes.Create();

            aes.GenerateKey();
            aes.GenerateIV();

            return new SymmetricKeyAesGcm
            {
                KeyBytes = aes.Key,
                IVBytes = aes.IV,
            };
        }

        public static ISymmetricTransformer Open(SymmetricKeyAesGcm key)
        {
            Aes aes = Aes.Create();

            try
            {
                aes.Key = key.KeyBytes.ToArray(); /* copy */
                aes.IV = key.IVBytes.ToArray(); /* copy */
            }
            catch (CryptographicException ex)
            {
                throw new InvalidKeyException(ex);
            }

            return new SymmetricTransformer(aes /* move */);
        }

        public ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                using MemoryStream output = new MemoryStream();
                using ICryptoTransform encryptor = aes.CreateEncryptor();
                using CryptoStream encryptorStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);

                encryptorStream.Write(data);
                encryptorStream.FlushFinalBlock();

                return output.ToArray();
            }
            catch (CryptographicException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }

        public ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                using MemoryStream output = new MemoryStream();
                using ICryptoTransform decryptor = aes.CreateDecryptor();
                using CryptoStream decryptorStream = new CryptoStream(output, decryptor, CryptoStreamMode.Write);

                decryptorStream.Write(data);
                decryptorStream.FlushFinalBlock();

                return output.ToArray();
            }
            catch (CryptographicException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }

        public void Dispose()
        {
            aes.Dispose();
        }
    }
}
