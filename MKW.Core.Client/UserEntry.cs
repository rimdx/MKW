using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserEntry : Entry, IEntrySession
    {
        protected readonly UserSession user;

        public UserEntry(ClientSession client,
                         ICryptographyProvider crypto,
                         UserSession user,
                         IDatabaseEntry entry)
            : base(client, crypto, user.TrustController, entry)
        {
            this.user = user;
        }

        public override EntryPayload? OpenPayload()
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
    }
}
