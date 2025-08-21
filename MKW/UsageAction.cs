using System.CommandLine;
using System.CommandLine.Invocation;

namespace MKW
{
    internal class UsageAction : SynchronousCommandLineAction
    {
        public UsageAction()
        {
        }

        public override int Invoke(ParseResult parseResult)
        {
            TextWriter output = parseResult.InvocationConfiguration.Output;

            output.WriteLine("Type 'mkw help' for usage.");

            return 0;
        }
    }
}
