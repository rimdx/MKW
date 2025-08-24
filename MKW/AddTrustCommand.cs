using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using System.CommandLine;

namespace MKW
{
    public class AddTrustCommand : MKWCommand
    {
        public AddTrustCommand() : base(/* admin */ "trust", "todo")
        {
            Add(CommonOptions.Password);
            Add(CommonOptions.UserId);
        }

        protected override void Execute(ParseResult argv)
        {
            UserId userId = UserId.FromGuid(argv.GetRequiredValue(CommonOptions.UserId));

            using ClientSession session = OpenSession(argv);
            using AdminSession admin = session.OpenAdmin(GetPassword(argv));

            admin.UpdateTrust(userId, Trust.ExplicitTrust);

            Console.WriteLine($"Marked user '{userId}' as trusted.");
        }
    }
}
