using MKW.Core.Client;
using MKW.Core.Client.Notify;
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

        public SandBox(bool init = true)
        {
            DatabasePath = Path.GetTempFileName();

            if (init)
            {
                using JSONDatabaseSession db = JSONDatabaseSession.Create(DatabasePath);
                using ClientSession client = ClientSession.Create(db, AdminSecret);
            }
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
            return JSONDatabaseSession.Open(DatabasePath, false);
        }

        public ClientSession OpenSession()
        {
            return ClientSession.Open(OpenDatabase(), true);
        }

        public AdminSession OpenAdmin(ClientSession client)
        {
            return client.OpenAdmin(AdminSecret);
        }

        public UserSession CreateUser(ClientSession client,
                                      string password,
                                      out UserInfo user,
                                      bool trusted = true)
        {
            using AdminSession admin = OpenAdmin(client);

            user = client.PromoteUser(password);

            UserSession userSession = client.OpenUser(user.Id, password);

            if (trusted)
            {
                admin.UpdateTrust(user.Id, Trust.ExplicitTrust);
                userSession.UpdateTrust(admin.Id, Trust.ExplicitTrust);
            }

            return userSession;
        }

        private string TrimString(string str)
        {
            return str.Trim([' ', '\n', '\r', '\t']);
        }
    }
}
