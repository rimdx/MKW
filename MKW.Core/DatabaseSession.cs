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

            using Aes aes = Aes.Create();

            aes.GenerateIV();
            aes.Key = creds.GetEncodingHash();

            using MemoryStream msEncrypt = new MemoryStream();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, aes.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] privateKeyBytes = userKey.ExportRSAPrivateKey();

            csEncrypt.Write(privateKeyBytes);
            csEncrypt.Flush();

            byte[] privateKeyEncrypted = msEncrypt.ToArray();

            //

            byte[] publicKeyBytes = userKey.ExportRSAPublicKey();

            User user = new User
            {
                Id = Guid.NewGuid(),
                PublicKey = publicKeyBytes,
                EncryptedPrivateKey = privateKeyEncrypted,
                Salt = creds.ExportSalt(),
                IV = aes.IV,
            };

            db.Users.Add(user);
        }
    }
}
