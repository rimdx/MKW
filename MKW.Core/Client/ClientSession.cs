using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;
using System.Security.Cryptography;

namespace MKW.Core.Client
{
    public class ClientSession
    {
        private readonly IDatabase db;

        public ClientSession(IDatabase db)
        {
            this.db = db;
        }

        public UserInfo AddUser(string password)
        {
            using var userKey = AsymmetricTransformer.Create();

            var creds = UserCredentials.Create(password);

            using SymmetricTransformer encoder = SymmetricTransformer.Open(creds.GetEncodingHash(),
                                                                           creds.ExportSalt());

            byte[] privateKeyBytes = userKey.ExportPrivateKey();
            byte[] privateKeyEncrypted = encoder.Encrypt(privateKeyBytes);

            byte[] publicKeyBytes = userKey.ExportPublicKey();

            DatabaseUser user = new DatabaseUser
            {
                Id = Guid.NewGuid(),
                PublicKey = publicKeyBytes,
                PrivateKey = privateKeyEncrypted,
                Salt = creds.ExportSalt(),
            };

            db.AddUser(user.Id, user);

            return UserInfo.FromDatabaseUser(user);
        }

        public UserSession OpenUser(Guid id, string password)
        {
            DatabaseUser user = db.GetUser(id);

            // Credentials can be opened within the entered password and the public salt
            UserCredentials creds = UserCredentials.Open(password, user.Salt);

            return OpenUser(user, creds);
        }

        public UserSession OpenUser(string password)
        {
            foreach (DatabaseUser user in db.EnumerateUsers())
            {
                try
                {
                    UserCredentials creds = UserCredentials.Open(password, user.Salt);

                    return OpenUser(user, creds);
                }
                catch (CryptographicException)
                {
                    // Ignore this user if the credentials are invalid
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        public UserSession OpenUser(DatabaseUser user, UserCredentials creds)
        {
            // Private data of the user is encrypted symmetrically using our creds (decoder
            // also needs some data stored in the public section of the object).
            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetEncodingHash(),
                                                                           creds.ExportSalt());

            // Let's try'N decode the private key. We could potentially fail here. So
            // some validation may be required.
            byte[] privateKeyBytes = decoder.Decrypt(user.PrivateKey);

            return new UserSession(db, user, privateKeyBytes);
        }

        public EntryInfo UpdateEntry(Guid id, EntryPayload? entry)
        {
            if (entry == null)
            {
                db.UpdateEntry(id, null);

                return new EntryInfo
                {
                    Id = id,
                    Action = ActionInfo.Deleted,
                    EncodedForUsers = []
                };
            }
            else
            {
                DatabaseUser[] users = db.EnumerateUsers().ToArray();

                DatabaseSecretEntry encodedEntry = EncodeEntry(entry, users);

                List<UserInfo> encodedForUsers = [];
                foreach (DatabaseUser user in users)
                {
                    encodedForUsers.Add(UserInfo.FromDatabaseUser(user));
                }

                DatabaseSecretEntry? oldEntry = db.QueryEntry(id);
                db.UpdateEntry(id, encodedEntry);

                return new EntryInfo
                {
                    Id = id,
                    Action = oldEntry == null ? ActionInfo.Added : ActionInfo.Updated,
                    EncodedForUsers = encodedForUsers
                };
            }
        }

        public DatabaseSecretEntry EncodeEntry(EntryPayload payload, IEnumerable<DatabaseUser> users)
        {
            using SymmetricTransformer payloadEncoder = SymmetricTransformer.Create();

            byte[] data = payloadEncoder.Encrypt(payload.Data);

            var keys = new Dictionary<Guid, byte[]>();

            foreach (DatabaseUser user in db.EnumerateUsers())
            {
                using AsymmetricTransformer keyEncoder = AsymmetricTransformer.Open(user.PublicKey);

                byte[] encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey());

                keys.Add(user.Id, encyptedKey);
            }

            DatabaseSecretEntry entry = new DatabaseSecretEntry
            {
                Keys = keys,
                Data = data,
                Salt = payloadEncoder.ExportIV(),
            };

            return entry;
        }
    }
}
