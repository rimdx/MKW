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

        public EntryPayload GetEntry(Guid id)
        {
            DatabaseSecretEntry entry = db.QueryEntry(id);

            if (entry == null)
            {
                throw new Exception($"No entry found for ID: {id}");
            }

            byte[] encodedKey = entry.Keys[user.Id];

            using AsymmetricTransformer keyDecoder = AsymmetricTransformer.Open(user.PublicKey, decryptedPrivateKey);

            byte[] decryptedKey = keyDecoder.Decrypt(encodedKey);

            using SymmetricTransformer dataDecoder = SymmetricTransformer.Open(decryptedKey, entry.Salt);

            byte[] decryptedData = dataDecoder.Decrypt(entry.Data);

            return new EntryPayload(decryptedData);
        }
    }
}
