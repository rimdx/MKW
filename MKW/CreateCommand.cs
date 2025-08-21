using MKW.Core.Storage.JSON;
using System.CommandLine;

namespace MKW
{
    public class CreateCommand : MKWCommand
    {
        public CreateCommand() : base("create", "initializes empty database")
        {
        }

        protected override void Execute(ParseResult argv)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(
                GetFilePath(argv),
                Core.Storage.DatabaseOpenMode.OpenOrCreate);

            // no-op

            db.Dispose();
        }
    }
}
