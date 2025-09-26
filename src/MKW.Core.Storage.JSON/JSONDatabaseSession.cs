using MKW.Common;
using System.Text.Json;

namespace MKW.Core.Storage.JSON
{
    public class JSONDatabaseSession : MemoryDatabaseSession, IDatabase
    {
        private readonly string path;
        private readonly FileSystemWatcher watcher;

        private readonly object saveLock;
        private int pendingSaves;

        internal JSONDatabaseSession(JSONDatabase db, string path)
            : base(db)
        {
            this.path = Path.GetFullPath(path); /* ? */

            saveLock = new object();
            pendingSaves = 0;

            watcher = new FileSystemWatcher(Path.GetDirectoryName(this.path)!)
            {
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
                Filter = Path.GetFileName(this.path),
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.FileName |
                               NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Security |
                               NotifyFilters.Size,
            };

            watcher.Changed += Watcher_Changed;
        }

        // watcher thread
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (saveLock)
            {
                if (pendingSaves > 0)
                {
                    pendingSaves--;
                    // don't trigger update
                }
                else
                {
                    OnDatabaseFileUpdated();
                }
            }
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
            lock (saveLock)
            {
                using TempFile file = TempFile.Create(path);

                JsonSerializer.Serialize(file, Database);

                file.Accept();

                pendingSaves++;
            }
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
