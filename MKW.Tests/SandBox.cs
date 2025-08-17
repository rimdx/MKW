using NUnit.Framework;
using System.Diagnostics;
using System.Text;

namespace MKW.Tests
{
    public class SandBox : IDisposable
    {
        public SandBox()
        {
            // todo: safer way?
            File.Delete("./test.json");
        }

        public void Dispose()
        {
        }

        public string Run(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {cmd}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = Process.Start(startInfo)!;

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

            return result.ToString();
        }

        private string TrimString(string str)
        {
            return str.Trim([' ', '\n', '\r', '\t']);
        }
    }
}
