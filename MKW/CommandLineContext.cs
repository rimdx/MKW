using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace MKW
{
    public class CommandLineContext
    {
        private readonly RootCommand rootCommand;

        private readonly Command cmdAddUser;
        private readonly Command cmdTouch;
        private readonly Command cmdEntries;

        private readonly Argument<string> argFile = new("file")
        {
            Description = "path to the database file",
        };

        private readonly Option<string> optPassword = new("--password")
        {
            Description = "password to perform operation with",
            DefaultValueFactory = PasswordDefaultValueFactory,
        };

        public CommandLineContext()
        {
            rootCommand = new RootCommand("Multi-Key Wallet");

            cmdAddUser = new Command("add-user", "adds a user to the database");
            cmdAddUser.Arguments.Add(argFile);
            cmdAddUser.Options.Add(optPassword);

            cmdAddUser.SetAction(AddUserAction);

            cmdTouch = new Command("touch", "initializes empty database");
            cmdTouch.Arguments.Add(argFile);
            cmdTouch.SetAction(TouchAction);

            cmdEntries = new Command("entries", "list all entries in the database");
            cmdEntries.Arguments.Add(argFile);
            cmdEntries.Options.Add(optPassword);
            cmdEntries.SetAction(ListEntriesAction);

            rootCommand.Subcommands.Add(cmdAddUser);
            rootCommand.Subcommands.Add(cmdTouch);
            rootCommand.Subcommands.Add(cmdEntries);
        }

        public int Execute(IReadOnlyList<string> args)
        {
            ParseResult parsed = rootCommand.Parse(args);
            return parsed.Invoke();
        }

        public IDatabaseSession OpenDatabase(ParseResult argv)
        {
            string path = argv.GetRequiredValue(argFile);

            return JSONDatabaseSession.Open(path);
        }

        private void AddUserAction(ParseResult argv)
        {
            string password = argv.GetRequiredValue(optPassword);

            using IDatabaseSession database = OpenDatabase(argv);

            ClientSession session = new ClientSession(database);

            UserInfo user = session.AddUser(password);

            Console.WriteLine($"User added with ID: {user.Id}");
        }

        private void TouchAction(ParseResult argv)
        {
            using IDatabaseSession database = OpenDatabase(argv);
        }

        private void ListEntriesAction(ParseResult argv)
        {
            using IDatabaseSession database = OpenDatabase(argv);
            ClientSession session = new ClientSession(database);

            UserSession userSession = session.OpenUser(argv.GetRequiredValue(optPassword));

            foreach (var entry in userSession.EnumerateEntries())
            {
                Console.WriteLine($"-- {entry.Id}:");

                if (entry.Payload == null)
                {
                    Console.WriteLine($"[hidden]");
                }
                else
                {
                    Console.WriteLine($"{entry.Payload}");
                }
            }
        }

        private static string PasswordDefaultValueFactory(ArgumentResult result)
        {
            Console.Write("Enter password: ");

            string? password = Console.ReadLine();

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }
            else
            {
                return password;
            }
        }
    }
}