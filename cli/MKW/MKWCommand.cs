using System.CommandLine;

namespace MKW
{
    public abstract class MKWCommand : Command
    {
        protected MKWCommand(string name, string? description = null)
            : base(name, description)
        {
            Add(CommonOptions.NonInteractive);
            Add(CommonOptions.ForceInteractive);

            SetAction(ExecuteInternal);
        }

        private void ExecuteInternal(ParseResult argv)
        {
            try
            {
                using ExecutionContext ctx = new ExecutionContext();

                Execute(argv, ctx);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        protected virtual void Execute(ParseResult argv, ExecutionContext ctx)
        {
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
                return PromptPassword(argv, "Enter password:");
            }
            else
            {
                return password;
            }
        }

        protected string PromptPassword(ParseResult argv, string message)
        {
            EnsureInteractive(argv, "Please provide password using --password option.");

            Console.Write(message);

            string? password = Console.ReadLine();

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            return password;
        }
    }
}
