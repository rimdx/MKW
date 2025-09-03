using System.Diagnostics;
using System.Text;

namespace MKW.Testing.Common
{
    public class SandBoxBase : IDisposable
    {
        public string DatabasePath { get; }

        public SandBoxBase()
        {
            DatabasePath = Path.GetTempFileName();
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

            string stderr = SandBoxBase.TrimString(process.StandardError.ReadToEnd());
            string stdout = SandBoxBase.TrimString(process.StandardOutput.ReadToEnd());

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

        private static string TrimString(string str)
        {
            return str.Trim([' ', '\n', '\r', '\t']);
        }
    }
}
