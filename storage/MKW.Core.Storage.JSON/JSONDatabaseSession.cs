using MKW.Core.Storage.JSON.Types;
using System.Text.Json;

namespace MKW.Core.Storage.JSON
{
    public class JSONDatabaseSession : MemoryDatabaseSession, IDatabase, ISavable
    {
        private readonly string path;

        internal JSONDatabaseSession(JSONDatabase db, string path)
            : base(db)
        {
            this.path = path;
        }

        public static JSONDatabaseSession Open(string path)
        {
            using FileStream file = File.Open(path,
                                              FileMode.Open,
                                              FileAccess.Read);

            JSONDatabase database = JsonSerializer.Deserialize<JSONDatabase>(file)!;

            return new JSONDatabaseSession(database, path);
        }

        public static JSONDatabaseSession Create(string path)
        {
            JSONDatabase database = new JSONDatabase();
            JSONDatabaseSession session = new JSONDatabaseSession(database, path /* move */);

            using (File.Create(path))
            {
            }

            session.Save();

            return session;
        }

        public override void Save()
        {
            // TODO: properly generate file name
            string tmpPath = path + ".tmp";

            try
            {
                using (FileStream file = new FileStream(tmpPath,
                                                        FileMode.CreateNew,
                                                        FileAccess.Write))
                {
                    JsonSerializer.Serialize(file, Database);
                    file.Flush(true);
                }

                File.Replace(tmpPath, path, null);
            }
            catch
            {
                // cleanup
                File.Delete(tmpPath);
                throw;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
