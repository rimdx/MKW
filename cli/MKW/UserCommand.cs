using System.CommandLine;

namespace MKW
{
    public abstract class UserCommand : DatabaseCommand
    {
        protected UserCommand(string name, string? description = null)
            : base(name, description)
        {
            Add(CommonOptions.Password);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);
            ctx.OpenUser(GetPassword(argv));
        }
    }
}
