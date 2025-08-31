using MKW.Core.Cryptography;

namespace MKW.Core.Client
{
    public class SystemCredentialsManager
    {
        private readonly ClientSession client;

        public SystemCredentialsManager(ClientSession client)
        {
            this.client = client;
        }

        public SystemCredentials GenerateCredentials(IUserCredentials userCredentials)
        {
            // Generate asymmetric pair of public and private keys
            using IAsymmetricPrivateTransformer userKey = client.CryptographyProvider.CreateAsymmetricTransformer();

            // Symmetric encoder for secret section.
            using ISymmetricTransformer encoder = client.CryptographyProvider.OpenSymmetricTransformer(
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
