using MKW.Core.Storage;

namespace MKW.Core
{
    public class DatabaseSession
    {
        private readonly Database db;

        public DatabaseSession(Database db)
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

            db.Users.Add(user);
        }

        public UserSession OpenUser(Guid id, string password)
        {
            // Let's first find the user in the database
            // TODO: reject if one wasn't found
            // TODO: move to another service
            User user = db.Users.First(u => u.Id == id);

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
    }
}
