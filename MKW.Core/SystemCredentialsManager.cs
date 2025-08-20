using MKW.Core.Cryptography;

namespace MKW.Core.Client
{
    public class SystemCredentialsManager
    {
        public SystemCredentialsManager()
        {
        }

        public SystemCredentials GenerateCredentials(UserCredentials userCredentials)
        {
            // Generate asymmetric pair of public and private keys
            using AsymmetricTransformer userKey = AsymmetricTransformer.Create();

            // Symmetric encoder for secret section.
            using SymmetricTransformer encoder = SymmetricTransformer.Open(userCredentials.GetEncodingHash().Span,
                                                                           userCredentials.ExportSalt().Span);

            Memory<byte> privateKeyBytes = userKey.ExportPrivateKey();
            Memory<byte> privateKeyEncrypted = encoder.Encrypt(privateKeyBytes.Span);

            Memory<byte> publicKeyBytes = userKey.ExportPublicKey();

            return new SystemCredentials
            {
                PublicKey = publicKeyBytes,
                PrivateKey = privateKeyEncrypted,
                Salt = userCredentials.ExportSalt().ToArray(),
            };
        }
    }
}
