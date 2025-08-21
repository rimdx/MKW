using System.CommandLine;

namespace MKW
{
    public class CommandLineContext : RootCommand
    {
        public CommandLineContext() : base("Multi-Key Wallet")
        {
            Add(new Command("entry", "entry management commands")
            {
                new AddEntryCommand(),
                new ListEntriesCommand(),
            });

            Add(new Command("user", "user management commands")
            {
                new AddUserCommand(),
            });

            Add(new CreateCommand());

            Action = new UsageAction();
        }

        public int Execute(IReadOnlyList<string> args)
        {
            ParseResult parsed = Parse(args);
            return parsed.Invoke();
        }
    }
}