using MKW.Core;
using MKW.Core.Client;
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
            using IAdminSession admin = session.OpenAdmin(GetPassword(argv));

            admin.AddTrust(userId);

            Console.WriteLine($"Marked user '{userId}' as trusted.");
        }
    }
}
