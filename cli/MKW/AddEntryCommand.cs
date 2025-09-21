using MKW.Core;
using MKW.Core.Notify;
using MKW.Core.Storage;
using System.CommandLine;

namespace MKW
{
    public class AddEntryCommand : UserCommand
    {
        public AddEntryCommand() : base(/* entry */ "add", "adds an encrypted entry to the database")
        {
            Add(CommonOptions.Payload);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);

            string payload = argv.GetRequiredValue(CommonOptions.Payload);

            EntryInfo entry = ctx.User.UpdateEntry(EntryId.Create(), new EntryPayload(payload));

            Console.WriteLine($"{entry.Action}: {entry.Id} for {entry.EncodedForUsers.Count} users");
        }
    }
}
