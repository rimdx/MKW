using MKW.Core;
using MKW.Core.Notify;
using System.CommandLine;

namespace MKW
{
    public class AddUserCommand : AdminCommand
    {
        public AddUserCommand() : base("add", "adds a user to the database")
        {
            Add(CommonOptions.UserPassword);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);

            string password = argv.GetRequiredValue(CommonOptions.UserPassword);

            UserAccessRequest request = ctx.Client.CreateUserAccessRequest(password);

            UserInfo user = ctx.Admin.CreateUser(
                request,
                new UserMetadata
                {
                    UserId = "",
                    DisplayName = "",
                });

            Console.WriteLine($"User added with ID: {user.Id}");
        }
    }
}
