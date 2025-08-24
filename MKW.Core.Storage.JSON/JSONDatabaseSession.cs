using MKW.Core.Storage.JSON.Types;
using System.Text.Json;

namespace MKW.Core.Storage.JSON
{
    public class JSONDatabaseSession : MemoryDatabaseSession, IDatabase, ISavable
    {
        private readonly FileStream file;

        internal JSONDatabaseSession(JSONDatabase db, FileStream file)
            : base(db)
        {
            this.file = file;
        }

        public static JSONDatabaseSession Open(string path, bool readOnly)
        {
            FileStream file = File.Open(path,
                                        FileMode.Open,
                                        readOnly ? FileAccess.Read : FileAccess.ReadWrite);

            JSONDatabase database = JsonSerializer.Deserialize<JSONDatabase>(file)!;

            return new JSONDatabaseSession(database, file /* move */);
        }

        public static JSONDatabaseSession Create(string path)
        {
            FileStream file = File.Create(path);

            JSONDatabase database = new JSONDatabase();

            JSONDatabaseSession session = new JSONDatabaseSession(database, file /* move */);

            // Writes empty database to file to the disk
            session.Save();

            return session;
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
