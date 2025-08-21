using System.CommandLine;

namespace MKW
{
    public static class CommonOptions
    {
        public readonly static Argument<string> File = new("file")
        {
            Description = "path to the database file",
        };

        public readonly static Argument<string> Payload = new("payload")
        {
            Description = "secret payload of the entry",
        };

        public readonly static Option<string> Password = new("--password")
        {
            Description = "password to perform operation with",
        };

        public readonly static Option<bool> NonInteractive = new("--non-interactive")
        {
            Description = "do no interactive prompting (default is to prompt only if standard input is a terminal device)"
        };

        public readonly static Option<bool> ForceInteractive = new("--force-interactive")
        {
            Description = "do interactive prompting even if standard input is not a terminal device"
        };
    }
}
