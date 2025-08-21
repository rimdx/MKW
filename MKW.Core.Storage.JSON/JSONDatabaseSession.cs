using MKW.Core.Storage.JSON.Types;
using System.Text.Json;

namespace MKW.Core.Storage.JSON
{
    public class JSONDatabaseSession : MemoryDatabaseSession, IDatabase, ISavable
    {
        private readonly FileStream file;

        protected JSONDatabaseSession(JSONDatabase db, FileStream file)
            : base(db)
        {
            this.file = file;
        }

        public static JSONDatabaseSession Open(string path, DatabaseOpenMode mode)
        {
            if (File.Exists(path))
            {
                FileStream file = new FileStream(path,
                                                 mode.GetNativeFileMode(),
                                                 mode.GetNativeFileAccess());

                JSONDatabase database = JsonSerializer.Deserialize<JSONDatabase>(file)!;

                return new JSONDatabaseSession(database, file /* move */);
            }
            else
            {
                FileStream file = new FileStream(path,
                                                 mode.GetNativeFileMode(),
                                                 mode.GetNativeFileAccess());

                JSONDatabase database = new JSONDatabase();

                JSONDatabaseSession session = new JSONDatabaseSession(database, file /* move */);

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
