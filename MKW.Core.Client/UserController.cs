using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;
using MKW.Cryptography.Exceptions;

namespace MKW.Core.Client
{
    public class UserController : IUserController, IDisposable
    {
        private readonly ClientSession client;
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;

        public UserController(ClientSession client,
                              ICryptographyProvider crypto,
                              IDatabase database)
        {
            this.client = client;
            this.crypto = crypto;
            this.database = database;
        }

        public UserInfo PromoteUser(string password)
        {
            // TODO: sign admin
            IDatabaseUser admin = database.OpenAdmin(true);

            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            IUserCredentials userCreds = crypto.CreateUserCredentials(password);
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
                IUserCredentials creds = crypto.OpenUserCredentials(password, user.Salt);

                return OpenUser(user, creds);
            }
        }

        public UserSession OpenUser(string password)
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                try
                {
                    IUserCredentials creds = crypto.OpenUserCredentials(password, user.Salt);

                    return OpenUser(client.OpenDatabaseUser(user.Id, false), creds);
                }
                catch (SymmetricOperationFailedException)
                {
                    // Ignore this user if the credentials are invalid
                }
                catch (InvalidKeyException)
                {
                    // Possible occurrence, as experiments have shown. Fails in 1/~35 times.
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        public UserSession OpenUser(IDatabaseUser user, IUserCredentials creds)
        {
            // Private data of the user is encrypted symmetrically using our creds (decoder
            // also needs some data stored in the public section of the object).
            using ISymmetricTransformer decoder = crypto.OpenSymmetricTransformer(
                creds.GetSecretKey().Span, creds.ExportSalt().Span);

            // Let's try'N decode the private key. We could potentially fail here. So
            // some validation may be required.
            Memory<byte> privateKeyBytes = decoder.Decrypt(user.PrivateKey.Span);

            return new UserSession(client, crypto, database, user, privateKeyBytes.Span);
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
