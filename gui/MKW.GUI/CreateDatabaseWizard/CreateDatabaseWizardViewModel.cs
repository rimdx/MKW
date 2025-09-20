using MKW.Common;
using MKW.GUI.Model;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI
{
    public class CreateDatabaseWizardViewModel : WizardViewModel
    {
        public DatabaseModel? Database { get; private set; }

        private readonly RegistryService registry;
        private string password;
        private string passwordRepeat;
        private double passwordEntropy;
        private int passwordLength;
        private bool passwordMismatch;

        public CreateDatabaseWizardViewModel(RegistryService registry)
            : base("Create New Database")
        {
            this.registry = registry;
            this.password = "";
            this.passwordRepeat = "";
            
            passwordEntropy = GetPasswordEntropy(password);
            passwordLength = GetPasswordLength(password);
            passwordMismatch = GetPasswordMismatch(password, passwordRepeat);

            databaseDirectory = registry.GetLastDatabaseDirectory();
            databaseName = "New Database.mkw";

            AddPage(new CreateDatabaseLocation(this));
            AddPage(new CreateDatabaseMasterPassword(this));
            AddPage(new CreateDatabaseConfirm(this));
        }

        public string Password
        {
            get => password;
            set
            {
                if (SetProperty(ref password, value))
                {
                    PasswordEntropy = GetPasswordEntropy(password);
                    PasswordLength = GetPasswordLength(password);
                    PasswordMismatch = GetPasswordMismatch(password, passwordRepeat);
                }
            }
        }

        public string PasswordRepeat
        {
            get => passwordRepeat;
            set
            {
                if (SetProperty(ref passwordRepeat, value))
                {
                    PasswordMismatch = GetPasswordMismatch(password, passwordRepeat);
                }
            }
        }

        public bool PasswordMismatch
        {
            get => passwordMismatch;
            private set => SetProperty(ref passwordMismatch, value);
        }

        public double PasswordEntropy
        {
            get => passwordEntropy;
            private set => SetProperty(ref passwordEntropy, value);
        }

        public int PasswordLength
        {
            get => passwordLength;
            private set => SetProperty(ref passwordLength, value);
        }

        public string DatabasePath => Path.Combine(DatabaseDirectory, DatabaseName);

        private string databaseName;
        public string DatabaseName
        {
            get => databaseName;
            set => SetProperty(ref databaseName, value);
        }

        private string databaseDirectory;

        public string DatabaseDirectory
        {
            get => databaseDirectory;
            set
            {
                if (SetProperty(ref databaseDirectory, value))
                {
                    DirectoryInfo info = new DirectoryInfo(value);

                    if (info.Exists)
                    {
                        registry.SetLastDatabaseDirectory(info.FullName);
                    }
                }
            }
        }

        public bool Exists()
        {
            return File.Exists(DatabasePath);
        }

        public override bool Finish()
        {
            Database = DatabaseModel.Create(DatabasePath, Password);

            return true;
        }

        private static int GetPasswordLength(string password)
        {
            return password.Length;
        }

        private static double GetPasswordEntropy(string password)
        {
            return PasswordUtils.MeasurePasswordEntropy(password);
        }

        private static bool GetPasswordMismatch(string password, string passwordRepeat)
        {
            return password != passwordRepeat;
        }

    }
}
