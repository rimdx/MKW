using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class EntryDecoder : IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IUserSession user;
        private readonly IAsymmetricPrivateTransformer transformer;

        public EntryDecoder(ICryptographyProvider crypto,
                            IUserSession user,
                            IAsymmetricPrivateTransformer transformer)
        {
            this.crypto = crypto;
            this.user = user;
            this.transformer = transformer;
        }

        public EntryPayload? DecodeEntry(IDatabaseEntry entry)
        {
            if (entry.Keys.TryGetValue(user.Id, out ReadOnlyMemory<byte> encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            Memory<byte> decryptedKey = transformer.Decrypt(encodedKey.Span);

            using ISymmetricTransformer dataDecoder = crypto.OpenSymmetricTransformer(
                decryptedKey.Span, entry.Salt.Span);

            Memory<byte> decryptedData = dataDecoder.Decrypt(entry.Data.Span);

            return new EntryPayload(decryptedData);
        }

        public void Dispose()
        {
        }
    }
}
