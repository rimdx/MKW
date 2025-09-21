using System.CommandLine;

namespace MKW
{
    public abstract class DatabaseCommand : MKWCommand
    {
        protected DatabaseCommand(string name, string? description = null)
            : base(name, description)
        {
            Add(CommonOptions.File);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);
            ctx.OpenDatabase(argv.GetRequiredValue(CommonOptions.File));
        }
    }
}
