using System.CommandLine;

namespace MKW
{
    public abstract class AdminCommand : DatabaseCommand
    {
        protected AdminCommand(string name, string? description = null)
            : base(name, description)
        {
            Add(CommonOptions.Password);
        }

        protected override void Execute(ParseResult argv, ExecutionContext ctx)
        {
            base.Execute(argv, ctx);
            ctx.OpenAdmin(GetPassword(argv));
        }
    }
}
