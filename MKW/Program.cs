using MKW.Core.Storage;

namespace MKW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Please provide the path to the database file.");
                return;
            }

            string path = args[0];

            Database database = EnsureDatabase(path);

            Console.WriteLine($"[Verbose] Successfully opened database file.");
        }

        private static Database EnsureDatabase(string path)
        {
            var service = new DatabaseFileService();

            if (File.Exists(path))
            {
                Console.WriteLine($"[Verbose] Opening database file: {path}.");

                return service.Open(path);
            }
            else
            {
                Console.WriteLine($"[Verbose] Initializing database file at {path}.");

                Database database = new Database
                {
                    Users = new List<User>()
                };

                service.Save(database, path);

                return database;
            }
        }
    }
}