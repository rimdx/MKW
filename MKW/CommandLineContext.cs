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
        private readonly Command cmdAddEntry;
        private readonly Command cmdTouch;
        private readonly Command cmdEntries;

        private readonly Argument<string> argFile = new("file")
        {
            Description = "path to the database file",
        };

        private readonly Argument<string> argPayload = new("payload")
        {
            Description = "secret payload of the entry",
        };

        private readonly Option<string> optPassword = new("--password")
        {
            Description = "password to perform operation with",
        };

        private readonly Option<bool> optNonInteractive = new("--non-interactive")
        {
            Description = "do no interactive prompting (default is to prompt only if standard input is a terminal device)"
        };

        private readonly Option<bool> optForceInteractive = new("--force-interactive")
        {
            Description = "do interactive prompting even if standard input is not a terminal device"
        };

        public CommandLineContext()
        {
            rootCommand = new RootCommand("Multi-Key Wallet");

            cmdAddUser = new Command("add-user", "adds a user to the database");
            cmdAddUser.Arguments.Add(argFile);
            cmdAddUser.Options.Add(optPassword);
            cmdAddUser.Options.Add(optNonInteractive);
            cmdAddUser.Options.Add(optForceInteractive);
            cmdAddUser.SetAction(AddUserAction);

            cmdAddEntry = new Command("add-entry", "adds an encrypted entry to the database");
            cmdAddEntry.Arguments.Add(argFile);
            cmdAddEntry.Arguments.Add(argPayload);
            cmdAddEntry.Options.Add(optNonInteractive);
            cmdAddEntry.Options.Add(optForceInteractive);
            cmdAddEntry.SetAction(AddEntryAction);

            cmdTouch = new Command("touch", "initializes empty database");
            cmdTouch.Arguments.Add(argFile);
            cmdTouch.Options.Add(optNonInteractive);
            cmdTouch.Options.Add(optForceInteractive);
            cmdTouch.SetAction(TouchAction);

            cmdEntries = new Command("entries", "list all entries in the database");
            cmdEntries.Arguments.Add(argFile);
            cmdEntries.Options.Add(optPassword);
            cmdEntries.Options.Add(optNonInteractive);
            cmdEntries.Options.Add(optForceInteractive);
            cmdEntries.SetAction(ListEntriesAction);

            rootCommand.Subcommands.Add(cmdAddUser);
            rootCommand.Subcommands.Add(cmdAddEntry);
            rootCommand.Subcommands.Add(cmdTouch);
            rootCommand.Subcommands.Add(cmdEntries);
        }

        public int Execute(IReadOnlyList<string> args)
        {
            ParseResult parsed = rootCommand.Parse(args);
            return parsed.Invoke();
        }

        public IDatabase OpenDatabase(ParseResult argv)
        {
            string path = argv.GetRequiredValue(argFile);

            return JSONDatabaseSession.Open(path);
        }

        private void AddUserAction(ParseResult argv)
        {
            using IDatabase database = OpenDatabase(argv);

            using ClientSession session = new ClientSession(database);

            UserInfo user = session.PromoteUser(GetPassword(argv));

            Console.WriteLine($"User added with ID: {user.Id}");
        }

        private void AddEntryAction(ParseResult argv)
        {
            string payload = argv.GetRequiredValue(argPayload);

            using IDatabase database = OpenDatabase(argv);
            using ClientSession session = new ClientSession(database);

            EntryInfo entry = session.UpdateEntry(Guid.NewGuid(), new EntryPayload(payload));

            Console.WriteLine($"{entry.Action}: {entry.Id} for {entry.EncodedForUsers.Count} users");
        }

        private void TouchAction(ParseResult argv)
        {
            using IDatabase database = OpenDatabase(argv);
        }

        private void ListEntriesAction(ParseResult argv)
        {
            using IDatabase database = OpenDatabase(argv);
            using ClientSession session = new ClientSession(database);

            using UserSession userSession = session.OpenUser(GetPassword(argv));

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

        private void EnsureInteractive(ParseResult argv, string errorMessage)
        {
            bool non_interactive = argv.GetValue(optNonInteractive);
            bool force_interactive = argv.GetValue(optForceInteractive);

            /* The --non-interactive and --force-interactive options are mutually
             * exclusive. */
            if (non_interactive && force_interactive)
            {
                throw new Exception(errorMessage, new Exception("--non-interactive and --force-interactive are mutually exclusive"));
            }

            /* If neither --non-interactive nor --force-interactive was passed,
             * be interactive if stdin is a terminal.
             * If --force-interactive was passed, always be interactive. */
            if (!force_interactive && !non_interactive)
            {
                if (Console.IsInputRedirected)
                {
                    throw new Exception(errorMessage, new Exception("Standard input has been redirected."));
                }
            }

            if (non_interactive)
                throw new Exception(errorMessage, new Exception("Interactive prompts are disabled by --non-interactive option."));
        }

        private string GetPassword(ParseResult argv)
        {
            string? password = argv.GetValue(optPassword);

            if (password == null)
            {
                EnsureInteractive(argv, "Please provide password using --password option.");

                Console.Write("Enter password: ");

                password = Console.ReadLine();

                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Password cannot be empty.");
                }
            }

            return password;
        }
    }
}