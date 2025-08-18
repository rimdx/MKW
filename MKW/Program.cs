using MKW.Core;
using MKW.Core.Storage;
using System.CommandLine;

namespace MKW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Argument<string> fileArgument = new Argument<string>("file")
            {
                Description = "path to the database file",
            };

            Option<string> passwordOption = new Option<string>("--password")
            {
                Description = "password to perform operation with",
            };

            Option<string> userId = new Option<string>("--user")
            {
                Description = "user ID",
            };

            RootCommand rootCommand = new RootCommand("Multi-Key Wallet");

            Command addUserCommand = new Command("add-user", "adds a user to the database");
            addUserCommand.Arguments.Add(fileArgument);
            addUserCommand.Options.Add(passwordOption);

            addUserCommand.SetAction(argv =>
            {
                string path = argv.GetRequiredValue(fileArgument);
                string password = argv.GetRequiredValue(passwordOption);

                using JSONDatabaseSession database = JSONDatabaseSession.Open(path);
                ClientSession session = new ClientSession(database);

                session.AddUser(password);
            });

            Command touchCommand = new Command("touch", "initializes empty database");
            touchCommand.Arguments.Add(fileArgument);

            touchCommand.SetAction(argv =>
            {
                string path = argv.GetRequiredValue(fileArgument);

                using JSONDatabaseSession database = JSONDatabaseSession.Open(path);
                ClientSession session = new ClientSession(database);
            });

            rootCommand.Subcommands.Add(addUserCommand);
            rootCommand.Subcommands.Add(touchCommand);

            ParseResult parsed = rootCommand.Parse(args);
            int code = parsed.Invoke();
        }
    }
}