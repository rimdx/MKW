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
            using UserSession userSession = session.OpenUser(GetPassword(argv));

            foreach (KeyedEntry entry in userSession.EnumerateEntries())
            {
                Console.WriteLine($"-- {entry.Id}:");

                if (entry.Payload == null)
                {
                    Console.WriteLine($"[hidden]");
                }
                else
                {
                    Console.WriteLine($"{entry.Payload}");
                }
            }
        }
    }
}
