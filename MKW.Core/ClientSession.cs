using MKW.Core.Storage;

namespace MKW.Core
{
    public class ClientSession
    {
        private readonly IDatabaseSession db;

        public ClientSession(IDatabaseSession db)
        {
            this.db = db;
        }

        public void AddUser(string password)
        {
            using var userKey = AsymmetricTransformer.Create();

            var creds = UserCredentials.Create(password);

            //

            using SymmetricTransformer encoder = SymmetricTransformer.Create(creds.GetEncodingHash());

            byte[] privateKeyBytes = userKey.ExportPrivateKey();
            byte[] privateKeyEncrypted = encoder.Encrypt(privateKeyBytes);

            //

            byte[] publicKeyBytes = userKey.ExportPublicKey();

            User user = new User
            {
                Id = Guid.NewGuid(),
                PublicKey = publicKeyBytes,
                EncryptedPrivateKey = privateKeyEncrypted,
                Salt = creds.ExportSalt(),
                IV = encoder.ExportIV(),
            };

            db.AddUser(user.Id, user);
        }

        public UserSession OpenUser(Guid id, string password)
        {
            User user = db.GetUser(id);

            // Credentials can be opened within the entered password and the public salt
            UserCredentials creds = UserCredentials.Open(password, user.Salt);

            // Private data of the user is encrypted symmetrically using our creds (decoder
            // also needs some data stored in the public section of the object).
            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetEncodingHash(), user.IV);

            // Let's try'N decode the private key. We could potentially fail here. So
            // some validation may be required.
            byte[] privateKeyBytes = decoder.Decrypt(user.EncryptedPrivateKey);

            return new UserSession(db, user, privateKeyBytes);
        }

        public void UpdateEntry(Guid id, EntryPayload? entry)
        {
            if (entry == null)
            {
                db.UpdateEntry(id, null);
            }
            else
            {
                using SymmetricTransformer payloadEncoder = SymmetricTransformer.Create();

                byte[] data = payloadEncoder.Encrypt(entry.Data);

                var keys = new Dictionary<Guid, byte[]>();

                foreach (User user in db.EnumerateUsers())
                {
                    using AsymmetricTransformer keyEncoder = AsymmetricTransformer.Open(user.PublicKey);

                    byte[] encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey());

                    keys.Add(user.Id, encyptedKey);
                }

                Entry newEntry = new Entry
                {
                    Keys = keys,
                    Data = data,
                    Salt = payloadEncoder.ExportIV(),
                };

                db.UpdateEntry(id, newEntry);
            }
        }
    }
}
