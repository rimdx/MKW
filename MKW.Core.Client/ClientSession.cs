using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IDatabase Database { get; }

        private readonly bool ownsDb;
        private readonly UserController userController;

        protected ClientSession(IDatabase db, bool ownsDb)
        {
            Database = db;
            this.ownsDb = ownsDb;
            userController = new UserController(this, Database);
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
