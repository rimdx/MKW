using MKW.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class EntryDecoder : IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly UserSession user;

        public EntryDecoder(ICryptographyProvider crypto,
                            UserSession user)
        {
            this.crypto = crypto;
            this.user = user;
        }

        public EntryPayload? DecodeEntry(IDatabaseEntry entry)
        {
            if (entry.Keys.TryGetValue(user.Id, out ReadOnlyMemory<byte> encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            Memory<byte> decryptedKey = user.Transformer.Decrypt(encodedKey.Span);

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
