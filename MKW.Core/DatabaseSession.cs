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
    }
}
