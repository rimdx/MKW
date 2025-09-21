using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage.JSON;
using System.CommandLine;

namespace MKW
{
    public class CreateCommand : MKWCommand
    {
        public CreateCommand() : base("create", "initializes empty database")
        {
            Add(CommonOptions.File);
            Add(CommonOptions.Password);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);

            string path = argv.GetRequiredValue(CommonOptions.File);

            UserMetadata metadata = new UserMetadata
            {
                DisplayName = "",
                UserId = ""
            };

            string password = GetPassword(argv);

            ctx.CreateDatabase(path, password, metadata);
        }
    }
}
