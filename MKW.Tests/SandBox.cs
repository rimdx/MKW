using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using System.Diagnostics;
using System.Text;

namespace MKW.Tests
{
    public class SandBox : IDisposable
    {
        public string DatabasePath { get; }
        public string AdminSecret => "adminsecret123";

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

            string stderr = TrimString(process.StandardError.ReadToEnd());
            string stdout = TrimString(process.StandardOutput.ReadToEnd());

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

        public IDatabase OpenDatabase()
        {
            return JSONDatabaseSession.Open(DatabasePath, DatabaseOpenMode.OpenOrCreate);
        }

        public ClientSession OpenSession()
        {
            return ClientSession.Open(OpenDatabase(), true);
        }

        public AdminSession OpenAdmin(ClientSession client)
        {
            return client.OpenAdmin(AdminSecret);
        }

        private string TrimString(string str)
        {
            return str.Trim([' ', '\n', '\r', '\t']);
        }
    }
}
