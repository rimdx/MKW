using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession
    {
        private readonly IDatabaseSession db;
        private readonly DatabaseUser user;
        private readonly byte[] decryptedPrivateKey;

        public UserSession(IDatabaseSession db, DatabaseUser user, byte[] decryptedPrivateKey)
        {
            this.db = db;
            this.user = user;
            this.decryptedPrivateKey = decryptedPrivateKey;
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
            if (entry.Keys.TryGetValue(user.Id, out byte[]? encodedKey) == false)
            {
                // No key for this user, cannot decode the entry
                return null;
            }

            using AsymmetricTransformer keyDecoder = AsymmetricTransformer.Open(user.PublicKey, decryptedPrivateKey);

            byte[] decryptedKey = keyDecoder.Decrypt(encodedKey);

            using SymmetricTransformer dataDecoder = SymmetricTransformer.Open(decryptedKey, entry.Salt);

            byte[] decryptedData = dataDecoder.Decrypt(entry.Data);

            return new EntryPayload(decryptedData);
        }
    }
}
