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
            Add(CommonOptions.Password);
        }

        protected override void Execute(ParseResult argv)
        {
            using JSONDatabaseSession db = JSONDatabaseSession.Create(GetFilePath(argv));

            using ClientSession client = ClientSession.Create(db,
                                                              GetPassword(argv),
                                                              new UserMetadata
                                                              {
                                                                  DisplayName = "",
                                                                  UserId = ""
                                                              });
        }
    }
}
