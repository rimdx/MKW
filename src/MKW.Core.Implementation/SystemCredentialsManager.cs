using MKW.Core.Exceptions;
using MKW.Cryptography;
using MKW.Cryptography.Exceptions;
using MKW.Storage;

namespace MKW.Core.Implementation
{
    public class SystemCredentialsManager
    {
        private readonly ICryptographyProvider crypto;

        public SystemCredentialsManager(ICryptographyProvider crypto)
        {
            this.crypto = crypto;
        }

        public SystemCredentials GenerateCredentials(IUserCredentials userCredentials)
        {
            // Generate asymmetric pair of public and private keys
            AsymmetricPrivateKey userKey = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);

            IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                userKey, CommonCryptographyAlgorithms.Rsa2048);

            SymmetricKey symkey = new SymmetricKey
            {
                KeyBytes = userCredentials.GetSecretKey(),
                IVBytes = userCredentials.ExportSalt(),
            };

            // Symmetric encoder for secret section.
            using ISymmetricTransformer encoder = crypto.OpenSymmetricTransformer(symkey);

            Memory<byte> privateKeyBytes = crypto.EncodePkcsPrivateKey(userKey);
            Memory<byte> privateKeyEncrypted = encoder.Encrypt(privateKeyBytes.Span);

            Memory<byte> publicKeyBytes = crypto.EncodePkcsPublicKey(userKey.GetPublicKey());

            return new SystemCredentials
            {
                PublicKey = publicKeyBytes,
                PrivateKey = new SecretPayload(privateKeyEncrypted),
                Salt = userCredentials.ExportSalt().ToArray(),
                Transformer = transformer /* move */,
            };
        }

        public SystemCredentials OpenCredentials(DatabaseUser user, IUserCredentials userCredentials)
        {
            // Symmetric decoder for secret section.
            // Uses user's secret key and public salt from the database.

            SymmetricKey symkey = new SymmetricKey
            {
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

            ReadOnlyMemory<byte> publicKeyBytes = user.PublicKey.Payload;

            IAsymmetricPrivateTransformer userKey = crypto.OpenAsymmetricTransformer(
                crypto.DecodePkcsPrivateKey(privateKeyBytes.Span),
                CommonCryptographyAlgorithms.Rsa2048);

            return new SystemCredentials
            {
                PublicKey = publicKeyBytes,
                PrivateKey = new SecretPayload(privateKeyEncrypted),
                Salt = user.Salt,
                Transformer = userKey,
            };
        }
    }
}
