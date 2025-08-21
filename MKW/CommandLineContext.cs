using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using System.CommandLine;
using System.CommandLine.Help;

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


        public CommandLineContext()
        {
            rootCommand = new RootCommand("Multi-Key Wallet")
            {
                Action = new UsageAction()
            };

            cmdUser = new Command("user", "user management commands");

            cmdAddUser = new Command("add", "adds a user to the database");
            cmdAddUser.Arguments.Add(CommonOptions.File);
            cmdAddUser.Options.Add(CommonOptions.Password);
            cmdAddUser.Options.Add(CommonOptions.NonInteractive);
            cmdAddUser.Options.Add(CommonOptions.ForceInteractive);
            cmdAddUser.SetAction(AddUserAction);
            cmdUser.Subcommands.Add(cmdAddUser);

            cmdEntry = new Command("entry", "entry management commands");

            cmdAddEntry = new Command("add", "adds an encrypted entry to the database");
            cmdAddEntry.Arguments.Add(CommonOptions.File);
            cmdAddEntry.Arguments.Add(CommonOptions.Payload);
            cmdAddEntry.Options.Add(CommonOptions.NonInteractive);
            cmdAddEntry.Options.Add(CommonOptions.ForceInteractive);
            cmdAddEntry.SetAction(AddEntryAction);
            cmdEntry.Subcommands.Add(cmdAddEntry);

            cmdListEntries = new Command("list", "list all entries in the database");
            cmdListEntries.Arguments.Add(CommonOptions.File);
            cmdListEntries.Options.Add(CommonOptions.Password);
            cmdListEntries.Options.Add(CommonOptions.NonInteractive);
            cmdListEntries.Options.Add(CommonOptions.ForceInteractive);
            cmdListEntries.SetAction(ListEntriesAction);
            cmdEntry.Subcommands.Add(cmdListEntries);

            cmdTouch = new Command("create", "initializes empty database");
            cmdTouch.Arguments.Add(CommonOptions.File);
            cmdTouch.Options.Add(CommonOptions.NonInteractive);
            cmdTouch.Options.Add(CommonOptions.ForceInteractive);
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
            return argv.GetRequiredValue(CommonOptions.File);
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
            bool non_interactive = argv.GetValue(CommonOptions.NonInteractive);
            bool force_interactive = argv.GetValue(CommonOptions.ForceInteractive);

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
            string? password = argv.GetValue(CommonOptions.Password);

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