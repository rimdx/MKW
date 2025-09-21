using MKW.Core;
using MKW.Core.Client;
using System.CommandLine;

namespace MKW
{
    public class ListEntriesCommand : UserCommand
    {
        public ListEntriesCommand() : base("list", "list all entries in the database")
        {
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);

            foreach (IEntrySession entry in ctx.User.EnumerateEntries())
            {
                Console.WriteLine($"-- {entry.Id}:");

                EntryPayload? payload = entry.OpenPayload();

                if (payload == null)
                {
                    Console.WriteLine($"[hidden]");
                }
                else
                {
                    Console.WriteLine($"{payload}");
                }
            }
        }
    }
}
