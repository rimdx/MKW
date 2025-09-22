using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public record class SystemCredentials : IDisposable
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        // Encrypted using user's password
        public required SecretPayload PrivateKey { get; set; }

        public required IAsymmetricPrivateTransformer Transformer { get; set; }

        public void Dispose()
        {
            Transformer.Dispose();
        }
    }
}
