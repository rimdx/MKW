using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;
using MKW.Cryptography.Loader;

namespace MKW.Core.Client
{
    public class ClientSession : IUserController, IAdminController, IEntryController, IDisposable
    {
        public IDatabase Database { get; }

        private readonly bool ownsDb;

        private readonly ICryptographyProvider crypto;
        private readonly UserController userController;
        private readonly AdminController adminController;

        protected ClientSession(IDatabase db, bool ownsDb)
        {
            Database = db;
            this.ownsDb = ownsDb;

            crypto = CryptographyLoader.GetProvider();
            userController = new UserController(this, crypto, Database);
            adminController = new AdminController(this, crypto, Database);
        }

        public static ClientSession Open(IDatabase db /* reference */)
        {
            return Open(db, false);
        }

        public static ClientSession Open(IDatabase db, bool ownsDb)
        {
            ClientSession client = new ClientSession(db, ownsDb);

            // ensure the admin actually exists
            // a database without admin is invalid
            client.Database.OpenAdmin(true);

            return client;
        }

        public static ClientSession Create(IDatabase db /* reference */, string adminPassword)
        {
            return Create(db, false, adminPassword);
        }

        public static ClientSession Create(IDatabase db, bool ownsDb, string adminPassword)
        {
            ClientSession client = new ClientSession(db, ownsDb);
            client.PromoteAdmin(adminPassword);
            return client;
        }

        internal IEnumerable<IDatabaseUser> EnumerateDatabaseUsers()
        {
            yield return Database.OpenAdmin(true);

            foreach (IDatabaseUser user in Database.EnumerateUsers())
            {
                yield return user;
            }
        }

        internal IDatabaseUser OpenDatabaseUser(UserId id, bool readOnly)
        {
            if (id.IsAdmin)
            {
                return Database.OpenAdmin(readOnly);
            }
            else
            {
                return Database.OpenUser(id, readOnly);
            }
        }

        // todo: ITrustProvider

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseUser admin = Database.OpenAdmin(true);
            using UserTrustProvider trustProvider = new UserTrustProvider(this, crypto, admin);

            IEnumerable<UserInfo> trust = trustProvider.EnumerateImplicitlyTrustedUsers();

            // Convert IEnumerable to an array, before returning from function,
            // because outside the trustProvider will be disposed.
            //
            // Dear .NET, why??
            return trust.ToArray();
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust(UserId userId)
        {
            IDatabaseUser user = OpenDatabaseUser(userId, true);
            using UserTrustProvider trustProvider = new UserTrustProvider(this, crypto, user);

            foreach (UserInfo trust in trustProvider.EnumerateImplicitlyTrustedUsers())
            {
                yield return trust;
            }
        }

        // IUserController

        public UserInfo PromoteUser(string password)
        {
            return userController.PromoteUser(password);
        }

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

        public IUserSession OpenUser(IDatabaseUser user, IUserCredentials creds)
        {
            return userController.OpenUser(user, creds);
        }

        public UserInfo GetUserInfo(UserId id)
        {
            return userController.GetUserInfo(id);
        }

        // IAdminController

        public UserInfo PromoteAdmin(string password)
        {
            return adminController.PromoteAdmin(password);
        }

        public IUserSession OpenAdmin(string password)
        {
            return adminController.OpenAdmin(password);
        }

        public UserInfo GetAdminInfo()
        {
            return adminController.GetAdminInfo();
        }

        // IEntryController

        private EntryController OpenEntryController()
        {
            return new EntryController(this, crypto, Database.OpenAdmin(true));
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.OpenEntry(id);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.CreateEntry(id);
        }

        public IEntrySession CreateEntry()
        {
            using EntryController entryController = OpenEntryController();
            return entryController.CreateEntry();
        }

        public EntryInfo DeleteEntry(EntryId id)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.DeleteEntry(id);
        }

        public EntryInfo UpdateEntry(EntryId id, EntryPayload? payload)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.UpdateEntry(id, payload);
        }

        public IEnumerable<IEntrySession> EnumerateEntries()
        {
            using EntryController entryController = OpenEntryController();

            foreach (IEntrySession entry in entryController.EnumerateEntries())
            {
                yield return entry;
            }
        }

        public void Dispose()
        {
            userController.Dispose();

            if (ownsDb)
            {
                Database.Dispose();
            }
        }
    }
}
