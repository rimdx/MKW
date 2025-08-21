using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage.JSON;
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
            string payload = argv.GetRequiredValue(CommonOptions.Payload);

            using ClientSession session = OpenSession(argv);

            EntryInfo entry = session.UpdateEntry(Guid.NewGuid(), new EntryPayload(payload));

            Console.WriteLine($"{entry.Action}: {entry.Id} for {entry.EncodedForUsers.Count} users");
        }

        private void CreateAction(ParseResult argv)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(
                GetFilePath(argv),
                Core.Storage.DatabaseOpenMode.OpenOrCreate);

            // no-op

            db.Dispose();
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