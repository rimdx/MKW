using MKW.Core;
using MKW.Core.Storage;
using System.CommandLine;

namespace MKW
{
    public class AddTrustCommand : AdminCommand
    {
        public AddTrustCommand() : base("trust", "todo")
        {
            Add(CommonOptions.UserId);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);

            UserId userId = UserId.FromGuid(argv.GetRequiredValue(CommonOptions.UserId));

            ctx.Admin.AddTrust(userId);

            Console.WriteLine($"Marked user '{userId}' as trusted.");
        }
    }
}
