using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        private readonly IDatabase db;
        private readonly bool ownsDb;

        protected ClientSession(IDatabase db, bool ownsDb)
        {
            this.db = db;
            this.ownsDb = ownsDb;
        }

        public static ClientSession Open(IDatabase db /* reference */)
        {
            return new ClientSession(db, false);
        }

        public static ClientSession Open(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path);
            return new ClientSession(db, true);
        }

        public void Dispose()
        {
            if (ownsDb)
            {
                db.Dispose();
            }
        }
    }
}
