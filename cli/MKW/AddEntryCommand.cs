using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Notify;
using MKW.Core.Storage;
using System.CommandLine;

namespace MKW
{
    public class AddEntryCommand : MKWCommand
    {
        public AddEntryCommand() : base(/* entry */ "add", "adds an encrypted entry to the database")
        {
            Add(CommonOptions.Password);
            Add(CommonOptions.Payload);
        }

        protected override void Execute(ParseResult argv)
        {
            string payload = argv.GetRequiredValue(CommonOptions.Payload);

            using ClientSession client = OpenSession(argv);
            using IUserSession user = client.OpenUser(GetPassword(argv));

            EntryInfo entry = user.UpdateEntry(EntryId.Create(), new EntryPayload(payload));

            Console.WriteLine($"{entry.Action}: {entry.Id} for {entry.EncodedForUsers.Count} users");
        }
    }
}
