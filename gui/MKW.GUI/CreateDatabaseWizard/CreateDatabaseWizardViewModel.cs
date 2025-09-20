using MKW.Common;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI
{
    public class CreateDatabaseWizardViewModel : WizardViewModel
    {
        public DatabaseModel? Database { get; private set; }
        public PasswordViewModel Password { get; }
        private readonly RegistryService registry;

        public CreateDatabaseWizardViewModel(RegistryService registry)
            : base("Create New Database", ImageMoniker.AddDatabase)
        {
            this.registry = registry;

            databaseDirectory = registry.GetLastDatabaseDirectory();
            databaseName = "New Database";

            Password = new PasswordViewModel();

            AddPage(new CreateDatabaseLocation(this));
            AddPage(new CreateDatabaseMasterPassword(this));
            AddPage(new CreateDatabaseConfirm(this));
        }

        public string DatabasePath
        {
            get
            {
                string databaseFilename;

                if (string.Equals(Path.GetExtension(databaseName), ".mkw", StringComparison.InvariantCultureIgnoreCase))
                {
                    databaseFilename = databaseName;
                }
                else
                {
                    databaseFilename = databaseName + ".mkw";
                }

                return Path.Combine(DatabaseDirectory, databaseFilename);
            }
        }

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
            Database = DatabaseModel.Create(DatabasePath, Password.Password);

            return true;
        }
    }
}
