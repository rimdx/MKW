using MKW.Core;
using MKW.Core.Storage;
using System.CommandLine;

namespace MKW
{
    public class CommandLineContext
    {
        private readonly RootCommand rootCommand;

        private readonly Command cmdAddUser;
        private readonly Command cmdTouch;

        private readonly Argument<string> argFile = new Argument<string>("file")
        {
            Description = "path to the database file",
        };

        private readonly Option<string> optPassword = new Option<string>("--password")
        {
            Description = "password to perform operation with",
        };

        private readonly Option<string> optUserId = new Option<string>("--user")
        {
            Description = "user ID",
        };

        public CommandLineContext()
        {
            rootCommand = new RootCommand("Multi-Key Wallet");

            cmdAddUser = new Command("add-user", "adds a user to the database");
            cmdAddUser.Arguments.Add(argFile);
            cmdAddUser.Options.Add(optPassword);

            cmdAddUser.SetAction(argv =>
            {
                string path = argv.GetRequiredValue(argFile);
                string password = argv.GetRequiredValue(optPassword);

                using JSONDatabaseSession database = JSONDatabaseSession.Open(path);
                ClientSession session = new ClientSession(database);

                session.AddUser(password);
            });

            cmdTouch = new Command("touch", "initializes empty database");
            cmdTouch.Arguments.Add(argFile);

            cmdTouch.SetAction(argv =>
            {
                string path = argv.GetRequiredValue(argFile);

                using JSONDatabaseSession database = JSONDatabaseSession.Open(path);
                ClientSession session = new ClientSession(database);
            });

            rootCommand.Subcommands.Add(cmdAddUser);
            rootCommand.Subcommands.Add(cmdTouch);
        }

        public int Execute(IReadOnlyList<string> args)
        {
            ParseResult parsed = rootCommand.Parse(args);
            return parsed.Invoke();
        }
    }
}