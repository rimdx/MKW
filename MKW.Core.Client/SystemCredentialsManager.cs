using MKW.Cryptography;

namespace MKW.Core.Client
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
            using IAsymmetricPrivateTransformer userKey = crypto.CreateAsymmetricTransformer();

            // Symmetric encoder for secret section.
            using ISymmetricTransformer encoder = crypto.OpenSymmetricTransformer(
                userCredentials.GetSecretKey().Span, userCredentials.ExportSalt().Span);

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
