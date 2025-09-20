using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Notify;
using System.CommandLine;

namespace MKW
{
    public class AddUserCommand : MKWCommand
    {
        public AddUserCommand() : base(/* user, */ "add", "adds a user to the database")
        {
            Add(CommonOptions.Password);
        }

        protected override void Execute(ParseResult argv)
        {
            using ClientSession session = OpenSession(argv);

            // TODO:

            UserAccessRequest request = session.CreateUserAccessRequest(GetPassword(argv));

            UserInfo user = session.CreateUser(
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
