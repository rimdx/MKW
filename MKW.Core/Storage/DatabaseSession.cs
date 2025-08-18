using System.Text.Json;

namespace MKW.Core.Storage
{
    public class MemoryDatabaseSession : IDatabaseSession, IDisposable
    {
        public readonly Database Database;

        public MemoryDatabaseSession()
        {
            Database = new Database();
        }

        public void AddUser(Guid id, User user)
        {
            Database.Users.Add(user);
        }

        public User GetUser(Guid id)
        {
            return Database.Users.First(u => u.Id == id);
        }

        public void Save()
        {
        }

        public void Dispose()
        {
        }
    }

    public class DatabaseSession : IDatabaseSession, IDisposable
    {
        private readonly Database db;
        private readonly FileStream file;

        protected DatabaseSession(Database db, FileStream file)
        {
            this.db = db;
            this.file = file;
        }

        public static DatabaseSession Open(string path)
        {
            if (File.Exists(path))
            {
                FileStream file = new FileStream(path, FileMode.OpenOrCreate);

                Database database = JsonSerializer.Deserialize<Database>(file)!;

                return new DatabaseSession(database,
                                           file /* move */);
            }
            else
            {
                FileStream file = new FileStream(path, FileMode.OpenOrCreate);

                Database database = new Database();

                DatabaseSession session = new DatabaseSession(database,
                                                              file /* move */);

                // Writes empty database to file to the disk
                session.Save();

                return session;
            }
        }

        public void AddUser(Guid id, User user)
        {
            db.Users.Add(user);
            Save();
        }

        public User GetUser(Guid id)
        {
            return db.Users.First(u => u.Id == id);
        }

        public void Save()
        {
            file.Seek(0, SeekOrigin.Begin);
            JsonSerializer.Serialize(file, db);
        }

        public void Dispose()
        {
            file.Dispose();
        }
    }
}
