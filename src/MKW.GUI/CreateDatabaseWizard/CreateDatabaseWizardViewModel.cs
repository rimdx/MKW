using MKW.GUI.Images;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI.CreateDatabaseWizard
{
    public class CreateDatabaseWizardViewModel : WizardViewModel
    {
        public PasswordViewModel Password { get; }

        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly RegistryService registry;

        public CreateDatabaseWizardViewModel(MainWindowViewModel mainWindowViewModel, RegistryService registry)
            : base("Create New Database", ImageMoniker.AddDatabase)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            this.registry = registry;

            databaseDirectory = registry.GetLastDatabaseDirectory();
            databaseName = "New Database";

            Password = new PasswordViewModel();

            AddPage(new PageLocation(this));
            AddPage(new PageMasterPassword(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));
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

        public void DoCreate()
        {
            mainWindowViewModel.CreateDatabase(DatabasePath, Password.Password);
        }
    }
}
