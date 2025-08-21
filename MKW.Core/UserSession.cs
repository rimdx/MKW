using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession : IDisposable
    {
        private readonly IDatabase db;
        private readonly IDatabaseUser user;
        private readonly AsymmetricTransformer transformer;

        public UserSession(IDatabase db /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.db = db;
            this.user = user;
            transformer = AsymmetricTransformer.Open(user.PublicKey.Span, privateKey);
        }

        public KeyedEntry GetEntry(Guid id)
        {
            IDatabaseEntry entry = db.OpenEntry(id, DatabaseOpenMode.ReadOnly);

            EntryPayload? payload = DecodeEntry(entry);

            return new KeyedEntry
            {
                Id = id,
                Payload = payload
            };
        }

        public IEnumerable<KeyedEntry> EnumerateEntries()
        {
            foreach (IDatabaseEntry entry in db.EnumerateEntries())
            {
                yield return new KeyedEntry
                {
                    Id = entry.Id,
                    Payload = DecodeEntry(entry)
                };
            }
        }

        public EntryPayload? DecodeEntry(IDatabaseEntry entry)
        {
            if (entry.Keys.TryGetValue(user.Id, out ReadOnlyMemory<byte> encodedKey) == false)
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
            user.Dispose();
        }
    }
}
