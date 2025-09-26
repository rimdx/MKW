using MKW.Common;
using System.Text.Json;

namespace MKW.Core.Storage.JSON
{
    public class JSONDatabaseSession : MemoryDatabaseSession, IDatabase
    {
        private readonly string path;
        private readonly FileSystemWatcher watcher;

        internal JSONDatabaseSession(JSONDatabase db, string path)
            : base(db)
        {
            this.path = Path.GetFullPath(path); /* ? */

            watcher = new FileSystemWatcher(Path.GetDirectoryName(this.path)!)
            {
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
                Filter = Path.GetFileName(this.path),
            };

            watcher.Changed += Watcher_Changed;
        }

        // watcher thread
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            OnDatabaseFileUpdated();
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

            using (TempFile file = TempFile.Create(path))
            {
                JsonSerializer.Serialize(file, database);

                file.Accept();
            }

            return new JSONDatabaseSession(database, path);
        }

        public override void Save()
        {
            using TempFile file = TempFile.Create(path);

            JsonSerializer.Serialize(file, Database);

            file.Accept();
        }

        public override void ReloadDatabaseFile()
        {
            using FileStream file = File.Open(path,
                                              FileMode.Open,
                                              FileAccess.Read);

            Database = JsonSerializer.Deserialize<JSONDatabase>(file)!;
        }

        public override void Dispose()
        {
            base.Dispose();
            watcher.Dispose();
        }
    }
}
