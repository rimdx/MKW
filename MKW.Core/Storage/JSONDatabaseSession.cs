using System.Text.Json;

namespace MKW.Core.Storage
{
    public class JSONDatabaseSession : MemoryDatabaseSession, IDatabase, IDisposable
    {
        private readonly FileStream file;

        protected JSONDatabaseSession(Database db, FileStream file)
            : base(db)
        {
            this.file = file;
        }

        public static JSONDatabaseSession Open(string path)
        {
            if (File.Exists(path))
            {
                FileStream file = new FileStream(path, FileMode.OpenOrCreate);

                Database database = JsonSerializer.Deserialize<Database>(file)!;

                return new JSONDatabaseSession(database,
                                           file /* move */);
            }
            else
            {
                FileStream file = new FileStream(path, FileMode.OpenOrCreate);

                Database database = new Database();

                JSONDatabaseSession session = new JSONDatabaseSession(database,
                                                              file /* move */);

                // Writes empty database to file to the disk
                session.Save();

                return session;
            }
        }

        public override void Save()
        {
            file.Seek(0, SeekOrigin.Begin);
            JsonSerializer.Serialize(file, Database);
        }

        public override void Dispose()
        {
            base.Dispose();
            file.Dispose();
        }
    }
}
