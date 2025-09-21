using System.CommandLine;

namespace MKW
{
    public static class CommonOptions
    {
        public static readonly Argument<string> File = new("file")
        {
            Description = "path to the database file",
        };

        public static readonly Argument<string> Payload = new("payload")
        {
            Description = "secret payload of the entry",
        };

        public static readonly Option<string> Password = new("--password")
        {
            Description = "password to perform operation with",
        };

        public static readonly Option<string> UserPassword = new("--userpassword")
        {
            Description = "password to perform operation with",
        };

        public static readonly Option<Guid> UserId = new("--userid")
        {
            Description = "user id",
        };

        public static readonly Option<bool> NonInteractive = new("--non-interactive")
        {
            Description = "do no interactive prompting (default is to prompt only if standard input is a terminal device)"
        };

        public static readonly Option<bool> ForceInteractive = new("--force-interactive")
        {
            Description = "do interactive prompting even if standard input is not a terminal device"
        };
    }
}
