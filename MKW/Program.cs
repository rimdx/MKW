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

            Console.WriteLine($"[Verbose] Opening database file: '{path}'.");

            using DatabaseSession database = DatabaseSession.Open(path);
            ClientSession session = new ClientSession(database);

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

            database.Save();
            Console.WriteLine($"[Verbose] Successfully closed database file.");
        }
    }
}