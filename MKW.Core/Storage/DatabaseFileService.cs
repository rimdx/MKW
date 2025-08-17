using System.Text.Json;

namespace MKW.Core.Storage
{
    public interface IDatabaseService
    {
        Database Open(string path);
        void Save(Database database, string path);
    }

    public class DatabaseFileService : IDatabaseService
    {
        public Database Open(string path)
        {
            using FileStream fstream = File.OpenRead(path);

            return JsonSerializer.Deserialize<Database>(fstream)!;
        }

        public void Save(Database database, string path)
        {
            using FileStream fstream = File.OpenWrite(path);

            JsonSerializer.Serialize(fstream, database);
        }
    }
}
