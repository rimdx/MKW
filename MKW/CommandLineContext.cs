using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using System.CommandLine;

namespace MKW
{
    public partial class CommandLineContext
    {
        private readonly RootCommand rootCommand;

        private readonly Command cmdAddUser;
        private readonly Command cmdUser;
        private readonly Command cmdEntry;
        private readonly Command cmdAddEntry;
        private readonly Command cmdTouch;
        private readonly Command cmdListEntries;

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

            cmdUser = new Command("user", "user management commands");

            cmdAddUser = new Command("add", "adds a user to the database");
            cmdAddUser.Arguments.Add(argFile);
            cmdAddUser.Options.Add(optPassword);
            cmdAddUser.Options.Add(optNonInteractive);
            cmdAddUser.Options.Add(optForceInteractive);
            cmdAddUser.SetAction(AddUserAction);
            cmdUser.Subcommands.Add(cmdAddUser);

            cmdEntry = new Command("entry", "entry management commands");

            cmdAddEntry = new Command("add", "adds an encrypted entry to the database");
            cmdAddEntry.Arguments.Add(argFile);
            cmdAddEntry.Arguments.Add(argPayload);
            cmdAddEntry.Options.Add(optNonInteractive);
            cmdAddEntry.Options.Add(optForceInteractive);
            cmdAddEntry.SetAction(AddEntryAction);
            cmdEntry.Subcommands.Add(cmdAddEntry);

            cmdListEntries = new Command("list", "list all entries in the database");
            cmdListEntries.Arguments.Add(argFile);
            cmdListEntries.Options.Add(optPassword);
            cmdListEntries.Options.Add(optNonInteractive);
            cmdListEntries.Options.Add(optForceInteractive);
            cmdListEntries.SetAction(ListEntriesAction);
            cmdEntry.Subcommands.Add(cmdListEntries);

            cmdTouch = new Command("create", "initializes empty database");
            cmdTouch.Arguments.Add(argFile);
            cmdTouch.Options.Add(optNonInteractive);
            cmdTouch.Options.Add(optForceInteractive);
            cmdTouch.SetAction(CreateAction);

            rootCommand.Subcommands.Add(cmdUser);
            rootCommand.Subcommands.Add(cmdEntry);
            rootCommand.Subcommands.Add(cmdTouch);
        }

        public int Execute(IReadOnlyList<string> args)
        {
            ParseResult parsed = rootCommand.Parse(args);
            return parsed.Invoke();
        }

        private string GetFilePath(ParseResult argv)
        {
            return argv.GetRequiredValue(argFile);
        }

        public IDatabase OpenDatabase(ParseResult argv)
        {
            return JSONDatabaseSession.Open(GetFilePath(argv), DatabaseOpenMode.Open);
        }

        public ClientSession OpenSession(ParseResult argv)
        {
            return ClientSession.Open(OpenDatabase(argv), true);
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