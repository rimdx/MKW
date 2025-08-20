using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession : IDisposable
    {
        private readonly IDatabase db;
        private readonly DatabaseUser user;
        private readonly AsymmetricTransformer transformer;

        public UserSession(IDatabase db /* reference */,
                           DatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.db = db;
            this.user = user;
            transformer = AsymmetricTransformer.Open(user.PublicKey.Span, privateKey);
        }

        public KeyedEntry GetEntry(Guid id)
        {
            DatabaseSecretEntry entry = db.QueryEntry(id);

            if (entry == null)
            {
                throw new Exception($"No entry found for ID: {id}");
            }

            EntryPayload? payload = DecodeEntry(entry);

            if (payload == null)
            {
                throw new Exception($"Cannot decode entry for ID: {id} - no key for user {user.Id}");
            }

            return new KeyedEntry
            {
                Id = id,
                Payload = payload
            };
        }

        public IEnumerable<KeyedEntry> EnumerateEntries()
        {
            foreach (DatabaseSecretKeyedEntry entry in db.EnumerateEntries())
            {
                yield return new KeyedEntry
                {
                    Id = entry.Id,
                    Payload = DecodeEntry(entry)
                };
            }
        }

        public EntryPayload? DecodeEntry(DatabaseSecretEntry entry)
        {
            if (entry.Keys.TryGetValue(user.Id, out Memory<byte> encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            Memory<byte> decryptedKey = transformer.Decrypt(encodedKey.Span);

            using SymmetricTransformer dataDecoder = SymmetricTransformer.Open(decryptedKey.Span,
                                                                               entry.Salt.Span);

            Memory<byte> decryptedData = dataDecoder.Decrypt(entry.Data.Span);

            return new EntryPayload(decryptedData);
        }

        public void Dispose()
        {
            transformer.Dispose();
        }
    }
}
