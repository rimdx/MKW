using System.CommandLine;

namespace MKW
{
    public class CommandLineContext
    {
        private readonly RootCommand rootCommand;

        public CommandLineContext()
        {
            rootCommand = new RootCommand("Multi-Key Wallet")
            {
                new Command("entry", "entry management commands")
                {
                    new AddEntryCommand(),
                    new ListEntriesCommand(),
                },
                new Command("user", "user management commands")
                {
                    new AddUserCommand(),
                },
                new CreateCommand(),
            };
            rootCommand.Action = new UsageAction();
        }

        public int Execute(IReadOnlyList<string> args)
        {
            ParseResult parsed = rootCommand.Parse(args);
            return parsed.Invoke();
        }
    }
}