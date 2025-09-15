using MKW.Core;
using MKW.Core.Client;
using System.CommandLine;

namespace MKW
{
    public class ListEntriesCommand : MKWCommand
    {
        public ListEntriesCommand() : base(/* entry */ "list", "list all entries in the database")
        {
            Add(CommonOptions.Password);
        }

        protected override void Execute(ParseResult argv)
        {
            using ClientSession session = OpenSession(argv);
            using IUserSession userSession = session.OpenUser(GetPassword(argv));

            foreach (IEntrySession entry in userSession.EnumerateEntries())
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
