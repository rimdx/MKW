using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using System.CommandLine;

namespace MKW
{
    public abstract class MKWCommand : Command
    {
        protected MKWCommand(string name, string? description = null)
            : base(name, description)
        {
            Add(CommonOptions.File);
            Add(CommonOptions.NonInteractive);
            Add(CommonOptions.ForceInteractive);

            SetAction(Execute);
        }

        protected abstract void Execute(ParseResult argv);

        protected string GetFilePath(ParseResult argv)
        {
            return argv.GetRequiredValue(CommonOptions.File);
        }

        protected IDatabase OpenDatabase(ParseResult argv)
        {
            return JSONDatabaseSession.Open(GetFilePath(argv), DatabaseOpenMode.Open);
        }

        protected ClientSession OpenSession(ParseResult argv)
        {
            return ClientSession.Open(OpenDatabase(argv), true);
        }

        protected void EnsureInteractive(ParseResult argv, string errorMessage)
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

        protected string GetPassword(ParseResult argv)
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
