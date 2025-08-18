using MKW.Core.Storage;
using System.Security.Cryptography;

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
            var userKey = RSA.Create();

            var creds = UserCredentials.Create(password);

            //

            using SymmetricTransformer encoder = SymmetricTransformer.Create(creds.GetEncodingHash());

            byte[] privateKeyBytes = userKey.ExportRSAPrivateKey();
            byte[] privateKeyEncrypted = encoder.Encrypt(privateKeyBytes);

            //

            byte[] publicKeyBytes = userKey.ExportRSAPublicKey();

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
