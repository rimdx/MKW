using MKW.GUI.Model;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI
{
    public class CreateDatabaseWindowViewModel : WizardViewModel
    {
        public DatabaseModel? Database { get; private set; }

        private readonly RegistryService registry;

        public CreateDatabaseWindowViewModel(RegistryService registry)
            : base("Create New Database")
        {
            this.registry = registry;

            databaseDirectory = registry.GetLastDatabaseDirectory();
            databaseName = "New Database.mkw";

            AddPage(new CreateDatabaseLocation(this));
            AddPage(new CreateDatabaseMasterPassword(this));
            AddPage(new CreateDatabaseConfirm(this));
        }

        public string Password { get; set; } = "";
        public bool IsPasswordMatch { get; set; } = true;

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
            if (!IsPasswordMatch)
            {
                throw new Exception("Password and repeated password don't match.");
            }

            if (Password == null)
            {
                throw new ArgumentNullException(nameof(Password));
            }

            Database = DatabaseModel.Create(DatabasePath, Password);

            return true;
        }
    }
}
