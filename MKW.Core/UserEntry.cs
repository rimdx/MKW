using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserEntry : Entry
    {
        protected readonly UserSession user;

        public UserEntry(ClientSession client, UserSession user, IDatabaseEntry entry)
            : base(client, user.TrustController, entry)
        {
            this.user = user;
        }

        public EntryPayload? OpenPayload()
        {
            if (entry.Keys.TryGetValue(user.Id, out ReadOnlyMemory<byte> encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            Memory<byte> decryptedKey = user.Transformer.Decrypt(encodedKey.Span);

            using SymmetricTransformer dataDecoder = SymmetricTransformer.Open(decryptedKey.Span,
                                                                               entry.Salt.Span);

            Memory<byte> decryptedData = dataDecoder.Decrypt(entry.Data.Span);

            return new EntryPayload(decryptedData);
        }
    }
}
