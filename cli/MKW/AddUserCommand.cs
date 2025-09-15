using MKW.Core.Client;
using MKW.Core.Client.Notify;
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

            UserInfo user = session.PromoteUser(GetPassword(argv));

            Console.WriteLine($"User added with ID: {user.Id}");
        }
    }
}
