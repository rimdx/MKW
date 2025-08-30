using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;
using System.Security.Cryptography;

namespace MKW.Core.Client
{
    public class UserController : IUserController, IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabase database;

        public UserController(ClientSession client, IDatabase database)
        {
            this.client = client;
            this.database = database;
        }

        public UserInfo PromoteUser(string password)
        {
            // TODO: sign admin
            IDatabaseUser admin = database.OpenAdmin(true);

            SystemCredentialsManager credManager = new SystemCredentialsManager();

            UserCredentials userCreds = UserCredentials.Create(password);
            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            IDatabaseUser user = database.CreateUser(UserId.Create());

            user.PublicKey = systemCreds.PublicKey;
            user.PrivateKey = systemCreds.PrivateKey;
            user.Salt = systemCreds.Salt;

            user.Save();

            return UserInfo.FromDatabaseUser(user);
        }

        public UserSession OpenUser(UserId id, string password)
        {
            if (id.IsAdmin)
            {
                // todo:
                return client.OpenAdmin(password);
            }
            else
            {
                IDatabaseUser user = database.OpenUser(id, false);

                // Credentials can be opened within the entered password and the public salt
                UserCredentials creds = UserCredentials.Open(password, user.Salt);

                return OpenUser(user, creds);
            }
        }

        public UserSession OpenUser(string password)
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                try
                {
                    UserCredentials creds = UserCredentials.Open(password, user.Salt);

                    return OpenUser(client.OpenDatabaseUser(user.Id, false), creds);
                }
                catch (CryptographicException)
                {
                    // Ignore this user if the credentials are invalid
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        public UserSession OpenUser(IDatabaseUser user, UserCredentials creds)
        {
            // Private data of the user is encrypted symmetrically using our creds (decoder
            // also needs some data stored in the public section of the object).
            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetSecretKey().Span,
                                                                           creds.ExportSalt().Span);

            // Let's try'N decode the private key. We could potentially fail here. So
            // some validation may be required.
            Memory<byte> privateKeyBytes = decoder.Decrypt(user.PrivateKey.Span);

            return new UserSession(client, user, privateKeyBytes.Span);
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                yield return UserInfo.FromDatabaseUser(user);
            }
        }

        public UserInfo GetUserInfo(UserId id)
        {
            IDatabaseUser user = client.OpenDatabaseUser(id, true);
            return UserInfo.FromDatabaseUser(user);
        }

        public void Dispose()
        {
            /* no-op */
        }
    }
}
