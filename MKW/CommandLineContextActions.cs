using MKW.Core.Client;
using MKW.Core.Client.Notify;
using System.CommandLine;

namespace MKW
{
    public partial class CommandLineContext
    {
        private void AddUserAction(ParseResult argv)
        {
            using ClientSession session = OpenSession(argv);

            UserInfo user = session.PromoteUser(GetPassword(argv));

            Console.WriteLine($"User added with ID: {user.Id}");
        }

        private void AddEntryAction(ParseResult argv)
        {
            string payload = argv.GetRequiredValue(argPayload);

            using ClientSession session = OpenSession(argv);

            EntryInfo entry = session.UpdateEntry(Guid.NewGuid(), new EntryPayload(payload));

            Console.WriteLine($"{entry.Action}: {entry.Id} for {entry.EncodedForUsers.Count} users");
        }

        private void TouchAction(ParseResult argv)
        {
            using ClientSession session = OpenSession(argv);
        }

        private void ListEntriesAction(ParseResult argv)
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