using MKW.Core.Storage;
using NUnit.Framework;
using System.Diagnostics;
using System.Text;

namespace MKW.Tests
{
    public class SandBox : IDisposable
    {
        public string DatabasePath { get; }

        public SandBox()
        {
            DatabasePath = Path.GetFullPath("./test.json");

            // todo: safer way?
            File.Delete("./test.json");
        }

        public void Dispose()
        {
        }

        public string Run(string cmd, string stdin = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {cmd}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = Process.Start(startInfo)!;

            process.StandardInput.Write(stdin);
            process.StandardInput.Close();

            process.WaitForExit();

            StringBuilder result = new StringBuilder();

            result.AppendLine($"  -- EXIT CODE: {process.ExitCode}");

            var stderr = TrimString(process.StandardError.ReadToEnd());
            var stdout = TrimString(process.StandardOutput.ReadToEnd());

            if (stderr != "")
            {
                result.AppendLine("  -- STDERR:");
                result.AppendLine(stderr);
            }

            if (stdout != "")
            {
                result.AppendLine("  -- STDOUT:");
                result.AppendLine(stdout);
            }

            Console.WriteLine(result.ToString());

            return result.ToString();
        }

        public IDatabaseSession OpenDatabase()
        {
            return JSONDatabaseSession.Open(DatabasePath);
        }

        private string TrimString(string str)
        {
            return str.Trim([' ', '\n', '\r', '\t']);
        }
    }
}
