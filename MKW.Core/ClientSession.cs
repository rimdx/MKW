using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IDatabase Database { get; }

        private readonly bool ownsDb;

        protected ClientSession(IDatabase db, bool ownsDb)
        {
            Database = db;
            this.ownsDb = ownsDb;
        }

        public static ClientSession Open(IDatabase db /* reference */)
        {
            return Open(db, false);
        }

        public static ClientSession Open(IDatabase db, bool ownsDb)
        {
            return new ClientSession(db, ownsDb);
        }

        public static ClientSession Create(IDatabase db /* reference */, string adminPassword)
        {
            return Create(db, adminPassword, false);
        }

        public static ClientSession Create(IDatabase db, string adminPassword, bool ownsDb)
        {
            ClientSession client = new ClientSession(db, ownsDb);
            client.PromoteAdmin(adminPassword);
            return client;
        }

        public void Dispose()
        {
            if (ownsDb)
            {
                Database.Dispose();
            }
        }
    }
}
