using MKW.Core.Storage;

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
        }
    }
}
