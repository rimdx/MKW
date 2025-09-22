using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;
using MKW.Cryptography.Loader;

namespace MKW.Core.Client
{
    public class ClientSession : IUserController, IAdminController, IDisposable
    {
        public IDatabase Database { get; }

        private readonly ICryptographyProvider crypto;
        private readonly UserController userController;
        private readonly AdminController adminController;

        protected ClientSession(IDatabase db)
        {
            Database = db;

            crypto = CryptographyLoader.GetProvider();
            userController = new UserController(this, crypto, Database);
            adminController = new AdminController(crypto, Database);
        }

        public static ClientSession Open(IDatabase db)
        {
            ClientSession client = new ClientSession(db);

            // ensure the admin actually exists
            // a database without admin is invalid
            client.Database.OpenUser(UserId.Admin());

            return client;
        }

        public static ClientSession Create(IDatabase db,
                                           string adminPassword,
                                           UserMetadata adminMetadata)
        {
            ClientSession client = new ClientSession(db);

            client.CreateAdmin(adminPassword, adminMetadata);

            return client;
        }

        // todo: ITrustProvider

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseUser admin = Database.OpenUser(UserId.Admin());
            using IAsymmetricPublicTransformer key = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span);

            using UserTrustProvider trustProvider = new UserTrustProvider(Database, crypto, key, key);

            IEnumerable<UserInfo> trust = trustProvider.EnumerateTrustedUsers();

            // Convert IEnumerable to an array, before returning from function,
            // because outside the trustProvider will be disposed.
            //
            // Dear .NET, why??
            return trust.ToArray();
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust(UserId userId)
        {
            IDatabaseUser user = Database.OpenUser(userId);
            IDatabaseUser admin = Database.OpenUser(UserId.Admin());

            using IAsymmetricPublicTransformer userKey = crypto.OpenAsymmetricTransformer(
                user.PublicKey.Payload.Span);
            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span);

            using UserTrustProvider trustProvider = new UserTrustProvider(Database, crypto, userKey, adminKey);

            foreach (UserInfo trust in trustProvider.EnumerateTrustedUsers())
            {
                yield return trust;
            }
        }

        // IUserController

        public IUserSession OpenUser(UserId id, string password)
        {
            return userController.OpenUser(id, password);
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            foreach (UserInfo user in userController.EnumerateUsers())
            {
                yield return user;
            }
        }

        public IUserSession OpenUser(string password)
        {
            return userController.OpenUser(password);
        }

        public UserInfo GetUserInfo(UserId id)
        {
            return userController.GetUserInfo(id);
        }

        public UserAccessRequest CreateUserAccessRequest(string password)
        {
            return userController.CreateUserAccessRequest(password);
        }

        // IAdminController

        public UserInfo CreateAdmin(string password, UserMetadata metadata)
        {
            return adminController.CreateAdmin(password, metadata);
        }

        public IAdminSession OpenAdmin(string password)
        {
            return adminController.OpenAdmin(password);
        }

        public UserInfo GetAdminInfo()
        {
            return adminController.GetAdminInfo();
        }

        public void Dispose()
        {
            userController.Dispose();
        }
    }
}
