// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Exceptions;
using MKW.Cryptography;
using MKW.Cryptography.Exceptions;
using MKW.Storage;

namespace MKW.Core.Client
{
    public class SystemCredentialsManager
    {
        private readonly ClientCryptography crypto;

        public SystemCredentialsManager(ClientCryptography crypto)
        {
            this.crypto = crypto;
        }

        public SystemCredentials GenerateCredentials(IUserCredentials userCredentials)
        {
            // Generate asymmetric pair of public and private keys
            AsymmetricPrivateKey privateKey = crypto.CreateAsymmetricKey();
            AsymmetricPublicKey publicKey = privateKey.GetPublicKey();

            SymmetricKey symkey = new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesGcm,
                KeyBytes = userCredentials.GetSecretKey(),
                IVBytes = userCredentials.ExportSalt(),
            };

            // Symmetric encoder for secret section.
            using ISymmetricTransformer encoder = crypto.OpenSymmetricTransformer(symkey);

            Memory<byte> privateKeyBytes = crypto.EncodePkcsPrivateKey(privateKey);
            Memory<byte> privateKeyEncrypted = encoder.Encrypt(privateKeyBytes.Span);

            Memory<byte> publicKeyBytes = crypto.EncodePkcsPublicKey(publicKey);

            return new SystemCredentials
            {
                Salt = userCredentials.ExportSalt().ToArray(),
                PublicKey = publicKeyBytes,
                EncryptedPrivateKey = new SecretPayload(privateKeyEncrypted),
                PrivateKey = privateKey,
            };
        }

        public SystemCredentials OpenCredentials(DatabaseUser user, IUserCredentials userCredentials)
        {
            // Symmetric decoder for secret section.
            // Uses user's secret key and public salt from the database.

            SymmetricKey symkey = new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesGcm,
                KeyBytes = userCredentials.GetSecretKey(),
                IVBytes = user.Salt,
            };

            using ISymmetricTransformer decoder = crypto.OpenSymmetricTransformer(symkey);

            // The user's key-pair can be obtained by decrypting the private key
            // using the symmetric  decoder and public key publicly stored in the
            // database.
            ReadOnlyMemory<byte> privateKeyEncrypted = user.PrivateKey.EncryptedPayload;

            ReadOnlyMemory<byte> privateKeyBytes;

            // AesGcm verifies that decryption was successful and the password,
            // meaning if wrong password was provided (e.g. symmetric secret key
            // is not valid), decryption will result an error.
            try
            {
                privateKeyBytes = decoder.Decrypt(privateKeyEncrypted.Span);
            }
            catch (SymmetricOperationFailedException ex)
            {
                throw new InvalidPasswordException(ex);
            }

            AsymmetricPrivateKey privateKey = crypto.DecodePkcsPrivateKey(privateKeyBytes.Span);

            IAsymmetricPrivateTransformer userKey = crypto.OpenAsymmetricTransformer(
                crypto.DecodePkcsPrivateKey(privateKeyBytes.Span));

            return new SystemCredentials
            {
                Salt = userCredentials.ExportSalt().ToArray(),
                PublicKey = user.ProtectedData.PublicKey,
                EncryptedPrivateKey = new SecretPayload(privateKeyEncrypted),
                PrivateKey = privateKey,
            };
        }
    }
}
