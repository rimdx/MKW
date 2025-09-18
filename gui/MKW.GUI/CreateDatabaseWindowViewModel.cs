using MKW.GUI.Model;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI
{
    public class CreateDatabaseWindowViewModel : WizardViewModel
    {
        public DatabaseModel? Database { get; private set; }

        public CreateDatabaseWindowViewModel()
        {
            // TODO: factor out
            // TODO: save latest directory to registry storage

            databaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            databaseName = "New Database.mkw";

            AddPage(new CreateDatabaseLocation(this));
            AddPage(new CreateDatabaseMasterPassword(this));
        }

        public string Password { get; set; }
        public bool IsPasswordMatch { get; set; }

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
            set => SetProperty(ref databaseDirectory, value);
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
