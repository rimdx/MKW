using MKW.Core;
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

            path = Path.GetFullPath(path);

            DatabaseFileService databaseFileService = new DatabaseFileService();
            Database database = EnsureDatabase(databaseFileService, path);
            DatabaseSession session = new DatabaseSession(database);

            Console.WriteLine($"[Verbose] Successfully opened database file.");

            // [path, "--add-user", "password123"] $= 3
            if (args.Length >= 3)
            {
                string command = args[1].ToLowerInvariant();
                string value = args[2];

                if (command == "--add-user")
                {
                    Console.WriteLine($"[Verbose] Adding user with password: '{value}'.");
                    session.AddUser(value);
                    Console.WriteLine($"[Verbose] User added successfully.");
                }
                else
                {
                    Console.Error.WriteLine($"mkw.exe: invalid option: '{command}'");
                    Console.Error.WriteLine($"Type 'mkw --help' for usage.");
                    return;
                }
            }

            databaseFileService.Save(database, path);
            Console.WriteLine($"[Verbose] Successfully closed database file.");
        }

        private static Database EnsureDatabase(DatabaseFileService databaseFileService, string path)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"[Verbose] Opening database file: '{path}'.");

                return databaseFileService.Open(path);
            }
            else
            {
                Console.WriteLine($"[Verbose] Initializing database file: '{path}'.");

                Database database = new Database
                {
                    Users = new List<User>()
                };

                databaseFileService.Save(database, path);

                return database;
            }
        }
    }
}